using UnityEngine;

public class BallSoundEffect : MonoBehaviour
{
    [Header("Sound Settings")]
    public AudioClip ballHitSound;
    public float volume = 0.5f;
    
    private AudioSource audioSource;
    
    void Start()
    {
        // Tạo AudioSource component
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = ballHitSound;
        audioSource.volume = volume;
        audioSource.playOnAwake = false;
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // Kiểm tra tag của object va chạm
        if (collision.gameObject.CompareTag("purpleAgent") || 
            collision.gameObject.CompareTag("blueAgent"))
        {
            // Phát âm thanh
            if (audioSource != null && ballHitSound != null)
            {
                audioSource.Play();
            }
        }
    }
}