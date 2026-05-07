using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip clickSound;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        if (musicSource == null || backgroundMusic == null)
            return;

        musicSource.clip = backgroundMusic;
        musicSource.loop = true;

        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    public void PlayClickSound()
    {
        if (sfxSource == null || clickSound == null)
            return;

        sfxSource.PlayOneShot(clickSound);
    }
}