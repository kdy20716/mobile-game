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
            { "pickup_modal_feature_4", new[] { "• 일반/희귀 말랑이 조각 1개 또는 5개 획득! 60개 수집 시 즉시 획득!", "• Common/Rare shards (1 or 5) drop! Collect 60 shards to unlock!", "• ノーマル/レアの欠片1個または5個獲得！60個で解放！", "• 掉落1个或5个普通/稀有碎片！集齐60个即可解锁！" } },

            // Summon Result Modal
            { "summon_result_count_suffix", new[] { "회 소환 결과", "x Summon Results", "回召喚結果", "连召唤结果" } },
            { "summon_result_unlocked", new[] { "획득 완료!", "Acquired!", "獲得完了！", "获得成功！" } },
            { "summon_result_unlocked_desc", new[] { "최초 소환 성공! 강력한 보드 올 클리어 능력이 해금되었습니다!", "First summon success! Board All Clear skill is unlocked!", "初召喚成功！強力なボード全消去能力が解放されました！", "首次召唤成功！强力全屏引爆技能已解锁！" } },
            { "summon_result_duplicate", new[] { "중복 획득!", "Duplicate Acquired!", "重複獲得！", "重复获得！" } },
            { "summon_result_duplicate_desc", new[] { "스페셜 조각 60개로 즉시 변환되어 바로 돌파 가능!", "Instantly converted to 60 Special Shards for breakthrough!", "スペシャル欠片60個に即座変換！すぐに突破可能！", "直接转化为60个特选碎片，可立即进行突破！" } },
            { "summon_result_shards_title", new[] { "말랑이 조각 획득!", "Mallang Shards Acquired!", "マランの欠片獲得！", "获得伙伴碎片！" } },
            { "summon_result_shards_desc", new[] { "조각 60개를 모으면 해당 말랑이를 획득할 수 있어요!", "Collect 60 shards to acquire the mascot!", "欠片60個を集めるとマランを獲得できます！", "集齐60个碎片即可解锁该吉祥物！" } },
            { "mascot_owned_shards", new[] { "보유", "Owned", "所持", "持有" } },

            // Mascot Codex (냥냥시노비 도감 스타일)
            { "codex_title", new[] { "말랑이 도감", "Mascot Codex", "マラン図鑑", "软萌图鉴" } },
            { "codex_subtitle", new[] { "말랑이들을 모아 강화하고 강력한 능력을 돌파하세요!", "Collect Mallangs, level them up, and breakthrough!", "マランたちを集めて強化・限界突破しよう！", "收集软萌小伙伴，升级并突破更强上限！" } },
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
            { "mascot_btn_unlock", new[] { "해금하기 (조각 60개)", "Unlock (60 Shards)", "解放する (欠片60個)", "解锁 (60碎片)" } },
            { "mascot_max_level", new[] { "최고 레벨 달성", "MAX Level", "最大レベル達成", "已达最高等级" } },
            { "mascot_max_breakthrough", new[] { "최대 돌파 완료 (5★)", "MAX Breakthrough (5★)", "最大突破完了 (5★)", "已满星突破 (5★)" } },
            { "mascot_need_shards", new[] { "조각 60개로 해금", "Need 60 Shards", "欠片60個で解放", "需集齐60碎片解锁" } },
            { "mascot_obtain_pickup", new[] { "획득처: 픽업 소환", "Obtained via: Pickup Gacha", "獲得先: ピックアップ召喚", "获取途径：精选召唤" } },

            // 9 Mascots: Names, Titles, Abilities
            { "mascot_0_name", new[] { "핑크 말랑이", "Pink Mallang", "ピンクマラン", "粉萌团子" } },
            { "mascot_0_title", new[] { "기본 말랑이", "Starter Mallang", "基本マラン", "初识团子" } },
            { "mascot_0_desc", new[] { "고유능력: 추가 시간 보너스\n(턴 제한 시간이 넉넉해집니다)", "Ability: Extra Turn Time\n(Gives you more time per turn)", "固有能力: 追加時間ボーナス\n(ターンの制限時間が延長されます)", "固有能力: 额外时间加成\n(每回合放置方块时间更从容)" } },

            { "mascot_1_name", new[] { "민트 말랑이", "Mint Mallang", "ミントマラン", "薄荷团子" } },
            { "mascot_1_title", new[] { "스킵 마스터", "Skip Master", "スキップマスター", "跳过大师" } },
            { "mascot_1_desc", new[] { "고유능력: 스킵 스택 강화\n(시작 2개 / 최대 4개 보유 가능)", "Ability: Skip Stack Boost\n(Start with 2 / Hold up to 4 skips)", "固有能力: スキップスタック強化\n(開始2個 / 最大4個ストック可能)", "固有能力: 跳过次数强化\n(初始2次 / 最大可存4次跳过)" } },

            { "mascot_2_name", new[] { "골드 말랑이", "Gold Mallang", "ゴールドマラン", "黄金团子" } },
            { "mascot_2_title", new[] { "보물 사냥꾼", "Treasure Hunter", "トレジャーハンター", "寻宝猎人" } },
            { "mascot_2_desc", new[] { "고유능력: 라인 추가 점수\n(클리어 줄마다 +100점 추가 보너스)", "Ability: Line Clear Score\n(+100 extra bonus pts per cleared line)", "固有能力: ライン追加スコア\n(消去列ごとに+100点ボーナス)", "固有能力: 消除整行额外加分\n(每消除一行额外加100分)" } },

            { "mascot_3_name", new[] { "퍼플 말랑이", "Purple Mallang", "パープルマラン", "紫晶团子" } },
            { "mascot_3_title", new[] { "매직 큐브", "Magic Cube", "マジックキューブ", "魔方使者" } },
            { "mascot_3_desc", new[] { "고유능력: 2×2 매직 블록\n(5% 확률로 2×2 보라 블록 3개 소환)", "Ability: 2x2 Magic Blocks\n(5% chance to spawn 3 2x2 purple blocks)", "固有能力: 2×2マジックブロック\n(5%の確率で2×2紫ブロック3個召喚)", "固有能力: 2×2魔方方块\n(5%概率召唤3个2×2紫色方块)" } },

            { "mascot_4_name", new[] { "블루 말랑이", "Blue Mallang", "ブルーマラン", "碧蓝团子" } },
            { "mascot_4_title", new[] { "아쿠아 쉴드", "Aqua Shield", "アクアシールド", "水波护盾" } },
            { "mascot_4_desc", new[] { "고유능력: 아쿠아 쉴드\n(게임오버 위기 시 1회 부활 & 블록 재배치)", "Ability: Aqua Shield\n(Revives once upon game over & reshuffles)", "固有能力: アクアシールド\n(ピンチ時1回復活＆ブロック再配置)", "固有能力: 水波护盾\n(陷入死局时触发1次复活重抽方块)" } },

            { "mascot_5_name", new[] { "베리 말랑이", "Berry Mallang", "ベリーマラン", "莓果团子" } },
            { "mascot_5_title", new[] { "슈가 버스트", "Sugar Burst", "シュガーバースト", "糖爆甜心" } },
            { "mascot_5_desc", new[] { "고유능력: 슈가 버스트\n(콤보 달성 시 주변 블록 추가 폭파 & +15% 점수)", "Ability: Sugar Burst\n(Extra blast & +15% score on combos)", "固有能力: シュガーバースト\n(コンボ時周囲追加爆破＆スコア+15%)", "固有能力: 糖爆甜心\n(达成连击时额外爆破周围并提升15%分数)" } },

            { "mascot_6_name", new[] { "레몬 말랑이", "Lemon Mallang", "レモンマラン", "柠檬团子" } },
            { "mascot_6_title", new[] { "번개 팡", "Lemon Spark", "レモンスパーク", "闪电爆破" } },
            { "mascot_6_desc", new[] { "고유능력: 번개 팡\n(3연속 콤보 시 가로 한 줄 번개 즉시 폭파)", "Ability: Lemon Spark\n(Instantly zaps and clears a row at 3 combos)", "固有能力: レモンスパーク\n(3連続コンボ時雷で横1列即座爆破)", "固有能力: 闪电爆破\n(达成3连击时触发闪电瞬间消灭整行)" } },

            { "mascot_7_name", new[] { "클라우드 말랑이", "Cloud Mallang", "クラウドマラン", "云朵团子" } },
            { "mascot_7_title", new[] { "푹신 구름", "Fluffy Cloud", "ふわふわクラウド", "蓬松云朵" } },
            { "mascot_7_desc", new[] { "고유능력: 푹신 구름\n(시간 감소 속도 20% 완화 & 슬로우 피버)", "Ability: Fluffy Cloud\n(Slows time decay by 20% for cozy puzzle play)", "固有能力: ふわふわクラウド\n(時間減少速度20%緩和＆まったりプレイ)", "固有能力: 蓬松云朵\n(时间衰减速度减缓20%，更轻松畅快)" } },

            { "mascot_8_name", new[] { "스페셜 말랑이", "Special Angel", "スペシャルマラン", "星天使团子" } },
            { "mascot_8_title", new[] { "올 클리어 엔젤", "All Clear Angel", "オールクリアエンジェル", "全清天使" } },
            { "mascot_8_desc", new[] { "고유능력: 보드 올 클리어\n(스킬 터치 시 보드의 모든 블록 전멸 폭파!)", "Ability: Board Wipe Magic\n(Tap skill button to blast all blocks away!)", "固有能力: オールクリア\n(スキルタップで全ブロック一掃爆破！)", "固有能力: 全屏清盘魔法\n(点击专属大招按钮瞬间清除全场方块！)" } },

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
            { "ingame_resume", new[] { "계속하기 (Resume)", "Resume", "再開 (Resume)", "继续" } },
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
