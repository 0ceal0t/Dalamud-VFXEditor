using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Interface;
using Dalamud.Plugin;
using ImGuiNET;
using VFXEditor.Data.Texture;
using VFXEditor.UI.VFX;

namespace VFXEditor.UI {
    public class TextureDialog : GenericDialog {
        public TextureDialog( Plugin plugin ) : base( plugin, "Imported Textures" ) {
        }

        public override void OnDraw() {
            var id = "##ImportTex";

            if( TextureManager.Manager.PathToTextureReplace.Count == 0 ) {
                ImGui.Text( "No Textures Have Been Imported..." );
                return;
            }

            ImGui.BeginChild( id + "/Child", new Vector2( 0, 0 ), true );
            ImGui.Columns( 3, id + "/Columns" );
            ImGui.SetColumnWidth( 0, ImGui.GetWindowContentRegionWidth() - 150 );
            ImGui.SetColumnWidth( 1, 75 );
            ImGui.SetColumnWidth( 2, 75 );

            foreach(var path in TextureManager.Manager.PathToTextureReplace.Keys ) {
                ImGui.Text( path );
            }

            ImGui.NextColumn();
            foreach( var item in TextureManager.Manager.PathToTextureReplace.Values ) {
                ImGui.Text( $"({item.Format})" );
            }

            var idx = 0;
            ImGui.NextColumn();
            foreach( var entry in TextureManager.Manager.PathToTextureReplace ) {
                if(UIUtils.RemoveButton("Remove" + id + idx, small: true ) ) {
                    TextureManager.Manager.RemoveReplaceTexture( entry.Key );
                    TextureManager.Manager.RefreshPreviewTexture( entry.Key );
                }
                idx++;
            }

            ImGui.Columns( 1 );
            ImGui.EndChild();
        }
    }
}
