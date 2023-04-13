using ImGuiNET;
using System;
using System.IO;
using System.Numerics;
using System.Text;
using VfxEditor.AvfxFormat.Nodes;

namespace VfxEditor.AvfxFormat {
    public class AvfxTexture : AvfxNode {
        public const string NAME = "Tex";

        public readonly AvfxString Path = new( "Path", "Path" );
        public readonly UiNodeGraphView NodeView;

        private string LoadedTexturePath = "";

        public AvfxTexture() : base( NAME, AvfxNodeGroupSet.TextureColor ) {
            NodeView = new( this );
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            Path.SetValue( Encoding.ASCII.GetString( reader.ReadBytes( size ) ) );
            UpdateTexture();
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) => writer.Write( Encoding.ASCII.GetBytes( Path.GetValue() ) );

        public string UpdateTexture() {
            if( Path.GetValue() != LoadedTexturePath ) {
                LoadedTexturePath = Path.GetValue();
                Plugin.TextureManager.LoadPreviewTexture( Path.GetValue() );
            }
            return Path.GetValue();
        }

        public override void Draw( string parentId ) {
            var id = parentId + "/Texture";
            NodeView.Draw( id );
            DrawRename( id );

            Path.Draw( id );
            Plugin.TextureManager.DrawTexture( UpdateTexture(), id );
        }

        public override void ShowTooltip() {
            if( Plugin.TextureManager.GetPreviewTexture( UpdateTexture(), out var t ) ) {
                ImGui.BeginTooltip();
                ImGui.Image( t.Wrap.ImGuiHandle, new Vector2( t.Width, t.Height ) );
                ImGui.EndTooltip();
            }
        }

        public override string GetDefaultText() => $"Texture {GetIdx()}";

        public override string GetWorkspaceId() => $"Tex{GetIdx()}";
    }
}
