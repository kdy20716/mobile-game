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

        [MenuItem("Block Blast/Build/4. Apply WebGL Settings (No Compression, 9-16)", priority = 103)]
        public static void ConfigureWebGLSettings()
        {
            // Basic metadata
            PlayerSettings.productName = "Mallang Blast";
            PlayerSettings.companyName = "MallangGames";
            PlayerSettings.bundleVersion = "1.2.0";

            // WebGL Screen Size (9:16 portrait)
            PlayerSettings.defaultWebScreenWidth = 540;
            PlayerSettings.defaultWebScreenHeight = 960;
            PlayerSettings.runInBackground = true;

            // Splash Screen OFF
            PlayerSettings.SplashScreen.show = false;
            PlayerSettings.SplashScreen.showUnityLogo = false;

            // WebGL Compression: Disabled for maximum compatibility with any static web host (GitHub Pages, Vercel, Netlify, Itch.io)
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
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

            AssetDatabase.SaveAssets();
            Debug.Log("<color=green><b>[BlockBlastBuild] WebGL 빌드 설정 완료! (무압축 호환 모드, 540x960 9:16, 스플래시 비활성화)</b></color>");
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
                CreatePortfolioEmbedGuide(fullOutputDir);
                EditorUtility.RevealInFinder(Path.Combine(fullOutputDir, "index.html"));
            }
            else
            {
                Debug.LogError($"<color=red>[BlockBlastBuild] WebGL 빌드 실패! 결과: {summary.result}, 에러 수: {summary.totalErrors}</color>");
            }
        }

        private static void CreatePortfolioEmbedGuide(string outputDir)
        {
            try
            {
                string guidePath = Path.Combine(outputDir, "PORTFOLIO_EMBED_GUIDE.md");
                string content = @"# Mallang Blast - WebGL 포트폴리오 삽입 가이드

## 1. 포트폴리오 웹사이트에 iframe으로 삽입하기
포트폴리오 페이지(React, Vue, HTML 등)에 아래 코드를 추가하여 9:16 모바일 비율로 깔끔하게 임베드할 수 있습니다.

```html
<!-- 말랑블라스트 게임 플레이 컨테이너 -->
<div style=""display: flex; justify-content: center; align-items: center; padding: 20px; background: #1a1a24; border-radius: 16px;"">
    <div style=""position: relative; width: 100%; max-width: 450px; aspect-ratio: 9/16; border-radius: 20px; overflow: hidden; box-shadow: 0 10px 30px rgba(0,0,0,0.5);"">
        <iframe 
            src=""/games/mallang-blast/index.html"" 
            style=""width: 100%; height: 100%; border: none;"" 
            allow=""autoplay; fullscreen"" 
            title=""Mallang Blast"">
        </iframe>
    </div>
</div>
```

## 2. 배포 시 팁
- 본 빌드는 **무압축(Uncompressed)** 설정으로 빌드되어 있어, 별도의 서버 gzip/brotli 헤더 설정 없이도 GitHub Pages, Netlify, Vercel, S3 등에 바로 업로드하여 실행할 수 있습니다.
";
                File.WriteAllText(guidePath, content, System.Text.Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[BlockBlastBuild] Guide creation skipped: {ex.Message}");
            }
        }
    }
}
#endif
