using ImGuiNET;
using System.Numerics;
using VFXEditor.UI;

namespace VFXEditor.Texture {
    public partial class TextureManager {
        public override void OnDraw() {
            var id = "##ImportTex";

            if( PathToTextureReplace.IsEmpty ) {
                ImGui.Text( "No Textures Have Been Imported..." );
                return;
            }

            ImGui.BeginChild( id + "/Child", new Vector2( 0, 0 ), true );
            ImGui.Columns( 3, id + "/Columns" );
            ImGui.SetColumnWidth( 0, ImGui.GetWindowContentRegionWidth() - 150 );
            ImGui.SetColumnWidth( 1, 75 );
            ImGui.SetColumnWidth( 2, 75 );

            foreach( var path in PathToTextureReplace.Keys ) {
                ImGui.Text( path );
            }

            ImGui.NextColumn();
            foreach( var item in PathToTextureReplace.Values ) {
                ImGui.Text( $"({item.Format})" );
            }

            var idx = 0;
            ImGui.NextColumn();
            foreach( var entry in PathToTextureReplace ) {
                if( UIUtils.RemoveButton( "Remove" + id + idx, small: true ) ) {
                    RemoveReplaceTexture( entry.Key );
                    RefreshPreviewTexture( entry.Key );
                }
                idx++;
            }

            ImGui.Columns( 1 );
            ImGui.EndChild();
        }
    }
}
