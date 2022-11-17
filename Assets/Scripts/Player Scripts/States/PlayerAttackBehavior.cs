using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class PlayerAttackBehavior : IPlayerBehavior
{
    Vector3 startPos;

    public void Enter(Player player)
    {
        startPos = player.transform.position;

        player.animator.SetBool(player.IS_ATTACKING_TAG,true);

    }

    public void Exit(Player player)
    {
        player.animator.SetBool(player.IS_ATTACKING_TAG, false);
    }

    public void FixedUpdate(Player player)
    {
        if (player.forwardJumpDistance > Vector3.Distance(startPos, player.transform.position))
        {
            player.controller.Move(player.movementSpeed * player.forwardJumpImpulse * player.transform.TransformDirection(Vector3.forward) * Time.deltaTime);
        }

    }

    public void InputHandler(Player player)
    {
        if (player.forwardJumpDistance <= Vector3.Distance(startPos, player.transform.position))
        {
            player.input.x = Input.GetAxis("Horizontal");
            player.input.z = Input.GetAxis("Vertical");

            if (player.input.magnitude == 0) player.SetBehaviorIdle();
            else player.SetBehaviorMove();
        }
    }

    public void Update(Player player)
    {

    }
}
