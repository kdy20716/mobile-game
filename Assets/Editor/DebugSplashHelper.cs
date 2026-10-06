using UnityEngine;
using UnityEditor;
using BlockBlast;

public static class DebugSplashHelper
{
    [MenuItem("Tools/Debug Splash Info")]
    public static void LogSplashInfo()
    {
        var canvas = GameObject.Find("BlockBlastCanvas");
        if (canvas == null)
        {
            Debug.LogError("BlockBlastCanvas not found!");
            return;
        }

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine($"=== BlockBlastCanvas Children (Count={canvas.transform.childCount}) ===");
        for (int i = 0; i < canvas.transform.childCount; i++)
        {
            var child = canvas.transform.GetChild(i);
            var cg = child.GetComponent<CanvasGroup>();
            var img = child.GetComponent<UnityEngine.UI.Image>();
            string cgInfo = cg != null ? $" [CG: a={cg.alpha}, interact={cg.interactable}, blocks={cg.blocksRaycasts}]" : "";
            string imgInfo = img != null ? $" [Img: col={img.color}, enabled={img.enabled}]" : "";
            sb.AppendLine($"[{i}] {child.name} (activeSelf={child.gameObject.activeSelf}){cgInfo}{imgInfo}");
        }

        var splash = GameObject.Find("SplashScreenOverlay");
        if (splash != null)
        {
            var ctrl = splash.GetComponent<SplashScreenController>();
            sb.AppendLine($"SplashScreenOverlay ctrl: {(ctrl != null ? ("enable=" + ctrl.GetType().GetField("enableSplashScreen", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(ctrl)) : "null")}");
            sb.AppendLine($"SplashScreenOverlay siblingIndex={splash.transform.GetSiblingIndex()}, activeInHierarchy={splash.activeInHierarchy}");
        }
        else
        {
            sb.AppendLine("SplashScreenOverlay NOT FOUND!");
        }
        Debug.Log(sb.ToString());
    }
}
