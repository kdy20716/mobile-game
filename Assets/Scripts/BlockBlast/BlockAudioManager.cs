using UnityEngine;

namespace BlockBlast
{
    public class BlockAudioManager : MonoBehaviour
    {
        public static BlockAudioManager Instance { get; private set; }

        private AudioSource _audioSource;

        // Sweet Kalimba / Music Box Pentatonic Frequencies (C4 to C6)
        private readonly float[] _kalimbaFreqs = new float[]
        {
            523.25f, // C5 (Do)
            587.33f, // D5 (Re)
            659.25f, // E5 (Mi)
            783.99f, // G5 (Sol)
            880.00f, // A5 (La)
            1046.50f,// C6 (High Do)
            1174.66f,// D6 (High Re)
            1318.51f // E6 (High Mi)
        };

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
        }

        // 🫧 ASMR 1: Cute Water Bubble / Jelly Pop
        public void PlayPickup()
        {
            AudioClip clip = GenerateBubblePop(420f, 680f, 0.09f);
            _audioSource.PlayOneShot(clip, 0.45f);
        }

        // 🪵 ASMR 2: Soft Jelly / Wood Block Tap
        public void PlayPlace()
        {
            AudioClip clip = GenerateSoftTap(260f, 0.08f);
            _audioSource.PlayOneShot(clip, 0.55f);
        }

        // 🔔 ASMR 3: Kalimba / Music Box Melodic Combo Chimes
        public void PlayClear(int combo)
        {
            int noteIdx = Mathf.Clamp(combo - 1, 0, _kalimbaFreqs.Length - 1);
            float freq = _kalimbaFreqs[noteIdx];

            AudioClip clip = GenerateKalimbaNote(freq, 0.35f);
            _audioSource.PlayOneShot(clip, 0.7f);
        }

        // 🎉 ASMR 4: Cute Party Popper & Bubble Burst
        public void PlayBomb()
        {
            AudioClip clip = GeneratePartyPopper(0.3f);
            _audioSource.PlayOneShot(clip, 0.8f);
        }

        // 🔨 ASMR 5: Squeaky Toy Hammer "뾱!"
        public void PlayHammer()
        {
            AudioClip clip = GenerateSqueakyToy(850f, 1350f, 0.14f);
            _audioSource.PlayOneShot(clip, 0.65f);
        }

        // --- Procedural ASMR Sound Synthesizers ---

        private AudioClip GenerateBubblePop(float startFreq, float endFreq, float duration)
        {
            int sampleRate = 44100;
            int count = Mathf.CeilToInt(sampleRate * duration);
            float[] samples = new float[count];

            for (int i = 0; i < count; i++)
            {
                float t = (float)i / count;
                float currentFreq = Mathf.Lerp(startFreq, endFreq, Mathf.Pow(t, 0.5f));
                float phase = 2f * Mathf.PI * currentFreq * ((float)i / sampleRate);

                // Quick pop envelope
                float env = Mathf.Sin(t * Mathf.PI);
                samples[i] = Mathf.Sin(phase) * env;
            }

            AudioClip clip = AudioClip.Create("BubblePop", count, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip GenerateSoftTap(float baseFreq, float duration)
        {
            int sampleRate = 44100;
            int count = Mathf.CeilToInt(sampleRate * duration);
            float[] samples = new float[count];

            for (int i = 0; i < count; i++)
            {
                float t = (float)i / sampleRate;
                float env = Mathf.Exp(-t * 28f); // Very snappy decay
                float wave = Mathf.Sin(2f * Mathf.PI * baseFreq * t) +
                             0.25f * Mathf.Sin(2f * Mathf.PI * (baseFreq * 2.2f) * t);

                samples[i] = wave * env;
            }

            AudioClip clip = AudioClip.Create("SoftTap", count, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip GenerateKalimbaNote(float freq, float duration)
        {
            int sampleRate = 44100;
            int count = Mathf.CeilToInt(sampleRate * duration);
            float[] samples = new float[count];

            for (int i = 0; i < count; i++)
            {
                float t = (float)i / sampleRate;
                // Pure harmonic bell/chime decay
                float env = Mathf.Exp(-t * 6.5f);
                float fundamental = Mathf.Sin(2f * Mathf.PI * freq * t);
                float harmonic = 0.35f * Mathf.Sin(2f * Mathf.PI * (freq * 2.76f) * t); // Metal tine chime overtone
                float shimmer = 0.15f * Mathf.Sin(2f * Mathf.PI * (freq * 4.02f) * t);

                samples[i] = (fundamental + harmonic + shimmer) * env;
            }

            AudioClip clip = AudioClip.Create($"Kalimba_{freq}", count, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip GeneratePartyPopper(float duration)
        {
            int sampleRate = 44100;
            int count = Mathf.CeilToInt(sampleRate * duration);
            float[] samples = new float[count];

            for (int i = 0; i < count; i++)
            {
                float t = (float)i / sampleRate;
                float env = Mathf.Exp(-t * 12f);
                // Pitch drop pop + sparkle
                float popFreq = Mathf.Lerp(380f, 90f, t * 8f);
                float pop = Mathf.Sin(2f * Mathf.PI * popFreq * t);
                float sparkle = (Random.value * 2f - 1f) * 0.3f * Mathf.Exp(-t * 20f);

                samples[i] = (pop * 0.7f + sparkle) * env;
            }

            AudioClip clip = AudioClip.Create("PartyPopper", count, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip GenerateSqueakyToy(float startF, float endF, float duration)
        {
            int sampleRate = 44100;
            int count = Mathf.CeilToInt(sampleRate * duration);
            float[] samples = new float[count];

            for (int i = 0; i < count; i++)
            {
                float t = (float)i / count;
                float f = (t < 0.5f) ? Mathf.Lerp(startF, endF, t * 2f) : Mathf.Lerp(endF, startF * 1.1f, (t - 0.5f) * 2f);
                float phase = 2f * Mathf.PI * f * ((float)i / sampleRate);
                float env = Mathf.Sin(t * Mathf.PI);

                samples[i] = Mathf.Sin(phase) * env;
            }

            AudioClip clip = AudioClip.Create("SqueakyToy", count, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}
