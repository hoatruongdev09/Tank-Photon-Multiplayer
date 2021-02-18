using System.Data.Common;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class NetworkPhotonManager : MonoBehaviourPunCallbacks {
    private void Start () {
        PhotonNetwork.NickName = $"Player: {Random.Range(0, 9999)}";
        PhotonNetwork.GameVersion = "0.0.0";
        PhotonNetwork.ConnectUsingSettings ();
    }

    public override void OnConnectedToMaster () {
        PhotonNetwork.JoinLobby ();
        Debug.Log ("Connect to server");
    }
    public override void OnJoinedLobby () {
        Debug.Log ("Lobby joined");
    }
    public override void OnDisconnected (DisconnectCause cause) {
        base.OnDisconnected (cause);
        Debug.Log ("Disconnected to server");
    }
}