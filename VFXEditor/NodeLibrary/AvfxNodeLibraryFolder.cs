using Dalamud.Interface;
using ImGuiNET;
using OtterGui.Raii;
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
            using var _ = ImRaii.PushId( Id );

            var listModified = false;
            var open = true;

            if( !IsRoot ) {
                // So that you can drag an item BEFORE a folder, rather than only inside of it

                if( DrawDragDrop( library, true, "Child" ) ) listModified = true;

                // Main folder item

                using( var color = ImRaii.PushColor( ImGuiCol.Header, new Vector4( 0 ) ) ) {
                    open = ImGui.TreeNodeEx( "###Node",
                        ImGuiTreeNodeFlags.SpanAvailWidth |
                        ImGuiTreeNodeFlags.FramePadding |
                        ImGuiTreeNodeFlags.Framed
                    );
                }
                DragDrop( library, Name, ref listModified );

                if( DrawPopup( library ) ) listModified = true;

                using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                    ImGui.SameLine();
                    ImGui.Text( FontAwesomeIcon.Folder.ToIconString() );
                }

                ImGui.SameLine();
                ImGui.Text( Name );
            }

            if( !open ) return listModified;

            // =========== Open ===========

            foreach( var item in Children ) {
                if( !item.Matches( searchInput ) ) continue;
                if( item.Draw( library, searchInput ) ) {
                    listModified = true;
                    break;
                }
            }

            if( DrawDragDrop( library, false, "EndChild" ) ) listModified = true;

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );

            if( !IsRoot ) ImGui.TreePop();

            return listModified;
        }

        private bool DrawDragDrop( AvfxNodeLibrary library, bool overridePosition, string name ) {
            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 0 ) );
            style.Push( ImGuiStyleVar.FramePadding, new Vector2( 0 ) );

            ImGui.BeginChild( name, new Vector2( ImGui.GetContentRegionAvail().X, 1 ), false );
            ImGui.EndChild();

            using var dragDrop = ImRaii.DragDropTarget();
            if( !dragDrop ) return false;

            if( library.StopDragging( this, overridePosition ) ) return true;

            return false;
        }

        private bool DrawPopup( AvfxNodeLibrary library ) {
            if( ImGui.IsItemClicked( ImGuiMouseButton.Right ) ) ImGui.OpenPopup( "Popup" );

            using var popup = ImRaii.Popup( "Popup" );
            if( !popup ) return false;

            if( UiUtils.IconSelectable( FontAwesomeIcon.FolderPlus, "New Sub-Folder" ) ) {
                var newFolder = new AvfxNodeLibraryFolder( this, "New Folder", UiUtils.RandomString( 12 ), new List<AvfxNodeLibraryProps>() );
                Add( newFolder );
                library.Save();
                return true;
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
