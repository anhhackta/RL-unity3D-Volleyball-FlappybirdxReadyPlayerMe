using UnityEngine;

namespace GameAI.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerPaddle : MonoBehaviour
    {
        public float moveSpeed = 10f;
        private Rigidbody rb;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.constraints = RigidbodyConstraints.FreezePositionY | 
                             RigidbodyConstraints.FreezeRotationX | 
                             RigidbodyConstraints.FreezeRotationZ;
        }

        void FixedUpdate()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            // Di chuyển paddle
            Vector3 move = (Vector3.right * v + Vector3.forward * h) * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Puck"))
            {
                Rigidbody puckRb = collision.collider.GetComponent<Rigidbody>();
                Vector3 hitDir = (collision.transform.position - transform.position).normalized;
                puckRb.AddForce(hitDir * 5f, ForceMode.Impulse);
            }
        }
    }
}
