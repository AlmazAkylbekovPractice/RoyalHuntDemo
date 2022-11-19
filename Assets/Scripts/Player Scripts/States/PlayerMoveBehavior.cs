using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveBehavior : IPlayerBehavior
{
    //Movement Properties
    private float _turmSmoothTime = 0.1f;
    private float _turmSmoothvelocity;


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
        if (player.input.magnitude >= 0)
        {
            float targetAngle = Mathf.Atan2(player.input.x, player.input.z) * Mathf.Rad2Deg + player.cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(player.transform.eulerAngles.y, targetAngle, ref _turmSmoothvelocity, _turmSmoothTime);
            player.transform.rotation = Quaternion.Euler(0f, angle, 0f);

            player.direction = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }

        player.controller.Move(player.direction.normalized * player.movementSpeed * Time.deltaTime);
    }

    public void InputHandler(Player player)
    {
        player.input.x = Input.GetAxis("Horizontal");
        player.input.z = Input.GetAxis("Vertical");

        if (player.input.magnitude == 0) player.SetBehaviorIdle();
        if (Input.GetMouseButtonDown(0)) player.SetBehaviorAttack();
    }

    public void Update(Player player)
    {
        

    }
}
