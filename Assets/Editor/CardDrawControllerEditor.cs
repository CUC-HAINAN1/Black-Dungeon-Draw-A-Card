using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardDrawController))]
public class CardDrawControllerEditor : Editor
{
    public override void OnInspectorGUI() {
    
        base.OnInspectorGUI();
        
        CardDrawController controller = (CardDrawController)target;
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("当前卡牌信息", EditorStyles.boldLabel);
        
        for (int i = 0; i < controller.queueSystem.currentCards.Length; i++) {
        
            EditorGUILayout.BeginVertical("Box");
            
            var cardInfo = controller.queueSystem.currentCards[i];
            
            string status = cardInfo.cardData != null ? 
                $"Slot {i}: {cardInfo.cardData.displayName}" : 
                $"Slot {i}: 空";
            
            EditorGUILayout.LabelField(status);
            
            if (cardInfo.cardData != null) {
            
                EditorGUI.indentLevel++;
                EditorGUILayout.ObjectField("卡牌数据", cardInfo.cardData, typeof(CardDataBase), false);
                EditorGUILayout.ObjectField("实例", cardInfo.cardInstance, typeof(GameObject), true);
                EditorGUI.indentLevel--;
            
            }
            
            EditorGUILayout.EndVertical();
        
        }
    
    }

}