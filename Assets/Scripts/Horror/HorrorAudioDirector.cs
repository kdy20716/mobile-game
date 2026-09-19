using UnityEngine;

namespace HorrorEscape
{
    public class HorrorAudioDirector : MonoBehaviour
    {
        [SerializeField] private AudioSource humSource;
        [SerializeField] private AudioSource heartbeatSource;
        [SerializeField] private AudioSource sfxSource;

        private HorrorEntityAI _entity;

        private void Awake()
        {
            // 1. Fluorescent Light Hum
            humSource = gameObject.AddComponent<AudioSource>();
            humSource.loop = true;
            humSource.volume = 0.22f;
            humSource.clip = CreateHumClip();
            humSource.Play();

            // 2. Heartbeat Source
            heartbeatSource = gameObject.AddComponent<AudioSource>();
            heartbeatSource.loop = true;
            heartbeatSource.volume = 0f;
            heartbeatSource.clip = CreateHeartbeatClip();
            heartbeatSource.Play();

            // 3. SFX Source (Screams, Key Pickup, Door)
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
        }

        private void Start()
        {
            _entity = Object.FindFirstObjectByType<HorrorEntityAI>();
            EscapeKeyItem.OnKeyCollected += (k) => PlayKeyPickupSound();
            HorrorEntityAI.OnPlayerCaught += PlayJumpscareSound;
        }

        private void Update()
        {
            if (_entity == null)
            {
                _entity = Object.FindFirstObjectByType<HorrorEntityAI>();
                return;
            }

            // Increase heartbeat volume and pitch as entity gets closer
            float dist = _entity.DistanceToPlayer;
            if (dist < 18f)
            {
                float urgency = 1f - Mathf.Clamp01(dist / 18f);
                heartbeatSource.volume = Mathf.Lerp(heartbeatSource.volume, urgency * 0.85f, Time.deltaTime * 3f);
                heartbeatSource.pitch = Mathf.Lerp(0.9f, 1.8f, urgency);
            }
            else
            {
                heartbeatSource.volume = Mathf.Lerp(heartbeatSource.volume, 0f, Time.deltaTime * 2f);
            }
        }

        public void PlayKeyPickupSound()
        {
            AudioClip chime = CreateToneClip(784f, 0.4f, 0.5f); // G5 note
            sfxSource.PlayOneShot(chime);
        }

        public void PlayJumpscareSound()
        {
            AudioClip screech = CreateScreechClip();
            sfxSource.PlayOneShot(screech, 1f);
        }

        private AudioClip CreateHumClip()
        {
            int sampleRate = 44100;
            float duration = 1.0f;
            int totalSamples = (int)(sampleRate * duration);
            float[] samples = new float[totalSamples];

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;
                // 60Hz and 120Hz electrical hum with slight buzz
                float s1 = Mathf.Sin(2f * Mathf.PI * 60f * t) * 0.5f;
                float s2 = Mathf.Sin(2f * Mathf.PI * 120f * t) * 0.3f;
                float s3 = Mathf.Sin(2f * Mathf.PI * 180f * t) * 0.15f;
                samples[i] = (s1 + s2 + s3);
            }

            AudioClip clip = AudioClip.Create("FluorescentHum", totalSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateHeartbeatClip()
        {
            int sampleRate = 44100;
            float duration = 1.0f;
            int totalSamples = (int)(sampleRate * duration);
            float[] samples = new float[totalSamples];

            // Thump-thump pattern
            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;
                float val = 0f;
                // First thump (0.1s ~ 0.25s)
                if (t >= 0.05f && t <= 0.2f)
                {
                    float localT = t - 0.05f;
                    val = Mathf.Sin(2f * Mathf.PI * 55f * localT) * Mathf.Sin(localT / 0.15f * Mathf.PI);
                }
                // Second thump (0.28s ~ 0.42s)
                else if (t >= 0.28f && t <= 0.42f)
                {
                    float localT = t - 0.28f;
                    val = Mathf.Sin(2f * Mathf.PI * 50f * localT) * Mathf.Sin(localT / 0.14f * Mathf.PI) * 0.75f;
                }
                samples[i] = val * 0.8f;
            }

            AudioClip clip = AudioClip.Create("Heartbeat", totalSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateScreechClip()
        {
            int sampleRate = 44100;
            float duration = 1.5f;
            int totalSamples = (int)(sampleRate * duration);
            float[] samples = new float[totalSamples];

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;
                float freq = Mathf.Lerp(1200f, 300f, t / duration);
                float noise = (Random.value * 2f - 1f) * 0.4f;
                samples[i] = (Mathf.Sin(2f * Mathf.PI * freq * t) + noise) * (1f - t / duration);
            }

            AudioClip clip = AudioClip.Create("Jumpscare", totalSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateToneClip(float freq, float duration, float vol)
        {
            int sampleRate = 44100;
            int totalSamples = (int)(sampleRate * duration);
            float[] samples = new float[totalSamples];

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;
                samples[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * (1f - (float)i / totalSamples) * vol;
            }

            AudioClip clip = AudioClip.Create("Chime", totalSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}
