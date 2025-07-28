using System;
using Mirror;
using Network;
using Player;
using Player.Gun;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

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
    public void TargetSetUpLocalNickname(NetworkConnection target)
    {
        if (NetworkManager.singleton is CustomNetworkManager manager)
        {
            string name = manager.GetNickName();
            _nicknameManager.CmdSetNickname(name);
            _nicknameManager.LogJoin(name);
        }
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
        if (!isDead) return;
        isDead = false;
        transform.position = _startPosition;
        _visualObjects.SetActive(true);
        CmdRespawn();
    }

    [Command(requiresAuthority = false)]
    private void CmdRespawn()
    {
        _visualObjects.SetActive(true);
        isTrigger = false;
    }

    public void SetUsing(bool active)
    {
        isUsing = active;
    }

    private void ChangeDirection(InputAction.CallbackContext _context)
    {
        if (isDead || isUsing) return;
        
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
            if(_inputActions.Player.Shoot.IsPressed()) _gun.Shoot();
        }
            
    }

    public void Dispose()
    {
        _inputActions.Player.Move.performed -= ChangeDirection;
        _inputActions.Player.Move.canceled -= ChangeDirection;
    
        _inputActions.Player.Disable();
    }
}
