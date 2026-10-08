# audit_multilang.ps1
$artifactDir = "C:\Users\kdy02\.gemini\antigravity\brain\11e13a30-67ac-4ca4-aa02-999cf9c4f657"

function Run-Eval($c) {
    & unity cmd eval --code $c
}

function Take-Shot($name) {
    Start-Sleep -Milliseconds 700
    $res = & unity cmd screenshot
    if ($res -match 'Temp\\pipeline-screenshots\\[^\s]+\.png') {
        $p = $matches[0]
        $dest = Join-Path $artifactDir "$name.png"
        Copy-Item -Path $p -Destination $dest -Force
        Write-Host "Captured $name -> $dest"
    } else {
        Write-Host "Screenshot output: $res"
    }
}

Write-Host "Waiting 2s for boot..."
Start-Sleep -Seconds 2

Write-Host "1. Transitioning to Lobby..."
Run-Eval "var mm = UnityEngine.Object.FindAnyObjectByType<BlockBlast.MainMenuCinematicController>(); if (mm != null) mm.gameObject.SetActive(false); var splash = UnityEngine.GameObject.Find(\"SplashScreenOverlay\"); if (splash != null) splash.SetActive(false); if (BlockBlast.LobbyManager.Instance != null) BlockBlast.LobbyManager.Instance.ShowLobby(); return true;"
Start-Sleep -Seconds 1

# Verify Korean
Write-Host "2. Checking Korean (KO)..."
Run-Eval "if (BlockBlast.LobbyManager.Instance != null) BlockBlast.LobbyManager.Instance.SelectLanguage(BlockBlast.GameLanguage.KO); return true;"
Take-Shot "lang_audit_01_ko_lobby"

Run-Eval "if (BlockBlast.LobbyManager.Instance != null) { BlockBlast.LobbyManager.Instance.OpenShopModal(); BlockBlast.LobbyManager.Instance.SelectShopTab(0); } return true;"
Take-Shot "lang_audit_02_ko_shop_packages"

Run-Eval "if (BlockBlast.LobbyManager.Instance != null) BlockBlast.LobbyManager.Instance.OpenProbabilityModal(); return true;"
Take-Shot "lang_audit_03_ko_probability"

Run-Eval "if (BlockBlast.LobbyManager.Instance != null) { BlockBlast.LobbyManager.Instance.CloseProbabilityModal(); BlockBlast.LobbyManager.Instance.CloseShopModal(); BlockBlast.LobbyManager.Instance.OpenSettingsModal(); } return true;"
Take-Shot "lang_audit_04_ko_settings"

Run-Eval "if (BlockBlast.LobbyManager.Instance != null) BlockBlast.LobbyManager.Instance.CloseSettingsModal(); return true;"

# Verify English
Write-Host "3. Checking English (EN)..."
Run-Eval "if (BlockBlast.LobbyManager.Instance != null) BlockBlast.LobbyManager.Instance.SelectLanguage(BlockBlast.GameLanguage.EN); return true;"
Take-Shot "lang_audit_05_en_lobby"

Run-Eval "if (BlockBlast.LobbyManager.Instance != null) { BlockBlast.LobbyManager.Instance.OpenShopModal(); BlockBlast.LobbyManager.Instance.SelectShopTab(0); } return true;"
Take-Shot "lang_audit_06_en_shop_packages"

Run-Eval "if (BlockBlast.LobbyManager.Instance != null) BlockBlast.LobbyManager.Instance.SelectShopTab(1); return true;"
Take-Shot "lang_audit_07_en_shop_pickup"

Run-Eval "if (BlockBlast.LobbyManager.Instance != null) BlockBlast.LobbyManager.Instance.OpenProbabilityModal(); return true;"
Take-Shot "lang_audit_08_en_probability"

Run-Eval "if (BlockBlast.LobbyManager.Instance != null) { BlockBlast.LobbyManager.Instance.CloseProbabilityModal(); BlockBlast.LobbyManager.Instance.CloseShopModal(); BlockBlast.LobbyManager.Instance.OpenSettingsModal(); } return true;"
Take-Shot "lang_audit_09_en_settings"

Run-Eval "if (BlockBlast.LobbyManager.Instance != null) { BlockBlast.LobbyManager.Instance.CloseSettingsModal(); BlockBlast.LobbyManager.Instance.OpenMascotModal(); } return true;"
Take-Shot "lang_audit_10_en_mascot_hub"

Run-Eval "if (BlockBlast.LobbyManager.Instance != null) BlockBlast.LobbyManager.Instance.CloseMascotModal(); return true;"

# Verify Japanese
Write-Host "4. Checking Japanese (JA)..."
Run-Eval "if (BlockBlast.LobbyManager.Instance != null) BlockBlast.LobbyManager.Instance.SelectLanguage(BlockBlast.GameLanguage.JA); return true;"
Take-Shot "lang_audit_11_ja_lobby"

Run-Eval "if (BlockBlast.LobbyManager.Instance != null) { BlockBlast.LobbyManager.Instance.OpenShopModal(); BlockBlast.LobbyManager.Instance.SelectShopTab(0); } return true;"
Take-Shot "lang_audit_12_ja_shop_packages"

Run-Eval "if (BlockBlast.LobbyManager.Instance != null) BlockBlast.LobbyManager.Instance.OpenProbabilityModal(); return true;"
Take-Shot "lang_audit_13_ja_probability"

Run-Eval "if (BlockBlast.LobbyManager.Instance != null) { BlockBlast.LobbyManager.Instance.CloseProbabilityModal(); BlockBlast.LobbyManager.Instance.CloseShopModal(); BlockBlast.LobbyManager.Instance.OpenSettingsModal(); } return true;"
Take-Shot "lang_audit_14_ja_settings"

Run-Eval "if (BlockBlast.LobbyManager.Instance != null) BlockBlast.LobbyManager.Instance.CloseSettingsModal(); return true;"

# Verify Chinese
Write-Host "5. Checking Chinese (ZH)..."
Run-Eval "if (BlockBlast.LobbyManager.Instance != null) BlockBlast.LobbyManager.Instance.SelectLanguage(BlockBlast.GameLanguage.ZH); return true;"
Take-Shot "lang_audit_15_zh_lobby"

Run-Eval "if (BlockBlast.LobbyManager.Instance != null) { BlockBlast.LobbyManager.Instance.OpenShopModal(); BlockBlast.LobbyManager.Instance.SelectShopTab(0); } return true;"
Take-Shot "lang_audit_16_zh_shop_packages"

Run-Eval "if (BlockBlast.LobbyManager.Instance != null) BlockBlast.LobbyManager.Instance.OpenProbabilityModal(); return true;"
Take-Shot "lang_audit_17_zh_probability"

Run-Eval "if (BlockBlast.LobbyManager.Instance != null) { BlockBlast.LobbyManager.Instance.CloseProbabilityModal(); BlockBlast.LobbyManager.Instance.CloseShopModal(); BlockBlast.LobbyManager.Instance.OpenSettingsModal(); } return true;"
Take-Shot "lang_audit_18_zh_settings"

Run-Eval "if (BlockBlast.LobbyManager.Instance != null) BlockBlast.LobbyManager.Instance.CloseSettingsModal(); return true;"

# Return to Korean
Write-Host "6. Restoring to Korean..."
Run-Eval "if (BlockBlast.LobbyManager.Instance != null) BlockBlast.LobbyManager.Instance.SelectLanguage(BlockBlast.GameLanguage.KO); return true;"
Take-Shot "lang_audit_19_ko_restored"

Write-Host "Complete Multi-Language Audit Finished!"
