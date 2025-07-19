using UnityEngine;

namespace GameAI.Environment
{
    public class MidWallTrigger : MonoBehaviour
    {
        public float pushBackForce = 10f;
        public bool isPlayer1Side = true; // true nếu là sân Player 1, false nếu là sân Player 2

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Paddle"))
            {
                PushPaddleBack(other);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Paddle"))
            {
                PushPaddleBack(other);
            }
        }

        private void PushPaddleBack(Collider paddleCollider)
        {
            Rigidbody rb = paddleCollider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Xác định hướng đẩy về sân nhà
                Vector3 pushDirection;
                if (isPlayer1Side)
                {
                    // Đẩy về phía Z âm (sân Player 1)
                    pushDirection = Vector3.back;
                }
                else
                {
                    // Đẩy về phía Z dương (sân Player 2)
                    pushDirection = Vector3.forward;
                }

                // Đẩy paddle về sân nhà
                rb.linearVelocity = Vector3.zero; // Dừng chuyển động hiện tại
                rb.AddForce(pushDirection * pushBackForce, ForceMode.VelocityChange);
            }
        }
    }
}
