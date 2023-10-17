using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RushEnemyStateMachine : MonoBehaviour
{
    [Header("공격을 시전할 사거리")]
    public float moveRadius = 16f;
    public float rushRadius = 5f;
    public bool isRoll = false;

    public string tagToIgnore;

    private Mover _mover;
    private Dasher _dasher;
    private Animator _animator;

    enum State
    {
        Idle,
        Move,
        Dash,
    }

    private State state = State.Idle;

    private GameObject target = null;

    //IDLE<->MOVE->RUSH->IDLE

    private void Start()
    {
        _mover = GetComponent<Mover>();
        _dasher = GetComponent<Dasher>();
        _animator = GetComponent<Animator>();
        SetState(State.Idle);
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.Idle:
                UpdateIdle();
                break;
            case State.Move:
                UpdateMove();
                break;
            case State.Dash:
                break;
            default:
                break;
        }

        if (isRoll)
        {
            if (target != null)
            {
                Vector3 v = target.transform.position - transform.position;
                v.y = 0f;
                _dasher.direction = Vector3.Slerp(_dasher.direction, v.normalized, 0.1f);
            }
        }
    }


    void SetState(State next)
    {
        //state 변경
        _animator.SetBool(state.ToString(), false);
        state = next;
        //애니메이터의 새로운 변수 true
        _animator.SetBool(state.ToString(), true);
        //Debug.Log($"State: {state.ToString()}");
        //Rush는 코루틴으로 진행되므로 Coroutine 실행
        if (next == State.Dash) StartCoroutine(RushCoroutine());

    }

    GameObject FindPlayerInRadius(float radius)
    {
        //플레이어 탐색
        //플레이어가 원 안에 들어오면 Idle 멈추고 다른 State로 넘어간다.
        Collider[] cols = Physics.OverlapSphere(transform.position, radius, (1 << LayerMask.NameToLayer("Player")));
        int i = 0;
        for (i = 0; i < cols.Length; i++)
        {
            if (!cols[i].CompareTag(tagToIgnore))
            {
                return cols[i].gameObject;
            }
        }
        return null;
    }

    void UpdateIdle()
    {
        target = FindPlayerInRadius(moveRadius);
        if (target != null)
        {
            //플레이어 이동 State로 변경
            SetState(State.Move);
        }

    }

    void UpdateMove()
    {
        //플레이어 탐색
        //플레이어가 돌진 사거리 안에 들어오면 돌진으로 넘어간다.
        GameObject rushTo = FindPlayerInRadius(rushRadius);
        if (rushTo != null && !_dasher.isCooldown)
        {
            //돌진 가능
            _mover.StopMove();

            SetState(State.Dash);
            return;
        }

        //만약 돌진 사거리 내에 없을 경우, 이동 사거리 내에 플레이어가 있는지 확인
        target = FindPlayerInRadius(moveRadius);
        if (target == null)
        {
            _mover.StopMove();
            SetState(State.Idle);
            return;
        }

        //플레이어를 향해 움직인다.
        Vector3 v = target.transform.position - transform.position;
        v.y = 0f;
        _mover.Move(v.normalized);
    }

    IEnumerator RushCoroutine()
    {
        //대쉬 준비, 대쉬, 스턴 현재 하나로 묶여있음
        //만약 필요하다면 Dasher에 Event를 제작할것.

        _dasher.Dash(_mover.lastDirection);
        while (_dasher.IsDashing)
        {
            yield return null;
        }
        //STUN까지 끝남, IDLE로 상태 전환
        SetState(State.Idle);
    }

}
