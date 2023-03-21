using Dalamud.Interface;
using ImGuiFileDialog;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.FileManager;
using VfxEditor.Ui;

namespace VfxEditor {
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
        GameQuest
    }

    public struct SelectResult {
        public SelectResultType Type;
        public string DisplayString;
        public string Path;

        public SelectResult( SelectResultType type, string displayString, string path ) {
            Type = type;
            DisplayString = displayString;
            Path = path;
        }

        public override readonly bool Equals( object obj ) => obj is SelectResult other && Equals( other );
        public readonly bool Equals( SelectResult p ) => p.Type == Type && p.DisplayString == DisplayString && p.Path == Path;
        public override readonly int GetHashCode() => (Type, DisplayString, Path).GetHashCode();
        public static bool operator ==( SelectResult lhs, SelectResult rhs ) => lhs.Equals( rhs );
        public static bool operator !=( SelectResult lhs, SelectResult rhs ) => !( lhs == rhs );

        public static SelectResult None() => new() {
            DisplayString = "[NONE]",
            Path = ""
        };
    }

    public abstract class SelectDialog : GenericDialog {
        public static readonly uint FavoriteColor = ImGui.GetColorU32( new Vector4( 1.0f, 0.878f, 0.1058f, 1 ) );
        public static readonly uint TransparentColor = ImGui.GetColorU32( new Vector4( 0, 0, 0, 0 ) );

        private readonly FileManagerWindow Manager;
        private readonly string Extension;
        public readonly bool IsSource; // as opposed to replaced

        protected abstract List<SelectTab> GetTabs();

        protected readonly List<SelectResult> Favorites;
        protected readonly SelectDialogList RecentTab;
        protected readonly SelectDialogList FavoritesTab;

        private string LocalPathInput = "";
        private string GamePathInput = "";

        public SelectDialog( string name, string extension, FileManagerWindow manager, bool isSource ) : base( name, false, 800, 500 ) {
            Manager = manager;
            Extension = extension;
            Favorites = manager.GetConfig().Favorites;
            IsSource = isSource;

            RecentTab = new( this, "Recent", manager.GetConfig().RecentItems );
            FavoritesTab = new( this, "Favorites", manager.GetConfig().Favorites );
        }

        public void Invoke( SelectResult result ) {
            if( IsSource ) Manager.SetSource( result );
            else Manager.SetReplace( result );
        }

        public bool IsFavorite( SelectResult result ) => Favorites.Contains( result );

        public void AddFavorite( SelectResult result ) {
            Favorites.Add( result );
            Plugin.Configuration.Save();
        }

        public void RemoveFavorite( SelectResult result ) {
            Favorites.Remove( result );
            Plugin.Configuration.Save();
        }

        public virtual void Play( string playPath, string id ) { }

        public override void DrawBody() {
            var id = $"##{Name}";
            ImGui.BeginTabBar( $"Tabs{id}" );
            DrawGame( id );
            DrawGamePath( id );
            if( IsSource ) DrawLocal( id );
            RecentTab.Draw( id );
            FavoritesTab.Draw( id );
            ImGui.EndTabBar();
        }

        // =========== LOCAL ================

        private void DrawLocal( string parentId ) {
            var id = $"{parentId}/Local";

            if( !ImGui.BeginTabItem( "Local File" + id ) ) return;
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Text( $".{Extension} file located on your computer, eg: C:/Users/me/Downloads/awesome.{Extension}" );
            ImGui.Text( "Path" );
            ImGui.SameLine();
            ImGui.InputText( id + "-Input", ref LocalPathInput, 255 );
            ImGui.SameLine();
            if( ImGui.Button( ( "Browse" + id ) ) ) {
                FileDialogManager.OpenFileDialog( "Select a File", $".{Extension},.*", ( bool ok, string res ) => {
                    if( !ok ) return;
                    Invoke( new SelectResult( SelectResultType.Local, "[LOCAL] " + res, res ) );
                } );
            }
            ImGui.SameLine();
            if( ImGui.Button( "SELECT" + id ) ) Invoke( new SelectResult( SelectResultType.Local, "[LOCAL] " + LocalPathInput, LocalPathInput ) );
            ImGui.EndTabItem();
        }

        // ============= GAME =================

        public void DrawGame( string parentId ) {
            var id = $"{parentId}/Game";

            if( GetTabs().Count == 0 ) return;
            if( !ImGui.BeginTabItem( "Game Items" + id ) ) return;

            ImGui.BeginTabBar( "Tabs" + id );
            foreach( var tab in GetTabs() ) tab.Draw( id );
            ImGui.EndTabBar();
            ImGui.EndTabItem();
        }

        // ============== GAME FILE =============

        public void DrawGamePath( string parentId ) {
            var id = $"{parentId}/Path";
            if( !ImGui.BeginTabItem( "Game Path" + id ) ) return;

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Text( $"In-game .{Extension} file, eg: vfx/common/eff/wp_astro1h.{Extension}" );
            ImGui.Text( "Path" );
            ImGui.SameLine();
            ImGui.InputText( id + "-Input", ref GamePathInput, 255 );
            ImGui.SameLine();
            if( ImGui.Button( "SELECT" + id ) ) {
                var cleanedGamePath = GamePathInput.Replace( "\\", "/" );
                Invoke( new SelectResult( SelectResultType.GamePath, "[GAME] " + cleanedGamePath, cleanedGamePath ) );
            }
            ImGui.EndTabItem();
        }

        public bool DrawFavorite( string path, SelectResultType resultType, string resultName ) => DrawFavorite( SelectTab.GetSelectResult( path, resultType, resultName ) );

        public bool DrawFavorite( SelectResult selectResult ) {
            var res = false;
            var isFavorite = IsFavorite( selectResult );
            if( isFavorite ) ImGui.PushStyleColor( ImGuiCol.Text, FavoriteColor );
            ImGui.PushFont( UiBuilder.IconFont );

            ImGui.Text( $"{( char )FontAwesomeIcon.Star}" );
            if( ImGui.IsItemClicked() ) {
                if( isFavorite ) RemoveFavorite( selectResult );
                else AddFavorite( selectResult );
                res = true;
            }
            ImGui.PopFont();
            if( isFavorite ) ImGui.PopStyleColor();
            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 2 );
            return res;
        }
    }
}
