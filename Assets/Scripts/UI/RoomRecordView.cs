using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class RoomRecordView : MonoBehaviourPunCallbacks {
    [SerializeField] private Text _textName;
    [SerializeField] private Button _buttonJoin;

    private RoomInfo _roomInfo;

    private void Start() {
        _buttonJoin.onClick.AddListener(OnJoinRoomClicked);
    }

    private void OnJoinRoomClicked() {
        if (!PhotonNetwork.IsConnected) return;
        if (!_roomInfo.IsOpen) return;
        if(_roomInfo.PlayerCount == _roomInfo.MaxPlayers) return;
        PhotonNetwork.JoinRoom(_roomInfo.Name);
    }

    public void SetRoomInfo(RoomInfo info) {
        _roomInfo = info;
        _textName.text = $"{_roomInfo.Name} - {_roomInfo.PlayerCount}/{_roomInfo.MaxPlayers} ";
    }
}