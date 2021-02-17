using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class PanelJoinRoom : MonoBehaviourPunCallbacks {
    public Button.ButtonClickedEvent onBackClick => _buttonBack.onClick;
    
    [SerializeField] private Button _buttonBack;
    [SerializeField] private Button _buttonCreateRoom;
    [SerializeField] private PanelCreateRoom _panelCreateRoom;
    [SerializeField] private RectTransform _roomContent;
    [SerializeField] private RoomRecordView _roomRecordPrefab;
    
    
    private List<RoomRecordView> listRoomRecords = new List<RoomRecordView>();

    private void Start() {
        _buttonBack.onClick.AddListener(OnBackToMainMenuClicked);
        _buttonCreateRoom.onClick.AddListener(OnCreateRoomClicked);
    }

    private void OnCreateRoomClicked() {
        _panelCreateRoom.gameObject.SetActive(true);
    }

    private void OnBackToMainMenuClicked() {
        
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        ClearRoomRecord();
        foreach (var info in roomList) {
            if(info.RemovedFromList) continue;
            var roomRecord = Instantiate(_roomRecordPrefab, _roomContent);
            roomRecord.SetRoomInfo(info);
            listRoomRecords.Add(roomRecord);
        }    
    }

    private void ClearRoomRecord() {
        foreach (var room in listRoomRecords) {
            Destroy(room.gameObject);
        }
        listRoomRecords.Clear();
    }
    
}
