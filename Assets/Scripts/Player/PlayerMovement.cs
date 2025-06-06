using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerMovement: MonoBehaviour {

    PlayerAttributes playerAttributes;

    public event Action<Vector2> OnMovementInputChanged;

    [Header("Movement")]
    public float acceleration = 30f; //加速度
    public float maxSpeed = 8f; //最大速度
    public float drag = 5f; //手动设置的初始阻力
    public float stopDrag = 20f; //停止阻力更大，快速停止以符合肉鸽游戏操作手感
    
    [Header("Rolling")]
    public float rollDistance = 5f;
    public float rollDuration = 0.3f;
    public float rollCoolDown = 1f;
    public float rollStopTime = 0.05f;

    [Header("Death Settings")]
    public float deathDrag = 5f;
    public float deathSpeedMutiplier = 3f;
    private Vector2 lastNonZeroMovementInput;
    private bool deathIsHandled = false;


    [Header("Rolling Curve")]
    public AnimationCurve rollSpeedCurve = new AnimationCurve(
        
        new Keyframe(0, 0),
        new Keyframe(0.4f, 1),
        new Keyframe(0.6f, 1),
        new Keyframe(1, 0)

    );

    private Rigidbody2D rb;
    private Vector2 movementInput;
    private Animator animator;
    private float defaultDrag; //默认阻力
    private bool isFacingRight = true;
    private float lastRollTime = -999f;


    void Start()
    {
        
        playerAttributes = PlayerAttributes.Instance;
        
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        
        rb.drag = drag;
        defaultDrag = rb.drag; //存储初始阻力

    }

    void Update() {

        //假如死亡直接返回，翻滚时禁用常规移动
        if (!CheckAndHandleDeath() || playerAttributes.IsRolling) return;

        Vector2 newInput = new Vector2(
            
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        
        ).normalized;

        //触发输入变化事件
        if (newInput != movementInput) {
        
            movementInput = newInput;
            OnMovementInputChanged?.Invoke(movementInput);
        
        }

        //记录输入时更新最后移动方向
        if (movementInput.magnitude > 0.1f) lastNonZeroMovementInput = newInput.normalized;

        //翻滚输入检测
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time > lastRollTime + rollCoolDown && playerAttributes.Mana >= 2) {
            
            playerAttributes.UseMana(2);
            StartCoroutine(PerformRoll());
            
        }
        
        //水平移动时改变角色朝向
        if (newInput.x != 0) {

            FilpCharacter(movementInput.x > 0);

        }

    }

    void FixedUpdate() {
    
        //处于翻滚或死亡状态直接返回
        if (playerAttributes.IsRolling || playerAttributes.IsDead) return;

        if (movementInput.magnitude > 0.1f) {

            //应用默认阻力
            rb.drag = defaultDrag;
            
            //加速度模拟
            Vector2 targetVelocity = movementInput * maxSpeed;
            Vector2 velocityChange = (targetVelocity - rb.velocity) * acceleration;
            rb.AddForce(velocityChange, ForceMode2D.Force);

        }
        else {

            rb.drag = stopDrag;

        }       

    }   

    //翻滚动作
    IEnumerator PerformRoll() {

        playerAttributes.StartRolling();
        
        lastRollTime = Time.time;
        
        //计算初始转向方向
        Vector2 rollDirection = movementInput.magnitude > 0.1f ? 
                                movementInput.normalized : 
                                (isFacingRight ? Vector2.right : Vector2.left);

        //计算初始速度
        float basedSpeed = rollDistance / rollDuration;
        
        //保存物理参数
        float originalDrag = rb.drag;
        rb.drag = 0;

        //翻滚过程
        float elapsed = 0f;
        while (elapsed < rollDuration) {
            
            float progress = elapsed / rollDuration;
            float speedMutiplier = rollSpeedCurve.Evaluate(progress);

            //应用平滑曲线速度
            rb.velocity = rollDirection * basedSpeed * speedMutiplier;

            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();

        }

        //停止过程
        float stopTimer = 0f;
        Vector2 startvelocity = rb.velocity;

        while (stopTimer < rollStopTime) {
            
            float progress = stopTimer / rollStopTime;

            rb.velocity = Vector2.Lerp(startvelocity, Vector2.zero, progress);
            
            stopTimer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();

        }

        //防止多余移动
        rb.velocity = Vector2.zero;
        
        //恢复阻力参数
        rb.drag = originalDrag;
        
        //重置翻滚状态
        playerAttributes.EndRolling();

    }

    //翻转角色朝向
    private void FilpCharacter(bool faceRight) {

        if (faceRight != isFacingRight) {

            isFacingRight = faceRight;
            transform.localScale = new Vector3(faceRight ? 4 : -4, 4, 1);

        }

    }

    //死亡状态特判与处理
    private bool CheckAndHandleDeath() {

        bool IsDead = PlayerAttributes.Instance.IsDead;

        if (IsDead && !deathIsHandled) {

            OnDeathStart();
            deathIsHandled = true;

        }

        if (IsDead) return false;

        return true;

    }
    
    //处理死亡滑行
    private void OnDeathStart() {
        
        Vector2 deathDirection = lastNonZeroMovementInput;

        if (deathDirection.magnitude < 0.1f) {

            deathDirection = isFacingRight ? Vector2.right : Vector2.left;

        }

        float deathSpeed = maxSpeed * deathSpeedMutiplier;
        rb.velocity = deathDirection * deathSpeed;

        rb.drag = deathDrag;

        movementInput = Vector2.zero;
        StopAllCoroutines();
        playerAttributes.EndRolling();

    }

}
