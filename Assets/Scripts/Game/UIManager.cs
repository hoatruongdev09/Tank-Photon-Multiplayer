using System;
using ExitGames.Client.Photon;
using Game;
using Photon.Pun;
using UnityEngine;

public class UIManager : MonoBehaviourPunCallbacks {
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private PanelJoinRoom _panelJoinRoom;
    [SerializeField] private PanelCreateRoom _panelCreateRoom;
    [SerializeField] private PanelInRoom _panelInRoom;
    [SerializeField] private PanelInGame _panelInGame;
    private void Awake () {
        if (Instance == null) { Instance = this; }
    }
    public override void OnEnable () {
        base.OnEnable ();
        PhotonNetwork.NetworkingClient.EventReceived += OnStartGame;
    }
    private void Start () {
        loadingPanel.SetActive (true);
        _panelJoinRoom.onBackClick.AddListener (OnBackToMenu);
        _panelInRoom.onBackClicked.AddListener (OnBackToLobby);
    }
    public override void OnDisable () {
        base.OnDisable ();
        PhotonNetwork.NetworkingClient.EventReceived += OnStartGame;
    }

    private void OnStartGame (EventData photonEvent) {
        if (photonEvent.Code != GameEventDefine.START_GAME) { return; }
        StartGame ();
    }
    public void StartGame () {
        _panelInRoom.gameObject.SetActive (false);
        _panelInGame.gameObject.SetActive(true);
    }
    private void OnBackToLobby () {
        PhotonNetwork.LeaveRoom (true);
    }

    private void OnBackToMenu () {

    }

    public override void OnConnectedToMaster () {
        loadingPanel.SetActive (false);
    }

    public override void OnJoinedLobby () {
        Debug.Log ($"Lobby joined UI");
        loadingPanel.gameObject.SetActive (false);
        _panelInGame.gameObject.SetActive(false);
        _panelJoinRoom.gameObject.SetActive (true);
    }

    public override void OnJoinedRoom () {
        _panelInRoom.gameObject.SetActive (true);
        _panelCreateRoom.gameObject.SetActive (false);
        _panelJoinRoom.gameObject.SetActive (false);
        _panelInGame.gameObject.SetActive(false);
    }

    public override void OnLeftRoom () {
        _panelInRoom.gameObject.SetActive (false);
        _panelCreateRoom.gameObject.SetActive (false);
        _panelJoinRoom.gameObject.SetActive (false);
        _panelInGame.gameObject.SetActive(false);
    }
}