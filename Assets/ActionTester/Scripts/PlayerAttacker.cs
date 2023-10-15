using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
// ReSharper disable MemberCanBePrivate.Global

public class PlayerAttacker : MonoBehaviour
{
    private Animator _animator;
    Rigidbody body;
    private Mover _mover;

    private int _lastAttackCount = 0;
    //공격 모션 지속 시간
    private float[] _attackDurations = {0.4f, 0.4f, 0.8f};
    private float _currentDuration = 0f;

    //공격 후 딜레이
    private float _attackDelay = 1.5f;
    private float _currentDelay = 0f;
    
    //이동 거리
    public float moveAmount = 4f;
    public float moveTime = 0.2f;

    public int maxAttackCount = 3;

    public bool IsAttacking { get; private set; } = false;
    private bool _attackKeyPressedWhileAttacking = false;

    public List<GameObject> slashEffects;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody>();
        _mover = GetComponent<Mover>();

        foreach (var t in slashEffects)
        {
            t.SetActive(false);
        }
    }

    private void Update()
    {
        if (_currentDuration > 0f)
        {
            _currentDuration -= Time.deltaTime;
            if (_currentDuration <= 0f)
            {
                _currentDuration = 0f;
                OnAttackEnded();
            }
        }
        if (_currentDelay > 0f)
        {
            _currentDelay -= Time.deltaTime;
            if (_currentDelay <= 0f)
            {
                _currentDelay = 0f;
            }
        }
    }

    public void Attack(Vector3 dir)
    {
        if (_currentDelay > 0f)
        {
            return;
        }
        if (_currentDuration > 0f)
        {
            //공격 모션이 진행중이므로, 공격 모션 중 공격 키가 눌림.
            _attackKeyPressedWhileAttacking = true;

            return;
        }

        //Debug.Log("렛츠어택!");
        //지속 시간 초기화
        _currentDuration = _attackDurations[_lastAttackCount];
        //공격 중이라는 것을 true로 변경
        IsAttacking = true;
        //공격 애니메이션 재생
        _animator.SetBool($"Attack{_lastAttackCount}", true);
        //공격 이펙트 킨다.
        slashEffects[_lastAttackCount].SetActive(true);
        //앞으로 약간 대쉬
        StartCoroutine(DashCoroutine(dir, moveAmount, moveAmount/moveTime));
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
        IsAttacking = false;

        _lastAttackCount += 1;
        if (_attackKeyPressedWhileAttacking
            &&_lastAttackCount<maxAttackCount)
        {
            //공격 모션 중에 공격 키가 눌렸고, 다음 공격 콤보가 남아있다면 다음 공격 실행
            _attackKeyPressedWhileAttacking = false;
            Attack(_mover.lastDirection);
        }
        else
        {
            //더이상 공격이 없다면, 애니메이션을 초기화하고, 딜레이를 설정한다.
            for (int i = 0; i < maxAttackCount; i++)
            {
                _animator.SetBool($"Attack{i}", false);
                slashEffects[i].SetActive(false);
            }
            _lastAttackCount = 0;
            _currentDelay = _attackDelay;
            
        }
    }


}
