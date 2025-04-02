#pragma warning disable 612,618
#if UNITY_2022_2_OR_NEWER
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/*
 * @Cyanilux https://github.com/Cyanilux/URP_BlitRenderFeature
*/
namespace Edgar.Unity
{
    public class FogOfWarURPFeature : ScriptableRendererFeature
    {
        internal class BlitPass : ScriptableRenderPass
        {

            private BlitSettings settings;

            private RTHandle source;
            private RTHandle destination;
            private RTHandle temp;

            //private RTHandle srcTextureId;
            private RTHandle srcTextureObject;
            private RTHandle dstTextureId;
            private RTHandle dstTextureObject;

            private string m_ProfilerTag;

            public BlitPass(RenderPassEvent renderPassEvent, BlitSettings settings, string tag)
            {
                this.renderPassEvent = renderPassEvent;
                this.settings = settings;
                m_ProfilerTag = tag;
                if (settings.srcType == Target.RenderTextureObject && settings.srcTextureObject)
                    srcTextureObject = RTHandles.Alloc(settings.srcTextureObject);
                if (settings.dstType == Target.RenderTextureObject && settings.dstTextureObject)
                    dstTextureObject = RTHandles.Alloc(settings.dstTextureObject);
            }

            public void Setup(ScriptableRenderer renderer)
            {
                if (settings.requireDepthNormals)
                    ConfigureInput(ScriptableRenderPassInput.Normal);
            }

            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
            {
                var desc = renderingData.cameraData.cameraTargetDescriptor;
                desc.depthBufferBits = 0; // Color and depth cannot be combined in RTHandles

                //RenderingUtils.ReAllocateIfNeeded(ref temp, Vector2.one, desc, name: "_TemporaryColorTexture");
                // These resizable RTHandles seem quite glitchy when switching between game and scene view :\
                // instead,
                RenderingUtils.ReAllocateIfNeeded(ref temp, desc, name: "_TemporaryColorTexture");

                var renderer = renderingData.cameraData.renderer;
                if (settings.srcType == Target.CameraColor)
                {
                    source = renderer.cameraColorTargetHandle;
                }
                else if (settings.srcType == Target.TextureID)
                {
                    //RenderingUtils.ReAllocateIfNeeded(ref srcTextureId, Vector2.one, desc, name: settings.srcTextureId);
                    //source = srcTextureId;
                    /*
                    Doesn't seem to be a good way to get an existing target with this new RTHandle system.
                    The above would work but means I'd need fields to set the desc too, which is just messy. If they don't match completely we get a new target
                    Previously we could use a RenderTargetIdentifier... but the Blitter class doesn't have support for those in 2022.1 -_-
                    Instead, I guess we'll have to rely on the shader sampling the global textureID
                    */
                    source = temp;
                }
                else if (settings.srcType == Target.RenderTextureObject)
                {
                    source = srcTextureObject;
                }

                if (settings.dstType == Target.CameraColor)
                {
                    destination = renderer.cameraColorTargetHandle;
                }
                else if (settings.dstType == Target.TextureID)
                {
                    desc.graphicsFormat = settings.graphicsFormat;
                    RenderingUtils.ReAllocateIfNeeded(ref dstTextureId, Vector2.one, desc, name: settings.dstTextureId);
                    destination = dstTextureId;
                }
                else if (settings.dstType == Target.RenderTextureObject)
                {
                    destination = dstTextureObject;
                }
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (renderingData.cameraData.cameraType == CameraType.Preview) return;

                CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
                if (settings.setInverseViewMatrix)
                {
                    cmd.SetGlobalMatrix("_InverseView", renderingData.cameraData.camera.cameraToWorldMatrix);
                }

                //Debug.Log("blit : src = " + source.name + ", dst = " + destination.name);
                if (source == destination)
                {
                    Blitter.BlitCameraTexture(cmd, source, temp, settings.blitMaterial, settings.blitMaterialPassIndex);
                    Blitter.BlitCameraTexture(cmd, temp, destination, Vector2.one);
                }
                else
                {
                    Blitter.BlitCameraTexture(cmd, source, destination, settings.blitMaterial,
                        settings.blitMaterialPassIndex);
                }

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            public override void OnCameraCleanup(CommandBuffer cmd)
            {
                source = null;
                destination = null;
            }

            public void Dispose()
            {
                temp?.Release();
                dstTextureId?.Release();
            }
        }

