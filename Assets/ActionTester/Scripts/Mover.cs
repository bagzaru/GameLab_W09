using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Mover : MonoBehaviour
{
    //움직임 bool 변수
    public bool CanMove { get; private set; } = true;
    public bool IsMoving { get; private set; } = false;
    public bool useRotation { get; private set; } = true;

    //이동 관련 변수
    public Vector3 direction = Vector3.zero;
    public Vector3 lastDirection = Vector3.forward;
    public float speed = 8f;
    
    //이벤트
    public UnityEvent onMove;
    public UnityEvent onMoveEnd;

    //private Components
    protected Rigidbody _body;

    protected void Start()
    {
        _body = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 dir)
    {
        //움직임 방향 전환
        if (direction != Vector3.zero)
        {
            lastDirection = direction;
        }
        direction = dir;
        IsMoving = true;
        onMove.Invoke();
    }

    public void StopMove()
    {
        direction = Vector3.zero;
        IsMoving = false;
        onMoveEnd.Invoke();
    }

    protected void FixedUpdate()
    {
        if (IsMoving && CanMove)
        {
            BeforeMovement();
            _body.MovePosition(_body.position + direction.normalized*(speed*Time.fixedDeltaTime));
            if (useRotation) SetRotation(lastDirection);
            AfterMovement();
        }
    }

    protected void BeforeMovement() { }
    protected void AfterMovement() { }

    public float SetRotation(Vector3 dir)
    {
        Vector3 a = Vector3.forward;
        Vector3 b = dir.normalized;
        float angle = Vector3.Angle(a, b);
        float t = ((b.x*a.z - a.x*b.z) >= 0) ? 1f : -1f;
        transform.rotation = Quaternion.Euler(0f, angle*t, 0f);
        return angle*t;
    }

}
