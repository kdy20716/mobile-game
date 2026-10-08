$code = @'
var canvas = UnityEngine.GameObject.Find("BlockBlastCanvas");
if (canvas == null) return "No Canvas";
var menu = canvas.transform.Find("MainMenuScreen");
if (menu != null) menu.gameObject.SetActive(false);
var splash = canvas.transform.Find("SplashScreenOverlay");
if (splash != null) splash.gameObject.SetActive(false);
var lobby = canvas.transform.Find("LobbyScreen");
if (lobby != null) {
    lobby.gameObject.SetActive(true);
    var cg = lobby.GetComponent<UnityEngine.CanvasGroup>();
    if (cg != null) { cg.alpha = 1f; cg.blocksRaycasts = true; cg.interactable = true; }
}
var lm = canvas.GetComponent<BlockBlast.LobbyManager>();
if (lm != null) {
    lm.ShowLobby();
    return "Lobby Shown";
}
return "No LobbyManager";
'@

$res = & unity cmd eval --code $code
Write-Host "Result: $res"

Start-Sleep -Milliseconds 600
$shot = & unity cmd screenshot
Write-Host "Shot: $shot"
