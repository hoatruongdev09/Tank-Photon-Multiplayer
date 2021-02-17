using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Game {
    public class TankHealth : MonoBehaviourPun {
        public float m_StartingHealth = 100f; 
        public Slider m_Slider; 
        public Image m_FillImage; 
        public Color m_FullHealthColor = Color.green; 
        public Color m_ZeroHealthColor = Color.red; 

        public GameObject m_ExplosionPrefab; 

        private float m_CurrentHealth; 
        private bool m_Dead; 


        private AudioSource m_ExplosionAudio; 
        private ParticleSystem m_ExplosionParticles;

        private void Awake() {
            m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
            m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();
            m_ExplosionParticles.gameObject.SetActive(false);
        }

        private void OnEnable() {
            m_CurrentHealth = m_StartingHealth;
            m_Dead = false;
        }

        public void TakeDamage(float amount) {
            photonView.RPC("RpcTakeDamage",RpcTarget.All,photonView.ViewID,amount);
        }
        [PunRPC]
        private void RpcTakeDamage(int viewID, float amount) {
            if(viewID != this.photonView.ViewID) return;
            m_CurrentHealth -= amount;
            SetHealthUI();
            if(m_CurrentHealth<=0 && !m_Dead) OnDeath();
        }
        private void SetHealthUI() {
            m_Slider.value = m_CurrentHealth;
            m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
        }
        private void OnDeath() {
            m_Dead = true;
            m_ExplosionParticles.transform.position = transform.position;
            m_ExplosionParticles.gameObject.SetActive(true);
            m_ExplosionParticles.Play();
            m_ExplosionAudio.Play();
            gameObject.SetActive(false);
            var tankDestroyData = new object[] {PhotonNetwork.LocalPlayer, photonView.ViewID};
            var raiseOptions = new RaiseEventOptions() {Receivers = ReceiverGroup.All};
            PhotonNetwork.RaiseEvent(GameEventDefine.TANK_DESTROY, tankDestroyData, raiseOptions,
                SendOptions.SendReliable);
        }
    }
}