using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace VfxEditor.Data {
    public static class SheetData {
        // ==== UI COLORS =====

        private static bool UiColorsInitialized = false;
        public static readonly Dictionary<uint, Vector4> UiColors = new();

        public static void InitUiColors() {
            if( UiColorsInitialized ) return;
            UiColorsInitialized = true;

            foreach( var item in Dalamud.DataManager.GetExcelSheet<UIColor>() ) {
                var bytes = BitConverter.GetBytes( item.UIForeground );
                UiColors[item.RowId] = new( bytes[3] / 255f, bytes[2] / 255f, bytes[1] / 255f, bytes[0] / 255f );
            }
        }

        // ==== WEAPON TIMELINES ====

        private static bool WeaponTimelinesInitialized = false;
        public static readonly Dictionary<ushort, string> WeaponTimelines = new();

        public static void InitWeaponTimelines() {
            if( WeaponTimelinesInitialized ) return;
            WeaponTimelinesInitialized = true;

            foreach( var item in Dalamud.DataManager.GetExcelSheet<WeaponTimeline>() ) {
                WeaponTimelines[( ushort )item.RowId] = item.File.ToString();
            }
        }

        // ==== MOTION TIMELINES ====

        private static bool MotionTimelinesInitialized = false;
        public static readonly Dictionary<string, MotionTimelineData> MotionTimelines = new();

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
