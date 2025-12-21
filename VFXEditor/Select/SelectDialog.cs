using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.FileManager;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Select.Lists;
using VfxEditor.Select.Penumbra;
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
        public string Name;
        public string DisplayString; // [PREFIX] Name
        public string Path;

        public SelectResult( SelectResultType type, string name, string displayString, string path ) {
            Type = type;
            Name = name;
            DisplayString = displayString;
            Path = path;
        }

        public bool CompareTo( SelectResult other ) => Type == other.Type && DisplayString == other.DisplayString && Path == other.Path;
    }

    public abstract partial class SelectDialog : DalamudWindow {
        public static readonly List<string> LoggedFiles = [];

        public readonly IFileManagerSelect Manager;

        public readonly List<string> Extensions;

        public readonly bool ShowLocal;
        private readonly Action<SelectResult> Action;

        protected readonly List<SelectTab> GameTabs = [];
        protected readonly List<SelectResult> Favorites;
        protected readonly SelectRecentTab RecentTab;
        protected readonly SelectFavoriteTab FavoritesTab;
        protected readonly SelectPenumbraTab PenumbraTab;

        private string GamePathInput = "";
        private string LocalPathInput = "";
        private string LoggedFilesSearch = "";

        public SelectDialog( string name, string extension, FileManagerBase manager, bool showLocal ) :
            this( name, extension, manager, showLocal, showLocal ? ( ( SelectResult result ) => manager.SetSource( result ) ) : ( ( SelectResult result ) => manager.SetReplace( result ) ) ) { }

        public SelectDialog( string name, string extensions, IFileManagerSelect manager, bool showLocal, Action<SelectResult> action ) : base( name, false, new( 800, 500 ), manager.GetWindowSystem() ) {
            Manager = manager;
            Extensions = new( extensions.Split( '|' ) );
            Favorites = manager.GetConfig().Favorites;
            ShowLocal = showLocal;
            Action = action;

            RecentTab = new( this, "Recent", manager.GetConfig().RecentItems );
            FavoritesTab = new( this, "Favorites", manager.GetConfig().Favorites );
            PenumbraTab = new( this );
        }

        public void Invoke( SelectResult result ) => Action?.Invoke( result );

        public virtual bool CanPlay => false;

        public virtual void PlayButton( string path ) { }

        public virtual void PlayPopupItems( string path ) { }

        public override void DrawBody() {
            using var _ = ImRaii.PushId( $"{Manager.GetId()}/{WindowName}" );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            DrawGameTabs();
            DrawPaths();
            PenumbraTab.Draw();
            RecentTab.Draw();
            FavoritesTab.Draw();
        }

        // ============= GAME =================

        private void DrawGameTabs() {
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

        protected bool DrawFavorite( string path, string resultName, SelectResultType resultType ) => DrawFavorite( SelectUiUtils.GetSelectResult( path, resultType, resultName ) );

        public bool DrawFavorite( SelectResult selectResult ) {
            var res = false;
            var isFavorite = IsFavorite( selectResult );

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) )
            using( var color = ImRaii.PushColor( ImGuiCol.Text, UiUtils.DALAMUD_ORANGE, isFavorite ) ) {
                ImGui.Text( FontAwesomeIcon.Star.ToIconString() );

                if( ImGui.IsItemClicked() ) {
                    if( isFavorite ) RemoveFavorite( selectResult );
                    else AddFavorite( selectResult );
                    res = true;
                }
            }
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
