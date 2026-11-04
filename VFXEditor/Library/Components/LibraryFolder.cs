using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VfxEditor.Library.Node;
using VfxEditor.Library.Texture;
using VfxEditor.Ui.Components;
using VfxEditor.Utils;

namespace VfxEditor.Library.Components {
    public unsafe class LibraryFolder : LibraryGeneric {
        public readonly List<LibraryGeneric> Children = [];
        public bool IsRoot => Parent == null;

        public LibraryFolder( LibraryFolder parent, string name, string id, List<LibraryProps> items ) : base( parent, name, id ) {
            foreach( var item in items ) {
                Children.Add( item.PropType switch {
                    ItemType.Folder => new LibraryFolder( this, item ),
                    ItemType.Texture => new TextureLeaf( this, item ),
                    _ => new NodeLeaf( this, item )
                } );
            }
        }

        public LibraryFolder( LibraryFolder parent, LibraryProps props ) : this( parent, props.Name, props.Id, props.Children ) { }

        public List<LibraryProps> ChildrenToProps() => [.. Children.Select( x => x.ToProps() )];

        public void Add( LibraryGeneric item ) {
            item.Parent = this;
            Children.Add( item );
        }

        public void Remove( LibraryGeneric item ) {
            if( item.Parent == this ) item.Parent = null;
            Children.Remove( item );
        }

        public override bool Draw( LibraryManager library, string searchInput ) {
            using var _ = ImRaii.PushId( Id );

            var listModified = false;
            var open = true;

            if( !IsRoot ) {
                // So that you can drag an item BEFORE a folder, rather than only inside of it
                if( library.DragDropManager.DrawDropTarget( this, Parent, true, "Child" ) ) listModified = true;

                // Main folder item

                using( var color = ImRaii.PushColor( ImGuiCol.Header, new Vector4( 0 ) ) ) {
                    open = ImGui.TreeNodeEx( "###Node",
                        ImGuiTreeNodeFlags.SpanAvailWidth |
                        ImGuiTreeNodeFlags.FramePadding |
                        ImGuiTreeNodeFlags.Framed
                    );
                }
                if( library.DragDropManager.DragDrop( this, Parent ) ) listModified = true;

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

            if( library.DragDropManager.DrawDropTarget( this, Parent, false, "EndChild" ) ) listModified = true;

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );

            if( !IsRoot ) ImGui.TreePop();

            return listModified;
        }

        private bool DrawPopup( LibraryManager library ) {
            if( ImGui.IsItemClicked( ImGuiMouseButton.Right ) ) ImGui.OpenPopup( "Popup" );

            using var popup = ImRaii.Popup( "Popup" );
            if( !popup ) return false;

            if( UiUtils.IconSelectable( FontAwesomeIcon.FolderPlus, "New Sub-Folder" ) ) {
                var newFolder = new LibraryFolder( this, "New Folder", UiUtils.RandomString( 12 ), [] );
                Add( newFolder );
                library.Save();
                return true;
            }

            if( UiUtils.IconSelectable( FontAwesomeIcon.Trash, "Delete" ) ) {
                Plugin.AddModal( new TextModal(
                    "Delete Folder",
                    "Are you sure you want to delete this folder?",
                    () => {
                        Cleanup();
                        Parent.Remove( this );
                        library.Save();
                    }
                ) );
                return true;
            }

            if( ImGui.InputText( "##Rename", ref Name, 128, ImGuiInputTextFlags.AutoSelectAll ) ) {
                library.Save();
                return true;
            }

            return false;
        }

        public override bool Matches( string input ) => true;

        public override LibraryProps ToProps() => new() {
            Name = Name,
            Id = Id,
            PropType = ItemType.Folder,
            Children = ChildrenToProps()
        };

        public override void Cleanup() {
            Children.ForEach( x => x.Cleanup() );
        }

        public override bool Contains( LibraryGeneric item ) {
            foreach( var child in Children ) {
                if( child == item || child.Contains( item ) ) return true;
            }
            return false;
        }
    }
}