using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.IO;
using System.Numerics;
using VfxEditor.Library.Components;
using VfxEditor.Utils;

namespace VfxEditor.Library.Node {
    public class NodeLeaf : LibraryLeaf {
        private readonly string Path;
        private string Description;

        public NodeLeaf( LibraryFolder parent, string name, string id, string path, string description, Vector4 color ) : base( parent, name, id, color ) {
            Path = path;
            Description = description;
        }

        public NodeLeaf( LibraryFolder parent, LibraryProps props ) : this( parent, props.Name, props.Id, props.Path, props.Description, props.Color ) { }

        public override LibraryProps ToProps() => new() {
            Name = Name,
            Id = Id,
            Path = Path,
            Description = Description,
            Color = Color,
            PropType = ItemType.Node
        };

        public override void Cleanup() {
            if( File.Exists( Path ) ) File.Delete( Path );
        }

        protected override void DrawTooltip() {
            if( !string.IsNullOrEmpty( Description ) ) UiUtils.Tooltip( Description );
        }

        protected override void DrawEditing() {
            var preX = ImGui.GetCursorPosX();
            ImGui.InputText( "Name", ref Name, 255 );
            var w = ImGui.GetCursorPosX() - preX;
            ImGui.ColorEdit4( "Color", ref Color, ImGuiColorEditFlags.DisplayHex | ImGuiColorEditFlags.AlphaPreviewHalf | ImGuiColorEditFlags.AlphaBar );
            ImGui.InputTextMultiline( "Description", ref Description, 1000, new Vector2( w, 100 ) );
        }

        protected override void DrawImport() {
            var importDisabled = Plugin.AvfxManager.File == null;
            using var disabled = ImRaii.Disabled( importDisabled );
            if( UiUtils.IconSelectable( FontAwesomeIcon.Download, "Import" ) && !importDisabled ) Plugin.AvfxManager.Import( Path );
        }

        protected override void DrawBody() { }
    }
}
