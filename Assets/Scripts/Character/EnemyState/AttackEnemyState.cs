﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEnemyState : IState
{
    private EnemyCharacterController parent;

    private float attackColldown = 0.8f;
    private float extraRange = .1f;

    public void Enter(EnemyCharacterController parent)
    {
        this.parent = parent;
    }

    public void Exit()
    {
        
    }

    public void Update()
    {
        if(parent.AttackTime >= attackColldown && !parent.IsAttacking)
        {
            parent.AttackTime = 0;
            parent.StartCoroutine(Attack());
        }

        if (parent.Target != null)
        {
            //Check range and attack
            float distance = Vector2.Distance(parent.Target.position, parent.transform.position);
            if (distance >= parent.AttackRange + extraRange && !parent.IsAttacking)
            {
                parent.ChangeState(new FollowEnemyBehavior());
            }
        } else
        {
            parent.ChangeState(new IdleEnemyBehavior());
        }
    }

    public IEnumerator Attack()
    {
        parent.IsAttacking = true;
        parent.Animator.SetTrigger("attack");
        
        yield return new WaitForSeconds(parent.Animator.GetCurrentAnimatorStateInfo(2).length);

        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(parent.MeleeAttackPosition.position, parent.MeleeRange);
        if (hitObjects.Length > 0)
        {
            foreach (Collider2D collider2D in hitObjects)
            {
                if (collider2D.tag == "Player")
                {
                    collider2D.SendMessage("TakeDamage", parent.MeleeDamage, SendMessageOptions.DontRequireReceiver);
                    Debug.Log("Enemy Hit " + collider2D.name);
                }
            }
        }

        parent.IsAttacking = false;
    }
}
