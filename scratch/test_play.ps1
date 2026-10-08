Write-Host "Triggering editor_play..."
$res = & unity cmd editor_play
Write-Host "Result: $res"
Start-Sleep -Seconds 3

Write-Host "Checking status..."
$status = & unity cmd eval "UnityEditor.EditorApplication.isPlaying"
Write-Host "isPlaying: $status"

Write-Host "Stopping editor..."
& unity cmd editor_stop
Write-Host "Done."
