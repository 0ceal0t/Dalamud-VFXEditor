using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Select.Tabs.Npc {
    public class NpcTabMdl : NpcTab {
        public NpcTabMdl( SelectDialog dialog, string name ) : base( dialog, name ) { }

        protected override void GetLoadedFiles( NpcFilesStruct files, out List<string> loaded ) {
            loaded = Selected.IsMonster ?
                [Selected.GetMdlPath( "" )] :
                [.. new List<string>() {
                    Selected.GetMdlPath( "met" ),
                    Selected.GetMdlPath( "glv" ),
                    Selected.GetMdlPath( "dwn" ),
                    Selected.GetMdlPath( "sho" ),
                    Selected.GetMdlPath( "top" )
                }.Where( Dalamud.DataManager.FileExists )];
        }
    }
}
