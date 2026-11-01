using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace VfxEditor.Data {
    public static class SheetData {
        // ==== UI COLORS =====

        private static bool UiColorsInitialized = false;
        public static readonly Dictionary<uint, Vector4> UiColors = [];

        public static void InitUiColors() {
            if( UiColorsInitialized ) return;
            UiColorsInitialized = true;

            foreach( var item in Dalamud.DataManager.GetExcelSheet<UIColor>() ) {
                var bytes = BitConverter.GetBytes( item.Dark );
                UiColors[item.RowId] = new( bytes[3] / 255f, bytes[2] / 255f, bytes[1] / 255f, bytes[0] / 255f );
            }
        }

        // ==== WEAPON TIMELINES ====

        private static bool WeaponTimelinesInitialized = false;
        public static readonly Dictionary<ushort, string> WeaponTimelines = [];

        public static void InitWeaponTimelines() {
            if( WeaponTimelinesInitialized ) return;
            WeaponTimelinesInitialized = true;

            foreach( var item in Dalamud.DataManager.GetExcelSheet<WeaponTimeline>() ) {
                WeaponTimelines[( ushort )item.RowId] = item.File.ToString();
            }
        }

        // ==== BGM ID ====

        private static bool BgmIdInitialized = false;
        public static readonly Dictionary<int, string> BgmId = [];

        public static void InitBgmId() {
            if( BgmIdInitialized ) return;
            BgmIdInitialized = true;

            foreach( var item in Dalamud.DataManager.GetExcelSheet<BGM>() ) {
                BgmId[( int )item.RowId] = item.File.ToString();
            }
        }

        // ==== NPC NAME ====

        private static bool NpcNameInitialized = false;
        public static readonly Dictionary<int, string> NpcName = [];

        public static void InitNpcName() {
            if( NpcNameInitialized ) return;
            NpcNameInitialized = true;

            foreach( var item in Dalamud.DataManager.GetExcelSheet<BNpcName>() ) {
                NpcName[( int )item.RowId] = item.Singular.ToString();
            }
        }

        // ==== SUBTITLES ====

        private static bool SubtitleInitialized = false;
        public static readonly Dictionary<int, string> Subtitle = [];

        public static void InitSubtitle() {
            if( SubtitleInitialized ) return;
            SubtitleInitialized = true;

            foreach( var item in Dalamud.DataManager.GetExcelSheet<InstanceContentTextData>() ) {
                Subtitle[( int )item.RowId] = item.Text.ToString();
            }
        }

        // ==== MOTION TIMELINES ====

        private static bool MotionTimelinesInitialized = false;
        public static readonly Dictionary<string, MotionTimelineData> MotionTimelines = [];

        public struct MotionTimelineData {
            public int Group;
            public bool Loop;
            public bool Blink;
            public bool Lip;
        }

        public static void InitMotionTimelines() {
            if( MotionTimelinesInitialized ) return;
            MotionTimelinesInitialized = true;

            foreach( var item in Dalamud.DataManager.GetExcelSheet<MotionTimeline>() ) {
                MotionTimelines[item.Filename.ToString()] = new MotionTimelineData() {
                    Group = item.BlendGroup,
                    Loop = item.IsLoop,
                    Blink = item.IsBlinkEnable,
                    Lip = item.IsLipEnable
                };
            }
        }
    }
}
