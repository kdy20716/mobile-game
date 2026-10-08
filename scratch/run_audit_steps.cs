// Helper functions for driving multi-language audit
var canvas = UnityEngine.GameObject.Find("BlockBlastCanvas");
if (canvas != null) {
    var menu = canvas.transform.Find("MainMenuScreen");
    if (menu != null && menu.gameObject.activeSelf) menu.gameObject.SetActive(false);
    var splash = canvas.transform.Find("SplashScreenOverlay");
    if (splash != null && splash.gameObject.activeSelf) splash.gameObject.SetActive(false);
}

var lm = UnityEngine.Object.FindAnyObjectByType<BlockBlast.LobbyManager>();
if (lm == null) return "No LobbyManager";

// Read step argument from scratch/step.txt
int step = 0;
string stepFile = "scratch/step.txt";
if (System.IO.File.Exists(stepFile))
{
    int.TryParse(System.IO.File.ReadAllText(stepFile).Trim(), out step);
}

switch (step)
{
    case 0: // KO Lobby
        lm.SelectLanguage(BlockBlast.GameLanguage.KO);
        break;
    case 1: // KO Shop Packages
        lm.OpenShopModal();
        lm.SelectShopTab(0);
        break;
    case 2: // KO Probability Modal
        lm.OpenProbabilityModal();
        break;
    case 3: // KO Settings Modal
        lm.CloseProbabilityModal();
        lm.CloseShopModal();
        lm.OpenSettingsModal();
        break;
    case 4: // EN Lobby
        lm.CloseSettingsModal();
        lm.SelectLanguage(BlockBlast.GameLanguage.EN);
        break;
    case 5: // EN Shop Packages
        lm.OpenShopModal();
        lm.SelectShopTab(0);
        break;
    case 6: // EN Shop Pickup
        lm.SelectShopTab(1);
        break;
    case 7: // EN Probability Modal
        lm.OpenProbabilityModal();
        break;
    case 8: // EN Settings Modal
        lm.CloseProbabilityModal();
        lm.CloseShopModal();
        lm.OpenSettingsModal();
        break;
    case 9: // EN Mascot Hub
        lm.CloseSettingsModal();
        lm.OpenMascotModal();
        break;
    case 10: // JA Lobby
        lm.CloseMascotModal();
        lm.SelectLanguage(BlockBlast.GameLanguage.JA);
        break;
    case 11: // JA Shop Packages
        lm.OpenShopModal();
        lm.SelectShopTab(0);
        break;
    case 12: // JA Probability Modal
        lm.OpenProbabilityModal();
        break;
    case 13: // JA Settings Modal
        lm.CloseProbabilityModal();
        lm.CloseShopModal();
        lm.OpenSettingsModal();
        break;
    case 14: // ZH Lobby
        lm.CloseSettingsModal();
        lm.SelectLanguage(BlockBlast.GameLanguage.ZH);
        break;
    case 15: // ZH Shop Packages
        lm.OpenShopModal();
        lm.SelectShopTab(0);
        break;
    case 16: // ZH Probability Modal
        lm.OpenProbabilityModal();
        break;
    case 17: // ZH Settings Modal
        lm.CloseProbabilityModal();
        lm.CloseShopModal();
        lm.OpenSettingsModal();
        break;
    case 18: // KO Restored Lobby
        lm.CloseSettingsModal();
        lm.SelectLanguage(BlockBlast.GameLanguage.KO);
        break;
}

return $"Step {step} configured";
