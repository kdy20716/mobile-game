Add-Type -AssemblyName System.Drawing

$brainDir = 'C:\Users\kdy02\.gemini\antigravity\brain\11e13a30-67ac-4ca4-aa02-999cf9c4f657'
$targetDir = 'C:\Users\kdy02\mobile-game\Assets\Textures\BlockBlastCute'

if (-not (Test-Path $targetDir)) {
    New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
}

$mappings = @(
    @{ Pattern = 'jelly_background_dreamy_*.jpg'; Dst = 'Jelly_Background.png'; Transparent = $false },
    @{ Pattern = 'jelly_mascot_smile_*.jpg'; Dst = 'Jelly_Mascot_Smile.png'; Transparent = $true },
    @{ Pattern = 'jelly_mascot_mint_*.jpg'; Dst = 'Jelly_Mascot_Mint.png'; Transparent = $true },
    @{ Pattern = 'jelly_crown_gold_*.jpg'; Dst = 'Jelly_Crown_Gold.png'; Transparent = $true },
    @{ Pattern = 'jelly_flame_pink_*.jpg'; Dst = 'Jelly_Flame_Pink.png'; Transparent = $true },
    @{ Pattern = 'jelly_dice_skip_*.jpg'; Dst = 'Jelly_Dice_Skip.png'; Transparent = $true },
    @{ Pattern = 'jelly_rotate_arrow_*.jpg'; Dst = 'Jelly_Rotate_Arrow.png'; Transparent = $true },
    @{ Pattern = 'jelly_button_pink_*.jpg'; Dst = 'Jelly_Button_Pink.png'; Transparent = $true },
    @{ Pattern = 'jelly_button_teal_*.jpg'; Dst = 'Jelly_Button_Teal.png'; Transparent = $true },
    @{ Pattern = 'jelly_star_bomb_*.jpg'; Dst = 'Jelly_StarBomb.png'; Transparent = $true },
    @{ Pattern = 'jelly_gem_cube_*.jpg'; Dst = 'Jelly_Tile_Base.png'; Transparent = $true }
)

foreach ($m in $mappings) {
    $files = Get-ChildItem -Path $brainDir -Filter $m.Pattern | Sort-Object LastWriteTime -Descending
    if ($files.Count -gt 0) {
        $srcFile = $files[0].FullName
        $dstPath = Join-Path $targetDir $m.Dst
        Write-Host "Processing $($m.Dst) from $($files[0].Name)..."

        $bmp = [System.Drawing.Bitmap]::FromFile($srcFile)
        $newBmp = New-Object System.Drawing.Bitmap($bmp.Width, $bmp.Height, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
        $g = [System.Drawing.Graphics]::FromImage($newBmp)
        $g.DrawImage($bmp, 0, 0, $bmp.Width, $bmp.Height)
        $g.Dispose()
        $bmp.Dispose()

        if ($m.Transparent) {
            for ($y = 0; $y -lt $newBmp.Height; $y++) {
                for ($x = 0; $x -lt $newBmp.Width; $x++) {
                    $c = $newBmp.GetPixel($x, $y)
                    $brightness = ($c.R + $c.G + $c.B) / 3.0
                    $diff = [Math]::Max([Math]::Abs($c.R - $c.G), [Math]::Max([Math]::Abs($c.G - $c.B), [Math]::Abs($c.B - $c.R)))
                    if ($brightness -gt 235 -and $diff -lt 25) {
                        $alpha = [Math]::Max(0, [int]((255 - $brightness) * 12))
                        if ($alpha -gt 255) { $alpha = 255 }
                        $newBmp.SetPixel($x, $y, [System.Drawing.Color]::FromArgb($alpha, $c.R, $c.G, $c.B))
                    }
                }
            }
        }

        $newBmp.Save($dstPath, [System.Drawing.Imaging.ImageFormat]::Png)
        $newBmp.Dispose()
        Write-Host "Saved $($m.Dst)"
    }
}
Write-Host "All 11 NanoBanana images converted and saved!"
