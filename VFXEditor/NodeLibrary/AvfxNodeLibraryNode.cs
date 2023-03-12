using System;
using System.Numerics;
using System.IO;
using ImGuiNET;
using VfxEditor.Utils;
using Dalamud.Interface;

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
            var id = $"##NodeLibrary{Id}";
            var uniqueId = $"###NodeLibrary{Id}";
            var listModified = false;

            ImGui.PushStyleColor( ImGuiCol.Header, Color );
            ImGui.PushStyleColor( ImGuiCol.HeaderHovered, Color * 0.75f );
            var open = ImGui.CollapsingHeader( $"{Name}{uniqueId}" );
            DragDrop( library, ref listModified );
            ImGui.PopStyleColor( 2 );
            if( ImGui.IsItemClicked( ImGuiMouseButton.Right ) ) ImGui.OpenPopup( $"{id}-context" );

            if( ImGui.BeginPopup( $"{id}-context" ) ) {
                if( ImGui.Selectable( $"Import{id}" ) && Plugin.AvfxManager.CurrentFile != null ) Plugin.AvfxManager.Import( Path );

                if( ImGui.Selectable( $"Edit{id}" ) ) {
                    Editing = true;
                }
                if( ImGui.Selectable( $"Delete{id}" ) ) {
                    Cleanup();
                    Parent.Remove( this );
                    library.Save();
                    listModified = true;
                }
                ImGui.EndPopup();
            }

            if( open ) {
                ImGui.Indent();

                ImGui.PushFont( UiBuilder.IconFont );
                ImGui.PushStyleVar( ImGuiStyleVar.ItemSpacing, new Vector2( 3, 4 ) );

                if( UiUtils.DisabledButton( $"{( char )FontAwesomeIcon.Download}{id}", Plugin.AvfxManager.CurrentFile != null ) ) {
                    Plugin.AvfxManager.Import( Path );
                }

                ImGui.SameLine();
                if( ImGui.Button( Editing ? $"{( char )FontAwesomeIcon.Save}{id}" : $"{( char )FontAwesomeIcon.Edit}{id}" ) ) {
                    Editing = !Editing;
                    if( !Editing ) { // done editing
                        library.Save();
                        listModified = true;
                    }
                }
                ImGui.SameLine();
                if( UiUtils.RemoveButton( $"{( char )FontAwesomeIcon.Trash}{id}" ) ) {
                    Cleanup();
                    Parent.Remove( this );
                    library.Save();
                    listModified = true;
                }

                ImGui.PopStyleVar( 1 );
                ImGui.PopFont();

                if( !Editing ) {
                    if( string.IsNullOrEmpty( Description ) ) ImGui.TextDisabled( "No description" );
                    else ImGui.TextWrapped( Description );
                }
                else {
                    var preX = ImGui.GetCursorPosX();
                    ImGui.InputText( $"Name{id}", ref Name, 255 );
                    var w = ImGui.GetCursorPosX() - preX;
                    ImGui.ColorEdit4( $"Color{id}", ref Color, ImGuiColorEditFlags.DisplayHex | ImGuiColorEditFlags.AlphaPreviewHalf | ImGuiColorEditFlags.AlphaBar );
                    ImGui.InputTextMultiline( $"Description{id}", ref Description, 1000, new Vector2( w, 100 ) );
                }

                ImGui.Unindent();
            }

            return listModified;
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
