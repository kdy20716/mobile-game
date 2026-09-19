using UnityEngine;

namespace BlockBlast
{
    public class BlockAudioManager : MonoBehaviour
    {
        public static BlockAudioManager Instance { get; private set; }

        private AudioSource _audioSource;
        private readonly float[] _scaleFreqs = new float[]
        {
            261.63f, // C4 (Do)
            293.66f, // D4 (Re)
            329.63f, // E4 (Mi)
            392.00f, // G4 (Sol)
            440.00f, // A4 (La)
            523.25f, // C5 (Do)
            587.33f, // D5 (Re)
            659.25f, // E5 (Mi)
            783.99f  // G5 (Sol)
        };

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
        }

        public void PlayPickup()
        {
            AudioClip clip = GenerateTone(350f, 0.08f, WaveType.Sine);
            _audioSource.PlayOneShot(clip, 0.4f);
        }

        public void PlayPlace()
        {
            AudioClip clip = GenerateTone(220f, 0.1f, WaveType.Triangle);
            _audioSource.PlayOneShot(clip, 0.6f);
        }

        public void PlayClear(int combo)
        {
            int noteIndex = Mathf.Clamp(combo - 1, 0, _scaleFreqs.Length - 1);
            float freq = _scaleFreqs[noteIndex];
            AudioClip clip = GenerateTone(freq, 0.28f, WaveType.Bell);
            _audioSource.PlayOneShot(clip, 0.75f);
        }

        public void PlayBomb()
        {
            AudioClip clip = GenerateNoise(0.45f);
            _audioSource.PlayOneShot(clip, 0.9f);
        }

        private enum WaveType { Sine, Triangle, Bell }

        private AudioClip GenerateTone(float frequency, float duration, WaveType type)
        {
            int sampleRate = 44100;
            int sampleCount = Mathf.CeilToInt(sampleRate * duration);
            float[] samples = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleRate;
                float envelope = Mathf.Exp(-t * (type == WaveType.Bell ? 7f : 12f));

                float val = 0f;
                if (type == WaveType.Sine)
                {
                    val = Mathf.Sin(2f * Mathf.PI * frequency * t);
                }
                else if (type == WaveType.Triangle)
                {
                    val = Mathf.PingPong(t * frequency * 4f, 2f) - 1f;
                }
                else if (type == WaveType.Bell)
                {
                    val = 0.7f * Mathf.Sin(2f * Mathf.PI * frequency * t) +
                          0.3f * Mathf.Sin(2f * Mathf.PI * frequency * 2f * t);
                }

                samples[i] = val * envelope;
            }

            AudioClip clip = AudioClip.Create($"Tone_{frequency}", sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip GenerateNoise(float duration)
        {
            int sampleRate = 44100;
            int sampleCount = Mathf.CeilToInt(sampleRate * duration);
            float[] samples = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleRate;
                float envelope = Mathf.Exp(-t * 5f);
                float noise = (Random.value * 2f - 1f);
                float lowBoom = Mathf.Sin(2f * Mathf.PI * 65f * t);
                samples[i] = (noise * 0.4f + lowBoom * 0.6f) * envelope;
            }

            AudioClip clip = AudioClip.Create("NoiseBoom", sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}
