using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Numerics;
using VfxEditor.Library.Components;
using VfxEditor.Utils;

namespace VfxEditor.Library.Texture {
    public class TextureLeaf : LibraryLeaf {
        private string Path;

        public TextureLeaf( LibraryFolder parent, string name, string id, string path, Vector4 color ) : base( parent, name, id, color ) {
            Path = path;
        }

        public TextureLeaf( LibraryFolder parent, LibraryProps props ) : this( parent, props.Name, props.Id, props.Path, props.Color ) { }

        public override LibraryProps ToProps() => new() {
            Name = Name,
            Id = Id,
            Path = Path,
            Color = Color,
            PropType = ItemType.Texture
        };

        public override void Cleanup() { }

        protected override void DrawTooltip() { }

        protected override void DrawEditing() {
            ImGui.InputText( "Name", ref Name, 255 );
            ImGui.InputText( "Path", ref Path, 255 );
            ImGui.ColorEdit4( "Color", ref Color, ImGuiColorEditFlags.DisplayHex | ImGuiColorEditFlags.AlphaPreviewHalf | ImGuiColorEditFlags.AlphaBar );
        }

        protected override void DrawImport() {
            if( UiUtils.IconSelectable( FontAwesomeIcon.Clipboard, "Copy" ) ) ImGui.SetClipboardText( Path );
        }

        protected override void DrawBody() {
            using var indent = ImRaii.PushIndent( 5f );
            Plugin.TextureManager.GetTexture( Path )?.DrawImage( 70 );

            if( ImGui.IsItemHovered() ) {
                ImGui.BeginTooltip();
                Plugin.TextureManager.GetTexture( Path )?.DrawFullImage();
                ImGui.EndTooltip();
            }
        }

        public bool DrawSelectable() {
            var ret = ImGui.Selectable( $"{NameOrNone}##{Id}" );

            if( ImGui.IsItemHovered() ) {
                ImGui.BeginTooltip();
                Plugin.TextureManager.GetTexture( Path )?.DrawFullImage();
                ImGui.EndTooltip();
            }

            return ret;
        }

        public string GetPath() => Path;

        public string GetName() => Name;
    }
}
