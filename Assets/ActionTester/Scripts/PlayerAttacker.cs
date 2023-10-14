using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttacker : MonoBehaviour
{
    private Animator _animator;

    private int lastAttackCount = 0;
    //공격 모션 지속 시간
    private float[] attackDurations = {0.8f, 0.8f, 1.7f};
    private float currentDuration = 0f;

    //공격 후 딜레이
    private float attackDelay = 1.5f;
    private float currentDelay = 0f;

    public int maxAttackCount = 3;

    public bool isAttacking { get; private set; } = false;
    private bool attackKeyPressedWhileAttakcing = false;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (currentDuration > 0f)
        {
            currentDuration -= Time.deltaTime;
            if (currentDuration <= 0f)
            {
                currentDuration = 0f;
                OnAttackEnded();
            }
        }
        if (currentDelay > 0f)
        {
            currentDelay -= Time.deltaTime;
            if (currentDelay <= 0f)
            {
                currentDelay = 0f;
            }
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Attack();
        }

    }

    public void Attack()
    {
        if (currentDelay > 0f)
        {
            return;
        }
        if (currentDuration > 0f)
        {
            //공격 모션이 진행중이므로, 공격 모션 중 공격 키가 눌림.
            attackKeyPressedWhileAttakcing = true;

            return;
        }

        //Debug.Log("렛츠어택!");
        currentDuration = attackDurations[lastAttackCount];
        isAttacking = true;
        _animator.SetBool($"Attack{lastAttackCount}", true);
        Debug.Log($"Attack{lastAttackCount}");

        //_animator.SetBool($"Attack{lastAttackCount}", false);
        // if (lastAttackCount >= 2)
        // {
        // 	lastAttackCount = 0; 
        // }
        // else
        // {
        // 	lastAttackCount += 1;
        // }

        //_animator.SetBool($"Attack{lastAttackCount}", true);
    }

    public void OnAttackEnded()
    {
        isAttacking = false;

        lastAttackCount += 1;
        if (attackKeyPressedWhileAttakcing
            &&lastAttackCount<maxAttackCount)
        {
            //공격 모션 중에 공격 키가 눌렸고, 다음 공격 콤보가 남아있다면 다음 공격 실행
            attackKeyPressedWhileAttakcing = false;
            Attack();
        }
        else
        {
            //더이상 공격이 없다면, 애니메이션을 초기화하고, 딜레이를 설정한다.
            for (int i = 0; i <= lastAttackCount; i++)
            {
                _animator.SetBool($"Attack{i}", false);
            }
            lastAttackCount = 0;
            currentDelay = attackDelay;
            
        }
    }


}
