
using System.Collections;
using System.Data.Common;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Game {
    public class PanelInGame : MonoBehaviourPunCallbacks {
        [SerializeField] private LeaderboardOverView _leaderboardOverView;
        [SerializeField] private Text _textTextRespawning;
        [SerializeField] private Image _imageCountdown;

        public override void OnEnable() {
            PhotonNetwork.NetworkingClient.EventReceived += OnShowTankRespawning;
        }

        public override void OnDisable() {
            PhotonNetwork.NetworkingClient.EventReceived -= OnShowTankRespawning;
        }

        private void OnShowTankRespawning(EventData obj) {
            if(obj.Code != GameEventDefine.TANK_DESTROY) return;
            var data = (object[]) obj.CustomData;
            var player = (Player) data[0];
            Debug.Log($"player: {player.NickName}");
            if(player != PhotonNetwork.LocalPlayer) return;
            ShowRespawningCountdown(5);
        }

        private void ShowRespawningCountdown(float time) {
            StartCoroutine(CountDownCoroutine(time));
        }

        private IEnumerator CountDownCoroutine(float time) {
            float currentTime = time;
            _textTextRespawning.gameObject.SetActive(true);
            _imageCountdown.gameObject.SetActive(true);
            while (currentTime > 0) {
                _textTextRespawning.text = $"RESPAWN IN: {Mathf.CeilToInt(currentTime)}";
                _imageCountdown.fillAmount = currentTime / time;
                currentTime -= Time.deltaTime;
                yield return null;
            }
            _textTextRespawning.gameObject.SetActive(false);
            _imageCountdown.gameObject.SetActive(false);
        }
    }
}