using UnityEngine;
using GameAI.Environment;

namespace GameAI
{
    public class PuckController : MonoBehaviour
    {
        public ArenaSetup arenaSetup;
        public Rigidbody rb;
        public bool isTrainingMode = true; // Toggle cho training/gameplay
        public bool disableAutoReset = true; // Tắt auto reset
        private Vector3 startPosition;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            // Lưu vị trí ban đầu
            startPosition = transform.position;
        }

        private void Start()
        {
            // Chỉ auto-reset khi KHÔNG phải training mode VÀ không tắt auto reset
            if (!isTrainingMode && !disableAutoReset && arenaSetup != null)
            {
                arenaSetup.ResetPositions(-1);
            }
            else
            {
                Debug.Log($"PuckController: Keeping initial position {startPosition}");
            }
        }

        public void ResetPuck(int lastGoalPlayer)
        {
            if (arenaSetup != null)
            {
                arenaSetup.ResetPositions(lastGoalPlayer);
            }
            
            // Đảm bảo puck dừng hoàn toàn
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        // Reset đơn giản chỉ vị trí và velocity (cho training)
        public void ResetForTraining(Vector3 position)
        {
            transform.position = position;
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        // Reset về vị trí ban đầu
        public void ResetToStart()
        {
            transform.position = startPosition;
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}