using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Ui;
using VfxEditor.Utils;

namespace VfxEditor.NodeLibrary {
    public class AvfxNodeLibrary : GenericDialog {
        private readonly AvfxNodeLibraryFolder Root;
        private readonly string RootPath;

        private string SearchInput = string.Empty;
        private AvfxNodeLibraryGeneric DraggingItem = null;

        public AvfxNodeLibrary( List<AvfxNodeLibraryProps> items, string rootPath ) : base( "Node Library", false, 500, 750 ) {
            Root = new( null, null, null, items );
            RootPath = rootPath;
        }

        public override void DrawBody() {
            ImGui.TextDisabled( "Save VFX nodes here using the button:" );
            ImGui.SameLine();
            UiUtils.IconText( FontAwesomeIcon.BookMedical, true );

            ImGui.PushFont( UiBuilder.IconFont );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.FolderPlus}##NodeLibrary" ) ) {
                var newFolder = new AvfxNodeLibraryFolder( Root, "New Folder", UiUtils.RandomString( 12 ), new List<AvfxNodeLibraryProps>() );
                Root.Add( newFolder );
                Save();
            }
            ImGui.PopFont();
            ImGui.SameLine();

            ImGui.InputTextWithHint( "##NodeLibrary/Search", "Search", ref SearchInput, 255 );

            ImGui.BeginChild( "##NodeLibrary-Region", ImGui.GetContentRegionAvail(), true );

            if( Root.Children.Count == 0 ) ImGui.Text( "No nodes saved..." );
            Root.Draw( this, SearchInput );

            ImGui.EndChild();
        }

        public string GetPath( string id ) => Path.Combine( RootPath, $"VFX_NodeLibrary_{id}.vfxedit2" );

        public unsafe void AddToRoot( string name, string id, string description, string path ) {
            Root.Add( new AvfxNodeLibraryNode( Root, name, id, path, description, *ImGui.GetStyleColorVec4( ImGuiCol.Header ) ) );
            Save();
        }

        public void Save() {
            Plugin.Configuration.VFXNodeLibraryItems.Clear();
            Plugin.Configuration.VFXNodeLibraryItems.AddRange( Root.ChildrenToProps() );
            Plugin.Configuration.Save();
        }

        public void StartDragging( AvfxNodeLibraryGeneric item ) {
            ImGui.SetDragDropPayload( $"NODE_LIBRARY", IntPtr.Zero, 0 );
            DraggingItem = item;
        }

        public bool StopDragging( AvfxNodeLibraryGeneric destination, bool overridePosition = false ) {
            if( DraggingItem == null ) return false;
            var payload = ImGui.AcceptDragDropPayload( $"NODE_LIBRARY" );
            unsafe {
                if( payload.NativePtr != null ) {
                    // Move them here
                    if( DraggingItem != destination ) {
                        if( DraggingItem is AvfxNodeLibraryFolder folderCheck && folderCheck.Contains( destination ) ) {
                            PluginLog.Log( "Tried to put folder into itself" );
                        }
                        else {
                            DraggingItem.Parent?.Remove( DraggingItem );
                            if( destination is AvfxNodeLibraryFolder folder && !overridePosition ) {
                                folder.Add( DraggingItem );
                            }
                            else {
                                var idx = destination.Parent.Children.IndexOf( destination );
                                if( idx != -1 ) {
                                    DraggingItem.Parent = destination.Parent;
                                    destination.Parent.Children.Insert( idx, DraggingItem );
                                }
                            }
                            Save();
                        }
                    }
                    DraggingItem = null;
                    return true;
                }
            }
            return false;
        }
    }
}
