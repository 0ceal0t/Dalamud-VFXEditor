using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VfxEditor.Utils;

namespace VfxEditor.NodeLibrary {
    public unsafe class AvfxNodeLibraryFolder : AvfxNodeLibraryGeneric {
        public readonly List<AvfxNodeLibraryGeneric> Children = new();
        public bool IsRoot => Parent == null;
        private string Name;
        private readonly string Id;

        private bool Editing = false;
        private string EditingName = "";

        public AvfxNodeLibraryFolder( AvfxNodeLibraryFolder parent, string name, string id, List<AvfxNodeLibraryProps> items ) : base( parent ) {
            Name = name;
            Id = id;

            foreach( var item in items ) {
                if( item.PropType == AvfxNodeLibraryProps.Type.Folder ) Children.Add( new AvfxNodeLibraryFolder( this, item ) );
                else Children.Add( new AvfxNodeLibraryNode( this, item ) );
            }
        }

        public AvfxNodeLibraryFolder( AvfxNodeLibraryFolder parent, AvfxNodeLibraryProps props ) : this( parent, props.Name, props.Id, props.Children ) { }

        public List<AvfxNodeLibraryProps> ChildrenToProps() => Children.Select( x => x.ToProps() ).ToList();

        public void Add( AvfxNodeLibraryGeneric item ) {
            item.Parent = this;
            Children.Add( item );
        }

        public void Remove( AvfxNodeLibraryGeneric item ) {
            if( item.Parent == this ) item.Parent = null;
            Children.Remove( item );
        }

        public override bool Draw( AvfxNodeLibrary library, string searchInput ) {
            var id = $"##NodeLibrary{Id}";
            var uniqueId = $"###NodeLibrary{Id}";
            var listModified = false;

            var open = true;
            if( !IsRoot ) {
                // So that you can drag an item BEFORE a folder, rather than only inside of it

                ImGui.PushStyleVar( ImGuiStyleVar.ItemSpacing, new Vector2( 0 ) );
                ImGui.PushStyleVar( ImGuiStyleVar.FramePadding, new Vector2( 0 ) );

                ImGui.BeginChild( $"{id}-child", new Vector2( ImGui.GetContentRegionAvail().X, 1 ), false );
                ImGui.EndChild();

                if( ImGui.BeginDragDropTarget() ) {
                    if( library.StopDragging( this, overridePosition: true ) ) listModified = true;
                    ImGui.EndDragDropTarget();
                }
                ImGui.PopStyleVar( 2 );

                // Main folder item

                ImGui.PushStyleColor( ImGuiCol.Header, new Vector4( 0 ) );
                open = ImGui.TreeNodeEx( $"{uniqueId}",
                    ( Editing ? ImGuiTreeNodeFlags.None : ImGuiTreeNodeFlags.SpanAvailWidth ) |
                    ImGuiTreeNodeFlags.FramePadding |
                    ImGuiTreeNodeFlags.Framed
                );
                DragDrop( library, ref listModified );
                ImGui.PopStyleColor( 1 );
                if( ImGui.IsItemClicked( ImGuiMouseButton.Right ) ) ImGui.OpenPopup( $"{id}-context" );

                if( ImGui.BeginPopup( $"{id}-context" ) ) {
                    if( ImGui.Selectable( $"Rename{id}" ) ) {
                        EditingName = Name;
                        Editing = true;
                    }
                    if( ImGui.Selectable( $"New sub-folder{id}" ) ) {
                        var newFolder = new AvfxNodeLibraryFolder( this, "New Folder", UiUtils.RandomString( 12 ), new List<AvfxNodeLibraryProps>() );
                        Add( newFolder );
                        library.Save();
                        listModified = true;
                    }
                    if( ImGui.Selectable( $"Delete{id}" ) ) {
                        Cleanup();
                        Parent.Remove( this );
                        library.Save();
                        listModified = true;
                    }
                    ImGui.EndPopup();
                }

                ImGui.SameLine();

                if( !Editing ) {
                    ImGui.PushFont( UiBuilder.IconFont );
                    ImGui.Text( $"{( char )FontAwesomeIcon.Folder}" );
                    ImGui.PopFont();

                    ImGui.SameLine();
                    ImGui.Text( Name );
                }
                else {
                    var checkSize = UiUtils.GetPaddedIconSize( FontAwesomeIcon.Check );
                    var removeSize = UiUtils.GetPaddedIconSize( FontAwesomeIcon.Trash );
                    ImGui.PushStyleVar( ImGuiStyleVar.ItemSpacing, new Vector2( 3, 4 ) );

                    // Input
                    var inputSize = ImGui.GetContentRegionAvail().X - checkSize - removeSize - 6;
                    ImGui.SetNextItemWidth( inputSize );
                    ImGui.InputText( $"{id}-input", ref EditingName, 256 );

                    ImGui.PushFont( UiBuilder.IconFont );
                    ImGui.SameLine();
                    if( ImGui.Button( $"{( char )FontAwesomeIcon.Check}" + id ) ) {
                        Name = EditingName;
                        Editing = false;
                        library.Save();
                        listModified = true;
                    }
                    ImGui.SameLine();
                    if( UiUtils.RemoveButton( $"{( char )FontAwesomeIcon.Trash}" + id ) ) {
                        Editing = false;
                    }
                    ImGui.PopFont();

                    ImGui.PopStyleVar( 1 );
                }
            }

            if( open ) {
                foreach( var item in Children ) {
                    if( !item.Matches( searchInput ) ) continue;
                    if( item.Draw( library, searchInput ) ) {
                        listModified = true;
                        break;
                    }
                }

                ImGui.PushStyleVar( ImGuiStyleVar.ItemSpacing, new Vector2( 0 ) );
                ImGui.PushStyleVar( ImGuiStyleVar.FramePadding, new Vector2( 0 ) );

                ImGui.BeginChild( $"{id}-end-child", new Vector2( ImGui.GetContentRegionAvail().X, 1 ), false );
                ImGui.EndChild();

                if( ImGui.BeginDragDropTarget() ) {
                    if( library.StopDragging( this ) ) listModified = true;
                    ImGui.EndDragDropTarget();
                }
                ImGui.PopStyleVar( 2 );
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );

                if( !IsRoot ) ImGui.TreePop();
            }

            return listModified;
        }

        public override bool Matches( string input ) => true;

        public override AvfxNodeLibraryProps ToProps() => new() {
            Name = Name,
            Id = Id,
            PropType = AvfxNodeLibraryProps.Type.Folder,
            Children = ChildrenToProps()
        };

        public override void Cleanup() {
            Children.ForEach( x => x.Cleanup() );
        }

        public override bool Contains( AvfxNodeLibraryGeneric item ) {
            foreach( var child in Children ) {
                if( child == item || child.Contains( item ) ) return true;
            }
            return false;
        }
    }
}
