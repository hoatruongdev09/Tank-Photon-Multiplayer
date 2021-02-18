using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameController : MonoBehaviourPunCallbacks {
    [SerializeField] private Transform[] spawnHolders;
    [SerializeField] private Transform playgroundHolder;
    private List<Vector3> spawnPositions;
    public static GameController Instance { get; private set; }

    private void Awake() {
        if (Instance == null) Instance = this;
    }

    public override void OnEnable() {
        PhotonNetwork.NetworkingClient.EventReceived += OnGameStart;
        PhotonNetwork.NetworkingClient.EventReceived += OnTankDestroy;
        PhotonNetwork.NetworkingClient.EventReceived += OnGameBegin;
    }

    public override void OnDisable() {
        PhotonNetwork.NetworkingClient.EventReceived -= OnGameStart;
        PhotonNetwork.NetworkingClient.EventReceived -= OnTankDestroy;
        PhotonNetwork.NetworkingClient.EventReceived -= OnGameBegin;
    }

    private void OnGameBegin(EventData obj) {
        if (obj.Code != GameEventDefine.GAME_BEGIN) return;
        Debug.Log("game begin");
    }

    private void OnTankDestroy(EventData obj) {
        if (obj.Code != GameEventDefine.TANK_DESTROY) return;
        if (!PhotonNetwork.IsMasterClient) return;
        var data = (object[]) obj.CustomData;
        Debug.Log("on tank destroy event");
        // var player = (Player) data[0];
        var tankViewID = (int) data[1];
        StartCoroutine(RespawnTank(tankViewID, 5));
    }

    private void OnGameStart(EventData photonEvent) {
        if (photonEvent.Code != GameEventDefine.START_GAME) return;
        FindObjectOfType<LeaderboardOverView>().UpdateBoard();
        playgroundHolder.gameObject.SetActive(true);
        StartGame();
    }

    public void StartGame() {
        if (!PhotonNetwork.IsMasterClient) return;
        spawnPositions = spawnHolders.Select(holder => holder.position).ToList();
        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values) {
            var randomIndex = Random.Range(0, spawnPositions.Count);
            CreateTankForPlayer(player, spawnPositions[randomIndex], Quaternion.identity);
            spawnPositions.RemoveAt(randomIndex);
        }

        // CreatePlayer ();
    }

    private IEnumerator RespawnTank(int tankID, float time) {
        yield return new WaitForSeconds(time);
        photonView.RPC("RpcEnableTank", RpcTarget.All, tankID);
    }
    [PunRPC]
    private void RpcCreateTankForPlayer( float[] position, float[] rotation) {
        Vector3 receivedPosition = new Vector3(position[0],position[1],position[2]);
        Quaternion receivedRotation = Quaternion.Euler(rotation[0],rotation[1],rotation[2]);
        // if(PhotonNetwork.LocalPlayer != player) return;
        var tank = PhotonNetwork.Instantiate("Tank", receivedPosition, receivedRotation);
    }
    private void CreateTankForPlayer(Player player, Vector3 position, Quaternion rotation) {
        var sendPosition = new float[] {position.x, position.y, position.z};
        var sendRotation = new float[] {rotation.x, rotation.y, rotation.z};
        photonView.RPC("RpcCreateTankForPlayer",player,sendPosition,sendRotation);
    }

    [PunRPC]
    private void RpcEnableTank(int tankViewID) {
        var tank = PhotonView.Find(tankViewID);
        if (tank == null) return;
        var spawnPosition = spawnHolders[Random.Range(0, spawnHolders.Length)].position;
        tank.transform.position = spawnPosition;
        tank.gameObject.SetActive(true);
    }
}