using Dalamud.Interface;
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
using VFXEditor.Utils;

namespace VFXEditor.NodeLibrary {
    public class AvfxNodeLibrary : GenericDialog {
        private readonly List<AvfxNodeLibraryItem> Items;
        private readonly string RootPath;

        private string SearchInput = string.Empty;
        private AvfxNodeLibraryItem DraggingItem = null;

        public AvfxNodeLibrary( List<AvfxNodeLibraryItem> items, string rootPath ) : base( "Node Library" ) {
            Size = new( 580, 750 );
            Items = items;
            RootPath = rootPath;
        }

        public override void DrawBody() {
            UiUtils.IconText( FontAwesomeIcon.InfoCircle, true );
            ImGui.SameLine();
            ImGui.TextDisabled( "Save VFX nodes here using the button:" );
            ImGui.SameLine();
            UiUtils.IconText( FontAwesomeIcon.BookMedical, true );

            ImGui.InputText( "Search##NodeLibrary", ref SearchInput, 255 );

            ImGui.BeginChild( "##NodeLibrary-Region", ImGui.GetContentRegionAvail(), true );

            if( Items.Count == 0 ) ImGui.Text( "No nodes saved..." );

            var itemIdx = 0;
            foreach( var item in Items ) {
                if( !item.Matches( SearchInput ) ) continue;
                if( item.Draw( this ) ) break;
                itemIdx++;
            }

            ImGui.EndChild();
        }

        public string GetPath( string id ) => Path.Combine( RootPath, $"VFX_NodeLibrary_{id}.vfxedit2" );

        public unsafe void Add( string name, string id, string path ) {
            Items.Add( new AvfxNodeLibraryItem( name, id, path, "", *ImGui.GetStyleColorVec4( ImGuiCol.Header ) ) );
            Save();
        }

        public void Delete( AvfxNodeLibraryItem item ) {
            Items.Remove( item );
            item.Delete();
            Save();
        }

        public void StartDragging( AvfxNodeLibraryItem item ) {
            ImGui.SetDragDropPayload( $"NODE_LIBRARY", IntPtr.Zero, 0 );
            DraggingItem = item;
        }

        public bool StopDragging( AvfxNodeLibraryItem item ) {
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

        public static void Save() => VfxEditor.Configuration.Save();
    }
}
