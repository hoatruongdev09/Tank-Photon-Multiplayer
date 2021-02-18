using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardRecordView : MonoBehaviour {
    [SerializeField] private Text _textRankIndex;
    [SerializeField] private Text _textPlayerName;
    [SerializeField] private Text _textScore;
    
    
    public int Score { get; private set; }
    public Player Player { get; private set; }
    
    public void SetPlayer(Player player) {
        Player = player;
        Score = player.GetScore();
        _textPlayerName.text = player.NickName;
        _textScore.text = player.GetScore().ToString();
    }

    public void UpdateScore() {
        Score = Player.GetScore();
        _textPlayerName.text = Player.NickName;
        _textScore.text = Player.GetScore().ToString();
    }
    
    public void SetIndex(int index) {
        _textRankIndex.text = $"{index + 1}";
        transform.SetSiblingIndex(index);
    }
}