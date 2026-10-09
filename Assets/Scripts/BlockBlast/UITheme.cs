using UnityEngine;

namespace BlockBlast
{
    /// <summary>
    /// 말랑 캔디 플랫 디자인 토큰. 모든 UI 색/크기는 여기서만 정의한다.
    /// 명세: Docs/UI_REDESIGN_PROMPT.md
    /// </summary>
    public static class UITheme
    {
        // --- Colors ---
        public static readonly Color Dim        = Hex(0x14092A, 0.82f);
        public static readonly Color Card       = Hex(0xFFFFFF);
        public static readonly Color CardSoft   = Hex(0xF4EEFC);
        public static readonly Color CardBorder = Hex(0xD9C8F2);
        public static readonly Color TextMain   = Hex(0x3A2455);
        public static readonly Color TextSub    = Hex(0x8C7AA6);
        public static readonly Color Pink       = Hex(0xFF5C8A); // 메인 CTA
        public static readonly Color Mint       = Hex(0x2ED3A4); // 성장/긍정
        public static readonly Color Purple     = Hex(0x9B6BFF); // 보조 CTA
        public static readonly Color Gold       = Hex(0xFFB800); // 재화/별
        public static readonly Color Blue       = Hex(0x4C8DFF); // 시간 스탯
        public static readonly Color Locked     = Hex(0xC9C4D1);
        public static readonly Color GaugeTrack = Hex(0xEEE8F8);

        // --- Font sizes (최소 20) ---
        public const int FontTitle   = 40;
        public const int FontSection = 28;
        public const int FontBody    = 24;
        public const int FontValue   = 28;
        public const int FontSub     = 20;
        public const int FontButton  = 30;

        // --- Sizes ---
        public const float CardRadius    = 40f;
        public const float BorderWidth   = 4f;
        public const float CardPadding   = 32f;
        public const float ElementGap    = 20f;
        public const float ButtonMinW    = 260f;
        public const float ButtonH       = 96f;
        public const float GaugeH        = 22f;
        public const float CloseBtnSize  = 56f;

        private static Color Hex(int rgb, float a = 1f)
        {
            return new Color(((rgb >> 16) & 0xFF) / 255f, ((rgb >> 8) & 0xFF) / 255f, (rgb & 0xFF) / 255f, a);
        }
    }
}
