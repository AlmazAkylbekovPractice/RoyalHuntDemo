using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    #region Singleton
    private static InputManager _instance;

    public static InputManager Instance
    {
        get
        {
            return _instance;
        }
    }
    #endregion

    private Vector3 movementInput;
    private Player playerObj;

    private void Awake()
    {
        _instance = this;
    }

    private void Update()
    {
        MoveInput();
    }

    public void SetPlayer(Player p)
    {
        playerObj = p;
    }

    private void MoveInput()
    {
        movementInput.x = Input.GetAxis("Horizontal");
        movementInput.z = Input.GetAxis("Vertical");
    }
}
