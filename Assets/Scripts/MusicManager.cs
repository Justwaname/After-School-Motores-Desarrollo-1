using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
        public AudioSource[] musicLayers;
        public float fadeDuration = 1.0f;

        private Coroutine[] fadeCoroutines;

        void Awake()
        {
            fadeCoroutines = new Coroutine[musicLayers.Length];
        }

        void Start()
        {
            double startDspTime = AudioSettings.dspTime + 0.5;

            foreach (AudioSource layer in musicLayers)
            {
                layer.PlayScheduled(startDspTime);
            }
        }

        public void SetLayerActive(int layerIndex, bool active)
        {
            if (layerIndex >= 0 && layerIndex < musicLayers.Length)
            {
                float targetVolume = active ? 1.0f : 0.0f;

                if (fadeCoroutines[layerIndex] != null)
                {
                    StopCoroutine(fadeCoroutines[layerIndex]);
                }

                fadeCoroutines[layerIndex] = StartCoroutine(FadeLayer(musicLayers[layerIndex], targetVolume, fadeDuration));
            }
        }

        private IEnumerator FadeLayer(AudioSource source, float targetVol, float duration)
        {
            float startVol = source.volume;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += UnityEngine.Time.deltaTime;
                source.volume = Mathf.Lerp(startVol, targetVol, elapsed / duration);
                yield return null;
            }

            source.volume = targetVol;
        }
    }
