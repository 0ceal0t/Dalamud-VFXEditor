using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXSelect.Data.Sheets;

namespace VFXSelect {
    public class SheetManager {
        public DalamudPluginInterface PluginInterface;
        public string NpcCsv;
        public string MonsterJson;

        public ItemSheetLoader Items;
        public ActionSheetLoader Actions;
        public CutsceneSheetLoader Cutscenes;
        public EmoteSheetLoader Emotes;
        public GimmickSheetLoader Gimmicks;
        public NonPlayerActionSheetLoader NonPlayerActions;
        public NpcSheetLoader Npcs;
        public StatusSheetLoader Statuses;
        public ZoneSheetLoader Zones;
        public MountSheeetLoader Mounts;
        public HousingSheetLoader Housing;
        public CommonLoader Misc;

        public SheetManager( DalamudPluginInterface pluginInterface, string npcCsv, string monsterJson ) {
            PluginInterface = pluginInterface;
            NpcCsv = npcCsv;
            MonsterJson = monsterJson;

            Items = new ItemSheetLoader( this, PluginInterface );
            Actions = new ActionSheetLoader( this, PluginInterface );
            Cutscenes = new CutsceneSheetLoader( this, PluginInterface );
            Emotes = new EmoteSheetLoader( this, PluginInterface );
            Gimmicks = new GimmickSheetLoader( this, PluginInterface );
            NonPlayerActions = new NonPlayerActionSheetLoader( this, PluginInterface );
            Npcs = new NpcSheetLoader( this, PluginInterface );
            Statuses = new StatusSheetLoader( this, PluginInterface );
            Zones = new ZoneSheetLoader( this, PluginInterface );
            Mounts = new MountSheeetLoader( this, PluginInterface );
            Housing = new HousingSheetLoader( this, PluginInterface );
            Misc = new CommonLoader( this, PluginInterface );
        }
    }
}
