using Dalamud.Interface;
using ImGuiNET;
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
                   ImGuiTreeNodeFlags.SpanAvailWidth |
                    ImGuiTreeNodeFlags.FramePadding |
                    ImGuiTreeNodeFlags.Framed
                );
                DragDrop( library, Name, ref listModified );
                ImGui.PopStyleColor( 1 );
                if( ImGui.IsItemClicked( ImGuiMouseButton.Right ) ) ImGui.OpenPopup( $"{id}/Popup" );

                if( ImGui.BeginPopup( $"{id}/Popup" ) ) {
                    if( ImGui.Selectable( $"New Sub-Folder{id}" ) ) {
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
                    if( ImGui.InputText( $"{id}/Rename", ref Name, 128, ImGuiInputTextFlags.AutoSelectAll ) ) {
                        library.Save();
                        listModified = true;
                    }
                    ImGui.EndPopup();
                }

                ImGui.SameLine();

                ImGui.PushFont( UiBuilder.IconFont );
                ImGui.Text( $"{( char )FontAwesomeIcon.Folder}" );
                ImGui.PopFont();

                ImGui.SameLine();
                ImGui.Text( Name );
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
