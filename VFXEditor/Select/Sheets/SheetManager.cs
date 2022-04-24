using Dalamud.Data;
using Dalamud.Plugin;
using System.Collections.Generic;
using System.Linq;
using VFXSelect.Select.Sheets;

namespace VFXSelect {
    public class SheetManager {
        public static string NpcCsv { get; private set; }
        public static string MonsterJson { get; private set; }
        public static string MiscVfxTxt { get; private set; }

        public static DataManager DataManager { get; private set; }
        public static DalamudPluginInterface PluginInterface { get; private set; }

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

        public static void Initialize( string npcCsv, string monsterJson, string miscVfxTxt, DataManager dataManager, DalamudPluginInterface pluginInterface ) {
            NpcCsv = npcCsv;
            MonsterJson = monsterJson;
            MiscVfxTxt = miscVfxTxt;
            DataManager = dataManager;
            PluginInterface = pluginInterface;

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
            return dict.Where( i => DataManager.FileExists( i.Value ) ).ToDictionary( i => i.Key, i => i.Value );
        }
    }
}
