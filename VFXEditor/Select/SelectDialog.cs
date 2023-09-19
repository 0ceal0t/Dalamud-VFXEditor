using Dalamud.Interface;
using ImGuiFileDialog;
using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using VfxEditor.FileManager;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Select.Lists;
using VfxEditor.Ui;
using VfxEditor.Utils;

namespace VfxEditor.Select {
    public enum SelectResultType {
        Local,
        GamePath,
        GameItem,
        GameStatus,
        GameAction,
        GameZone,
        GameEmote,
        GameGimmick,
        GameCutscene,
        GameNpc,
        GameMusic,
        GameQuest,
        GameCharacter,
        GameJob,
        GameMisc,
        GameMount,
        GameHousing,
        GameUi
    }

    [Serializable]
    public class SelectResult {
        public SelectResultType Type;
        public string DisplayString;
        public string Path;

        public SelectResult( SelectResultType type, string displayString, string path ) {
            Type = type;
            DisplayString = displayString;
            Path = path;
        }

        public bool CompareTo( SelectResult other ) => Type == other.Type && DisplayString == other.DisplayString && Path == other.Path;
    }

    public abstract class SelectDialog : GenericDialog {
        public static readonly uint FavoriteColor = ImGui.GetColorU32( new Vector4( 1.0f, 0.878f, 0.1058f, 1 ) );
        public static readonly uint TransparentColor = ImGui.GetColorU32( new Vector4( 0, 0, 0, 0 ) );

        public readonly IFileManagerSelect Manager;
        public readonly string Extension;
        public readonly bool ShowLocal;
        private readonly Action<SelectResult> Action;

        protected readonly List<SelectTab> GameTabs = new();
        protected readonly List<SelectResult> Favorites;
        protected readonly SelectListTab RecentTab;
        protected readonly SelectFavoriteTab FavoritesTab;
        protected readonly SelectPenumbraTab PenumbraTab;

        private string PathInput = "";

        public SelectDialog( string name, string extension, FileManagerBase manager, bool showLocal ) : this( name, extension, manager, showLocal,
                showLocal ? ( ( SelectResult result ) => manager.SetSource( result ) ) : ( ( SelectResult result ) => manager.SetReplace( result ) )
            ) { }

        public SelectDialog( string name, string extension, IFileManagerSelect manager, bool showLocal, Action<SelectResult> action ) : base( name, false, 800, 500 ) {
            Manager = manager;
            Extension = extension;
            Favorites = manager.GetConfig().Favorites;
            ShowLocal = showLocal;
            Action = action;

            RecentTab = new( this, "Recent", manager.GetConfig().RecentItems );
            FavoritesTab = new( this, "Favorites", manager.GetConfig().Favorites );
            if( showLocal ) PenumbraTab = new( this );
        }

        public void Invoke( SelectResult result ) => Action?.Invoke( result );

        public virtual void Play( string path ) { }

        public override void DrawBody() {
            using var _ = ImRaii.PushId( $"{Manager.GetId()}/{Name}" );

            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                ImGui.InputTextWithHint( "##PathInput", ShowLocal ? "Game or local path" : "Game path", ref PathInput, 255 );

                if( ShowLocal ) {
                    ImGui.SameLine();
                    using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                        if( ImGui.Button( FontAwesomeIcon.FolderOpen.ToIconString() ) ) {
                            FileDialogManager.OpenFileDialog( "Select a File", $".{Extension},.*", ( bool ok, string res ) => {
                                if( !ok ) return;
                                Invoke( new SelectResult( SelectResultType.Local, "[LOCAL] " + res, res ) );
                            } );
                        }
                    }
                    UiUtils.Tooltip( "Browse for a local file" );
                }

                ImGui.SameLine();
                if( ImGui.Button( "SELECT" ) ) {
                    if( ShowLocal && Path.IsPathRooted( PathInput ) && File.Exists( PathInput ) ) {
                        Invoke( new SelectResult( SelectResultType.Local, "[LOCAL] " + PathInput, PathInput ) );
                        PathInput = "";
                    }
                    else {
                        var cleanedPath = PathInput.Trim().Replace( "\\", "/" );
                        if( Plugin.DataManager.FileExists( cleanedPath ) ) {
                            Invoke( new SelectResult( SelectResultType.GamePath, "[GAME] " + cleanedPath, cleanedPath ) );
                            PathInput = "";
                        }
                    }
                }
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            DrawGameTabs();
            if( ShowLocal ) PenumbraTab.Draw();
            RecentTab.Draw();
            FavoritesTab.Draw();
        }

        // ============= GAME =================

        public void DrawGameTabs() {
            if( GameTabs.Count == 0 ) return;
            using var _ = ImRaii.PushId( "Game" );

            using var tabItem = ImRaii.TabItem( "Game Items" );
            if( !tabItem ) return;

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );

            using var tabBar = ImRaii.TabBar( "Tabs" );
            if( !tabBar ) return;

            foreach( var tab in GameTabs ) tab.Draw();
        }

        // ======== FAVORITES ======

        public bool DrawFavorite( SelectResult selectResult ) {
            var res = false;
            var isFavorite = IsFavorite( selectResult );

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) )
            using( var color = ImRaii.PushColor( ImGuiCol.Text, FavoriteColor, isFavorite ) ) {
                ImGui.Text( FontAwesomeIcon.Star.ToIconString() );

                if( ImGui.IsItemClicked() ) {
                    if( isFavorite ) RemoveFavorite( selectResult );
                    else AddFavorite( selectResult );
                    res = true;
                }
            }

            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 2 );
            return res;
        }

        private bool IsFavorite( SelectResult result ) => Favorites.Any( result.CompareTo );

        private void AddFavorite( SelectResult result ) {
            Favorites.Add( result );
            Plugin.Configuration.Save();
        }

        private void RemoveFavorite( SelectResult result ) {
            Favorites.RemoveAll( result.CompareTo );
            Plugin.Configuration.Save();
        }
    }
}
