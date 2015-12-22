using System;
using System.Collections;
using Akka.Interfaced.SlimSocket.Client;
using DG.Tweening;
using Domain;
using EntityNetwork;
using TypeAlias;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameScene : MonoBehaviour, IUserPairingObserver, IGameObserver, IByteChannel
{
    public RectTransform LoadingPanel;
    public RectTransform GamePanel;

    public Text LoadingText;
    public Transform GameEntityRoot;
    public RectTransform ResultBox;
    public Text ResultText;

    private Tuple<long, string> _pairedGame;
    private int _gameObserverId;
    private ObserverEventDispatcher _gameObserver;
    private GameClientRef _gameClient;
    private int _gameClientId;
    private GameInfo _gameInfo;
    private ClientZone _zone;
    private ProtobufChannelToClientZoneInbound _zoneChannel;

    protected void Start()
    {
        ClientEntityFactory.Default.RootTransform = GameEntityRoot;

        ApplicationComponent.TryInit();
        UiManager.Initialize();

        StartJoinGame();
    }

    protected void Update()
    {
        if (_zone != null)
        {
            // make gameObserver work in main thread
            _gameObserver.Pending = false;
            _gameObserver.Pending = true;
        }
    }

    private void StartJoinGame()
    {
        LoadingPanel.gameObject.SetActive(true);
        GamePanel.gameObject.SetActive(false);

        StartCoroutine(G.User == null
                           ? ProcessLoginAndJoinGame()
                           : ProcessJoinGame());
    }

    private IEnumerator ProcessLoginAndJoinGame()
    {
        var loginId = PlayerPrefs.GetString("LoginId");
        var loginPassword = PlayerPrefs.GetString("LoginPassword");

        // TEST
        loginId = "editor";
        loginPassword = "1234";

        if (string.IsNullOrEmpty(loginId))
        {
            UiMessageBox.ShowMessageBox("Cannot find id");
            yield break;
        }

        yield return StartCoroutine(ProcessLoginUser(loginId, loginPassword));
        if (G.User == null)
        {
            UiMessageBox.ShowMessageBox("Failed to login");
            yield break;
        }

        yield return StartCoroutine(ProcessJoinGame());
    }

    private IEnumerator ProcessLoginUser(string id, string password)
    {
        G.Logger.Info("ProcessLoginUser");

        var task = LoginProcessor.Login(G.ServerEndPoint, id, password, null);
        yield return task.WaitHandle;
    }

    private IEnumerator ProcessJoinGame()
    {
        G.Logger.Info("ProcessJoinGame");

        // Finding Game
        // Register user to pairing queue and waiting for 5 secs.

        LoadingText.text = "Finding Game...";

        _pairedGame = null;

        var observerId = G.Comm.IssueObserverId();
        G.Comm.AddObserver(observerId, new ObserverEventDispatcher(this));
        yield return G.User.RegisterPairing(G.GameDifficulty, observerId).WaitHandle;

        var startTime = DateTime.Now;
        while ((DateTime.Now - startTime).TotalSeconds < 5 && _pairedGame == null)
        {
            yield return null;
        }

        G.Comm.RemoveObserver(observerId);
        if (_pairedGame == null)
        {
            yield return G.User.UnregisterPairing().WaitHandle;
            var box = UiMessageBox.ShowMessageBox("Cannot find game");
            yield return StartCoroutine(box.WaitForHide());
            SceneManager.LoadScene("MainScene");
            yield break;
        }

        // Join Game

        var roomId = _pairedGame.Item1;
        var observerId2 = G.Comm.IssueObserverId();
        _gameObserver = new ObserverEventDispatcher(this, startPending: true, keepOrder: true);
        G.Comm.AddObserver(observerId2, _gameObserver);
        var joinRet = G.User.JoinGame(roomId, observerId2);
        yield return joinRet.WaitHandle;

        if (joinRet.Exception != null)
        {
            UiMessageBox.ShowMessageBox("Failed to join\n" + joinRet.Exception);
            G.Comm.RemoveObserver(observerId2);
            _gameObserver = null;
            yield break;
        }

        _gameObserverId = observerId2;
        _gameClient = new GameClientRef(new SlimActorRef(joinRet.Result.Item1), G.SlimRequestWaiter, null);
        _gameClientId = joinRet.Result.Item2;
        _gameInfo = joinRet.Result.Item3;

        _zone = new ClientZone(
            ClientEntityFactory.Default,
            new ProtobufChannelToServerZoneOutbound
            {
                TypeTable = new TypeAliasTable(),
                TypeModel = new DomainProtobufSerializer(),
                OutboundChannel = this
            });

        _zoneChannel = new ProtobufChannelToClientZoneInbound
        {
            TypeTable = new TypeAliasTable(),
            TypeModel = new DomainProtobufSerializer(),
            InboundClientZone = _zone
        };

        LoadingText.text = "Waiting for " + _pairedGame.Item2 + "...";
    }

    void IUserPairingObserver.MakePair(long gameId, string opponentName)
    {
        Debug.Log(string.Format("IUserPairingObserver.MakePair {0} {1}", gameId, opponentName));
        _pairedGame = Tuple.Create(gameId, opponentName);
    }

    void IGameObserver.Join(long userId, string userName, int clientId)
    {
        Debug.Log(string.Format("IGameObserver.Join {0} {1} {2}", userId, userName, clientId));
    }

    void IGameObserver.Leave(long userId)
    {
        Debug.Log(string.Format("IGameObserver.Leave {0}", userId));
    }

    void IGameObserver.ZoneMessage(byte[] bytes)
    {
        Debug.Log(string.Format("IGameObserver.ZoneMessage {0}", bytes.Length));
        _zoneChannel.Write(bytes);
    }

    void IGameObserver.Begin()
    {
        Debug.Log(string.Format("IGameObserver.Begin"));
        BeginGame();
    }

    void IGameObserver.End(int winnerId)
    {
        Debug.Log(string.Format("IGameObserver.End {0}", winnerId));
        EndGame(winnerId);
    }

    void IGameObserver.Abort()
    {
        Debug.Log(string.Format("IGameObserver.Abort"));
        EndGame(-1);
    }

    void IByteChannel.Write(byte[] bytes)
    {
        _gameClient.ZoneMessage(bytes);
    }

    private void BeginGame()
    {
        LoadingPanel.gameObject.SetActive(false);
        GamePanel.gameObject.SetActive(true);
        ResultBox.gameObject.SetActive(false);
    }

    private void EndGame(int winnerId)
    {
        ResultBox.gameObject.SetActive(true);
        var ap = ResultBox.anchoredPosition;
        ResultBox.anchoredPosition = new Vector2(ap.x, ap.y - 200);
        ResultBox.DOAnchorPosY(ap.y, 0.5f).SetEase(Ease.OutBounce).SetDelay(0);

        if (winnerId == 0)
        {
            ResultText.text = "Draw";
        }
        else if (winnerId == -1)
        {
            ResultText.text = "Abort";
        }
        else
        {
            var snake = (ClientSnake)_zone.GetEntity(winnerId);
            if (snake == null)
                ResultText.text = "?";
            else if (snake.IsControllable)
                ResultText.text = "WIN";
            else
                ResultText.text = "LOSE";
        }
    }

    public void OnLeaveButtonClick()
    {
        if (_gameInfo != null)
        {
            G.User.LeaveGame(_gameInfo.Id);
            G.Comm.RemoveObserver(_gameObserverId);
        }

        SceneManager.LoadScene("MainScene");
    }
}
