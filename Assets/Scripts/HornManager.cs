using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("AudioManager");
                instance = go.AddComponent<AudioManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    private AudioSource audioSource;
    public AudioClip hornSound;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayHornSound(Vector3 position)
    {
        if (hornSound != null)
        {
            audioSource.PlayOneShot(hornSound);
            audioSource.transform.position = position;
        }
    }
    public void StopHornSound(){
        audioSource.Stop();
    }
}