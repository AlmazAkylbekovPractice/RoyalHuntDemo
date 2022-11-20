using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Mirror;

public class PlayerAttackBehavior : IPlayerBehavior
{
    Vector3 startPos;
    Vector3 lastPos;

    public void Enter(Player player)
    {
        startPos = player.transform.position;

        player.isAttacking = true;
        player.animator.SetBool(player.IS_ATTACKING_TAG,true);

    }

    public void Exit(Player player)
    {
        player.isAttacking = false;
        player.animator.SetBool(player.IS_ATTACKING_TAG, false);
    }

    public void FixedUpdate(Player player)
    {
        lastPos = player.transform.position;

        if (player.forwardJumpDistance > Vector3.Distance(startPos, player.transform.position))
        {
            player.controller.Move(player.movementSpeed * player.forwardJumpImpulse * player.transform.TransformDirection(Vector3.forward) * Time.deltaTime);
        }

    }

    public void InputHandler(Player player)
    {
        if (player.forwardJumpDistance <= Vector3.Distance(startPos, player.transform.position))
        {
            if (player.input.magnitude == 0) player.SetBehaviorIdle();
            else player.SetBehaviorMove();
        }
    }

    public void Update(Player player)
    {
        if (Vector3.Distance(lastPos, player.transform.position) / Time.deltaTime < 0.01f)
        {
            player.SetBehaviorMove();
        }
    }
}


