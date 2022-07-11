using Dalamud.Logging;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.Dialogs;
using VFXEditor.Helper;

namespace VFXEditor.NodeLibrary {
    public class VFXNodeLibrary : GenericDialog {
        private readonly List<VFXNodeLibraryItem> Items;
        private readonly string RootPath;

        private string SearchInput = string.Empty;
        private VFXNodeLibraryItem DraggingItem = null;

        public VFXNodeLibrary( List<VFXNodeLibraryItem> items, string rootPath ) : base( "Node Library" ) {
            Size = new( 580, 750 );
            Items = items;
            RootPath = rootPath;
        }

        public override void DrawBody() {
            ImGui.InputText( "Search##NodeLibrary", ref SearchInput, 255 );

            ImGui.BeginChild( "##NodeLibrary-Region", ImGui.GetContentRegionAvail(), true );

            var itemIdx = 0;

            if( Items.Count == 0 ) ImGui.Text( "No nodes saved..." );

            foreach( var item in Items ) {
                if( !item.Matches( SearchInput ) ) continue;
                if( item.Draw( this ) ) break;
                itemIdx++;
            }

            ImGui.EndChild();
        }

        public string GetNextPath() => Path.Combine( RootPath, $"VFX_NodeLibrary_{Items.Count}.vfxedit2" );

        public unsafe void Add( string name, string path ) {
            Items.Add( new VFXNodeLibraryItem( name, UIHelper.RandomString(12), path, "", *ImGui.GetStyleColorVec4( ImGuiCol.Header ) ) );
            Save();
        }

        public void Delete( VFXNodeLibraryItem item ) {
            Items.Remove( item );
            item.Delete();
            Save();
        }

        public void StartDragging( VFXNodeLibraryItem item ) {
            ImGui.SetDragDropPayload( $"NODE_LIBRARY", IntPtr.Zero, 0 );
            DraggingItem = item;
        }

        public bool StopDragging( VFXNodeLibraryItem item ) {
            if( DraggingItem == null ) return false;
            var payload = ImGui.AcceptDragDropPayload( $"NODE_LIBRARY" );
            unsafe {
                if( payload.NativePtr != null ) {
                    // Move them here
                    if (DraggingItem != item) {
                        Items.Remove( DraggingItem );
                        var idx = Items.IndexOf( item );
                        if (idx != -1) {
                            Items.Insert( idx, DraggingItem );
                        }
                    }
                    DraggingItem = null;
                    return true;
                }
            }
            return false;
        }

        public static void Save() => Plugin.Configuration.Save();
    }
}
