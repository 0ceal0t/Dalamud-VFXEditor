using System.Collections.Generic;

namespace VfxEditor.Select.Tabs.Emotes {
    public class EmoteTabTmb : EmoteTab<List<string>> {
        public EmoteTabTmb( SelectDialog dialog, string name ) : base( dialog, name ) { }

        public override void LoadSelection( EmoteRow item, out List<string> loaded ) {
            loaded = item.TmbFiles;
        }

        protected override void DrawSelected() {
            DrawIcon( Selected.Icon );
            DrawPaths( "Path", Loaded, Selected.Name );
        }
    }
}