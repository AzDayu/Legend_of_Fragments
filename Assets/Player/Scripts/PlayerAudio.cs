using UnityEngine;

public enum PlayerSoundType
{
    None = 0,
    Walk,
    Sprint,
    Landing,
    Slash
}

[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip walkSound;
    public AudioClip sprintSound;
    public AudioClip landSound;
    public AudioClip slashSound;
    public float soundVolume = 0.6f;

    private AudioSource audioSource;
    private PlayerSoundType currentType;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayPlayerSound(PlayerSoundType type)
    {
        if (audioSource == null) return;

        if (currentType != type)
        {
            audioSource.Stop();
            currentType = type;
        }

        AudioClip clip = null;

        switch (type)
        {
            case PlayerSoundType.Walk: clip = walkSound; break;
            case PlayerSoundType.Sprint: clip = sprintSound; break;
            case PlayerSoundType.Landing: clip = landSound; break;
            case PlayerSoundType.Slash: clip = slashSound; break;
        }

        if (clip != null)
        {
            audioSource.PlayOneShot(clip, soundVolume);
        }
    }

    public void PlayerSoundAllStop()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            currentType = PlayerSoundType.None;
        }
    }
}
