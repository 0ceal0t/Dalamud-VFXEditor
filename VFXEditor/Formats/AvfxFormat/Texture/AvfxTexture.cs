using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Formats.TextureFormat.Textures;
using VfxEditor.Library.Texture;
using VfxEditor.Parsing;

namespace VfxEditor.AvfxFormat {
    public class AvfxTexture : AvfxNode {
        public const string NAME = "Tex";

        public readonly AvfxString Path = new( "Path", "Path", false, false );
        public readonly UiNodeGraphView NodeView;

        public string PathTrimmed => Path.Value.Trim( '\0' );

        public AvfxTexture() : base( NAME, AvfxNodeGroupSet.TextureColor ) {
            NodeView = new( this );
        }

        public override void ReadContents( BinaryReader reader, int size ) => Path.ReadContents( reader, size );

        public override void WriteContents( BinaryWriter writer ) => Path.WriteContents( writer );

        protected override IEnumerable<AvfxBase> GetChildren() {
            yield break;
        }

        public override void Draw() {
            using var _ = ImRaii.PushId( "Texture" );
            NodeView.Draw();
            DrawRename();

            var preCombo = ImGui.GetCursorPosX();

            Plugin.LibraryManager.DrawTextureCombo( Path.Value, ( TextureLeaf texture ) => {
                if( texture.DrawSelectable() ) {
                    var newValue = texture.GetPath().Trim().Trim( '\0' );
                    CommandManager.Add( new ParsedSimpleCommand<string>( Path.Parsed, newValue ) );
                }
            } );

            var imguiStyle = ImGui.GetStyle();
            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( imguiStyle.ItemInnerSpacing.X, imguiStyle.ItemSpacing.Y ) ) ) {
                ImGui.SameLine();
                Path.Draw( ImGui.GetCursorPosX() - preCombo );
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            GetTexture()?.Draw();
        }

        public override void ShowTooltip() {
            ImGui.BeginTooltip();
            GetTexture()?.DrawFullImage();
            ImGui.EndTooltip();
        }

        public bool CanConvertToCustom() => Plugin.TextureManager.CanConvertToCustom( PathTrimmed );

        public void ConvertToCustom( List<ICommand> commands ) {
            if( Plugin.TextureManager.ConvertToCustom( PathTrimmed, out var newPath ) ) {
                commands.Add( new ParsedSimpleCommand<string>( Path.Parsed, Path.Value, newPath ) );
            }
        }

        public bool FileExists() => Plugin.TextureManager.FileExists( PathTrimmed );

        public TextureDrawable GetTexture() => Plugin.TextureManager.GetTexture( Path.Value );

        public override string GetDefaultText() => $"Texture {GetIdx()}";

        public override string GetWorkspaceId() => $"Tex{GetIdx()}";
    }
}
