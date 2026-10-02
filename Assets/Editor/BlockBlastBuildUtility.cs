#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace BlockBlast.Editor
{
    public static class BlockBlastBuildUtility
    {
        public const string ScenePath = "Assets/Scenes/BlockBlastScene.unity";
        public const string BuildOutputDir = "Builds/MallangBlast_Steam_x64";
        public const string ExeName = "MallangBlast.exe";

        [MenuItem("Block Blast/Build/1. Apply Steam & 9-16 & No Splash Settings", priority = 100)]
        public static void ConfigureAllSettings()
        {
            // 1. Basic Metadata
            PlayerSettings.productName = "Mallang Blast";
            PlayerSettings.companyName = "MallangGames";
            PlayerSettings.bundleVersion = "1.2.0";

            // 2. 9:16 Resolution & Window Mode (Steam Default: Borderless Window)
            PlayerSettings.defaultScreenWidth = 720;
            PlayerSettings.defaultScreenHeight = 1280;
            PlayerSettings.fullScreenMode = FullScreenMode.FullScreenWindow;
            PlayerSettings.resizableWindow = true;
            PlayerSettings.runInBackground = true;
            PlayerSettings.visibleInBackground = true;

            // 3. Disable Unity Splash Screen
            PlayerSettings.SplashScreen.show = false;
            PlayerSettings.SplashScreen.showUnityLogo = false;

            // 4. Input System handling (Both: New + Legacy)
            var pSettingsAsset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset");
            if (pSettingsAsset != null && pSettingsAsset.Length > 0)
            {
                SerializedObject playerSettings = new SerializedObject(pSettingsAsset[0]);
                SerializedProperty activeInputHandler = playerSettings.FindProperty("activeInputHandler");
                if (activeInputHandler != null && activeInputHandler.intValue != 2)
                {
                    activeInputHandler.intValue = 2; // Both
                    playerSettings.ApplyModifiedProperties();
                }
            }

            // 5. Ensure high-resolution lossless textures (2048px, RGBA32, Mipmaps OFF)
            HighResTextureOptimizer.OptimizeAllMascotTextures();

            // 6. App Icon Setup (Dedicated AppIcon.png)
            string pinkMascotPath = "Assets/Textures/BlockBlastCute/Block_Pink_Mascot.png";
            string iconPath = "Assets/Textures/AppIcon.png";
            if (!File.Exists(iconPath) && File.Exists(pinkMascotPath))
            {
                File.Copy(pinkMascotPath, iconPath, true);
                AssetDatabase.ImportAsset(iconPath, ImportAssetOptions.ForceUpdate);
            }
            Texture2D iconTex = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
            if (iconTex != null)
            {
                Texture2D[] icons = new Texture2D[] { iconTex };
                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Standalone, icons);
                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, icons);
            }

            // 6. Set Build Scenes (BlockBlastScene as Scene #0)
            EditorBuildSettingsScene[] buildScenes = new EditorBuildSettingsScene[]
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };
            EditorBuildSettings.scenes = buildScenes;

            AssetDatabase.SaveAssets();
            Debug.Log("<color=green><b>[BlockBlastBuild] Successfully applied Steam release settings (9:16 720x1280, No Splash, BlockBlastScene #0, Icon)!</b></color>");
        }

        [MenuItem("Block Blast/Build/2. Build Windows 64-bit for Steam", priority = 101)]
        public static void BuildWindows64()
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }

            ConfigureAllSettings();

            string fullOutputDir = Path.GetFullPath(BuildOutputDir);
            if (!Directory.Exists(fullOutputDir))
            {
                Directory.CreateDirectory(fullOutputDir);
            }

            string exeFullPath = Path.Combine(fullOutputDir, ExeName);

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = exeFullPath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            Debug.Log($"<color=cyan>[BlockBlastBuild] Windows 64-bit Steam 빌드 시작: {exeFullPath}</color>");

            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                // Copy steam_appid.txt to output folder so direct execution works seamlessly
                string rootAppId = Path.GetFullPath("steam_appid.txt");
                string destAppId = Path.Combine(fullOutputDir, "steam_appid.txt");
                if (File.Exists(rootAppId))
                {
                    File.Copy(rootAppId, destAppId, true);
                    Debug.Log($"<color=cyan>[BlockBlastBuild] Copied steam_appid.txt to: {destAppId}</color>");
                }

                Debug.Log($"<color=green><b>[BlockBlastBuild] ★★★ 빌드 성공! ★★★\n경로: {exeFullPath}\n크기: {summary.totalSize / (1024 * 1024):N1} MB\n소요시간: {summary.totalTime.TotalSeconds:F1}초</b></color>");
                EditorUtility.RevealInFinder(exeFullPath);
            }
            else
            {
                Debug.LogError($"<color=red>[BlockBlastBuild] 빌드 실패! 결과: {summary.result}, 에러 수: {summary.totalErrors}</color>");
            }
        }

        [MenuItem("Block Blast/Build/3. Open Build Folder", priority = 102)]
        public static void OpenBuildFolder()
        {
            string fullOutputDir = Path.GetFullPath(BuildOutputDir);
            if (Directory.Exists(fullOutputDir))
            {
                EditorUtility.RevealInFinder(fullOutputDir);
            }
            else
            {
                Debug.LogWarning("[BlockBlastBuild] Build folder does not exist yet. Please run Build first.");
            }
        }

        public const string WebGLOutputDir = "Builds/MallangBlast_WebGL";

        [MenuItem("Block Blast/Build/4. Apply WebGL Settings (16:9 PC Widescreen)", priority = 103)]
        public static void ConfigureWebGLSettings()
        {
            // Basic metadata
            PlayerSettings.productName = "Mallang Blast";
            PlayerSettings.companyName = "MallangGames";
            PlayerSettings.bundleVersion = "1.2.0";

            // WebGL Screen Size (16:9 PC Widescreen - no side cropping!)
            PlayerSettings.defaultWebScreenWidth = 960;
            PlayerSettings.defaultWebScreenHeight = 540;
            PlayerSettings.runInBackground = true;

            // Splash Screen OFF
            PlayerSettings.SplashScreen.show = false;
            PlayerSettings.SplashScreen.showUnityLogo = false;

            // WebGL Compression: Gzip + Decompression Fallback ON for GitHub (under 100MB limit) & GitHub Pages compatibility
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
            PlayerSettings.WebGL.decompressionFallback = true;

            // Icons
            string iconPath = "Assets/Textures/AppIcon.png";
            Texture2D iconTex = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
            if (iconTex != null)
            {
                Texture2D[] icons = new Texture2D[] { iconTex };
                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.WebGL, icons);
            }

            // Build Scenes
            EditorBuildSettingsScene[] buildScenes = new EditorBuildSettingsScene[]
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };
            EditorBuildSettings.scenes = buildScenes;

            // WebGL 최적화 설정은 이미 메타 파일에 반영되어 있으므로 빌드 시 재임포트 루프 생략
            // OptimizeAssetsForWebGL();

            AssetDatabase.SaveAssets();
            Debug.Log("<color=green><b>[BlockBlastBuild] WebGL 빌드 설정 완료! (16:9 PC 와이드스크린 960x540, Gzip 압축, Decompression Fallback ON)</b></color>");
        }

        public static void OptimizeAssetsForWebGL()
        {
            // 1. Textures Optimization for WebGL
            string[] texGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Textures/BlockBlastCute", "Assets/Textures" });
            int texCount = 0;
            foreach (string guid in texGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                // BlockBlastCute 및 BlockBlastCute_Backup 마스코트/블록 이미지는 압축 제외 (품질 및 원본 보존)
                if (path.Contains("BlockBlastCute") || path.Contains("BlockBlastCute_Backup")) continue;
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null) continue;

                TextureImporterPlatformSettings webglSettings = importer.GetPlatformTextureSettings("WebGL");
                webglSettings.overridden = true;
                if (path.Contains("BG") || path.Contains("Wide") || path.Contains("Logo"))
                {
                    webglSettings.maxTextureSize = 1024;
                }
                else
                {
                    webglSettings.maxTextureSize = 512;
                }
                webglSettings.textureCompression = TextureImporterCompression.Compressed;
                webglSettings.crunchedCompression = true;
                webglSettings.compressionQuality = 85;

                importer.SetPlatformTextureSettings(webglSettings);
                importer.SaveAndReimport();
                texCount++;
            }

            // 2. Audio Optimization for WebGL (SFX: DecompressOnLoad + ADPCM, BGM: Streaming + Vorbis 50%)
            AudioSettingsFixer.FixAllAudioSettings();

            Debug.Log($"<color=cyan>[BlockBlastBuild] WebGL 에셋 최적화 완료: 텍스처 {texCount}개 (512/1024 Crunched), 오디오 설정 최적화 완료</color>");
        }

        [MenuItem("Block Blast/Build/5. Build WebGL (For Portfolio)", priority = 104)]
        public static void BuildWebGL()
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }

            ConfigureWebGLSettings();

            string fullOutputDir = Path.GetFullPath(WebGLOutputDir);
            if (!Directory.Exists(fullOutputDir))
            {
                Directory.CreateDirectory(fullOutputDir);
            }

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = fullOutputDir,
                target = BuildTarget.WebGL,
                options = BuildOptions.None
            };

            Debug.Log($"<color=cyan>[BlockBlastBuild] WebGL 포트폴리오용 빌드 시작: {fullOutputDir}</color>");

            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"<color=green><b>[BlockBlastBuild] ★★★ WebGL 빌드 성공! ★★★\n경로: {fullOutputDir}\n크기: {summary.totalSize / (1024 * 1024):N1} MB\n소요시간: {summary.totalTime.TotalSeconds:F1}초</b></color>");
                PatchIndexHtml(fullOutputDir);
                CreatePortfolioEmbedGuide(fullOutputDir);
                if (!Application.isBatchMode)
                {
                    EditorUtility.RevealInFinder(Path.Combine(fullOutputDir, "index.html"));
                }
            }
            else
            {
                Debug.LogError($"<color=red>[BlockBlastBuild] WebGL 빌드 실패! 결과: {summary.result}, 에러 수: {summary.totalErrors}</color>");
            }
        }

        private static void PatchIndexHtml(string outputDir)
        {
            try
            {
                string indexPath = Path.Combine(outputDir, "index.html");
                if (File.Exists(indexPath))
                {
                    string html = File.ReadAllText(indexPath);
                    if (html.Contains("// config.autoSyncPersistentDataPath = true;"))
                    {
                        html = html.Replace("// config.autoSyncPersistentDataPath = true;", "config.autoSyncPersistentDataPath = true;");
                    }

                    // Responsive iframe styling so portfolio embed fills container nicely at 16:9
                    string oldDesktopStyle = @"canvas.style.width = ""960px"";
        canvas.style.height = ""540px"";";
                    string responsiveDesktopStyle = @"if (window.self !== window.top) {
          // Inside portfolio iframe: fill container seamlessly
          var container = document.querySelector(""#unity-container"");
          container.style.position = ""absolute"";
          container.style.left = ""0"";
          container.style.top = ""0"";
          container.style.width = ""100%"";
          container.style.height = ""100%"";
          container.style.transform = ""none"";
          canvas.style.width = ""100%"";
          canvas.style.height = ""100%"";
          var footer = document.querySelector(""#unity-footer"");
          if (footer) footer.style.display = ""none"";
        } else {
          canvas.style.width = ""960px"";
          canvas.style.height = ""540px"";
        }";
                    if (html.Contains(oldDesktopStyle))
                    {
                        html = html.Replace(oldDesktopStyle, responsiveDesktopStyle);
                    }

                    File.WriteAllText(indexPath, html);
                    Debug.Log("[BlockBlastBuild] index.html patched (autoSyncPersistentDataPath + 16:9 iframe responsive).");
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[BlockBlastBuild] Failed to patch index.html: " + ex.Message);
            }
        }

        private static void CreatePortfolioEmbedGuide(string outputDir)
        {
            try
            {
                string guidePath = Path.Combine(outputDir, "PORTFOLIO_EMBED_GUIDE.md");
                string content = @"# Mallang Blast - WebGL 포트폴리오 삽입 가이드 (16:9 PC 와이드스크린)

## 1. 포트폴리오 웹사이트에 iframe으로 삽입하기
좌우 사이드 윙 배경과 데코레이션이 잘리지 않고 모두 담긴 **16:9 와이드 화면**으로 포트폴리오 페이지(React, Vue, HTML 등)에 임베드할 수 있습니다.

```html
<!-- 말랑블라스트 게임 플레이 컨테이너 (16:9 PC 와이드스크린) -->
<div style=""display: flex; justify-content: center; align-items: center; padding: 20px; background: #1a1a24; border-radius: 16px;"">
    <div style=""position: relative; width: 100%; max-width: 960px; aspect-ratio: 16/9; border-radius: 20px; overflow: hidden; box-shadow: 0 10px 30px rgba(0,0,0,0.5);"">
        <iframe 
            src=""/games/mallang-blast/index.html"" 
            style=""width: 100%; height: 100%; border: none;"" 
            allow=""autoplay; fullscreen"" 
            title=""Mallang Blast"">
        </iframe>
    </div>
</div>
```

## 2. 배포 및 최적화 안내
- **좌우 잘림 없음 (16:9 원본)**: PC 버전의 양옆 파스텔 사이드 윙 아트와 반짝이 파티클이 온전히 표시됩니다.
- **용량 최적화 완료**: Gzip 압축 및 Decompression Fallback이 적용되어 GitHub의 100MB 파일 제한(현재 77MB)에 걸리지 않고 원활하게 푸시됩니다.
- **자동 저장 지원**: `autoSyncPersistentDataPath = true`가 적용되어 있어 브라우저 캐시에 플레이 기록이 안전하게 저장됩니다.
- **반응형 지원**: iframe 내부에서는 불필요한 푸터 없이 컨테이너 크기에 맞춰 100% 깔끔하게 채워집니다.
";
                File.WriteAllText(guidePath, content, System.Text.Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[BlockBlastBuild] Guide creation skipped: {ex.Message}");
            }
        }

        public const string AndroidOutputDir = "Builds/MallangBlast_Android";

        [MenuItem("Block Blast/Build/6. Apply Android Settings (Portrait & com.MallangGames.MallangBlast)", priority = 105)]
        public static void ConfigureAndroidSettings()
        {
            // 1. Basic Metadata
            PlayerSettings.productName = "Mallang Blast";
            PlayerSettings.companyName = "MallangGames";
            PlayerSettings.bundleVersion = "1.2.0";

            // 2. Portrait Orientation Lock
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;

            // 3. Android Application Identifier (Package Name)
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.MallangGames.MallangBlast");

            // 4. Internet Permission
            PlayerSettings.Android.forceInternetPermission = true;

            // 5. Scripting Backend & Architectures for Google Play (IL2CPP + ARM64 + ARMv7)
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;

            // 6. Build Scenes
            EditorBuildSettingsScene[] buildScenes = new EditorBuildSettingsScene[]
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };
            EditorBuildSettings.scenes = buildScenes;

            // 7. App Icon Setup for Android
            string iconPath = "Assets/Textures/AppIcon.png";
            Texture2D iconTex = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
            if (iconTex != null)
            {
                Texture2D[] icons = new Texture2D[] { iconTex };
                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, icons);
            }

            // 8. Splash Screen
            PlayerSettings.SplashScreen.show = false;
            PlayerSettings.SplashScreen.showUnityLogo = false;

            AssetDatabase.SaveAssets();
            Debug.Log("<color=green><b>[BlockBlastBuild] Android 모바일 빌드 설정 완료! (세로 Portrait 고정, com.MallangGames.MallangBlast, 인터넷 권한 허용, IL2CPP 64비트)</b></color>");
        }

        [MenuItem("Block Blast/Build/7. Build Android AAB (For Google Play Store)", priority = 106)]
        public static void BuildAndroidAAB()
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }

            ConfigureAndroidSettings();
            EditorUserBuildSettings.buildAppBundle = true; // Google Play Store용 AAB 생성

            string fullOutputDir = Path.GetFullPath(AndroidOutputDir);
            if (!Directory.Exists(fullOutputDir))
            {
                Directory.CreateDirectory(fullOutputDir);
            }

            string aabFullPath = Path.Combine(fullOutputDir, "MallangBlast.aab");

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = aabFullPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };

            Debug.Log($"<color=cyan>[BlockBlastBuild] Android Google Play용 AAB 빌드 시작: {aabFullPath}</color>");

            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"<color=green><b>[BlockBlastBuild] ★★★ Android AAB 빌드 성공! ★★★\n경로: {aabFullPath}\n크기: {summary.totalSize / (1024 * 1024):N1} MB\n소요시간: {summary.totalTime.TotalSeconds:F1}초</b></color>");
                if (!Application.isBatchMode)
                {
                    EditorUtility.RevealInFinder(aabFullPath);
                }
            }
            else
            {
                Debug.LogError($"<color=red>[BlockBlastBuild] Android AAB 빌드 실패! 결과: {summary.result}, 에러 수: {summary.totalErrors}</color>");
            }
        }

        [MenuItem("Block Blast/Build/8. Build Android APK (For Test Device)", priority = 107)]
        public static void BuildAndroidAPK()
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }

            ConfigureAndroidSettings();
            EditorUserBuildSettings.buildAppBundle = false; // 테스트용 APK 생성

            string fullOutputDir = Path.GetFullPath(AndroidOutputDir);
            if (!Directory.Exists(fullOutputDir))
            {
                Directory.CreateDirectory(fullOutputDir);
            }

            string apkFullPath = Path.Combine(fullOutputDir, "MallangBlast.apk");

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = apkFullPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };

            Debug.Log($"<color=cyan>[BlockBlastBuild] Android 테스트용 APK 빌드 시작: {apkFullPath}</color>");

            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"<color=green><b>[BlockBlastBuild] ★★★ Android APK 빌드 성공! ★★★\n경로: {apkFullPath}\n크기: {summary.totalSize / (1024 * 1024):N1} MB\n소요시간: {summary.totalTime.TotalSeconds:F1}초</b></color>");
                if (!Application.isBatchMode)
                {
                    EditorUtility.RevealInFinder(apkFullPath);
                }
            }
            else
            {
                Debug.LogError($"<color=red>[BlockBlastBuild] Android APK 빌드 실패! 결과: {summary.result}, 에러 수: {summary.totalErrors}</color>");
            }
        }
    }
}
#endif
