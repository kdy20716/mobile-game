$artifactDir = "C:\Users\kdy02\.gemini\antigravity\brain\11e13a30-67ac-4ca4-aa02-999cf9c4f657"

$names = @(
  "lang_audit_01_ko_lobby",
  "lang_audit_02_ko_shop_packages",
  "lang_audit_03_ko_probability",
  "lang_audit_04_ko_settings",
  "lang_audit_05_en_lobby",
  "lang_audit_06_en_shop_packages",
  "lang_audit_07_en_shop_pickup",
  "lang_audit_08_en_probability",
  "lang_audit_09_en_settings",
  "lang_audit_10_en_mascot_hub",
  "lang_audit_11_ja_lobby",
  "lang_audit_12_ja_shop_packages",
  "lang_audit_13_ja_probability",
  "lang_audit_14_ja_settings",
  "lang_audit_15_zh_lobby",
  "lang_audit_16_zh_shop_packages",
  "lang_audit_17_zh_probability",
  "lang_audit_18_zh_settings",
  "lang_audit_19_ko_restored"
)

# First ensure in lobby
& unity cmd eval_file "scratch/show_lobby.cs"
Start-Sleep -Milliseconds 600

for ($i = 0; $i -lt $names.Count; $i++) {
    $name = $names[$i]
    Write-Host "Configuring Step $i ($name)..."
    Set-Content -Path "scratch/step.txt" -Value "$i"
    Start-Sleep -Milliseconds 100

    $res = & unity cmd eval_file "scratch/run_audit_steps.cs"
    Write-Host "  Setup: $res"
    Start-Sleep -Milliseconds 700

    $shotRes = & unity cmd screenshot
    $foundPath = ""
    # Search for path in the output lines
    foreach ($line in ($shotRes -split "`r?`n")) {
        if ($line -match '"path"\s*:\s*"([^"]+)"') {
            $foundPath = $matches[1] -replace '\\\\', '\'
            break
        }
    }

    if ($foundPath -and (Test-Path $foundPath)) {
        $dest = Join-Path $artifactDir "$name.png"
        Copy-Item -Path $foundPath -Destination $dest -Force
        Write-Host "  -> Successfully Saved: $name.png"
    } else {
        Write-Host "  -> Path not found in: $shotRes"
    }
}

Write-Host "=== All 19 localized audit screenshots successfully generated and saved! ==="
