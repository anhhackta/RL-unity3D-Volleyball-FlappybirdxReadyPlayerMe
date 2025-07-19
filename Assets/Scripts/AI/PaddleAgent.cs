using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

namespace GameAI.AI
{
    [RequireComponent(typeof(Rigidbody))]
    public class PaddleAgent : Agent
    {
        [Header("Training Setup")]
        public Transform puck;
        public Transform opponentPaddle; // Paddle đối thủ
        public int playerIndex; // 0: Player1, 1: Player2
        public bool isReverseDirection = false; // true nếu paddle ở phía đối diện
        public float moveSpeed = 10f;
        
        [Header("Training Arena")]
        public GameAI.GameLogic.GameManager gameManager;
        public bool disableAutoReset = true; // Tắt auto reset để giữ vị trí ban đầu
        
        private Rigidbody rb;
        private Vector3 startPosition;

        public override void Initialize()
        {
            rb = GetComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.constraints = RigidbodyConstraints.FreezePositionY |
                             RigidbodyConstraints.FreezeRotationX |
                             RigidbodyConstraints.FreezeRotationZ;
            
            // Lưu vị trí ban đầu (vị trí bạn đã đặt đúng)
            startPosition = transform.position;
            
            // Tìm GameManager nếu chưa có
            if (gameManager == null)
            {
                gameManager = FindFirstObjectByType<GameAI.GameLogic.GameManager>();
            }
            
            // Debug thông tin paddle
            Debug.Log($"PaddleAgent {playerIndex} initialized at position: {transform.position}, startPosition saved: {startPosition}");
        }

        public override void OnEpisodeBegin()
        {
            if (disableAutoReset)
            {
                // Chỉ reset về vị trí ban đầu, không gọi GameManager
                transform.position = startPosition;
                Debug.Log($"Agent {playerIndex} reset to saved startPosition: {startPosition}");
            }
            else
            {
                // Chỉ 1 agent reset toàn bộ game để tránh conflict
                if (playerIndex == 0 && gameManager != null)
                {
                    gameManager.ResetForTraining();
                }
            }
            
            // Reset velocity của paddle này
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            
            Debug.Log($"Episode Begin - Agent {playerIndex} at position: {transform.position}");
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            // Debug để kiểm tra agent có được gọi không
            Debug.Log($"Agent {playerIndex} CollectObservations called");
            
            // Vị trí và vận tốc của paddle này
            sensor.AddObservation(transform.localPosition);
            sensor.AddObservation(rb.linearVelocity);
            
            // Vị trí và vận tốc của puck
            sensor.AddObservation(puck.localPosition);
            sensor.AddObservation(puck.GetComponent<Rigidbody>().linearVelocity);
            
            // Vị trí của paddle đối thủ
            sensor.AddObservation(opponentPaddle.localPosition);
            
            // Khoảng cách tới puck
            float distanceToPuck = Vector3.Distance(transform.position, puck.position);
            sensor.AddObservation(distanceToPuck);
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            float moveX = actions.ContinuousActions[0];
            float moveZ = actions.ContinuousActions[1];
            
            // Debug movement input
            if (Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveZ) > 0.1f)
            {
                Debug.Log($"Agent {playerIndex} receiving actions: X={moveX:F2}, Z={moveZ:F2}");
            }
            
            // Thống nhất hướng di chuyển với PlayerPaddle (game ngang)
            Vector3 move;
            if (isReverseDirection)
            {
                // Paddle đối diện: đảo cả X và Z
                move = (Vector3.right * moveZ - Vector3.forward * moveX) * moveSpeed * Time.fixedDeltaTime;
            }
            else
            {
                // Paddle chính: giống PlayerPaddle
                move = (Vector3.right * moveZ + Vector3.forward * moveX) * moveSpeed * Time.fixedDeltaTime;
            }
            
            rb.MovePosition(rb.position + move);
            
            // Small negative reward để khuyến khích kết thúc game nhanh
            AddReward(-0.001f);
        }

        void FixedUpdate()
        {
            // Test manual movement cho debug
            if (Input.GetKey(KeyCode.T)) // Nhấn T để test manual
            {
                Vector3 testMove = Vector3.zero;
                
                if (playerIndex == 0) // Agent 0 dùng WASD - giống PlayerPaddle
                {
                    float h = Input.GetAxis("Horizontal");
                    float v = Input.GetAxis("Vertical");
                    testMove = (Vector3.right * v + Vector3.forward * h) * moveSpeed * Time.fixedDeltaTime;
                }
                else // Agent 1 dùng IJKL - ngược lại
                {
                    float h = (Input.GetKey(KeyCode.L) ? 1 : 0) - (Input.GetKey(KeyCode.J) ? 1 : 0);
                    float v = (Input.GetKey(KeyCode.I) ? 1 : 0) - (Input.GetKey(KeyCode.K) ? 1 : 0);
                    testMove = (Vector3.right * v - Vector3.forward * h) * moveSpeed * Time.fixedDeltaTime;
                }
                
                if (testMove != Vector3.zero)
                {
                    rb.MovePosition(rb.position + testMove);
                    Debug.Log($"Manual test move Agent {playerIndex}: {testMove}");
                }
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Puck"))
            {
                Rigidbody puckRb = collision.collider.GetComponent<Rigidbody>();
                Vector3 hitDir = (collision.transform.position - transform.position).normalized;
                puckRb.AddForce(hitDir * 8f, ForceMode.Impulse);
                
                // Reward nhỏ khi chạm bóng
                AddReward(0.1f);
            }
        }

        // Gọi từ GoalTrigger khi AI thắng/thua
        public void OnGoalScored(bool isWinner)
        {
            if (isWinner)
            {
                AddReward(1f);
                Debug.Log($"Agent {playerIndex} WON! +1 reward");
            }
            else
            {
                AddReward(-1f);
                Debug.Log($"Agent {playerIndex} LOST! -1 reward");
            }
            
            EndEpisode(); // Kết thúc episode, sẽ gọi OnEpisodeBegin()
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var continuousActionsOut = actionsOut.ContinuousActions;
            
            // Player 0 dùng WASD
            if (playerIndex == 0)
            {
                continuousActionsOut[0] = Input.GetAxis("Horizontal");
                continuousActionsOut[1] = Input.GetAxis("Vertical");
            }
            // Player 1 dùng IJKL
            else
            {
                float h = (Input.GetKey(KeyCode.L) ? 1 : 0) - (Input.GetKey(KeyCode.J) ? 1 : 0);
                float v = (Input.GetKey(KeyCode.I) ? 1 : 0) - (Input.GetKey(KeyCode.K) ? 1 : 0);
                continuousActionsOut[0] = h;
                continuousActionsOut[1] = v;
            }
        }
    }
}
