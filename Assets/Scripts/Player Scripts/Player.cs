using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    [Header("Player Characteristics")]
    //Player Parameters
    public float movementSpeed;
    public float forwardJumpDistance;
    public float forwardJumpImpulse;
    public float recoverTime;

    [Header("Player Conditions")]
    public bool isAttacking;
    public bool isHurted;
    public bool isFreezed;

    [Header("Player Points")]
    [SerializeField] private int hitScores;
    [SerializeField] private int damageScores;

    [HideInInspector] public CharacterController controller;
    [HideInInspector] public Animator animator;
    [SerializeField] private SkinnedMeshRenderer meshRenderer;

    public Transform cam;

    //State Patter Properties
    private Dictionary<Type, IPlayerBehavior> behaviorsMap;
    private IPlayerBehavior currentBehavior;

    //Player animation properties
    [HideInInspector] public string IS_RUNNING_TAG = "isRunning";
    [HideInInspector] public string IS_ATTACKING_TAG = "isAttacking";

    [HideInInspector] public Vector3 input;
    [HideInInspector] public Vector3 direction;
    private float timeRemaining;

    private void Awake()
    {
        LoadComponents();
        LoadStates();
        LoadParameters();
    }

    private void LoadComponents()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    private void LoadStates()
    {
        this.InitializeBehabiors();
        this.SetDefaultBehavior();
    }

    private void LoadParameters()
    {
        timeRemaining = recoverTime;
    }

    private void InitializeBehabiors()
    {
        this.behaviorsMap = new Dictionary<Type, IPlayerBehavior>();

        this.behaviorsMap[typeof(PlayerIdleBehavior)] = new PlayerIdleBehavior();
        this.behaviorsMap[typeof(PlayerMoveBehavior)] = new PlayerMoveBehavior();
        this.behaviorsMap[typeof(PlayerAttackBehavior)] = new PlayerAttackBehavior();
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
            this.PlayerRecover();
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

    public void SetBehaviorAttack()
    {
        var behavior = this.GetBehavior<PlayerAttackBehavior>();
        this.SetBehavior(behavior);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && this.isAttacking)
        {
            hitScores += other.gameObject.GetComponent<Player>().ApplyDamage();
        }
    }

    public int ApplyDamage()
    {
        int point = 0;
        float timer = recoverTime;

        if (!this.isHurted)
        {
            this.damageScores++;
            this.isHurted = true;
            point = 1;
        }

        return point;
    }

    public void PlayerRecover()
    {
        if (this.isHurted)
        {
            if (timeRemaining > 0)
            {
                meshRenderer.material = meshRenderer.materials[2];
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                meshRenderer.material = meshRenderer.materials[1];
                timeRemaining = recoverTime;
                this.isHurted = false;
            }
        }
    }
}
