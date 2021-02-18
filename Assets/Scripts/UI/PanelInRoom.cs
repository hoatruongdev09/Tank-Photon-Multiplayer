using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class PanelInRoom : MonoBehaviourPunCallbacks {
    [SerializeField] private Button _buttonBack;
    [SerializeField] private Button _buttonReady;
    [SerializeField] private Text _textReadyButton;
    [SerializeField] private Text _textCountDown;

    [SerializeField] private GameObject _countDownHolder;

    [SerializeField] private PlayerRecordView _playerRecordPrefab;
    [SerializeField] private RectTransform playerContent;

    private readonly List<PlayerRecordView> listPlayer = new List<PlayerRecordView> ();
    private bool ready = false;

    private bool isLockCountDown = false;
    public Button.ButtonClickedEvent onBackClicked => _buttonBack.onClick;

    private void Awake () {
        ClearListPlayer ();
        UpdateListPlayer ();
    }
    public override void OnEnable () {
        base.OnEnable ();
        ClearListPlayer ();
        UpdateListPlayer ();
    }
    public override void OnDisable () {
        base.OnDisable ();
        ready = false;
        _textReadyButton.text = ready ? "CANCEL" : "READY";
    }

    private void Start () {
        _buttonBack.onClick.AddListener (OnBackClicked);
        _buttonReady.onClick.AddListener (OnReadyClicked);
    }

    private void OnBackClicked () { }

    private void OnReadyClicked () {
        ready = !ready;
        _textReadyButton.text = ready ? "CANCEL" : "READY";
        photonView.RpcSecure ("RpcReady", RpcTarget.All, false, PhotonNetwork.LocalPlayer, ready);
    }

    [PunRPC]
    private void RpcReady (Player targetPlayer, bool ready) {
        if (!ready) {
            isLockCountDown = false;
            StopCoroutine ("DelayToStartGame");
        }
        SetPlayerReady (targetPlayer, ready);
        if (PhotonNetwork.IsMasterClient) CheckAllPlayerIsReady ();
    }
    private void SetPlayerReady (Player targetPlayer, bool ready) {
        foreach (var player in listPlayer) {
            if (player.Player == targetPlayer) {
                player.SetReady (ready);
                return;
            }
        }
    }
    private void CheckAllPlayerIsReady () {
        var readyPlayer = listPlayer.Count (player => player.IsReady);
        Debug.Log ($"All player connect: {readyPlayer} - {PhotonNetwork.CurrentRoom.MaxPlayers}");
        if (readyPlayer == PhotonNetwork.CurrentRoom.MaxPlayers) {
            photonView.RpcSecure ("CountdownToStartGame", RpcTarget.All, false, 5);
        }
    }

    [PunRPC]
    private void CountdownToStartGame (int time) {
        StartCoroutine (DelayToStartGame (time, () => {
            if (PhotonNetwork.IsMasterClient) { RaiseStartGame (); }
        }));
    }
    private IEnumerator DelayToStartGame (int time, Action callback) {
        isLockCountDown = true;
        float countDownTime = time;
        _countDownHolder.gameObject.SetActive(true);
        while (countDownTime > 0 && isLockCountDown) {
            countDownTime -= Time.deltaTime;
            _textCountDown.text = $"GAME START IN {Mathf.CeilToInt(countDownTime)}";
            yield return null;
        }
        if (isLockCountDown) {
            isLockCountDown = false;
            callback ();
        }
        _countDownHolder.gameObject.SetActive(false);
    }
    private void RaiseStartGame () {
        Debug.Log ($"Raise event");
        // GameController.Instance.StartGame ();
        UIManager.Instance.StartGame ();
        var raiseOptions = new RaiseEventOptions() {Receivers = ReceiverGroup.All};
        PhotonNetwork.RaiseEvent(GameEventDefine.START_GAME, null, raiseOptions,
            SendOptions.SendReliable);
    }
    private void UpdateListPlayer () {
        foreach (var player in PhotonNetwork.CurrentRoom.Players) CreatePlayer (player.Value);
    }

    private void ClearListPlayer () {
        foreach (var player in listPlayer) Destroy (player.gameObject);
        listPlayer.Clear ();
    }

    private void CreatePlayer (Player player) {
        var playerGO = Instantiate (_playerRecordPrefab, playerContent);
        playerGO.SetPlayerInfo (player);
        listPlayer.Add (playerGO);
    }

    public override void OnPlayerEnteredRoom (Player newPlayer) {
        CreatePlayer (newPlayer);
    }

    public override void OnPlayerLeftRoom (Player otherPlayer) {
        var player = listPlayer.Find (pl => pl.Player == otherPlayer);
        if (player == null) return;
        isLockCountDown = false;
        StopCoroutine ("DelayToStartGame");
        listPlayer.Remove (player);
        Destroy (player.gameObject);
    }
}