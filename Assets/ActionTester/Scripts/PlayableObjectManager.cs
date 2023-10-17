using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayableObjectManager : MonoBehaviour
{
    public List<PlayerController> playables;
    private int current = 0;
    public float maxhp = 0;
    public float hp = 0;
    public Slider slider;

    private void Start()
    {
        playables[current] = playables[current];
        maxhp = hp = playables[current].GetComponent<Damageable>().maxHp;
        playables[current].GetComponent<Damageable>().hp = hp;
    }

    private void Update()
    {
        hp = playables[current].GetComponent<Damageable>().hp;
        slider.value = hp/maxhp;
    }

    private void FixedUpdate()
    {
        transform.position = playables[current].transform.position;
        transform.rotation = playables[current].transform.rotation;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        playables[current].OnMove(context);
    }


    public void OnAttack(InputAction.CallbackContext context)
    {
        playables[current].OnAttack(context);
    }


    public void OnDash(InputAction.CallbackContext context)
    {
        playables[current].OnDash(context);
    }

    public void OnSwitchRight(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            int next = current + 1;
            if (next >= playables.Count)
            {
                next = 0;
            }
            UpdateCurrent(next);
        }
    }

    public void OnSwitchLeft(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            int next = current - 1;
            if (next < 0)
            {
                next = playables.Count - 1;
            }
            UpdateCurrent(next);
        }
    }

    private void UpdateCurrent(int next)
    {
        float curHp = playables[current].GetComponent<Damageable>().hp;
        playables[current].gameObject.SetActive(false);
        current = next;
        playables[current].gameObject.SetActive(true);
        playables[current].transform.position = transform.position;
        playables[current].transform.rotation = transform.rotation;
        playables[current].GetComponent<Damageable>().hp = curHp;
    }

}
