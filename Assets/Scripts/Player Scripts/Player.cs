using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

public class Player : MonoBehaviour
{
    [Header("Player Characteristics")]
    //Player Parameters
    [HideInInspector] public float movementSpeed;
    [HideInInspector] public float forwardJumpDistance;
    [HideInInspector] public float forwardJumpImpulse;
    [HideInInspector] public float recoverTime;

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

    private void Awake()
    {
        LoadComponents();
        LoadStates();
        LoadCharacteristics();
    }

    private void LoadComponents()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        cam = GameObject.FindWithTag("MainCamera").transform;
    }

    private void LoadStates()
    {
        this.InitializeBehabiors();
        this.SetDefaultBehavior();
    }

    private void LoadCharacteristics()
    {
        this.movementSpeed = GameManager.instance.GetSpeed;
        this.forwardJumpImpulse = GameManager.instance.GetJumpForce;
        this.forwardJumpDistance = GameManager.instance.GetJumpDistance;
        this.recoverTime = GameManager.instance.GetRecoveryTime;
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

    
    public int ApplyDamage()
    {
        int point = 0;
        float timer = recoverTime;

        if (!this.isHurted)
        {
            this.damageScores++;
            this.isHurted = true;

            this.meshRenderer.material = meshRenderer.materials[2];

            point = 1;

            Invoke("PlayerRecover", recoverTime);
        }

        return point;
    }

    public void PlayerRecover()
    {
        this.meshRenderer.material = meshRenderer.materials[1];
        this.isHurted = false;
    }

    public void UpdateUI()
    {
        UInterfaceManager.instance.UpdateScore(hitScores);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && this.isAttacking)
        {
            this.hitScores += other.gameObject.GetComponent<Player>().ApplyDamage();
            this.UpdateUI();
        }
    }
}
