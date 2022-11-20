using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerIdleBehavior : IPlayerBehavior
{
    public void Enter(Player player)
    {

    }

    public void Exit(Player player)
    {

    }

    public void FixedUpdate(Player player)
    {

    }

    public void InputHandler(Player player)
    {
        if (player.input.magnitude != 0) player.SetBehaviorMove();
        if (Input.GetMouseButtonDown(0)) player.SetBehaviorAttack();
    }

    public void Update(Player player)
    {

    }
}
