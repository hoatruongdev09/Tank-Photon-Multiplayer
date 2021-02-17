using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRecordView : MonoBehaviourPunCallbacks {
    [SerializeField] private Text textPlayerName;
    [SerializeField] private Button _buttonKick;
    [SerializeField] private GameObject _readyStatus;
    [SerializeField] private GameObject _hostStatus;

    public Player Player { get; private set; }

    public bool IsReady { get; private set; }

    public void SetPlayerInfo (Player targetPlayer) {
        Player = targetPlayer;

        textPlayerName.text = targetPlayer.NickName;
        _hostStatus.SetActive (targetPlayer.IsMasterClient);
        _buttonKick.gameObject.SetActive (PhotonNetwork.IsMasterClient && !Player.IsMasterClient);
        _readyStatus.gameObject.SetActive (false);
    }

    public void SetReady (bool ready) {
        IsReady = ready;
        _readyStatus.SetActive (IsReady);
    }
}