using UnityEngine;
using UnityEngine.SceneManagement;

public class BackScene : MonoBehaviour
{
    public void BackToMainScene()
    {
        SceneManager.LoadScene("GameOfficical");
    }
}
