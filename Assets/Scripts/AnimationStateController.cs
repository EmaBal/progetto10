using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    private Animator animator;
    private AudioSource[] sounds;
    private AudioSource atkSFX;
    private AudioSource deathSFX;
    private AudioSource critSFX;

    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        sounds = GetComponents<AudioSource>();
        atkSFX = sounds[0];
        deathSFX = sounds[1];
        critSFX = sounds[2];
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
            atkSFX.PlayDelayed(0.3f);
            
        } else if (who == "enemy")
        {
            animator.Play("attack_1");
            atkSFX.PlayDelayed(0.5f);
        }
    }
    
    public void CritAttack()
    {
        animator.Play("Attack02");
        critSFX.PlayDelayed(0.1f);
    }
    
    public void ComboAttack()
    {
        animator.Play("Attack01 0");
        atkSFX.PlayDelayed(0.1f);
        critSFX.PlayDelayed(0.5f);

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
            deathSFX.Play();
        } else if (who == "enemy")
        {
            animator.Play("death");
            deathSFX.Play();
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
