using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Random = UnityEngine.Random;

namespace BlockBlast
{
    public class LobbyManager : MonoBehaviour
    {
        private static LobbyManager _instance;
        public static LobbyManager Instance
        {
            get
            {
                if (_instance == null) _instance = FindFirstObjectByType<LobbyManager>();
                return _instance;
            }
            private set => _instance = value;
        }

        private const string KEY_NICKNAME = "Mallang_Nickname";
        private const string KEY_BIO = "Mallang_Bio";
        private const string KEY_AVATAR = "Mallang_Avatar";
        private const string KEY_COINS = "Mallang_Coins";
        private const string KEY_DIAMONDS = "Mallang_Diamonds";
        private const string KEY_LOGGED_IN = "Mallang_LoggedIn";
        public const string KEY_THEME_OWNED_PREFIX = "Mallang_Theme_Owned_";
        public const string KEY_EQUIPPED_THEME = "Mallang_EquippedTheme";
        public const string KEY_LOBBY_THEME_OWNED_PREFIX = "Mallang_LobbyTheme_Owned_";
        public const string KEY_EQUIPPED_LOBBY_THEME = "Mallang_EquippedLobbyTheme";
        public const string KEY_SELECTED_MASCOT = "Selected_Mascot_Idx";
        public const string KEY_MASCOT_OWNED_PREFIX = "Mallang_Mascot_Owned_";
        private const string KEY_MOBILE_INIT_ECONOMY = "Mallang_Mobile_Init_v2";
        public const string KEY_MASCOT_SHARDS_PREFIX = "Mallang_Mascot_Shards_";
        public const string KEY_MASCOT_LEVEL_PREFIX = "Mallang_Mascot_Level_";
        public const string KEY_PICKUP_PITY = "Mallang_Pickup_PityCount";
        public const int PICKUP_PITY_TARGET = 60;

        public static readonly int[] ThemePrices = new int[] { 0, 1000, 2000, 3000 };
        public static readonly string[] ThemeNames = new string[] { "몽환의 밤", "캔디 랜드", "크리스탈 바다", "별빛 우주" };

        public static readonly int[] LobbyThemePrices = new int[] { 0, 1000, 2000 };
        public static readonly string[] LobbyThemeNames = new string[] { "몽환의 방", "달콤 캔디룸", "신비 바다룸" };
        public static readonly string[] LobbyThemeDescs = new string[] { "기본 로비 - 아늑한 파스텔 방", "달콤한 디저트와 와플 무대", "신비로운 바다 궁전 무대" };

        public const string KEY_MASCOT_BREAKTHROUGH_PREFIX = "Mallang_Mascot_Breakthrough_";

        public static readonly string[] MascotNames = new string[]
        {
            "핑크 말랑이", "민트 말랑이", "골드 말랑이", "퍼플 말랑이",
            "블루 말랑이", "베리 말랑이", "레몬 말랑이", "클라우드 말랑이",
            "엔젤 말랑이"
        };
        public static readonly string[] MascotTitles = new string[]
        {
            "기본 말랑이", "스킵 마스터", "보물 사냥꾼", "매직 큐브",
            "아쿠아 쉴드", "슈가 버스트", "번개 팡", "푹신 구름",
            "올 클리어 엔젤"
        };
        public static readonly string[] MascotAbilities = new string[]
        {
            "고유능력: 추가 시간 보너스\n(턴 제한 시간이 넉넉해집니다)",
            "고유능력: 스킵 스택 강화\n(시작 2개 / 최대 4개 보유 가능)",
            "고유능력: 라인 추가 점수\n(블록 라인 클리어 시 골드 보너스)",
            "고유능력: 2×2 매직 블록\n(확률적으로 보라 매직 블록 소환)",
            "고유능력: 아쿠아 쉴드\n(게임오버 위기 시 1회 부활 & 재배치)",
            "고유능력: 슈가 버스트\n(콤보 달성 시 추가 폭파 & +15% 점수)",
            "고유능력: 번개 팡\n(3연속 콤보 시 가로 한 줄 즉시 폭파)",
            "고유능력: 푹신 구름\n(시간 감소 속도 20% 완화 & 몽글 피버)",
            "고유능력: 엔젤 크로스 폭파 블록\n(스페셜 블록끼리 닿으면 가로·세로 줄 전부 폭파!)"
        };

        public struct MascotStats
        {
            public string uniqueBlockName;
            public string uniqueBlockShape;
            public string uniqueBlockAbility;
            public float extraTime;
            public int skipCount;
            public float bonusScore;
            public float bonusExp;
        }

        [System.Serializable]
        public struct MascotUniqueBlockInfo
        {
            public string blockName;
            public string shapeDesc;
            public string abilityDesc;
            public int[,] shapeMatrix;
            public Color blockColor;
            public bool isSpecial;
            public bool isOneByOne;
        }

        public static MascotUniqueBlockInfo GetMascotUniqueBlockInfo(int idx, int tier)
        {
            tier = Mathf.Clamp(tier, 0, 2);
            MascotUniqueBlockInfo info = new MascotUniqueBlockInfo();
            info.isSpecial = (tier >= 1);
            info.isOneByOne = (tier == 2);

            if (tier == 2)
            {
                // 5돌파 MAX: 1x1 초소형 단일 셀 특수 블록!
                info.shapeMatrix = new int[,] { { 1 } };
            }

            switch (idx)
            {
                case 0: // 핑크 (사랑/하트)
                    info.blockColor = (tier == 2) ? new Color(1.0f, 0.25f, 0.55f) : (tier == 1) ? new Color(1.0f, 0.35f, 0.60f) : new Color(1.0f, 0.48f, 0.64f);
                    if (tier == 0)
                    {
                        info.blockName = "러블리 하트 블록";
                        info.shapeDesc = "♥ 하트 모양 (3x3 중심 하트형)";
                        info.abilityDesc = "달콤한 딸기 시럽 가득한 3x3 중앙 특화 블록 (일반)";
                        info.shapeMatrix = new int[,] { { 1, 0, 1 }, { 1, 1, 1 }, { 0, 1, 0 } };
                    }
                    else if (tier == 1)
                    {
                        info.blockName = "스위트 하트 특수 블록";
                        info.shapeDesc = "♥ 하트 모양 (3x3 특수 블록화)";
                        info.abilityDesc = "★ [하트 팝 폭파] 클리어 시 주변 3x3 영역 하트 폭죽 폭파 & 대량 추가 점수!";
                        info.shapeMatrix = new int[,] { { 1, 0, 1 }, { 1, 1, 1 }, { 0, 1, 0 } };
                    }
                    else
                    {
                        info.blockName = "미니 하트 원더";
                        info.shapeDesc = "♥ 1×1 초소형 단일 셀";
                        info.abilityDesc = "★ [슈퍼 하트 버스트] 1x1 초소형! 터질 때 보드 위 랜덤 6개 블록 즉시 하트 팝 폭파!";
                    }
                    break;

                case 1: // 민트 (시간/클로버)
                    info.blockColor = (tier == 2) ? new Color(0.10f, 1.0f, 0.80f) : (tier == 1) ? new Color(0.20f, 0.95f, 0.75f) : new Color(0.31f, 0.88f, 0.71f);
                    if (tier == 0)
                    {
                        info.blockName = "행운 클로버 블록";
                        info.shapeDesc = "♣ 4엽 클로버 모양 (3x3 十자형)";
                        info.abilityDesc = "가로·세로 균형 잡힌 안정적인 십자 클리어 블록 (일반)";
                        info.shapeMatrix = new int[,] { { 0, 1, 0 }, { 1, 1, 1 }, { 0, 1, 0 } };
                    }
                    else if (tier == 1)
                    {
                        info.blockName = "타임 클로버 특수 블록";
                        info.shapeDesc = "♣ 4엽 클로버 모양 (3x3 특수 블록화)";
                        info.abilityDesc = "★ [시간 충전 젤리] 특수 블록이 터질 때마다 타이머 제한 시간 +5초 즉시 연장!";
                        info.shapeMatrix = new int[,] { { 0, 1, 0 }, { 1, 1, 1 }, { 0, 1, 0 } };
                    }
                    else
                    {
                        info.blockName = "미니 클로버 원더";
                        info.shapeDesc = "♣ 1×1 초소형 단일 셀";
                        info.abilityDesc = "★ [영원한 시간의 클로버] 1x1 초소형! 터질 때마다 타이머 +10초 대량 연장 & 스킵 +1회 즉시 회복!";
                    }
                    break;

                case 2: // 골드 (보물/다이아)
                    info.blockColor = (tier == 2) ? new Color(1.0f, 0.88f, 0.05f) : (tier == 1) ? new Color(1.0f, 0.82f, 0.15f) : new Color(1.0f, 0.75f, 0.26f);
                    if (tier == 0)
                    {
                        info.blockName = "골든 다이아 블록";
                        info.shapeDesc = "◆ 다이아몬드 마름모 모양 (3x3 마름모형)";
                        info.abilityDesc = "콤보 연결에 유리한 황금빛 마름모형 블록 (일반)";
                        info.shapeMatrix = new int[,] { { 0, 1, 0 }, { 1, 0, 1 }, { 0, 1, 0 } };
                    }
                    else if (tier == 1)
                    {
                        info.blockName = "황금 다이아 특수 블록";
                        info.shapeDesc = "◆ 다이아몬드 마름모 모양 (3x3 특수 블록화)";
                        info.abilityDesc = "★ [골드 피버 블래스트] 클리어 시 +500 골드 즉시 획득 및 골드 파티클 폭풍 발생!";
                        info.shapeMatrix = new int[,] { { 0, 1, 0 }, { 1, 0, 1 }, { 0, 1, 0 } };
                    }
                    else
                    {
                        info.blockName = "미니 골든 원더";
                        info.shapeDesc = "◆ 1×1 초소형 단일 셀";
                        info.abilityDesc = "★ [미다스의 황금 블록] 1x1 초소형! 터질 때마다 +1,000 골드 즉시 획득 & 가로 라인 전체 골드 폭파!";
                    }
                    break;

                case 3: // 퍼플 (번개/Z자)
                    info.blockColor = (tier == 2) ? new Color(0.85f, 0.30f, 1.0f) : (tier == 1) ? new Color(0.75f, 0.40f, 1.0f) : new Color(0.65f, 0.49f, 1.0f);
                    if (tier == 0)
                    {
                        info.blockName = "썬더 지그재그 블록";
                        info.shapeDesc = "⚡ 번개 Z자 모양 (2x3 지그재그형)";
                        info.abilityDesc = "테트리스 Z자형 틈새 공간 메꿈 특화 블록 (일반)";
                        info.shapeMatrix = new int[,] { { 1, 1, 0 }, { 0, 1, 1 } };
                    }
                    else if (tier == 1)
                    {
                        info.blockName = "체인 썬더 특수 블록";
                        info.shapeDesc = "⚡ 번개 Z자 모양 (2x3 특수 블록화)";
                        info.abilityDesc = "★ [체인 라이트닝 전격] 클리어 시 같은 색상 블록들을 번개로 연쇄 감전 폭파!";
                        info.shapeMatrix = new int[,] { { 1, 1, 0 }, { 0, 1, 1 } };
                    }
                    else
                    {
                        info.blockName = "미니 썬더 원더";
                        info.shapeDesc = "⚡ 1×1 초소형 단일 셀";
                        info.abilityDesc = "★ [천둥번개 낙뢰] 1x1 초소형! 배치 즉시 닿은 가로·세로 교차점 전격 폭파!";
                    }
                    break;

                case 4: // 블루 (워터/T자)
                    info.blockColor = (tier == 2) ? new Color(0.10f, 0.65f, 1.0f) : (tier == 1) ? new Color(0.20f, 0.70f, 1.0f) : new Color(0.36f, 0.77f, 1.0f);
                    if (tier == 0)
                    {
                        info.blockName = "아쿠아 볼록 T자 블록";
                        info.shapeDesc = "⚓ 볼록 T자 모양 (2x3 돌출형)";
                        info.abilityDesc = "3열 라인과 1열 돌출을 한 번에 맞추는 볼록형 블록 (일반)";
                        info.shapeMatrix = new int[,] { { 0, 1, 0 }, { 1, 1, 1 } };
                    }
                    else if (tier == 1)
                    {
                        info.blockName = "쓰나미 T자 특수 블록";
                        info.shapeDesc = "⚓ 볼록 T자 모양 (2x3 특수 블록화)";
                        info.abilityDesc = "★ [해일 라인 관통] 클리어 시 강력한 쓰나미 파도가 가로 전체 1줄을 휩쓸어 자동 소거!";
                        info.shapeMatrix = new int[,] { { 0, 1, 0 }, { 1, 1, 1 } };
                    }
                    else
                    {
                        info.blockName = "미니 아쿠아 원더";
                        info.shapeDesc = "⚓ 1×1 초소형 단일 셀";
                        info.abilityDesc = "★ [대양의 소용돌이] 1x1 초소형! 터질 때 보드 최하단 2개 라인을 깨끗이 씻어내어 공간 대량 확보!";
                    }
                    break;

                case 5: // 베리 (부메랑/L자)
                    info.blockColor = (tier == 2) ? new Color(1.0f, 0.05f, 0.25f) : (tier == 1) ? new Color(1.0f, 0.15f, 0.35f) : new Color(0.92f, 0.22f, 0.44f);
                    if (tier == 0)
                    {
                        info.blockName = "부메랑 코너 L자 블록";
                        info.shapeDesc = "🪃 코너 L자 모양 (3x3 대형 꺾임형)";
                        info.abilityDesc = "구석진 3x3 코너 라인을 시원하게 채워주는 블록 (일반)";
                        info.shapeMatrix = new int[,] { { 1, 0, 0 }, { 1, 0, 0 }, { 1, 1, 1 } };
                    }
                    else if (tier == 1)
                    {
                        info.blockName = "체리 폭탄 L자 특수 블록";
                        info.shapeDesc = "🪃 코너 L자 모양 (3x3 특수 블록화)";
                        info.abilityDesc = "★ [크로스 메가 밤] 클리어 시 꺾인 코너 중심 반경 2칸 십자 폭탄 폭파!";
                        info.shapeMatrix = new int[,] { { 1, 0, 0 }, { 1, 0, 0 }, { 1, 1, 1 } };
                    }
                    else
                    {
                        info.blockName = "미니 베리 원더";
                        info.shapeDesc = "🪃 1×1 초소형 단일 셀";
                        info.abilityDesc = "★ [초소형 핵폭탄 젤리] 1x1 초소형! 터질 때 반경 3x3(9칸) 전 영역 초토화 폭파!";
                    }
                    break;

                case 6: // 레몬 (큐브/2x2)
                    info.blockColor = (tier == 2) ? new Color(1.0f, 0.78f, 0.0f) : (tier == 1) ? new Color(1.0f, 0.82f, 0.10f) : new Color(1.0f, 0.88f, 0.20f);
                    if (tier == 0)
                    {
                        info.blockName = "스위트 큐브 블록";
                        info.shapeDesc = "■ 2x2 정사각 큐브 모양 (2x2 큐브형)";
                        info.abilityDesc = "보드 어디든 쏙 들어가는 고효율 2x2 정사각 블록 (일반)";
                        info.shapeMatrix = new int[,] { { 1, 1 }, { 1, 1 } };
                    }
                    else if (tier == 1)
                    {
                        info.blockName = "비타민 큐브 특수 블록";
                        info.shapeDesc = "■ 2x2 정사각 큐브 모양 (2x2 특수 블록화)";
                        info.abilityDesc = "★ [비타민 부스터] 클리어 시 경험치 +50% 획득 및 다음 3턴간 모든 클리어 점수 2배!";
                        info.shapeMatrix = new int[,] { { 1, 1 }, { 1, 1 } };
                    }
                    else
                    {
                        info.blockName = "미니 레몬 원더";
                        info.shapeDesc = "■ 1×1 초소형 단일 셀";
                        info.abilityDesc = "★ [울트라 레몬 젤리] 1x1 초소형! 터질 때 빈 칸 8칸 자동 채움 후 즉시 콤보 연계 폭파!";
                    }
                    break;

                case 7: // 클라우드 (롱바/1x4)
                    info.blockColor = (tier == 2) ? new Color(0.40f, 0.75f, 1.0f) : (tier == 1) ? new Color(0.50f, 0.78f, 1.0f) : new Color(0.60f, 0.82f, 1.0f);
                    if (tier == 0)
                    {
                        info.blockName = "와이드 롱바 블록";
                        info.shapeDesc = "━ 1x4 일자형 롱바 모양 (1x4 직사각 라인)";
                        info.abilityDesc = "가로 4칸을 시원하게 관통하는 직사각형 롱바 블록 (일반)";
                        info.shapeMatrix = new int[,] { { 1, 1, 1, 1 } };
                    }
                    else if (tier == 1)
                    {
                        info.blockName = "무지개 롱바 특수 블록";
                        info.shapeDesc = "━ 1x4 일자형 롱바 모양 (1x4 특수 블록화)";
                        info.abilityDesc = "★ [무지개 라인 관통] 줄을 다 채우지 않아도 놓이는 즉시 해당 가로 라인 전체 싹쓸이 관통!";
                        info.shapeMatrix = new int[,] { { 1, 1, 1, 1 } };
                    }
                    else
                    {
                        info.blockName = "미니 클라우드 원더";
                        info.shapeDesc = "━ 1×1 초소형 단일 셀";
                        info.abilityDesc = "★ [천공의 구름 쉴드] 1x1 초소형! 터질 때 게임오버 1회 방지 배리어 생성 & 콤보 유지!";
                    }
                    break;

                case 8: // 엔젤 (스페셜/십자)
                default:
                    info.blockColor = (tier == 2) ? new Color(1.0f, 0.70f, 0.90f) : (tier == 1) ? new Color(1.0f, 0.78f, 0.92f) : new Color(1.0f, 0.85f, 0.95f);
                    if (tier == 0)
                    {
                        info.blockName = "엔젤 크로스 블록";
                        info.shapeDesc = "✝ 대형 십자 크로스 모양 (3x3 십자형)";
                        info.abilityDesc = "성스러운 빛을 머금은 십자형 스페셜 블록 (일반)";
                        info.shapeMatrix = new int[,] { { 0, 1, 0 }, { 1, 1, 1 }, { 0, 1, 0 } };
                    }
                    else if (tier == 1)
                    {
                        info.blockName = "성스러운 십자 특수 블록";
                        info.shapeDesc = "✝ 대형 십자 크로스 모양 (3x3 특수 블록화)";
                        info.abilityDesc = "★ [스페셜 체인 공명] 특수 블록끼리 서로 맞닿으면 줄을 안 채워도 닿은 블록 기준 가로·세로 전 라인 일제 대폭파!";
                        info.shapeMatrix = new int[,] { { 0, 1, 0 }, { 1, 1, 1 }, { 0, 1, 0 } };
                    }
                    else
                    {
                        info.blockName = "미니 엔젤 원더";
                        info.shapeDesc = "✝ 1×1 초소형 단일 셀";
                        info.abilityDesc = "★ [기적의 올 클리어 엔젤] 1x1 초소형! 어디든 놓는 즉시 닿은 가로·세로 전 라인 폭파 + 보드 전체 올 클리어 발동!";
                    }
                    break;
            }

            return info;
        }

        public static MascotStats GetMascotStats(int idx, int level)
        {
            level = Mathf.Max(1, level);
            MascotStats stats = new MascotStats();

            int stars = GetBreakthroughStars(idx);
            int blockTier = (stars >= 5) ? 2 : (stars >= 2) ? 1 : 0;
            var uInfo = GetMascotUniqueBlockInfo(idx, blockTier);
            stats.uniqueBlockName = uInfo.blockName;
            stats.uniqueBlockShape = uInfo.shapeDesc;
            stats.uniqueBlockAbility = uInfo.abilityDesc;

            if (idx == 8)
            {
                stats.extraTime = 5.0f + (level - 1) * 0.25f;
                stats.skipCount = 4 + (level / 10);
                stats.bonusScore = 15.0f + (level - 1) * 1.0f;
                stats.bonusExp = 15.0f + (level - 1) * 1.0f;
            }
            else
            {
                switch (idx)
                {
                    case 0:
                        stats.extraTime = 1.0f + (level - 1) * 0.05f;
                        stats.skipCount = 1;
                        stats.bonusScore = 8.0f + (level - 1) * 0.6f;
                        stats.bonusExp = 8.0f + (level - 1) * 0.6f;
                        break;
                    case 1:
                        stats.extraTime = 4.0f + (level - 1) * 0.20f;
                        stats.skipCount = 1 + (level / 25);
                        stats.bonusScore = 5.0f + (level - 1) * 0.4f;
                        stats.bonusExp = 3.0f + (level - 1) * 0.2f;
                        break;
                    case 2:
                        stats.extraTime = 1.0f;
                        stats.skipCount = 2 + (level / 15);
                        stats.bonusScore = 3.0f + (level - 1) * 0.3f;
                        stats.bonusExp = 10.0f + (level - 1) * 0.8f;
                        break;
                    case 3:
                        stats.extraTime = 3.5f + (level - 1) * 0.18f;
                        stats.skipCount = 2 + (level / 15);
                        stats.bonusScore = 4.0f + (level - 1) * 0.3f;
                        stats.bonusExp = 4.0f + (level - 1) * 0.3f;
                        break;
                    case 4:
                        stats.extraTime = 2.5f + (level - 1) * 0.15f;
                        stats.skipCount = 3 + (level / 10);
                        stats.bonusScore = 5.0f + (level - 1) * 0.3f;
                        stats.bonusExp = 5.0f + (level - 1) * 0.3f;
                        break;
                    case 5:
                        stats.extraTime = 1.5f + (level - 1) * 0.05f;
                        stats.skipCount = 2 + (level / 20);
                        stats.bonusScore = 10.0f + (level - 1) * 0.7f;
                        stats.bonusExp = 4.0f + (level - 1) * 0.2f;
                        break;
                    case 6:
                        stats.extraTime = 2.0f + (level - 1) * 0.08f;
                        stats.skipCount = 2;
                        stats.bonusScore = 9.0f + (level - 1) * 0.6f;
                        stats.bonusExp = 9.0f + (level - 1) * 0.6f;
                        break;
                    case 7:
                        stats.extraTime = 4.5f + (level - 1) * 0.22f;
                        stats.skipCount = 1;
                        stats.bonusScore = 4.0f + (level - 1) * 0.2f;
                        stats.bonusExp = 12.0f + (level - 1) * 0.9f;
                        break;
                }
            }
            return stats;
        }

        public static string GetMascotGrowthFocusText(int idx)
        {
            switch (idx)
            {
                case 0: return "특화: 레벨업 시 추가 점수 & 추가 경험치 집중 상승!";
                case 1: return "특화: 레벨업 시 추가 시간 & 추가 점수 집중 상승!";
                case 2: return "특화: 레벨업 시 추가 경험치 & 스킵 갯수 집중 상승!";
                case 3: return "특화: 레벨업 시 추가 시간 & 스킵 갯수 집중 상승!";
                case 4: return "특화: 레벨업 시 스킵 갯수 & 추가 시간 집중 상승!";
                case 5: return "특화: 레벨업 시 추가 점수 & 스킵 갯수 집중 상승!";
                case 6: return "특화: 레벨업 시 추가 점수 & 추가 경험치 동시 상승!";
                case 7: return "특화: 레벨업 시 추가 경험치 & 추가 시간 집중 상승!";
                case 8: return "특화: 올라운드 전 스텟 강력 성장 (시간·스킵·점수·경험치)!";
                default: return "";
            }
        }
        public static readonly int[] BreakthroughCostsNormal = new int[] { 30, 40, 50, 60, 77 }; // 5 breakthroughs
        public static readonly int[] BreakthroughCostsSpecial = new int[] { 60, 60, 60, 60, 60 }; // 60 shards each
        public static readonly int[] MascotPricesCoins = new int[] { 0, 1000, 2000, 3000, 5000, 5000, 5000, 5000, 10000 };
        public static readonly int[] MascotPricesDiamonds = new int[] { 0, 50, 100, 150, 300, 300, 300, 300, 800 };

        public static int GetMascotShards(int idx)
        {
            return PlayerPrefs.GetInt(KEY_MASCOT_SHARDS_PREFIX + idx, 0);
        }

        public static void SetMascotShards(int idx, int count)
        {
            PlayerPrefs.SetInt(KEY_MASCOT_SHARDS_PREFIX + idx, Mathf.Max(0, count));
        }

        public static void AddMascotShards(int idx, int count)
        {
            SetMascotShards(idx, GetMascotShards(idx) + count);
        }

        public static int GetMascotLevel(int idx)
        {
            return PlayerPrefs.GetInt(KEY_MASCOT_LEVEL_PREFIX + idx, 1);
        }

        public static void SetMascotLevel(int idx, int level)
        {
            PlayerPrefs.SetInt(KEY_MASCOT_LEVEL_PREFIX + idx, Mathf.Max(1, level));
        }

        public static int GetBreakthroughStars(int idx)
        {
            return Mathf.Clamp(PlayerPrefs.GetInt(KEY_MASCOT_BREAKTHROUGH_PREFIX + idx, 0), 0, 5);
        }

        public static void SetBreakthroughStars(int idx, int stars)
        {
            PlayerPrefs.SetInt(KEY_MASCOT_BREAKTHROUGH_PREFIX + idx, Mathf.Clamp(stars, 0, 5));
        }

        public static int GetMaxLevelCap(int idx)
        {
            int stars = GetBreakthroughStars(idx);
            return Mathf.Clamp(50 + stars * 10, 50, 100);
        }

        public static int GetBreakthroughCost(int idx)
        {
            int stars = GetBreakthroughStars(idx);
            if (stars >= 5) return -1;
            if (idx == 8) return BreakthroughCostsSpecial[stars];
            return BreakthroughCostsNormal[stars];
        }

        public static int GetLevelUpCoinCost(int idx)
        {
            int curLvl = GetMascotLevel(idx);
            return curLvl * 20;
        }

        public static string GetLocalizedMascotName(int idx)
        {
            return LocalizationManager.Get($"mascot_{idx}_name", (idx >= 0 && idx < MascotNames.Length) ? MascotNames[idx] : "");
        }

        public static string GetLocalizedMascotTitle(int idx)
        {
            return LocalizationManager.Get($"mascot_{idx}_title", (idx >= 0 && idx < MascotTitles.Length) ? MascotTitles[idx] : "");
        }

        public static string GetLocalizedMascotDesc(int idx)
        {
            return LocalizationManager.Get($"mascot_{idx}_desc", GetMascotAbilityDescription(idx, GetMascotLevel(idx)));
        }

        public static string GetLocalizedMascotConcept(int idx)
        {
            return LocalizationManager.Get($"mascot_{idx}_concept", "");
        }

        public static string GetLocalizedMascotStory(int idx)
        {
            return LocalizationManager.Get($"mascot_{idx}_story", "");
        }

        public static string GetMascotAbilityDescription(int idx, int level)
        {
            level = Mathf.Max(1, level);
            switch (idx)
            {
                case 0:
                    float sec = 1.0f + (level - 1) * 0.05f;
                    return $"고유능력: 추가 시간 +{sec:F2}초\n(첫 턴 {180 + sec:F1}초 / 최소 {5 + sec:F1}초)";
                case 1:
                    int startStock = 1 + Mathf.Min(3, level / 20);
                    int maxStock = 3 + (level / 15);
                    return $"고유능력: 스킵 스택 강화\n(시작 {startStock}개 / 최대 {maxStock}개 보유)";
                case 2:
                    int bonus = 100 + (level - 1) * 5;
                    return $"고유능력: 라인 추가 점수\n(줄당 +{bonus}점 추가 보너스)";
                case 3:
                    int rate = 5 + (level / 10);
                    return $"고유능력: 2×2 매직 블록\n({rate}% 확률로 2×2 보라 블록 소환)";
                case 4:
                    return $"고유능력: 아쿠아 쉴드\n(게임오버 위기 시 1회 부활 & 재배치)";
                case 5:
                    float burst = 15f + (level - 1) * 0.2f;
                    return $"고유능력: 슈가 버스트\n(콤보 달성 시 추가 폭파 & +{burst:F1}% 점수)";
                case 6:
                    return $"고유능력: 번개 팡\n(3연속 콤보 시 가로 한 줄 번개 즉시 폭파)";
                case 7:
                    float slow = Mathf.Min(40f, 20f + (level - 1) * 0.2f);
                    return $"고유능력: 푹신 구름\n(시간 감소 속도 {slow:F1}% 완화 & 몽글 피버)";
                case 8:
                    int lines = Mathf.Max(15, 30 - (level / 10));
                    return $"고유능력: 보드 올 클리어\n(스킬 터치 시 모든 블록 폭파!\n{lines}줄 클리어마다 충전)";
                default:
                    return "";
            }
        }

        [Header("Lobby Root & Groups")]
        [SerializeField] private GameObject lobbyRoot;
        [SerializeField] private CanvasGroup lobbyCanvasGroup;

        [Header("Top Right Profile, Coins & Diamonds")]
        [SerializeField] private Button profileBtn;
        [SerializeField] private Image profileBtnAvatar;
        [SerializeField] private TMP_Text lobbyCoinsText;
        [SerializeField] private TMP_Text lobbyDiamondsText;

        [Header("Lobby Mascots & Floating Labels (0:Pink, 1:Mint, 2:Gold, 3:Purple)")]
        [SerializeField] private RectTransform[] partyMascots;
        [SerializeField] private GameObject[] partyLabels;
        [SerializeField] private Image[] partyLabelBgs;
        [SerializeField] private TMP_Text[] partyLabelTexts;
        [SerializeField] private GameObject[] partyGlowAuras;

        [Header("Bottom Action Button")]
        [SerializeField] private Button btnPlayGame;
        [SerializeField] private Image btnPlayGameBg;
        [SerializeField] private TMP_Text btnPlayGameText;
        [SerializeField] private Image btnPlayGameGlow;

        [Header("Language Logos")]
        [SerializeField] private Image lobbyLogoImage;
        [SerializeField] private Sprite[] languageLogos;

        [Header("Profile Modal")]
        [SerializeField] private GameObject profileModal;
        [SerializeField] private Image profileModalAvatar;
        [SerializeField] private Image profileModalAvatarPlate;
        [SerializeField] private Image profileBtnAvatarPlate;
        [SerializeField] private TMP_Text profileModalNickname;
        [SerializeField] private TMP_InputField profileModalNicknameInput;
        [SerializeField] private TMP_InputField profileModalTagInput;
        [SerializeField] private Button btnStartEditNick;
        [SerializeField] private Button btnCheckTag;
        [SerializeField] private Button btnConfirmNick;
        [SerializeField] private TMP_Text profileModalNickStatus;
        [SerializeField] private TMP_InputField profileModalBioInput;
        [SerializeField] private TMP_Text profileModalBestScore;
        [SerializeField] private TMP_Text profileModalCoins;
        [SerializeField] private Button[] avatarSelectButtons;
        [SerializeField] private Button btnLogout;
        [SerializeField] private Button btnCloseProfile;

        [Header("Login Modal")]
        [SerializeField] private GameObject loginModal;
        [SerializeField] private TMP_InputField loginIdInput;
        [SerializeField] private TMP_InputField loginPwInput;
        [SerializeField] private Button btnSubmitLogin;
        [SerializeField] private Button btnGoogleLogin;
        [SerializeField] private Button btnCloseLogin;
        [SerializeField] private TMP_Text loginStatusText;

        [Header("Shop Modal & Currency")]
        [SerializeField] private GameObject shopModal;
        [SerializeField] private TMP_Text shopCoinsText;
        [SerializeField] private TMP_Text shopDiamondsText;
        [SerializeField] private Button btnCloseShop;

        [Header("Mascot Management Modal (Legacy Support)")]
        [SerializeField] private GameObject mascotModal;
        [SerializeField] private Button btnCloseMascotModal;
        [SerializeField] private Button[] mascotActionButtons;
        [SerializeField] private TMP_Text[] mascotActionTexts;
        [SerializeField] private TMP_Text[] mascotStatusTexts;
        [SerializeField] private Button[] mascotUpgradeButtons;
        [SerializeField] private TMP_Text[] mascotUpgradeTexts;
        [SerializeField] private TMP_Text[] mascotLevelTexts;
        [SerializeField] private TMP_Text[] mascotShardTexts;
        [SerializeField] private TMP_Text[] mascotAbilityTexts;

