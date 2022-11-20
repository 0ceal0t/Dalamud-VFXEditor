using ImGuiNET;
using System;
using System.IO;
using System.Numerics;
using System.Text;

namespace VfxEditor.AvfxFormat {
    public class AvfxTexture : AvfxNode {
        public const string NAME = "Tex";

        public readonly AvfxString Path = new( "Path", "Path" );
        public readonly UiNodeGraphView NodeView;

        private string LastValue = "";

        public AvfxTexture() : base( NAME, UiNodeGroup.TextureColor ) {
            NodeView = new( this );
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            Path.SetValue( Encoding.ASCII.GetString( reader.ReadBytes( size ) ) );
            LastValue = Path.GetValue();
            Plugin.TextureManager.LoadPreviewTexture( Path.GetValue() );
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) => writer.Write( Encoding.ASCII.GetBytes( Path.GetValue() ) );

        public string LoadTex() {
            var currentPathValue = Path.GetValue();
            if( currentPathValue != LastValue ) {
                LastValue = currentPathValue;
                Plugin.TextureManager.LoadPreviewTexture( currentPathValue );
            }
            return currentPathValue;
        }

        public override void Draw( string parentId ) {
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

        public override string GetDefaultText() => $"Texture {GetIdx()}";

        public override string GetWorkspaceId() => $"Tex{GetIdx()}";
    }
}
