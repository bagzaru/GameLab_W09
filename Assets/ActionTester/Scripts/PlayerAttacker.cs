using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class PlayerAttacker : MonoBehaviour
{
    private Animator _animator;
    Rigidbody body;
    private PlayerMover _mover;

    private int lastAttackCount = 0;
    //공격 모션 지속 시간
    private float[] attackDurations = {0.4f, 0.4f, 0.8f};
    private float currentDuration = 0f;

    //공격 후 딜레이
    private float attackDelay = 1.5f;
    private float currentDelay = 0f;
    
    //이동 거리
    public float moveAmount = 4f;
    public float moveTime = 0.2f;

    public int maxAttackCount = 3;

    public bool isAttacking { get; private set; } = false;
    private bool attackKeyPressedWhileAttakcing = false;

    public List<GameObject> slashEffects;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody>();
        _mover = GetComponent<PlayerMover>();

        foreach (var t in slashEffects)
        {
            t.SetActive(false);
        }
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
        //지속 시간 초기화
        currentDuration = attackDurations[lastAttackCount];
        //공격 중이라는 것을 true로 변경
        isAttacking = true;
        //공격 애니메이션 재생
        _animator.SetBool($"Attack{lastAttackCount}", true);
        //공격 이펙트 킨다.
        slashEffects[lastAttackCount].SetActive(true);
        //앞으로 약간 대쉬
        StartCoroutine(DashCoroutine(_mover.input, moveAmount, moveAmount/moveTime));
    }

    private IEnumerator DashCoroutine(Vector3 dir, float dist, float speed)
    {
        float counter = 0f;
        dir = dir.normalized;
        _mover.SetRotation(dir);
        while (counter < dist)
        {
            float fixedMoveAmount = speed*Time.fixedDeltaTime;
            
            if (counter + fixedMoveAmount >= dist)
            {
                fixedMoveAmount -= dist - counter;
                body.MovePosition(body.position + dir*fixedMoveAmount);
                yield break;
            }
            else
            {
                body.MovePosition(body.position + dir*fixedMoveAmount);
                yield return new WaitForFixedUpdate();
            }
            counter += fixedMoveAmount;
        }
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
            for (int i = 0; i < maxAttackCount; i++)
            {
                _animator.SetBool($"Attack{i}", false);
                slashEffects[i].SetActive(false);
            }
            lastAttackCount = 0;
            currentDelay = attackDelay;
            
        }
    }


}
