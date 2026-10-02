#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BlockBlast.Editor
{
    public static class AudioSettingsFixer
    {
        [MenuItem("Block Blast/Audio/Fix All Audio Import Settings", priority = 200)]
        public static void FixAllAudioSettings()
        {
            string[] searchFolders = new[] { "Assets/Sounds", "Assets/Resources/Audio" };
            string[] guids = AssetDatabase.FindAssets("t:AudioClip", searchFolders);

            int sfxCount = 0;
            int bgmCount = 0;

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(path)) continue;

                AudioImporter importer = AssetImporter.GetAtPath(path) as AudioImporter;
                if (importer == null) continue;

                string normalizedPath = path.Replace('\\', '/');
                bool isBgm = normalizedPath.Contains("/BGM/") || 
                             normalizedPath.Contains("Mallang Blast - ");

                if (isBgm)
                {
                    // BGM Configuration: Streaming + Vorbis (Low RAM usage, seamless loop)
                    importer.loadInBackground = true;

                    AudioImporterSampleSettings defSettings = importer.defaultSampleSettings;
                    defSettings.loadType = AudioClipLoadType.Streaming;
                    defSettings.compressionFormat = AudioCompressionFormat.Vorbis;
                    defSettings.quality = 0.7f;
                    defSettings.preloadAudioData = false;
                    importer.defaultSampleSettings = defSettings;

                    // Clear any corrupted WebGL override so it uses clean Streaming + Vorbis
                    importer.ClearSampleSettingOverride("WebGL");

                    importer.SaveAndReimport();
                    bgmCount++;
                }
                else
                {
                    // SFX Configuration: DecompressOnLoad + PCM (Zero latency, uncompressed raw audio, completely eliminates FMOD error)
                    importer.loadInBackground = false;

                    AudioImporterSampleSettings defSettings = importer.defaultSampleSettings;
                    defSettings.loadType = AudioClipLoadType.DecompressOnLoad;
                    defSettings.compressionFormat = AudioCompressionFormat.PCM;
                    defSettings.quality = 1.0f;
                    defSettings.preloadAudioData = true;
                    importer.defaultSampleSettings = defSettings;

                    // Clear any corrupted WebGL override so it uses clean DecompressOnLoad + PCM
                    importer.ClearSampleSettingOverride("WebGL");

                    importer.SaveAndReimport();
                    sfxCount++;
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"<color=green><b>[AudioSettingsFixer] 오디오 설정 복구 완료! SFX {sfxCount}개 (DecompressOnLoad + PCM), BGM {bgmCount}개 (Streaming + Vorbis)</b></color>");
        }
    }
}
#endif
