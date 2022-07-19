using Dalamud.Logging;
using ImGuiFileDialog;
using ImGuiNET;
using System;
using System.IO;
using System.Numerics;
using VFXEditor.AVFXLib.Texture;
using VFXEditor.Helper;

namespace VFXEditor.AVFX.VFX {
    public class UITexture : UINode {
        public readonly AVFXTexture Texture;
        public readonly UIString Path;
        public readonly UINodeGraphView NodeView;

        private string LastValue;

        public UITexture( AVFXTexture texture ) : base( UINodeGroup.TextureColor, false ) {
            Texture = texture;
            NodeView = new UINodeGraphView( this );

            Path = new UIString( "Path", Texture.Path );
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

        public override void DrawBody( string parentId ) {
            var id = parentId + "/Texture";
            NodeView.Draw( id );
            DrawRename( id );
            Path.Draw( id );

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
