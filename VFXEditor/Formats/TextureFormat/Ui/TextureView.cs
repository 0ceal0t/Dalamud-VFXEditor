using ImGuiNET;
using Newtonsoft.Json.Linq;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.TextureFormat.Textures;
using VfxEditor.Ui.Components.SplitViews;
using VfxEditor.Utils;

namespace VfxEditor.Formats.TextureFormat.Ui {
    public class TextureView : SplitView<TextureReplace> {
        public readonly List<TextureReplace> Textures;

        public TextureView( List<TextureReplace> textures ) : base( "Textures" ) {
            Textures = textures;
            InitialWidth = 400;
        }

        public void WorkspaceImport( JObject meta, string loadLocation ) {
            var items = WorkspaceUtils.ReadFromMeta<WorkspaceMetaTex>( meta, "Tex" );
            if( items == null ) return;
            foreach( var item in items ) {
                var fullPath = WorkspaceUtils.ResolveWorkspacePath( item.RelativeLocation, Path.Combine( loadLocation, "Tex" ) );
                var newReplace = new TextureReplace( Plugin.TextureManager.NewWriteLocation, item );
                newReplace.ImportFile( fullPath );
                Textures.Add( newReplace );
            }
        }

        public void WorkspaceExport( Dictionary<string, string> meta, string saveLocation ) {
            var texRootPath = Path.Combine( saveLocation, "Tex" );
            Directory.CreateDirectory( texRootPath );

            var idx = 0;
            var texMeta = new List<WorkspaceMetaTex>();
            foreach( var texture in Textures ) {
                texMeta.Add( texture.WorkspaceExport( texRootPath, idx ) );
                idx++;
            }
            WorkspaceUtils.WriteToMeta( meta, texMeta.ToArray(), "Tex" );
        }

        // ==============

        protected override void DrawPreLeft() {
            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );
        }

        protected override void DrawLeftColumn() {
            for( var idx = 0; idx < Textures.Count; idx++ ) {
                using var _ = ImRaii.PushId( idx );
                var item = Textures[idx];
                if( ImGui.Selectable( item.GetExportReplace(), item == Selected ) ) Selected = item;
            }
        }

        protected override void DrawRightColumn() => Selected?.DrawBody();

        public void ClearSelected() { Selected = null; }
    }
}
