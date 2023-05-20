using Dalamud.Interface;
using ImGuiNET;
using OtterGui.Raii;
using System.IO;
using System.Numerics;
using VfxEditor.Utils;

namespace VfxEditor.NodeLibrary {
    public class AvfxNodeLibraryNode : AvfxNodeLibraryGeneric {
        private string Name;
        private readonly string Id;
        private readonly string Path;
        private string Description;
        private Vector4 Color;

        private bool Editing = false;

        public AvfxNodeLibraryNode( AvfxNodeLibraryFolder parent, string name, string id, string path, string description, Vector4 color ) : base( parent ) {
            Name = name;
            Id = id;
            Path = path;
            Description = description;
            Color = color;
        }

        public AvfxNodeLibraryNode( AvfxNodeLibraryFolder parent, AvfxNodeLibraryProps props ) : this( parent, props.Name, props.Id, props.Path, props.Description, props.Color ) { }

        public override bool Draw( AvfxNodeLibrary library, string searchInput ) {
            using var _ = ImRaii.PushId( Id );

            var listModified = false;

            ImGui.PushStyleColor( ImGuiCol.Header, Color );
            ImGui.PushStyleColor( ImGuiCol.HeaderHovered, Color * 0.75f );
            var open = ImGui.CollapsingHeader( Name );
            DragDrop( library, Name, ref listModified );
            ImGui.PopStyleColor( 2 );

            if( DrawPopup( library ) ) listModified = true;

            if( !open ) return listModified;

            // =========== Open ===========

            using var indent = ImRaii.PushIndent();

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) )
            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 3, 4 ) ) ) {
                if( UiUtils.DisabledButton( FontAwesomeIcon.Download.ToIconString(), Plugin.AvfxManager.CurrentFile != null ) ) Plugin.AvfxManager.Import( Path );

                ImGui.SameLine();
                if( ImGui.Button( Editing ? FontAwesomeIcon.Save.ToIconString() : FontAwesomeIcon.PencilAlt.ToIconString() ) ) {
                    Editing = !Editing;
                    if( !Editing ) { // done editing
                        library.Save();
                        listModified = true;
                    }
                }

                ImGui.SameLine();
                if( UiUtils.RemoveButton( FontAwesomeIcon.Trash.ToIconString() ) ) {
                    Cleanup();
                    Parent.Remove( this );
                    library.Save();
                    listModified = true;
                }
            }

            if( !Editing ) {
                if( string.IsNullOrEmpty( Description ) ) ImGui.TextDisabled( "No description" );
                else ImGui.TextWrapped( Description );
            }
            else {
                var preX = ImGui.GetCursorPosX();
                ImGui.InputText( "Name", ref Name, 255 );
                var w = ImGui.GetCursorPosX() - preX;
                ImGui.ColorEdit4( "Color", ref Color, ImGuiColorEditFlags.DisplayHex | ImGuiColorEditFlags.AlphaPreviewHalf | ImGuiColorEditFlags.AlphaBar );
                ImGui.InputTextMultiline( "Description", ref Description, 1000, new Vector2( w, 100 ) );
            }

            return listModified;
        }

        private bool DrawPopup( AvfxNodeLibrary library ) {
            if( ImGui.IsItemClicked( ImGuiMouseButton.Right ) ) ImGui.OpenPopup( "Popup" );

            using var popup = ImRaii.Popup( "Popup" );
            if( !popup ) return false;

            var importDisabled = Plugin.AvfxManager.CurrentFile == null;
            using( var disabled = ImRaii.Disabled( importDisabled ) ) {
                if( UiUtils.IconSelectable( FontAwesomeIcon.Download, "Import" ) && !importDisabled ) Plugin.AvfxManager.Import( Path );
            }

            if( UiUtils.IconSelectable( FontAwesomeIcon.Trash, "Delete" ) ) {
                Cleanup();
                Parent.Remove( this );
                library.Save();
                return true;
            }

            if( ImGui.InputText( "##Rename", ref Name, 128, ImGuiInputTextFlags.AutoSelectAll ) ) {
                library.Save();
                return true;
            }

            return false;
        }

        public override bool Matches( string input ) {
            if( string.IsNullOrEmpty( input ) ) return true;
            if( Name.ToLower().Contains( input.ToLower() ) ) return true;
            return false;
        }

        public override AvfxNodeLibraryProps ToProps() => new() {
            Name = Name,
            Id = Id,
            Path = Path,
            Description = Description,
            Color = Color,
            PropType = AvfxNodeLibraryProps.Type.Node
        };

        public override void Cleanup() {
            if( File.Exists( Path ) ) File.Delete( Path );
        }

        public override bool Contains( AvfxNodeLibraryGeneric item ) => false;
    }
}
