using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlockBlast
{
    public enum GameLanguage
    {
        KO = 0, // 한국어
        EN = 1, // English
        JA = 2, // 日本語
        ZH = 3  // 简体中文
    }

    public static class LocalizationManager
    {
        private const string PREF_KEY = "Mallang_Language_Selection";
        private const string PREF_MANUAL_OVERRIDE = "Mallang_Language_ManualOverride";
        private static GameLanguage _currentLang = GameLanguage.KO;
        private static bool _initialized = false;

        public static event Action<GameLanguage> OnLanguageChanged;

        public static GameLanguage CurrentLanguage
        {
            get
            {
                if (!_initialized) Init();
                return _currentLang;
            }
            set
            {
                _currentLang = value;
                _initialized = true;
                PlayerPrefs.SetInt(PREF_KEY, (int)_currentLang);
                PlayerPrefs.SetInt(PREF_MANUAL_OVERRIDE, 1);
                PlayerPrefs.Save();
                OnLanguageChanged?.Invoke(_currentLang);
            }
        }

        public static void Init()
        {
            if (_initialized) return;

            // If user explicitly picked a language previously, use it
            if (PlayerPrefs.GetInt(PREF_MANUAL_OVERRIDE, 0) == 1 && PlayerPrefs.HasKey(PREF_KEY))
            {
                _currentLang = (GameLanguage)PlayerPrefs.GetInt(PREF_KEY, (int)GameLanguage.KO);
            }
            else
            {
                _currentLang = DetectLanguage();
            }

            _initialized = true;
            Debug.Log($"<color=cyan>[LocalizationManager] Initialized with language: {_currentLang}</color>");
        }

        /// <summary>
        /// Called automatically when Steamworks API is initialized.
        /// If user has not manually locked a language choice, automatically syncs with Steam!
        /// </summary>
        public static void OnSteamInitialized()
        {
            if (PlayerPrefs.GetInt(PREF_MANUAL_OVERRIDE, 0) == 0)
            {
                string steamLang = SteamManager.GetCurrentSteamLanguage();
                if (!string.IsNullOrEmpty(steamLang))
                {
                    GameLanguage lang = ConvertSteamLanguage(steamLang);
                    _currentLang = lang;
                    _initialized = true;
                    PlayerPrefs.SetInt(PREF_KEY, (int)lang);
                    PlayerPrefs.Save();
                    OnLanguageChanged?.Invoke(_currentLang);
                    Debug.Log($"<color=green><b>[LocalizationManager] Successfully auto-applied Steam Language: {lang} (Steam API: '{steamLang}')</b></color>");
                }
            }
        }

        public static GameLanguage ConvertSteamLanguage(string steamLang)
        {
            if (string.IsNullOrEmpty(steamLang)) return GameLanguage.KO;

            switch (steamLang.ToLowerInvariant())
            {
                case "korean":
                    return GameLanguage.KO;
                case "english":
                    return GameLanguage.EN;
                case "japanese":
                    return GameLanguage.JA;
                case "schinese":
                case "tchinese":
                    return GameLanguage.ZH;
                default:
                    return GameLanguage.EN;
            }
        }

        /// <summary>
        /// Detect language from Steamworks API (SteamApps.GetCurrentGameLanguage()),
        /// with fallback to Application.systemLanguage.
        /// </summary>
        public static GameLanguage DetectLanguage()
        {
            // 1. Try Steamworks API (only during runtime play)
            if (Application.isPlaying)
            {
                try
                {
                    string steamLang = SteamManager.GetCurrentSteamLanguage();
                    if (!string.IsNullOrEmpty(steamLang))
                    {
                        return ConvertSteamLanguage(steamLang);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[LocalizationManager] Steam language detection failed: {ex.Message}");
                }
            }

            // 2. Fallback to System Language (OS)
            try
            {
                Debug.Log($"[LocalizationManager] Fallback to System Language: {Application.systemLanguage}");
                switch (Application.systemLanguage)
                {
                    case SystemLanguage.Korean:
                        return GameLanguage.KO;
                    case SystemLanguage.Japanese:
                        return GameLanguage.JA;
                    case SystemLanguage.Chinese:
                    case SystemLanguage.ChineseSimplified:
                    case SystemLanguage.ChineseTraditional:
                        return GameLanguage.ZH;
                    default:
                        return GameLanguage.EN;
                }
            }
            catch
            {
                return GameLanguage.KO;
            }
        }

        /// <summary>
        /// Force re-detection from Steam and apply immediately (e.g. from an Auto-Detect button).
        /// </summary>
        public static void SyncWithSteamLanguage(bool saveToPrefs = true)
        {
            GameLanguage detected = DetectLanguage();
            if (saveToPrefs)
            {
                CurrentLanguage = detected;
            }
            else
            {
                _currentLang = detected;
                _initialized = true;
                OnLanguageChanged?.Invoke(_currentLang);
            }
        }

        private static readonly Dictionary<string, string[]> Strings = new Dictionary<string, string[]>
        {
            // Intro
            { "intro_touch", new[] { "화면을 터치해주세요", "Tap to Start", "画面をタップしてください", "点击屏幕开始" } },

            // Lobby Main
            { "lobby_start", new[] { "게임 시작", "Start Game", "ゲームスタート", "开始游戏" } },
            { "lobby_settings", new[] { "설정", "Settings", "設定", "设置" } },
            { "lobby_shop", new[] { "상점", "Shop", "ショップ", "商店" } },
            { "lobby_help", new[] { "도움말", "Help", "ヘルプ", "帮助" } },
            { "lobby_profile", new[] { "프로필", "Profile", "プロフィール", "个人资料" } },
            { "lobby_party_tip", new[] { "말랑이들을 톡톡 눌러보세요!", "Tap the Mallangs to play!", "マランたちをタップしてみてね！", "轻点软萌小伙伴试试吧！" } },
            { "my_coins", new[] { "내 코인", "Coins", "コイン", "金币" } },

            // Settings Modal
            { "settings_title", new[] { "게임 설정", "Settings", "ゲーム設定", "游戏设置" } },
            { "settings_bgm", new[] { "배경음악 (BGM)", "Music (BGM)", "BGM音量", "背景音乐 (BGM)" } },
            { "settings_sfx", new[] { "효과음 (SFX)", "Sound Effects (SFX)", "効果音 (SFX)", "音效 (SFX)" } },
            { "settings_language", new[] { "언어 설정 (Language)", "Language", "言語設定", "语言设置" } },
            { "settings_version", new[] { "말랑블라스트 v1.2.0 (Fairy Party Edition)", "Mallang Blast v1.2.0 (Fairy Party Edition)", "マランブラスト v1.2.0 (Fairy Party Edition)", "软萌爆破 v1.2.0 (Fairy Party Edition)" } },
            { "settings_screen_mode", new[] { "화면 모드 및 해상도 (Display & Resolution)", "Screen & Resolution", "画面モード・解像度", "显示模式与分辨率" } },
            { "settings_aspect_ratio_title", new[] { "화면 비율 (Aspect Ratio)", "Aspect Ratio", "画面比率 (Aspect Ratio)", "画面比例 (Aspect Ratio)" } },
            { "settings_window_mode_title", new[] { "창 모드 (Window Mode)", "Window Mode", "ウィンドウモード (Window Mode)", "窗口模式 (Window Mode)" } },
            { "aspect_16_9", new[] { "16:9", "16:9", "16:9", "16:9" } },
            { "aspect_16_10", new[] { "16:10", "16:10", "16:10", "16:10" } },
            { "aspect_4_3", new[] { "4:3", "4:3", "4:3", "4:3" } },
            { "aspect_9_16", new[] { "9:16", "9:16", "9:16", "9:16" } },
            { "window_mode_windowed", new[] { "창 모드", "Windowed", "ウィンドウ", "窗口模式" } },
            { "window_mode_borderless", new[] { "테두리 없는 창", "Borderless", "枠なし", "无边框窗口" } },
            { "window_mode_fullscreen", new[] { "전체화면", "Fullscreen", "全画面", "全屏模式" } },
            { "settings_btn_fullscreen", new[] { "전체 화면", "Fullscreen", "全画面", "全屏" } },
            { "settings_btn_windowed", new[] { "창 모드 (720x1280)", "Windowed (720x1280)", "ウィンドウ (720x1280)", "窗口模式 (720x1280)" } },
            { "settings_btn_quit", new[] { "게임 종료 (Exit Game)", "Quit Game", "ゲーム終了", "退出游戏" } },

            // Resolution Options (Legacy & Fallback)
            { "res_window_720_1280", new[] { "창 모드 720x1280 (9:16)", "Windowed 720x1280 (9:16)", "ウィンドウ 720x1280 (9:16)", "窗口模式 720x1280 (9:16)" } },
            { "res_window_540_960", new[] { "창 모드 540x960 (9:16 소형)", "Windowed 540x960 (9:16 Small)", "ウィンドウ 540x960 (9:16 小型)", "窗口模式 540x960 (9:16 小型)" } },
            { "res_window_900_1600", new[] { "창 모드 900x1600 (9:16 대형)", "Windowed 900x1600 (9:16 Large)", "ウィンドウ 900x1600 (9:16 大型)", "窗口模式 900x1600 (9:16 大型)" } },
            { "res_window_1280_720", new[] { "창 모드 1280x720 (16:9 와이드)", "Windowed 1280x720 (16:9 Wide)", "ウィンドウ 1280x720 (16:9 ワイド)", "窗口模式 1280x720 (16:9 宽屏)" } },
            { "res_window_1600_900", new[] { "창 모드 1600x900 (16:9 와이드)", "Windowed 1600x900 (16:9 Wide)", "ウィンドウ 1600x900 (16:9 ワイド)", "窗口模式 1600x900 (16:9 宽屏)" } },
            { "res_fullscreen", new[] { "전체화면 (Fullscreen)", "Fullscreen", "全画面 (Fullscreen)", "全屏模式" } },

            // Preset Quick Buttons
            { "res_preset_720", new[] { "720p (기본)", "720p (Def)", "720p (標準)", "720p (默认)" } },
            { "res_preset_540", new[] { "540p", "540p", "540p", "540p" } },
            { "res_preset_900", new[] { "900p", "900p", "900p", "900p" } },
            { "res_preset_full", new[] { "전체화면", "Full", "全画面", "全屏" } },

            // Quit Confirm Modal
            { "quit_modal_title", new[] { "게임 종료", "Quit Game", "ゲーム終了", "退出游戏" } },
            { "quit_modal_desc", new[] { "정말 말랑블라스트를 종료하시겠어요?", "Are you sure you want to quit Mallang Blast?", "本当にマランブラストを終了しますか？", "确定要退出软萌爆破吗？" } },
            { "quit_modal_yes", new[] { "종료", "Quit", "終了", "退出" } },
            { "quit_modal_no", new[] { "취소", "Cancel", "キャンセル", "取消" } },

            // Shop Modal
            { "shop_title", new[] { "배경 테마 상점", "Theme Shop", "テーマショップ", "背景主题商店" } },
            { "shop_tab_game", new[] { "게임 테마", "Game Themes", "ゲームテーマ", "游戏主题" } },
            { "shop_tab_lobby", new[] { "로비 테마", "Lobby Themes", "ロビーテーマ", "大厅主题" } },
            { "shop_btn_equip", new[] { "장착하기", "Equip", "装備", "装备" } },
            { "shop_btn_equipped", new[] { "적용 중", "Equipped", "適用中", "使用中" } },
            { "shop_btn_buy", new[] { "구매", "Buy", "購入", "购买" } },

            // In-Game Themes
            { "theme_game_0_name", new[] { "몽환의 밤", "Dreamy Night", "夢幻の夜", "梦幻之夜" } },
            { "theme_game_0_desc", new[] { "기본 테마 - 달콤한 보랏빛 밤", "Default Theme - Sweet Purple Night", "デフォルト - 甘い紫の夜", "默认主题 - 甜蜜紫夜" } },
            { "theme_game_1_name", new[] { "캔디 랜드", "Candy Land", "キャンディランド", "糖果乐园" } },
            { "theme_game_1_desc", new[] { "달콤한 디저트와 사탕 세상", "Sweet Desserts & Candy World", "甘いデザートとキャンディの世界", "甜品与糖果的世界" } },
            { "theme_game_2_name", new[] { "크리스탈 바다", "Crystal Sea", "クリスタルオーシャン", "水晶之海" } },
            { "theme_game_2_desc", new[] { "신비로운 반짝임의 바다 궁전", "Mystic Sparkling Ocean Palace", "神秘的に輝く海底宮殿", "闪烁神秘光芒的海底宫殿" } },
            { "theme_game_3_name", new[] { "별빛 우주", "Starlight Galaxy", "星空の宇宙", "星空宇宙" } },
            { "theme_game_3_desc", new[] { "아름다운 보랏빛 은하수 별빛", "Beautiful Purple Milky Way", "美しい紫の天の川", "绚丽的紫色银河星空" } },

            // Lobby Themes
            { "theme_lobby_0_name", new[] { "몽환의 방", "Dreamy Room", "夢幻の部屋", "梦幻之屋" } },
            { "theme_lobby_0_desc", new[] { "기본 로비 - 아늑한 파스텔 방", "Default Lobby - Cozy Pastel Room", "デフォルト - 居心地の良い部屋", "默认大厅 - 温馨柔和之屋" } },
            { "theme_lobby_1_name", new[] { "달콤 캔디룸", "Sweet Candy Room", "スイートキャンディ", "甜蜜糖果屋" } },
            { "theme_lobby_1_desc", new[] { "달콤한 디저트와 와플 무대", "Sweet Desserts & Waffle Stage", "甘いデザートとワッフルのステージ", "甜品与华夫饼舞台" } },
            { "theme_lobby_2_name", new[] { "신비 바다룸", "Mystic Ocean Room", "神秘の海底ルーム", "神秘海洋屋" } },
            { "theme_lobby_2_desc", new[] { "신비로운 바다 궁전 무대", "Mystic Ocean Palace Stage", "神秘的な海底宮殿のステージ", "神秘海底宫殿舞台" } },

            // Help Modal
            { "help_title", new[] { "말랑블라스트 가이드", "Mallang Blast Guide", "マランブラストガイド", "玩法说明" } },
            { "help_subtitle", new[] { "달콤하고 쉬운 말랑이 블록 퍼즐 룰!", "Sweet & Easy Mallang Block Puzzle Rules!", "甘くて簡単なマランブロックパズルルール！", "甜蜜简单的软萌方块拼图规则！" } },
            { "help_confirm", new[] { "이해했어요!", "Got It!", "理解した！", "确认" } },
            { "help_step_1_title", new[] { "말랑 젤리 블록 놓기", "Place Jelly Blocks", "ゼリーブロックを配置", "放置软萌果冻方块" } },
            { "help_step_1_desc", new[] { "하단 3개의 블록을 터치 & 드래그하여 보드판에 올려놓아요.", "Touch & drag 3 bottom blocks onto the board.", "下の3つのブロックをドラッグしてボードに置きます。", "触摸并拖动底部的3个方块到棋盘上。" } },
            { "help_step_2_title", new[] { "가로 / 세로 줄 폭파", "Clear Rows & Columns", "縦横のラインを爆破", "消除整行或整列" } },
            { "help_step_2_desc", new[] { "가로 또는 세로 한 줄을 빈틈없이 채우면 팡팡 터져요!", "Fill a full row or column to blast it away!", "縦または横の1列を隙間なく埋めると弾けます！", "填满一整行或一整列即可瞬间爆破！" } },
            { "help_step_3_title", new[] { "달콤한 콤보 보너스", "Sweet Combo Bonus", "甘いコンボボーナス", "甜蜜连击奖励" } },
            { "help_step_3_desc", new[] { "연속으로 줄을 터뜨리면 피버 보너스 점수를 획득해요!", "Chain clears to get high fever combo bonus!", "連続でラインを消すとフィーバーボーナス獲得！", "连续消除可获得狂热连击加成积分！" } },
            { "help_step_4_title", new[] { "긴장감 넘치는 타임어택", "Thrilling Time Attack", "スリリングなタイムアタック", "紧张刺激的时间挑战" } },
            { "help_step_4_desc", new[] { "점수가 오를수록 제한 시간이 점점 줄어드니 서두르세요!", "Time ticks faster as score rises, so hurry up!", "スコアが上がるほど制限時間が短くなります！", "得分越高时间越紧迫，快抓紧放置！" } },

            // Profile Modal
            { "profile_title", new[] { "내 프로필", "My Profile", "マイプロフィール", "玩家个人信息" } },
            { "profile_best_score_prefix", new[] { "최고 점수", "Best Score", "ベストスコア", "最高记录" } },
            { "profile_best_score_suffix", new[] { "점", " pts", "点", "分" } },
            { "profile_coins_label", new[] { "보유 코인", "Coins", "所持コイン", "持有金币" } },
            { "profile_bio_label", new[] { "자기소개 (터치하여 수정)", "Bio (Tap to edit)", "自己紹介 (タップして編集)", "个人简介 (点击修改)" } },
            { "profile_bio_placeholder", new[] { "자기소개를 입력해주세요", "Enter your bio...", "自己紹介を入力してください", "请输入个人简介..." } },
            { "profile_bio_default", new[] { "말랑블라스트에 오신 걸 환영해요!", "Welcome to Mallang Blast!", "マランブラストへようこそ！", "欢迎来到软萌爆破！" } },
            { "profile_pick_label", new[] { "프로필 꾸미기 (말랑이 선택)", "Decorate Profile (Choose Mascot)", "プロフィール装飾 (マラン選択)", "装扮主页 (选择吉祥物)" } },
            { "profile_btn_logout", new[] { "로그아웃", "Log Out", "ログアウト", "退出登录" } },
            { "profile_btn_login", new[] { "게스트로 로그인", "Log In as Guest", "ゲストログイン", "游客登录" } },
            { "profile_login_required", new[] { "로그인이 필요합니다", "Login required", "ログインが必要です", "需要登录" } },

            // In-Game UI
            { "ingame_pause", new[] { "일시정지", "PAUSE", "一時停止", "暂停" } },
            { "ingame_pause_sub", new[] { "잠시 쉬어가는 중이에요~", "Taking a sweet little break~", "ひと休み中だよ〜", "稍微休息一下吧~" } },
            { "ingame_score", new[] { "SCORE", "SCORE", "スコア", "得分" } },
            { "ingame_best", new[] { "BEST", "BEST", "ベスト", "最高" } },
            { "ingame_time", new[] { "TIME", "TIME", "TIME", "时间" } },
            { "ingame_tip", new[] { "한 줄을 채우면 블록이 팡팡!", "Clear lines to blast blocks!", "ラインを揃えてブロックを弾けさせよう！", "填满整行即可消除方块！" } },
            { "ingame_resume", new[] { "계속하기 (Resume)", "Resume", "再開 (Resume)", "继续" } },
            { "ingame_restart", new[] { "다시 시작", "Restart", "やり直す", "重新开始" } },
            { "ingame_lobby", new[] { "로비로", "To Lobby", "ロビーへ", "返回大厅" } },
            { "ingame_gameover", new[] { "게임 오버", "Game Over", "ゲームオーバー", "游戏结束" } },
            { "ingame_gameover_no_moves", new[] { "NO MORE MOVES", "NO MORE MOVES", "NO MORE MOVES", "步数用尽" } },
            { "ingame_gameover_sub", new[] { "더 이상 블록을 놓을 자리가 없어요!", "No more space to place blocks!", "これ以上ブロックを置く場所がありません！", "没有空间放置更多方块了！" } },
            { "ingame_gameover_score", new[] { "최종 점수", "FINAL SCORE", "最終スコア", "最终得分" } },

            // Guide Tips
            { "ingame_tip_0", new[] { "돌리기 버튼으로 블록 회전!", "Use rotate button to turn blocks!", "回転ボタンでブロックの向きを変更！", "使用旋转按钮调整方块方向！" } },
            { "ingame_tip_1", new[] { "한 줄을 채우면 블록이 팡팡!", "Clear lines to blast blocks!", "ラインを揃えてブロックを弾けさせよう！", "填满整行即可消除方块！" } },
            { "ingame_tip_2", new[] { "연속으로 터뜨려 콤보 보너스!", "Chain clears for combo bonus!", "連続で消してコンボボーナス！", "连续消除触发连击奖励！" } },
            { "ingame_tip_3", new[] { "시간 내에 서둘러 블록을 놓으세요!", "Place blocks before time runs out!", "時間内に素早くブロックを配置！", "时间紧迫，快放置方块！" } },
            { "ingame_tip_4", new[] { "스킵 버튼으로 블록 교체!", "Use skip button to swap blocks!", "スキップボタンでブロック交換！", "使用跳过按钮更换方块！" } },
            { "ingame_tip_5", new[] { "놓을 자리가 없으면 게임 종료!", "Game over if no space left!", "置く場所がないとゲームオーバー！", "无处放置则游戏结束！" } },
            { "ingame_tip_6", new[] { "긴 콤보로 최고 점수 도전!", "Aim for high scores with big combos!", "ロングコンボでハイスコアに挑戦！", "挑战超长连击刷新最高记录！" } }
        };

        public static string Get(string key, string fallback = "")
        {
            if (Strings.TryGetValue(key, out var arr))
            {
                int idx = (int)CurrentLanguage;
                if (idx >= 0 && idx < arr.Length) return arr[idx];
            }
            return string.IsNullOrEmpty(fallback) ? key : fallback;
        }
    }
}
