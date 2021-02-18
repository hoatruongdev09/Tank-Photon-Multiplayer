using UnityEngine;

namespace Game {
    public class CameraControl : MonoBehaviour {
        public static CameraControl Instance { get; private set; }
        public Transform TargetFocus {
            set => targetFocus = value;
        }
        private Vector3 TargetPosition {
            get => targetFocus.position;
        }
        [SerializeField] private Transform targetFocus;
        [SerializeField] private float lerpValue = 2;

        private void Start() {
            if (Instance == null) Instance = this;
        }
        private void Update() {
            if(targetFocus == null) return;
            transform.position = Vector3.Lerp(transform.position,
                new Vector3(TargetPosition.x, 0, TargetPosition.z), lerpValue * Time.deltaTime);
        }
    }
}