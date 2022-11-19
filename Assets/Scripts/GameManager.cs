using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private float playerSpeed;
    [SerializeField] private float playerJumpForce;
    [SerializeField] private float playerJumpDistance;
    [SerializeField] private float playerRecoveryTime;

    public static GameManager instance;

    private void Awake()
    {
        if (instance != this && instance != null)
        {
            Destroy(this);
        }
        else instance = this;
    }

    public float GetSpeed
    {
        get
        {
            return playerSpeed;
        }
    }

    public float GetJumpForce
    {
        get
        {
            return playerJumpForce;
        }
    }

    public float GetJumpDistance
    {
        get
        {
            return playerJumpDistance;
        }
    }

    public float GetRecoveryTime
    {
        get
        {
            return playerRecoveryTime;
        }
    }
}
