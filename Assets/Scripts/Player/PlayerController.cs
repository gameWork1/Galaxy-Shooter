using System;
using Logger;
using Mirror;
using Mirror.BouncyCastle.Math.Field;
using Network;
using Player;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class PlayerController : NetworkBehaviour, IDisposable
{
    [SerializeField] private float _speed;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private TMP_Text nickNameText;

    private Vector2 _direction;
    private InputActions _inputActions;

    [SyncVar(hook = nameof(ChangeNameText))]
    private string _playerName;

    public string PlayerName
    {
        get { return _playerName; }
    }

    private LoggerManager _logger;
    [SyncVar] public bool isJoined = false;

    #region NickName
    
    [TargetRpc]
    public void TargetSetUpLocalNickname(NetworkConnection target)
    {
        if (NetworkManager.singleton is CustomNetworkManager manager)
        {
            string name = manager.GetNickName();
            CmdSetNickname(name);
            LogJoin(name);
        }
    }

    [Client]
    public void LogJoin(string name)
    {
        if (!isJoined)
        {
            isJoined = true;
            CmdLogInJoin(name);
        }
           
    }
    
    [Command(requiresAuthority = false)]
    private void CmdLogInJoin(string name)
    {
        _logger.AddPlayerConnectedMessage(name);
    }

    [Command(requiresAuthority = false)]
    private void CmdSetNickname(string text)
    {
        _playerName = text;
    }
    
    private void ChangeNameText(string oldText, string newText)
    {
        nickNameText.text = newText;
        CmdChangeNameText(newText);
    }
    
    [Command(requiresAuthority = false)]
    private void CmdChangeNameText(string text)
    {
        nickNameText.text = text;
    }

    #endregion

    public override void OnStartClient()
    {
        _logger = FindObjectOfType<LoggerManager>();
    }
    private void Start()
    {
        if (!isLocalPlayer) return;
            
        CinemachineCamera _camera = FindAnyObjectByType<CinemachineCamera>();
        _camera.Follow = transform;
        _camera.transform.position = transform.position;
        _inputActions = new InputActions();
        
        _inputActions.Player.Enable();
        _inputActions.Player.Move.performed += ChangeDirection;
        _inputActions.Player.Move.canceled += ChangeDirection;
        
    }

    private void ChangeDirection(InputAction.CallbackContext _context)
    {
        _direction = _context.ReadValue<Vector2>();
    }
    
    private void FixedUpdate()
    {
        _rb.linearVelocity = _direction * _speed;
    }

    public void Dispose()
    {
        _inputActions.Player.Move.performed -= ChangeDirection;
        _inputActions.Player.Move.canceled -= ChangeDirection;
    
        _inputActions.Player.Disable();
    }
}
