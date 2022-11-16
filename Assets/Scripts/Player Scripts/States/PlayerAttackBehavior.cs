using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerAttackBehavior : IPlayerBehavior
{
    public void Enter(Player player)
    {
        //Enable forward push animation
        player.animator.Play("Flip");
    }

    public void Exit(Player player)
    {
        player.animator.SetBool(player.IS_ATTACKING_TAG, false);
    }

    public void FixedUpdate(Player player)
    {
        player.controller.Move(player.forwardJump * player.movementSpeed * Time.deltaTime * player.direction.normalized);
    }

    public void InputHandler(Player player)
    {
        if (player.input.magnitude == 0) player.SetBehaviorIdle();
        else player.SetBehaviorMove();
    }

    public void Update(Player player)
    {

    }
}
