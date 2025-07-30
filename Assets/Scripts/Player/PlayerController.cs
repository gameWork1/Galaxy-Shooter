using System;
using Logger;
using Mirror;
using Network;
using Player;
using Player.Gun;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class PlayerController : NetworkBehaviour, IDisposable
{
    [SerializeField] private float _speed;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private Animator _animator;
    [SerializeField] private NicknameManager _nicknameManager;
    [SerializeField] private HealthManager _healthManager;
    [SerializeField] private Gun _gun;
    [SerializeField] private bool isFacingRight = true;
    [SerializeField] private GameObject _spriteGameObject;
    [SerializeField] private GameObject _visualObjects;
    [SerializeField] private int _deadTime;

    private GameManager _gameManager;
    private LoggerManager _logger;
    
    public int Health
    {
        get { return _healthManager.Health; }
    }
    private Vector2 _direction;
    private Vector3 _startPosition;
    private InputActions _inputActions;
    private bool isUsing = false;
    private bool isDead = false;
    [SyncVar(hook = nameof(OnChangeTrigger))] private bool isTrigger;

    [Inject]
    private void Construct(GameManager gameManager, LoggerManager logger)
    {
        _gameManager = gameManager;
        _logger = logger;

        if (isLocalPlayer)
        {
            _gameManager.SetPlayer(this);
            SetUpLocalNickname();
        }
    }

    private void OnChangeTrigger(bool oldTrigger, bool newTrigger)
    {
        isTrigger = newTrigger;
        _collider.isTrigger = newTrigger;
    }

    public string PlayerName
    {
        get { return _nicknameManager.PlayerName; }
    }

    public int DeadTime
    {
        get { return _deadTime; }
    }
    
    [TargetRpc]
    public void TargetKill(NetworkConnection target, string killedPlayerName)
    {
        CmdKillMessage(killedPlayerName, _logger);
        _gameManager.KilledPlayer(PlayerName);
    }

    [Command(requiresAuthority = false)]
    private void CmdKillMessage(string killedPlayerName, LoggerManager logger)
    {
        logger.AddPlayerKillMessage(PlayerName, killedPlayerName);
    }
    
    public void SetUpLocalNickname()
    {
        if (NetworkManager.singleton is CustomNetworkManager manager)
        {
            string name = manager.GetNickName();
            _nicknameManager.SetNickname(name);
            _nicknameManager.LogJoin(name);
            _gameManager.AddPlayer(name);
        }
    }

    public void StartGame()
    {
        _gun.SetSpriteState(true);
        Respawn();
    }

    public void StopGame()
    {
        _gun.SetSpriteState(false);
    }
    
    private void Start()
    {
        if (!isLocalPlayer) return;
            
        CinemachineCamera _camera = FindAnyObjectByType<CinemachineCamera>();
        _camera.Follow = transform;
        _camera.ForceCameraPosition(transform.position, transform.rotation);
        _inputActions = new InputActions();
        
        _inputActions.Player.Enable();
        _inputActions.Player.Move.performed += ChangeDirection;
        _inputActions.Player.Move.canceled += ChangeDirection;
        
    }

    public override void OnStartClient()
    {
        if (!isLocalPlayer) return;
        var sceneContext = FindFirstObjectByType<SceneContext>();
        sceneContext.Container.Inject(_gun);
        sceneContext.Container.Inject(_nicknameManager);
        sceneContext.Container.Inject(this);
        _startPosition = transform.position;
    }

    public void Dead()
    {
        isDead = true;
        _visualObjects.SetActive(false);
        CmdDead();
    }

    [Command(requiresAuthority = false)]
    private void CmdDead()
    {
        _visualObjects.SetActive(false);
        _rb.linearVelocity = Vector2.zero;
        isTrigger = true;
    }

    public void Respawn()
    {
        isDead = false;
        isTrigger = false;
        _visualObjects.SetActive(true);
        CmdRespawn();
    }

    [TargetRpc]
    private void TargetRespawn(NetworkConnection target)
    {
        transform.position = _startPosition;
    }

    [Command(requiresAuthority = false)]
    private void CmdRespawn()
    {
        _visualObjects.SetActive(true);
        isTrigger = false;
        TargetRespawn(GetComponent<NetworkIdentity>().connectionToClient);
    }

    public void SetUsing(bool active)
    {
        isUsing = active;
    }

    private void ChangeDirection(InputAction.CallbackContext _context)
    {
        if (!isLocalPlayer || isDead || isUsing) return;
        
        _direction = _context.ReadValue<Vector2>();

        if (_direction.x > 0 && !isFacingRight) Flip();
        else if (_direction.x < 0 && isFacingRight) Flip();
        
        if(_direction != Vector2.zero) _animator.SetBool("isWalk", true);
        else _animator.SetBool("isWalk", false);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;

        if (isFacingRight) _spriteGameObject.transform.eulerAngles = new Vector3(0, 0, 0);
        else _spriteGameObject.transform.eulerAngles = new Vector3(0, 180, 0);
    }

    public void TakeDamage(int damage)
    {
        _healthManager.TakeDamage(damage);
    }
    
    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;
        
        if (!isUsing && !isDead)
        { 
            _rb.linearVelocity = _direction * _speed;
            if(_inputActions.Player.Shoot.IsPressed() && _gameManager.IsGameStarted) _gun.Shoot();
            if(_inputActions.Player.Recharge.IsPressed() && _gameManager.IsGameStarted) _gun.Recharge();
        }
            
    }

    public void Dispose()
    {
        _inputActions.Player.Move.performed -= ChangeDirection;
        _inputActions.Player.Move.canceled -= ChangeDirection;
    
        _inputActions.Player.Disable();
    }
}
