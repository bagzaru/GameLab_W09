using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
	public float hp;
	public float maxHp = 5f;

	[HideInInspector]public UnityEvent onDamaged;
	[HideInInspector]public UnityEvent onKilled;

	private void Start()
	{
		hp = maxHp;
	}

	public void Hit(float damage)
	{
		hp -= damage;
		onDamaged.Invoke();
		Debug.Log($"{gameObject.name}: 데미지 입음");
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
