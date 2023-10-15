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

	[Header("선딜레이")]
	public float dashCastingTime = 0.5f;
	[Header("후딜레이")]
	public float dashStunTime = 0.5f;
	
	[FormerlySerializedAs("isDashDelayed")] [Header("대쉬 딜레이")]
	public bool isCooldown = false;
	[FormerlySerializedAs("dashDelay")] public float dashCooldown = 0.8f;
	private float _coolDownCounter = 0f;

	[HideInInspector]public UnityEvent onCasting;
	[HideInInspector]public UnityEvent onDash;
	[HideInInspector] public UnityEvent onFixedUpdateDash;
	[HideInInspector]public UnityEvent onStun;
	[HideInInspector]public UnityEvent onFinish;

	private Rigidbody _body;

	protected void Start()
	{
		_body = GetComponent<Rigidbody>();
		onFinish.AddListener(OnDashEnd);
	}
	
	private void Update()
	{
		//대쉬 쿨타임 계산
		if (isCooldown)
		{
			_coolDownCounter += Time.deltaTime;
			if (_coolDownCounter >= dashCooldown)
			{
				isCooldown = false;
				_coolDownCounter = 0f;
			}
		}
	}

	public void Dash(Vector3 dir)
	{
		if (isCooldown||!CanDash||IsDashing)
		{
			return;
		}
		IsDashing = true;
		direction = dir.normalized;
		StartCoroutine(DashCoroutine());
	}

	private IEnumerator DashCoroutine()
	{
		//대쉬 시전 딜레이
		onCasting.Invoke();
		yield return new WaitForSeconds(dashCastingTime);
		
		//실제 대쉬 실행
		float counter = 0f;
		float dashSpeed = dashDistance/dashTime;		//시간과 대시 거리를 설정하면 자동 반영
		float nextDashAmount = 0f;
		onDash.Invoke();
		while (counter <= dashDistance)
		{
			nextDashAmount = dashSpeed*Time.fixedDeltaTime;
			if (nextDashAmount + counter >= dashDistance)
			{
				//이번 프레임 움직일 거리보다 남은 거리가 작으면 마지막 움직임
				nextDashAmount = dashDistance - counter;
				_body.MovePosition(_body.position + direction*nextDashAmount);
				break;
			}
			else
			{
				_body.MovePosition(_body.position + direction*nextDashAmount);
			}
			counter += nextDashAmount;
			onFixedUpdateDash.Invoke();
			yield return new WaitForFixedUpdate();
		}
		//대쉬 종료
		onStun.Invoke();
		yield return new WaitForSeconds(dashStunTime);
		onFinish.Invoke();	
	}
	
	private void OnDashEnd()
	{
		IsDashing = false;
		isCooldown = true;
	}
	
}
