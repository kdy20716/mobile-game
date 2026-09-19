#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace MobileRacing.Editor
{
    public static class BuildScript
    {
        private static readonly string[] GameScenes = new string[]
        {
            "Assets/Scenes/MobileRacingScene.unity"
        };

        /// <summary>
        /// Unity CLI용 Windows Standalone 빌드 함수
        /// 실행법: Unity.exe -projectPath <경로> -batchmode -quit -executeMethod MobileRacing.Editor.BuildScript.BuildWindows
        /// </summary>
        public static void BuildWindows()
        {
            string outputPath = "Builds/Windows/MobileRacing.exe";
            Debug.Log($"[CLI Build] Windows 빌드 시작: {outputPath}");

            BuildPlayerOptions buildOptions = new BuildPlayerOptions
            {
                scenes = GameScenes,
                locationPathName = outputPath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"<color=green>[CLI Build] 빌드 성공! 크기: {summary.totalSize / (1024 * 1024)} MB, 소요시간: {summary.totalTime.TotalSeconds:F1}초</color>");
            }
            else
            {
                Debug.LogError($"[CLI Build] 빌드 실패! 결과: {summary.result}, 에러 수: {summary.totalErrors}");
                EditorApplication.Exit(1);
            }
        }

        /// <summary>
        /// Unity CLI용 Android APK 빌드 함수
        /// 실행법: Unity.exe -projectPath <경로> -batchmode -quit -executeMethod MobileRacing.Editor.BuildScript.BuildAndroid
        /// </summary>
        public static void BuildAndroid()
        {
            string outputPath = "Builds/Android/MobileRacing.apk";
            Debug.Log($"[CLI Build] Android 빌드 시작: {outputPath}");

            BuildPlayerOptions buildOptions = new BuildPlayerOptions
            {
                scenes = GameScenes,
                locationPathName = outputPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"<color=green>[CLI Build] Android APK 빌드 성공! 크기: {summary.totalSize / (1024 * 1024)} MB</color>");
            }
            else
            {
                Debug.LogError($"[CLI Build] Android 빌드 실패: {summary.result}");
                EditorApplication.Exit(1);
            }
        }
    }
}
#endif
