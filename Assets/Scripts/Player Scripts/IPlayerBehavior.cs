using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public interface IPlayerBehavior
{
    void Enter(Player player);
    void Update(Player player);
    void FixedUpdate(Player player);
    void InputHandler(Player player);
    void Exit(Player player);
}
