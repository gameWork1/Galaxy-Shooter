using System;
using System.Collections;
using System.Linq;
using Mirror;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private TMP_Text _leaderBoardText;
    [SerializeField] private int startTimer;
    [SyncVar(hook = nameof(TimerHook))] private int timer;
    private UIManager _uiManager;

    private SyncDictionary<string, int> _leaderBoard = new SyncDictionary<string, int>();
    [SyncVar] private bool isGameStarted = false;

    private PlayerController _player;

    private void TimerHook(int oldTime, int newTime)
    {
        timer = newTime;
        _timerText.text = $"{newTime / 60}:{newTime % 60}";
    }

    [Client]
    public void SetPlayer(PlayerController player)
    {
        _player = player;
    }

    public bool IsGameStarted
    {
        get { return isGameStarted; }
    }

    [Inject]
    private void Construct(UIManager uiManager)
    {
        _uiManager = uiManager;

        if (NetworkServer.active && NetworkClient.active)
        {
            _uiManager.StartGameButton.SetActive(true);
            _uiManager.StartGameButton.GetComponent<Button>().onClick.AddListener(StartGame);
        }
        else if(NetworkClient.active)
        {
            _uiManager.StartGameButton.SetActive(false);
        }
        _uiManager.StopGameButton.GetComponent<Button>().onClick.AddListener(QuitGame);
    }
    
    private void Start()
    {
        _leaderBoard.OnChange += OnChangeLeaderBoard;
        FindFirstObjectByType<SceneContext>().Container.Inject(this);
        if(NetworkServer.active && NetworkClient.active) timer = startTimer;
    }
    
    public void StartGame()
    {
        if (isGameStarted) return;

        isGameStarted = true;
        
        CmdStartGame();
    }

    [Command(requiresAuthority = false)]
    private void CmdStartGame()
    {
        StartCoroutine(StartTimer());
        isGameStarted = true;
        RpcStartGame();
    }

    [ClientRpc]
    private void RpcStartGame()
    {
        _uiManager.LobbyUI.SetActive(false);
        _uiManager.GamelayUI.SetActive(true);
        
        if(_player != null) _player.StartGame();
    }

    private void OnChangeLeaderBoard(SyncDictionary<string, int>.Operation o, string playerName, int count)
    {
        var sorted = _leaderBoard.OrderBy(pair => pair.Value).Reverse();

        string text = "Игрок: Кол-во убийств\n";
        foreach (var pair in sorted)
        {
            text += $"{pair.Key}: {pair.Value}\n";
        }

        _leaderBoardText.text = text;
    }

    public void AddPlayer(string playerName)
    {
        if (!_leaderBoard.ContainsKey(playerName))
        {
            CmdAddPlayer(playerName);
        }
        else
        {
            Debug.LogWarning($"Player '{playerName}' is already in the dictionary.");
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdAddPlayer(string playerName)
    {
        _leaderBoard.Add(playerName, 0);
    }

    public void KilledPlayer(string playerName)
    {
        if (!isGameStarted || !_leaderBoard.ContainsKey(playerName)) return;
        
        CmdKilledPlayer(playerName);
    }

    [Command(requiresAuthority = false)]
    private void CmdKilledPlayer(string playerName)
    {
        _leaderBoard[playerName]++;
    }

    private IEnumerator StartTimer()
    {
        if (!isServer) yield break;
        
        while (timer > 0)
        {
            yield return new WaitForSeconds(1);
            timer--;
        }

        isGameStarted = false;
        CmdStopGame();
        yield return new WaitForSeconds(1);
        
        CmdOpenWinPanel();
    }

    [Command(requiresAuthority = false)]
    private void CmdStopGame()
    {
        RpcStopGame();
    }

    [ClientRpc]
    private void RpcStopGame()
    {
        _player.StopGame();
    }

    [Command(requiresAuthority = false)]
    private void CmdOpenWinPanel()
    {
        var maxPlayer = _leaderBoard.OrderBy(pair => pair.Value).Last();
        if(maxPlayer.Value != 0)
            RpcOpenWinPanel(maxPlayer.Key);
        else
            RpcOpenWinPanel("-");
    }

    [ClientRpc]
    private void RpcOpenWinPanel(string winPlayer)
    {
        _uiManager.DeadPanel.SetActive(false);
        _uiManager.WinPlayerText.text = winPlayer;
        _uiManager.WinPanel.SetActive(true);
        
        if(NetworkServer.active && NetworkClient.active) _uiManager.BackToMenuButton.onClick.AddListener(() => NetworkManager.singleton.StopHost());
        else if(NetworkClient.active) _uiManager.BackToMenuButton.onClick.AddListener(() => NetworkManager.singleton.StopClient());
    }

    private void QuitGame()
    {
        if(NetworkServer.active && NetworkClient.active) NetworkManager.singleton.StopHost();
        else if(NetworkClient.active) NetworkManager.singleton.StopClient();
    }
}
