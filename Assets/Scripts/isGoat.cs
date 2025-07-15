using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    public AirHockeyGameManager gameManager;
    public bool isGoal1; // True cho gôn 1 (Z dương), False cho gôn 2

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Puck"))
        {
            if (gameManager != null)
            {
                gameManager.OnGoalScored(isGoal1);
            }
        }
    }
}