        [Header("Mascot Codex Modal (냥냥시노비 도감 스타일)")]
        [SerializeField] private GameObject mascotCodexModal;
        [SerializeField] private Button btnCloseMascotCodex;
        [SerializeField] private TMP_Text codexTitleText;
        [SerializeField] private TMP_Text codexSubtitleText;
        [SerializeField] private TMP_Text codexCollectionCountText;
        [SerializeField] private Button[] codexCardButtons;
        [SerializeField] private Image[] codexCardAvatars;
        [SerializeField] private TMP_Text[] codexCardNames;
        [SerializeField] private TMP_Text[] codexCardLevels;
        [SerializeField] private TMP_Text[] codexCardStars;
        [SerializeField] private GameObject[] codexCardLockedOverlays;
        [SerializeField] private TMP_Text[] codexCardStatusBadges;
        [SerializeField] private TMP_Text[] codexCardRarityTexts;

        [Header("Mascot Detail & Growth Modal (강화/돌파/장착 팝업)")]
        [SerializeField] private GameObject mascotDetailModal;
        [SerializeField] private Button btnCloseMascotDetail;
        [SerializeField] private Image detailMascotAvatar;
        [SerializeField] private TMP_Text detailMascotName;
        [SerializeField] private TMP_Text detailMascotTitle;
        [SerializeField] private TMP_Text detailRarityText;
        [SerializeField] private TMP_Text detailStarsText;
        [SerializeField] private TMP_Text detailLevelText;
        [SerializeField] private Image detailLevelFill;
        [SerializeField] private TMP_Text detailShardsText;
        [SerializeField] private TMP_Text detailAbilityDesc;
        [SerializeField] private Button btnDetailLevelUp;
        [SerializeField] private TMP_Text txtDetailLevelUp;
        [SerializeField] private Button btnDetailBreakthrough;
        [SerializeField] private TMP_Text txtDetailBreakthrough;
        [SerializeField] private Button btnDetailEquip;
        [SerializeField] private TMP_Text txtDetailEquip;
        [Header("Mascot Detail Stat Gauges (일자 게이지 및 강화 수치 프리뷰)")]
        [SerializeField] private TMP_Text detailStoryText;
        [Header("Mascot Detail Unique Block Evolution (3단계 동시 표시 & 잠금 회색화)")]
        [SerializeField] private RectTransform[] detailBlockContainers = new RectTransform[3]; // 0=기본, 1=2돌, 2=5돌
        [SerializeField] private Image[] detailStageCardBgs = new Image[3];
        [SerializeField] private TMP_Text[] detailStageBadges = new TMP_Text[3];
        [SerializeField] private TMP_Text[] detailStageNames = new TMP_Text[3];
        [SerializeField] private TMP_Text[] detailStageStatuses = new TMP_Text[3];
        [SerializeField] private GameObject[] detailStageLocks = new GameObject[3];
        [SerializeField] private Button[] detailStageButtons = new Button[3];
        [SerializeField] private TMP_Text detailEvolutionOverview;
        [SerializeField] private Sprite miniBlockTileSprite;
        private int _selectedEvolutionPreviewTier = 0;
        [SerializeField] private TMP_Text detailGrowthFocusText;
        [SerializeField] private Image detailGaugeTimeFill;
        [SerializeField] private TMP_Text detailStatTimeVal;
        [SerializeField] private Image detailGaugeSkipFill;
        [SerializeField] private TMP_Text detailStatSkipVal;
        [SerializeField] private Image detailGaugeScoreFill;
        [SerializeField] private TMP_Text detailStatScoreVal;
        [SerializeField] private Image detailGaugeExpFill;
        [SerializeField] private TMP_Text detailStatExpVal;
        private int _selectedDetailMascotIdx = 0;

        [Header("Pickup Skill Details Modal")]
        [SerializeField] private GameObject pickupSkillDetailModal;
        [SerializeField] private Button btnClosePickupSkillDetail;
        [SerializeField] private Button btnOpenPickupSkillDetail;
        [SerializeField] private TMP_Text txtPickupBannerBadge;
        [SerializeField] private TMP_Text txtPickupSummon1;
        [SerializeField] private TMP_Text txtPickupSummon10;
        [SerializeField] private TMP_Text txtPickupBtnRates;
        [SerializeField] private TMP_Text txtPickupBtnDetail;

        [Header("Legal & Compliance: Probability Modal")]
        [SerializeField] private GameObject probabilityModal;
        [SerializeField] private Button btnCloseProbability;
        [SerializeField] private Button btnConfirmProbability;
        [SerializeField] private Button btnProbabilityDarkBg;

        [Header("Mobile Settings: Haptics & Privacy")]
        [SerializeField] private Button btnHapticToggle;
        [SerializeField] private TMP_Text hapticToggleText;
        [SerializeField] private Button btnPrivacyPolicy;

        [Header("Summon Result Modal")]
        [SerializeField] private GameObject summonResultModal;
        [SerializeField] private Button btnCloseSummonResult;
        [SerializeField] private TMP_Text summonResultTitleText;
        [SerializeField] private TMP_Text summonResultHighlightText;
        [SerializeField] private TMP_Text summonResultShardsText;
        [SerializeField] private Image summonResultMascotIcon;

        [Header("Theme Shop")]
        [SerializeField] private Image inGameBackgroundImg;
        [SerializeField] private Button[] themeActionButtons;
        [SerializeField] private TMP_Text[] themeActionTexts;
        [SerializeField] private TMP_Text[] themePriceTexts;
        [SerializeField] private Sprite[] themeSprites;

        [Header("Lobby Background & Themes")]
        [SerializeField] private Image lobbyBackgroundImg;
        [SerializeField] private Sprite[] lobbyThemeSprites;
        [SerializeField] private Button[] lobbyThemeActionButtons;
        [SerializeField] private TMP_Text[] lobbyThemeActionTexts;
        [SerializeField] private TMP_Text[] lobbyThemePriceTexts;

        [Header("Shop 5 Vertical Tabs & Panels")]
        [SerializeField] private Button[] shopTabButtons;
        [SerializeField] private Image[] shopTabBgs;
        [SerializeField] private TMP_Text[] shopTabTexts;
        [SerializeField] private GameObject[] shopTabPanels;
        [SerializeField] private GameObject shopRecommendedPanel;
        [SerializeField] private GameObject shopPickupPanel;
        [SerializeField] private GameObject shopMascotsPanel;
        [SerializeField] private GameObject shopInGameThemesPanel;
        [SerializeField] private GameObject shopLobbyThemesPanel;
        [SerializeField] private AnimatedPickupBannerController pickupBannerController;
        [SerializeField] private ShopUIAnimationController shopAnimController;

        [Header("Shop Packages & Summons & Mascot Items")]
        [SerializeField] private Button[] shopPackageButtons;
        [SerializeField] private Button btnSummon1;
        [SerializeField] private Button btnSummon10;
        [SerializeField] private Button[] mascotShopActionButtons;
        [SerializeField] private TMP_Text[] mascotShopActionTexts;

        [Header("Custom Shop Tab Sprites")]
        public bool useNanoBananaTabButtons = true;
        [SerializeField] private Sprite tabGameActiveSprite;
        [SerializeField] private Sprite tabLobbyActiveSprite;
        [SerializeField] private Sprite tabInactiveSprite;
        [SerializeField] private Sprite tabOriginalPillSprite;
        [SerializeField] private Sprite tabVerticalActiveSprite;
        [SerializeField] private Sprite tabVerticalInactiveSprite;

        private int _currentShopTab = 0;

        [Header("Settings Modal")]
        [SerializeField] private GameObject settingsModal;
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Button btnCloseSettings;

        [Header("Language Selection (Settings Modal)")]
        [SerializeField] private Button[] languageButtons;
        [SerializeField] private Image[] languageButtonBgs;
        [SerializeField] private TMP_Text[] languageButtonTexts;
        [SerializeField] private TMP_Text settingsTitleText;
        [SerializeField] private TMP_Text settingsBgmText;
        [SerializeField] private TMP_Text settingsSfxText;
        [SerializeField] private TMP_Text settingsLangText;
        [SerializeField] private TMP_Text shopTitleText;
        [SerializeField] private TMP_Text helpTitleText;
        [SerializeField] private TMP_Text helpConfirmText;
        [SerializeField] private TMP_Text profileTitleText;
        [SerializeField] private TMP_Text settingsVersionText;

        [Header("Help Modal")]
        [SerializeField] private GameObject helpModal;
        [SerializeField] private Button btnCloseHelp;
        [SerializeField] private Button btnConfirmHelp;

        [Header("Party Stage Tip")]
        [SerializeField] private TMP_Text partyTipText;

        [Header("Player Level Badge")]
        [SerializeField] private TMP_Text playerLevelBadgeText;

        [Header("Quit Modal (PC)")]
        [SerializeField] private GameObject quitModal;
        [SerializeField] private Button btnQuitConfirmYes;
        [SerializeField] private Button btnQuitConfirmNo;
        [SerializeField] private Button btnQuitDarkBg;
        [SerializeField] private Button btnOpenQuitModal;
        [SerializeField] private TMP_Text quitModalTitleText;
        [SerializeField] private TMP_Text quitModalDescText;
        [SerializeField] private TMP_Text quitModalYesText;
        [SerializeField] private TMP_Text quitModalNoText;
        [SerializeField] private TMP_Text settingsQuitButtonText;

        [Header("Screen Settings (Aspect Ratio & Window Mode)")]
        [SerializeField] private TMP_Text settingsAspectTitleText;
        [SerializeField] private Button[] aspectButtons; // 0: 16:9, 1: 16:10, 2: 4:3, 3: 9:16
        [SerializeField] private Image[] aspectBgs;
        [SerializeField] private TMP_Text[] aspectTexts;

        [SerializeField] private TMP_Text settingsWindowModeTitleText;
        [SerializeField] private Button[] windowModeButtons; // 0: 창모드, 1: 테두리없는 창모드, 2: 전체화면
        [SerializeField] private Image[] windowModeBgs;
        [SerializeField] private TMP_Text[] windowModeTexts;

        [SerializeField] private Sprite screenActiveSprite;
        [SerializeField] private Sprite screenInactiveSprite;
        [SerializeField] private Sprite languageActiveSprite;
        [SerializeField] private Sprite languageInactiveSprite;

        private const string KEY_ASPECT_RATIO_INDEX = "Mallang_AspectRatio_Idx";
        private const string KEY_WINDOW_MODE_INDEX = "Mallang_WindowMode_Idx";
        private int _currentAspectIdx = 3; // 0: 16:9, 1: 16:10, 2: 4:3, 3: 9:16 (Default: 9:16)
        private int _currentWindowModeIdx = 1; // 0: 창모드, 1: 테두리없는 창모드, 2: 전체화면 (Default: 테두리없는 창모드)

        [Header("Mascot Avatars (0:Pink, 1:Mint, 2:Gold, 3:Purple)")]
        [SerializeField] private Sprite[] mascotAvatars;

        [Header("Shop Action Button Sprites")]
        [SerializeField] private Sprite shopEquipBtnSprite;
        [SerializeField] private Sprite shopEquippedBtnSprite;
        [SerializeField] private Sprite shopCreamBtnSprite;
        [SerializeField] private Sprite shopGoldBtnSprite;
        [SerializeField] private Sprite shopPurpleBtnSprite;

        private int _currentAvatarIdx = 0;
        private string _currentNickname = "";
        private string _nicknameOnly = "말랑이";
        private string _tagOnly = "8276";
        private string _currentBio = "";
        private int _currentCoins = 100; // Mobile default: 100 Gold
        private int _currentDiamonds = 10; // Mobile default: 10 Diamonds
        private bool _isLoggedIn = true;

        private bool _isEditingNick = false;
        private bool _isNickVerified = false;
        private string _verifiedNick = "";
        private string _verifiedTag = "";

        private const string KEY_NICKNAME_ONLY = "Lobby_NicknameOnly";
        private const string KEY_TAG_ONLY = "Lobby_TagOnly";

        // Simulated central database of existing players for duplicate checking
        // In production with your web server (Node.js/Express, Python, etc.), this is queried via UnityWebRequest!
        private static readonly HashSet<string> ReservedUserTagDatabase = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
        {
            "말랑이#0001", "말랑이#1234", "말랑이#8276", "젤리왕#7777", "핑크퐁#1004", "민트초코#9999", "골드킹#0007", "퍼플베리#3333"
        };

        private int _selectedMenuIdx = 0;
        private Coroutine _mascotBounceCoroutine;
        private float[] _mascotPunchScale = new float[4] { 1f, 1f, 1f, 1f };
        private float[] _mascotPunchY = new float[4] { 0f, 0f, 0f, 0f };
        private Coroutine[] _mascotPunchCoroutines = new Coroutine[4];
        private static readonly Vector2[] DefaultMascotPositions = new Vector2[]
        {
            new Vector2(-315f, -60f),
            new Vector2(-105f, 10f),
            new Vector2(105f, 10f),
            new Vector2(315f, -60f)
        };
        private float _ignoreEscUntil = 0f;

        // Button Colors & Texts matching mascots (Adorable Pastel Palette)
        private static readonly Color ColorPink = new Color(1f, 0.52f, 0.68f, 1f);     // Soft Strawberry Milk (#FFA6C4)
        private static readonly Color ColorMint = new Color(0.42f, 0.88f, 0.78f, 1f);   // Soft Pastel Mint (#85E8D1)
        private static readonly Color ColorGold = new Color(1f, 0.82f, 0.42f, 1f);       // Soft Honey Butter (#FFDC85)
        private static readonly Color ColorPurple = new Color(0.78f, 0.60f, 0.98f, 1f);  // Soft Sweet Lavender (#C9ADFA)

        private static readonly Color[] MenuColors = new Color[]
        {
            ColorPink,
            ColorMint,
            ColorGold,
            ColorPurple
        };

        private void Awake()
        {
            if (_instance == null) _instance = this;
            else if (_instance != this)
            {
                Destroy(this);
                return;
            }

            // Migrate V1 (5 mascots, special=4) to V2 (9 mascots, special=8)
            if (PlayerPrefs.GetInt("Mallang_Migrated_V2", 0) == 0)
            {
                if (PlayerPrefs.GetInt(KEY_MASCOT_OWNED_PREFIX + 4, 0) == 1)
                {
                    PlayerPrefs.SetInt(KEY_MASCOT_OWNED_PREFIX + 8, 1);
                    PlayerPrefs.SetInt(KEY_MASCOT_OWNED_PREFIX + 4, 0); // 4 is now Blue Mascot
                }
                if (PlayerPrefs.GetInt(KEY_SELECTED_MASCOT, 0) == 4)
                {
                    PlayerPrefs.SetInt(KEY_SELECTED_MASCOT, 8);
                }
                int oldSpecialShards = PlayerPrefs.GetInt(KEY_MASCOT_SHARDS_PREFIX + 4, 0);
                if (oldSpecialShards > 0)
                {
                    PlayerPrefs.SetInt(KEY_MASCOT_SHARDS_PREFIX + 8, oldSpecialShards);
                    PlayerPrefs.SetInt(KEY_MASCOT_SHARDS_PREFIX + 4, 0);
                }
                PlayerPrefs.SetInt("Mallang_Migrated_V2", 1);
                PlayerPrefs.Save();
            }

            PerformAutoLogin();
            EnsurePartyTipReference();
        }

        private void EnsurePartyTipReference()
        {
            if (partyTipText == null)
            {
                var tipObj = GameObject.Find("PartyTip");
                if (tipObj != null)
                {
                    partyTipText = tipObj.GetComponent<TMP_Text>();
                }
            }
        }

        private void Start()
        {
            LocalizationManager.Init();
            LocalizationManager.OnLanguageChanged += UpdateLanguageUI;

            SetupEventListeners();
            UpdateLanguageUI(LocalizationManager.CurrentLanguage);
            RefreshProfileUI();
            SelectMenu(0, false); // Default: Game Start (Pink Mascot)

            int equippedTheme = PlayerPrefs.GetInt(KEY_EQUIPPED_THEME, 0);
            ApplyTheme(equippedTheme);

            int equippedLobby = PlayerPrefs.GetInt(KEY_EQUIPPED_LOBBY_THEME, 0);
            ApplyLobbyTheme(equippedLobby, playMusic: false);

            if (profileModal != null) profileModal.SetActive(false);
            if (shopModal != null) shopModal.SetActive(false);
            if (settingsModal != null) settingsModal.SetActive(false);
            if (helpModal != null) helpModal.SetActive(false);
            if (quitModal != null) quitModal.SetActive(false);
            if (mascotModal != null) mascotModal.SetActive(false);
            if (summonResultModal != null) summonResultModal.SetActive(false);
            if (mascotCodexModal != null) mascotCodexModal.SetActive(false);
            if (mascotDetailModal != null) mascotDetailModal.SetActive(false);
            if (probabilityModal != null) probabilityModal.SetActive(false);
            if (pickupSkillDetailModal != null) pickupSkillDetailModal.SetActive(false);

            _currentAspectIdx = PlayerPrefs.GetInt(KEY_ASPECT_RATIO_INDEX, 3);
            if (_currentAspectIdx < 0 || _currentAspectIdx > 3) _currentAspectIdx = 3;
            _currentWindowModeIdx = PlayerPrefs.GetInt(KEY_WINDOW_MODE_INDEX, 1);
            if (_currentWindowModeIdx < 0 || _currentWindowModeIdx > 2) _currentWindowModeIdx = 1;
            ApplyScreenSettings();
            UpdateScreenSettingsUI();

            EnsureCodexCardButtonsBound();
            RefreshCurrenciesUI();

            UpdateLanguageUI(LocalizationManager.CurrentLanguage);
            StartMascotIdleBounce();
        }

        private void OnEnable()
        {
            StartMascotIdleBounce();
        }

        private void OnDisable()
        {
            if (_mascotBounceCoroutine != null)
            {
                StopCoroutine(_mascotBounceCoroutine);
                _mascotBounceCoroutine = null;
            }
        }

        private void OnDestroy()
        {
            LocalizationManager.OnLanguageChanged -= UpdateLanguageUI;
            if (_mascotBounceCoroutine != null)
            {
                StopCoroutine(_mascotBounceCoroutine);
                _mascotBounceCoroutine = null;
            }
        }

        // ==========================================
        // AUTO-LOGIN & ACCOUNT DATA
        // ==========================================

        private void PerformAutoLogin()
        {
            if (!PlayerPrefs.HasKey(KEY_LOGGED_IN) || PlayerPrefs.GetInt(KEY_LOGGED_IN, 0) == 0)
            {
                _nicknameOnly = "말랑이";
                _tagOnly = Random.Range(1000, 9999).ToString();
                _currentNickname = $"{_nicknameOnly}#{_tagOnly}";
                _currentBio = "말랑블라스트에 오신 걸 환영해요!";
                _currentAvatarIdx = 0; // Pink Mascot default
                _currentCoins = 0;     // Steam default: 0 Gold
                _isLoggedIn = true;

                PlayerPrefs.SetString(KEY_NICKNAME_ONLY, _nicknameOnly);
                PlayerPrefs.SetString(KEY_TAG_ONLY, _tagOnly);
                PlayerPrefs.SetString(KEY_NICKNAME, _currentNickname);
                PlayerPrefs.SetString(KEY_BIO, _currentBio);
                PlayerPrefs.SetInt(KEY_AVATAR, _currentAvatarIdx);
                PlayerPrefs.SetInt(KEY_COINS, _currentCoins);
                PlayerPrefs.SetInt(KEY_LOGGED_IN, 1);
            }
            else
            {
                if (PlayerPrefs.HasKey(KEY_NICKNAME_ONLY) && PlayerPrefs.HasKey(KEY_TAG_ONLY))
                {
                    _nicknameOnly = PlayerPrefs.GetString(KEY_NICKNAME_ONLY, "말랑이");
                    _tagOnly = PlayerPrefs.GetString(KEY_TAG_ONLY, "8276");
                    _currentNickname = $"{_nicknameOnly}#{_tagOnly}";
                }
                else
                {
                    string raw = PlayerPrefs.GetString(KEY_NICKNAME, "말랑이#8276");
                    if (raw.Contains("#"))
                    {
                        var parts = raw.Split('#');
                        _nicknameOnly = parts[0].Trim();
                        _tagOnly = parts.Length > 1 ? parts[1].Trim() : Random.Range(1000, 9999).ToString();
                    }
                    else
                    {
                        _nicknameOnly = raw.Trim();
                        _tagOnly = Random.Range(1000, 9999).ToString();
                    }
                    PlayerPrefs.SetString(KEY_NICKNAME_ONLY, _nicknameOnly);
                    PlayerPrefs.SetString(KEY_TAG_ONLY, _tagOnly);
                    _currentNickname = $"{_nicknameOnly}#{_tagOnly}";
                }

                _currentBio = PlayerPrefs.GetString(KEY_BIO, "말랑블라스트에 오신 걸 환영해요!");
                _currentNickname = System.Text.RegularExpressions.Regex.Replace(_currentNickname, @"[^\u0000-\u007F\uAC00-\uD7AF\u1100-\u11FF\u3130-\u318F\s.,!?:;~()#+-]", "").Trim();
                _currentBio = System.Text.RegularExpressions.Regex.Replace(_currentBio, @"[^\u0000-\u007F\uAC00-\uD7AF\u1100-\u11FF\u3130-\u318F\s.,!?:;~()#+-]", "").Trim();
                _currentAvatarIdx = PlayerPrefs.GetInt(KEY_AVATAR, 0);
                _currentCoins = PlayerPrefs.GetInt(KEY_COINS, 100);
                _currentDiamonds = PlayerPrefs.GetInt(KEY_DIAMONDS, 10);
                _isLoggedIn = PlayerPrefs.GetInt(KEY_LOGGED_IN, 1) == 1;
            }

            // Mobile Launch Economy Configuration: 100 Gold, 10 Diamonds, Theme 0 & Lobby 0 & Mascot 0 owned!
            if (PlayerPrefs.GetInt(KEY_MOBILE_INIT_ECONOMY, 0) == 0)
            {
                PlayerPrefs.SetInt(KEY_MOBILE_INIT_ECONOMY, 1);
                _currentCoins = 100;
                _currentDiamonds = 10;
                PlayerPrefs.SetInt(KEY_COINS, 100);
                PlayerPrefs.SetInt(KEY_DIAMONDS, 10);

                // Game Themes: Theme 0 owned (default free), 1, 2, 3 unowned (1k, 2k, 3k)
                PlayerPrefs.SetInt(KEY_THEME_OWNED_PREFIX + 0, 1);
                for (int i = 1; i < 10; i++)
                {
                    PlayerPrefs.SetInt(KEY_THEME_OWNED_PREFIX + i, 0);
                }
                PlayerPrefs.SetInt(KEY_EQUIPPED_THEME, 0);

                // Lobby Themes: Theme 0 owned (default free), 1, 2 unowned (1k, 2k)
                PlayerPrefs.SetInt(KEY_LOBBY_THEME_OWNED_PREFIX + 0, 1);
                for (int i = 1; i < 10; i++)
                {
                    PlayerPrefs.SetInt(KEY_LOBBY_THEME_OWNED_PREFIX + i, 0);
                }
                PlayerPrefs.SetInt(KEY_EQUIPPED_LOBBY_THEME, 0);

                // Mascots: Mascot 0 (Pink) owned (default free), 1, 2, 3, 4 locked
                PlayerPrefs.SetInt(KEY_MASCOT_OWNED_PREFIX + 0, 1);
                PlayerPrefs.SetInt(KEY_MASCOT_OWNED_PREFIX + 1, 0);
                PlayerPrefs.SetInt(KEY_MASCOT_OWNED_PREFIX + 2, 0);
                PlayerPrefs.SetInt(KEY_MASCOT_OWNED_PREFIX + 3, 0);
                PlayerPrefs.SetInt(KEY_MASCOT_OWNED_PREFIX + 4, 0);
                PlayerPrefs.SetInt(KEY_SELECTED_MASCOT, 0);

                PlayerPrefs.Save();
            }
            else
            {
                _currentCoins = PlayerPrefs.GetInt(KEY_COINS, 100);
                _currentDiamonds = PlayerPrefs.GetInt(KEY_DIAMONDS, 10);
            }

            // Ensure player has diamonds to test new gacha features
            if (_currentDiamonds < 2000)
            {
                _currentDiamonds = 2000;
                PlayerPrefs.SetInt(KEY_DIAMONDS, _currentDiamonds);
                PlayerPrefs.Save();
            }
        }

        private void SetupEventListeners()
        {
            // Profile Button
            if (profileBtn != null)
            {
                profileBtn.onClick.RemoveAllListeners();
                profileBtn.onClick.AddListener(() => { PlayClickSound(); OpenProfileModal(); });
            }

            // Bottom Action Button
            if (btnPlayGame != null)
            {
                btnPlayGame.onClick.RemoveAllListeners();
                btnPlayGame.onClick.AddListener(() => { PlayClickSound(); ExecuteSelectedMenuAction(); });
            }

            // Profile Modal
            if (btnCloseProfile != null)
            {
                btnCloseProfile.onClick.RemoveAllListeners();
                btnCloseProfile.onClick.AddListener(() => { PlayClickSound(); CloseProfileModal(); });
            }
            if (btnStartEditNick != null)
            {
                btnStartEditNick.onClick.RemoveAllListeners();
                btnStartEditNick.onClick.AddListener(() => { PlayClickSound(); SetNickEditMode(true); });
            }
            if (btnCheckTag != null)
            {
                btnCheckTag.onClick.RemoveAllListeners();
                btnCheckTag.onClick.AddListener(() => { PlayClickSound(); CheckAndVerifyNicknameAndTag(); });
            }
            if (btnConfirmNick != null)
            {
                btnConfirmNick.onClick.RemoveAllListeners();
                btnConfirmNick.onClick.AddListener(() => { PlayClickSound(); OnConfirmNicknameChange(); });
            }
            if (profileModalNicknameInput != null)
            {
                profileModalNicknameInput.onValueChanged.RemoveAllListeners();
                profileModalNicknameInput.onValueChanged.AddListener(OnNickOrTagValueChanged);
            }
            if (profileModalTagInput != null)
            {
                profileModalTagInput.onValueChanged.RemoveAllListeners();
                profileModalTagInput.onValueChanged.AddListener(OnNickOrTagValueChanged);
            }
            if (profileModalBioInput != null)
            {
                profileModalBioInput.onEndEdit.RemoveAllListeners();
                profileModalBioInput.onEndEdit.AddListener(OnBioEndEdit);
            }
            if (btnLogout != null)
            {
                btnLogout.onClick.RemoveAllListeners();
                btnLogout.onClick.AddListener(OnLogoutClicked);
            }

            // Login Modal
            if (btnCloseLogin != null)
            {
                btnCloseLogin.onClick.RemoveAllListeners();
                btnCloseLogin.onClick.AddListener(CloseLoginModal);
            }
            if (btnSubmitLogin != null)
            {
                btnSubmitLogin.onClick.RemoveAllListeners();
                btnSubmitLogin.onClick.AddListener(OnSubmitLogin);
            }
            if (btnGoogleLogin != null)
            {
                btnGoogleLogin.onClick.RemoveAllListeners();
                btnGoogleLogin.onClick.AddListener(OnGoogleLoginClicked);
            }

            // Avatar Select Buttons
            if (avatarSelectButtons != null)
            {
                for (int i = 0; i < avatarSelectButtons.Length; i++)
                {
                    int idx = i;
                    if (avatarSelectButtons[i] != null)
                    {
                        avatarSelectButtons[i].onClick.RemoveAllListeners();
                        avatarSelectButtons[i].onClick.AddListener(() => { PlayClickSound(); SelectAvatar(idx); });
                    }
                }
            }

            // Shop Modal
            if (btnCloseShop != null)
            {
                btnCloseShop.onClick.RemoveAllListeners();
                btnCloseShop.onClick.AddListener(() => { PlayClickSound(); CloseShopModal(); });
            }

            // Mascot Modal
            if (btnCloseMascotModal != null)
            {
                btnCloseMascotModal.onClick.RemoveAllListeners();
                btnCloseMascotModal.onClick.AddListener(() => { PlayClickSound(); CloseMascotModal(); });
            }

            // 5 Vertical Shop Tabs
            if (shopTabButtons != null)
            {
                for (int i = 0; i < shopTabButtons.Length; i++)
                {
                    int tabIdx = i;
                    if (shopTabButtons[i] != null)
                    {
                        shopTabButtons[i].onClick.RemoveAllListeners();
                        shopTabButtons[i].onClick.AddListener(() => { PlayClickSound(); SelectShopTab(tabIdx); });
                    }
                }
            }

            // Shop Mascot Action Buttons
            if (mascotShopActionButtons != null)
            {
                for (int i = 0; i < mascotShopActionButtons.Length; i++)
                {
                    int idx = i;
                    if (mascotShopActionButtons[i] != null)
                    {
                        mascotShopActionButtons[i].onClick.RemoveAllListeners();
                        mascotShopActionButtons[i].onClick.AddListener(() =>
                        {
                            BuyOrEquipMascot(idx);
                        });
                    }
                }
            }

            // Mascot Modal Equip Buttons
            if (mascotActionButtons != null)
            {
                for (int i = 0; i < mascotActionButtons.Length; i++)
                {
                    int idx = i;
                    if (mascotActionButtons[i] != null)
                    {
                        mascotActionButtons[i].onClick.RemoveAllListeners();
                        mascotActionButtons[i].onClick.AddListener(() =>
                        {
                            EquipMascot(idx);
                        });
                    }
                }
            }

            // Summon Buttons
            if (btnSummon1 != null)
            {
                btnSummon1.onClick.RemoveAllListeners();
                btnSummon1.onClick.AddListener(() => SummonPickup(1));
            }
            if (btnSummon10 != null)
            {
                btnSummon10.onClick.RemoveAllListeners();
                btnSummon10.onClick.AddListener(() => SummonPickup(10));
            }

            // Package Buttons
            if (shopPackageButtons != null)
            {
                for (int i = 0; i < shopPackageButtons.Length; i++)
                {
                    int pIdx = i;
                    if (shopPackageButtons[i] != null)
                    {
                        shopPackageButtons[i].onClick.RemoveAllListeners();
                        shopPackageButtons[i].onClick.AddListener(() => BuyPackage(pIdx));
                    }
                }
            }

            if (themeActionButtons != null)
            {
                for (int i = 0; i < themeActionButtons.Length; i++)
                {
                    int idx = i;
                    if (themeActionButtons[i] != null)
                    {
                        themeActionButtons[i].onClick.RemoveAllListeners();
                        themeActionButtons[i].onClick.AddListener(() =>
                        {
                            BuyOrEquipTheme(idx);
                        });
                    }
                }
            }

            if (lobbyThemeActionButtons != null)
            {
                for (int i = 0; i < lobbyThemeActionButtons.Length; i++)
                {
                    int idx = i;
                    if (lobbyThemeActionButtons[i] != null)
                    {
                        lobbyThemeActionButtons[i].onClick.RemoveAllListeners();
                        lobbyThemeActionButtons[i].onClick.AddListener(() =>
                        {
                            BuyOrEquipLobbyTheme(idx);
                        });
                    }
                }
            }

            // Settings Modal
            if (btnCloseSettings != null) btnCloseSettings.onClick.AddListener(() => { PlayClickSound(); CloseSettingsModal(); });
            if (bgmSlider != null)
            {
                float defBgm = BlockAudioManager.Instance != null ? BlockAudioManager.Instance.bgmSliderLevel : BlockAudioManager.DEFAULT_SLIDER_PERCENT;
                bgmSlider.value = PlayerPrefs.GetFloat("BGM_Slider_Level", defBgm);
                bgmSlider.onValueChanged.RemoveAllListeners();
                bgmSlider.onValueChanged.AddListener((v) =>
                {
                    if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.SetBGMVolume(v);
                });
            }
            if (sfxSlider != null)
            {
                float defSfx = BlockAudioManager.Instance != null ? BlockAudioManager.Instance.sfxSliderLevel : BlockAudioManager.DEFAULT_SLIDER_PERCENT;
                sfxSlider.value = PlayerPrefs.GetFloat("SFX_Slider_Level", defSfx);
                sfxSlider.onValueChanged.RemoveAllListeners();
                sfxSlider.onValueChanged.AddListener((v) =>
                {
                    if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.SetSFXVolume(v);
                });
            }

            // Language Selection Buttons (Runtime binding)
            if (languageButtons != null)
            {
                for (int i = 0; i < languageButtons.Length; i++)
                {
                    int langIdx = i;
                    if (languageButtons[i] != null)
                    {
                        languageButtons[i].onClick.RemoveAllListeners();
                        languageButtons[i].onClick.AddListener(() =>
                        {
                            PlayClickSound();
                            SelectLanguage((GameLanguage)langIdx);
                        });
                    }
                }
            }

            // Resolution & Screen Mode Events (Runtime binding)
            if (aspectButtons != null)
            {
                for (int i = 0; i < aspectButtons.Length; i++)
                {
                    int idx = i;
                    if (aspectButtons[i] != null)
                    {
                        aspectButtons[i].onClick.RemoveAllListeners();
                        aspectButtons[i].onClick.AddListener(() => SetAspectRatio(idx));
                    }
                }
            }
            if (windowModeButtons != null)
            {
                for (int j = 0; j < windowModeButtons.Length; j++)
                {
                    int idx = j;
                    if (windowModeButtons[j] != null)
                    {
                        windowModeButtons[j].onClick.RemoveAllListeners();
                        windowModeButtons[j].onClick.AddListener(() => SetWindowMode(idx));
                    }
                }
            }

            // Quit Confirm Modal Events (Runtime binding)
            if (btnQuitConfirmYes != null)
            {
                btnQuitConfirmYes.onClick.RemoveAllListeners();
                btnQuitConfirmYes.onClick.AddListener(QuitGame);
            }
            if (btnQuitConfirmNo != null)
            {
                btnQuitConfirmNo.onClick.RemoveAllListeners();
                btnQuitConfirmNo.onClick.AddListener(CloseQuitModal);
            }
            if (btnQuitDarkBg != null)
            {
                btnQuitDarkBg.onClick.RemoveAllListeners();
                btnQuitDarkBg.onClick.AddListener(CloseQuitModal);
            }
            if (btnOpenQuitModal != null)
            {
                btnOpenQuitModal.onClick.RemoveAllListeners();
                btnOpenQuitModal.onClick.AddListener(OpenQuitModal);
            }

            // Help Modal
            if (btnCloseHelp != null) btnCloseHelp.onClick.AddListener(() => { PlayClickSound(); CloseHelpModal(); });
            if (btnConfirmHelp != null) btnConfirmHelp.onClick.AddListener(() => { PlayClickSound(); CloseHelpModal(); });

            // Mascot Codex Modal
            if (btnCloseMascotCodex != null)
            {
                btnCloseMascotCodex.onClick.RemoveAllListeners();
                btnCloseMascotCodex.onClick.AddListener(() => { PlayClickSound(); CloseMascotModal(); });
            }

            // Mascot Detail Modal
            if (btnCloseMascotDetail != null)
            {
                btnCloseMascotDetail.onClick.RemoveAllListeners();
                btnCloseMascotDetail.onClick.AddListener(() => { PlayClickSound(); CloseMascotDetail(); });
            }

            // Pickup Skill Detail Modal
            if (btnClosePickupSkillDetail != null)
            {
                btnClosePickupSkillDetail.onClick.RemoveAllListeners();
                btnClosePickupSkillDetail.onClick.AddListener(() => { PlayClickSound(); ClosePickupSkillDetail(); });
            }

            // Probability Modal
            if (btnCloseProbability != null)
            {
                btnCloseProbability.onClick.RemoveAllListeners();
                btnCloseProbability.onClick.AddListener(() => { PlayClickSound(); CloseProbabilityModal(); });
            }
            if (btnConfirmProbability != null)
            {
                btnConfirmProbability.onClick.RemoveAllListeners();
                btnConfirmProbability.onClick.AddListener(() => { PlayClickSound(); CloseProbabilityModal(); });
            }
            if (btnProbabilityDarkBg != null)
            {
                btnProbabilityDarkBg.onClick.RemoveAllListeners();
                btnProbabilityDarkBg.onClick.AddListener(() => { PlayClickSound(); CloseProbabilityModal(); });
            }

            // Summon Result Modal
            if (btnCloseSummonResult != null)
            {
                btnCloseSummonResult.onClick.RemoveAllListeners();
                btnCloseSummonResult.onClick.AddListener(() => { PlayClickSound(); CloseSummonResultModal(); });
            }

            // Quit Modal
            if (btnQuitConfirmNo != null)
            {
                btnQuitConfirmNo.onClick.RemoveAllListeners();
                btnQuitConfirmNo.onClick.AddListener(() => { PlayClickSound(); CloseQuitModal(); });
            }
            if (btnQuitDarkBg != null)
            {
                btnQuitDarkBg.onClick.RemoveAllListeners();
                btnQuitDarkBg.onClick.AddListener(() => { PlayClickSound(); CloseQuitModal(); });
            }
            if (btnQuitConfirmYes != null)
            {
                btnQuitConfirmYes.onClick.RemoveAllListeners();
                btnQuitConfirmYes.onClick.AddListener(QuitGame);
            }

            // Universal modal close wire-up (guarantees (X) buttons, Confirm buttons, DarkBg click-outside, and z-order)
            WireModalAutoClose(shopModal, CloseShopModal);
            WireModalAutoClose(mascotCodexModal, CloseMascotModal);
            WireModalAutoClose(mascotDetailModal, CloseMascotDetail);
            WireModalAutoClose(mascotModal, CloseMascotModal);
            WireModalAutoClose(settingsModal, CloseSettingsModal);
            WireModalAutoClose(profileModal, CloseProfileModal);
            WireModalAutoClose(loginModal, CloseLoginModal);
            WireModalAutoClose(pickupSkillDetailModal, ClosePickupSkillDetail);
            WireModalAutoClose(probabilityModal, CloseProbabilityModal);
            WireModalAutoClose(summonResultModal, CloseSummonResultModal);
            WireModalAutoClose(helpModal, CloseHelpModal);
            WireModalAutoClose(quitModal, CloseQuitModal);

            // Party Mascot & Floating Label Clicks
            if (partyMascots != null)
            {
                for (int i = 0; i < partyMascots.Length; i++)
                {
                    int idx = i;
                    Button btn = partyMascots[i].GetComponent<Button>();
                    if (btn == null) btn = partyMascots[i].gameObject.AddComponent<Button>();
                    btn.transition = Selectable.Transition.None;
                    btn.onClick.AddListener(() =>
                    {
                        PlayClickSound();
                        SelectMenu(idx, true);
                    });

                    // Also wire up any button on children (like mascot image)
                    Button[] childBtns = partyMascots[i].GetComponentsInChildren<Button>(true);
                    foreach (var cBtn in childBtns)
                    {
                        if (cBtn != btn)
                        {
                            cBtn.transition = Selectable.Transition.None;
                            cBtn.onClick.AddListener(() =>
                            {
                                PlayClickSound();
                                SelectMenu(idx, true);
                            });
                        }
                    }
                }
            }

            if (partyLabels != null)
            {
                for (int i = 0; i < partyLabels.Length; i++)
                {
                    int idx = i;
                    if (partyLabels[i] != null)
                    {
                        Button lBtn = partyLabels[i].GetComponent<Button>();
                        if (lBtn == null) lBtn = partyLabels[i].AddComponent<Button>();
                        lBtn.transition = Selectable.Transition.None;
                        lBtn.onClick.AddListener(() =>
                        {
                            PlayClickSound();
                            SelectMenu(idx, false);
                            ExecuteSelectedMenuAction();
                        });
                    }
                }
            }
        }

