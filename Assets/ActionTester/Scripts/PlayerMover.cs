using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMover : MonoBehaviour
{
    public float speed = 3f;
    public bool canMove = true;

    [Header("대쉬 프로퍼티")]
    public bool isDashing = false;
    public bool canDash = true;
    public float dashTime = 0.2f;
    private float dashSpeed = 0f;
    public float dashDistance = 10f;
    private Vector3 dashDirection = Vector3.forward;
    
    //대쉬 쿨타임
    public bool isDashCoolTime = false;
    public float dashCoolTime = 0.8f;
    private float dashCoolTimer = 0f;

    public Vector3 input = Vector3.zero;
    public Vector3 lastInput = Vector3.down;
    private Animator animator;
    private Rigidbody body;
    private PlayerAttacker _attacker;
    private static readonly int animMoveCode = Animator.StringToHash("Move");

    private void Start()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody>();
        _attacker = GetComponent<PlayerAttacker>();
    }
    

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 tempInput = context.ReadValue<Vector2>();
        input = new Vector3(tempInput.x, 0f, tempInput.y);
        if (tempInput != Vector2.zero)
        {
            lastInput = new Vector3(tempInput.x, 0f, tempInput.y);
            animator.SetBool(animMoveCode, true);
        }
        else
        {
            animator.SetBool(animMoveCode,false);
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            dashSpeed = dashDistance/dashTime;
            dashDirection = lastInput;
            isDashing = true;
            animator.SetBool("Dash",true);
        }
    }

    
    public float SetRotation(Vector3 dir)
    {
        Vector3 a = Vector3.forward;
        Vector3 b = dir.normalized;
        float angle = Vector3.Angle(a, b);
        float t = (( b.x * a.z - a.x * b.z ) >= 0) ? 1f : -1f;
        transform.rotation = Quaternion.Euler(0f, angle * t,0f);
        return angle * t;
    }

    private void Update()
    {
        if (isDashCoolTime)
        {
            dashCoolTimer += Time.deltaTime;
            if (dashCoolTimer >= dashCoolTime)
            {
                isDashCoolTime = false;
                dashCoolTimer = 0f;
            }
        }
    }


    private void FixedUpdate()
    {
        if (isDashing)
        {
            FixedUpdateDash();
        }
        if (canMove&&!isDashing&&!_attacker.isAttacking)
        {
            FixedUpdateMovement();
        }
    }

    private void OnDashEnd()
    {
        animator.SetBool("Dash",false);
        isDashCoolTime = true;
    }

    private void FixedUpdateMovement()
    {
        //body.velocity = speed* input.normalized;
        body.MovePosition(body.position + input.normalized*(speed*Time.fixedDeltaTime));
        SetRotation(lastInput);
    }

    private float dashTimer = 0f;
    private void FixedUpdateDash()
    {
        dashTimer += Time.fixedDeltaTime;
        if (dashTimer >= dashTime)
        {
            dashTimer = 0f;
            isDashing = false;
            OnDashEnd();
        }
        
        body.MovePosition(body.position + dashDirection*(dashSpeed*Time.fixedDeltaTime));
        //body.velocity = dashSpeed* dashDirection;
        SetRotation(dashDirection);

    }
    
}
