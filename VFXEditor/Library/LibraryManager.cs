using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Library.Node;
using VfxEditor.Library.Texture;
using VfxEditor.Ui;
using VfxEditor.Utils;

namespace VfxEditor.Library {
    public class LibraryManager : GenericDialog {
        public readonly NodeRoot NodeRoot;
        public readonly TextureRoot TextureRoot;
        private readonly string RootPath;

        private string SearchInput = string.Empty;
        private LibraryGeneric DraggingItem = null;
        private LibraryFolder LastDrawnRoot;

        private string ComboSearchInput = string.Empty;

        public LibraryManager() : base( "Library", false, 500, 750 ) {
            NodeRoot = new( Plugin.Configuration.VFXNodeLibraryItems );
            TextureRoot = new( Plugin.Configuration.VfxTextureLibraryItems );
            RootPath = Plugin.Configuration.WriteLocation;

            if( !Plugin.Configuration.VfxTextureDefaultLoaded ) {
                ImportTextures( Path.Combine( Plugin.RootLocation, "Files", "useful_textures.txt" ) );

                Plugin.Configuration.VfxTextureDefaultLoaded = true;
                Save();
            }
        }

        public override void DrawBody() {
            using var _ = ImRaii.PushId( "Library" );

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( ImGui.Button( FontAwesomeIcon.FolderPlus.ToIconString() ) ) {
                    var newFolder = new LibraryFolder( NodeRoot, "New Folder", UiUtils.RandomString( 12 ), new List<LibraryProps>() );
                    LastDrawnRoot.Add( newFolder );
                    Save();
                }
            }

            ImGui.SameLine();
            ImGui.InputTextWithHint( "##Search", "Search", ref SearchInput, 255 );

            // Info circle

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                ImGui.SameLine();
                ImGui.TextDisabled( FontAwesomeIcon.InfoCircle.ToIconString() );
            }
            UiUtils.Tooltip( "Import and edit items by right-clicking them" );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );

            using var tabBar = ImRaii.TabBar( "TabBar" );
            if( !tabBar ) return;

            DrawRoot( NodeRoot, "Nodes" );
            DrawRoot( TextureRoot, "Textures" );
        }

        private void DrawRoot( LibraryFolder root, string name ) {
            using var tab = ImRaii.TabItem( name );
            if( !tab ) return;

            LastDrawnRoot = root;

            root.Draw( this, SearchInput );
        }

        public void Save() {
            Plugin.Configuration.VFXNodeLibraryItems.Clear();
            Plugin.Configuration.VFXNodeLibraryItems.AddRange( NodeRoot.ChildrenToProps() );

            Plugin.Configuration.VfxTextureLibraryItems.Clear();
            Plugin.Configuration.VfxTextureLibraryItems.AddRange( TextureRoot.ChildrenToProps() );

            Plugin.Configuration.Save();
        }

        public void StartDragging( LibraryGeneric item ) {
            ImGui.SetDragDropPayload( "NODE_LIBRARY", IntPtr.Zero, 0 );
            DraggingItem = item;
        }

        public unsafe bool StopDragging( LibraryGeneric destination, bool overridePosition = false ) {
            if( DraggingItem == null ) return false;
            var payload = ImGui.AcceptDragDropPayload( "NODE_LIBRARY" );
            if( payload.NativePtr == null ) return false;

            // Move them here
            if( DraggingItem != destination ) {
                if( DraggingItem is LibraryFolder folderCheck && folderCheck.Contains( destination ) ) {
                    Dalamud.Log( "Tried to put folder into itself" );
                }
                else {
                    DraggingItem.Parent?.Remove( DraggingItem );

                    if( destination is LibraryFolder folder && !overridePosition ) { // Just add it at the end
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

        // ========= NODES ===============

        public unsafe void AddNode( string name, string id, string description, string path ) {
            NodeRoot.Add( new NodeLeaf( NodeRoot, name, id, path, description, *ImGui.GetStyleColorVec4( ImGuiCol.Header ) ) );
            Save();
        }

        public string GetNodePath( string id ) => Path.Combine( RootPath, $"VFX_NodeLibrary_{id}.vfxedit2" );

        // ======== TEXTURES ==============

        public void DrawTextureCombo( string currentPath, Action<TextureLeaf> perTexture ) {
            using var combo = ImRaii.Combo( "##Library", string.Empty, ImGuiComboFlags.NoPreview | ImGuiComboFlags.PopupAlignLeft | ImGuiComboFlags.HeightLargest );
            if( !combo ) return;

            var resetScroll = ImGui.InputTextWithHint( "##Search", "Search", ref ComboSearchInput, 255 );

            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 4, 4 ) ) ) {
                ImGui.SameLine();
                using var font = ImRaii.PushFont( UiBuilder.IconFont );
                if( ImGui.Button( FontAwesomeIcon.Save.ToIconString() ) ) {
                    TextureRoot.AddTexture( currentPath, currentPath );
                    Save();
                }
            }

            using var child = ImRaii.Child( "Child", new Vector2( ImGui.GetWindowContentRegionMax().X - ImGui.GetWindowContentRegionMin().X, 300 ), true );

            if( resetScroll ) ImGui.SetScrollHereY();

            IterateTextures( TextureRoot, ( TextureLeaf texture ) => {
                if( string.IsNullOrEmpty( ComboSearchInput ) || texture.Matches( ComboSearchInput ) ) perTexture( texture );
            } );
        }

        private void IterateTextures( LibraryFolder folder, Action<TextureLeaf> perTexture ) {
            foreach( var child in folder.Children ) {
                if( child is LibraryFolder f ) {
                    IterateTextures( f, perTexture );
                }
                else if( child is TextureLeaf texture ) {
                    perTexture( texture );
                }
            }
        }

        public void ImportTextures( string localPath ) {
            var lines = File.ReadAllLines( localPath );
            foreach( var line in lines ) {
                var parts = line.Split( ' ', 2 );
                TextureRoot.AddTexture( parts[1], parts[0] );
            }
            Save();
        }

        public void ExportTextures( string localPath ) {
            var lines = new List<string>();
            IterateTextures( TextureRoot, ( TextureLeaf texture ) => {
                lines.Add( $"{texture.GetPath()} {texture.GetName()}" );
            } );
            File.WriteAllLines( localPath, lines );
        }
    }
}