        // ==========================================
        // MENU SELECTION & BOTTOM BUTTON SYNC
        // ==========================================

        public string GetMenuActionText(int index)
        {
            switch (index)
            {
                case 0: return LocalizationManager.Get("lobby_start") + "!";
                case 1: return LocalizationManager.Get("lobby_shop") + "!";
                case 2: return LocalizationManager.Get("lobby_mascot") + "!";
                case 3: return LocalizationManager.Get("lobby_settings") + "!";
                default: return "";
            }
        }

        public void SelectMenu(int index, bool triggerActionIfAlreadySelected = false)
        {
            if (index < 0 || index >= MenuColors.Length) return;

            // If already selected and user clicked again, trigger action immediately!
            if (_selectedMenuIdx == index && triggerActionIfAlreadySelected)
            {
                ExecuteSelectedMenuAction();
                return;
            }

            _selectedMenuIdx = index;

            // Update Bottom Button Text & Color
            if (btnPlayGameText != null)
            {
                btnPlayGameText.text = GetMenuActionText(index);
            }

            if (btnPlayGameBg != null)
            {
                btnPlayGameBg.color = MenuColors[index];
            }
            else if (btnPlayGame != null)
            {
                var img = btnPlayGame.GetComponent<Image>();
                if (img != null) img.color = MenuColors[index];
            }

            if (btnPlayGameGlow != null)
            {
                btnPlayGameGlow.color = new Color(MenuColors[index].r, MenuColors[index].g, MenuColors[index].b, 0.50f);
            }

            if (triggerActionIfAlreadySelected && btnPlayGame != null)
            {
                StartCoroutine(PunchMascot(btnPlayGame.GetComponent<RectTransform>()));
            }

            // Update Glow Auras behind characters (light radiates from behind selected mascot!)
            if (partyGlowAuras != null)
            {
                for (int i = 0; i < partyGlowAuras.Length; i++)
                {
                    if (partyGlowAuras[i] != null)
                    {
                        partyGlowAuras[i].SetActive(i == index);
                    }
                }
            }

            // Update Floating Labels Highlight
            if (partyLabels != null)
            {
                for (int i = 0; i < partyLabels.Length; i++)
                {
                    bool isSel = (i == index);
                    if (partyLabelBgs != null && i < partyLabelBgs.Length && partyLabelBgs[i] != null)
                    {
                        partyLabelBgs[i].color = isSel ? Color.white : new Color(1f, 1f, 1f, 0.7f);
                    }
                    if (partyLabelTexts != null && i < partyLabelTexts.Length && partyLabelTexts[i] != null)
                    {
                        partyLabelTexts[i].color = Color.white;
                        partyLabelTexts[i].fontStyle = FontStyles.Bold;
                    }
                }
            }

            // Punch animation on chosen mascot
            if (partyMascots != null && index < partyMascots.Length && partyMascots[index] != null)
            {
                TriggerPunchMascot(index);
            }
        }

        private void ExecuteSelectedMenuAction()
        {
            switch (_selectedMenuIdx)
            {
                case 0:
                    StartGameFromLobby();
                    break;
                case 1:
                    OpenShopModal();
                    break;
                case 2:
                    OpenMascotModal();
                    break;
                case 3:
                    OpenSettingsModal();
                    break;
            }
        }

        // ==========================================
        // PROFILE MODAL
        // ==========================================

        public void OpenProfileModal()
        {
            if (profileModal != null)
            {
                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
                if (btnPlayGame != null) btnPlayGame.gameObject.SetActive(false);
                if (FairyScreenTransition.Instance != null) FairyScreenTransition.Instance.EmitCornerSparkles();
                SetNickEditMode(false);
                SetNickStatus("", Color.white);
                RefreshProfileUI();
                WireModalAutoClose(profileModal, CloseProfileModal);
                profileModal.transform.SetAsLastSibling();
                profileModal.SetActive(true);
            }
        }

        public void CloseProfileModal()
        {
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
            SetNickEditMode(false);
            if (profileModal != null) profileModal.SetActive(false);
            if (btnPlayGame != null) btnPlayGame.gameObject.SetActive(true);
        }

        private void SelectAvatar(int idx)
        {
            if (mascotAvatars != null && idx >= 0 && idx < mascotAvatars.Length)
            {
                _currentAvatarIdx = idx;
                PlayerPrefs.SetInt(KEY_AVATAR, _currentAvatarIdx);
                PlayerPrefs.Save();
                RefreshProfileUI();
            }
        }

        private void OnNicknameEndEdit(string newNick)
        {
            newNick = System.Text.RegularExpressions.Regex.Replace(newNick ?? "", @"[^\u0000-\u007F\uAC00-\uD7AF\u1100-\u11FF\u3130-\u318F\s.,!?:;~()#+-]", "").Trim();
            if (!string.IsNullOrWhiteSpace(newNick))
            {
                _currentNickname = newNick;
                PlayerPrefs.SetString(KEY_NICKNAME, _currentNickname);
                PlayerPrefs.Save();
                RefreshProfileUI();
            }
        }

        private void OnBioEndEdit(string newBio)
        {
            _currentBio = System.Text.RegularExpressions.Regex.Replace(newBio ?? "", @"[^\u0000-\u007F\uAC00-\uD7AF\u1100-\u11FF\u3130-\u318F\s.,!?:;~()#+-]", "").Trim();
            PlayerPrefs.SetString(KEY_BIO, _currentBio);
            PlayerPrefs.Save();
            RefreshProfileUI();
        }

        public static string FormatCoins(int amount)
        {
            if (amount < 0) amount = 0;
            if (amount < 1000)
            {
                return amount.ToString();
            }
            if (amount < 10000)
            {
                return amount.ToString("N0"); // e.g. 1,234
            }
            if (amount < 1000000)
            {
                double val = amount / 1000.0;
                return val >= 100 ? $"{val:0}K" : $"{val:0.#}K"; // e.g. 10K, 12.5K
            }
            if (amount < 1000000000)
            {
                double val = amount / 1000000.0;
                return val >= 100 ? $"{val:0}M" : $"{val:0.##}M"; // e.g. 1M, 1.25M
            }
            return $"{amount / 1000000000.0:0.##}B";
        }

        public int Coins => _currentCoins;
        public int Diamonds => _currentDiamonds;

        public void AddCoins(int amount)
        {
            _currentCoins += amount;
            if (_currentCoins < 0) _currentCoins = 0;
            PlayerPrefs.SetInt(KEY_COINS, _currentCoins);
            PlayerPrefs.Save();
            RefreshCurrenciesUI();
        }

        public void AddDiamonds(int amount)
        {
            _currentDiamonds += amount;
            if (_currentDiamonds < 0) _currentDiamonds = 0;
            PlayerPrefs.SetInt(KEY_DIAMONDS, _currentDiamonds);
            PlayerPrefs.Save();
            RefreshCurrenciesUI();
        }

        public void RefreshCurrenciesUI()
        {
            string coinStr = $"{FormatCoins(_currentCoins)} G";
            string diaStr = $"{FormatCoins(_currentDiamonds)}";

            if (lobbyCoinsText != null) lobbyCoinsText.text = coinStr;
            if (lobbyDiamondsText != null) lobbyDiamondsText.text = diaStr;
            if (shopCoinsText != null) shopCoinsText.text = coinStr;
            if (shopDiamondsText != null) shopDiamondsText.text = diaStr;
            if (playerLevelBadgeText != null) playerLevelBadgeText.text = $"{PlayerPrefs.GetInt("Mallang_Player_Level", 1)}";
        }

        public void SetNickEditMode(bool isEditing)
        {
            _isEditingNick = isEditing;
            if (!_isEditingNick)
            {
                _isNickVerified = false;
                if (profileModalNicknameInput != null)
                {
                    profileModalNicknameInput.text = _nicknameOnly;
                    profileModalNicknameInput.interactable = false;
                }
                if (profileModalTagInput != null)
                {
                    profileModalTagInput.text = _tagOnly;
                    profileModalTagInput.interactable = false;
                }
                if (btnStartEditNick != null) btnStartEditNick.gameObject.SetActive(true);
                if (btnCheckTag != null) btnCheckTag.gameObject.SetActive(false);
                if (btnConfirmNick != null) btnConfirmNick.gameObject.SetActive(false);
            }
            else
            {
                _isNickVerified = false;
                if (profileModalNicknameInput != null)
                {
                    profileModalNicknameInput.interactable = true;
                    profileModalNicknameInput.ActivateInputField();
                }
                if (profileModalTagInput != null)
                {
                    profileModalTagInput.interactable = true;
                }
                if (btnStartEditNick != null) btnStartEditNick.gameObject.SetActive(false);
                if (btnCheckTag != null)
                {
                    btnCheckTag.gameObject.SetActive(true);
                    btnCheckTag.interactable = true;
                }
                if (btnConfirmNick != null)
                {
                    btnConfirmNick.gameObject.SetActive(true);
                    btnConfirmNick.interactable = false; // 중복 확인 완료 전에는 비활성화!
                }
                SetNickStatus("닉네임/태그 입력 후 [중복 확인]을 눌러주세요.", new Color(0.55f, 0.45f, 0.70f));
            }
        }

        private void OnNickOrTagValueChanged(string _)
        {
            if (_isEditingNick)
            {
                _isNickVerified = false;
                if (btnConfirmNick != null) btnConfirmNick.interactable = false;
                SetNickStatus("[중복 확인]을 먼저 진행해주세요.", new Color(0.85f, 0.55f, 0.20f));
            }
        }

        public void CheckAndVerifyNicknameAndTag()
        {
            string newNick = System.Text.RegularExpressions.Regex.Replace(profileModalNicknameInput?.text ?? "", @"[^\u0000-\u007F\uAC00-\uD7AF\u1100-\u11FF\u3130-\u318F\s.,!?:;~()+-]", "").Trim();
            string newTag = System.Text.RegularExpressions.Regex.Replace(profileModalTagInput?.text ?? "", @"[^0-9a-zA-Z]", "").Trim();

            if (string.IsNullOrWhiteSpace(newNick))
            {
                SetNickStatus("닉네임을 입력해주세요.", new Color(1f, 0.35f, 0.45f));
                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWarning();
                _isNickVerified = false;
                if (btnConfirmNick != null) btnConfirmNick.interactable = false;
                return;
            }

            if (string.IsNullOrWhiteSpace(newTag))
            {
                newTag = Random.Range(1000, 9999).ToString();
                if (profileModalTagInput != null) profileModalTagInput.text = newTag;
            }

            string combined = $"{newNick}#{newTag}";
            string currentSaved = $"{_nicknameOnly}#{_tagOnly}";

            if (combined.Equals(currentSaved, System.StringComparison.OrdinalIgnoreCase))
            {
                SetNickStatus("현재 사용 중인 본인의 닉네임입니다.", new Color(0.18f, 0.82f, 0.45f));
                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayBuy();
                _isNickVerified = true;
                _verifiedNick = newNick;
                _verifiedTag = newTag;
                if (btnConfirmNick != null) btnConfirmNick.interactable = true;
                return;
            }

            // Central server duplicate check simulation
            if (ReservedUserTagDatabase.Contains(combined))
            {
                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWarning();
                SetNickStatus("이미 있는 닉네임입니다.", new Color(1f, 0.28f, 0.38f)); // Coral Red
                _isNickVerified = false;
                if (btnConfirmNick != null) btnConfirmNick.interactable = false;
                return;
            }

            // Available & unique!
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayBuy();
            SetNickStatus("사용 가능한 닉네임입니다! [변경 완료]를 눌러주세요.", new Color(0.18f, 0.82f, 0.45f)); // Vibrant Mint Green
            _isNickVerified = true;
            _verifiedNick = newNick;
            _verifiedTag = newTag;
            if (btnConfirmNick != null) btnConfirmNick.interactable = true;
        }

        public void OnConfirmNicknameChange()
        {
            if (!_isNickVerified)
            {
                SetNickStatus("먼저 [중복 확인]을 진행해주세요.", new Color(1f, 0.35f, 0.45f));
                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWarning();
                return;
            }

            string oldCombined = $"{_nicknameOnly}#{_tagOnly}";
            string newCombined = $"{_verifiedNick}#{_verifiedTag}";

            ReservedUserTagDatabase.Remove(oldCombined);
            ReservedUserTagDatabase.Add(newCombined);

            _nicknameOnly = _verifiedNick;
            _tagOnly = _verifiedTag;
            _currentNickname = newCombined;

            PlayerPrefs.SetString(KEY_NICKNAME_ONLY, _nicknameOnly);
            PlayerPrefs.SetString(KEY_TAG_ONLY, _tagOnly);
            PlayerPrefs.SetString(KEY_NICKNAME, _currentNickname);
            PlayerPrefs.Save();

            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayBuy();

            SetNickEditMode(false);
            SetNickStatus("닉네임이 성공적으로 변경되었습니다!", new Color(0.18f, 0.82f, 0.45f));
            RefreshProfileUI();
        }

        public void CheckAndSaveNicknameAndTag(string newNick, string newTag)
        {
            if (profileModalNicknameInput != null) profileModalNicknameInput.text = newNick;
            if (profileModalTagInput != null) profileModalTagInput.text = newTag;
            CheckAndVerifyNicknameAndTag();
            if (_isNickVerified)
            {
                OnConfirmNicknameChange();
            }
        }

        private void SetNickStatus(string msg, Color col)
        {
            if (profileModalNickStatus != null)
            {
                profileModalNickStatus.text = msg;
                profileModalNickStatus.color = col;
            }
        }

        // ==========================================
        // LOGIN MODAL & GOOGLE OAUTH
        // ==========================================

        public void OpenLoginModal()
        {
            PlayClickSound();
            if (loginModal != null)
            {
                WireModalAutoClose(loginModal, CloseLoginModal);
                loginModal.transform.SetAsLastSibling();
                loginModal.SetActive(true);
                if (loginStatusText != null) loginStatusText.text = "";
                if (loginIdInput != null) loginIdInput.text = "";
                if (loginPwInput != null) loginPwInput.text = "";
            }
        }

        public void CloseLoginModal()
        {
            PlayClickSound();
            if (loginModal != null) loginModal.SetActive(false);
        }

        private void OnSubmitLogin()
        {
            PlayClickSound();
            string id = loginIdInput != null ? loginIdInput.text.Trim() : "";
            string pw = loginPwInput != null ? loginPwInput.text.Trim() : "";

            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(pw))
            {
                if (loginStatusText != null)
                {
                    loginStatusText.text = "아이디와 비밀번호를 모두 입력해주세요.";
                    loginStatusText.color = new Color(1f, 0.35f, 0.45f);
                }
                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWarning();
                return;
            }

            _isLoggedIn = true;
            PlayerPrefs.SetInt(KEY_LOGGED_IN, 1);
            PlayerPrefs.Save();

            if (loginStatusText != null)
            {
                loginStatusText.text = "로그인되었습니다!";
                loginStatusText.color = new Color(0.18f, 0.82f, 0.45f);
            }
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayBuy();

            CloseLoginModal();
            RefreshProfileUI();
        }

        private void OnGoogleLoginClicked()
        {
            PlayClickSound();

            // Google OAuth 2.0 Web Client URL
            // In WebGL/Browser, this opens Google account sign-in in Chrome / external browser!
            string googleAuthUrl = "https://accounts.google.com/o/oauth2/v2/auth" +
                "?client_id=YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com" +
                "&redirect_uri=https://your-game-server.com/auth/callback" +
                "&response_type=code" +
                "&scope=openid%20profile%20email";

            Application.OpenURL(googleAuthUrl);

            // In-game immediate session login feedback
            _isLoggedIn = true;
            PlayerPrefs.SetInt(KEY_LOGGED_IN, 1);
            PlayerPrefs.Save();

            if (loginStatusText != null)
            {
                loginStatusText.text = "구글 계정 연동 창이 열렸습니다!";
                loginStatusText.color = new Color(0.26f, 0.52f, 0.96f);
            }

            CloseLoginModal();
            RefreshProfileUI();
        }

        private void OnLogoutClicked()
        {
            PlayClickSound();
            OpenLoginModal();
        }

        private void RefreshProfileUI()
        {
            Sprite avatarSprite = (mascotAvatars != null && _currentAvatarIdx < mascotAvatars.Length)
                ? mascotAvatars[_currentAvatarIdx]
                : null;

            if (profileBtnAvatar != null && avatarSprite != null)
            {
                profileBtnAvatar.sprite = avatarSprite;
                profileBtnAvatar.color = Color.white;
            }

            if (lobbyCoinsText != null)
            {
                lobbyCoinsText.text = $"{FormatCoins(_currentCoins)} G";
            }

            if (profileModalAvatar != null && avatarSprite != null)
            {
                profileModalAvatar.sprite = avatarSprite;
            }

            if (!_isEditingNick)
            {
                if (profileModalNicknameInput != null)
                {
                    profileModalNicknameInput.text = _nicknameOnly;
                    profileModalNicknameInput.interactable = false;
                }

                if (profileModalTagInput != null)
                {
                    profileModalTagInput.text = _tagOnly;
                    profileModalTagInput.interactable = false;
                }

                if (btnStartEditNick != null) btnStartEditNick.gameObject.SetActive(true);
                if (btnCheckTag != null) btnCheckTag.gameObject.SetActive(false);
                if (btnConfirmNick != null) btnConfirmNick.gameObject.SetActive(false);
            }

            if (profileModalNickname != null)
            {
                profileModalNickname.text = $"{_nicknameOnly}#{_tagOnly}";
            }

            if (profileModalBioInput != null)
            {
                profileModalBioInput.text = _currentBio;
                profileModalBioInput.interactable = true;
                if (profileModalBioInput.placeholder is TMP_Text phText)
                {
                    phText.text = LocalizationManager.Get("profile_bio_placeholder");
                }
            }

            if (profileModalBestScore != null)
            {
                int best = PlayerPrefs.GetInt("BlockBlast_Best", 0);
                profileModalBestScore.text = $"{LocalizationManager.Get("profile_best_score_prefix")}: {best:N0}{LocalizationManager.Get("profile_best_score_suffix")}";
            }

            // Remove 보유 코인 row from profile modal as requested
            if (profileModalCoins != null)
            {
                profileModalCoins.gameObject.SetActive(false);
            }

            if (btnLogout != null)
            {
                TMP_Text btnText = btnLogout.GetComponentInChildren<TMP_Text>();
                if (btnText != null)
                {
                    btnText.text = "로그인";
                }
            }

            if (profileModal != null)
            {
                var card = profileModal.transform.Find("DialogCard");
                if (card != null)
                {
                    var bioLbl = card.Find("BioLbl")?.GetComponent<TMP_Text>();
                    if (bioLbl != null) bioLbl.text = LocalizationManager.Get("profile_bio_label");
                    var pickLbl = card.Find("PickLbl")?.GetComponent<TMP_Text>();
                    if (pickLbl != null) pickLbl.text = LocalizationManager.Get("profile_pick_label");
                }
            }
        }

        // ==========================================
        // SHOP MODAL & THEME STORE
        // ==========================================

        public void OpenShopModal()
        {
            if (shopModal != null)
            {
                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
                if (btnPlayGame != null) btnPlayGame.gameObject.SetActive(false);
                if (FairyScreenTransition.Instance != null) FairyScreenTransition.Instance.EmitCornerSparkles();
                shopModal.transform.SetAsLastSibling();
                shopModal.SetActive(true);
                PolishShopUIElements();
                SelectShopTab(_currentShopTab);
                RefreshThemeShopUI();
                RefreshLobbyThemeShopUI();
                WireModalAutoClose(shopModal, CloseShopModal);
                if (shopAnimController != null && shopAnimController.gameObject.activeInHierarchy) shopAnimController.AnimateOpen();
            }
        }

        public void CloseShopModal()
        {
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
            if (pickupBannerController != null && pickupBannerController.gameObject.activeInHierarchy) pickupBannerController.StopVideo();
            if (btnPlayGame != null) btnPlayGame.gameObject.SetActive(true);
            if (shopAnimController != null && shopAnimController.gameObject.activeInHierarchy)
            {
                shopAnimController.AnimateClose(() =>
                {
                    if (shopModal != null) shopModal.SetActive(false);
                });
            }
            else
            {
                if (shopModal != null) shopModal.SetActive(false);
            }
        }

        public void BuyOrEquipTheme(int themeIdx)
        {
            if (themeIdx < 0 || themeIdx >= ThemePrices.Length) return;

            bool isOwned = (themeIdx == 0) || (PlayerPrefs.GetInt(KEY_THEME_OWNED_PREFIX + themeIdx, 1) == 1);

            if (isOwned)
            {
                // Equip Theme
                PlayerPrefs.SetInt(KEY_EQUIPPED_THEME, themeIdx);
                PlayerPrefs.Save();
                ApplyTheme(themeIdx);
                RefreshThemeShopUI();
                PlayClickSound();
            }
            else
            {
                int price = ThemePrices[themeIdx];
                if (_currentCoins >= price)
                {
                    _currentCoins -= price;
                    PlayerPrefs.SetInt(KEY_COINS, _currentCoins);
                    PlayerPrefs.SetInt(KEY_THEME_OWNED_PREFIX + themeIdx, 1);
                    PlayerPrefs.SetInt(KEY_EQUIPPED_THEME, themeIdx);
                    PlayerPrefs.Save();

                    ApplyTheme(themeIdx);
                    RefreshProfileUI();
                    RefreshThemeShopUI();

                    if (FairyScreenTransition.Instance != null)
                    {
                        FairyScreenTransition.Instance.EmitCornerSparkles();
                    }
                    if (BlockAudioManager.Instance != null)
                    {
                        BlockAudioManager.Instance.PlayBuy();
                    }
                }
                else
                {
                    // Insufficient Coins
                    PlayClickSound();
                    if (shopCoinsText != null)
                    {
                        StartCoroutine(FlashCoinsTextRed());
                    }
                }
            }
        }

        private IEnumerator FlashCoinsTextRed()
        {
            if (shopCoinsText == null) yield break;
            Color orig = shopCoinsText.color;
            shopCoinsText.color = new Color(1f, 0.25f, 0.35f);
            yield return new WaitForSeconds(0.4f);
            shopCoinsText.color = orig;
        }

        public void ApplyTheme(int themeIdx)
        {
            if (inGameBackgroundImg == null)
            {
                var bgObj = GameObject.Find("BackgroundImage");
                if (bgObj != null) inGameBackgroundImg = bgObj.GetComponent<Image>();
            }

            if (inGameBackgroundImg != null && themeSprites != null && themeIdx >= 0 && themeIdx < themeSprites.Length)
            {
                if (themeSprites[themeIdx] != null)
                {
                    inGameBackgroundImg.sprite = themeSprites[themeIdx];
                    inGameBackgroundImg.color = Color.white;
                }
            }

            if (SideWingsDecorator.Instance != null)
            {
                SideWingsDecorator.Instance.SyncWithTheme(themeIdx);
            }
        }

