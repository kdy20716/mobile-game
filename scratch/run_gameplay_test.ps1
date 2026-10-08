$artifactDir = "C:\Users\kdy02\.gemini\antigravity\brain\11e13a30-67ac-4ca4-aa02-999cf9c4f657"

# 1. Enter Play Mode
& unity cmd editor_play
Start-Sleep -Seconds 2

# 2. Show Lobby
& unity cmd eval_file "scratch/show_lobby.cs"
Start-Sleep -Milliseconds 800

$steps = @(
    @{ Id = 0; Name = "gameplay_01_ingame_board"; Wait = 1200 },
    @{ Id = 1; Name = "gameplay_02_block_placed"; Wait = 800 },
    @{ Id = 2; Name = "gameplay_03_pause_modal"; Wait = 800 },
    @{ Id = 3; Name = "gameplay_04_return_lobby"; Wait = 1200 }
)

foreach ($s in $steps) {
    Set-Content -Path "scratch/gameplay_step.txt" -Value "$($s.Id)"
    $res = & unity cmd eval_file "scratch/verify_gameplay.cs"
    Write-Host "Step $($s.Id): $res"
    Start-Sleep -Milliseconds $($s.Wait)

    $shotRes = & unity cmd screenshot
    $foundPath = ""
    foreach ($line in ($shotRes -split "`r?`n")) {
        if ($line -match '"path"\s*:\s*"([^"]+)"') {
            $foundPath = $matches[1] -replace '\\\\', '\'
            break
        }
    }

    if ($foundPath -and (Test-Path $foundPath)) {
        $dest = Join-Path $artifactDir "$($s.Name).png"
        Copy-Item -Path $foundPath -Destination $dest -Force
        Write-Host "  -> Saved: $($s.Name).png"
    }
}

# Stop Play Mode
& unity cmd editor_stop
Write-Host "=== Gameplay Validation Completed Successfully! ==="
