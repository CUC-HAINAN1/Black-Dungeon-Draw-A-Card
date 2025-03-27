using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerMovement: MonoBehaviour

{
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
    private bool isRolling = false;
    private float lastRollTime = -999f;


    void Start()
    {
        
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rb.drag = drag;
        defaultDrag = rb.drag;

    }

    void Update()
    {

        if (PlayerAttributes.Instance.IsDead) {

            animator.SetBool("IsDead", true);
            return;

        }

        //翻滚时禁用常规移动
        if (isRolling) return;

        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");
        movementInput = movementInput.normalized;

        animator.SetBool("IsMoving", movementInput.magnitude > 0.1f);


        //翻滚输入检测
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time > lastRollTime + rollCoolDown) {

            StartCoroutine(PerformRoll());

        }


        //水平移动时改变角色朝向
        if (movementInput.x != 0) {

            FilpCharacter(movementInput.x > 0);

        }

    }

    void FixedUpdate()
    {
        
        if (isRolling) return;

        if (movementInput.magnitude > 0.1f) {

            rb.drag = defaultDrag;
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

        isRolling = true;
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
        isRolling = false;

    }

    //翻转角色朝向
    private void FilpCharacter(bool faceRight) {

        if (faceRight != isFacingRight) {

            isFacingRight = faceRight;
            transform.localScale = new Vector3(faceRight ? 4 : -4, 4, 1);

        }

    }

}
