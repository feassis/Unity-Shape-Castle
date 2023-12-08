using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void AnimateMovement(Vector2 movementDir)
    {
        if(movementDir == Vector2.zero)
        {
            PlayAnim(PlayerAnim.Idle);
            return;
        }

        PlayAnim(PlayerAnim.Run);
    }

    public void PlayAnim(PlayerAnim anim)
    {
        switch (anim)
        {
            case PlayerAnim.Idle:
                animator.Play("Idle");
                break;
            case PlayerAnim.Run:
                animator.Play("RunForward");
                break;
        }
    }

    public enum PlayerAnim
{
    Idle = 0,
    Run = 1
}
}