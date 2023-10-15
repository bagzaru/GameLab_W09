using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Dasher : MonoBehaviour
{
	public bool IsDashing { get; private set; } = false;
	public bool CanDash { get; private set; } = true;
	
	[Header("대쉬 관련 정보")]
	public float dashTime = 0.2f;
	public float dashDistance = 10f;
	private Vector3 direction;
	
	
	[Header("대쉬 딜레이")]
	public bool isDashDelayed = false;
	public float dashDelay = 0.8f;
	private float dashDelayTimer = 0f;

	public UnityEvent onDashStart;
	public UnityEvent onDashEnd;

	private Rigidbody _body;

	protected void Start()
	{
		_body = GetComponent<Rigidbody>();
		onDashEnd.AddListener(OnDashEnd);
	}
	
	private void Update()
	{
		//대쉬 쿨타임 계산
		if (isDashDelayed)
		{
			dashDelayTimer += Time.deltaTime;
			if (dashDelayTimer >= dashDelay)
			{
				isDashDelayed = false;
				dashDelayTimer = 0f;
			}
		}
	}

	public void Dash(Vector3 dir)
	{
		if (isDashDelayed||!CanDash||IsDashing)
		{
			return;
		}
		IsDashing = true;
		direction = dir.normalized;
		onDashStart.Invoke();
		StartCoroutine(DashCoroutine());
	}

	private IEnumerator DashCoroutine()
	{
		float counter = 0f;
		float dashSpeed = dashDistance/dashTime;		//시간과 대시 거리를 설정하면 자동 반영
		
		while (counter <= dashDistance)
		{
			float nextDashAmount = dashSpeed*Time.fixedDeltaTime;
			Debug.Log($"{nextDashAmount}, {dashSpeed} * {Time.fixedDeltaTime}");
			Debug.Log($"if: {nextDashAmount} + {counter} >= {dashDistance}");
			if (nextDashAmount + counter >= dashDistance)
			{
				//이번 프레임 움직일 거리보다 남은 거리가 작으면 마지막 움직임
				nextDashAmount = dashDistance - counter;
				_body.MovePosition(_body.position + direction*nextDashAmount);
				onDashEnd.Invoke();
				yield break;
			}
			else
			{
				_body.MovePosition(_body.position + direction*nextDashAmount);
			}
			counter += nextDashAmount;
			yield return new WaitForFixedUpdate();
		}
	}
	
	private void OnDashEnd()
	{
		IsDashing = false;
		isDashDelayed = true;
	}
	
}
