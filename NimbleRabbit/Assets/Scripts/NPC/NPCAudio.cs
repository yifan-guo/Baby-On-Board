using UnityEngine;

public class NPCAudio : MonoBehaviour
{
    public AudioClip startup;

    public AudioClip engine;

    public AudioClip crash;

    public AudioClip[] shatters;

    public AudioClip[] honks;

    public AudioClip[] sirens;

    /// <summary>
    /// Engine audio source component.
    /// </summary>
    private AudioSource engineSource;

    /// <summary>
    /// Other audio source component.
    /// </summary>
    private AudioSource otherSource;

    private AudioSource sirenSource;

    private NPC npc;


    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    private void Awake()
    {
        npc = GetComponent<NPC>();

        AudioSource[] sources = GetComponents<AudioSource>();
        engineSource = sources[0];
        otherSource = sources[1];

        if (npc.role == NPC.Role.Police) {
            sirenSource = sources[2];
        }
    }

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    private void Start()
    {
        engineSource.velocityUpdateMode = AudioVelocityUpdateMode.Dynamic;
        otherSource.velocityUpdateMode = AudioVelocityUpdateMode.Dynamic;

        otherSource.playOnAwake = false;
        otherSource.pitch = 1f;
        otherSource.loop = false;

        if (npc.role == NPC.Role.Police) {
            sirenSource.velocityUpdateMode = AudioVelocityUpdateMode.Dynamic;
            sirenSource.playOnAwake = false;
            sirenSource.pitch = 1f;
            sirenSource.loop = true;
        }
        

        UIManager.instance.settingsMenu.OnSoundVolumeChanged += SetSoundVolume;

        PlayEngine();
    }

    /// <summary>
    /// Settings sound audio bar slider listener.
    /// </summary>
    private void SetSoundVolume(float value)
    {
        engineSource.volume = value;
        otherSource.volume = value;

        if (npc.role == NPC.Role.Police) {
            sirenSource.volume = value;
        }
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

    public void PlaySiren()
    {
        sirenSource.PlayOneShot(sirens[0]);
    }

    public void StopSiren()
    {
        sirenSource.Stop();
    }
}