        [System.Serializable]
        public class BlitSettings
        {
            public RenderPassEvent Event = RenderPassEvent.BeforeRenderingPostProcessing;

            [HideInInspector] public Material blitMaterial = null;
            [HideInInspector] public int blitMaterialPassIndex = 0;
            [HideInInspector] public bool setInverseViewMatrix = false;
            [HideInInspector] public bool requireDepthNormals = false;

            [HideInInspector] public Target srcType = Target.CameraColor;

            //public string srcTextureId = "_CameraColorTexture";
            [HideInInspector] public RenderTexture srcTextureObject;

            [HideInInspector] public Target dstType = Target.CameraColor;
            [HideInInspector] public string dstTextureId = "_BlitPassTexture";
            [HideInInspector] public RenderTexture dstTextureObject;


            [HideInInspector] public bool overrideGraphicsFormat = false;
            [HideInInspector] public UnityEngine.Experimental.Rendering.GraphicsFormat graphicsFormat;
        }

        public enum Target
        {
            CameraColor,
            TextureID,
            RenderTextureObject
        }

        public BlitSettings settings = new BlitSettings();
        private BlitPass blitPass;

        public override void Create()
        {
            var passIndex = settings.blitMaterial != null ? settings.blitMaterial.passCount - 1 : 1;
            settings.blitMaterialPassIndex = Mathf.Clamp(settings.blitMaterialPassIndex, -1, passIndex);
            blitPass = new BlitPass(settings.Event, settings, name);

            if (settings.graphicsFormat == UnityEngine.Experimental.Rendering.GraphicsFormat.None)
            {
                settings.graphicsFormat =
                    SystemInfo.GetGraphicsFormat(UnityEngine.Experimental.Rendering.DefaultFormat.LDR);
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (IsFogOfWarEnabled(renderingData))
            {
                settings.blitMaterial = FogOfWarGrid2D.Instance.GetMaterial(renderingData.cameraData.camera, "Edgar/FogOfWarURP");
            }

            if (settings.blitMaterial == null)
            {
                return;
            }

            renderer.EnqueuePass(blitPass);
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            blitPass.Setup(renderer);
        }

        protected override void Dispose(bool disposing)
        {
            blitPass.Dispose();
        }

        private bool IsFogOfWarEnabled(in RenderingData renderingData)
        {
            // Do not execute the effect in Editor
            if (!Application.isPlaying)
            {
                return false;
            }

            // Do not execute the effect if there is no instance of the script
            if (FogOfWarGrid2D.Instance == null)
            {
                return false;
            }

            // Do not execute the effect if the FogOfWar instance is disabled
            if (!FogOfWarGrid2D.Instance.enabled)
            {
                return false;
            }

            // Do not execute the effect if the current camera does not have the FogOfWar component attached
            if (renderingData.cameraData.camera.GetComponent<FogOfWarGrid2D>() == null &&
                renderingData.cameraData.camera.GetComponent<FogOfWarAdditionalCameraGrid2D>() == null)
            {
                return false;
            }

            return true;
        }
    }
}
#else
/*
 * Adapted for the Fog of War feature in SRP
 * ------------------------------------------------------------------------------------------------------------------------
 * Blit Renderer Feature                                                https://github.com/Cyanilux/URP_BlitRenderFeature
 * ------------------------------------------------------------------------------------------------------------------------
 * Based on the Blit from the UniversalRenderingExamples
 * https://github.com/Unity-Technologies/UniversalRenderingExamples/tree/master/Assets/Scripts/Runtime/RenderPasses
 * 
 * Extended to allow for :
 * - Specific access to selecting a source and destination (via current camera's color / texture id / render texture object
 * - Automatic switching to using _AfterPostProcessTexture for After Rendering event, in order to correctly handle the blit after post processing is applied
 * - Setting a _InverseView matrix (cameraToWorldMatrix), for shaders that might need it to handle calculations from screen space to world.
 *     e.g. reconstruct world pos from depth : https://twitter.com/Cyanilux/status/1269353975058501636 
 * ------------------------------------------------------------------------------------------------------------------------
 * @Cyanilux
*/
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Edgar.Unity
{
    /// <summary>
    /// Scriptable renderer feature that has to be enabled to make the Fog of War work in URP.
    /// </summary>
    public class FogOfWarURPFeature : ScriptableRendererFeature
    {

        internal class BlitPass : ScriptableRenderPass
        {

            public Material blitMaterial = null;
            public FilterMode filterMode { get; set; }

            private BlitSettings settings;

            private RenderTargetIdentifier source { get; set; }
            private RenderTargetIdentifier destination { get; set; }

            RenderTargetHandle m_TemporaryColorTexture;
            RenderTargetHandle m_DestinationTexture;
            string m_ProfilerTag;

            public BlitPass(RenderPassEvent renderPassEvent, BlitSettings settings, string tag)
            {
                this.renderPassEvent = renderPassEvent;
                this.settings = settings;
                blitMaterial = settings.blitMaterial;
                m_ProfilerTag = tag;
                m_TemporaryColorTexture.Init("_TemporaryColorTexture");
                if (settings.dstType == Target.TextureID)
                {
                    m_DestinationTexture.Init(settings.dstTextureId);
                }
            }

            public void Setup(RenderTargetIdentifier source, RenderTargetIdentifier destination)
            {
                this.source = source;
                this.destination = destination;
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);

                RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
                opaqueDesc.depthBufferBits = 0;

                if (settings.setInverseViewMatrix)
                {
                    Shader.SetGlobalMatrix("_InverseView", renderingData.cameraData.camera.cameraToWorldMatrix);
                }

                if (settings.dstType == Target.TextureID)
                {
                    cmd.GetTemporaryRT(m_DestinationTexture.id, opaqueDesc, filterMode);
                }
                
                //Debug.Log($"src = {source},     dst = {destination} ");
                // Can't read and write to same color target, use a TemporaryRT
                if (source == destination || (settings.srcType == settings.dstType && settings.srcType == Target.CameraColor))
                {
                    cmd.GetTemporaryRT(m_TemporaryColorTexture.id, opaqueDesc, filterMode);
                    Blit(cmd, source, m_TemporaryColorTexture.Identifier(), blitMaterial, settings.blitMaterialPassIndex);
                    Blit(cmd, m_TemporaryColorTexture.Identifier(), destination);
                }
                else
                {
                    Blit(cmd, source, destination, blitMaterial, settings.blitMaterialPassIndex);
                }

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            public override void FrameCleanup(CommandBuffer cmd)
            {
                if (settings.dstType == Target.TextureID)
                {
                    cmd.ReleaseTemporaryRT(m_DestinationTexture.id);
                }
                if (source == destination || (settings.srcType == settings.dstType && settings.srcType == Target.CameraColor))
                {
                    cmd.ReleaseTemporaryRT(m_TemporaryColorTexture.id);
                }
            }
        }

        [System.Serializable]
        internal class BlitSettings
        {

            public RenderPassEvent Event = RenderPassEvent.BeforeRenderingPostProcessing;

            [HideInInspector]
            public Material blitMaterial = null;

            [HideInInspector]
            public int blitMaterialPassIndex = 0;

            [HideInInspector]
            public bool setInverseViewMatrix = false;

            [HideInInspector]
            public Target srcType = Target.CameraColor;

            [HideInInspector]
            public string srcTextureId = "_CameraColorTexture";

            #pragma warning disable 0649

            [HideInInspector]
            public RenderTexture srcTextureObject;

            #pragma warning restore 0649

            [HideInInspector]
            public Target dstType = Target.CameraColor;

            [HideInInspector]
            public string dstTextureId = "_BlitPassTexture";

            #pragma warning disable 0649

            [HideInInspector]
            public RenderTexture dstTextureObject;

            #pragma warning restore 0649
        }

        internal enum Target
        {
            CameraColor,
            TextureID,
            RenderTextureObject
        }

        [SerializeField]
        internal BlitSettings settings = new BlitSettings();

        BlitPass blitPass;

        private RenderTargetIdentifier srcIdentifier, dstIdentifier;

        public override void Create()
        {
            var passIndex = settings.blitMaterial != null ? settings.blitMaterial.passCount - 1 : 1;
            settings.blitMaterialPassIndex = Mathf.Clamp(settings.blitMaterialPassIndex, -1, passIndex);
            blitPass = new BlitPass(settings.Event, settings, name);

            if (settings.Event == RenderPassEvent.AfterRenderingPostProcessing)
            {
                Debug.LogWarning("Note that the \"After Rendering Post Processing\"'s Color target doesn't seem to work? (or might work, but doesn't contain the post processing) :( -- Use \"After Rendering\" instead!");
            }

            UpdateSrcIdentifier();
            UpdateDstIdentifier();
        }

        private void UpdateSrcIdentifier()
        {
            srcIdentifier = UpdateIdentifier(settings.srcType, settings.srcTextureId, settings.srcTextureObject);
        }

        private void UpdateDstIdentifier()
        {
            dstIdentifier = UpdateIdentifier(settings.dstType, settings.dstTextureId, settings.dstTextureObject);
        }

        private RenderTargetIdentifier UpdateIdentifier(Target type, string s, RenderTexture obj)
        {
            if (type == Target.RenderTextureObject)
            {
                return obj;
            }
            else if (type == Target.TextureID)
            {
                //RenderTargetHandle m_RTHandle = new RenderTargetHandle();
                //m_RTHandle.Init(s);
                //return m_RTHandle.Identifier();
                return s;
            }
            return new RenderTargetIdentifier();
        }

        private Material material;

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (IsFogOfWarEnabled(renderingData))
            {
                material = FogOfWarGrid2D.Instance.GetMaterial(renderingData.cameraData.camera);

                if (material == null)
                {
                    return;
                }
                
                #if !EDGAR_URP_13_OR_NEWER
                ApplyFogOfWar(renderer, renderingData);
                #endif
                
                renderer.EnqueuePass(blitPass);
            }
        }
        
        #if EDGAR_URP_13_OR_NEWER
        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            if (IsFogOfWarEnabled(renderingData))
            {
                ApplyFogOfWar(renderer, renderingData);
            }
        }
        #endif

