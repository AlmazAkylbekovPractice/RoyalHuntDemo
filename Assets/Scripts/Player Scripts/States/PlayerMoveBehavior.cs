using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveBehavior : IPlayerBehavior
{
    //Movement Properties
    private float _turmSmoothTime = 0.1f;
    private float _turmSmoothvelocity;

    private Vector3 moveDirection;

    public void Enter(Player player)
    {
        player.animator.SetBool(player.IS_RUNNING_TAG, true);
    }

    public void Exit(Player player)
    {
        player.animator.SetBool(player.IS_RUNNING_TAG, false);
    }

    public void FixedUpdate(Player player)
    {
        player.controller.Move(moveDirection.normalized * player.movementSpeed * Time.deltaTime);
    }

    public void InputHandler(Player player)
    {
        player.playerDirection.x = Input.GetAxis("Horizontal");
        player.playerDirection.z = Input.GetAxis("Vertical");

        if (player.playerDirection.magnitude == 0)
        {
            player.SetBehaviorIdle();
        }
    }

    public void Update(Player player)
    {
        if (player.playerDirection.magnitude >= 0)
        {
            float targetAngle = Mathf.Atan2(player.playerDirection.x, player.playerDirection.z) * Mathf.Rad2Deg + player.cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(player.transform.eulerAngles.y, targetAngle, ref _turmSmoothvelocity, _turmSmoothTime);
            player.transform.rotation = Quaternion.Euler(0f, angle, 0f);

            moveDirection = Quaternion.Euler(0f,targetAngle,0f) * Vector3.forward;
        }

    }
}
