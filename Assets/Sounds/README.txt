==================================================
   말랑블라스트 (MALLANG BLAST) - 사운드 파일 안내 가이드
==================================================

본 폴더(Assets/Sounds/)는 인게임에서 사용되는 효과음(SFX) 및 배경음악(BGM)을 넣는 폴더입니다.
원하시는 사운드 파일(.wav, .mp3, .ogg)을 아래 규격에 맞추어 넣어주시면
유니티 에디터의 [Block Blast > Generate Block Blast Mobile Scene] 실행 시
자동으로 연결(Auto-Binding)되거나, BlockAudioManager에서 직접 지정할 수 있습니다.

--------------------------------------------------
1. 폴더 구조 (Folder Structure)
--------------------------------------------------
Assets/
  └── Sounds/
        ├── SFX/   <-- 모든 인게임 효과음 (.wav 권장, .mp3 가능)
        └── BGM/   <-- 인게임 배경음악 (.mp3 권장, .wav, .ogg 가능)

--------------------------------------------------
2. 효과음 파일 이름 양식 (SFX Naming Convention)
   * 권장 포맷: 소문자 스네이크 케이스 (snake_case)
   * 지원 확장자: .wav (가장 권장, 지연 없음), .mp3, .ogg
--------------------------------------------------

① 블록 집을 때 (Pick Up):
   - 파일명: sfx_block_pickup.wav
   - 대체 가능 파일명: block_pickup.wav
   - 추천 사운드: 말랑말랑한 젤리를 쏙 집어 올리는 귀여운 팝/뽁 소리

② 블록 놓을 때 (Place):
   - 파일명: sfx_block_place.wav
   - 대체 가능 파일명: block_place.wav
   - 추천 사운드: 보드판 위에 부드러운 젤리가 착지하는 톡/통 소리

③ 블록 터질 때 (Clear / Blast):
   - 파일명: sfx_block_clear.wav
   - 대체 가능 파일명: block_clear.wav, block_blast.wav
   - 추천 사운드: 가로/세로 줄이 완성되어 시원하게 팡 터지는 블라스트 사운드
   * 콤보별 음계(도레미파솔라시도) 적용 시 자동으로 피치가 올라가 더욱 경쾌해집니다.

④ UI 버튼 클릭 (UI Click):
   - 파일명: sfx_ui_click.wav
   - 대체 가능 파일명: ui_click.wav, button_click.wav
   - 추천 사운드: 스핀(SPIN), 스킵(SKIP), 시작, 재시작 등 버튼을 탭할 때의 딸깍/뾱 소리

⑤ 화면 터치 (Screen Touch / Tap):
   - 파일명: sfx_screen_touch.wav
   - 대체 가능 파일명: screen_touch.wav, touch_tap.wav
   - 추천 사운드: 화면의 빈 공간이나 배경을 터치할 때의 가벼운 탭 사운드

⑥ 추가 추천 효과음 (Bonus SFX):
   - 블록 회전(스핀): sfx_block_rotate.wav (또는 block_rotate.wav)
   - 블록 스킵(새로고침): sfx_block_skip.wav (또는 block_skip.wav)
   - 폭탄 블록 폭발: sfx_block_bomb.wav (또는 block_bomb.wav)
   - 피버 모드 돌입: sfx_fever_start.wav (또는 fever_start.wav)
   - 게임 오버: sfx_game_over.wav (또는 game_over.wav)

--------------------------------------------------
3. 배경음악 파일 이름 양식 (BGM Naming Convention)
--------------------------------------------------
   - 파일명: bgm_main.mp3 (또는 bgm_main.wav, bgm_main.ogg)
   - 추천 사운드: 밝고 경쾌하며 힐링되는 룸피/칼림바 스타일의 루프 배경음악

--------------------------------------------------
4. 사운드 팁 (Tips)
--------------------------------------------------
- 효과음(SFX)은 지연(Latency)이 없는 비압축 PCM .wav 파일을 가장 권장합니다.
- 배경음악(BGM)은 파일 용량 절약을 위해 .mp3 또는 .ogg 형식을 권장합니다.
- 파일이 없더라도 기존 내장된 절차적 ASMR 합성 사운드가 안전하게 자동 재생됩니다.
==================================================
