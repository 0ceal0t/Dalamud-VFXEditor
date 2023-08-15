using System.Collections.Generic;
using VfxEditor.Select.Shared.Npc;

namespace VfxEditor.Select.Shared.Skeleton {
    public class SkeletonNpcTab : NpcTab {
        private readonly string Prefix;
        private readonly string Extension;

        public SkeletonNpcTab( SelectDialog dialog, string name, string prefix, string extension ) : base( dialog, name ) {
            Prefix = prefix;
            Extension = extension;
        }

        protected override void DrawSelected() {
            Dialog.DrawPaths( "Path", Loaded, SelectResultType.GameNpc, Selected.Name, true );
        }

        protected override void GetLoadedFiles( NpcFilesStruct files, out List<string> loaded ) {
            loaded = new();

            var path = $"chara/{Selected.PathPrefix}/{Selected.ModelString}/skeleton/base/b0001/{Prefix}_{Selected.ModelString}b0001.{Extension}";
            if( Plugin.DataManager.FileExists( path ) ) loaded.Add( path );
        }
    }
}
