using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
	public float hp;
	public float maxHp = 5f;

	public UnityEvent onDamaged;
	public UnityEvent onKilled;

	private void Start()
	{
		hp = maxHp;
	}

	public void Hit(float damage)
	{
		hp -= damage;
		onDamaged.Invoke();
		if (hp <= 0f)
		{
			hp = 0f;
			Kill();
		}
	}

	public void Kill()
	{
		onKilled.Invoke();
	}
}
