using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class RushGolemAttacker : MonoBehaviour
{
	public LayerMask damageLayer;
	public float damage = 1f;
	public float collisionRadius = 1.4f;
	private bool _isAttacking;
	public bool IsAttacking
	{
		get { return _isAttacking; }
		set
		{
			_isAttacking = value;
			if (!value) damagedList.Clear();
		}
	}
	
	private List<GameObject> damagedList = new List<GameObject>();
	private Dasher _dasher;

	private void Start()
	{
		_dasher = GetComponent<Dasher>();
		_dasher.onDash.AddListener(() => { IsAttacking = true;});
		_dasher.onStun.AddListener(() => { IsAttacking = false;});
	}

	private void FixedUpdate()
	{
		if (IsAttacking)
		{
			FindEnemies();
		}
	}

	void FindEnemies()
	{
		//공격 대상 체킹
		Collider[] cols = Physics.OverlapSphere(transform.position, collisionRadius);
		for(int i=0;i<cols.Length;i++)
		{
			GameObject target = cols[i].gameObject;
			if (((1 << target.layer) & damageLayer)>0
			    &&!damagedList.Contains(target))
			{
				damagedList.Add(target);
				Damageable d = target.GetComponent<Damageable>();
				d.Hit(damage);
			}    
		}
	}
}
