using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXSelect.Data.Sheets;

namespace VFXSelect {
    public class SheetManager {
        public DalamudPluginInterface _pi;
        public string NpcCsv;

        public ItemSheetLoader _Items;
        public ActionSheetLoader _Actions;
        public CutsceneSheetLoader _Cutscenes;
        public EmoteSheetLoader _Emotes;
        public GimmickSheetLoader _Gimmicks;
        public NonPlayerActionSheetLoader _NonPlayerActions;
        public NpcSheetLoader _Npcs;
        public StatusSheetLoader _Statuses;
        public ZoneSheetLoader _Zones;
        public MountSheeetLoader _Mounts;
        public HousingSheetLoader _Housing;

        public SheetManager(DalamudPluginInterface pi, string npcCsv ) {
            _pi = pi;
            NpcCsv = npcCsv;

            _Items = new ItemSheetLoader( this, pi );
            _Actions = new ActionSheetLoader( this, pi );
            _Cutscenes = new CutsceneSheetLoader( this, pi );
            _Emotes = new EmoteSheetLoader( this, pi );
            _Gimmicks = new GimmickSheetLoader( this, pi );
            _NonPlayerActions = new NonPlayerActionSheetLoader( this, pi );
            _Npcs = new NpcSheetLoader( this, pi );
            _Statuses = new StatusSheetLoader( this, pi );
            _Zones = new ZoneSheetLoader( this, pi );
            _Mounts = new MountSheeetLoader( this, pi );
            _Housing = new HousingSheetLoader( this, pi );
        }
    }
}
