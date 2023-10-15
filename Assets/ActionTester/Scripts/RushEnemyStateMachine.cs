using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RushEnemyStateMachine : MonoBehaviour
{
    public float moveRadius = 16f;
    public float rushRadius = 5f;

    public float rushCastingTime = 0.4f;
    public float stunTime = 1f;
    public float rushDelay = 2f;

    private bool _isRushDelaying = false;

    private Mover _mover;
    private Dasher _dasher;

    enum State
    {
        IDLE,
        MOVE,
        RUSH,
    }

    private State state = State.IDLE;

    private GameObject target = null;

    //IDLE<->MOVE->RUSH->IDLE

    private void Start()
    {
        _mover = GetComponent<Mover>();
        _dasher = GetComponent<Dasher>();
        SetState(State.IDLE);
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.IDLE:
                UpdateIdle();
                break;
            case State.MOVE:
                UpdateMove();
                break;
            case State.RUSH:
                break;
            default:
                break;
        }
    }


    void SetState(State next)
    {
        //애니메이터의 기존 변수 false
        //state 변경
        state = next;
        //애니메이터의 새로운 변수 true

        //Rush는 코루틴으로 진행되므로 Coroutine 실행
        if (next == State.RUSH)StartCoroutine(RushCoroutine());
        
    }

    GameObject FindPlayerInRadius(float radius)
    {
        //플레이어 탐색
        //플레이어가 원 안에 들어오면 Idle 멈추고 다른 State로 넘어간다.
        Collider[] cols = Physics.OverlapSphere(transform.position, radius);
        int i = 0;
        for (i = 0; i < cols.Length; i++)
        {
            if (cols[i].CompareTag("Player"))
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
            SetState(State.MOVE);
        }

    }

    void UpdateMove()
    {
        //플레이어 탐색
        //플레이어가 돌진 사거리 안에 들어오면 돌진으로 넘어간다.
        GameObject rushTo = FindPlayerInRadius(rushRadius);
        if (rushTo != null && !_isRushDelaying)
        {
            //돌진 가능
            _mover.StopMove();
            
            SetState(State.RUSH);
            return;
        }

        //만약 돌진 사거리 내에 없을 경우, 이동 사거리 내에 플레이어가 있는지 확인
        target = FindPlayerInRadius(moveRadius);
        if (target == null)
        {
            _mover.StopMove();
            SetState(State.IDLE);
            return;
        }
        
        //플레이어를 향해 움직인다.
        Vector3 v = target.transform.position - transform.position;
        v.y = 0f;
        _mover.Move(v.normalized);
    }

    IEnumerator RushCoroutine()
    {
        _isRushDelaying = true;
        //대쉬 준비 애니메이션(있을 경우)

        yield return new WaitForSeconds(rushCastingTime);
        //대쉬
        Vector3 v = target.transform.position - transform.position;
        v.y = 0f;
        _dasher.Dash(v.normalized);
        while (_dasher.IsDashing)
        {
            yield return null;
        }
        
        //대쉬 종료, STUN 애니메이션(있을 경우)
        
        yield return new WaitForSeconds(stunTime);
        //STUN까지 끝남, IDLE로 상태 전환
        SetState(State.IDLE);
        yield return new WaitForSeconds(rushDelay);
        _isRushDelaying = false;
    }

}
