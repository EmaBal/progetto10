using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    private Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public Animator GetAnimator()
    {
        return animator;
    }

    public void Attack(string who)
    {
        if (who == "player")
        {
            animator.Play("Attack01");
        } else if (who == "enemy")
        {
            animator.Play("attack_1");
        }
    }
    
    public void CritAttack()
    {
        animator.Play("Attack02");
    }
    
    public void ComboAttack()
    {
        animator.Play("Attack01 0");
    }

    public void GetHit(string who)
    {
        if (who == "player")
        {
            animator.Play("GetHit");
        } else if (who == "enemy")
        {
            animator.Play("damage");
        }
    }

    public void Death(string who)
    {
        if (who == "player")
        {
            animator.Play("Die");
        } else if (who == "enemy")
        {
            animator.Play("death");
        }
    }

    public void Update()
    {
        if (animator.GetBool("isAttacking"))
            animator.SetBool("isAttacking", false);
        if (animator.GetBool("isHit"))
            animator.SetBool("isHit", false);
    }
}
