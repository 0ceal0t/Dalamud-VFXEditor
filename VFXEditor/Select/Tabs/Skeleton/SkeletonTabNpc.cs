using System.Collections.Generic;
using VfxEditor.Select.Tabs.Npc;

namespace VfxEditor.Select.Tabs.Skeleton {
    public class SkeletonTabNpc : NpcTab {
        private readonly string Prefix;
        private readonly string Extension;

        public SkeletonTabNpc( SelectDialog dialog, string name, string prefix, string extension ) : base( dialog, name ) {
            Prefix = prefix;
            Extension = extension;
        }

        protected override void GetLoadedFiles( NpcFilesStruct files, out List<string> loaded ) {
            loaded = [];
            var path = Selected.GetSkeletonPath( Prefix, Extension );
            if( Dalamud.DataManager.FileExists( path ) ) loaded.Add( path );
        }

        protected override void DrawSelected() {
            Dialog.DrawPaths( Loaded, Selected.Name, SelectResultType.GameNpc );
        }
    }
}