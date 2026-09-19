using UnityEngine;

namespace MobileRacing
{
    public class CarAudioSystem : MonoBehaviour
    {
        [Header("Engine Sound Settings")]
        [SerializeField] private float minPitch = 0.7f;
        [SerializeField] private float maxPitch = 2.4f;
        [SerializeField] private float pitchMultiplier = 1f;

        private ArcadeCarController _car;
        private AudioSource _engineAudio;
        private AudioSource _sfxAudio;

        private void Awake()
        {
            _car = GetComponent<ArcadeCarController>();

            // Setup Engine Audio Source
            _engineAudio = gameObject.AddComponent<AudioSource>();
            _engineAudio.loop = true;
            _engineAudio.playOnAwake = true;
            _engineAudio.spatialBlend = 0.5f;
            _engineAudio.volume = 0.45f;
            _engineAudio.clip = CreateProceduralEngineClip();
            _engineAudio.Play();

            // Setup SFX Audio Source (Boost, Checkpoint, etc.)
            _sfxAudio = gameObject.AddComponent<AudioSource>();
            _sfxAudio.playOnAwake = false;
            _sfxAudio.spatialBlend = 0f;
            _sfxAudio.volume = 0.75f;
        }

        private void Start()
        {
            if (CheckpointTrackManager.Instance != null)
            {
                CheckpointTrackManager.Instance.OnLapCompleted += (time) => PlayLapChime();
                CheckpointTrackManager.Instance.OnRaceCompleted += () => PlayFinishFanfare();
            }
        }

        private void Update()
        {
            if (_car == null || _engineAudio == null) return;

            // Modulate engine pitch with speed & throttle
            float speedRatio = Mathf.Clamp01(_car.CurrentSpeedKmh / 140f);
            float throttle = MobileInputManager.Instance != null ? Mathf.Abs(MobileInputManager.Instance.ThrottleInput) : 0f;
            bool boost = MobileInputManager.Instance != null && MobileInputManager.Instance.BoostInput;

            float targetPitch = Mathf.Lerp(minPitch, maxPitch, speedRatio * 0.75f + throttle * 0.25f);
            if (boost) targetPitch *= 1.25f;

            _engineAudio.pitch = Mathf.Lerp(_engineAudio.pitch, targetPitch * pitchMultiplier, Time.deltaTime * 6f);
            _engineAudio.volume = Mathf.Lerp(_engineAudio.volume, 0.35f + (throttle * 0.25f) + (speedRatio * 0.2f), Time.deltaTime * 5f);
        }

        public void PlayLapChime()
        {
            if (_sfxAudio != null)
            {
                AudioClip chime = CreateProceduralTone(880f, 0.35f, 0.4f); // A5 note
                _sfxAudio.PlayOneShot(chime);
            }
        }

        public void PlayFinishFanfare()
        {
            if (_sfxAudio != null)
            {
                AudioClip fanfare = CreateProceduralTone(1174.66f, 0.7f, 0.5f); // D6 note
                _sfxAudio.PlayOneShot(fanfare);
            }
        }

        /// <summary>
        /// Generates a looping synthetic combustion engine sound sample (warm rumble)
        /// </summary>
        private AudioClip CreateProceduralEngineClip()
        {
            int sampleRate = 44100;
            float duration = 1.0f;
            int totalSamples = (int)(sampleRate * duration);
            float[] samples = new float[totalSamples];

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;
                // Rich harmonic engine rumble (sine + harmonics + subtle noise)
                float f1 = Mathf.Sin(2f * Mathf.PI * 65f * t);
                float f2 = Mathf.Sin(2f * Mathf.PI * 130f * t) * 0.5f;
                float f3 = Mathf.Sin(2f * Mathf.PI * 195f * t) * 0.25f;
                float noise = (Random.value * 2f - 1f) * 0.08f;
                samples[i] = (f1 + f2 + f3 + noise) * 0.35f;
            }

            AudioClip clip = AudioClip.Create("ProceduralEngine", totalSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateProceduralTone(float frequency, float duration, float volume)
        {
            int sampleRate = 44100;
            int totalSamples = (int)(sampleRate * duration);
            float[] samples = new float[totalSamples];

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;
                float envelope = 1f - (float)i / totalSamples; // Linear fade out
                samples[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * envelope * volume;
            }

            AudioClip clip = AudioClip.Create("Tone_" + frequency, totalSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}
