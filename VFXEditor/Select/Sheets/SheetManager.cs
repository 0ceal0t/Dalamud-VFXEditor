using Dalamud.Data;
using Dalamud.Plugin;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VFXEditor.Select.Sheets;

namespace VFXEditor.Select {
    public class SheetManager {
        public static string NpcNamesOld { get; private set; }
        public static string NpcFiles { get; private set; }
        public static string MiscVfx { get; private set; }
        public static string NpcNames { get; private set; } 

        public static ItemSheetLoader Items { get; private set; }
        public static ActionSheetLoader Actions { get; private set; }
        public static CutsceneSheetLoader Cutscenes { get; private set; }
        public static EmoteSheetLoader Emotes { get; private set; }
        public static GimmickSheetLoader Gimmicks { get; private set; }
        public static NonPlayerActionSheetLoader NonPlayerActions { get; private set; }
        public static NpcSheetLoader Npcs { get; private set; }
        public static StatusSheetLoader Statuses { get; private set; }
        public static ZoneSheetLoader Zones { get; private set; }
        public static MountSheeetLoader Mounts { get; private set; }
        public static HousingSheetLoader Housing { get; private set; }
        public static CommonLoader Misc { get; private set; }
        public static ActionTmbSheetLoader ActionTmb { get; private set; }
        public static ActionPapSheetLoader ActionPap { get; private set; }
        public static EmoteTmbSheetLoader EmoteTmb { get; private set; }
        public static EmotePapSheetLoader EmotePap { get; private set; }

        public static void Initialize() {
            NpcNamesOld = Path.Combine( Plugin.RootLocation, "Files", "npc.csv" );
            NpcFiles = Path.Combine( Plugin.RootLocation, "Files", "npc_files.json" );
            MiscVfx = Path.Combine( Plugin.RootLocation, "Files", "vfx_misc.txt" );
            NpcNames = Path.Combine( Plugin.RootLocation, "Files", "npc_names.json" );

            Items = new ItemSheetLoader();
            Actions = new ActionSheetLoader();
            Cutscenes = new CutsceneSheetLoader();
            Emotes = new EmoteSheetLoader();
            Gimmicks = new GimmickSheetLoader();
            NonPlayerActions = new NonPlayerActionSheetLoader();
            Npcs = new NpcSheetLoader();
            Statuses = new StatusSheetLoader();
            Zones = new ZoneSheetLoader();
            Mounts = new MountSheeetLoader();
            Housing = new HousingSheetLoader();
            Misc = new CommonLoader();
            ActionTmb = new ActionTmbSheetLoader();
            ActionPap = new ActionPapSheetLoader();
            EmoteTmb = new EmoteTmbSheetLoader();
            EmotePap = new EmotePapSheetLoader();
        }

        public static Dictionary<string, string> FilterByExists( Dictionary<string, string> dict ) {
            return dict.Where( i => Plugin.DataManager.FileExists( i.Value ) ).ToDictionary( i => i.Key, i => i.Value );
        }
    }
}