        public void RefreshThemeShopUI()
        {
            RefreshCurrenciesUI();

            int equippedTheme = PlayerPrefs.GetInt(KEY_EQUIPPED_THEME, 0);

            if (themeActionButtons != null)
            {
                for (int i = 0; i < themeActionButtons.Length; i++)
                {
                    if (themeActionButtons[i] == null) continue;

                    bool isEquipped = (equippedTheme == i);
                    bool isOwned = (i == 0) || (PlayerPrefs.GetInt(KEY_THEME_OWNED_PREFIX + i, 1) == 1);

                    Image btnImg = themeActionButtons[i].GetComponent<Image>();
                    TMP_Text txt = (themeActionTexts != null && i < themeActionTexts.Length && themeActionTexts[i] != null)
                        ? themeActionTexts[i]
                        : themeActionButtons[i].GetComponentInChildren<TMP_Text>();

                    if (isEquipped)
                    {
                        if (txt != null)
                        {
                            txt.text = LocalizationManager.Get("shop_btn_equipped");
                            txt.color = new Color(0.06f, 0.35f, 0.26f, 1f); // Dark Forest Teal
                        }
                        if (btnImg != null)
                        {
                            if (shopEquippedBtnSprite != null) btnImg.sprite = shopEquippedBtnSprite;
                            btnImg.color = Color.white;
                        }
                        themeActionButtons[i].interactable = false;
                    }
                    else if (isOwned)
                    {
                        if (txt != null)
                        {
                            txt.text = LocalizationManager.Get("shop_btn_equip");
                            txt.color = Color.white;
                            txt.enableAutoSizing = true;
                            txt.fontSizeMin = 13f;
                        }
                        if (btnImg != null)
                        {
                            if (shopEquipBtnSprite != null) btnImg.sprite = shopEquipBtnSprite;
                            btnImg.color = Color.white;
                        }
                        themeActionButtons[i].interactable = true;
                    }
                    else
                    {
                        int price = (i < ThemePrices.Length) ? ThemePrices[i] : 0;
                        bool canAfford = (_currentCoins >= price);
                        if (txt != null)
                        {
                            txt.text = $"{price:N0} C " + LocalizationManager.Get("shop_btn_buy");
                            txt.color = canAfford ? new Color(0.24f, 0.12f, 0.00f, 1f) : new Color(0.42f, 0.32f, 0.52f, 1f);
                            txt.fontStyle = FontStyles.Bold;
                            txt.enableAutoSizing = true;
                            txt.fontSizeMin = 16f;
                        }
                        if (btnImg != null)
                        {
                            if (canAfford)
                            {
                                btnImg.sprite = (shopGoldBtnSprite != null) ? shopGoldBtnSprite : shopEquipBtnSprite;
                            }
                            else
                            {
                                btnImg.sprite = (shopCreamBtnSprite != null) ? shopCreamBtnSprite : shopEquipBtnSprite;
                            }
                            btnImg.color = Color.white;
                        }
                        themeActionButtons[i].interactable = true;
                    }
                }
            }

            if (shopInGameThemesPanel != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    var itemCard = shopInGameThemesPanel.transform.Find($"ThemeItem_{i}");
                    if (itemCard != null)
                    {
                        var nameTxt = itemCard.Find("Name")?.GetComponent<TMP_Text>();
                        if (nameTxt != null) nameTxt.text = LocalizationManager.Get($"theme_game_{i}_name");
                        var descTxt = itemCard.Find("Desc")?.GetComponent<TMP_Text>();
                        if (descTxt != null) descTxt.text = LocalizationManager.Get($"theme_game_{i}_desc");
                    }
                }
            }
        }

        public void SetupShopThemes(Image inGameBg, Sprite[] bgSprites, Button[] actionBtns, TMP_Text[] actionTexts, TMP_Text[] priceTexts = null)
        {
            inGameBackgroundImg = inGameBg;
            themeSprites = bgSprites;
            themeActionButtons = actionBtns;
            themeActionTexts = actionTexts;
            themePriceTexts = priceTexts;

            if (themeActionButtons != null)
            {
                for (int i = 0; i < themeActionButtons.Length; i++)
                {
                    int idx = i;
                    if (themeActionButtons[i] != null)
                    {
                        themeActionButtons[i].onClick.RemoveAllListeners();
                        themeActionButtons[i].onClick.AddListener(() =>
                        {
                            BuyOrEquipTheme(idx);
                        });
                    }
                }
            }

            int equippedTheme = PlayerPrefs.GetInt(KEY_EQUIPPED_THEME, 0);
            ApplyTheme(equippedTheme);
            RefreshThemeShopUI();
        }

        public void SetupShopButtonSprites(Sprite equipSp, Sprite equippedSp, Sprite creamSp = null, Sprite goldSp = null, Sprite purpleSp = null)
        {
            shopEquipBtnSprite = equipSp;
            shopEquippedBtnSprite = equippedSp;
            if (creamSp != null) shopCreamBtnSprite = creamSp;
            if (goldSp != null) shopGoldBtnSprite = goldSp;
            if (purpleSp != null) shopPurpleBtnSprite = purpleSp;
            RefreshThemeShopUI();
            RefreshLobbyThemeShopUI();
            RefreshMascotShopUI();
        }

        // ==========================================
        // SHOP TABS & LOBBY THEME STORE
        // ==========================================

        public void SelectShopTab(int tabIndex)
        {
            _currentShopTab = Mathf.Clamp(tabIndex, 0, 4);

            if (shopRecommendedPanel != null) shopRecommendedPanel.SetActive(_currentShopTab == 0);
            if (shopPickupPanel != null) shopPickupPanel.SetActive(_currentShopTab == 1);
            if (shopMascotsPanel != null) shopMascotsPanel.SetActive(_currentShopTab == 2);
            if (shopInGameThemesPanel != null) shopInGameThemesPanel.SetActive(_currentShopTab == 3);
            if (shopLobbyThemesPanel != null) shopLobbyThemesPanel.SetActive(_currentShopTab == 4);

            GameObject activePanel = null;
            if (_currentShopTab == 0) activePanel = shopRecommendedPanel;
            else if (_currentShopTab == 1) activePanel = shopPickupPanel;
            else if (_currentShopTab == 2) activePanel = shopMascotsPanel;
            else if (_currentShopTab == 3) activePanel = shopInGameThemesPanel;
            else if (_currentShopTab == 4) activePanel = shopLobbyThemesPanel;

            if (shopAnimController != null && shopAnimController.gameObject.activeInHierarchy && activePanel != null)
            {
                shopAnimController.AnimateTabGlide(_currentShopTab, activePanel.GetComponent<RectTransform>());
            }

            if (_currentShopTab == 1)
            {
                if (pickupBannerController != null && pickupBannerController.gameObject.activeInHierarchy)
                {
                    pickupBannerController.PlayIntroVideo();
                }
            }
            else
            {
                if (pickupBannerController != null && pickupBannerController.gameObject.activeInHierarchy)
                {
                    pickupBannerController.StopVideo();
                }
            }

            if (shopTabBgs != null)
            {
                for (int i = 0; i < shopTabBgs.Length; i++)
                {
                    if (shopTabBgs[i] == null) continue;
                    bool isActive = (i == _currentShopTab);
                    if (shopAnimController != null)
                    {
                        shopTabBgs[i].color = Color.clear;
                    }
                    else if (tabVerticalActiveSprite != null && tabVerticalInactiveSprite != null)
                    {
                        shopTabBgs[i].sprite = isActive ? tabVerticalActiveSprite : tabVerticalInactiveSprite;
                        shopTabBgs[i].color = Color.white;
                    }
                    else
                    {
                        shopTabBgs[i].color = isActive ? ColorPink : new Color(0.92f, 0.90f, 0.96f, 0.85f);
                    }
                }
            }

            if (shopTabTexts != null)
            {
                for (int i = 0; i < shopTabTexts.Length; i++)
                {
                    if (shopTabTexts[i] == null) continue;
                    bool isActive = (i == _currentShopTab);
                    shopTabTexts[i].color = isActive ? Color.white : new Color(0.42f, 0.30f, 0.58f, 1f);
                    shopTabTexts[i].fontStyle = FontStyles.Bold;
                }
            }

            RefreshCurrenciesUI();

            switch (_currentShopTab)
            {
                case 0:
                    RefreshShopPackagesUI();
                    break;
                case 1:
                    RefreshShopSummonUI();
                    break;
                case 2:
                    RefreshMascotShopUI();
                    break;
                case 3:
                    RefreshThemeShopUI();
                    break;
                case 4:
                    RefreshLobbyThemeShopUI();
                    break;
            }
        }

        // ==========================================
        // MASCOT MANAGEMENT MODAL
        // ==========================================

        public void OpenMascotModal()
        {
            if (mascotCodexModal != null)
            {
                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
                if (btnPlayGame != null) btnPlayGame.gameObject.SetActive(false);
                if (FairyScreenTransition.Instance != null) FairyScreenTransition.Instance.EmitCornerSparkles();
                EnsureCodexCardButtonsBound();
                RefreshMascotCodexUI();
                WireModalAutoClose(mascotCodexModal, CloseMascotModal);
                mascotCodexModal.transform.SetAsLastSibling();
                mascotCodexModal.SetActive(true);
            }
            else if (mascotModal != null)
            {
                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
                if (btnPlayGame != null) btnPlayGame.gameObject.SetActive(false);
                if (FairyScreenTransition.Instance != null) FairyScreenTransition.Instance.EmitCornerSparkles();
                RefreshMascotModalUI();
                WireModalAutoClose(mascotModal, CloseMascotModal);
                mascotModal.transform.SetAsLastSibling();
                mascotModal.SetActive(true);
            }
        }

        public void CloseMascotModal()
        {
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
            if (mascotDetailModal != null && mascotDetailModal.activeSelf)
            {
                mascotDetailModal.SetActive(false);
            }
            if (mascotCodexModal != null) mascotCodexModal.SetActive(false);
            if (mascotModal != null) mascotModal.SetActive(false);
            if (btnPlayGame != null) btnPlayGame.gameObject.SetActive(true);
        }

        public void OpenMascotDetail(int idx)
        {
            if (idx < 0 || idx >= 9) return;
            _selectedDetailMascotIdx = idx;
            if (btnPlayGame != null) btnPlayGame.gameObject.SetActive(false);
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayUIClick();
            if (FairyScreenTransition.Instance != null) FairyScreenTransition.Instance.EmitCornerSparkles();
            RefreshMascotDetailUI();
            if (mascotDetailModal != null)
            {
                WireModalAutoClose(mascotDetailModal, CloseMascotDetail);
                mascotDetailModal.transform.SetAsLastSibling();
                mascotDetailModal.SetActive(true);
            }
        }

        public void CloseMascotDetail()
        {
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayUIClick();
            if (mascotDetailModal != null) mascotDetailModal.SetActive(false);
            RefreshMascotCodexUI();
            if ((mascotCodexModal == null || !mascotCodexModal.activeSelf) && (mascotModal == null || !mascotModal.activeSelf))
            {
                if (btnPlayGame != null) btnPlayGame.gameObject.SetActive(true);
            }
        }

        public void OnClickDetailLevelUp()
        {
            int idx = _selectedDetailMascotIdx;
            bool isOwned = (idx == 0) || (PlayerPrefs.GetInt(KEY_MASCOT_OWNED_PREFIX + idx, 0) == 1);
            if (!isOwned) return;

            int curLvl = GetMascotLevel(idx);
            int maxCap = GetMaxLevelCap(idx);
            int cost = GetLevelUpCoinCost(idx);

            if (curLvl < maxCap && _currentCoins >= cost)
            {
                _currentCoins -= cost;
                PlayerPrefs.SetInt(KEY_COINS, _currentCoins);
                SetMascotLevel(idx, curLvl + 1);
                PlayerPrefs.Save();

                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayBuy();
                if (FairyScreenTransition.Instance != null) FairyScreenTransition.Instance.EmitCornerSparkles();
                RefreshCurrenciesUI();
                RefreshMascotDetailUI();
                RefreshMascotCodexUI();
            }
            else
            {
                PlayClickSound();
            }
        }

        public void OnClickDetailBreakthrough()
        {
            int idx = _selectedDetailMascotIdx;
            bool isOwned = (idx == 0) || (PlayerPrefs.GetInt(KEY_MASCOT_OWNED_PREFIX + idx, 0) == 1);
            if (!isOwned) return;

            int stars = GetBreakthroughStars(idx);
            if (stars >= 5) return;

            int cost = GetBreakthroughCost(idx);
            int shards = GetMascotShards(idx);

            if (shards >= cost)
            {
                SetMascotShards(idx, shards - cost);
                SetBreakthroughStars(idx, stars + 1);
                PlayerPrefs.Save();

                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayFairyMagic();
                if (FairyScreenTransition.Instance != null) FairyScreenTransition.Instance.EmitCornerSparkles();
                RefreshMascotDetailUI();
                RefreshMascotCodexUI();
            }
            else
            {
                PlayClickSound();
            }
        }

        public void OnClickDetailEquipOrUnlock()
        {
            int idx = _selectedDetailMascotIdx;
            bool isOwned = (idx == 0) || (PlayerPrefs.GetInt(KEY_MASCOT_OWNED_PREFIX + idx, 0) == 1);

            if (!isOwned)
            {
                // Unowned mascots: Navigate to Pickup tab for Angel & Pickup mascots, else Shop Mascot tab!
                PlayClickSound();
                CloseMascotDetail();
                CloseMascotModal();
                OpenShopModal();
                SelectShopTab(idx >= 4 ? 1 : 2);
            }
            else
            {
                EquipMascot(idx);
                RefreshMascotDetailUI();
                RefreshMascotCodexUI();
            }
        }

        public void EquipMascot(int mascotIdx)
        {
            if (mascotIdx < 0 || mascotIdx >= 9) return;
            bool isOwned = (mascotIdx == 0) || (PlayerPrefs.GetInt(KEY_MASCOT_OWNED_PREFIX + mascotIdx, 0) == 1);
            if (!isOwned)
            {
                PlayClickSound();
                return;
            }

            PlayerPrefs.SetInt(KEY_SELECTED_MASCOT, mascotIdx);
            PlayerPrefs.Save();
            PlayClickSound();
            if (FairyScreenTransition.Instance != null) FairyScreenTransition.Instance.EmitCornerSparkles();
            RefreshMascotModalUI();
            RefreshMascotCodexUI();
            RefreshMascotShopUI();
        }

        public void TryUpgradeMascot(int idx)
        {
            if (idx < 0 || idx >= 9) return;
            int shards = GetMascotShards(idx);
            int cost = GetBreakthroughCost(idx);
            if (cost > 0 && shards >= cost)
            {
                SetMascotShards(idx, shards - cost);
                int stars = GetBreakthroughStars(idx);
                SetBreakthroughStars(idx, stars + 1);
                PlayerPrefs.Save();

                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayBuy();
                if (FairyScreenTransition.Instance != null) FairyScreenTransition.Instance.EmitCornerSparkles();

                RefreshMascotModalUI();
                RefreshMascotCodexUI();
                RefreshMascotShopUI();
            }
            else
            {
                PlayClickSound();
            }
        }

        public void RefreshMascotCodexUI()
        {
            int selectedMascot = PlayerPrefs.GetInt(KEY_SELECTED_MASCOT, 0);
            int ownedCount = 0;

            for (int i = 0; i < 9; i++)
            {
                bool isOwned = (i == 0) || (PlayerPrefs.GetInt(KEY_MASCOT_OWNED_PREFIX + i, 0) == 1);
                if (isOwned) ownedCount++;

                int stars = GetBreakthroughStars(i);
                int lvl = GetMascotLevel(i);
                int maxCap = GetMaxLevelCap(i);
                int shards = GetMascotShards(i);

                if (codexCardNames != null && i < codexCardNames.Length && codexCardNames[i] != null)
                {
                    codexCardNames[i].text = GetLocalizedMascotName(i);
                }

                if (codexCardLevels != null && i < codexCardLevels.Length && codexCardLevels[i] != null)
                {
                    codexCardLevels[i].text = $"Lv.{lvl}/{maxCap}";
                }

                if (codexCardStars != null && i < codexCardStars.Length && codexCardStars[i] != null)
                {
                    string starsStr = "";
                    for (int s = 0; s < 5; s++)
                    {
                        starsStr += (s < stars) ? "<color=#FFD700>★</color>" : "<color=#B0A8C0>☆</color>";
                    }
                    codexCardStars[i].text = starsStr;
                }

                if (codexCardLockedOverlays != null && i < codexCardLockedOverlays.Length && codexCardLockedOverlays[i] != null)
                {
                    codexCardLockedOverlays[i].SetActive(!isOwned);
                }

                if (codexCardStatusBadges != null && i < codexCardStatusBadges.Length && codexCardStatusBadges[i] != null)
                {
                    if (selectedMascot == i)
                    {
                        codexCardStatusBadges[i].text = $"<color=#00796B><b>● {LocalizationManager.Get("codex_equipped")}</b></color>";
                    }
                    else if (isOwned)
                    {
                        codexCardStatusBadges[i].text = $"<color=#C2185B><b>[ {LocalizationManager.Get("mascot_btn_equip")} ]</b></color>";
                    }
                    else
                    {
                        codexCardStatusBadges[i].text = $"<color=#1E3A8A><b>{string.Format(LocalizationManager.Get("mascot_shards_owned"), shards)}</b></color>";
                    }
                }

                if (codexCardRarityTexts != null && i < codexCardRarityTexts.Length && codexCardRarityTexts[i] != null)
                {
                    if (i < 4) codexCardRarityTexts[i].text = LocalizationManager.Get("codex_badge_common");
                    else if (i < 8) codexCardRarityTexts[i].text = LocalizationManager.Get("codex_badge_rare");
                    else codexCardRarityTexts[i].text = LocalizationManager.Get("codex_badge_special");
                }
            }

            if (codexCollectionCountText != null)
            {
                codexCollectionCountText.text = $"<color=#3A1C05>{LocalizationManager.Get("codex_collected")}:</color> <color=#C2185B><b>{ownedCount}</b></color> <color=#3A1C05>/ 9</color>";
            }
            if (codexTitleText != null) codexTitleText.text = LocalizationManager.Get("codex_title");
            if (codexSubtitleText != null) codexSubtitleText.text = LocalizationManager.Get("codex_subtitle");
        }

        public void RefreshMascotDetailUI()
        {
            int idx = _selectedDetailMascotIdx;
            bool isOwned = (idx == 0) || (PlayerPrefs.GetInt(KEY_MASCOT_OWNED_PREFIX + idx, 0) == 1);
            int selectedMascot = PlayerPrefs.GetInt(KEY_SELECTED_MASCOT, 0);
            int curLvl = GetMascotLevel(idx);
            int stars = GetBreakthroughStars(idx);
            int maxCap = GetMaxLevelCap(idx);
            int shards = GetMascotShards(idx);
            int breakthroughCost = GetBreakthroughCost(idx);
            int levelUpCost = GetLevelUpCoinCost(idx);

            if (detailMascotAvatar != null)
            {
                if (mascotAvatars != null && idx < mascotAvatars.Length)
                {
                    detailMascotAvatar.sprite = mascotAvatars[idx];
                }
                var jelly = detailMascotAvatar.GetComponent<PuddingJellyTouchPhysics>();
                if (jelly != null)
                {
                    jelly.CaptureCurrentAsOriginal();
                    jelly.ResetToRested();
                }
            }
            if (detailMascotName != null) detailMascotName.text = GetLocalizedMascotName(idx);
            if (detailMascotTitle != null) detailMascotTitle.text = $"[{GetLocalizedMascotTitle(idx)}]";

            // 상세 수치 및 다음 레벨 강화 프리뷰 계산
            var curStats = GetMascotStats(idx, curLvl);
            var nextStats = (curLvl < maxCap) ? GetMascotStats(idx, curLvl + 1) : curStats;
            float diffTime = nextStats.extraTime - curStats.extraTime;
            int diffSkip = nextStats.skipCount - curStats.skipCount;
            float diffScore = nextStats.bonusScore - curStats.bonusScore;
            float diffExp = nextStats.bonusExp - curStats.bonusExp;
            string story = GetLocalizedMascotStory(idx);
            string growthFocus = GetMascotGrowthFocusText(idx);

            // 스토리 텍스트 갱신
            if (detailStoryText != null)
            {
                detailStoryText.text = $"“{story}”";
            }

            // 고유 블록 3단 진화 비주얼 및 정보 갱신 (기본 / 2돌파 / 5돌파 동시 표시 및 잠금 회색화)
            RefreshUniqueBlockEvolutionUI();

            // 특화 성장 안내 문구
            if (detailGrowthFocusText != null)
            {
                detailGrowthFocusText.text = $"※ {growthFocus}";
            }

            void SetGaugeFill(Image img, float fill)
        {
            if (img == null) return;
            fill = Mathf.Clamp01(fill);
            img.fillAmount = fill;
            RectTransform rt = img.rectTransform;
            if (rt != null)
            {
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = new Vector2(fill, 1f);
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }
            img.gameObject.SetActive(fill > 0.005f);
        }

        // 1. 추가 시간 게이지 & 강화 프리뷰
        if (detailGaugeTimeFill != null)
        {
            SetGaugeFill(detailGaugeTimeFill, curStats.extraTime / 10.0f);
        }
        if (detailStatTimeVal != null)
        {
            if (curLvl < maxCap)
            {
                detailStatTimeVal.text = $"<b>+{curStats.extraTime:F1}초</b> <color=#00B894>➔ <b>+{nextStats.extraTime:F1}초</b></color> <color=#F39C12><b>(+{diffTime:F1}초 ▲)</b></color>";
            }
            else
            {
                detailStatTimeVal.text = $"<b>+{curStats.extraTime:F1}초</b> <color=#E17055><b>[MAX 레벨]</b></color>";
            }
        }

        // 2. 스킵 횟수 게이지 & 강화 프리뷰
        if (detailGaugeSkipFill != null)
        {
            SetGaugeFill(detailGaugeSkipFill, (float)curStats.skipCount / 6.0f);
        }
        if (detailStatSkipVal != null)
        {
            if (curLvl < maxCap)
            {
                if (diffSkip > 0)
                    detailStatSkipVal.text = $"<b>+{curStats.skipCount}회</b> <color=#00B894>➔ <b>+{nextStats.skipCount}회</b></color> <color=#F39C12><b>(+{diffSkip}회 ▲)</b></color>";
                else
                    detailStatSkipVal.text = $"<b>+{curStats.skipCount}회</b> <color=#00B894>➔ <b>+{nextStats.skipCount}회</b></color> <color=#888888>(유지)</color>";
            }
            else
            {
                detailStatSkipVal.text = $"<b>+{curStats.skipCount}회</b> <color=#E17055><b>[MAX 레벨]</b></color>";
            }
        }

        // 3. 추가 점수 게이지 & 강화 프리뷰
        if (detailGaugeScoreFill != null)
        {
            SetGaugeFill(detailGaugeScoreFill, curStats.bonusScore / 60.0f);
        }
        if (detailStatScoreVal != null)
        {
            if (curLvl < maxCap)
            {
                detailStatScoreVal.text = $"<b>+{curStats.bonusScore:F1}%</b> <color=#00B894>➔ <b>+{nextStats.bonusScore:F1}%</b></color> <color=#F39C12><b>(+{diffScore:F1}% ▲)</b></color>";
            }
            else
            {
                detailStatScoreVal.text = $"<b>+{curStats.bonusScore:F1}%</b> <color=#E17055><b>[MAX 레벨]</b></color>";
            }
        }

        // 4. 추가 경험치 게이지 & 강화 프리뷰
        if (detailGaugeExpFill != null)
        {
            SetGaugeFill(detailGaugeExpFill, curStats.bonusExp / 60.0f);
        }
            if (detailStatExpVal != null)
            {
                if (curLvl < maxCap)
                {
                    detailStatExpVal.text = $"<b>+{curStats.bonusExp:F1}%</b> <color=#00B894>➔ <b>+{nextStats.bonusExp:F1}%</b></color> <color=#F39C12><b>(+{diffExp:F1}% ▲)</b></color>";
                }
                else
                {
                    detailStatExpVal.text = $"<b>+{curStats.bonusExp:F1}%</b> <color=#E17055><b>[MAX 레벨]</b></color>";
                }
            }

            // 상세 창 통합 텍스트 설명 (기존 하위 호환)
            if (detailAbilityDesc != null)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                // 1. 스토리 (2줄 스토리)
                if (!string.IsNullOrEmpty(story))
                {
                    sb.AppendLine($"<color=#7C5295><b>[말랑이 스토리]</b></color>\n{story}\n");
                }

                // 2. 고유 블록 정보
                sb.AppendLine($"<color=#D63031><b>[고유 블록]</b></color>");
                sb.AppendLine($"• <b>{curStats.uniqueBlockName}</b>");
                sb.AppendLine($"• 효과: {curStats.uniqueBlockAbility}\n");

                // 3. 5대 스텟 및 특화 성장 정보
                sb.AppendLine($"<color=#0984E3><b>[말랑이 강화 & 스텟 (Lv.{curLvl})]</b></color>");
                sb.AppendLine($"• 추가 시간: +{curStats.extraTime:F1}초 ➔ +{nextStats.extraTime:F1}초 (+{diffTime:F1}초 ▲)");
                sb.AppendLine($"• 스킵 횟수: +{curStats.skipCount}회 ➔ +{nextStats.skipCount}회");
                sb.AppendLine($"• 추가 점수: +{curStats.bonusScore:F1}% ➔ +{nextStats.bonusScore:F1}% (+{diffScore:F1}% ▲)");
                sb.AppendLine($"• 추가 경험치: +{curStats.bonusExp:F1}% ➔ +{nextStats.bonusExp:F1}% (+{diffExp:F1}% ▲)");

                if (!string.IsNullOrEmpty(growthFocus))
                {
                    sb.AppendLine($"\n<color=#E67E22><b>※ {growthFocus}</b></color>");
                }

                detailAbilityDesc.text = sb.ToString();
            }

            if (mascotDetailModal != null)
            {
                var card = mascotDetailModal.transform.Find("DialogCard");
                if (card != null)
                {
                    var t = card.Find("Title")?.GetComponent<TMP_Text>();
                    if (t != null) t.text = LocalizationManager.Get("mascot_detail_title");
                    var hdr = card.Find("AbilityBox/AbilHeader");
                    if (hdr != null) hdr.gameObject.SetActive(false);
                }
            }

            if (detailRarityText != null)
            {
                if (idx < 4) detailRarityText.text = LocalizationManager.Get("codex_badge_common");
                else if (idx < 8) detailRarityText.text = LocalizationManager.Get("codex_badge_rare");
                else detailRarityText.text = LocalizationManager.Get("codex_badge_special");
            }

            if (detailStarsText != null)
            {
                string starsStr = "";
                for (int s = 0; s < 5; s++)
                {
                    starsStr += (s < stars) ? "<color=#FFD700>★ </color>" : "<color=#D0C8E0>☆ </color>";
                }
                detailStarsText.text = starsStr.TrimEnd();
            }

            if (detailLevelText != null)
            {
                detailLevelText.text = $"{LocalizationManager.Get("mascot_detail_level")} {curLvl} / {maxCap}";
            }
            if (detailLevelFill != null)
            {
                SetGaugeFill(detailLevelFill, (float)curLvl / maxCap);
            }

            if (detailShardsText != null)
            {
                if (breakthroughCost > 0)
                {
                    detailShardsText.text = string.Format(LocalizationManager.Get("mascot_shards_breakthrough_fmt"), LocalizationManager.Get("mascot_detail_shards"), shards, breakthroughCost);
                }
                else
                {
                    detailShardsText.text = $"{LocalizationManager.Get("mascot_detail_shards")}: {shards} ({LocalizationManager.Get("mascot_max_breakthrough")})";
                }
            }

            // [강화 / Level Up] Button
            if (btnDetailLevelUp != null)
            {
                bool canLevelUp = isOwned && (curLvl < maxCap) && (_currentCoins >= levelUpCost);
                btnDetailLevelUp.interactable = canLevelUp;
                if (txtDetailLevelUp != null)
                {
                    if (!isOwned)
                        txtDetailLevelUp.text = LocalizationManager.Get("mascot_status_locked");
                    else if (curLvl >= maxCap)
                        txtDetailLevelUp.text = LocalizationManager.Get("mascot_max_level");
                    else
                        txtDetailLevelUp.text = $"{LocalizationManager.Get("mascot_btn_levelup")}\n{levelUpCost:N0} G";
                }
            }

            // [돌파 / Breakthrough] Button
            if (btnDetailBreakthrough != null)
            {
                bool canBreakthrough = isOwned && (stars < 5) && (breakthroughCost > 0) && (shards >= breakthroughCost);
                btnDetailBreakthrough.interactable = canBreakthrough;
                if (txtDetailBreakthrough != null)
                {
                    if (!isOwned)
                        txtDetailBreakthrough.text = LocalizationManager.Get("mascot_status_locked");
                    else if (stars >= 5)
                        txtDetailBreakthrough.text = LocalizationManager.Get("mascot_max_breakthrough");
                    else
                        txtDetailBreakthrough.text = string.Format(LocalizationManager.Get("mascot_shards_progress_fmt"), LocalizationManager.Get("mascot_btn_breakthrough"), shards, breakthroughCost);
                }
            }

            // [장착 / Equip] Button
            if (btnDetailEquip != null)
            {
                if (!isOwned)
                {
                    btnDetailEquip.interactable = true;
                    if (txtDetailEquip != null)
                    {
                        txtDetailEquip.text = (idx >= 4) ? LocalizationManager.Get("mascot_obtain_pickup") : LocalizationManager.Get("mascot_btn_buy_in_shop");
                    }
                }
                else
                {
                    bool isEquipped = (selectedMascot == idx);
                    btnDetailEquip.interactable = !isEquipped;
                    if (txtDetailEquip != null)
                    {
                        txtDetailEquip.text = isEquipped ? LocalizationManager.Get("mascot_btn_equipped") : LocalizationManager.Get("mascot_btn_equip");
                    }
                }
            }
        }

        public void SetupCodexModal(
            GameObject cModal, Button cClose, TMP_Text cTitle, TMP_Text cSub, TMP_Text cCount,
            Button[] cardBtns, Image[] cardAvatars, TMP_Text[] cardNames, TMP_Text[] cardLevels,
            TMP_Text[] cardStars, GameObject[] cardLocked, TMP_Text[] cardStatuses, TMP_Text[] cardRarities)
        {
            mascotCodexModal = cModal;
            btnCloseMascotCodex = cClose;
            codexTitleText = cTitle;
            codexSubtitleText = cSub;
            codexCollectionCountText = cCount;
            codexCardButtons = cardBtns;
            codexCardAvatars = cardAvatars;
            codexCardNames = cardNames;
            codexCardLevels = cardLevels;
            codexCardStars = cardStars;
            codexCardLockedOverlays = cardLocked;
            codexCardStatusBadges = cardStatuses;
            codexCardRarityTexts = cardRarities;

            if (mascotCodexModal != null) mascotCodexModal.SetActive(false);

            if (btnCloseMascotCodex != null)
            {
                btnCloseMascotCodex.onClick.RemoveAllListeners();
                btnCloseMascotCodex.onClick.AddListener(CloseMascotModal);
            }

            EnsureCodexCardButtonsBound();
        }

        public void EnsureCodexCardButtonsBound()
        {
            if (codexCardButtons != null)
            {
                for (int i = 0; i < codexCardButtons.Length; i++)
                {
                    int idx = i;
                    if (codexCardButtons[i] != null)
                    {
                        codexCardButtons[i].onClick.RemoveAllListeners();
                        codexCardButtons[i].onClick.AddListener(() => OpenMascotDetail(idx));
                    }
                }
            }
        }

        public void SetupPlayerLevelBadge(TMP_Text lvlText)
        {
            playerLevelBadgeText = lvlText;
            if (playerLevelBadgeText != null)
            {
                int pLvl = PlayerPrefs.GetInt("Mallang_Player_Level", 1);
                playerLevelBadgeText.text = $"{pLvl}";
            }
        }

        public void SetupMascotDetailModal(
            GameObject dModal, Button dClose, Image dAvatar, TMP_Text dName, TMP_Text dTitle,
            TMP_Text dRarity, TMP_Text dStars, TMP_Text dLevel, Image dLevelFill, TMP_Text dShards,
            TMP_Text dDesc, Button dBtnLevelUp, TMP_Text dTxtLevelUp, Button dBtnBreakthrough,
            TMP_Text dTxtBreakthrough, Button dBtnEquip, TMP_Text dTxtEquip)
        {
            mascotDetailModal = dModal;
            btnCloseMascotDetail = dClose;
            detailMascotAvatar = dAvatar;
            detailMascotName = dName;
            detailMascotTitle = dTitle;
            detailRarityText = dRarity;
            detailStarsText = dStars;
            detailLevelText = dLevel;
            detailLevelFill = dLevelFill;
            detailShardsText = dShards;
            detailAbilityDesc = dDesc;
            btnDetailLevelUp = dBtnLevelUp;
            txtDetailLevelUp = dTxtLevelUp;
            btnDetailBreakthrough = dBtnBreakthrough;
            txtDetailBreakthrough = dTxtBreakthrough;
            btnDetailEquip = dBtnEquip;
            txtDetailEquip = dTxtEquip;

            if (mascotDetailModal != null) mascotDetailModal.SetActive(false);

            if (btnCloseMascotDetail != null)
            {
                btnCloseMascotDetail.onClick.RemoveAllListeners();
                btnCloseMascotDetail.onClick.AddListener(CloseMascotDetail);
            }

            if (btnDetailLevelUp != null)
            {
                btnDetailLevelUp.onClick.RemoveAllListeners();
                btnDetailLevelUp.onClick.AddListener(OnClickDetailLevelUp);
            }

            if (btnDetailBreakthrough != null)
            {
                btnDetailBreakthrough.onClick.RemoveAllListeners();
                btnDetailBreakthrough.onClick.AddListener(OnClickDetailBreakthrough);
            }

            if (btnDetailEquip != null)
            {
                btnDetailEquip.onClick.RemoveAllListeners();
                btnDetailEquip.onClick.AddListener(OnClickDetailEquipOrUnlock);
            }
        }

        public void SetupMascotDetailGauges(
            TMP_Text storyTxt,
            TMP_Text uniqueBlockTitle,
            TMP_Text uniqueBlockDesc,
            TMP_Text growthFocusTxt,
            Image timeFill, TMP_Text timeVal,
            Image skipFill, TMP_Text skipVal,
            Image scoreFill, TMP_Text scoreVal,
            Image expFill, TMP_Text expVal)
        {
            detailStoryText = storyTxt;
            detailGrowthFocusText = growthFocusTxt;
            detailGaugeTimeFill = timeFill;
            detailStatTimeVal = timeVal;
            detailGaugeSkipFill = skipFill;
            detailStatSkipVal = skipVal;
            detailGaugeScoreFill = scoreFill;
            detailStatScoreVal = scoreVal;
            detailGaugeExpFill = expFill;
            detailStatExpVal = expVal;
        }

        public void SetupMascotDetailUniqueBlockEvolution(
            RectTransform[] containers,
            Image[] cardBgs,
            TMP_Text[] badges,
            TMP_Text[] names,
            TMP_Text[] statuses,
            GameObject[] locks,
            Button[] buttons,
            TMP_Text overviewTxt,
            Sprite tileSprite)
        {
            detailBlockContainers = containers;
            detailStageCardBgs = cardBgs;
            detailStageBadges = badges;
            detailStageNames = names;
            detailStageStatuses = statuses;
            detailStageLocks = locks;
            detailStageButtons = buttons;
            detailEvolutionOverview = overviewTxt;
            miniBlockTileSprite = tileSprite;

            for (int i = 0; i < 3; i++)
            {
                int tier = i;
                if (detailStageButtons != null && i < detailStageButtons.Length && detailStageButtons[i] != null)
                {
                    detailStageButtons[i].onClick.RemoveAllListeners();
                    detailStageButtons[i].onClick.AddListener(() => OnClickEvolutionStage(tier));
                }
            }
        }

        public void RefreshUniqueBlockEvolutionUI()
        {
            int mascotIdx = _selectedDetailMascotIdx;
            int stars = GetBreakthroughStars(mascotIdx);

            bool[] isUnlocked = new bool[3];
            isUnlocked[0] = true;             // 기본: 상시 해금
            isUnlocked[1] = (stars >= 2);     // ★2돌: 2돌파 이상 해금
            isUnlocked[2] = (stars >= 5);     // ★5돌: 5돌파 이상 해금

            int activeTier = (stars >= 5) ? 2 : (stars >= 2) ? 1 : 0;
            _selectedEvolutionPreviewTier = activeTier;

            for (int tier = 0; tier < 3; tier++)
            {
                var info = GetMascotUniqueBlockInfo(mascotIdx, tier);
                bool unlocked = isUnlocked[tier];
                bool isActive = (tier == activeTier);

                // 1. Text & Badges
                if (detailStageBadges != null && tier < detailStageBadges.Length && detailStageBadges[tier] != null)
                {
                    if (tier == 0)
                        detailStageBadges[tier].text = "<color=#0984E3>[기본 고유]</color>";
                    else if (tier == 1)
                        detailStageBadges[tier].text = unlocked ? "<color=#E67E22>[★2돌 특수]</color>" : "<color=#888888>[★2돌 특수]</color>";
                    else
                        detailStageBadges[tier].text = unlocked ? "<color=#D63031>[★5돌 1×1]</color>" : "<color=#888888>[★5돌 1×1]</color>";
                }

                if (detailStageNames != null && tier < detailStageNames.Length && detailStageNames[tier] != null)
                {
                    detailStageNames[tier].text = unlocked ? $"<b>{info.blockName}</b>" : $"<color=#777777>{info.blockName}</color>";
                }

                if (detailStageStatuses != null && tier < detailStageStatuses.Length && detailStageStatuses[tier] != null)
                {
                    if (isActive)
                        detailStageStatuses[tier].text = "<color=#00B894><b>● 현재 적용</b></color>";
                    else if (unlocked)
                        detailStageStatuses[tier].text = "<color=#0984E3><b>✓ 해금됨</b></color>";
                    else
                        detailStageStatuses[tier].text = (tier == 1) ? "<color=#888888><b>🔒 2돌파 잠김</b></color>" : "<color=#888888><b>🔒 5돌파 잠김</b></color>";
                }

                // 2. Lock Overlay
                if (detailStageLocks != null && tier < detailStageLocks.Length && detailStageLocks[tier] != null)
                {
                    detailStageLocks[tier].SetActive(!unlocked);
                }

                // 3. Card Background Tint
                if (detailStageCardBgs != null && tier < detailStageCardBgs.Length && detailStageCardBgs[tier] != null)
                {
                    if (isActive)
                    {
                        detailStageCardBgs[tier].color = (tier == 2) ? new Color(1f, 0.93f, 0.95f, 1f) :
                                                         (tier == 1) ? new Color(1f, 0.96f, 0.91f, 1f) :
                                                                       new Color(0.93f, 0.98f, 0.96f, 1f);
                    }
                    else if (unlocked)
                    {
                        detailStageCardBgs[tier].color = new Color(0.98f, 0.98f, 1f, 0.95f);
                    }
                    else
                    {
                        // Gray / muted stone tint for locked feel
                        detailStageCardBgs[tier].color = new Color(0.90f, 0.88f, 0.93f, 0.65f);
                    }
                }

                // 4. Render Mini Block (Vibrant Color for Unlocked, Stone Gray for Locked!)
                if (detailBlockContainers != null && tier < detailBlockContainers.Length && detailBlockContainers[tier] != null)
                {
                    var container = detailBlockContainers[tier];
                    for (int cIdx = container.childCount - 1; cIdx >= 0; cIdx--)
                    {
                        GameObject childGo = container.GetChild(cIdx).gameObject;
                        if (Application.isPlaying) Destroy(childGo);
                        else DestroyImmediate(childGo);
                    }

                    // Grayscale color if locked, character color if unlocked!
                    Color cellColor = unlocked ? info.blockColor : new Color(0.55f, 0.53f, 0.60f, 0.80f);

                    if (info.isOneByOne)
                    {
                        // 1x1 Single Jewel Cell
                        GameObject cellObj = new GameObject("Cell_1x1", typeof(RectTransform), typeof(Image));
                        cellObj.transform.SetParent(container, false);
                        RectTransform rt = cellObj.GetComponent<RectTransform>();
                        rt.anchorMin = new Vector2(0.5f, 0.5f);
                        rt.anchorMax = new Vector2(0.5f, 0.5f);
                        rt.pivot = new Vector2(0.5f, 0.5f);
                        rt.sizeDelta = new Vector2(46f, 46f);
                        rt.anchoredPosition = Vector2.zero;

                        Image img = cellObj.GetComponent<Image>();
                        if (miniBlockTileSprite != null) img.sprite = miniBlockTileSprite;
                        img.type = Image.Type.Sliced;
                        img.color = cellColor;

                        // Star emblem
                        GameObject starObj = new GameObject("Star", typeof(RectTransform), typeof(TextMeshProUGUI));
                        starObj.transform.SetParent(cellObj.transform, false);
                        RectTransform srt = starObj.GetComponent<RectTransform>();
                        srt.anchorMin = Vector2.zero;
                        srt.anchorMax = Vector2.one;
                        srt.sizeDelta = Vector2.zero;
                        TextMeshProUGUI stmp = starObj.GetComponent<TextMeshProUGUI>();
                        stmp.text = "★";
                        stmp.fontSize = 24;
                        stmp.alignment = TextAlignmentOptions.Center;
                        stmp.color = unlocked ? Color.white : new Color(0.38f, 0.38f, 0.44f, 0.85f);
                    }
                    else
                    {
                        int rows = info.shapeMatrix.GetLength(0);
                        int cols = info.shapeMatrix.GetLength(1);
                        float cellSize = (cols >= 4 || rows >= 4) ? 14f : 18f;
                        float spacing = 2f;

                        float totalW = cols * cellSize + (cols - 1) * spacing;
                        float totalH = rows * cellSize + (rows - 1) * spacing;

                        float startX = -totalW * 0.5f + cellSize * 0.5f;
                        float startY = totalH * 0.5f - cellSize * 0.5f;

                        for (int r = 0; r < rows; r++)
                        {
                            for (int c = 0; c < cols; c++)
                            {
                                if (info.shapeMatrix[r, c] == 1)
                                {
                                    GameObject cellObj = new GameObject($"Cell_{r}_{c}", typeof(RectTransform), typeof(Image));
                                    cellObj.transform.SetParent(container, false);
                                    RectTransform rt = cellObj.GetComponent<RectTransform>();
                                    rt.anchorMin = new Vector2(0.5f, 0.5f);
                                    rt.anchorMax = new Vector2(0.5f, 0.5f);
                                    rt.pivot = new Vector2(0.5f, 0.5f);
                                    rt.sizeDelta = new Vector2(cellSize, cellSize);
                                    rt.anchoredPosition = new Vector2(startX + c * (cellSize + spacing), startY - r * (cellSize + spacing));

                                    Image img = cellObj.GetComponent<Image>();
                                    if (miniBlockTileSprite != null) img.sprite = miniBlockTileSprite;
                                    img.type = Image.Type.Sliced;
                                    img.color = cellColor;

                                    if (info.isSpecial)
                                    {
                                        GameObject starObj = new GameObject("Star", typeof(RectTransform), typeof(TextMeshProUGUI));
                                        starObj.transform.SetParent(cellObj.transform, false);
                                        RectTransform srt = starObj.GetComponent<RectTransform>();
                                        srt.anchorMin = Vector2.zero;
                                        srt.anchorMax = Vector2.one;
                                        srt.sizeDelta = Vector2.zero;
                                        TextMeshProUGUI stmp = starObj.GetComponent<TextMeshProUGUI>();
                                        stmp.text = "✦";
                                        stmp.fontSize = (cellSize >= 18f) ? 11 : 9;
                                        stmp.alignment = TextAlignmentOptions.Center;
                                        stmp.color = unlocked ? Color.white : new Color(0.38f, 0.38f, 0.44f, 0.80f);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // 5. Update Overview Text
            UpdateEvolutionOverviewText(mascotIdx, activeTier);
        }

        public void OnClickEvolutionStage(int tier)
        {
            PlayClickSound();
            _selectedEvolutionPreviewTier = Mathf.Clamp(tier, 0, 2);
            UpdateEvolutionOverviewText(_selectedDetailMascotIdx, _selectedEvolutionPreviewTier);
        }

        private void UpdateEvolutionOverviewText(int mascotIdx, int selectedTier)
        {
            if (detailEvolutionOverview == null) return;

            int stars = GetBreakthroughStars(mascotIdx);
            int activeTier = (stars >= 5) ? 2 : (stars >= 2) ? 1 : 0;
            var viewInfo = GetMascotUniqueBlockInfo(mascotIdx, selectedTier);

            bool isViewActive = (selectedTier == activeTier);
            bool isViewUnlocked = (selectedTier == 0) || (selectedTier == 1 && stars >= 2) || (selectedTier == 2 && stars >= 5);

            string headerTag = isViewActive ? "<color=#00B894><b>[● 현재 적용 능력]</b></color>" :
                               isViewUnlocked ? "<color=#0984E3><b>[✓ 해금된 능력]</b></color>" :
                               (selectedTier == 1) ? "<color=#E67E22><b>[🔒 ★2돌파 시 해금 미리보기]</b></color>" :
                                                     "<color=#D63031><b>[🔒 ★5돌파 MAX 시 해금 미리보기]</b></color>";

            detailEvolutionOverview.text = $"{headerTag} <b>{viewInfo.blockName}</b>: {viewInfo.abilityDesc}";
        }

        public void RefreshMascotModalUI()
        {
            int selectedMascot = PlayerPrefs.GetInt(KEY_SELECTED_MASCOT, 0);

            if (mascotActionButtons != null)
            {
                for (int i = 0; i < mascotActionButtons.Length; i++)
                {
                    if (mascotActionButtons[i] == null) continue;
                    bool isOwned = (i == 0) || (PlayerPrefs.GetInt(KEY_MASCOT_OWNED_PREFIX + i, 0) == 1);
                    bool isSelected = (selectedMascot == i);

                    TMP_Text txt = (mascotActionTexts != null && i < mascotActionTexts.Length && mascotActionTexts[i] != null)
                        ? mascotActionTexts[i]
                        : mascotActionButtons[i].GetComponentInChildren<TMP_Text>();

                    Image btnImg = mascotActionButtons[i].GetComponent<Image>();

                    if (isSelected)
                    {
                        if (txt != null)
                        {
                            txt.text = LocalizationManager.Get("codex_equipped");
                            txt.color = new Color(0.06f, 0.35f, 0.26f, 1f);
                        }
                        if (btnImg != null && shopEquippedBtnSprite != null)
                        {
                            btnImg.sprite = shopEquippedBtnSprite;
                            btnImg.color = Color.white;
                        }
                        mascotActionButtons[i].interactable = false;
                    }
                    else if (isOwned)
                    {
                        if (txt != null)
                        {
                            txt.text = LocalizationManager.Get("mascot_btn_equip");
                            txt.color = new Color(0.46f, 0.08f, 0.24f, 1f);
                        }
                        if (btnImg != null && shopEquipBtnSprite != null)
                        {
                            btnImg.sprite = shopEquipBtnSprite;
                            btnImg.color = Color.white;
                        }
                        mascotActionButtons[i].interactable = true;
                    }
                    else
                    {
                        if (txt != null)
                        {
                            txt.text = (i >= 4) ? LocalizationManager.Get("mascot_obtain_pickup") : LocalizationManager.Get("codex_locked");
                            txt.color = new Color(0.45f, 0.45f, 0.50f, 1f);
                        }
                        if (btnImg != null)
                        {
                            btnImg.color = new Color(0.85f, 0.85f, 0.88f, 1f);
                        }
                        mascotActionButtons[i].interactable = false;
                    }
                }
            }

            if (mascotStatusTexts != null)
            {
                for (int i = 0; i < mascotStatusTexts.Length; i++)
                {
                    if (mascotStatusTexts[i] == null) continue;
                    bool isOwned = (i == 0) || (PlayerPrefs.GetInt(KEY_MASCOT_OWNED_PREFIX + i, 0) == 1);
                    bool isSelected = (selectedMascot == i);
                    if (isSelected)
                    {
                        mascotStatusTexts[i].text = $"<color=#2ECC71>● {LocalizationManager.Get("codex_equipped")}</color>";
                    }
                    else if (isOwned)
                    {
                        mascotStatusTexts[i].text = "<color=#3498DB>보유 중</color>";
                    }
                    else
                    {
                        mascotStatusTexts[i].text = (i >= 4) ? "<color=#9B59B6>픽업 소환 전용</color>" : "<color=#95A5A6>상점에서 획득</color>";
                    }
                }
            }

            if (mascotLevelTexts != null)
            {
                for (int i = 0; i < mascotLevelTexts.Length; i++)
                {
                    if (mascotLevelTexts[i] == null) continue;
                    int lvl = GetMascotLevel(i);
                    mascotLevelTexts[i].text = $"Lv.{lvl}";
                }
            }

            if (mascotShardTexts != null)
            {
                for (int i = 0; i < mascotShardTexts.Length; i++)
                {
                    if (mascotShardTexts[i] == null) continue;
                    int shards = GetMascotShards(i);
                    int cost = GetBreakthroughCost(i);
                    mascotShardTexts[i].text = (cost > 0)
                        ? $"말랑 조각: <color=#3498DB>{shards}</color> / {cost}"
                        : $"말랑 조각: {shards} (최대 돌파)";
                }
            }

            if (mascotAbilityTexts != null)
            {
                for (int i = 0; i < mascotAbilityTexts.Length; i++)
                {
                    if (mascotAbilityTexts[i] == null) continue;
                    mascotAbilityTexts[i].text = GetLocalizedMascotDesc(i);
                }
            }

            if (mascotUpgradeButtons != null)
            {
                for (int i = 0; i < mascotUpgradeButtons.Length; i++)
                {
                    if (mascotUpgradeButtons[i] == null) continue;
                    int shards = GetMascotShards(i);
                    int cost = GetBreakthroughCost(i);
                    bool canUpgrade = (cost > 0) && (shards >= cost);
                    mascotUpgradeButtons[i].interactable = canUpgrade;

                    if (mascotUpgradeTexts != null && i < mascotUpgradeTexts.Length && mascotUpgradeTexts[i] != null)
                    {
                        mascotUpgradeTexts[i].text = canUpgrade ? LocalizationManager.Get("mascot_btn_breakthrough") : $"돌파 ({cost})";
                        mascotUpgradeTexts[i].color = canUpgrade ? new Color(0.06f, 0.35f, 0.26f, 1f) : new Color(0.45f, 0.45f, 0.50f, 1f);
                    }
                }
            }
        }

        // ==========================================
        // MASCOT SHOP (TAB 2)
        // ==========================================

        public void BuyOrEquipMascot(int mascotIdx)
        {
            if (mascotIdx < 0 || mascotIdx >= 9) return;
            bool isOwned = (mascotIdx == 0) || (PlayerPrefs.GetInt(KEY_MASCOT_OWNED_PREFIX + mascotIdx, 0) == 1);

            if (isOwned)
            {
                // Shop is strictly purchase-only. Owned mascots cannot be bought again or equipped here.
                PlayClickSound();
                return;
            }

            int price = MascotPricesCoins[mascotIdx];
            if (_currentCoins >= price)
            {
                _currentCoins -= price;
                PlayerPrefs.SetInt(KEY_COINS, _currentCoins);
                PlayerPrefs.SetInt(KEY_MASCOT_OWNED_PREFIX + mascotIdx, 1);
                PlayerPrefs.Save();

                RefreshCurrenciesUI();
                RefreshMascotShopUI();
                RefreshMascotModalUI();
                RefreshMascotCodexUI();

                if (FairyScreenTransition.Instance != null) FairyScreenTransition.Instance.EmitCornerSparkles();
                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayBuy();
            }
            else
            {
                PlayClickSound();
                if (shopCoinsText != null) StartCoroutine(FlashCoinsTextRed());
            }
        }

        public void RefreshMascotShopUI()
        {
            RefreshCurrenciesUI();

            if (mascotShopActionButtons != null)
            {
                for (int i = 0; i < mascotShopActionButtons.Length; i++)
                {
                    if (mascotShopActionButtons[i] == null) continue;
                    bool isOwned = (i == 0) || (PlayerPrefs.GetInt(KEY_MASCOT_OWNED_PREFIX + i, 0) == 1);

                    TMP_Text txt = (mascotShopActionTexts != null && i < mascotShopActionTexts.Length && mascotShopActionTexts[i] != null)
                        ? mascotShopActionTexts[i]
                        : mascotShopActionButtons[i].GetComponentInChildren<TMP_Text>();

                    Image btnImg = mascotShopActionButtons[i].GetComponent<Image>();

                    if (txt != null)
                    {
                        txt.enableAutoSizing = true;
                        txt.fontSizeMin = 13f;
                        txt.margin = new Vector4(10f, 0f, 10f, 0f);
                    }

                    if (isOwned)
                    {
                        if (txt != null)
                        {
                            txt.text = LocalizationManager.Get("shop_btn_owned");
                            txt.color = new Color(0.38f, 0.30f, 0.50f, 1f);
                            txt.fontStyle = FontStyles.Bold;
                        }
                        if (btnImg != null && shopCreamBtnSprite != null)
                        {
                            btnImg.sprite = shopCreamBtnSprite;
                            btnImg.color = new Color(0.92f, 0.90f, 0.95f, 0.85f);
                        }
                        mascotShopActionButtons[i].interactable = false;
                    }
                    else
                    {
                        int price = MascotPricesCoins[i];
                        bool canAfford = (_currentCoins >= price);
                        if (txt != null)
                        {
                            txt.text = $"{price:N0} G " + LocalizationManager.Get("shop_btn_buy");
                            txt.color = canAfford ? new Color(0.24f, 0.12f, 0.00f, 1f) : new Color(0.42f, 0.32f, 0.52f, 1f);
                            txt.fontStyle = FontStyles.Bold;
                            txt.enableAutoSizing = true;
                            txt.fontSizeMin = 16f;
                        }
                        if (btnImg != null)
                        {
                            if (canAfford)
                            {
                                btnImg.sprite = (shopGoldBtnSprite != null) ? shopGoldBtnSprite : shopEquipBtnSprite;
                            }
                            else
                            {
                                btnImg.sprite = (shopCreamBtnSprite != null) ? shopCreamBtnSprite : shopEquipBtnSprite;
                            }
                            btnImg.color = Color.white;
                        }
                        mascotShopActionButtons[i].interactable = true;
                    }
                }
            }
        }

        // ==========================================
        // RECOMMENDED PACKAGES (TAB 0) & PICKUP SUMMON (TAB 1)
        // ==========================================

        private void ShowShopToast(string message)
        {
            if (AdManager.Instance != null)
            {
                AdManager.Instance.ShowRewardPopup("안내", message, null, "");
            }
        }

        public void BuyPackage(int packIdx)
        {
            Transform dCard = shopModal != null ? shopModal.transform.Find("DialogCard") : null;
            Sprite coinSpr = dCard?.Find("CoinBadge/Icon")?.GetComponent<Image>()?.sprite;
            Sprite diaSpr = dCard?.Find("DiaBadge/Icon")?.GetComponent<Image>()?.sprite;
            Sprite mintSpr = (mascotAvatars != null && mascotAvatars.Length > 1) ? mascotAvatars[1] : null;

            if (packIdx == 0)
            {
                // 1. 일일 골드 (Daily Gold: Free once per day, +100 G)
                string todayDate = DateTime.UtcNow.ToString("yyyyMMdd");
                if (PlayerPrefs.GetString("Mallang_DailyGold_ClaimDate", "") == todayDate)
                {
                    PlayClickSound();
                    ShowShopToast(LocalizationManager.Get("shop_daily_gold_claimed_toast"));
                    return;
                }

                PlayerPrefs.SetString("Mallang_DailyGold_ClaimDate", todayDate);
                PlayerPrefs.Save();

                AddCoins(100);
                if (AdManager.Instance != null)
                {
                    AdManager.Instance.ShowRewardPopup(
                        LocalizationManager.Get("reward_claim_title"),
                        "+100 G",
                        coinSpr,
                        "일일 골드 100 G가 지급되었습니다!"
                    );
                }
                if (FairyScreenTransition.Instance != null) FairyScreenTransition.Instance.EmitCornerSparkles();
                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayBuy();
                RefreshShopPackagesUI();
            }
            else if (packIdx == 1)
            {
                // 2. 일일 다이아 (Daily Diamonds: Watch Ad -> +30 Diamonds, once per day)
                string todayDate = DateTime.UtcNow.ToString("yyyyMMdd");
                if (PlayerPrefs.GetString("Mallang_DailyDiamond_ClaimDate", "") == todayDate)
                {
                    PlayClickSound();
                    ShowShopToast(LocalizationManager.Get("shop_daily_diamond_claimed_toast"));
                    return;
                }

                PlayClickSound();
                if (AdManager.Instance != null)
                {
                    AdManager.Instance.ShowRewardedAd(
                        onRewardEarned: () =>
                        {
                            PlayerPrefs.SetString("Mallang_DailyDiamond_ClaimDate", todayDate);
                            PlayerPrefs.Save();
                            AddDiamonds(30);
                            if (FairyScreenTransition.Instance != null) FairyScreenTransition.Instance.EmitCornerSparkles();
                            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayBuy();
                            RefreshShopPackagesUI();
                        },
                        onAdFailed: (err) =>
                        {
                            Debug.LogWarning($"[LobbyManager] Ad failed: {err}");
                        }
                    );
                }
                else
                {
                    PlayerPrefs.SetString("Mallang_DailyDiamond_ClaimDate", todayDate);
                    PlayerPrefs.Save();
                    AddDiamonds(30);
                    if (FairyScreenTransition.Instance != null) FairyScreenTransition.Instance.EmitCornerSparkles();
                    if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayBuy();
                    RefreshShopPackagesUI();
                }
            }
            else if (packIdx == 2)
            {
                // 3. 웰컴 팩 (Welcome Pack: Mint Mascot + 1,000 G + 100 Diamonds, 1-time free per account)
                if (PlayerPrefs.GetInt("Mallang_WelcomePack_Claimed", 0) == 1)
                {
                    PlayClickSound();
                    ShowShopToast(LocalizationManager.Get("shop_welcome_pack_claimed_toast"));
                    return;
                }

                PlayerPrefs.SetInt("Mallang_WelcomePack_Claimed", 1);
                PlayerPrefs.SetInt(KEY_MASCOT_OWNED_PREFIX + 1, 1);
                AddCoins(1000);
                AddDiamonds(100);
                PlayerPrefs.Save();

                RefreshMascotShopUI();
                RefreshMascotModalUI();
                RefreshShopPackagesUI();

                if (AdManager.Instance != null)
                {
                    AdManager.Instance.ShowRewardPopup(
                        "🎉 웰컴 팩 수령 완료!",
                        "민트 말랑이 해금!\n+1,000 G   +100 다이아",
                        mintSpr,
                        "말랑 블라스트에 오신 것을 환영합니다!"
                    );
                }

                if (FairyScreenTransition.Instance != null) FairyScreenTransition.Instance.EmitCornerSparkles();
                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayBuy();
            }
        }

        public void RefreshShopPackagesUI()
        {
            RefreshCurrenciesUI();
            PolishShopUIElements();

            if (shopModal == null) return;
            Transform recPanel = shopModal.transform.Find("DialogCard/ContentContainer/RecommendedPanel");

            // Enforce Card Icons Dynamically (Card 0: Gold Coin, Card 1: Diamond Gem, Card 2: Mint Mascot)
            if (recPanel != null)
            {
                Transform dCard = shopModal.transform.Find("DialogCard");
                Sprite coinSpr = dCard?.Find("CoinBadge/Icon")?.GetComponent<Image>()?.sprite;
                Sprite diaSpr = dCard?.Find("DiaBadge/Icon")?.GetComponent<Image>()?.sprite;
                Sprite mintSpr = (mascotAvatars != null && mascotAvatars.Length > 1) ? mascotAvatars[1] : null;

                var c0Icon = recPanel.Find("PackageCard_0/IconFrame/Icon")?.GetComponent<Image>();
                if (c0Icon != null && coinSpr != null) c0Icon.sprite = coinSpr;

                var c1Icon = recPanel.Find("PackageCard_1/IconFrame/Icon")?.GetComponent<Image>();
                if (c1Icon != null && diaSpr != null) c1Icon.sprite = diaSpr;

                var c2Icon = recPanel.Find("PackageCard_2/IconFrame/Icon")?.GetComponent<Image>();
                if (c2Icon != null && mintSpr != null) c2Icon.sprite = mintSpr;
            }

            if (shopPackageButtons != null)
            {
                string todayDate = DateTime.UtcNow.ToString("yyyyMMdd");
                bool goldClaimed = PlayerPrefs.GetString("Mallang_DailyGold_ClaimDate", "") == todayDate;
                bool diamondClaimed = PlayerPrefs.GetString("Mallang_DailyDiamond_ClaimDate", "") == todayDate;
                bool welcomeClaimed = PlayerPrefs.GetInt("Mallang_WelcomePack_Claimed", 0) == 1;

                if (shopPackageButtons.Length > 0 && shopPackageButtons[0] != null)
                {
                    var txt = shopPackageButtons[0].GetComponentInChildren<TMP_Text>();
                    if (txt != null) txt.text = goldClaimed ? LocalizationManager.Get("shop_pack_claimed") : LocalizationManager.Get("shop_pack_0_price");
                    shopPackageButtons[0].interactable = !goldClaimed;
                    StylePackButton(shopPackageButtons[0], goldClaimed);
                }
                if (shopPackageButtons.Length > 1 && shopPackageButtons[1] != null)
                {
                    var txt = shopPackageButtons[1].GetComponentInChildren<TMP_Text>();
                    if (txt != null) txt.text = diamondClaimed ? LocalizationManager.Get("shop_pack_claimed") : LocalizationManager.Get("shop_pack_1_price");
                    shopPackageButtons[1].interactable = !diamondClaimed;
                    StylePackButton(shopPackageButtons[1], diamondClaimed);
                }
                if (shopPackageButtons.Length > 2 && shopPackageButtons[2] != null)
                {
                    var txt = shopPackageButtons[2].GetComponentInChildren<TMP_Text>();
                    if (txt != null) txt.text = welcomeClaimed ? LocalizationManager.Get("shop_pack_claimed") : LocalizationManager.Get("shop_pack_2_price");
                    shopPackageButtons[2].interactable = !welcomeClaimed;
                    StylePackButton(shopPackageButtons[2], welcomeClaimed);
                }
            }
        }

        private void StylePackButton(Button btn, bool isClaimed)
        {
            if (btn == null) return;
            var img = btn.GetComponent<Image>();
            var txt = btn.GetComponentInChildren<TMP_Text>();
            if (isClaimed)
            {
                if (img != null) img.color = new Color(0.72f, 0.68f, 0.78f, 0.75f);
                if (txt != null) txt.color = new Color(0.38f, 0.32f, 0.44f, 1f);
            }
            else
            {
                if (img != null) img.color = Color.white;
                if (txt != null) txt.color = Color.white;
            }
        }

        public void PolishShopUIElements()
        {
            if (shopModal == null) return;
            Transform dCard = shopModal.transform.Find("DialogCard");
            if (dCard == null) return;

            // 1. Diamond Currency Badge (Clean Marshmallow Pill)
            Transform diaBadge = dCard.Find("DiaBadge");
            if (diaBadge != null)
            {
                RectTransform rt = diaBadge.GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.anchoredPosition = new Vector2(-380, -48);
                    rt.sizeDelta = new Vector2(180, 56);
                }
                Image bg = diaBadge.GetComponent<Image>();
                if (bg != null) bg.color = new Color(0.95f, 0.92f, 1f, 0.95f);
                if (shopDiamondsText != null)
                {
                    shopDiamondsText.color = new Color(0.18f, 0.08f, 0.26f, 1f); // Deep crisp purple
                    shopDiamondsText.fontStyle = FontStyles.Bold;
                    shopDiamondsText.fontSize = 24;
                }
                Transform icon = diaBadge.Find("Icon");
                if (icon != null)
                {
                    RectTransform irt = icon.GetComponent<RectTransform>();
                    if (irt != null)
                    {
                        irt.anchoredPosition = new Vector2(26, 0);
                        irt.sizeDelta = new Vector2(38, 38);
                    }
                }
            }

            // 2. Gold Currency Badge (Clean Marshmallow Pill)
            Transform coinBadge = dCard.Find("CoinBadge");
            if (coinBadge != null)
            {
                RectTransform rt = coinBadge.GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.anchoredPosition = new Vector2(-180, -48);
                    rt.sizeDelta = new Vector2(180, 56);
                }
                Image bg = coinBadge.GetComponent<Image>();
                if (bg != null) bg.color = new Color(0.95f, 0.92f, 1f, 0.95f);
                if (shopCoinsText != null)
                {
                    shopCoinsText.color = new Color(0.18f, 0.08f, 0.26f, 1f); // Deep crisp purple
                    shopCoinsText.fontStyle = FontStyles.Bold;
                    shopCoinsText.fontSize = 24;
                }
                Transform icon = coinBadge.Find("Icon");
                if (icon != null)
                {
                    RectTransform irt = icon.GetComponent<RectTransform>();
                    if (irt != null)
                    {
                        irt.anchoredPosition = new Vector2(26, 0);
                        irt.sizeDelta = new Vector2(38, 38);
                    }
                }
            }

            // 3. Tab contrast polish
            Transform tabTrack = dCard.Find("TabHeaderBar");
            if (tabTrack != null)
            {
                for (int t = 0; t < 5; t++)
                {
                    var tabText = tabTrack.Find($"Tab_{t}")?.GetComponentInChildren<TMP_Text>();
                    if (tabText != null && _currentShopTab != t)
                    {
                        tabText.color = new Color(0.42f, 0.30f, 0.58f, 1f); // Rich legible violet
                        tabText.fontStyle = FontStyles.Bold;
                    }
                }
            }

            // 4. Recommended Panel section layout & text polish
            Transform recPanel = dCard.Find("ContentContainer/RecommendedPanel");
            if (recPanel != null)
            {
                // Top Ribbon for Recommended Panel
                Transform recRibbon = recPanel.Find("TopRibbon");
                if (recRibbon == null)
                {
                    GameObject ribbonObj = new GameObject("TopRibbon", typeof(RectTransform), typeof(Image));
                    ribbonObj.transform.SetParent(recPanel, false);
                    ribbonObj.transform.SetAsFirstSibling();
                    recRibbon = ribbonObj.transform;

                    RectTransform rrt = ribbonObj.GetComponent<RectTransform>();
                    rrt.anchorMin = new Vector2(0.5f, 1f);
                    rrt.anchorMax = new Vector2(0.5f, 1f);
                    rrt.pivot = new Vector2(0.5f, 0.5f);
                    rrt.anchoredPosition = new Vector2(0f, -42f);
                    rrt.sizeDelta = new Vector2(880f, 60f);

                    Image rImg = ribbonObj.GetComponent<Image>();
                    Image cardImg = recPanel.Find("RecTipCard")?.GetComponent<Image>();
                    if (cardImg != null && cardImg.sprite != null) rImg.sprite = cardImg.sprite;
                    rImg.type = Image.Type.Sliced;
                    rImg.color = new Color(0.18f, 0.11f, 0.32f, 0.95f);

                    GameObject txtObj = new GameObject("RibbonTxt", typeof(RectTransform), typeof(TextMeshProUGUI));
                    txtObj.transform.SetParent(ribbonObj.transform, false);
                    RectTransform trt = txtObj.GetComponent<RectTransform>();
                    trt.anchorMin = Vector2.zero;
                    trt.anchorMax = Vector2.one;
                    trt.offsetMin = Vector2.zero;
                    trt.offsetMax = Vector2.zero;

                    TMP_Text rt = txtObj.GetComponent<TMP_Text>();
                    rt.text = LocalizationManager.Get("rec_top_ribbon", "★ [데일리 혜택] 매일 무료 보상 & 스페셜 스타터 팩! ★");
                    rt.alignment = TextAlignmentOptions.Center;
                    rt.fontSize = 22;
                    rt.fontStyle = FontStyles.Bold;
                    rt.color = new Color(1f, 0.92f, 0.45f);
                }
                else
                {
                    RectTransform rrt = recRibbon.GetComponent<RectTransform>();
                    if (rrt != null)
                    {
                        rrt.anchoredPosition = new Vector2(0f, -42f);
                        rrt.sizeDelta = new Vector2(880f, 60f);
                    }
                    var rt = recRibbon.Find("RibbonTxt")?.GetComponent<TMP_Text>();
                    if (rt != null) rt.text = LocalizationManager.Get("rec_top_ribbon", "★ [데일리 혜택] 매일 무료 보상 & 스페셜 스타터 팩! ★");
                }

                // New Mascot Banner (Shifted down towards center)
                Transform newBanner = recPanel.Find("NewMascotBanner");
                if (newBanner != null)
                {
                    RectTransform nbrt = newBanner.GetComponent<RectTransform>();
                    if (nbrt != null)
                    {
                        nbrt.anchoredPosition = new Vector2(0f, -295f);
                        nbrt.sizeDelta = new Vector2(880f, 420f);
                    }
                }

                // Subtitle
                Transform packSub = recPanel.Find("PackSubtitle");
                if (packSub != null)
                {
                    RectTransform psrt = packSub.GetComponent<RectTransform>();
                    if (psrt != null)
                    {
                        psrt.anchoredPosition = new Vector2(0f, -545f);
                        psrt.sizeDelta = new Vector2(860f, 36f);
                    }
                    var subTxt = packSub.GetComponent<TMP_Text>();
                    if (subTxt != null)
                    {
                        subTxt.color = new Color(0.24f, 0.12f, 0.38f);
                        subTxt.fontStyle = FontStyles.Bold;
                    }
                }

                // Cards typography and position polish
                float[] packXOffsets = new float[] { -295f, 0f, 295f };
                for (int p = 0; p < 3; p++)
                {
                    var pCard = recPanel.Find($"PackageCard_{p}");
                    if (pCard != null)
                    {
                        RectTransform pcrt = pCard.GetComponent<RectTransform>();
                        if (pcrt != null)
                        {
                            pcrt.anchoredPosition = new Vector2(packXOffsets[p], -780f);
                            pcrt.sizeDelta = new Vector2(275f, 420f);
                        }
                        var title = pCard.Find("Title")?.GetComponent<TMP_Text>();
                        if (title != null)
                        {
                            title.color = new Color(0.22f, 0.10f, 0.32f);
                            title.fontStyle = FontStyles.Bold;
                            title.fontSize = 26;
                        }
                        var rewards = pCard.Find("Rewards")?.GetComponent<TMP_Text>();
                        if (rewards != null)
                        {
                            rewards.color = new Color(0.68f, 0.28f, 0.05f); // Rich caramel
                            rewards.fontStyle = FontStyles.Bold;
                            rewards.fontSize = 22;
                        }
                    }
                }

                var tipCard = recPanel.Find("RecTipCard");
                if (tipCard != null)
                {
                    RectTransform tcrt = tipCard.GetComponent<RectTransform>();
                    if (tcrt != null)
                    {
                        tcrt.anchoredPosition = new Vector2(0f, -1045f);
                        tcrt.sizeDelta = new Vector2(880f, 95f);
                    }
                    var tipTxt = tipCard.Find("TipTxt")?.GetComponent<TMP_Text>();
                    if (tipTxt != null)
                    {
                        tipTxt.color = new Color(0.25f, 0.15f, 0.38f);
                        tipTxt.fontStyle = FontStyles.Normal;
                    }
                }
            }

            // 5. Pickup Panel Polish & Pity UI
            EnsurePickupPityUI(dCard);
        }

        public void EnsurePickupPityUI(Transform dCard = null)
        {
            if (dCard == null && shopModal != null)
            {
                dCard = shopModal.transform.Find("DialogCard");
            }
            if (dCard == null) return;

            Transform pickPanel = dCard.Find("ContentContainer/PickupPanel");
            if (pickPanel == null) return;

            // 1. Top Event Header Ribbon
            Transform topRibbon = pickPanel.Find("TopRibbon");
            Image refCardImg = pickPanel.Find("PickHintCard")?.GetComponent<Image>();
            if (topRibbon == null)
            {
                GameObject ribbonObj = new GameObject("TopRibbon", typeof(RectTransform), typeof(Image));
                ribbonObj.transform.SetParent(pickPanel, false);
                ribbonObj.transform.SetAsFirstSibling();
                topRibbon = ribbonObj.transform;

                RectTransform rrt = ribbonObj.GetComponent<RectTransform>();
                rrt.anchorMin = new Vector2(0.5f, 1f);
                rrt.anchorMax = new Vector2(0.5f, 1f);
                rrt.pivot = new Vector2(0.5f, 0.5f);
                rrt.anchoredPosition = new Vector2(0f, -42f);
                rrt.sizeDelta = new Vector2(880f, 60f);

                Image rImg = ribbonObj.GetComponent<Image>();
                if (refCardImg != null && refCardImg.sprite != null) rImg.sprite = refCardImg.sprite;
                rImg.type = Image.Type.Sliced;
                rImg.color = new Color(0.18f, 0.11f, 0.32f, 0.95f);

                GameObject txtObj = new GameObject("RibbonTxt", typeof(RectTransform), typeof(TextMeshProUGUI));
                txtObj.transform.SetParent(ribbonObj.transform, false);
                RectTransform trt = txtObj.GetComponent<RectTransform>();
                trt.anchorMin = Vector2.zero;
                trt.anchorMax = Vector2.one;
                trt.offsetMin = Vector2.zero;
                trt.offsetMax = Vector2.zero;

                TMP_Text rt = txtObj.GetComponent<TMP_Text>();
                rt.text = LocalizationManager.Get("pickup_top_ribbon", "★ [시즌 1] 천상의 천사 말랑이 스페셜 픽업 소환 ★");
                rt.alignment = TextAlignmentOptions.Center;
                rt.fontSize = 22;
                rt.fontStyle = FontStyles.Bold;
                rt.color = new Color(1f, 0.92f, 0.45f);
            }
            else
            {
                RectTransform rrt = topRibbon.GetComponent<RectTransform>();
                if (rrt != null)
                {
                    rrt.anchoredPosition = new Vector2(0f, -42f);
                    rrt.sizeDelta = new Vector2(880f, 60f);
                }
                var rt = topRibbon.Find("RibbonTxt")?.GetComponent<TMP_Text>();
                if (rt != null) rt.text = LocalizationManager.Get("pickup_top_ribbon", "★ [시즌 1] 천상의 천사 말랑이 스페셜 픽업 소환 ★");
            }

            // 2. Pickup Banner (Shifted down towards center)
            Transform banner = pickPanel.Find("PickupBanner");
            if (banner != null)
            {
                RectTransform brt = banner.GetComponent<RectTransform>();
                if (brt != null)
                {
                    brt.anchoredPosition = new Vector2(0f, -295f);
                    brt.sizeDelta = new Vector2(880f, 415f);
                }
            }

            // 3. Pity Gauge Card (Between Banner and Buttons)
            Transform pityCard = pickPanel.Find("PityGaugeCard");
            Sprite coinSpr = dCard.Find("CoinBadge/Icon")?.GetComponent<Image>()?.sprite;
            Sprite specialSpr = (mascotAvatars != null && mascotAvatars.Length > 8) ? mascotAvatars[8] : null;

            if (pityCard == null)
            {
                GameObject pCardObj = new GameObject("PityGaugeCard", typeof(RectTransform), typeof(Image));
                pCardObj.transform.SetParent(pickPanel, false);
                if (banner != null) pCardObj.transform.SetSiblingIndex(banner.GetSiblingIndex() + 1);
                pityCard = pCardObj.transform;

                RectTransform prt = pCardObj.GetComponent<RectTransform>();
                prt.anchorMin = new Vector2(0.5f, 1f);
                prt.anchorMax = new Vector2(0.5f, 1f);
                prt.pivot = new Vector2(0.5f, 0.5f);
                prt.anchoredPosition = new Vector2(0f, -565f);
                prt.sizeDelta = new Vector2(880f, 125f);

                Image pImg = pCardObj.GetComponent<Image>();
                if (refCardImg != null && refCardImg.sprite != null) pImg.sprite = refCardImg.sprite;
                pImg.type = Image.Type.Sliced;
                pImg.color = new Color(0.14f, 0.09f, 0.25f, 0.96f);

                // Header Row: Title & Counter
                GameObject titleObj = new GameObject("TitleTxt", typeof(RectTransform), typeof(TextMeshProUGUI));
                titleObj.transform.SetParent(pCardObj.transform, false);
                RectTransform titRt = titleObj.GetComponent<RectTransform>();
                titRt.anchorMin = new Vector2(0f, 0.5f);
                titRt.anchorMax = new Vector2(0.55f, 0.5f);
                titRt.pivot = new Vector2(0f, 0.5f);
                titRt.anchoredPosition = new Vector2(25f, 34f);
                titRt.sizeDelta = new Vector2(400f, 32f);
                TMP_Text titTxt = titleObj.GetComponent<TMP_Text>();
                titTxt.text = LocalizationManager.Get("pickup_pity_title", "★ 60회 확정 소환 천장 게이지 ★");
                titTxt.fontSize = 22;
                titTxt.fontStyle = FontStyles.Bold;
                titTxt.color = new Color(1f, 0.90f, 0.55f);

                GameObject cntObj = new GameObject("CounterTxt", typeof(RectTransform), typeof(TextMeshProUGUI));
                cntObj.transform.SetParent(pCardObj.transform, false);
                RectTransform cntRt = cntObj.GetComponent<RectTransform>();
                cntRt.anchorMin = new Vector2(0.45f, 0.5f);
                cntRt.anchorMax = new Vector2(1f, 0.5f);
                cntRt.pivot = new Vector2(1f, 0.5f);
                cntRt.anchoredPosition = new Vector2(-25f, 34f);
                cntRt.sizeDelta = new Vector2(400f, 32f);
                TMP_Text cntTxt = cntObj.GetComponent<TMP_Text>();
                cntTxt.alignment = TextAlignmentOptions.Right;
                cntTxt.fontSize = 19;
                cntTxt.fontStyle = FontStyles.Bold;
                cntTxt.color = new Color(0.00f, 0.92f, 1f);

                // Rail Background
                GameObject railObj = new GameObject("RailBg", typeof(RectTransform), typeof(Image));
                railObj.transform.SetParent(pCardObj.transform, false);
                RectTransform railRt = railObj.GetComponent<RectTransform>();
                railRt.anchorMin = new Vector2(0.5f, 0.5f);
                railRt.anchorMax = new Vector2(0.5f, 0.5f);
                railRt.pivot = new Vector2(0.5f, 0.5f);
                railRt.anchoredPosition = new Vector2(0f, -6f);
                railRt.sizeDelta = new Vector2(740f, 16f);
                Image railImg = railObj.GetComponent<Image>();
                if (refCardImg != null && refCardImg.sprite != null) railImg.sprite = refCardImg.sprite;
                railImg.type = Image.Type.Sliced;
                railImg.color = new Color(0.06f, 0.03f, 0.12f, 0.95f);

                // Fill Bar
                GameObject fillObj = new GameObject("FillBar", typeof(RectTransform), typeof(Image));
                fillObj.transform.SetParent(railObj.transform, false);
                RectTransform fillRt = fillObj.GetComponent<RectTransform>();
                fillRt.anchorMin = new Vector2(0f, 0f);
                fillRt.anchorMax = new Vector2(0f, 1f);
                fillRt.pivot = new Vector2(0f, 0.5f);
                fillRt.offsetMin = Vector2.zero;
                fillRt.offsetMax = Vector2.zero;
                Image fillImg = fillObj.GetComponent<Image>();
                if (refCardImg != null && refCardImg.sprite != null) fillImg.sprite = refCardImg.sprite;
                fillImg.type = Image.Type.Sliced;
                fillImg.color = new Color(1f, 0.82f, 0.35f, 1f);

                // Nodes Container
                GameObject nodesRoot = new GameObject("NodesRoot", typeof(RectTransform));
                nodesRoot.transform.SetParent(pCardObj.transform, false);
                RectTransform nRootRt = nodesRoot.GetComponent<RectTransform>();
                nRootRt.anchorMin = new Vector2(0.5f, 0.5f);
                nRootRt.anchorMax = new Vector2(0.5f, 0.5f);
                nRootRt.pivot = new Vector2(0.5f, 0.5f);
                nRootRt.anchoredPosition = new Vector2(0f, -6f);
                nRootRt.sizeDelta = new Vector2(740f, 0f);

                // 6 Milestones (10, 20, 30, 40, 50 = Gold coin, 60 = Angel mascot)
                float totalWidth = 740f;
                for (int m = 1; m <= 6; m++)
                {
                    float xPos = -370f + (m * totalWidth / 6f);
                    bool isFinal = (m == 6);
                    float discSize = isFinal ? 46f : 36f;

                    GameObject nodeObj = new GameObject($"Node_{m}", typeof(RectTransform));
                    nodeObj.transform.SetParent(nodesRoot.transform, false);
                    RectTransform nrt = nodeObj.GetComponent<RectTransform>();
                    nrt.anchorMin = new Vector2(0.5f, 0.5f);
                    nrt.anchorMax = new Vector2(0.5f, 0.5f);
                    nrt.pivot = new Vector2(0.5f, 0.5f);
                    nrt.anchoredPosition = new Vector2(xPos, 0f);
                    nrt.sizeDelta = new Vector2(discSize, discSize);

                    // Disc Image
                    GameObject discObj = new GameObject("Disc", typeof(RectTransform), typeof(Image));
                    discObj.transform.SetParent(nodeObj.transform, false);
                    RectTransform drt = discObj.GetComponent<RectTransform>();
                    drt.anchorMin = Vector2.zero;
                    drt.anchorMax = Vector2.one;
                    drt.offsetMin = Vector2.zero;
                    drt.offsetMax = Vector2.zero;
                    Image dImg = discObj.GetComponent<Image>();
                    if (refCardImg != null && refCardImg.sprite != null) dImg.sprite = refCardImg.sprite;
                    dImg.type = Image.Type.Sliced;
                    dImg.color = isFinal ? new Color(0.40f, 0.15f, 0.45f) : new Color(0.22f, 0.14f, 0.35f);

                    // Icon
                    GameObject iconObj = new GameObject("Icon", typeof(RectTransform), typeof(Image));
                    iconObj.transform.SetParent(discObj.transform, false);
                    RectTransform irt = iconObj.GetComponent<RectTransform>();
                    irt.anchorMin = new Vector2(0.5f, 0.5f);
                    irt.anchorMax = new Vector2(0.5f, 0.5f);
                    irt.pivot = new Vector2(0.5f, 0.5f);
                    irt.anchoredPosition = Vector2.zero;
                    irt.sizeDelta = isFinal ? new Vector2(34f, 34f) : new Vector2(26f, 26f);
                    Image iImg = iconObj.GetComponent<Image>();
                    iImg.preserveAspect = true;
                    if (isFinal)
                    {
                        if (specialSpr != null) iImg.sprite = specialSpr;
                    }
                    else
                    {
                        if (coinSpr != null) iImg.sprite = coinSpr;
                    }

                    // Sublabel
                    GameObject lblObj = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
                    lblObj.transform.SetParent(nodeObj.transform, false);
                    RectTransform lrt = lblObj.GetComponent<RectTransform>();
                    lrt.anchorMin = new Vector2(0.5f, 0.5f);
                    lrt.anchorMax = new Vector2(0.5f, 0.5f);
                    lrt.pivot = new Vector2(0.5f, 1f);
                    lrt.anchoredPosition = new Vector2(0f, -28f);
                    lrt.sizeDelta = new Vector2(120f, 36f);
                    TMP_Text lTxt = lblObj.GetComponent<TMP_Text>();
                    lTxt.alignment = TextAlignmentOptions.Top;
                    lTxt.fontSize = isFinal ? 16 : 15;
                    lTxt.fontStyle = FontStyles.Bold;
                    if (isFinal)
                    {
                        lTxt.text = "<color=#FF80AB>★ 60 확정</color>";
                    }
                    else
                    {
                        lTxt.text = $"<size=15>{m * 10}회</size>\n<color=#FFE082><size=13>+5천G</size></color>";
                    }
                }
            }
            else
            {
                RectTransform prt = pityCard.GetComponent<RectTransform>();
                if (prt != null)
                {
                    prt.anchoredPosition = new Vector2(0f, -565f);
                    prt.sizeDelta = new Vector2(880f, 125f);
                }
            }

            // 4. Pickup Info / Control Row
            Transform ctrlRow = pickPanel.Find("PickControlRow");
            if (ctrlRow != null)
            {
                RectTransform crt = ctrlRow.GetComponent<RectTransform>();
                if (crt != null)
                {
                    crt.anchoredPosition = new Vector2(0f, -650f);
                    crt.sizeDelta = new Vector2(880f, 50f);
                }
            }

            // 5. Summon Buttons (1x, 10x) - Moved down towards bottom
            Transform s1 = pickPanel.Find("BtnSummon1");
            if (s1 != null)
            {
                RectTransform s1Rt = s1.GetComponent<RectTransform>();
                if (s1Rt != null)
                {
                    s1Rt.anchoredPosition = new Vector2(-225f, -755f);
                    s1Rt.sizeDelta = new Vector2(415f, 115f);
                }
            }

            Transform s10 = pickPanel.Find("BtnSummon10");
            if (s10 != null)
            {
                RectTransform s10Rt = s10.GetComponent<RectTransform>();
                if (s10Rt != null)
                {
                    s10Rt.anchoredPosition = new Vector2(225f, -755f);
                    s10Rt.sizeDelta = new Vector2(415f, 115f);
                }
            }

            // 6. Hint Card at bottom
            Transform hintCard = pickPanel.Find("PickHintCard");
            if (hintCard != null)
            {
                RectTransform hrt = hintCard.GetComponent<RectTransform>();
                if (hrt != null)
                {
                    hrt.anchoredPosition = new Vector2(0f, -860f);
                    hrt.sizeDelta = new Vector2(880f, 68f);
                }
                var hTxt = hintCard.Find("HintTxt")?.GetComponent<TMP_Text>();
                if (hTxt != null)
                {
                    hTxt.text = LocalizationManager.Get("pickup_pity_hint_bottom", "★ 60회 소환 시 [천상의 천사 말랑이] 100% 확정! 10회 소환마다 5,000 골드 보너스! ★");
                }
            }

            UpdatePickupPityGaugeUI();
        }

        public void UpdatePickupPityGaugeUI()
        {
            if (shopModal == null) return;
            Transform dCard = shopModal.transform.Find("DialogCard");
            if (dCard == null) return;
            Transform pityCard = dCard.Find("ContentContainer/PickupPanel/PityGaugeCard");
            if (pityCard == null) return;

            int pity = PlayerPrefs.GetInt(KEY_PICKUP_PITY, 0);
            pity = Mathf.Clamp(pity, 0, PICKUP_PITY_TARGET);

            int nextTarget = ((pity / 10) + 1) * 10;
            if (nextTarget > 60) nextTarget = 60;
            int pullsToNext = nextTarget - pity;
            if (pity >= 60) pullsToNext = 0;

            var titTxt = pityCard.Find("TitleTxt")?.GetComponent<TMP_Text>();
            if (titTxt != null) titTxt.text = LocalizationManager.Get("pickup_pity_title", "★ 60회 확정 소환 천장 게이지 ★");

            var cntTxt = pityCard.Find("CounterTxt")?.GetComponent<TMP_Text>();
            if (cntTxt != null)
            {
                cntTxt.text = string.Format(LocalizationManager.Get("pickup_pity_progress_fmt", "진행도: {0}/60 (다음 보상까지 {1}회)"), pity, pullsToNext);
            }

            var fillRt = pityCard.Find("RailBg/FillBar")?.GetComponent<RectTransform>();
            if (fillRt != null)
            {
                fillRt.anchorMax = new Vector2(pity / 60.0f, 1f);
            }

            Transform nodesRoot = pityCard.Find("NodesRoot");
            if (nodesRoot != null)
            {
                for (int m = 1; m <= 6; m++)
                {
                    Transform node = nodesRoot.Find($"Node_{m}");
                    if (node == null) continue;
                    int reqPulls = m * 10;
                    bool reached = (pity >= reqPulls);
                    bool isFinal = (m == 6);

                    var discImg = node.Find("Disc")?.GetComponent<Image>();
                    if (discImg != null)
                    {
                        if (isFinal)
                        {
                            discImg.color = reached ? new Color(1f, 0.85f, 0.20f) : new Color(0.40f, 0.15f, 0.45f);
                        }
                        else
                        {
                            discImg.color = reached ? new Color(1f, 0.80f, 0.25f) : new Color(0.22f, 0.14f, 0.35f);
                        }
                    }

                    var lTxt = node.Find("Label")?.GetComponent<TMP_Text>();
                    if (lTxt != null)
                    {
                        lTxt.fontSize = isFinal ? 16 : 15;
                        lTxt.fontStyle = FontStyles.Bold;
                        if (isFinal)
                        {
                            lTxt.text = reached ? "<color=#00E676><b>★ 확정 달성!</b></color>" : "<color=#FF80AB><b>★ 60 확정</b></color>";
                        }
                        else
                        {
                            lTxt.text = reached ? $"<size=15>{reqPulls}회</size>\n<color=#00E676><size=13>✓ 지급완료</size></color>" : $"<size=15>{reqPulls}회</size>\n<color=#FFE082><size=13>+5천G</size></color>";
                        }
                    }
                }
            }
        }

        public void SummonPickup(int count)
        {
            int cost = (count == 10) ? 1000 : 100 * count;
            if (_currentDiamonds >= cost)
            {
                _currentDiamonds -= cost;
                PlayerPrefs.SetInt(KEY_DIAMONDS, _currentDiamonds);

                int specialCount = 0;
                int duplicateSpecialCount = 0;
                int[] shardGains = new int[9]; // 0~3: Common, 4~7: Rare, 8: Special
                List<GachaDropItem> dropsList = new List<GachaDropItem>();

                int currentPity = PlayerPrefs.GetInt(KEY_PICKUP_PITY, 0);
                int bonusGoldEarned = 0;
                bool hitGuaranteedSpecial = false;

                for (int c = 0; c < count; c++)
                {
                    currentPity++;
                    bool isGuaranteed = false;
                    if (currentPity >= PICKUP_PITY_TARGET)
                    {
                        isGuaranteed = true;
                        hitGuaranteedSpecial = true;
                        currentPity = 0; // 60-pull pity reached, guaranteed drop and reset!
                    }

                    // Single-pull milestone gold bonus check (every 10 pulls gives 5,000 Gold)
                    if (count == 1)
                    {
                        if (currentPity > 0 && currentPity % 10 == 0)
                        {
                            bonusGoldEarned += 5000;
                        }
                    }

                    if (isGuaranteed)
                    {
                        // Guaranteed Special Mascot (천상의 천사 말랑이, Mascot 8)
                        bool alreadyOwned = PlayerPrefs.GetInt(KEY_MASCOT_OWNED_PREFIX + 8, 0) == 1;
                        if (alreadyOwned)
                        {
                            AddMascotShards(8, 60);
                            shardGains[8] += 60;
                            duplicateSpecialCount++;
                            dropsList.Add(new GachaDropItem { isSpecial = true, mascotIndex = 8, shardCount = 60, isDuplicateSpecial = true });
                        }
                        else
                        {
                            PlayerPrefs.SetInt(KEY_MASCOT_OWNED_PREFIX + 8, 1);
                            specialCount++;
                            dropsList.Add(new GachaDropItem { isSpecial = true, mascotIndex = 8, shardCount = 0, isDuplicateSpecial = false });
                        }
                    }
                    else
                    {
                        float roll = Random.Range(0f, 100f);
                        if (roll < 1.0f) // 1.0% chance for Special Mascot!
                        {
                            bool alreadyOwned = PlayerPrefs.GetInt(KEY_MASCOT_OWNED_PREFIX + 8, 0) == 1;
                            if (alreadyOwned)
                            {
                                AddMascotShards(8, 60);
                                shardGains[8] += 60;
                                duplicateSpecialCount++;
                                dropsList.Add(new GachaDropItem { isSpecial = true, mascotIndex = 8, shardCount = 60, isDuplicateSpecial = true });
                            }
                            else
                            {
                                PlayerPrefs.SetInt(KEY_MASCOT_OWNED_PREFIX + 8, 1);
                                specialCount++;
                                dropsList.Add(new GachaDropItem { isSpecial = true, mascotIndex = 8, shardCount = 0, isDuplicateSpecial = false });
                            }
                        }
                        else
                        {
                            int shardAmount = (Random.value < 0.5f) ? 1 : 5;
                            int chosenIdx = Random.Range(0, 8); // 0..7
                            AddMascotShards(chosenIdx, shardAmount);
                            shardGains[chosenIdx] += shardAmount;

                            dropsList.Add(new GachaDropItem { isSpecial = false, mascotIndex = chosenIdx, shardCount = shardAmount, isDuplicateSpecial = false });
                        }
                    }
                }

                // 10-pull gives 5,000 Gold bonus!
                if (count == 10)
                {
                    bonusGoldEarned += 5000;
                }

                if (bonusGoldEarned > 0)
                {
                    _currentCoins += bonusGoldEarned;
                    PlayerPrefs.SetInt(KEY_COINS, _currentCoins);
                }

                PlayerPrefs.SetInt(KEY_PICKUP_PITY, currentPity);
                PlayerPrefs.Save();

                RefreshCurrenciesUI();
                RefreshMascotShopUI();
                RefreshMascotModalUI();
                RefreshMascotCodexUI();
                UpdatePickupPityGaugeUI();

                if (FairyScreenTransition.Instance != null) FairyScreenTransition.Instance.EmitCornerSparkles();
                if (BlockAudioManager.Instance != null)
                {
                    BlockAudioManager.Instance.PlayBuy();
                }

                if (GachaPresentationController.Instance != null)
                {
                    GachaPresentationController.Instance.StartGachaSequence(dropsList, () =>
                    {
                        ShowSummonResultModal(count, specialCount, duplicateSpecialCount, shardGains, bonusGoldEarned, hitGuaranteedSpecial);
                    });
                }
                else
                {
                    ShowSummonResultModal(count, specialCount, duplicateSpecialCount, shardGains, bonusGoldEarned, hitGuaranteedSpecial);
                }
            }
            else
            {
                PlayClickSound();
                if (shopDiamondsText != null) StartCoroutine(FlashDiamondsTextRed());
            }
        }

        public void SetupSummonResultModal(GameObject modal, Button closeBtn, TMP_Text titleTxt, TMP_Text hlTxt, TMP_Text shardsTxt, Image iconImg)
        {
            summonResultModal = modal;
            btnCloseSummonResult = closeBtn;
            summonResultTitleText = titleTxt;
            summonResultHighlightText = hlTxt;
            summonResultShardsText = shardsTxt;
            summonResultMascotIcon = iconImg;

            if (btnCloseSummonResult != null)
            {
                btnCloseSummonResult.onClick.RemoveAllListeners();
                btnCloseSummonResult.onClick.AddListener(CloseSummonResultModal);
            }
        }

        public void CloseSummonResultModal()
        {
            PlayClickSound();
            if (summonResultModal != null) summonResultModal.SetActive(false);
        }

        public void ShowSummonResultModal(int count, int specialCount, int duplicateSpecialCount, int[] shardGains, int bonusGold = 0, bool isPityGuaranteed = false)
        {
            if (summonResultModal == null) return;

            if (summonResultTitleText != null)
            {
                summonResultTitleText.text = $"{count}{LocalizationManager.Get("summon_result_count_suffix")}";
            }

            if (summonResultHighlightText != null)
            {
                if (isPityGuaranteed)
                {
                    summonResultHighlightText.text = $"<color=#FFE600>★ [60회 천장 확정!] {GetLocalizedMascotName(8)} 소환 완료! ★</color>\n<size=20><color=#554B64>{LocalizationManager.Get("summon_result_unlocked_desc")}</color></size>";
                }
                else if (specialCount > 0)
                {
                    summonResultHighlightText.text = $"<color=#FFE600>★ [{GetLocalizedMascotName(8)}] {LocalizationManager.Get("summon_result_unlocked")} ★</color>\n<size=20><color=#554B64>{LocalizationManager.Get("summon_result_unlocked_desc")}</color></size>";
                }
                else if (duplicateSpecialCount > 0)
                {
                    summonResultHighlightText.text = $"<color=#FFE600>★ [{GetLocalizedMascotName(8)}] {LocalizationManager.Get("summon_result_duplicate")} ★</color>\n<size=20><color=#554B64>{LocalizationManager.Get("summon_result_duplicate_desc")}</color></size>";
                }
                else
                {
                    summonResultHighlightText.text = $"<color=#845EC2>{LocalizationManager.Get("summon_result_shards_title")}</color>\n<size=20><color=#554B64>{LocalizationManager.Get("summon_result_shards_desc")}</color></size>";
                }
            }

            if (summonResultMascotIcon != null)
            {
                if (specialCount > 0 || duplicateSpecialCount > 0 || (shardGains != null && shardGains.Length > 8 && shardGains[8] > 0))
                {
                    if (mascotAvatars != null && mascotAvatars.Length > 8 && mascotAvatars[8] != null)
                    {
                        summonResultMascotIcon.sprite = mascotAvatars[8];
                    }
                    summonResultMascotIcon.gameObject.SetActive(true);
                }
                else if (shardGains != null)
                {
                    int maxIdx = 0;
                    for (int i = 1; i < shardGains.Length; i++)
                    {
                        if (shardGains[i] > shardGains[maxIdx]) maxIdx = i;
                    }
                    if (mascotAvatars != null && maxIdx < mascotAvatars.Length && mascotAvatars[maxIdx] != null)
                    {
                        summonResultMascotIcon.sprite = mascotAvatars[maxIdx];
                    }
                    summonResultMascotIcon.gameObject.SetActive(true);
                }
            }

            if (summonResultShardsText != null)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                if (bonusGold > 0)
                {
                    sb.AppendLine($"<color=#FFD32A>★ 10회 소환 보너스: +{bonusGold:N0} 골드 획득! ★</color>");
                }
                if (shardGains != null)
                {
                    for (int i = 0; i < shardGains.Length; i++)
                    {
                        if (shardGains[i] > 0)
                        {
                            string colTag = (i == 8) ? "#E056FD" : (i >= 4) ? "#3498DB" : "#FF6B8B";
                            sb.AppendLine($"<color={colTag}>• {GetLocalizedMascotName(i)}: +{shardGains[i]}개 ({LocalizationManager.Get("mascot_owned_shards")}: {GetMascotShards(i)})</color>");
                        }
                    }
                }
                summonResultShardsText.text = sb.ToString().TrimEnd();
            }

            WireModalAutoClose(summonResultModal, CloseSummonResultModal);
            summonResultModal.transform.SetAsLastSibling();
            summonResultModal.SetActive(true);
        }

        public void RefreshShopSummonUI()
        {
            RefreshCurrenciesUI();
        }

        // ==========================================
        // PROBABILITY MODAL (확률형 아이템 정보공개 의무화 대응)
        // ==========================================

        public void OpenProbabilityModal()
        {
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
            if (probabilityModal != null)
            {
                WireModalAutoClose(probabilityModal, CloseProbabilityModal);
                probabilityModal.transform.SetAsLastSibling();
                probabilityModal.SetActive(true);
            }
        }

        public void CloseProbabilityModal()
        {
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
            if (probabilityModal != null)
            {
                probabilityModal.SetActive(false);
            }
        }

        public void SetupProbabilityModal(GameObject pModal, Button closeBtn, Button confirmBtn, Button darkBgBtn = null)
        {
            probabilityModal = pModal;
            btnCloseProbability = closeBtn;
            btnConfirmProbability = confirmBtn;
            btnProbabilityDarkBg = darkBgBtn;

            if (probabilityModal != null) probabilityModal.SetActive(false);

            if (btnCloseProbability != null)
            {
                btnCloseProbability.onClick.RemoveAllListeners();
                btnCloseProbability.onClick.AddListener(CloseProbabilityModal);
            }
            if (btnConfirmProbability != null)
            {
                btnConfirmProbability.onClick.RemoveAllListeners();
                btnConfirmProbability.onClick.AddListener(CloseProbabilityModal);
            }
            if (btnProbabilityDarkBg != null)
            {
                btnProbabilityDarkBg.onClick.RemoveAllListeners();
                btnProbabilityDarkBg.onClick.AddListener(CloseProbabilityModal);
            }
        }

        // ==========================================
        // PICKUP SKILL DETAILS MODAL
        // ==========================================

        public void SetupPickupSkillDetailModal(
            GameObject pModal, Button pClose, Button pOpen,
            TMP_Text badgeTxt, TMP_Text s1Txt, TMP_Text s10Txt, TMP_Text ratesTxt, TMP_Text detailTxt)
        {
            pickupSkillDetailModal = pModal;
            btnClosePickupSkillDetail = pClose;
            btnOpenPickupSkillDetail = pOpen;
            txtPickupBannerBadge = badgeTxt;
            txtPickupSummon1 = s1Txt;
            txtPickupSummon10 = s10Txt;
            txtPickupBtnRates = ratesTxt;
            txtPickupBtnDetail = detailTxt;

            if (pickupSkillDetailModal != null) pickupSkillDetailModal.SetActive(false);

            if (btnClosePickupSkillDetail != null)
            {
                btnClosePickupSkillDetail.onClick.RemoveAllListeners();
                btnClosePickupSkillDetail.onClick.AddListener(ClosePickupSkillDetail);
            }

            if (btnOpenPickupSkillDetail != null)
            {
                btnOpenPickupSkillDetail.onClick.RemoveAllListeners();
                btnOpenPickupSkillDetail.onClick.AddListener(OpenPickupSkillDetail);
            }
        }

        public void OpenPickupSkillDetail()
        {
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
            if (pickupSkillDetailModal != null)
            {
                WireModalAutoClose(pickupSkillDetailModal, ClosePickupSkillDetail);
                pickupSkillDetailModal.transform.SetAsLastSibling();
                pickupSkillDetailModal.SetActive(true);
            }
        }

        public void ClosePickupSkillDetail()
        {
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
            if (pickupSkillDetailModal != null)
            {
                pickupSkillDetailModal.SetActive(false);
            }
        }

        // ==========================================
        // MOBILE SETTINGS (HAPTICS & PRIVACY POLICY)
        // ==========================================

        public void SetupMobileSettings(Button hapticBtn, TMP_Text hapticTxt, Button privacyBtn)
        {
            btnHapticToggle = hapticBtn;
            hapticToggleText = hapticTxt;
            btnPrivacyPolicy = privacyBtn;

            if (btnHapticToggle != null)
            {
                btnHapticToggle.onClick.RemoveAllListeners();
                btnHapticToggle.onClick.AddListener(ToggleHapticSetting);
            }

            if (btnPrivacyPolicy != null)
            {
                btnPrivacyPolicy.onClick.RemoveAllListeners();
                btnPrivacyPolicy.onClick.AddListener(OpenPrivacyPolicy);
            }

            UpdateHapticUI();
        }

        public void ToggleHapticSetting()
        {
            PlayClickSound();
            MobileDeviceManager.IsHapticEnabled = !MobileDeviceManager.IsHapticEnabled;
            if (MobileDeviceManager.IsHapticEnabled)
            {
                MobileDeviceManager.TriggerHapticLight();
            }
            UpdateHapticUI();
        }

        private void UpdateHapticUI()
        {
            bool enabled = MobileDeviceManager.IsHapticEnabled;
            if (hapticToggleText != null)
            {
                hapticToggleText.text = enabled
                    ? LocalizationManager.Get("settings_haptic_on", "진동: 켜짐")
                    : LocalizationManager.Get("settings_haptic_off", "진동: 꺼짐");
            }
        }

        public void OpenPrivacyPolicy()
        {
            PlayClickSound();
            Application.OpenURL("https://mallanggames.com/privacy");
        }

        private IEnumerator FlashDiamondsTextRed()
        {
            if (shopDiamondsText == null) yield break;
            Color orig = shopDiamondsText.color;
            shopDiamondsText.color = new Color(1f, 0.25f, 0.35f);
            yield return new WaitForSeconds(0.4f);
            shopDiamondsText.color = orig;
        }

        public void BuyOrEquipLobbyTheme(int lobbyIdx)
        {
            if (lobbyIdx < 0 || lobbyIdx >= LobbyThemePrices.Length) return;

            bool isOwned = (lobbyIdx == 0) || (PlayerPrefs.GetInt(KEY_LOBBY_THEME_OWNED_PREFIX + lobbyIdx, 1) == 1);

            if (isOwned)
            {
                PlayerPrefs.SetInt(KEY_EQUIPPED_LOBBY_THEME, lobbyIdx);
                PlayerPrefs.Save();
                ApplyLobbyTheme(lobbyIdx);
                RefreshLobbyThemeShopUI();
                PlayClickSound();
            }
            else
            {
                int price = LobbyThemePrices[lobbyIdx];
                if (_currentCoins >= price)
                {
                    _currentCoins -= price;
                    PlayerPrefs.SetInt(KEY_COINS, _currentCoins);
                    PlayerPrefs.SetInt(KEY_LOBBY_THEME_OWNED_PREFIX + lobbyIdx, 1);
                    PlayerPrefs.SetInt(KEY_EQUIPPED_LOBBY_THEME, lobbyIdx);
                    PlayerPrefs.Save();

                    ApplyLobbyTheme(lobbyIdx);
                    RefreshProfileUI();
                    RefreshThemeShopUI();
                    RefreshLobbyThemeShopUI();

                    if (FairyScreenTransition.Instance != null)
                    {
                        FairyScreenTransition.Instance.EmitCornerSparkles();
                    }
                    if (BlockAudioManager.Instance != null)
                    {
                        BlockAudioManager.Instance.PlayBuy();
                    }
                }
                else
                {
                    // Insufficient Coins
                    PlayClickSound();
                    if (shopCoinsText != null)
                    {
                        StartCoroutine(FlashCoinsTextRed());
                    }
                }
            }
        }

        public void ApplyLobbyTheme(int lobbyIdx, bool playMusic = true)
        {
            if (lobbyBackgroundImg == null)
            {
                var bgObj = GameObject.Find("LobbyBg");
                if (bgObj != null) lobbyBackgroundImg = bgObj.GetComponent<Image>();
            }

            if (lobbyBackgroundImg != null && lobbyThemeSprites != null && lobbyIdx >= 0 && lobbyIdx < lobbyThemeSprites.Length)
            {
                if (lobbyThemeSprites[lobbyIdx] != null)
                {
                    lobbyBackgroundImg.sprite = lobbyThemeSprites[lobbyIdx];
                    lobbyBackgroundImg.color = Color.white;
                }
            }

            // Play matching Lobby BGM track (Theme 0 -> robby 1, Theme 1 -> robby 2, Theme 2 -> robby 3)
            if (playMusic && BlockAudioManager.Instance != null)
            {
                BlockAudioManager.Instance.PlayLobbyBGM(lobbyIdx);
            }
        }

        public void RefreshLobbyThemeShopUI()
        {
            RefreshCurrenciesUI();

            int equippedLobby = PlayerPrefs.GetInt(KEY_EQUIPPED_LOBBY_THEME, 0);

            if (lobbyThemeActionButtons != null)
            {
                for (int i = 0; i < lobbyThemeActionButtons.Length; i++)
                {
                    if (lobbyThemeActionButtons[i] == null) continue;

                    bool isEquipped = (equippedLobby == i);
                    bool isOwned = (i == 0) || (PlayerPrefs.GetInt(KEY_LOBBY_THEME_OWNED_PREFIX + i, 1) == 1);

                    Image btnImg = lobbyThemeActionButtons[i].GetComponent<Image>();
                    TMP_Text txt = (lobbyThemeActionTexts != null && i < lobbyThemeActionTexts.Length && lobbyThemeActionTexts[i] != null)
                        ? lobbyThemeActionTexts[i]
                        : lobbyThemeActionButtons[i].GetComponentInChildren<TMP_Text>();

                    if (isEquipped)
                    {
                        if (txt != null)
                        {
                            txt.text = LocalizationManager.Get("shop_btn_equipped");
                            txt.color = Color.white;
                            txt.enableAutoSizing = true;
                            txt.fontSizeMin = 13f;
                        }
                        if (btnImg != null)
                        {
                            if (shopEquippedBtnSprite != null) btnImg.sprite = shopEquippedBtnSprite;
                            btnImg.color = Color.white;
                        }
                        lobbyThemeActionButtons[i].interactable = false;
                    }
                    else if (isOwned)
                    {
                        if (txt != null)
                        {
                            txt.text = LocalizationManager.Get("shop_btn_equip");
                            txt.color = Color.white;
                            txt.enableAutoSizing = true;
                            txt.fontSizeMin = 13f;
                        }
                        if (btnImg != null)
                        {
                            if (shopEquipBtnSprite != null) btnImg.sprite = shopEquipBtnSprite;
                            btnImg.color = Color.white;
                        }
                        lobbyThemeActionButtons[i].interactable = true;
                    }
                    else
                    {
                        int price = (i < LobbyThemePrices.Length) ? LobbyThemePrices[i] : 0;
                        bool canAfford = (_currentCoins >= price);
                        if (txt != null)
                        {
                            txt.text = $"{price:N0} C " + LocalizationManager.Get("shop_btn_buy");
                            txt.color = canAfford ? new Color(0.24f, 0.12f, 0.00f, 1f) : new Color(0.42f, 0.32f, 0.52f, 1f);
                            txt.fontStyle = FontStyles.Bold;
                            txt.enableAutoSizing = true;
                            txt.fontSizeMin = 16f;
                        }
                        if (btnImg != null)
                        {
                            if (canAfford)
                            {
                                btnImg.sprite = (shopGoldBtnSprite != null) ? shopGoldBtnSprite : shopEquipBtnSprite;
                            }
                            else
                            {
                                btnImg.sprite = (shopCreamBtnSprite != null) ? shopCreamBtnSprite : shopEquipBtnSprite;
                            }
                            btnImg.color = Color.white;
                        }
                        lobbyThemeActionButtons[i].interactable = true;
                    }
                }
            }

            if (shopLobbyThemesPanel != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    var itemCard = shopLobbyThemesPanel.transform.Find($"LobbyThemeItem_{i}");
                    if (itemCard != null)
                    {
                        var nameTxt = itemCard.Find("Name")?.GetComponent<TMP_Text>();
                        if (nameTxt != null) nameTxt.text = LocalizationManager.Get($"theme_lobby_{i}_name");
                        var descTxt = itemCard.Find("Desc")?.GetComponent<TMP_Text>();
                        if (descTxt != null) descTxt.text = LocalizationManager.Get($"theme_lobby_{i}_desc");
                    }
                }
            }
        }

        public void SetupShopVerticalTabs(
            Button[] tabBtns, Image[] tabBgs, TMP_Text[] tabTxts,
            GameObject recPanel, GameObject pickPanel, GameObject mascPanel,
            GameObject inGamePanel, GameObject lobbyPanel,
            Sprite activeSprite, Sprite inactiveSprite,
            TMP_Text sCoinsTxt = null, TMP_Text sDiaTxt = null)
        {
            shopTabButtons = tabBtns;
            shopTabBgs = tabBgs;
            shopTabTexts = tabTxts;
            shopRecommendedPanel = recPanel;
            shopPickupPanel = pickPanel;
            shopMascotsPanel = mascPanel;
            shopInGameThemesPanel = inGamePanel;
            shopLobbyThemesPanel = lobbyPanel;
            tabVerticalActiveSprite = activeSprite;
            tabVerticalInactiveSprite = inactiveSprite;
            if (sCoinsTxt != null) shopCoinsText = sCoinsTxt;
            if (sDiaTxt != null) shopDiamondsText = sDiaTxt;

            if (shopTabButtons != null)
            {
                for (int i = 0; i < shopTabButtons.Length; i++)
                {
                    int tabIdx = i;
                    if (shopTabButtons[i] != null)
                    {
                        shopTabButtons[i].onClick.RemoveAllListeners();
                        shopTabButtons[i].onClick.AddListener(() => { PlayClickSound(); SelectShopTab(tabIdx); });
                    }
                }
            }

            SelectShopTab(0);
        }

        public void SetupPickupBannerController(AnimatedPickupBannerController bannerCtrl)
        {
            pickupBannerController = bannerCtrl;
        }

        public void SetupShopAnimationController(ShopUIAnimationController animCtrl)
        {
            shopAnimController = animCtrl;
        }

        public void SetupShopMascots(Button[] actionBtns, TMP_Text[] actionTexts)
        {
            mascotShopActionButtons = actionBtns;
            mascotShopActionTexts = actionTexts;

            if (mascotShopActionButtons != null)
            {
                for (int i = 0; i < mascotShopActionButtons.Length; i++)
                {
                    int idx = i;
                    if (mascotShopActionButtons[i] != null)
                    {
                        mascotShopActionButtons[i].onClick.RemoveAllListeners();
                        mascotShopActionButtons[i].onClick.AddListener(() => BuyOrEquipMascot(idx));
                    }
                }
            }

            RefreshMascotShopUI();
        }

        public void SetupShopPackagesAndSummon(
            Button[] packBtns, Button summon1Btn, Button summon10Btn)
        {
            shopPackageButtons = packBtns;
            btnSummon1 = summon1Btn;
            btnSummon10 = summon10Btn;

            if (shopPackageButtons != null)
            {
                for (int i = 0; i < shopPackageButtons.Length; i++)
                {
                    int pIdx = i;
                    if (shopPackageButtons[i] != null)
                    {
                        shopPackageButtons[i].onClick.RemoveAllListeners();
                        shopPackageButtons[i].onClick.AddListener(() => BuyPackage(pIdx));
                    }
                }
            }

            if (btnSummon1 != null)
            {
                btnSummon1.onClick.RemoveAllListeners();
                btnSummon1.onClick.AddListener(() => SummonPickup(1));
            }

            if (btnSummon10 != null)
            {
                btnSummon10.onClick.RemoveAllListeners();
                btnSummon10.onClick.AddListener(() => SummonPickup(10));
            }
        }

        public void SetupMascotModal(
            GameObject modal, Button closeBtn,
            Button[] actionBtns, TMP_Text[] actionTexts, TMP_Text[] statusTexts = null,
            Button[] upgradeBtns = null, TMP_Text[] upgradeTexts = null,
            TMP_Text[] levelTexts = null, TMP_Text[] shardTexts = null,
            TMP_Text[] abilityTexts = null)
        {
            mascotModal = modal;
            btnCloseMascotModal = closeBtn;
            mascotActionButtons = actionBtns;
            mascotActionTexts = actionTexts;
            mascotStatusTexts = statusTexts;
            mascotUpgradeButtons = upgradeBtns;
            mascotUpgradeTexts = upgradeTexts;
            mascotLevelTexts = levelTexts;
            mascotShardTexts = shardTexts;
            mascotAbilityTexts = abilityTexts;

            if (btnCloseMascotModal != null)
            {
                btnCloseMascotModal.onClick.RemoveAllListeners();
                btnCloseMascotModal.onClick.AddListener(() => { PlayClickSound(); CloseMascotModal(); });
            }

            if (mascotActionButtons != null)
            {
                for (int i = 0; i < mascotActionButtons.Length; i++)
                {
                    int idx = i;
                    if (mascotActionButtons[i] != null)
                    {
                        mascotActionButtons[i].onClick.RemoveAllListeners();
                        mascotActionButtons[i].onClick.AddListener(() => EquipMascot(idx));
                    }
                }
            }

            if (mascotUpgradeButtons != null)
            {
                for (int i = 0; i < mascotUpgradeButtons.Length; i++)
                {
                    int idx = i;
                    if (mascotUpgradeButtons[i] != null)
                    {
                        mascotUpgradeButtons[i].onClick.RemoveAllListeners();
                        mascotUpgradeButtons[i].onClick.AddListener(() => TryUpgradeMascot(idx));
                    }
                }
            }

            RefreshMascotModalUI();
        }

        public void SetupShopTabs(
            Button tabInGame, Button tabLobby,
            Image tabInGameBg, Image tabLobbyBg,
            TMP_Text tabInGameTxt, TMP_Text tabLobbyTxt,
            GameObject inGamePanel, GameObject lobbyPanel)
        {
            shopInGameThemesPanel = inGamePanel;
            shopLobbyThemesPanel = lobbyPanel;
        }

        public void SetupShopTabSprites(Sprite gameActive, Sprite lobbyActive, Sprite inactive, Sprite originalPill)
        {
            tabGameActiveSprite = gameActive;
            tabLobbyActiveSprite = lobbyActive;
            tabInactiveSprite = inactive;
            tabOriginalPillSprite = originalPill;
        }

        public void SetupLobbyThemes(
            Image lobbyBg, Sprite[] bgSprites,
            Button[] actionBtns, TMP_Text[] actionTexts, TMP_Text[] priceTexts = null)
        {
            lobbyBackgroundImg = lobbyBg;
            lobbyThemeSprites = bgSprites;
            lobbyThemeActionButtons = actionBtns;
            lobbyThemeActionTexts = actionTexts;
            lobbyThemePriceTexts = priceTexts;

            if (lobbyThemeActionButtons != null)
            {
                for (int i = 0; i < lobbyThemeActionButtons.Length; i++)
                {
                    int idx = i;
                    if (lobbyThemeActionButtons[i] != null)
                    {
                        lobbyThemeActionButtons[i].onClick.RemoveAllListeners();
                        lobbyThemeActionButtons[i].onClick.AddListener(() =>
                        {
                            BuyOrEquipLobbyTheme(idx);
                        });
                    }
                }
            }

            int equippedLobby = PlayerPrefs.GetInt(KEY_EQUIPPED_LOBBY_THEME, 0);
            ApplyLobbyTheme(equippedLobby);
            RefreshLobbyThemeShopUI();
        }

        // ==========================================
        // SETTINGS MODAL
        // ==========================================

        public void OpenSettingsModal()
        {
            if (settingsModal != null)
            {
                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
                if (btnPlayGame != null) btnPlayGame.gameObject.SetActive(false);
                if (FairyScreenTransition.Instance != null) FairyScreenTransition.Instance.EmitCornerSparkles();
                WireModalAutoClose(settingsModal, CloseSettingsModal);
                settingsModal.transform.SetAsLastSibling();
                settingsModal.SetActive(true);
            }
        }

        public void CloseSettingsModal()
        {
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
            if (settingsModal != null) settingsModal.SetActive(false);
            if (btnPlayGame != null) btnPlayGame.gameObject.SetActive(true);
        }

        private void Update()
        {
            if (lobbyRoot == null || !lobbyRoot.activeInHierarchy || Time.unscaledTime < _ignoreEscUntil) return;

            bool isEsc = false;
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                isEsc = true;
            }
#endif
            // Mobile Android Back Button & fallback
            if (!isEsc && Input.GetKeyDown(KeyCode.Escape))
            {
                isEsc = true;
            }

            if (isEsc)
            {
                HandleEscapeKey();
            }
        }

        public void WireModalAutoClose(GameObject modalGo, System.Action closeAction)
        {
            if (modalGo == null) return;

            // 1. DarkBg click-to-close outside
            Transform darkBg = modalGo.transform.Find("DarkBg");
            if (darkBg != null)
            {
                Button dbBtn = darkBg.GetComponent<Button>();
                if (dbBtn == null) dbBtn = darkBg.gameObject.AddComponent<Button>();
                dbBtn.transition = Selectable.Transition.None;
                dbBtn.onClick.RemoveAllListeners();
                dbBtn.onClick.AddListener(() => { PlayClickSound(); closeAction(); });

                darkBg.SetAsFirstSibling();
                Image dbImg = darkBg.GetComponent<Image>();
                if (dbImg != null) dbImg.raycastTarget = true;
            }

            // 2. All close/cancel/confirm buttons inside this modal
            Button[] buttons = modalGo.GetComponentsInChildren<Button>(true);
            foreach (var b in buttons)
            {
                if (b == null || (darkBg != null && b.gameObject == darkBg.gameObject)) continue;
                string bName = b.name.ToLower();
                if (bName == "btnclose" || bName == "btncancel" || bName == "buttonclose" || bName == "closebtn" ||
                    (bName == "btnconfirm" && (modalGo == pickupSkillDetailModal || modalGo == probabilityModal || modalGo == summonResultModal || modalGo == helpModal)))
                {
                    // Remove conflicting EventTrigger if present
                    EventTrigger et = b.GetComponent<EventTrigger>();
                    if (et != null)
                    {
                        if (Application.isPlaying) Destroy(et);
                        else DestroyImmediate(et);
                    }

                    // Ensure button is at front of its parent so it is never covered
                    b.transform.SetAsLastSibling();

                    // Ensure raycast target is active
                    Image img = b.GetComponent<Image>();
                    if (img != null) img.raycastTarget = true;

                    b.onClick.RemoveAllListeners();
                    b.onClick.AddListener(() => { PlayClickSound(); closeAction(); });
                }
            }
        }

        private void HandleEscapeKey()
        {
            // 1. Legal Probability Modal
            if (probabilityModal != null && probabilityModal.activeSelf)
            {
                CloseProbabilityModal();
                return;
            }

            // 2. Pickup Skill Detail Modal
            if (pickupSkillDetailModal != null && pickupSkillDetailModal.activeSelf)
            {
                ClosePickupSkillDetail();
                return;
            }

            // 3. Summon Result Modal
            if (summonResultModal != null && summonResultModal.activeSelf)
            {
                CloseSummonResultModal();
                return;
            }

            // 4. Gacha Presentation Sequence
            if (GachaPresentationController.Instance != null && GachaPresentationController.Instance.IsActive)
            {
                GachaPresentationController.Instance.CloseModal();
                return;
            }

            // 5. Mascot Detail Modal
            if (mascotDetailModal != null && mascotDetailModal.activeSelf)
            {
                CloseMascotDetail();
                return;
            }

            // 6. Mascot Codex Modal
            if (mascotCodexModal != null && mascotCodexModal.activeSelf)
            {
                CloseMascotModal();
                return;
            }

            // 7. Mascot Management Modal
            if (mascotModal != null && mascotModal.activeSelf)
            {
                CloseMascotModal();
                return;
            }

            // 8. Standard Dialogs
            if (quitModal != null && quitModal.activeSelf)
            {
                CloseQuitModal();
                return;
            }
            if (settingsModal != null && settingsModal.activeSelf)
            {
                CloseSettingsModal();
                return;
            }
            if (shopModal != null && shopModal.activeSelf)
            {
                CloseShopModal();
                return;
            }
            if (helpModal != null && helpModal.activeSelf)
            {
                CloseHelpModal();
                return;
            }
            if (loginModal != null && loginModal.activeSelf)
            {
                CloseLoginModal();
                return;
            }
            if (profileModal != null && profileModal.activeSelf)
            {
                CloseProfileModal();
                return;
            }

            // 9. If no modal is open in Lobby, prompt quit confirmation
            OpenQuitModal();
        }

        // ==========================================
        // QUIT MODAL & MULTI-RESOLUTION SETTINGS
        // ==========================================

        public void OpenQuitModal()
        {
            if (quitModal != null)
            {
                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
                WireModalAutoClose(quitModal, CloseQuitModal);
                quitModal.transform.SetAsLastSibling();
                quitModal.SetActive(true);
            }
        }

        public void CloseQuitModal()
        {
            if (quitModal != null)
            {
                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
                quitModal.SetActive(false);
            }
        }

        public void QuitGame()
        {
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayUIClick();
            Debug.Log("<color=red>[LobbyManager] Quitting application...</color>");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void SetAspectRatio(int aspectIdx)
        {
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayUIClick();
            _currentAspectIdx = Mathf.Clamp(aspectIdx, 0, 3);
            PlayerPrefs.SetInt(KEY_ASPECT_RATIO_INDEX, _currentAspectIdx);
            PlayerPrefs.Save();
            ApplyScreenSettings();
            UpdateScreenSettingsUI();
        }

        public void SetWindowMode(int modeIdx)
        {
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayUIClick();
            _currentWindowModeIdx = Mathf.Clamp(modeIdx, 0, 2);
            PlayerPrefs.SetInt(KEY_WINDOW_MODE_INDEX, _currentWindowModeIdx);
            PlayerPrefs.Save();
            ApplyScreenSettings();
            UpdateScreenSettingsUI();
        }

        public void ApplyScreenSettings()
        {
            FullScreenMode mode = FullScreenMode.FullScreenWindow;
            if (_currentWindowModeIdx == 0) mode = FullScreenMode.Windowed;
            else if (_currentWindowModeIdx == 1) mode = FullScreenMode.FullScreenWindow;
            else if (_currentWindowModeIdx == 2) mode = FullScreenMode.ExclusiveFullScreen;

            int screenW = Display.main.systemWidth > 0 ? Display.main.systemWidth : Screen.currentResolution.width;
            int screenH = Display.main.systemHeight > 0 ? Display.main.systemHeight : Screen.currentResolution.height;
            if (screenW <= 0) screenW = 1920;
            if (screenH <= 0) screenH = 1080;

            int targetW = screenW;
            int targetH = screenH;

            if (mode == FullScreenMode.Windowed)
            {
                switch (_currentAspectIdx)
                {
                    case 0: targetW = 1600; targetH = 900; break;  // 16:9
                    case 1: targetW = 1440; targetH = 900; break;  // 16:10
                    case 2: targetW = 1200; targetH = 900; break;  // 4:3
                    case 3: targetW = 720; targetH = 1280; break;  // 9:16
                    default: targetW = 1600; targetH = 900; break;
                }
            }
            else if (mode == FullScreenMode.ExclusiveFullScreen)
            {
                switch (_currentAspectIdx)
                {
                    case 0: targetW = 1920; targetH = 1080; break; // 16:9
                    case 1: targetW = 1920; targetH = 1200; break; // 16:10
                    case 2: targetW = 1440; targetH = 1080; break; // 4:3
                    case 3: targetW = 1080; targetH = 1920; break; // 9:16
                    default: targetW = 1920; targetH = 1080; break;
                }
            }
            else // FullScreenWindow (Borderless)
            {
                targetW = screenW;
                targetH = screenH;
            }

            Screen.SetResolution(targetW, targetH, mode);
            Debug.Log($"<color=green>[LobbyManager] Applied Screen Settings: Aspect={_currentAspectIdx}, Mode={mode} -> {targetW}x{targetH}</color>");

            if (AspectRatioAdapter.Instance != null)
            {
                AspectRatioAdapter.Instance.UpdateScaler();
            }
            if (SideWingsDecorator.Instance != null)
            {
                SideWingsDecorator.Instance.UpdateVisibility();
            }
        }

        public void UpdateScreenSettingsUI()
        {
            bool isMobile = Application.isMobilePlatform;
#if !UNITY_STANDALONE
            isMobile = true;
#endif

            if (settingsAspectTitleText != null)
            {
                settingsAspectTitleText.text = LocalizationManager.Get("settings_aspect_ratio_title");
                settingsAspectTitleText.gameObject.SetActive(!isMobile);
            }
            if (settingsWindowModeTitleText != null)
            {
                settingsWindowModeTitleText.text = LocalizationManager.Get("settings_window_mode_title");
                settingsWindowModeTitleText.gameObject.SetActive(!isMobile);
            }

            string[] aspectKeys = new string[] { "aspect_16_9", "aspect_16_10", "aspect_4_3", "aspect_9_16" };
            if (aspectButtons != null && aspectBgs != null && aspectTexts != null)
            {
                for (int i = 0; i < aspectButtons.Length && i < aspectKeys.Length; i++)
                {
                    if (aspectButtons[i] != null) aspectButtons[i].gameObject.SetActive(!isMobile);
                    bool isActive = (_currentAspectIdx == i);
                    if (aspectBgs[i] != null)
                    {
                        if (screenActiveSprite != null && screenInactiveSprite != null)
                        {
                            aspectBgs[i].sprite = isActive ? screenActiveSprite : screenInactiveSprite;
                            aspectBgs[i].color = Color.white;
                        }
                        else
                        {
                            aspectBgs[i].color = isActive ? ColorPink : new Color(0.92f, 0.90f, 0.98f, 1f);
                        }
                    }
                    if (aspectTexts[i] != null)
                    {
                        aspectTexts[i].text = LocalizationManager.Get(aspectKeys[i]);
                        aspectTexts[i].color = isActive ? Color.white : new Color(0.35f, 0.22f, 0.55f);
                        aspectTexts[i].fontStyle = isActive ? FontStyles.Bold : FontStyles.Normal;
                    }
                }
            }

            string[] winModeKeys = new string[] { "window_mode_windowed", "window_mode_borderless", "window_mode_fullscreen" };
            if (windowModeButtons != null && windowModeBgs != null && windowModeTexts != null)
            {
                for (int j = 0; j < windowModeButtons.Length && j < winModeKeys.Length; j++)
                {
                    if (windowModeButtons[j] != null) windowModeButtons[j].gameObject.SetActive(!isMobile);
                    bool isActive = (_currentWindowModeIdx == j);
                    if (windowModeBgs[j] != null)
                    {
                        if (screenActiveSprite != null && screenInactiveSprite != null)
                        {
                            windowModeBgs[j].sprite = isActive ? screenActiveSprite : screenInactiveSprite;
                            windowModeBgs[j].color = Color.white;
                        }
                        else
                        {
                            windowModeBgs[j].color = isActive ? ColorPink : new Color(0.92f, 0.90f, 0.98f, 1f);
                        }
                    }
                    if (windowModeTexts[j] != null)
                    {
                        windowModeTexts[j].text = LocalizationManager.Get(winModeKeys[j]);
                        windowModeTexts[j].color = isActive ? Color.white : new Color(0.35f, 0.22f, 0.55f);
                        windowModeTexts[j].fontStyle = isActive ? FontStyles.Bold : FontStyles.Normal;
                    }
                }
            }
        }

        public void UpdateScreenModeUI(bool isFullscreen)
        {
            UpdateScreenSettingsUI();
        }

        // ==========================================
        // HELP MODAL
        // ==========================================

        public void OpenHelpModal()
        {
            if (helpModal != null)
            {
                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
                if (btnPlayGame != null) btnPlayGame.gameObject.SetActive(false);
                if (FairyScreenTransition.Instance != null) FairyScreenTransition.Instance.EmitCornerSparkles();
                WireModalAutoClose(helpModal, CloseHelpModal);
                helpModal.transform.SetAsLastSibling();
                helpModal.SetActive(true);
            }
        }

        public void CloseHelpModal()
        {
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
            if (helpModal != null) helpModal.SetActive(false);
            if (btnPlayGame != null) btnPlayGame.gameObject.SetActive(true);
        }

        // ==========================================
        // SCENE TRANSITIONS
        // ==========================================

        public void ShowLobby()
        {
            _ignoreEscUntil = Time.unscaledTime + 0.35f;
            if (quitModal != null) quitModal.SetActive(false);
            if (settingsModal != null) settingsModal.SetActive(false);
            if (shopModal != null) shopModal.SetActive(false);
            if (helpModal != null) helpModal.SetActive(false);
            if (profileModal != null) profileModal.SetActive(false);

            if (lobbyRoot != null) lobbyRoot.SetActive(true);
            if (lobbyCanvasGroup != null) lobbyCanvasGroup.alpha = 1f;
            SelectMenu(0, false);
            RefreshProfileUI();
            int equippedLobby = PlayerPrefs.GetInt(KEY_EQUIPPED_LOBBY_THEME, 0);
            ApplyLobbyTheme(equippedLobby);
            if (BlockBlastUIManager.Instance != null)
            {
                BlockBlastUIManager.Instance.ShowInGameUI(false);
            }
            StartMascotIdleBounce();
        }

        public void StartGameFromLobby()
        {
            if (quitModal != null) quitModal.SetActive(false);
            if (GachaPresentationController.Instance != null) GachaPresentationController.Instance.CloseModal();

            if (BlockAudioManager.Instance != null)
            {
                BlockAudioManager.Instance.PlayInGameBGM();
            }

            if (FairyScreenTransition.Instance != null)
            {
                FairyScreenTransition.Instance.DoTransition(() =>
                {
                    if (lobbyRoot != null) lobbyRoot.SetActive(false);
                    if (BlockBlastUIManager.Instance != null)
                    {
                        BlockBlastUIManager.Instance.ShowInGameUI(true);
                        BlockBlastUIManager.Instance.StartGameFromMenu();
                    }
                });
            }
            else
            {
                StartCoroutine(TransitionToGameRoutine());
            }
        }

        private IEnumerator TransitionToGameRoutine()
        {
            yield return StartCoroutine(FadeCanvasGroup(lobbyCanvasGroup, 1f, 0f, 0.25f));
            if (lobbyRoot != null) lobbyRoot.SetActive(false);

            if (BlockBlastUIManager.Instance != null)
            {
                BlockBlastUIManager.Instance.ShowInGameUI(true);
                BlockBlastUIManager.Instance.StartGameFromMenu();
            }
        }

        public void ReturnToLobby()
        {
            if (FairyScreenTransition.Instance != null)
            {
                FairyScreenTransition.Instance.DoTransition(() =>
                {
                    if (BlockBlastUIManager.Instance != null)
                    {
                        BlockBlastUIManager.Instance.RestartGame();
                    }
                    ShowLobby();
                });
            }
            else
            {
                if (BlockBlastUIManager.Instance != null)
                {
                    BlockBlastUIManager.Instance.RestartGame();
                }
                ShowLobby();
            }
        }

        private IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
        {
            if (cg == null) yield break;
            float elapsed = 0f;
            cg.alpha = from;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                cg.alpha = Mathf.Lerp(from, to, elapsed / duration);
                yield return null;
            }
            cg.alpha = to;
        }

        // ==========================================
        // PARTY MASCOT IDLE ANIMATION
        // ==========================================

        public void StartMascotIdleBounce()
        {
            if (_mascotBounceCoroutine != null)
            {
                StopCoroutine(_mascotBounceCoroutine);
                _mascotBounceCoroutine = null;
            }
            if (gameObject.activeInHierarchy)
            {
                _mascotBounceCoroutine = StartCoroutine(MascotIdleBounceRoutine());
            }
        }

        private IEnumerator MascotIdleBounceRoutine()
        {
            if (partyMascots == null || partyMascots.Length == 0) yield break;

            Vector2[] origPositions = new Vector2[partyMascots.Length];
            for (int i = 0; i < partyMascots.Length; i++)
            {
                if (partyMascots[i] != null)
                {
                    Vector2 pos = partyMascots[i].anchoredPosition;
                    if (pos == Vector2.zero && i < DefaultMascotPositions.Length)
                    {
                        pos = DefaultMascotPositions[i];
                    }
                    origPositions[i] = pos;
                }
                else if (i < DefaultMascotPositions.Length)
                {
                    origPositions[i] = DefaultMascotPositions[i];
                }
            }

            while (true)
            {
                // Pause animation when lobby root is hidden (e.g. during in-game play)
                if (lobbyRoot != null && !lobbyRoot.activeInHierarchy)
                {
                    yield return null;
                    continue;
                }

                float time = Time.time;
                for (int i = 0; i < partyMascots.Length; i++)
                {
                    if (partyMascots[i] != null)
                    {
                        // Staggered cute bounce rhythm with playful phase offset
                        float offset = i * 0.78f;
                        float bounceY = Mathf.Abs(Mathf.Sin(time * 3.0f + offset)) * 16f;
                        float squashX = 1f + Mathf.Sin(time * 3.0f + offset) * 0.05f;
                        float squashY = 1f - Mathf.Sin(time * 3.0f + offset) * 0.05f;
                        float tiltZ = Mathf.Sin(time * 2.2f + offset) * 2.5f;

                        float punchS = (i < _mascotPunchScale.Length) ? _mascotPunchScale[i] : 1f;
                        float punchY = (i < _mascotPunchY.Length) ? _mascotPunchY[i] : 0f;

                        partyMascots[i].anchoredPosition = origPositions[i] + new Vector2(0f, bounceY + punchY);
                        partyMascots[i].localScale = new Vector3(squashX * punchS, squashY * punchS, 1f);
                        partyMascots[i].localRotation = Quaternion.Euler(0f, 0f, tiltZ);
                    }
                }
                yield return null;
            }
        }

        public void TriggerPunchMascot(int index)
        {
            if (index < 0 || index >= _mascotPunchScale.Length) return;
            if (_mascotPunchCoroutines[index] != null) StopCoroutine(_mascotPunchCoroutines[index]);
            _mascotPunchCoroutines[index] = StartCoroutine(PunchMascotIndexRoutine(index));
        }

        private IEnumerator PunchMascotIndexRoutine(int index)
        {
            float elapsed = 0f;
            float duration = 0.35f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                // Elastic punch bounce curve (1.0 -> 1.30 -> 0.92 -> 1.05 -> 1.0)
                float scaleBounce = 1f + Mathf.Sin(t * Mathf.PI * 1.5f) * Mathf.Exp(-t * 3.2f) * 0.32f;
                float yHop = Mathf.Sin(t * Mathf.PI) * 22f;
                _mascotPunchScale[index] = scaleBounce;
                _mascotPunchY[index] = yHop;
                yield return null;
            }
            _mascotPunchScale[index] = 1f;
            _mascotPunchY[index] = 0f;
            _mascotPunchCoroutines[index] = null;
        }

        private IEnumerator PunchMascot(RectTransform mascotRT)
        {
            if (mascotRT == null) yield break;

            if (partyMascots != null)
            {
                for (int i = 0; i < partyMascots.Length; i++)
                {
                    if (partyMascots[i] == mascotRT)
                    {
                        TriggerPunchMascot(i);
                        yield break;
                    }
                }
            }

            float elapsed = 0f;
            float duration = 0.28f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float scale = 1f + Mathf.Sin(t * Mathf.PI) * 0.25f;
                mascotRT.localScale = new Vector3(scale, scale, 1f);
                yield return null;
            }
            mascotRT.localScale = Vector3.one;
        }

        private void PlayClickSound()
        {
            if (BlockAudioManager.Instance != null)
            {
                BlockAudioManager.Instance.PlayUIClick();
            }
        }

        // ==========================================
        // SETUP HELPER (Called by SceneBuilder)
        // ==========================================

        public void SetupReferences(
            GameObject root, CanvasGroup cg,
            Button pBtn, Image pAvatar, TMP_Text cText,
            RectTransform[] mascots, GameObject[] labels, Image[] labelBgs, TMP_Text[] labelTexts,
            Button playBtn, Image playBtnBg, TMP_Text playBtnText,
            GameObject pModal, Image pModalAv, TMP_Text pNick, TMP_InputField pBio, TMP_Text pBest, TMP_Text pCoin, Button[] avBtns, Button lBtn, Button clPBtn,
            GameObject sModal, TMP_Text sCoins, Button clSBtn,
            GameObject setModal, Slider bSlider, Slider sSlider, Button clSetBtn,
            GameObject hModal, Button clHBtn,
            Sprite[] avatars,
            GameObject[] glowAuras = null,
            TMP_InputField pNickInput = null,
            Button cfmHBtn = null,
            Image playBtnGlow = null,
            TMP_Text partyTip = null,
            TMP_Text dText = null)
        {
            lobbyRoot = root;
            lobbyCanvasGroup = cg;

            profileBtn = pBtn;
            profileBtnAvatar = pAvatar;
            lobbyCoinsText = cText;
            lobbyDiamondsText = dText;

            partyMascots = mascots;
            partyLabels = labels;
            partyLabelBgs = labelBgs;
            partyLabelTexts = labelTexts;
            partyGlowAuras = glowAuras;
            partyTipText = partyTip;

            btnPlayGame = playBtn;
            btnPlayGameBg = playBtnBg;
            btnPlayGameText = playBtnText;
            btnPlayGameGlow = playBtnGlow;

            profileModal = pModal;
            profileModalAvatar = pModalAv;
            profileModalNickname = pNick;
            profileModalNicknameInput = pNickInput;
            profileModalBioInput = pBio;
            profileModalBestScore = pBest;
            profileModalCoins = pCoin;
            avatarSelectButtons = avBtns;
            btnLogout = lBtn;
            btnCloseProfile = clPBtn;

            shopModal = sModal;
            shopCoinsText = sCoins;
            btnCloseShop = clSBtn;

            settingsModal = setModal;
            bgmSlider = bSlider;
            sfxSlider = sSlider;
            btnCloseSettings = clSetBtn;

            helpModal = hModal;
            btnCloseHelp = clHBtn;
            btnConfirmHelp = cfmHBtn;

            mascotAvatars = avatars;

            if (partyTipText != null)
            {
                partyTipText.text = LocalizationManager.Get("lobby_party_tip");
            }

            if (Application.isPlaying)
            {
                StartMascotIdleBounce();
            }
        }

        public void SetupProfileModalExtraReferences(
            TMP_InputField tagInput, Button checkBtn, TMP_Text statusTxt,
            Image modalPlate, Image btnPlate,
            GameObject lModal, TMP_InputField lId, TMP_InputField lPw,
            Button lSub, Button lGgl, Button lCls, TMP_Text lStat,
            Button startEditBtn = null, Button confirmNickBtn = null)
        {
            profileModalTagInput = tagInput;
            btnCheckTag = checkBtn;
            profileModalNickStatus = statusTxt;
            profileModalAvatarPlate = modalPlate;
            profileBtnAvatarPlate = btnPlate;

            loginModal = lModal;
            loginIdInput = lId;
            loginPwInput = lPw;
            btnSubmitLogin = lSub;
            btnGoogleLogin = lGgl;
            btnCloseLogin = lCls;
            loginStatusText = lStat;

            btnStartEditNick = startEditBtn;
            btnConfirmNick = confirmNickBtn;

            SetNickEditMode(false);
            SetupEventListeners();
        }

        public void SetupScreenSettingsAndQuitReferences(
            GameObject qModal, Button qYes, Button qNo, Button qDarkBg,
            TMP_Text qTitle, TMP_Text qDesc, TMP_Text qYesTxt, TMP_Text qNoTxt,
            TMP_Text aspectTitle, Button[] aBtns, Image[] aBgs, TMP_Text[] aTexts,
            TMP_Text winModeTitle, Button[] wBtns, Image[] wBgs, TMP_Text[] wTexts,
            Sprite activeSprite, Sprite inactiveSprite)
        {
            quitModal = qModal;
            btnQuitConfirmYes = qYes;
            btnQuitConfirmNo = qNo;
            btnQuitDarkBg = qDarkBg;
            quitModalTitleText = qTitle;
            quitModalDescText = qDesc;
            quitModalYesText = qYesTxt;
            quitModalNoText = qNoTxt;

            settingsAspectTitleText = aspectTitle;
            aspectButtons = aBtns;
            aspectBgs = aBgs;
            aspectTexts = aTexts;

            settingsWindowModeTitleText = winModeTitle;
            windowModeButtons = wBtns;
            windowModeBgs = wBgs;
            windowModeTexts = wTexts;

            screenActiveSprite = activeSprite;
            screenInactiveSprite = inactiveSprite;

            if (quitModal != null) quitModal.SetActive(false);

            if (btnQuitConfirmYes != null)
            {
                btnQuitConfirmYes.onClick.RemoveAllListeners();
                btnQuitConfirmYes.onClick.AddListener(QuitGame);
            }
            if (btnQuitConfirmNo != null)
            {
                btnQuitConfirmNo.onClick.RemoveAllListeners();
                btnQuitConfirmNo.onClick.AddListener(CloseQuitModal);
            }
            if (btnQuitDarkBg != null)
            {
                btnQuitDarkBg.onClick.RemoveAllListeners();
                btnQuitDarkBg.onClick.AddListener(CloseQuitModal);
            }

            if (aspectButtons != null)
            {
                for (int i = 0; i < aspectButtons.Length; i++)
                {
                    int idx = i;
                    if (aspectButtons[i] != null)
                    {
                        aspectButtons[i].onClick.RemoveAllListeners();
                        aspectButtons[i].onClick.AddListener(() => SetAspectRatio(idx));
                    }
                }
            }

            if (windowModeButtons != null)
            {
                for (int j = 0; j < windowModeButtons.Length; j++)
                {
                    int idx = j;
                    if (windowModeButtons[j] != null)
                    {
                        windowModeButtons[j].onClick.RemoveAllListeners();
                        windowModeButtons[j].onClick.AddListener(() => SetWindowMode(idx));
                    }
                }
            }

            UpdateScreenSettingsUI();
        }

        public void SetupLanguageButtons(
            Button[] langBtns, Image[] langBgs, TMP_Text[] langTexts,
            TMP_Text setT, TMP_Text setB, TMP_Text setS, TMP_Text setL,
            TMP_Text shpT = null, TMP_Text hlT = null, TMP_Text hlC = null, TMP_Text prT = null,
            TMP_Text verT = null,
            Sprite activeSprite = null, Sprite inactiveSprite = null)
        {
            languageButtons = langBtns;
            languageButtonBgs = langBgs;
            languageButtonTexts = langTexts;
            settingsTitleText = setT;
            settingsBgmText = setB;
            settingsSfxText = setS;
            settingsLangText = setL;
            shopTitleText = shpT;
            helpTitleText = hlT;
            helpConfirmText = hlC;
            profileTitleText = prT;
            settingsVersionText = verT;
            if (activeSprite != null) languageActiveSprite = activeSprite;
            if (inactiveSprite != null) languageInactiveSprite = inactiveSprite;

            if (languageButtons != null)
            {
                for (int i = 0; i < languageButtons.Length; i++)
                {
                    int langIdx = i;
                    if (languageButtons[i] != null)
                    {
                        languageButtons[i].onClick.RemoveAllListeners();
                        languageButtons[i].onClick.AddListener(() =>
                        {
                            PlayClickSound();
                            SelectLanguage((GameLanguage)langIdx);
                        });
                    }
                }
            }

            UpdateLanguageUI(LocalizationManager.CurrentLanguage);
        }

        public void SelectLanguage(GameLanguage lang)
        {
            LocalizationManager.CurrentLanguage = lang;
            UpdateLanguageUI(lang);
        }

        public void UpdateLanguageUI(GameLanguage lang)
        {
            // Update 4 Language Button visual states
            if (languageButtons != null)
            {
                for (int i = 0; i < languageButtons.Length; i++)
                {
                    bool isSelected = ((int)lang == i);
                    if (languageButtonBgs != null && i < languageButtonBgs.Length && languageButtonBgs[i] != null)
                    {
                        if (languageActiveSprite != null && languageInactiveSprite != null)
                        {
                            languageButtonBgs[i].sprite = isSelected ? languageActiveSprite : languageInactiveSprite;
                            languageButtonBgs[i].color = Color.white;
                        }
                        else
                        {
                            // Active: Vibrant Candy Pink / Inactive: Soft Lavender Cream
                            languageButtonBgs[i].color = isSelected ? new Color(1f, 0.35f, 0.55f, 1f) : new Color(0.92f, 0.90f, 0.96f, 1f);
                        }
                    }
                    if (languageButtonTexts != null && i < languageButtonTexts.Length && languageButtonTexts[i] != null)
                    {
                        languageButtonTexts[i].color = isSelected ? Color.white : new Color(0.18f, 0.08f, 0.26f, 1f);
                        languageButtonTexts[i].fontStyle = FontStyles.Bold;
                    }
                }
            }

            // Update Party Stage Tip
            EnsurePartyTipReference();
            if (partyTipText != null) partyTipText.text = LocalizationManager.Get("lobby_party_tip");

            // Update Settings Modal Texts
            if (settingsTitleText != null) settingsTitleText.text = LocalizationManager.Get("settings_title");
            if (settingsBgmText != null) settingsBgmText.text = LocalizationManager.Get("settings_bgm");
            if (settingsSfxText != null) settingsSfxText.text = LocalizationManager.Get("settings_sfx");
            if (settingsLangText != null) settingsLangText.text = LocalizationManager.Get("settings_language");
            if (settingsVersionText != null) settingsVersionText.text = LocalizationManager.Get("settings_version");
            if (settingsQuitButtonText != null) settingsQuitButtonText.text = LocalizationManager.Get("settings_btn_quit");
            UpdateScreenSettingsUI();

            // Update Quit Modal Texts
            if (quitModalTitleText != null) quitModalTitleText.text = LocalizationManager.Get("quit_modal_title");
            if (quitModalDescText != null) quitModalDescText.text = LocalizationManager.Get("quit_modal_desc");
            if (quitModalYesText != null) quitModalYesText.text = LocalizationManager.Get("quit_modal_yes");
            if (quitModalNoText != null) quitModalNoText.text = LocalizationManager.Get("quit_modal_no");

            // Update Shop Texts
            if (shopTitleText != null) shopTitleText.text = LocalizationManager.Get("shop_title");
            if (shopTabTexts != null)
            {
                string[] tabKeys = new string[] { "shop_tab_recommended", "shop_tab_pickup", "shop_tab_mascot", "shop_tab_game", "shop_tab_lobby" };
                for (int i = 0; i < shopTabTexts.Length && i < tabKeys.Length; i++)
                {
                    if (shopTabTexts[i] != null) shopTabTexts[i].text = LocalizationManager.Get(tabKeys[i]);
                }
            }

            // Update Help Modal Texts
            if (helpTitleText != null) helpTitleText.text = LocalizationManager.Get("help_title");
            if (helpConfirmText != null) helpConfirmText.text = LocalizationManager.Get("help_confirm");
            if (btnConfirmHelp != null)
            {
                var txt = btnConfirmHelp.GetComponentInChildren<TMP_Text>();
                if (txt != null) txt.text = LocalizationManager.Get("help_confirm");
            }
            if (helpModal != null)
            {
                var card = helpModal.transform.Find("DialogCard");
                if (card != null)
                {
                    var sub = card.Find("Subtitle")?.GetComponent<TMP_Text>();
                    if (sub != null) sub.text = LocalizationManager.Get("help_subtitle");

                    for (int i = 0; i < 4; i++)
                    {
                        var row = card.Find($"HelpRow_{i}");
                        if (row != null)
                        {
                            var stepT = row.Find("StepTitle")?.GetComponent<TMP_Text>();
                            if (stepT != null) stepT.text = LocalizationManager.Get($"help_step_{i + 1}_title");
                            var stepD = row.Find("StepDesc")?.GetComponent<TMP_Text>();
                            if (stepD != null) stepD.text = LocalizationManager.Get($"help_step_{i + 1}_desc");
                        }
                    }
                }
            }

            // Update Profile Modal Texts
            if (profileTitleText != null) profileTitleText.text = LocalizationManager.Get("profile_title");
            RefreshProfileUI();

            // Update Floating Mascot Labels
            if (partyLabelTexts != null && partyLabelTexts.Length >= 4)
            {
                if (partyLabelTexts[0] != null) partyLabelTexts[0].text = LocalizationManager.Get("lobby_start");
                if (partyLabelTexts[1] != null) partyLabelTexts[1].text = LocalizationManager.Get("lobby_shop");
                if (partyLabelTexts[2] != null) partyLabelTexts[2].text = LocalizationManager.Get("lobby_mascot");
                if (partyLabelTexts[3] != null) partyLabelTexts[3].text = LocalizationManager.Get("lobby_settings");
            }

            // Refresh Bottom Action Button Text
            if (btnPlayGameText != null)
            {
                btnPlayGameText.text = GetMenuActionText(_selectedMenuIdx);
            }

            // Refresh Shop Buttons ("적용 중", "장착하기", "내 코인")
            RefreshThemeShopUI();
            RefreshLobbyThemeShopUI();

            // Update Mascot Codex & Detail UI
            RefreshMascotCodexUI();
            if (mascotDetailModal != null && mascotDetailModal.activeSelf)
            {
                RefreshMascotDetailUI();
            }

            // Update Pickup Banner & Action Buttons
            if (txtPickupBannerBadge != null) txtPickupBannerBadge.text = LocalizationManager.Get("pickup_banner_badge");
            if (txtPickupBtnRates != null) txtPickupBtnRates.text = LocalizationManager.Get("pickup_btn_rates");
            if (txtPickupBtnDetail != null) txtPickupBtnDetail.text = LocalizationManager.Get("pickup_btn_skill_detail");
            if (txtPickupSummon1 != null) txtPickupSummon1.text = $"{LocalizationManager.Get("pickup_summon_1")}\n◆ 100";
            if (txtPickupSummon10 != null) txtPickupSummon10.text = $"{LocalizationManager.Get("pickup_summon_10")}\n<size=18><color=#FFE600>[+5,000 G]</color></size> ◆ 1,000";

            if (shopModal != null)
            {
                var pickPanel = shopModal.transform.Find("DialogCard/ContentContainer/PickupPanel");
                if (pickPanel != null)
                {
                    var ribTxt = pickPanel.Find("TopRibbon/RibbonTxt")?.GetComponent<TMP_Text>();
                    if (ribTxt != null) ribTxt.text = LocalizationManager.Get("pickup_top_ribbon");

                    var hint = pickPanel.Find("PickHintCard/HintTxt")?.GetComponent<TMP_Text>();
                    if (hint != null) hint.text = LocalizationManager.Get("pickup_pity_hint_bottom");
                }

                var recPanel = shopModal.transform.Find("DialogCard/ContentContainer/RecommendedPanel");
                if (recPanel != null)
                {
                    var ribTxt = recPanel.Find("TopRibbon/RibbonTxt")?.GetComponent<TMP_Text>();
                    if (ribTxt != null) ribTxt.text = LocalizationManager.Get("rec_top_ribbon");
                }

                UpdatePickupPityGaugeUI();
            }

            // Update Pickup Skill Detail Modal Content
            if (pickupSkillDetailModal != null)
            {
                var card = pickupSkillDetailModal.transform.Find("DialogCard");
                if (card != null)
                {
                    var t = card.Find("Title")?.GetComponent<TMP_Text>();
                    if (t != null) t.text = LocalizationManager.Get("pickup_modal_skill_title");
                    var hl = card.Find("ShowcaseCard/HeadTxt")?.GetComponent<TMP_Text>();
                    if (hl != null) hl.text = LocalizationManager.Get("pickup_modal_skill_headline");
                    var sub = card.Find("ShowcaseCard/SubTxt")?.GetComponent<TMP_Text>();
                    if (sub != null) sub.text = LocalizationManager.Get("pickup_modal_skill_sub");
                    for (int i = 0; i < 4; i++)
                    {
                        var feat = card.Find($"Feature_{i}/FeatTxt")?.GetComponent<TMP_Text>();
                        if (feat != null) feat.text = LocalizationManager.Get($"pickup_modal_feature_{i + 1}");
                    }
                    var confirm = card.Find("BtnConfirm")?.GetComponentInChildren<TMP_Text>();
                    if (confirm != null) confirm.text = LocalizationManager.Get("help_confirm");
                }
            }

            // Update Shop Recommended Packages Content
            if (shopModal != null)
            {
                var recPanel = shopModal.transform.Find("DialogCard/ContentContainer/RecommendedPanel");
                if (recPanel != null)
                {
                    var packSub = recPanel.Find("PackSubtitle")?.GetComponent<TMP_Text>();
                    if (packSub != null)
                    {
                        packSub.text = LocalizationManager.CurrentLanguage switch
                        {
                            GameLanguage.EN => "Special Limited Recommended Packages",
                            GameLanguage.JA => "今月の特別限定おすすめパッケージ",
                            GameLanguage.ZH => "本月特别限定推荐礼包",
                            _ => "이달의 특별 한정 추천 패키지"
                        };
                    }

                    for (int p = 0; p < 3; p++)
                    {
                        var pCard = recPanel.Find($"PackageCard_{p}");
                        if (pCard != null)
                        {
                            var title = pCard.Find("Title")?.GetComponent<TMP_Text>();
                            if (title != null) title.text = LocalizationManager.Get($"shop_pack_{p}_title");

                            var rewards = pCard.Find("Rewards")?.GetComponent<TMP_Text>();
                            if (rewards != null) rewards.text = LocalizationManager.Get($"shop_pack_{p}_reward");

                            var btnBuyText = pCard.Find("BtnBuy")?.GetComponentInChildren<TMP_Text>();
                            if (btnBuyText != null)
                            {
                                string todayDate = DateTime.UtcNow.ToString("yyyyMMdd");
                                if (p == 0 && PlayerPrefs.GetString("Mallang_DailyGold_ClaimDate", "") == todayDate)
                                    btnBuyText.text = LocalizationManager.Get("shop_pack_claimed");
                                else if (p == 2 && PlayerPrefs.GetInt("Mallang_WelcomePack_Claimed", 0) == 1)
                                    btnBuyText.text = LocalizationManager.Get("shop_pack_claimed");
                                else
                                    btnBuyText.text = LocalizationManager.Get($"shop_pack_{p}_price");
                            }
                        }
                    }

                    var tipTxt = recPanel.Find("RecTipCard/TipTxt")?.GetComponent<TMP_Text>();
                    if (tipTxt != null) tipTxt.text = LocalizationManager.Get("shop_rec_tip");
                }
            }

            // Update Mobile Settings Extra Controls
            UpdateHapticUI();
            if (btnPrivacyPolicy != null)
            {
                var pTxt = btnPrivacyPolicy.GetComponentInChildren<TMP_Text>();
                if (pTxt != null) pTxt.text = LocalizationManager.Get("settings_privacy_policy");
            }
            if (settingsModal != null)
            {
                var setCard = settingsModal.transform.Find("DialogCard");
                if (setCard != null)
                {
                    var mobileTitle = setCard.Find("MobileTitle")?.GetComponent<TMP_Text>();
                    if (mobileTitle != null)
                    {
                        mobileTitle.text = LocalizationManager.CurrentLanguage switch
                        {
                            GameLanguage.EN => "Mobile Support & Extras",
                            GameLanguage.JA => "モバイル＆便利機能",
                            GameLanguage.ZH => "移动端便利功能",
                            _ => "모바일 & 편의 기능 (Mobile Support)"
                        };
                    }
                    var btnProbSetTxt = setCard.Find("BtnProbSet")?.GetComponentInChildren<TMP_Text>();
                    if (btnProbSetTxt != null) btnProbSetTxt.text = LocalizationManager.Get("pickup_btn_rates");
                }
            }

            // Update Probability Modal Content
            if (probabilityModal != null)
            {
                var pCard = probabilityModal.transform.Find("DialogCard");
                if (pCard != null)
                {
                    var pTitle = pCard.Find("Title")?.GetComponent<TMP_Text>();
                    if (pTitle != null) pTitle.text = LocalizationManager.Get("pickup_btn_rates");

                    var lawTxt = pCard.Find("LawCard/LawTxt")?.GetComponent<TMP_Text>();
                    if (lawTxt != null) lawTxt.text = LocalizationManager.Get("prob_law_notice");

                    var thName = pCard.Find("TableHeader/ThName")?.GetComponent<TMP_Text>();
                    if (thName != null) thName.text = LocalizationManager.Get("prob_th_item");

                    var thType = pCard.Find("TableHeader/ThType")?.GetComponent<TMP_Text>();
                    if (thType != null) thType.text = LocalizationManager.Get("prob_th_type");

                    var thRate = pCard.Find("TableHeader/ThRate")?.GetComponent<TMP_Text>();
                    if (thRate != null) thRate.text = LocalizationManager.Get("prob_th_rate");

                    // Row 0: Special
                    var row0 = pCard.Find("Row_0");
                    if (row0 != null)
                    {
                        var r0Name = row0.Find("Name")?.GetComponent<TMP_Text>();
                        if (r0Name != null) r0Name.text = LocalizationManager.Get("prob_row_0_name");
                        var r0Type = row0.Find("Type")?.GetComponent<TMP_Text>();
                        if (r0Type != null) r0Type.text = LocalizationManager.Get("prob_row_0_type");
                    }
                    // Row 1 & 2: Common
                    var row1 = pCard.Find("Row_1");
                    if (row1 != null)
                    {
                        var r1Name = row1.Find("Name")?.GetComponent<TMP_Text>();
                        if (r1Name != null) r1Name.text = LocalizationManager.Get("prob_row_common");
                        var r1Type = row1.Find("Type")?.GetComponent<TMP_Text>();
                        if (r1Type != null) r1Type.text = LocalizationManager.Get("prob_type_shard_1");
                    }
                    var row2 = pCard.Find("Row_2");
                    if (row2 != null)
                    {
                        var r2Name = row2.Find("Name")?.GetComponent<TMP_Text>();
                        if (r2Name != null) r2Name.text = LocalizationManager.Get("prob_row_common");
                        var r2Type = row2.Find("Type")?.GetComponent<TMP_Text>();
                        if (r2Type != null) r2Type.text = LocalizationManager.Get("prob_type_shard_5");
                    }
                    // Row 3 & 4: Rare
                    var row3 = pCard.Find("Row_3");
                    if (row3 != null)
                    {
                        var r3Name = row3.Find("Name")?.GetComponent<TMP_Text>();
                        if (r3Name != null) r3Name.text = LocalizationManager.Get("prob_row_rare");
                        var r3Type = row3.Find("Type")?.GetComponent<TMP_Text>();
                        if (r3Type != null) r3Type.text = LocalizationManager.Get("prob_type_shard_1");
                    }
                    var row4 = pCard.Find("Row_4");
                    if (row4 != null)
                    {
                        var r4Name = row4.Find("Name")?.GetComponent<TMP_Text>();
                        if (r4Name != null) r4Name.text = LocalizationManager.Get("prob_row_rare");
                        var r4Type = row4.Find("Type")?.GetComponent<TMP_Text>();
                        if (r4Type != null) r4Type.text = LocalizationManager.Get("prob_type_shard_5");
                    }

                    // Enable autosizing on row names so they fit all languages without overlapping adjacent columns
                    for (int r = 0; r < 5; r++)
                    {
                        var rObj = pCard.Find($"Row_{r}");
                        if (rObj != null)
                        {
                            var rName = rObj.Find("Name")?.GetComponent<TMP_Text>();
                            if (rName != null)
                            {
                                rName.enableAutoSizing = true;
                                rName.fontSizeMin = 13f;
                                rName.fontSizeMax = (r == 0) ? 21f : 19f;
                            }
                        }
                    }

                    var totalLbl = pCard.Find("RowTotal/TotalLbl")?.GetComponent<TMP_Text>();
                    if (totalLbl != null) totalLbl.text = LocalizationManager.Get("prob_row_total");

                    var notesTxt = pCard.Find("NotesCard/NotesTxt")?.GetComponent<TMP_Text>();
                    if (notesTxt != null) notesTxt.text = LocalizationManager.Get("prob_notes_text");

                    var pConfirm = pCard.Find("BtnConfirm")?.GetComponentInChildren<TMP_Text>();
                    if (pConfirm != null) pConfirm.text = LocalizationManager.Get("help_confirm");
                }
            }

            // Update Lobby Logo based on language
            if (lobbyLogoImage != null && languageLogos != null && (int)lang >= 0 && (int)lang < languageLogos.Length)
            {
                if (languageLogos[(int)lang] != null)
                {
                    lobbyLogoImage.sprite = languageLogos[(int)lang];
                }
            }
        }

        public void SetupLanguageLogos(Image img, Sprite[] logos)
        {
            lobbyLogoImage = img;
            languageLogos = logos;
            if (lobbyLogoImage != null && languageLogos != null && (int)LocalizationManager.CurrentLanguage >= 0 && (int)LocalizationManager.CurrentLanguage < languageLogos.Length)
            {
                if (languageLogos[(int)LocalizationManager.CurrentLanguage] != null)
                {
                    lobbyLogoImage.sprite = languageLogos[(int)LocalizationManager.CurrentLanguage];
                }
            }
        }
    }
}
