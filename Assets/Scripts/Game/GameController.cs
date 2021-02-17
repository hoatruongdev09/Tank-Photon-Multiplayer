using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Game;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameController : MonoBehaviourPunCallbacks {
    private readonly List<TankInfo> listTanks = new List<TankInfo> ();
    private TankInfo localTankInfo;
    public static GameController Instance { get; private set; }

    private void Awake () {
        if (Instance == null) Instance = this;
    }

    public override void OnEnable () {
        PhotonNetwork.NetworkingClient.EventReceived += OnGameStart;
        PhotonNetwork.NetworkingClient.EventReceived += OnTankDestroy;
        PhotonNetwork.NetworkingClient.EventReceived += OnGameBegin;
    }

    public override void OnDisable () {
        PhotonNetwork.NetworkingClient.EventReceived -= OnGameStart;
        PhotonNetwork.NetworkingClient.EventReceived -= OnTankDestroy;
        PhotonNetwork.NetworkingClient.EventReceived -= OnGameBegin;
    }

    private void OnGameBegin (EventData obj) {
        if (obj.Code != GameEventDefine.GAME_BEGIN) return;
        Debug.Log ("game begin");
    }

    private void OnTankDestroy (EventData obj) {
        if (obj.Code != GameEventDefine.TANK_DESTROY) return;
        if (!PhotonNetwork.IsMasterClient) return;
        var data = (object[]) obj.CustomData;
        Debug.Log("on tank destroy event");
        var player = (Player) data[0];
        var tankViewID = (int) data[1];
        StartCoroutine (RespawnTank (tankViewID, 3));
    }

    private void OnGameStart (EventData photonEvent) {
        if (photonEvent.Code != GameEventDefine.START_GAME) return;
        StartGame ();
    }

    public void StartGame () {
        Debug.Log ("Game start");
        CreatePlayer ();
    }

    private IEnumerator RespawnTank (int tankID, float time) {
        yield return new WaitForSeconds (time);
        photonView.RPC ("RpcEnableTank", RpcTarget.All, tankID);
    }

    private void CreatePlayer () {
        var tank = PhotonNetwork.Instantiate ("Tank", new Vector3 (Random.Range (0f, 6f), 0, Random.Range (0f, 6f)),
            Quaternion.identity);
        if(!photonView.IsMine) return;
        localTankInfo = tank.GetComponent<TankInfo> ();
        photonView.RPC ("RpcPlayerReady", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer,
            tank.GetPhotonView ().ViewID);
    }

    [PunRPC]
    private void RpcEnableTank (int tankViewID) {
        var tank = PhotonView.Find(tankViewID);
        if(tank == null) return;
        tank.gameObject.SetActive(true);
    }

    [PunRPC]
    private void RpcPlayerReady (Player player, int tankViewID) {
        if (!PhotonNetwork.IsMasterClient) return;
        var tank = PhotonView.Find (tankViewID)?.GetComponent<TankInfo> ();
        tank.ownPlayer = player;
        listTanks.Add (tank);
        OnPlayerReady ();
    }

    private void OnPlayerReady () {
        if (!PhotonNetwork.IsMasterClient) return;
        if (listTanks.Count != PhotonNetwork.CurrentRoom.MaxPlayers) return;
        StartCoroutine (DelayToStartGame (3));
    }

    private IEnumerator DelayToStartGame (float time) {
        yield return new WaitForSeconds (time);
        Debug.Log ("All player ready");
        PhotonNetwork.RaiseEvent (GameEventDefine.GAME_BEGIN, new object[] { }, RaiseEventOptions.Default,
            SendOptions.SendReliable);
    }
}