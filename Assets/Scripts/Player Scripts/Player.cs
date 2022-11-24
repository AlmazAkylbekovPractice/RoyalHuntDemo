using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using Mirror;
using Cinemachine;

public class Player : NetworkBehaviour
{
    [Header("Player Characteristics")]
    //Player Parameters
    [SyncVar] [HideInInspector] public float movementSpeed;
    [SyncVar] [HideInInspector] public float forwardJumpDistance;
    [SyncVar] [HideInInspector] public float forwardJumpImpulse;
    [SyncVar] [HideInInspector] public float recoverTime;

    [Header("Player Conditions")]
    public bool isAttacking;
    public bool isHurted;
    public bool isFreezed;

    [Header("Player Points")]
    [SerializeField] public int hitScores;
    [SyncVar(hook = nameof(SyncHitScorePoints))]
    [SerializeField] private int syncHitScores;

    [SerializeField] public int injuredScores;
    [SyncVar(hook = nameof(SyncInjuredScorePoints))]
    [SerializeField] private int syncInjuredScores;

    [HideInInspector] public CharacterController controller;
    [HideInInspector] public Animator animator;
    [SerializeField] private SkinnedMeshRenderer meshRenderer;

    //Layers
    [SerializeField] public LayerMask playerLayer;

    //Cameras
    public Transform cam;
    public CinemachineFreeLook freeLookCam;

    //State Patter Properties
    private Dictionary<Type, IPlayerBehavior> behaviorsMap;
    private IPlayerBehavior currentBehavior;

    //Player animation properties
    [HideInInspector] public string IS_RUNNING_TAG = "isRunning";
    [HideInInspector] public string IS_ATTACKING_TAG = "isAttacking";

    [HideInInspector] public Vector3 input;
    [HideInInspector] public Vector3 direction;

    private void Start()
    {
        if (isServer)
        {
            LoadCharacteristics();
        }

        if (isClient && isLocalPlayer)
        {
            SendPlayerInput();
            LoadComponents();
            OnStartLocalPlayer();
            LoadStates();
        }
    }

    private void LoadComponents()
    {
        controller = this.GetComponent<CharacterController>();
        animator = this.GetComponent<Animator>();
        meshRenderer = this.GetComponentInChildren<SkinnedMeshRenderer>();
    }

    private void SendPlayerInput()
    {
        InputManager.Instance.SetPlayer(this);
    }

    public void MovePlayer(Vector3 movementInput)
    {
        this.input = movementInput;
    }

    public override void OnStartLocalPlayer()
    {
        cam = GameObject.FindWithTag("MainCamera").transform;

        freeLookCam = CinemachineFreeLook.FindObjectOfType<CinemachineFreeLook>();

        freeLookCam.LookAt = this.gameObject.transform;
        freeLookCam.Follow = this.gameObject.transform;

        UInterfaceManager.instance.UpdateScore(syncHitScores, syncInjuredScores);
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
        if (isLocalPlayer)
        {
            CmdSyncPlanePosition(this.transform.position);

            if (this.currentBehavior != null)
            {
                this.currentBehavior.Update(this);
            }
        }
    }

    [Command]
    private void CmdSyncPlanePosition(Vector3 currentPosition)
    {
        RpcServerSyncPlayer(currentPosition);
    }

    [ClientRpc]
    private void RpcServerSyncPlayer(Vector3 currentPosition)
    {
        if (!isLocalPlayer)
        {
            transform.position = currentPosition;
        }
    }


    /// <summary>
    /// Used for player physics
    /// </summary>
    private void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            if (this.currentBehavior != null)
            {
                this.currentBehavior.FixedUpdate(this);
            }
        }
    }

    /// <summary>
    /// Used for handling player input
    /// </summary>
    private void LateUpdate()
    {
        if (isLocalPlayer)
        {
            if (this.currentBehavior != null)
            {
                this.currentBehavior.InputHandler(this);
            }
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

    public void ApplyDamage()
    {
        //if (isServer)
        //    ChangeInjuredScoreValue(this.injuredScores + 1);
        //else
        //    CmdChangeInjuredScores(this.injuredScores + 1);
    }

    public void CountAsHit()
    {
        //if (isServer)
        //    ChangeHitScoreValue(this.hitScores + 1);
        //else
        //    CmdChangeHitScores(this.hitScores + 1);

    }

    public void PlayerRecover()
    {
        if (isLocalPlayer)
        {
            this.meshRenderer.material = this.meshRenderer.materials[1];
            this.isHurted = false;
        }
    }

    public void UpdateUI()
    {
        if (!isLocalPlayer) return;
        UInterfaceManager.instance.UpdateScore(syncHitScores,syncInjuredScores);
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() != null)
        {
            Player enemy = other.GetComponent<Player>();

            if(this.netId != enemy.netId)
            {
                if (enemy.isAttacking && !this.isHurted)
                {
                    if (isServer)
                        ChangeInjuredScoreValue(injuredScores + 1);
                    else
                        CmdChangeInjuredScores(injuredScores + 1);
                }
                else if (this.isAttacking && !enemy.isHurted)
                {
                    if (isServer)
                        ChangeHitScoreValue(hitScores + 1);
                    else
                        CmdChangeHitScores(hitScores + 1);
                }

                this.UpdateUI();
            }
        }
    }


    private void SyncHitScorePoints(int oldHitScore, int newHitScore)
    {
        this.hitScores = newHitScore;
    }

    private void SyncInjuredScorePoints(int oldInjuredScore, int newInjuredScore)
    {
        this.injuredScores = newInjuredScore;
    }

    #region HitScoreHandlers
    [Server]
    public void ChangeHitScoreValue(int newHitScore)
    {
        syncHitScores = newHitScore;
    }

    [Command]
    public void CmdChangeHitScores(int newScores)
    {
        ChangeHitScoreValue(newScores);
    }
    #endregion

    #region InjuredScoreHangler
    [Server]
    public void ChangeInjuredScoreValue(int newInjuredScore)
    {
        syncInjuredScores = newInjuredScore;
    }

    [Command]
    public void CmdChangeInjuredScores(int newScores)
    {
        ChangeInjuredScoreValue(newScores);
    }
    #endregion


}
