using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    private Mover _mover;
    private Dasher _dasher;
    private Animator animator;
    private PlayerAttacker _attacker;
    private static readonly int animMoveCode = Animator.StringToHash("Move");

    private void Start()
    {
        animator = GetComponent<Animator>();
        _attacker = GetComponent<PlayerAttacker>();
        _dasher = GetComponent<Dasher>();
        _mover = GetComponent<Mover>();
        
        
        _dasher.onCasting.AddListener(() => {animator.SetBool("Dash", true); });
        _dasher.onFinish.AddListener(() => {animator.SetBool("Dash", false); });
        _mover.onMove.AddListener(()=>{ animator.SetBool(animMoveCode,true);});
        _mover.onMoveEnd.AddListener(()=>{ animator.SetBool(animMoveCode,false);});
    }

    public bool CanSwitch()
    {

        return !(_dasher != null && _dasher.IsDashing || _attacker != null && _attacker.IsAttacking);
    }

    public bool isRoll = false;
    public void OnMove(InputAction.CallbackContext context)
    {
        if (isRoll)
        {
            Vector2 ti = context.ReadValue<Vector2>();
            _dasher.direction=(new Vector3(ti.x, 0f, ti.y));
        }
        if (_mover == null) return;
        if (_dasher!=null&&_dasher.IsDashing || _attacker!=null&&_attacker.IsAttacking)
        {
            //대쉬 중이거나 공격 중이면 움직임 금지
            _mover.StopMove();
            return;
        }
        
        Vector2 tempInput = context.ReadValue<Vector2>();
        if (tempInput != Vector2.zero)
        {
            //입력이 0이 아니다: 움직이겠다.
            _mover.Move(new Vector3(tempInput.x, 0f, tempInput.y));
        }
        else
        {
            _mover.StopMove();
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if(_dasher == null) return;
        if (context.performed)
        {
            _dasher.Dash(_mover.lastDirection);
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (_attacker == null) return;
        if (context.started)
        {
            _attacker.Attack(_mover.lastDirection);
        }

    }
    
}