        private bool IsFogOfWarEnabled(in RenderingData renderingData)
        {
            // Do not execute the effect in Editor
            if (!Application.isPlaying)
            {
                return false;
            }

            // Do not execute the effect if there is no instance of the script
            if (FogOfWarGrid2D.Instance == null)
            {
                return false;
            }

            // Do not execute the effect if the FogOfWar instance is disabled
            if (!FogOfWarGrid2D.Instance.enabled)
            {
                return false;
            }

            // Do not execute the effect if the current camera does not have the FogOfWar component attached
            if (renderingData.cameraData.camera.GetComponent<FogOfWarGrid2D>() == null && renderingData.cameraData.camera.GetComponent<FogOfWarAdditionalCameraGrid2D>() == null)
            {
                return false;
            }

            return true;
        }

        private void ApplyFogOfWar(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            if (material == null)
            {
                return;
            }
            
            blitPass.blitMaterial = material;

            if (settings.Event == RenderPassEvent.AfterRenderingPostProcessing)
            {
            }
            // Comment for LWRP
            else if (settings.Event == RenderPassEvent.AfterRendering && renderingData.postProcessingEnabled)
            {
                // If event is AfterRendering, and src/dst is using CameraColor, switch to _AfterPostProcessTexture instead.
                if (settings.srcType == Target.CameraColor)
                {
                    settings.srcType = Target.TextureID;
                    settings.srcTextureId = "_AfterPostProcessTexture";
                    UpdateSrcIdentifier();
                }
                if (settings.dstType == Target.CameraColor)
                {
                    settings.dstType = Target.TextureID;
                    settings.dstTextureId = "_AfterPostProcessTexture";
                    UpdateDstIdentifier();
                }
            }
            else
            {
                // If src/dst is using _AfterPostProcessTexture, switch back to CameraColor
                if (settings.srcType == Target.TextureID && settings.srcTextureId == "_AfterPostProcessTexture")
                {
                    settings.srcType = Target.CameraColor;
                    settings.srcTextureId = "";
                    UpdateSrcIdentifier();
                }
                if (settings.dstType == Target.TextureID && settings.dstTextureId == "_AfterPostProcessTexture")
                {
                    settings.dstType = Target.CameraColor;
                    settings.dstTextureId = "";
                    UpdateDstIdentifier();
                }
            }

            var src = (settings.srcType == Target.CameraColor) ? renderer.cameraColorTarget : srcIdentifier;
            var dest = (settings.dstType == Target.CameraColor) ? renderer.cameraColorTarget : dstIdentifier;

            blitPass.Setup(src, dest);
        }
    }
}
#endif