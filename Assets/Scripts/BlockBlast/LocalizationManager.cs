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
            { "lobby_shop", new[] { "상점", "Shop", "ショップ", "商店" } },
            { "lobby_mascot", new[] { "말랑이", "Mascots", "マラン", "软萌" } },
            { "lobby_settings", new[] { "설정", "Settings", "設定", "设置" } },
            { "lobby_help", new[] { "도움말", "Help", "ヘルプ", "帮助" } },
            { "lobby_profile", new[] { "프로필", "Profile", "プロフィール", "个人资料" } },
            { "lobby_party_tip", new[] { "말랑이들을 톡톡 눌러보세요!", "Tap the Mallangs to play!", "マランたちをタップしてみてね！", "轻点软萌小伙伴试试吧！" } },
            { "my_coins", new[] { "내 골드", "Gold", "ゴールド", "金币" } },
            { "my_diamonds", new[] { "내 다이아", "Diamonds", "ダイヤ", "钻石" } },
            { "shop_tab_recommended", new[] { "추천", "Featured", "おすすめ", "推荐" } },
            { "shop_tab_pickup", new[] { "픽업", "Pickup", "ピックアップ", "精选" } },
            { "shop_tab_mascot", new[] { "말랑이", "Mascots", "マラン", "软萌" } },
            { "shop_tab_game", new[] { "게임 배경", "Game Theme", "ゲーム背景", "游戏背景" } },
            { "shop_tab_lobby", new[] { "로비 배경", "Lobby Theme", "ロビー背景", "大厅背景" } },

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

            // Shop Modal & Pickup
            { "shop_title", new[] { "말랑 상점", "Mallang Shop", "マランショップ", "软萌商店" } },
            { "shop_btn_equip", new[] { "장착하기", "Equip", "装備", "装备" } },
            { "shop_btn_equipped", new[] { "적용 중", "Equipped", "適用中", "使用中" } },
            { "shop_btn_owned", new[] { "보유 중", "Owned", "所持中", "已拥有" } },
            { "shop_btn_buy", new[] { "구매", "Buy", "購入", "购买" } },
            { "pickup_event_title", new[] { "스페셜 픽업 소환", "Special Pickup", "スペシャルピックアップ", "特选限时精选" } },
            { "pickup_banner_badge", new[] { "1.0% 확률 UP!", "1.0% Rate UP!", "1.0% 確率UP!", "1.0% 概率提升!" } },
            { "pickup_btn_rates", new[] { "확률 정보", "Drop Rates", "確率情報", "概率详情" } },
            { "pickup_btn_skill_detail", new[] { "ⓘ 스킬 & 픽업 상세", "ⓘ Skill Details", "ⓘ スキル詳細", "ⓘ 技能与精选详情" } },
            { "pickup_summon_1", new[] { "1회 소환", "1x Summon", "1回召喚", "单次召唤" } },
            { "pickup_summon_10", new[] { "10회 소환", "10x Summon", "10回召喚", "十连召唤" } },
            { "pickup_dia_100", new[] { "◆ 100 다이아", "◆ 100 Dia", "◆ 100 ダイヤ", "◆ 100 钻石" } },
            { "pickup_dia_1000", new[] { "◆ 1,000 다이아", "◆ 1,000 Dia", "◆ 1,000 ダイヤ", "◆ 1,000 钻石" } },
            { "pickup_modal_skill_title", new[] { "스페셜 픽업 상세 안내", "Special Pickup Details", "ピックアップ詳細案内", "特选精选详情说明" } },
            { "pickup_modal_skill_headline", new[] { "★ [올 클리어 엔젤] 스페셜 말랑이 능력 미리보기 ★", "★ [All Clear Angel] Special Skill Preview ★", "★ [オールクリアエンジェル] 能力プレビュー ★", "★ [全清天使] 特选团子能力前瞻 ★" } },
            { "pickup_modal_skill_sub", new[] { "궁극기: [올 클리어 엔젤 폭파술] - 보드의 모든 블록을 한 번에 정화!", "Ultimate: [All Clear Angel Burst] - Clears all blocks on the board at once!", "アルティメット: [オールクリアエンジェル爆破術] - 盤面の全ブロックを一掃！", "大招: [全清天使引爆术] - 一键引爆清除棋盘上所有方块！" } },
            { "pickup_modal_feature_1", new[] { "• 30줄 클리어 시 스킬 게이지 100% 충전 (버튼 점등!)", "• Clears 30 lines to fully charge skill gauge!", "• 30ライン消去でスキルゲージ100%充電！", "• 累计消除30行即可蓄满100%大招能量！" } },
            { "pickup_modal_feature_2", new[] { "• 버튼 터치 시 화면의 모든 블록 폭파 + 뾰로롱 마법 연출!", "• Tap the skill button to clear all blocks on the board!", "• スキル発動で盤面の全ブロックが一掃爆破！", "• 点击技能按钮即可瞬间引爆消除棋盘所有方块！" } },
            { "pickup_modal_feature_3", new[] { "• 픽업 확률 1.0%! 중복 획득 시 60조각 즉시 지급 (바로 1회 돌파!)", "• 1.0% Rate! Duplicates grant 60 shards for instant breakthrough!", "• 確率1.0%！重複時は欠片60個即時支給で即座突破！", "• 概率提升至1.0%！重复获得直接转化60碎片立即突破！" } },
            { "pickup_modal_feature_4", new[] { "• 일반/희귀 말랑이 조각 획득! 모은 조각으로 성급 돌파 가능!", "• Common/Rare shards drop! Shards are strictly used for Breakthrough!", "• ノーマル/レアの欠片獲得！集めた欠片で星を限界突破！", "• 掉落普通/稀有伙伴碎片！碎片专用于星级突破！" } },

            // Summon Result Modal
            { "summon_result_count_suffix", new[] { "회 소환 결과", "x Summon Results", "回召喚結果", "连召唤结果" } },
            { "summon_result_unlocked", new[] { "획득 완료!", "Acquired!", "獲得完了！", "获得成功！" } },
            { "summon_result_unlocked_desc", new[] { "최초 소환 성공! 강력한 보드 올 클리어 능력이 해금되었습니다!", "First summon success! Board All Clear skill is unlocked!", "初召喚成功！強力なボード全消去能力が解放されました！", "首次召唤成功！强力全屏引爆技能已解锁！" } },
            { "summon_result_duplicate", new[] { "중복 획득!", "Duplicate Acquired!", "重複獲得！", "重复获得！" } },
            { "summon_result_duplicate_desc", new[] { "스페셜 조각 60개로 즉시 변환되어 바로 돌파 가능!", "Instantly converted to 60 Special Shards for breakthrough!", "スペシャル欠片60個に即座変換！すぐに突破可能！", "直接转化为60个特选碎片，可立即进行突破！" } },
            { "summon_result_shards_title", new[] { "말랑이 조각 획득!", "Mallang Shards Acquired!", "マランの欠片獲得！", "获得伙伴碎片！" } },
            { "summon_result_shards_desc", new[] { "조각을 모아 말랑이의 별을 돌파하세요!", "Collect shards to breakthrough your Mallangs' stars!", "欠片を集めてマランの星を限界突破しよう！", "收集碎片为你的软萌伙伴进行星级突破！" } },
            { "mascot_owned_shards", new[] { "보유", "Owned", "所持", "持有" } },

            // Mascot Screen (말랑이 육성/관리 허브)
            { "codex_title", new[] { "말랑이", "Mallangs", "マラン", "软萌伙伴" } },
            { "codex_subtitle", new[] { "말랑이를 선택하고 육성하여 모험을 함께하세요!", "Select and grow your Mallangs for your adventure!", "マランを選択・育成して冒険に出かけよう！", "选择并养成你的软萌伙伴，一起开启冒险！" } },
            { "codex_collected", new[] { "수집 완료", "Collection", "収集完了", "收集进度" } },
            { "codex_equipped", new[] { "장착 중", "Equipped", "装着中", "出战中" } },
            { "codex_locked", new[] { "미보유 (잠김)", "Locked", "未所持 (ロック)", "未获得 (已锁定)" } },
            { "codex_badge_common", new[] { "일반", "Common", "ノーマル", "普通" } },
            { "codex_badge_rare", new[] { "희귀", "Rare", "レア", "稀有" } },
            { "codex_badge_special", new[] { "스페셜", "Special", "スペシャル", "特选" } },

            // Mascot Growth & Detail Modal
            { "mascot_detail_title", new[] { "말랑이 육성 & 상세 정보", "Mascot Growth & Info", "マラン育成・詳細情報", "伙伴养成与详情" } },
            { "mascot_detail_ability_header", new[] { "★ 고유 능력 상세 (Ability & Effects)", "★ Ability & Effects Details", "★ 固有能力詳細 (Ability & Effects)", "★ 专属能力详情 (Ability & Effects)" } },
            { "mascot_detail_level", new[] { "레벨", "Level", "レベル", "等级" } },
            { "mascot_detail_breakthrough", new[] { "돌파 단계", "Breakthrough", "限界突破", "突破阶段" } },
            { "mascot_detail_shards", new[] { "말랑 조각", "Shards", "マランの欠片", "伙伴碎片" } },
            { "mascot_btn_levelup", new[] { "레벨업", "Level Up", "レベルアップ", "升级" } },
            { "mascot_btn_breakthrough", new[] { "돌파", "Breakthrough", "限界突破", "突破" } },
            { "mascot_btn_equip", new[] { "장착하기", "Equip", "装着する", "出战" } },
            { "mascot_btn_equipped", new[] { "장착 중", "Equipped", "装着中", "出战中" } },
            { "mascot_btn_unlock", new[] { "상점에서 구매", "Buy in Shop", "ショップで購入", "前往商店购买" } },
            { "mascot_btn_buy_in_shop", new[] { "상점에서 구매", "Buy in Shop", "ショップで購入", "前往商店购买" } },
            { "mascot_max_level", new[] { "최고 레벨 달성", "MAX Level", "最大レベル達成", "已达最高等级" } },
            { "mascot_max_breakthrough", new[] { "최대 돌파 완료 (5★)", "MAX Breakthrough (5★)", "最大突破完了 (5★)", "已满星突破 (5★)" } },
            { "mascot_need_shards", new[] { "돌파 전용 재료", "Breakthrough Material", "限界突破専用素材", "突破专用材料" } },
            { "mascot_obtain_pickup", new[] { "상점에서 구매", "Buy in Shop", "ショップで購入", "前往商店购买" } },

            { "mascot_0_name", new[] { "핑크 말랑이", "Pink Mallang", "ピンクマラン", "粉萌团子" } },
            { "mascot_0_title", new[] { "기본 말랑이", "Starter Mallang", "基本マラン", "初识团子" } },
            { "mascot_0_desc", new[] { "고유능력: 추가 시간 보너스\n(턴 제한 시간이 넉넉해집니다)", "Ability: Extra Turn Time\n(Gives you more time per turn)", "固有能力: 追加時間ボーナス\n(ターンの制限時間が延長されます)", "固有能力: 额外时间加成\n(每回合放置方块时间更从容)" } },
            { "mascot_0_concept", new[] { "달콤한 딸기 젤리에서 태어난 사랑스러운 말랑월드의 대표 마스코트입니다. 통통 튀는 탄력과 발그레한 볼터치가 매력적입니다.", "Born from sweet strawberry jelly, the lovely official mascot of Mallang World. Bouncy, cheerful, and full of positive energy.", "甘いイチゴゼリーから生まれたマランワールドの公式看板マスコット。ぷるぷる弾む愛らしい姿で癒やしを届けます。", "诞生自甜美草莓果冻的软萌世界招牌团子。Q弹软嫩，元气满满，治愈每一位旅人。" } },
            { "mascot_0_story", new[] { "퍼즐 숲에서 가장 처음 눈을 뜬 젤리로, 블록이 가득 차도 언제나 긍정적인 미소를 잃지 않습니다. 손길이 닿으면 보드 전체에 달콤한 향기와 함께 넉넉한 여유 시간을 불어넣어 줍니다.", "The very first jelly to awaken in the puzzle forest. It never loses its smile, granting precious extra thinking time to help you overcome tricky board layouts.", "パズルの森で最初に目を覚ましたゼリー。盤面がピンチになっても笑顔を絶やさず、甘い香りと共にたっぷりの思考時間をもたらしてくれます。", "在谜题森林中最先苏醒的初生团子。无论棋盘多么紧张局促，始终微笑面对，并为玩家带来宝贵的深思熟虑时间。" } },

            { "mascot_1_name", new[] { "민트 말랑이", "Mint Mallang", "ミントマラン", "薄荷团子" } },
            { "mascot_1_title", new[] { "스킵 마스터", "Skip Master", "スキップマスター", "跳过大师" } },
            { "mascot_1_desc", new[] { "고유능력: 스킵 스택 강화\n(시작 2개 / 최대 4개 보유 가능)", "Ability: Skip Stack Boost\n(Start with 2 / Hold up to 4 skips)", "固有能力: スキップスタック強化\n(開始2個 / 最大4個ストック可能)", "固有能力: 跳过次数强化\n(初始2次 / 最大可存4次跳过)" } },
            { "mascot_1_concept", new[] { "상쾌한 페퍼민트 허브 이슬을 머금은 장난꾸러기 요정 젤리입니다. 안테나처럼 솟은 뿔을 흔들며 신선한 바람을 일으킵니다.", "A mischievous fairy jelly infused with crisp peppermint dew. Waves its cute antennas to summon a refreshing gust of luck.", "爽やかなペパーミントの朝露を宿したイタズラ妖精ゼリー。触角を揺らして心地よい風を巻き起こします。", "凝聚清凉薄荷晨露的机灵小精灵团子。晃动着可爱触角，带来阵阵清爽与逆转机遇。" } },
            { "mascot_1_story", new[] { "어려운 난관에 부딪히면 재빠르게 한 발 물러서서 더 좋은 기회를 노립니다. 원치 않는 블록 세트를 시원하게 건너뛰는 마법으로 언제나 위기를 슬기롭게 모면합니다.", "Whenever an awkward shape threatens the board, Mint Mallang quickly skips ahead to find the perfect block.", "置きづらいブロックに直面しても機転を利かせてスキップ！いつでも爽快にピンチを脱出させてくれる頼もしい相棒です。", "面对棘手困难的方块时，薄荷团子总能机智跳过，助你化险为夷，轻松重整旗鼓。" } },

            { "mascot_2_name", new[] { "골드 말랑이", "Gold Mallang", "ゴールドマラン", "黄金团子" } },
            { "mascot_2_title", new[] { "보물 사냥꾼", "Treasure Hunter", "トレジャーハンター", "寻宝猎人" } },
            { "mascot_2_desc", new[] { "고유능력: 라인 추가 점수\n(클리어 줄마다 +100점 추가 보너스)", "Ability: Line Clear Score\n(+100 extra bonus pts per cleared line)", "固有能力: ライン追加スコア\n(消去列ごとに+100点ボーナス)", "固有能力: 消除整行额外加分\n(每消除一行额外加100分)" } },
            { "mascot_2_concept", new[] { "반짝이는 황금 왕관을 쓴 허니 골드 젤리입니다. 고귀한 왕가의 보물창고를 지키며 화려한 황금빛 광채를 뿜어냅니다.", "A royal honey jelly wearing a sparkling crown. Radiates golden luxury and blesses players with high-score treasures.", "キラリと輝く黄金の王冠を戴いたハニーゴールドゼリー。華やかな輝きでプレイヤーに財宝とハイスコアをもたらします。", "头戴耀眼王冠的蜜糖黄金小国王。浑身流淌华丽光芒，为每一次消行带来无尽财富与高分。" } },
            { "mascot_2_story", new[] { "보석과 동전을 세상에서 가장 사랑하는 부자 젤리입니다. 완벽하게 완성된 퍼즐 라인을 보면 신나서 황금 보너스 코인을 마구 쏟아붓습니다.", "Loves coins and jewels more than anything. Every single completed line sends Gold Mallang into celebrations, showering the board with bonus points.", "コインと宝石が大好きなリッチなマスコット。美しいライン消去を見るたびに大喜びで黄金ボーナススコアを授けます。", "最喜欢金币与亮晶晶的宝物。每当看见完美的连消，就会欢呼雀跃并慷慨撒下高额额外加分。" } },

            { "mascot_3_name", new[] { "퍼플 말랑이", "Purple Mallang", "パープルマラン", "紫晶团子" } },
            { "mascot_3_title", new[] { "매직 큐브", "Magic Cube", "マジックキューブ", "魔方使者" } },
            { "mascot_3_desc", new[] { "고유능력: 2×2 매직 블록\n(5% 확률로 2×2 보라 블록 3개 소환)", "Ability: 2x2 Magic Blocks\n(5% chance to spawn 3 2x2 purple blocks)", "固有能力: 2×2マジックブロック\n(5%の確率で2×2紫ブロック3個召喚)", "固有能力: 2×2魔方方块\n(5%概率召唤3个2×2紫色方块)" } },
            { "mascot_3_concept", new[] { "신비로운 우주 은하수의 별가루가 스며든 포도빛 글래스 젤리입니다. 차분하고 나긋나긋한 미소 뒤에 강력한 공간 마법을 감추고 있습니다.", "A grape glass jelly filled with stardust from cosmic nebulae. Hides powerful spatial magic behind its serene, cozy smile.", "神秘的な銀河の星屑を抱くグレープゼリー。穏やかな微笑みの裏に強力な空間操作魔法を秘めています。", "吸收了浩瀚星云星尘的葡萄紫晶团子。从容淡雅的笑靥下蕴藏着扭转空间的奇迹魔法。" } },
            { "mascot_3_story", new[] { "밤하늘 별자리와 대화하며 차원을 비트는 비전 마법을 연구합니다. 위기의 순간, 완벽한 정사각형의 2×2 보라 마법 블록을 소환해 빈자리를 채워줍니다.", "Studies arcana that reshape puzzle dimensions. Occasionally conjures neat 2x2 purple magic blocks to fill board voids effortlessly.", "星々の囁きを聞きながら空間を織りなす魔法使い。ピンチの盤面にすっきりと収まる2×2紫魔法ブロックを召喚してくれます。", "静观夜空并掌控空间维度。在需要救场时，会奇迹般召唤三枚完美的2×2方块来填补空缺。" } },

            { "mascot_4_name", new[] { "블루 말랑이", "Blue Mallang", "ブルーマラン", "碧蓝团子" } },
            { "mascot_4_title", new[] { "아쿠아 쉴드", "Aqua Shield", "アクアシールド", "水波护盾" } },
            { "mascot_4_desc", new[] { "고유능력: 아쿠아 쉴드\n(게임오버 위기 시 1회 부활 & 블록 재배치)", "Ability: Aqua Shield\n(Revives once upon game over & reshuffles)", "固有能力: アクアシールド\n(ピンチ時1回復活＆ブロック再配置)", "固有能力: 水波护盾\n(陷入死局时触发1次复活重抽方块)" } },
            { "mascot_4_concept", new[] { "청명하고 시원한 소다 바다 거품에서 태어난 물방울 젤리입니다. 맑고 투명한 표면 아래로 생명의 파도를 품고 있습니다.", "A water droplet jelly born from ocean soda waves. Soft, serene, and radiating calm protective energy.", "澄んだソーダの海泡から生まれた水滴ゼリー。透明な体の奥に生命を守る優しい波音を宿しています。", "由清澈苏打海浪孕育而生的水滴团子。澄澈透明的身体中蕴含着守护一切的温柔潮汐。" } },
            { "mascot_4_story", new[] { "더 이상 블록을 둘 곳이 없어 패배할 찰나, 시원한 물보라 보호막을 펼쳐 게임판을 씻어내고 다시 한번 새로운 블록을 쥐어주는 든든한 수호자입니다.", "When all hope seems lost and no moves remain, Blue Mallang summons an oceanic barrier to revive the run and reshuffle your blocks.", "もう置ける場所がない絶体絶命の瞬間、奇跡の波しぶきで盤面を清めてもう一度チャンスをくれる頼れる守護神です。", "当棋盘被填满即将绝望失败时，它会唤起波涛护盾，为你逆天改命重新抽选手牌方块。" } },

            { "mascot_5_name", new[] { "베리 말랑이", "Berry Mallang", "ベリーマラン", "莓果团子" } },
            { "mascot_5_title", new[] { "슈가 버스트", "Sugar Burst", "シュガーバースト", "糖爆甜心" } },
            { "mascot_5_desc", new[] { "고유능력: 슈가 버스트\n(콤보 달성 시 주변 블록 추가 폭파 & +15% 점수)", "Ability: Sugar Burst\n(Extra blast & +15% score on combos)", "固有能力: シュガーバースト\n(コンボ時周囲追加爆破＆スコア+15%)", "固有能力: 糖爆甜心\n(达成连击时额外爆破周围并提升15%分数)" } },
            { "mascot_5_concept", new[] { "새콤달콤한 산딸기와 체리 시럽이 듬뿍 들어간 활력 넘치는 젤리입니다. 톡톡 튀는 탄산 사탕을 머금어 폭발적인 에너지를 자랑합니다.", "Bursting with wild berries and sweet syrup. Playful, fiery, and loves explosive combo celebrations.", "甘酸っぱいベリーシロップが詰まった元気印ゼリー。パチパチ弾けるキャンディのような爆発的エネルギーを持っています。", "注入了野莓果酱与浓郁糖浆的活力团子。宛如跳跳糖般活泼好动，热爱狂热爆破连击。" } },
            { "mascot_5_story", new[] { "퍼즐이 연속으로 팡팡 터질 때마다 흥분을 감추지 못하고 주변 블록까지 화끈하게 날려버립니다. 짜릿한 연속 콤보 플레이의 최고 파트너입니다.", "Gets thrilled by chaining combos! Automatically detonates surrounding cells with sparkling sugar bursts for massive score multipliers.", "コンボが続くたびに大興奮して周囲のブロックもまとめて吹き飛ばす！爽快連鎖プレイのベストパートナーです。", "每当出现连击消除时，它便会兴奋地引爆周围方块，并给予高达15%的超强连击分数加成。" } },

            { "mascot_6_name", new[] { "레몬 말랑이", "Lemon Mallang", "レモンマラン", "柠檬团子" } },
            { "mascot_6_title", new[] { "번개 팡", "Lemon Spark", "レモンスパーク", "闪电爆破" } },
            { "mascot_6_desc", new[] { "고유능력: 번개 팡\n(3연속 콤보 시 가로 한 줄 번개 즉시 폭파)", "Ability: Lemon Spark\n(Instantly zaps and clears a row at 3 combos)", "固有能力: レモンスパーク\n(3連続コンボ時雷で横1列即座爆破)", "固有能力: 闪电爆破\n(达成3连击时触发闪电瞬间消灭整行)" } },
            { "mascot_6_concept", new[] { "비타민 가득한 레몬 전해질로 짜릿한 전류를 뿜어내는 젤리입니다. 동그란 볼에서 노란 스파크가 튀는 것이 특징입니다.", "Charged with zesty lemon electrolytes. Sparkles with yellow lightning and delivers electric satisfaction.", "ビタミンたっぷりのレモン果汁から電撃を放つゼリー。ほっぺから黄色いスパークを散らすのがトレードマークです。", "充满柠檬果汁电解质的雷电团子。双颊随时迸发出金黄电弧，充满势不可挡的穿透力。" } },
            { "mascot_6_story", new[] { "3번 연속으로 라인을 터뜨리면 온몸의 정전기가 모여 번개 광선을 쏩니다. 가로 한 줄을 번쩍하고 태워버려 막힌 혈을 뚫어줍니다.", "When you hit a 3-chain combo, Lemon Spark unleashes an electric zap that vaporizes an entire horizontal row instantly.", "3連続でラインを消去すると全身に溜まった電気が炸裂！横一列を一瞬で雷光消去して窮地を切り拓きます。", "只要连续达成3次连消，积蓄的电能便会化作耀眼闪电，瞬间电光石火般横扫清除一整行。" } },

            { "mascot_7_name", new[] { "클라우드 말랑이", "Cloud Mallang", "クラウドマラン", "云朵团子" } },
            { "mascot_7_title", new[] { "푹신 구름", "Fluffy Cloud", "ふわふわクラウド", "蓬松云朵" } },
            { "mascot_7_desc", new[] { "고유능력: 푹신 구름\n(시간 감소 속도 20% 완화 & 슬로우 피버)", "Ability: Fluffy Cloud\n(Slows time decay by 20% for cozy puzzle play)", "固有能力: ふわふわクラウド\n(時間減少速度20%緩和＆まったりプレイ)", "固有能力: 蓬松云朵\n(时间衰减速度减缓20%，更轻松畅快)" } },
            { "mascot_7_concept", new[] { "하늘 높은 곳의 몽실몽실한 솜사탕 구름에서 내려온 젤리입니다. 손을 대면 폭신하고 따스한 감촉에 모든 스트레스가 녹아내립니다.", "Drifted down from fluffy cotton candy clouds. Cozy, gentle, and melts away tension with its pillowy texture.", "空高くのわたあめ雲から舞い降りた極上ふんわりゼリー。触れるだけで日々の疲れが溶けていきます。", "自万米高空云端降落的蓬松棉花糖团子。抚摸它那软绵绵的云朵身躯，所有烦恼忧虑皆能烟消云散。" } },
            { "mascot_7_story", new[] { "시간의 흐름마저 푹신한 구름 속으로 끌어들여 느리게 만듭니다. 초조하게 쫓기는 일 없이 편안하게 생각하며 퍼즐을 즐길 수 있게 돕습니다.", "Envelops the clock in cozy clouds, slowing time decay by 20% so you can relax and strategize without rush.", "時間の流れすら雲の中に包み込んでゆったり穏やかに。焦ることなくマイペースにパズルを楽しめるよう支えてくれます。", "用绵密柔软的云层包裹时光，使倒计时衰减放慢20%，让你不再手忙脚乱，悠然享受每一次布局。" } },

            { "mascot_8_name", new[] { "스페셜 말랑이", "Special Angel", "スペシャルマラン", "星天使团子" } },
            { "mascot_8_title", new[] { "올 클리어 엔젤", "All Clear Angel", "オールクリアエンジェル", "全清天使" } },
            { "mascot_8_desc", new[] { "고유능력: 보드 올 클리어\n(스킬 터치 시 보드의 모든 블록 전멸 폭파!)", "Ability: Board Wipe Magic\n(Tap skill button to blast all blocks away!)", "固有能力: オールクリア\n(スキルタップで全ブロック一掃爆破！)", "固有能力: 全屏清盘魔法\n(点击专属大招按钮瞬间清除全场方块！)" } },
            { "mascot_8_concept", new[] { "말랑월드의 가장 높은 성소에서 강림한 1.0% 한정 전설의 천사 젤리입니다. 눈부신 천사 날개와 오로라 홀로그램 빛무리가 특징입니다.", "The 1.0% limited legendary angel jelly from the highest sanctuary. Flaunts radiant angel wings and a rainbow aura.", "マランワールド最奥の聖域より降臨した1.0%限定伝説の天使ゼリー。神々しい翼と虹色ホログラムのオーラを纏っています。", "自软萌世界最崇高圣所降临的1.0%特选限定神话天使团子。背生羽翼，环绕七彩极光神环。" } },
            { "mascot_8_story", new[] { "라인을 지울 때마다 성스러운 기운을 차곡차곡 모아 스킬 버튼을 점등시킵니다. 터치하는 순간 천사의 성스러운 종소리와 함께 보드의 모든 블록을 한 번에 소멸시킵니다.", "Charges holy energy with every line clear. When full, tap the skill button to trigger an All Clear miracle, wiping every single block on the board!", "ラインを消去するたび聖なる祈りを蓄積。スキルボタンをタップした瞬間、盤面の全ブロックを跡形もなく一掃する奇跡を起こします！", "每一次消行都会汇聚纯净圣光。蓄满大招点击按钮的瞬间，将以全屏爆破神迹彻底净化8×8全场方块！" } },

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

            // Restart Confirm Modal
            { "confirm_restart_title", new[] { "다시 시작할까요?", "Restart Game?", "最初から遊ぶ？", "重新开始？" } },
            { "confirm_restart_sub", new[] { "지금까지의 점수가 사라져요.", "Your current score will be lost.", "現在のスコアがリセットされます。", "当前的得分将被重置。" } },
            { "confirm_restart_desc", new[] { "그래도 다시 시작하시겠어요?", "Are you sure you want to restart?", "本当にやり直しますか？", "确定要重新开始吗？" } },
            { "confirm_restart_yes", new[] { "다시 시작", "Restart", "最初から", "重新开始" } },
            { "confirm_restart_no", new[] { "계속하기", "Resume", "続ける", "继续游戏" } },

            // Lobby Confirm Modal
            { "confirm_lobby_title", new[] { "로비로 이동할까요?", "Return to Lobby?", "ロビーに戻る？", "返回大厅？" } },
            { "confirm_lobby_sub", new[] { "진행 중인 게임 내용이 저장되지 않아요.", "Your current game will not be saved.", "進行中のゲームは保存されません。", "进行中的游戏进度将不会保存。" } },
            { "confirm_lobby_desc", new[] { "정말 로비로 나가시겠어요?", "Are you sure you want to exit to lobby?", "本当にロビーに戻りますか？", "确定要返回大厅吗？" } },
            { "confirm_lobby_yes", new[] { "로비로 이동", "Exit to Lobby", "ロビーに戻る", "返回大厅" } },
            { "confirm_lobby_no", new[] { "계속하기", "Resume", "続ける", "继续游戏" } },
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
            { "ingame_resume", new[] { "계속하기", "Resume", "再開", "继续" } },
            { "ingame_restart", new[] { "다시 시작", "Restart", "やり直す", "重新开始" } },
            { "ingame_lobby", new[] { "로비로", "To Lobby", "ロビーへ", "返回大厅" } },
            { "ingame_gameover", new[] { "게임 오버", "Game Over", "ゲームオーバー", "游戏结束" } },
            { "ingame_gameover_no_moves", new[] { "시간 종료!", "TIME'S UP!", "タイムアップ！", "时间到！" } },
            { "ingame_gameover_sub", new[] { "제한 시간이 모두 끝났어요!", "Time has completely run out!", "制限時間が終了しました！", "时间已经耗尽！" } },
            { "ingame_gameover_score", new[] { "최종 점수", "FINAL SCORE", "最終スコア", "最终得分" } },

            // Guide Tips
            { "ingame_tip_0", new[] { "돌리기 버튼으로 블록 회전!", "Use rotate button to turn blocks!", "回転ボタンでブロックの向きを変更！", "使用旋转按钮调整方块方向！" } },
            { "ingame_tip_1", new[] { "한 줄을 채우면 블록이 팡팡!", "Clear lines to blast blocks!", "ラインを揃えてブロックを弾けさせよう！", "填满整行即可消除方块！" } },
            { "ingame_tip_2", new[] { "연속으로 터뜨려 콤보 보너스!", "Chain clears for combo bonus!", "連続で消してコンボボーナス！", "连续消除触发连击奖励！" } },
            { "ingame_tip_3", new[] { "시간 내에 서둘러 블록을 놓으세요!", "Place blocks before time runs out!", "時間内に素早くブロックを配置！", "时间紧迫，快放置方块！" } },
            { "ingame_tip_4", new[] { "스킵 버튼으로 블록 교체!", "Use skip button to swap blocks!", "スキップボタンでブロック交換！", "使用跳过按钮更换方块！" } },
            { "ingame_tip_5", new[] { "놓을 자리가 없으면 스킵 활용!", "No moves? Use the skip button!", "置く場所がない時はスキップを活用！", "无处放置时请使用跳过按钮！" } },
            { "ingame_tip_6", new[] { "긴 콤보로 최고 점수 도전!", "Aim for high scores with big combos!", "ロングコンボでハイスコアに挑戦！", "挑战超长连击刷新最高记录！" } },

            // Ingame GameOver Revive & Continuation
            { "ingame_continue_ad", new[] { "이어하기", "Continue", "コンティニュー", "继续游戏" } },
            { "ingame_continue_reward_badge", new[] { "+30 다이아", "+30 Dia", "+30 ダイヤ", "+30 钻石" } },
            { "ingame_revived_toast", new[] { "이어하기 성공! +30 다이아가 지급되었습니다!", "Revived! +30 Diamonds received!", "コンティニュー成功！30ダイヤ獲得！", "复活成功！获得30颗钻石！" } },

            // Shop Packages & Recommendations
            { "shop_pack_0_title", new[] { "일일 골드", "Daily Gold", "デイリーゴールド", "每日金币" } },
            { "shop_pack_0_reward", new[] { "+100 G\n(매일 1회 무료)", "+100 G\n(Free 1/Day)", "+100 G\n(毎日1回無料)", "+100 G\n(每日免费1次)" } },
            { "shop_pack_0_price", new[] { "무료 받기", "Claim Free", "無料受取", "免费领取" } },
            { "shop_pack_1_title", new[] { "일일 다이아", "Daily Diamonds", "デイリーダイヤ", "每日钻石" } },
            { "shop_pack_1_reward", new[] { "+30 다이아\n(광고 시청)", "+30 Diamonds\n(Watch Ad)", "+30 ダイヤ\n(広告視聴)", "+30 钻石\n(观看广告)" } },
            { "shop_pack_1_price", new[] { "광고 보고 받기", "Watch Ad", "広告を見る", "观看广告" } },
            { "shop_pack_2_title", new[] { "웰컴 팩", "Welcome Pack", "ウェルカムパック", "迎新礼包" } },
            { "shop_pack_2_reward", new[] { "민트 말랑이\n+1,000 G +100 다이아", "Mint Mallang\n+1,000 G +100 Dia", "ミントマラン\n+1,000 G +100 ダイヤ", "薄荷团子\n+1,000 G +100 钻石" } },
            { "shop_pack_2_price", new[] { "무료 받기", "Claim Free", "無料受取", "免费领取" } },
            { "shop_pack_claimed", new[] { "수령 완료", "Claimed", "受取完了", "已领取" } },
            { "shop_daily_gold_claimed_toast", new[] { "오늘의 일일 골드를 이미 받았습니다! 내일 다시 만나요~", "Daily gold already claimed today! See you tomorrow!", "本日のデイリーゴールドは受取済です！また明日！", "今日每日金币已领取！明天再来吧~" } },
            { "shop_daily_diamond_claimed_toast", new[] { "오늘의 일일 다이아를 이미 받았습니다! 내일 다시 만나요~", "Daily diamonds already claimed today! See you tomorrow!", "本日のデイリーダイヤは受取済です！また明日！", "今日每日钻石已领取！明天再来吧~" } },
            { "shop_welcome_pack_claimed_toast", new[] { "웰컴 팩을 이미 수령하셨습니다!", "Welcome pack already claimed!", "ウェルカムパックは既に受取済です！", "迎新礼包已领取！" } },
            { "shop_ad_diamond_toast", new[] { "광고 시청 완료! +30 다이아가 지급되었습니다!", "Ad watched! +30 Diamonds received!", "広告視聴完了！30ダイヤ獲得！", "广告观看完毕！获得30颗钻石！" } },
            { "reward_claim_title", new[] { "보상 획득 완료!", "Reward Claimed!", "報酬獲得完了！", "奖励领取成功！" } },
            { "reward_claim_btn", new[] { "확인", "Confirm", "確認", "确认" } },
            { "shop_rec_tip", new[] { "TIP: 다이아몬드는 매일 퀘스트 및 업적 달성 시에도 무료로 획득할 수 있습니다!\n스페셜 말랑이 픽업 소환으로 판을 시원하게 쓸어담아 보세요!", "TIP: Diamonds can be earned for free through daily quests and achievements!\nSummon Special Angel to clear the board with ease!", "TIP: ダイヤはデイリークエストや実績達成で無料獲得できます！\nスペシャルマランのピックアップ召喚で盤面を一掃しましょう！", "提示: 每日任务与成就奖励均可免费获取钻石！\n参与特选精选召唤，体验全屏清盘的神奇魔法！" } },
            { "shop_buy_price_format", new[] { "{0:N0} G 구매", "Buy ({0:N0} G)", "{0:N0} Gで購入", "{0:N0} G购买" } },

            // Mobile Settings (Haptics, Privacy Policy, Rates)
            { "settings_haptic_on", new[] { "진동: 켜짐", "Haptics: ON", "振動: ON", "振动: 开启" } },
            { "settings_haptic_off", new[] { "진동: 꺼짐", "Haptics: OFF", "振動: OFF", "振动: 关闭" } },
            { "settings_privacy_policy", new[] { "개인정보방침", "Privacy Policy", "プライバシーポリシー", "隐私政策" } },

            // Probability Modal Full Localization & Updated Breakthrough Notice
            { "prob_law_notice", new[] {
                "본 게임은 대한민국 게임산업진흥에 관한 법률 제33조에 따라\n확률형 아이템의 소환 확률 정보를 100% 투명하게 공개하고 있습니다.",
                "In compliance with transparent gaming regulations,\nall summon drop rates are 100% publicly disclosed.",
                "透明性のある運営方針に基づき、\n召喚アイテムの排出確率情報を100%公開しています。",
                "本游戏遵循透明运营准则，\n召唤抽卡所有概率信息均100%公开透明。"
            } },
            { "prob_th_item", new[] { "등장 항목 / 구성품", "Item / Reward", "登場アイテム / 構成品", "获得内容 / 道具" } },
            { "prob_th_type", new[] { "구분", "Type", "区分", "类别" } },
            { "prob_th_rate", new[] { "소환 확률", "Drop Rate", "召喚確率", "召唤概率" } },
            { "prob_row_0_name", new[] { "★ [올 클리어 엔젤] 스페셜 말랑이", "★ [All Clear Angel] Special Angel", "★ [オールクリア] スペシャルマラン", "★ [全清天使] 特选团子" } },
            { "prob_row_0_type", new[] { "스페셜 완제", "Full Mascot", "スペシャル完品", "特选整卡" } },
            { "prob_row_common", new[] { "일반 말랑이 4종 (핑크/민트/골드/퍼플)", "Common 4 Types (Pink/Mint/Gold/Purple)", "ノーマル4種 (ピンク/ミント/金/パープル)", "普通伙伴4种 (粉萌/薄荷/黄金/紫晶)" } },
            { "prob_row_rare", new[] { "희귀 말랑이 4종 (블루/베리/레몬/클라우드)", "Rare 4 Types (Blue/Berry/Lemon/Cloud)", "レア4種 (ブルー/ベリー/レモン/雲)", "稀有伙伴4种 (碧蓝/莓果/柠檬/云朵)" } },
            { "prob_type_shard_1", new[] { "조각 1개 (80%)", "1 Shard (80%)", "欠片1個 (80%)", "碎片1个 (80%)" } },
            { "prob_type_shard_5", new[] { "조각 5개 (20%)", "5 Shards (20%)", "欠片5個 (20%)", "碎片5个 (20%)" } },
            { "prob_row_total", new[] { "합계 (Total Probability)", "Total Probability", "合計確率 (Total)", "总计概率 (Total)" } },
            { "prob_notes_text", new[] {
                "<b>[안내 사항]</b>\n• 스페셜 말랑이 소환 확률이 <b>1.0%</b>로 대폭 증가 적용되었습니다.\n• 스페셜 말랑이를 중복 획득 시 <b>스페셜 조각 60개</b>로 지급되어 즉시 1회 돌파가 가능합니다.\n• 획득한 일반/희귀 조각은 <b>말랑이 성급 돌파(육성) 전용 재료</b>로 사용됩니다.\n• 소환 확률은 독립 시행으로 적용되며, 구매 전 확률 정보를 상시 확인할 수 있습니다.",
                "<b>[Important Notice]</b>\n• Special Angel drop rate is significantly boosted to <b>1.0%</b>.\n• Duplicate Special Angels convert to <b>60 Special Shards</b> for instant breakthrough.\n• Acquired shards are strictly used as <b>Star Breakthrough Materials</b>.\n• Drop rates apply independently per summon and can be checked anytime.",
                "<b>[ご案内]</b>\n• スペシャルマランの排出確率が<b>1.0%</b>に大幅アップ中！\n• スペシャル重複獲得時は<b>スペシャル欠片60個</b>支給ですぐに突破可能。\n• 獲得した欠片は<b>マランの限界突破（育成）専用素材</b>として使用されます。\n• 召喚確率は独立試行で適用され、常時ご確認いただけます。",
                "<b>[温馨提示]</b>\n• 特选天使团子召唤概率大幅提升至 <b>1.0%</b>！\n• 重复获得特选角色将直接转化为 <b>60个特选碎片</b>，可立即进行1次突破。\n• 获得的伙伴碎片专用于<b>伙伴星级突破与进阶养成</b>。\n• 召唤概率均为独立计算，购买前可随时查看。"
            } },

            // Pickup Hint & Mascot Detail Modal Localization
            { "pickup_hint_bottom", new[] {
                "★ [1.0% 확률 UP!] 픽업 소환으로 스페셜 말랑이 & 일반/희귀 조각을 획득하세요! ★",
                "★ [1.0% Rate UP!] Get Special Mascot & Shards through Pickup Summon! ★",
                "★ [1.0% 確率UP!] ピックアップ召喚でスペシャルマラン＆育成欠片を獲得！ ★",
                "★ [1.0% 概率UP!] 通过限定招募获取特别软萌与养成碎片！ ★"
            } },
            { "mascot_shards_owned", new[] { "보유 조각: {0}개", "Shards: {0}", "所持欠片: {0}個", "持有碎片: {0}个" } },
            { "mascot_section_concept", new[] { "[캐릭터 컨셉]", "[Character Concept]", "[キャラクターコンセプト]", "[角色设定]" } },
            { "mascot_section_story", new[] { "[말랑이 스토리]", "[Mallang Lore]", "[マランストーリー]", "[软萌背景故事]" } },
            { "mascot_section_skill", new[] { "[고유 스킬]", "[Unique Skill]", "[固有スキル]", "[专属技能]" } },
            { "mascot_detail_lore_header", new[] { "★ 말랑이 정보 & 스토리", "★ Mascot Info & Lore", "★ マラン情報 & ストーリー", "★ 伙伴情报与背景故事" } },
            { "mascot_shards_breakthrough_fmt", new[] {
                "{0}: <color=#2E86DE><b>{1}</b></color> / {2}개 (돌파 재료)",
                "{0}: <color=#2E86DE><b>{1}</b></color> / {2} (Breakthrough Material)",
                "{0}: <color=#2E86DE><b>{1}</b></color> / {2}個 (限界突破素材)",
                "{0}: <color=#2E86DE><b>{1}</b></color> / {2}个 (突破升星材料)"
            } },
            { "mascot_status_locked", new[] { "미보유 (구매 필요)", "Locked (Purchase Req.)", "未所持 (要購入)", "未拥有 (需购买)" } },
            { "mascot_shards_progress_fmt", new[] { "{0}\n조각 {1}/{2}", "{0}\nShards {1}/{2}", "{0}\n欠片 {1}/{2}", "{0}\n碎片 {1}/{2}" } },

            // Gacha Presentation Interactive Texts
            { "gacha_touch_to_open", new[] { "가챠볼을 터치하여 열어보세요!", "Tap the capsule to open!", "カプセルをタップして開けよう！", "点击扭蛋开启！" } },
            { "gacha_touch_to_open_fmt", new[] { "가챠볼을 터치하여 열어보세요! ({0}/{1})", "Tap the capsule to open! ({0}/{1})", "カプセルをタップして開けよう！ ({0}/{1})", "点击扭蛋开启！ ({0}/{1})" } },
            { "gacha_touch_to_continue", new[] { "화면을 터치하여 계속하기", "Tap screen to continue", "タップして次へ進む", "点击屏幕继续" } },
            { "gacha_touch_for_next_fmt", new[] { "화면을 터치하여 다음으로 ({0}/{1})", "Tap screen for next ({0}/{1})", "タップして次へ ({0}/{1})", "点击屏幕查看下一个 ({0}/{1})" } },
            { "gacha_special_descended", new[] { "★ 스페셜 강림! ★", "★ SPECIAL MASCOT! ★", "★ スペシャル降臨！ ★", "★ 特选天使降临！ ★" } },
            { "gacha_shards_gained_fmt", new[] { "+{0} 조각 획득!", "+{0} Shards Acquired!", "+{0}個の欠片獲得！", "+{0} 碎片获得！" } },
            { "gacha_climax_title", new[] {
                "<size=46><color=#FFE600>★ SPECIAL MASCOT! ★</color></size>\n<size=32><color=#FFFFFF>스페셜 말랑이 강림!</color></size>",
                "<size=46><color=#FFE600>★ SPECIAL MASCOT! ★</color></size>\n<size=32><color=#FFFFFF>Special Angel Descended!</color></size>",
                "<size=46><color=#FFE600>★ SPECIAL MASCOT! ★</color></size>\n<size=32><color=#FFFFFF>スペシャルマラン降臨！</color></size>",
                "<size=46><color=#FFE600>★ SPECIAL MASCOT! ★</color></size>\n<size=32><color=#FFFFFF>特选天使团子降临！</color></size>"
            } },

            // Pity System & Shop Ribbon Localizations
            { "pickup_top_ribbon", new[] {
                "★ [시즌 1] 천상의 천사 말랑이 스페셜 픽업 소환 ★",
                "★ [Season 1] Celestial Angel Mascot Special Pickup ★",
                "★ [シーズン1] 天使マラン スペシャルピックアップ召喚 ★",
                "★ [第1赛季] 天使团子 特别限定招募 ★"
            } },
            { "rec_top_ribbon", new[] {
                "★ [데일리 혜택] 매일 무료 보상 & 스페셜 스타터 팩! ★",
                "★ [Daily Benefit] Free Daily Rewards & Special Starter Pack! ★",
                "★ [デイリー特典] 毎日無料報酬＆特別スターターパック！ ★",
                "★ [每日福利] 免费每日奖励与特惠新手礼包！ ★"
            } },
            { "pickup_pity_title", new[] {
                "★ 60회 확정 소환 천장 게이지 ★",
                "★ 60-Pull Guaranteed Pity Gauge ★",
                "★ 60連確定天井ゲージ ★",
                "★ 60抽保底召唤量表 ★"
            } },
            { "pickup_pity_progress_fmt", new[] {
                "진행도: {0}/60 (다음 보상까지 {1}회)",
                "Progress: {0}/60 ({1} pulls to next reward)",
                "進行度: {0}/60 (次回報酬まであと{1}回)",
                "当前进度: {0}/60 (距离下一奖励还差{1}抽)"
            } },
            { "pickup_pity_hint_bottom", new[] {
                "★ 60회 소환 시 [천상의 천사 말랑이] 100% 확정! 10회 소환마다 5,000 골드 보너스! ★",
                "★ Guaranteed [Celestial Angel] at 60 pulls! +5,000 Gold bonus per 10 pulls! ★",
                "★ 60連で[天使マラン]100%確定！10連ごとに5,000ゴールドボーナス！ ★",
                "★ 60抽必得[特选天使团子]！每进行10抽额外赠送5,000金币！ ★"
            } }
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
