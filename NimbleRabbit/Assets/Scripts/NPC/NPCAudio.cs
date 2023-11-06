using UnityEngine;

public class NPCAudio : MonoBehaviour
{
    public AudioClip startup;

    public AudioClip engine;

    public AudioClip crash;

    public AudioClip[] shatters;

    public AudioClip[] honks;

    /// <summary>
    /// Engine audio source component.
    /// </summary>
    private AudioSource engineSource;

    /// <summary>
    /// Other audio source component.
    /// </summary>
    private AudioSource otherSource;

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    private void Awake()
    {
        AudioSource[] sources = GetComponents<AudioSource>();
        engineSource = sources[0];
        otherSource = sources[1];
    }

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    private void Start()
    {
        otherSource.playOnAwake = false;
        otherSource.pitch = 1f;
        otherSource.loop = false;

        PlayEngine();
    }

    /// <summary>
    /// Stop sound.
    /// </summary>
    public void StopEngine()
    {
        engineSource.Stop();
    }

    /// <summary>
    /// Play startup sound.
    /// </summary>
    public void PlayStartup()
    {
        engineSource.clip = startup;
        engineSource.loop = false;
        engineSource.pitch = 1f;
        engineSource.Play();
    }

    /// <summary>
    /// Play the engine sound.
    /// </summary>
    public void PlayEngine()
    {
        engineSource.loop = true;
        engineSource.pitch = 0.2f;
        engineSource.clip = engine;
        engineSource.Play();
    }

    /// <summary>
    /// Play crash sound.
    /// </summary>
    public void PlayCrash()
    {
        otherSource.PlayOneShot(crash);
    }

    public void PlayShatter()
    {
        int roll = UnityEngine.Random.Range(
            0,
            shatters.Length);

        otherSource.PlayOneShot(shatters[roll]);
    }

    /// <summary>
    /// Play a random honk sound.
    /// </summary>
    public void PlayHonk()
    {
        int roll = UnityEngine.Random.Range(
            0,
            honks.Length);
        
        otherSource.PlayOneShot(honks[roll]);
    }
}