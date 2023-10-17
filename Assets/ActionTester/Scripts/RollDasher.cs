using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollDasher : Dasher
{
	private Mover _mover;
	[Header("RollDasher")]
	public float speed = 15f;
	
	
	public LayerMask damageLayer;
	public float damage = 1f;
	public float collisionRadius = 1.4f;
	
	private List<Damageable> damagedList = new();
	
	protected override void Start()
	{
		base.Start();
		_mover = GetComponent<Mover>();
	}

	public override void Dash(Vector3 dir)
	{
		if (isCooldown||!CanDash||IsDashing)
		{
			return;
		}
		IsDashing = true;
		direction = dir.normalized;
		onCasting.Invoke();
	}

	private float _counter = 0f;
	protected void FixedUpdate()
	{
		if (IsDashing)
		{
			_body.MovePosition(_body.position + direction.normalized*(speed*Time.fixedDeltaTime));
			_mover.SetRotation(direction);
			foreach (var t in damagedList)
			{
				t.Hit(damage*Time.fixedDeltaTime);
			}
			
			_counter += Time.fixedDeltaTime;
			if (_counter >= dashTime)
			{
				_counter = 0f;
				IsDashing = false;
				onFinish.Invoke();
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (((1 << other.gameObject.layer) & damageLayer)>0)
		{
			Damageable d = other.GetComponent<Damageable>();
			damagedList.Add(d);
		}    
	}

	private void OnTriggerExit(Collider other)
	{
		if (((1 << other.gameObject.layer) & damageLayer)>0)
		{
			Damageable d = other.GetComponent<Damageable>();
			damagedList.Remove(d);
		}   
	}
}
