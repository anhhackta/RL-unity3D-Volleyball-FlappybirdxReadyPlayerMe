using UnityEngine;
using TMPro;

public class NPCTriggerUI : MonoBehaviour
{
    public TextMeshProUGUI promptText;
    void Start()
    {
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            promptText.gameObject.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            promptText.gameObject.SetActive(false);
    }
}
