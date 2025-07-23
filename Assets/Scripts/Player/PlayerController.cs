using System;
using Mirror;
using Network;
using Player;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour, IDisposable
{
    [SerializeField] private float _speed;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private TMP_Text nickNameText;

    private Vector2 _direction;
    private InputActions _inputActions;
    [SyncVar(hook = nameof(ChangeNameText))]
    private string _playerName;

    #region NickName
    
    [TargetRpc]
    public void TargetSetUpLocalNickname(NetworkConnection target)
    {
        if (NetworkManager.singleton is CustomNetworkManager manager)
        {
            CmdSetNickname(manager.GetNickName());
        }
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
    
    
    
    void Start()
    {
        if (!isLocalPlayer) return;
        
        FindAnyObjectByType<CinemachineCamera>().Follow = transform;
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
