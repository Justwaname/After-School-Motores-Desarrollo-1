using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum HorrorSFXEvent
{
    ItemPickup,
    KeyCombined,
    ExitDoor,
    LockerOpen,
    LockerClose,
    EnemyFootsteps,
    EnemyAttack,
    UINotification
}

[Serializable]
public class HorrorSFXDefinition
{
    public HorrorSFXEvent eventType;
    public AudioClip[] clips;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Range(0f, 1f)]
    public float spatialBlend = 0f;

    public bool randomizePitch = true;
    public Vector2 pitchRange = new Vector2(0.95f, 1.05f);

    public bool randomizeVolume = true;
    public Vector2 volumeRange = new Vector2(0.9f, 1f);

    [NonSerialized] private int lastClipIndex = -1;

    public AudioClip GetRandomClip()
    {
        if (clips == null || clips.Length == 0)
            return null;

        if (clips.Length == 1)
        {
            lastClipIndex = 0;
            return clips[0];
        }

        int index = UnityEngine.Random.Range(0, clips.Length);

        if (index == lastClipIndex)
        {
            index = (index +
                UnityEngine.Random.Range(1, clips.Length))
                % clips.Length;
        }

        lastClipIndex = index;
        return clips[index];
    }
}

public class HorrorSFXManager : MonoBehaviour
{
    public static HorrorSFXManager Instance { get; private set; }

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixerGroup sfxGroup;

    [Header("Distancia del audio 3D")]
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 20f;

    [Header("Los 8 efectos de sonido")]
    [SerializeField]
    private HorrorSFXDefinition[] sounds =
    {
        new HorrorSFXDefinition
        {
            eventType = HorrorSFXEvent.ItemPickup,
            spatialBlend = 0f
        },
        new HorrorSFXDefinition
        {
            eventType = HorrorSFXEvent.KeyCombined,
            spatialBlend = 0f
        },
        new HorrorSFXDefinition
        {
            eventType = HorrorSFXEvent.ExitDoor,
            spatialBlend = 1f
        },
        new HorrorSFXDefinition
        {
            eventType = HorrorSFXEvent.LockerOpen,
            spatialBlend = 1f
        },
        new HorrorSFXDefinition
        {
            eventType = HorrorSFXEvent.LockerClose,
            spatialBlend = 1f
        },
        new HorrorSFXDefinition
        {
            eventType = HorrorSFXEvent.EnemyFootsteps,
            spatialBlend = 1f
        },
        new HorrorSFXDefinition
        {
            eventType = HorrorSFXEvent.EnemyAttack,
            spatialBlend = 1f
        },
        new HorrorSFXDefinition
        {
            eventType = HorrorSFXEvent.UINotification,
            spatialBlend = 0f
        }
    };

    private readonly Dictionary<HorrorSFXEvent, HorrorSFXDefinition>
        soundLookup = new Dictionary<HorrorSFXEvent, HorrorSFXDefinition>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        soundLookup.Clear();

        foreach (HorrorSFXDefinition sound in sounds)
        {
            if (sound == null)
                continue;

            if (soundLookup.ContainsKey(sound.eventType))
            {
                Debug.LogWarning(
                    "Evento SFX duplicado: " + sound.eventType
                );
                continue;
            }

            soundLookup.Add(sound.eventType, sound);
        }
    }

    public void PlaySFX(HorrorSFXEvent eventType, Vector3 position)
    {
        if (!soundLookup.TryGetValue(eventType, out var sound))
        {
            Debug.LogWarning("SFX no configurado: " + eventType);
            return;
        }

        AudioClip clip = sound.GetRandomClip();

        if (clip == null)
        {
            Debug.LogWarning(
                "Asigna clips al evento: " + eventType
            );
            return;
        }

        if (sfxGroup == null)
        {
            Debug.LogError(
                "Asigna el grupo SFX del HorrorMixer."
            );
            return;
        }

        GameObject audioObject = new GameObject("SFX_" + eventType);
        audioObject.transform.position = position;

        AudioSource source = audioObject.AddComponent<AudioSource>();

        source.outputAudioMixerGroup = sfxGroup;
        source.playOnAwake = false;
        source.loop = false;
        source.clip = clip;

        source.spatialBlend = Mathf.Clamp01(sound.spatialBlend);
        source.dopplerLevel = 0f;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.minDistance = minDistance;
        source.maxDistance = Mathf.Max(maxDistance, minDistance + 0.1f);

        float pitch = sound.randomizePitch
            ? UnityEngine.Random.Range(
                Mathf.Min(sound.pitchRange.x, sound.pitchRange.y),
                Mathf.Max(sound.pitchRange.x, sound.pitchRange.y))
            : 1f;

        float volumeMultiplier = sound.randomizeVolume
            ? UnityEngine.Random.Range(
                Mathf.Min(sound.volumeRange.x, sound.volumeRange.y),
                Mathf.Max(sound.volumeRange.x, sound.volumeRange.y))
            : 1f;

        source.pitch = pitch;
        source.volume = Mathf.Clamp01(sound.volume * volumeMultiplier);

        source.Play();

        float duration = clip.length / Mathf.Max(pitch, 0.01f);

        Destroy(audioObject, duration + 0.1f);
    }
}