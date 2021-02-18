using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;

public class LeaderboardOverView : MonoBehaviourPunCallbacks {
    [SerializeField] private LeaderboardRecordView _recordViewPrefab;
    [SerializeField] private Transform _recordContent; 
    private List<LeaderboardRecordView> _listRecords = new List<LeaderboardRecordView>();
    public override void OnJoinedRoom() {
        CreateRecord(PhotonNetwork.LocalPlayer);
        UpdateBoard();
    }


    public override void OnPlayerEnteredRoom(Player newPlayer) {
        CreateRecord(newPlayer);
        UpdateBoard();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        RemoveRecord(otherPlayer);
        UpdateBoard();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) {
        if(changedProps.ContainsKey(PunPlayerScores.PlayerScoreProp)) UpdateBoard();
    }
    private LeaderboardRecordView CreateRecord(Player localPlayer) {
        var record = Instantiate(_recordViewPrefab, _recordContent);
        record.SetPlayer(localPlayer);
        _listRecords.Add(record);
        return record;
    }

    private int RemoveRecord(Player targetPlayer) {
        var targetRecord = _listRecords.Find(x => x.Player == targetPlayer);
        if (targetRecord == null) return 0;
        _listRecords.Remove(targetRecord);
        Destroy(targetRecord.gameObject);
        return 1;
    }
    public void UpdateBoard() {
        foreach (var targetPlayer in PhotonNetwork.CurrentRoom.Players.Values) {
            var targetEntry = _listRecords.Find(record => record.Player == targetPlayer);
            if (targetEntry == null) {
                targetEntry = CreateRecord(targetPlayer);
            }
            targetEntry.UpdateScore();
        }
        SortRecord();
    }

    private void SortRecord() {
        _listRecords.Sort((a,b)=>b.Score.CompareTo(a.Score));
        for (int i = 0; i < _listRecords.Count; i++) {
            _listRecords[i].SetIndex(i);
        }
    }
}