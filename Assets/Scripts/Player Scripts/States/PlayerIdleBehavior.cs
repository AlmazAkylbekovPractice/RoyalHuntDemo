using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        player.playerDirection.x = Input.GetAxis("Horizontal");
        player.playerDirection.z = Input.GetAxis("Vertical");

        if (player.playerDirection.magnitude != 0)
        {
            player.SetBehaviorMove();
        }
    }

    public void Update(Player player)
    {

    }
}
