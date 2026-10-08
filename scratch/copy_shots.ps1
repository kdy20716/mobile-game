$dir = "C:\Users\kdy02\.gemini\antigravity\brain\11e13a30-67ac-4ca4-aa02-999cf9c4f657"
$map = @(
  @{ Src='screenshot_game_20261008_213000_280.png'; Dst='lang_audit_01_ko_lobby.png' },
  @{ Src='screenshot_game_20261008_213004_049.png'; Dst='lang_audit_02_ko_shop_packages.png' },
  @{ Src='screenshot_game_20261008_213008_047.png'; Dst='lang_audit_03_ko_probability.png' },
  @{ Src='screenshot_game_20261008_213011_841.png'; Dst='lang_audit_04_ko_settings.png' },
  @{ Src='screenshot_game_20261008_213017_094.png'; Dst='lang_audit_05_en_lobby.png' },
  @{ Src='screenshot_game_20261008_213020_711.png'; Dst='lang_audit_06_en_shop_packages.png' },
  @{ Src='screenshot_game_20261008_213024_299.png'; Dst='lang_audit_07_en_shop_pickup.png' },
  @{ Src='screenshot_game_20261008_213028_001.png'; Dst='lang_audit_08_en_probability.png' },
  @{ Src='screenshot_game_20261008_213031_613.png'; Dst='lang_audit_09_en_settings.png' },
  @{ Src='screenshot_game_20261008_213035_252.png'; Dst='lang_audit_10_en_mascot_hub.png' },
  @{ Src='screenshot_game_20261008_213040_306.png'; Dst='lang_audit_11_ja_lobby.png' },
  @{ Src='screenshot_game_20261008_213043_902.png'; Dst='lang_audit_12_ja_shop_packages.png' },
  @{ Src='screenshot_game_20261008_213047_619.png'; Dst='lang_audit_13_ja_probability.png' },
  @{ Src='screenshot_game_20261008_213051_201.png'; Dst='lang_audit_14_ja_settings.png' },
  @{ Src='screenshot_game_20261008_213056_314.png'; Dst='lang_audit_15_zh_lobby.png' },
  @{ Src='screenshot_game_20261008_213059_921.png'; Dst='lang_audit_16_zh_shop_packages.png' },
  @{ Src='screenshot_game_20261008_213103_549.png'; Dst='lang_audit_17_zh_probability.png' },
  @{ Src='screenshot_game_20261008_213107_189.png'; Dst='lang_audit_18_zh_settings.png' },
  @{ Src='screenshot_game_20261008_213112_312.png'; Dst='lang_audit_19_ko_restored.png' }
)
foreach ($m in $map) {
  $s = Join-Path 'Temp\pipeline-screenshots' $m.Src
  $d = Join-Path $dir $m.Dst
  if (Test-Path $s) {
    Copy-Item $s $d -Force
    Write-Host "Copied $($m.Dst)"
  }
}
