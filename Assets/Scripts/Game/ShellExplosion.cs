using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Game {
    public class ShellExplosion : MonoBehaviourPun {
        public LayerMask m_TankMask;
        public ParticleSystem m_ExplosionParticles;
        public AudioSource m_ExplosionAudio;
        public float m_MaxDamage = 100f;
        public float m_ExplosionForce = 1000f;
        public float m_MaxLifeTime = 2f;
        public float m_ExplosionRadius = 5f;
        public Player fromPlayer;

        private void Start() {
            Destroy(gameObject, m_MaxLifeTime);
        }

        private void OnTriggerEnter(Collider other) {
            var colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);
            for (var i = 0; i < colliders.Length; i++) {
                var targetRigidbody = colliders[i].GetComponent<Rigidbody>();
                if (!targetRigidbody) continue;
                var photonView = targetRigidbody.GetComponent<PhotonView>();
                if (photonView != null)
                    AddTargetForce(photonView.ViewID, m_ExplosionForce, transform.position, m_ExplosionRadius);
                // targetRigidbody.AddExplosionForce (m_ExplosionForce, transform.position, m_ExplosionRadius);
                var targetHealth = targetRigidbody.GetComponent<TankHealth>();
                if (!targetHealth) continue;
                var damage = CalculateDamage(targetRigidbody.position);
                targetHealth.TakeDamage(fromPlayer, damage);
            }

            m_ExplosionParticles.transform.parent = null;

            m_ExplosionParticles.Play();

            m_ExplosionAudio.Play();

            var mainModule = m_ExplosionParticles.main;
            Destroy(m_ExplosionParticles.gameObject, mainModule.duration);
            Destroy(gameObject);
        }

        private float CalculateDamage(Vector3 targetPosition) {
            var explosionToTarget = targetPosition - transform.position;
            var explosionDistance = explosionToTarget.magnitude;
            var relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;
            var damage = relativeDistance * m_MaxDamage;
            damage = Mathf.Max(0f, damage);
            return damage;
        }

        [PunRPC]
        private void RpcAddTargetForce(int viewID, float force, float[] position, float radius) {
            var view = PhotonView.Find(viewID);
            if(view == null) return;
            var rigBody = view.GetComponent<Rigidbody>();
            if (rigBody == null) return;
            var newPosition = new Vector3(position[0], position[1], position[2]);
            rigBody.AddExplosionForce(force, newPosition, radius);
        }

        private void AddTargetForce(int viewID, float force, Vector3 position, float radius) {
            var newPosition = new[] {position.x, position.y, position.z};
            photonView.RPC("RpcAddTargetForce", RpcTarget.All, viewID, force, newPosition, radius);
        }
    }
}