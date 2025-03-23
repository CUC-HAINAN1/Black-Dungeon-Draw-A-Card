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
    

    private Rigidbody2D rb;
    private Vector2 movementInput;
    private Animator animator;
    private float defaultDrag; //默认阻力
    private bool isFacingRight = true;


    void Start()
    {
        
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rb.drag = drag;
        defaultDrag = rb.drag;

    }

    void Update()
    {
        
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");
        movementInput = movementInput.normalized;

        animator.SetBool("IsMoving", movementInput.magnitude > 0.1f);


        //水平移动时改变角色朝向
        if (movementInput.x != 0) {

            FilpCharacter(movementInput.x > 0);

        }

    }

    void FixedUpdate()
    {
        
        if (movementInput.magnitude > 0.1f) {

            rb.drag = defaultDrag;
            Vector2 targetVelocity = movementInput * maxSpeed;
            rb.velocity = Vector2.MoveTowards(rb.velocity, targetVelocity, acceleration * Time.fixedDeltaTime);

        }
        else {

            rb.drag = stopDrag;

        }       

    }

    //翻转角色朝向
    private void FilpCharacter(bool faceRight) {

        if (faceRight != isFacingRight) {

            isFacingRight = faceRight;
            transform.localScale = new Vector3(faceRight ? 4 : -4, 4, 1);

        }

    }

}
