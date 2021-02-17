using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class PanelCreateRoom : MonoBehaviour {
    [SerializeField] private Button _buttonCancel;
    [SerializeField] private Button _buttonCreate;

    [SerializeField] private InputField _textRoomName;
    [SerializeField] private InputField _textMaxPlayers;

    private void Start () {
        _buttonCancel.onClick.AddListener (OnCancelClicked);
        _buttonCreate.onClick.AddListener (OnCreateRoomClicked);
    }

    private void OnCreateRoomClicked () {
        if (!PhotonNetwork.IsConnected) return;
        var roomOptions = new RoomOptions ();
        roomOptions.MaxPlayers = Convert.ToByte (_textMaxPlayers.text);
        roomOptions.PlayerTtl = 0;
        Debug.Log ($"create room max player: {roomOptions.MaxPlayers}");
        PhotonNetwork.CreateRoom (_textRoomName.text, roomOptions, TypedLobby.Default);
    }

    private void OnCancelClicked () {
        gameObject.SetActive (false);
    }
}