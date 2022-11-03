using Dalamud.Logging;
using ImGuiFileDialog;
using ImGuiNET;
using System;
using System.IO;
using System.Numerics;
using VfxEditor.AVFXLib.Texture;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiTexture : UiNode {
        public readonly AVFXTexture Texture;
        public readonly UiString Path;
        public readonly UiNodeGraphView NodeView;

        private string LastValue;

        public UiTexture( AVFXTexture texture ) : base( UiNodeGroup.TextureColor, false ) {
            Texture = texture;
            NodeView = new UiNodeGraphView( this );

            Path = new UiString( "Path", Texture.Path );
            LastValue = Texture.Path.GetValue();
            Plugin.TextureManager.LoadPreviewTexture( Texture.Path.GetValue() );
            HasDependencies = false; // if imported, all set now
        }

        public string LoadTex() {
            var currentPathValue = Path.Literal.GetValue();
            if( currentPathValue != LastValue ) {
                LastValue = currentPathValue;
                Plugin.TextureManager.LoadPreviewTexture( currentPathValue );
            }
            return currentPathValue;
        }

        public override void DrawInline( string parentId ) {
            var id = parentId + "/Texture";
            NodeView.DrawInline( id );
            DrawRename( id );
            Path.DrawInline( id );

            var currentPathValue = LoadTex();

            Plugin.TextureManager.DrawTexture( currentPathValue, id );
        }

        public override void ShowTooltip() {
            var currentPathValue = LoadTex();

            if( Plugin.TextureManager.GetPreviewTexture( currentPathValue, out var t ) ) {
                ImGui.BeginTooltip();
                ImGui.Image( t.Wrap.ImGuiHandle, new Vector2( t.Width, t.Height ) );
                ImGui.EndTooltip();
            }
        }

        public override string GetDefaultText() => $"Texture {Idx}";

        public override string GetWorkspaceId() => $"Tex{Idx}";

        public override void Write( BinaryWriter writer ) => Texture.Write( writer );
    }
}
