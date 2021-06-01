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

        public SheetManager(DalamudPluginInterface pluginInterface, string npcCsv ) {
            PluginInterface = pluginInterface;
            NpcCsv = npcCsv;

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
        }
    }
}
