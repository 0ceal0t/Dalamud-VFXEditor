using System.Collections.Generic;
using VfxEditor.Select.Shared.Npc;

namespace VfxEditor.Select.Phyb.Npc {
    public class NpcPhybTab : NpcTab {
        public NpcPhybTab( SelectDialog dialog, string name ) : base( dialog, name ) { }

        protected override void DrawSelected() {
            Dialog.DrawPaths( "Path", Loaded, SelectResultType.GameNpc, Selected.Name, true );
        }

        protected override void GetLoadedFiles( NpcFilesStruct files, out List<string> loaded ) {
            loaded = new();

            // chara/monster/m0805/skeleton/base/b0001/phy_m0805b0001.phyb

            var path = $"chara/{Selected.PathPrefix}/{Selected.ModelString}/skeleton/base/b0001/phy_{Selected.ModelString}b0001.phyb";
            if( Plugin.DataManager.FileExists( path ) ) {
                loaded.Add( path );
            }
        }
    }
}
