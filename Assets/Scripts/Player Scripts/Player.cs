using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    [HideInInspector] public CharacterController controller;
    [HideInInspector] public Animator animator;

    public Transform cam;

    //State Patter Properties
    private Dictionary<Type, IPlayerBehavior> behaviorsMap;
    private IPlayerBehavior currentBehavior;

    //Player animation properties
    [HideInInspector] public string IS_RUNNING_TAG = "isRunning";

    [HideInInspector] public Vector3 playerDirection;


    //Player Parameters
    public float movementSpeed;


    private void Awake()
    {
        LoadComponents();
        LoadStates();
    }

    private void LoadComponents()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void LoadStates()
    {
        this.InitializeBehabiors();
        this.SetDefaultBehavior();
    }

    private void InitializeBehabiors()
    {
        this.behaviorsMap = new Dictionary<Type, IPlayerBehavior>();

        this.behaviorsMap[typeof(PlayerIdleBehavior)] = new PlayerIdleBehavior();
        this.behaviorsMap[typeof(PlayerMoveBehavior)] = new PlayerMoveBehavior();
    }

    private void SetDefaultBehavior()
    {
        this.SetBehaviorIdle();
    }

    /// <summary>
    /// Updating main parameters and conditions
    /// </summary>
    private void Update()
    {
        if (this.currentBehavior != null)
        {
            this.currentBehavior.Update(this);
        }
    }

    /// <summary>
    /// Used for player physics
    /// </summary>
    private void FixedUpdate()
    {
        if (this.currentBehavior != null)
        {
            this.currentBehavior.FixedUpdate(this);
        }
    }

    /// <summary>
    /// Used for handling player input
    /// </summary>
    private void LateUpdate()
    {
        if (this.currentBehavior != null)
        {
            this.currentBehavior.InputHandler(this);
        }
    }

    private void SetBehavior(IPlayerBehavior newBehavior)
    {
        if (this.currentBehavior != null)
            this.currentBehavior.Exit(this);

        this.currentBehavior = newBehavior;
        this.currentBehavior.Enter(this);
    }

    private IPlayerBehavior GetBehavior<T>() where T : IPlayerBehavior
    {
        return this.behaviorsMap[typeof(T)];
    }

    public void SetBehaviorIdle()
    {
        var behavior = this.GetBehavior<PlayerIdleBehavior>();
        this.SetBehavior(behavior);
    }

    public void SetBehaviorMove()
    {
        var behavior = this.GetBehavior<PlayerMoveBehavior>();
        this.SetBehavior(behavior);
    }
}
