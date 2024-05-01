using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource musicSource;

    [SerializeField] private AudioSource SFXSource;

    [Header("Audio Clip")]
    [SerializeField] private AudioClip background;

    [SerializeField] internal AudioClip shoot;
    [SerializeField] internal AudioClip enemyDie;
    [SerializeField] internal AudioClip playerHit;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }

    internal void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}