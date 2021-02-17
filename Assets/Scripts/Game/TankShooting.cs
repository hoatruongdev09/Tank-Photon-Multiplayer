using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Game {
    public class TankShooting : MonoBehaviourPun, IOnEventCallback {
        public int m_PlayerNumber = 1;
        public Rigidbody m_Shell;
        public Transform m_FireTransform;
        public Slider m_AimSlider;
        public AudioSource m_ShootingAudio;
        public AudioClip m_ChargingClip;
        public AudioClip m_FireClip;
        public float m_MinLaunchForce = 15f;
        public float m_MaxLaunchForce = 30f;
        public float m_MaxChargeTime = 0.75f;

        private string m_FireButton;
        private float m_CurrentLaunchForce;
        private float m_ChargeSpeed;
        private bool m_Fired;
        private bool canControl = false;
        private void OnEnable () {
            PhotonNetwork.AddCallbackTarget(this);
            m_CurrentLaunchForce = m_MinLaunchForce;
            m_AimSlider.value = m_MinLaunchForce;
        }

        private void OnDisable() {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        private void OnGameStart(EventData obj) {
            if (obj.Code != GameEventDefine.GAME_BEGIN) return;
            canControl = true;
        }

        private void Start () {
            m_FireButton = "Fire" + m_PlayerNumber;
            m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
        }

        private void Update () {
            if (!photonView.IsMine) { return; }
            // if(!canControl) return;
            m_AimSlider.value = m_MinLaunchForce;

            if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired) {

                m_CurrentLaunchForce = m_MaxLaunchForce;
                Fire ();
            } else if (Input.GetButtonDown (m_FireButton)) {

                m_Fired = false;
                m_CurrentLaunchForce = m_MinLaunchForce;

                m_ShootingAudio.clip = m_ChargingClip;
                m_ShootingAudio.Play ();
            } else if (Input.GetButton (m_FireButton) && !m_Fired) {

                m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

                m_AimSlider.value = m_CurrentLaunchForce;
            } else if (Input.GetButtonUp (m_FireButton) && !m_Fired) {

                Fire ();
            }

        }
        private void Fire () {
            if (!photonView.IsMine) { return; }
            m_Fired = true;
            // Rigidbody shellInstance = Instantiate (m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
            Rigidbody shellInstance = PhotonNetwork.Instantiate ("TankShell", m_FireTransform.position, m_FireTransform.rotation).GetComponent<Rigidbody> ();
            shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;

            m_ShootingAudio.clip = m_FireClip;
            m_ShootingAudio.Play ();

            m_CurrentLaunchForce = m_MinLaunchForce;
        }

        public void OnEvent(EventData photonEvent) {
            OnGameStart(photonEvent);
        }
    }
}