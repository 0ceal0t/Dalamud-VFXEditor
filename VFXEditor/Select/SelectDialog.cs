using Dalamud.Interface;
using ImGuiFileDialog;
using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VfxEditor.FileManager;
using VfxEditor.Select.Lists;
using VfxEditor.Select.Scd.BgmQuest;
using VfxEditor.Ui;

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
        GameMisc
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

        public readonly FileManagerWindow Manager;
        public readonly string Extension;
        public readonly bool IsSource; // as opposed to replaced
        protected abstract List<SelectTab> GetTabs();

        protected readonly List<SelectResult> Favorites;
        protected readonly SelectListTab RecentTab;
        protected readonly SelectFavoriteTab FavoritesTab;
        protected readonly SelectPenumbraTab PenumbraTab;

        private string LocalPathInput = "";
        private string GamePathInput = "";

        public SelectDialog( string name, string extension, FileManagerWindow manager, bool isSource ) : base( name, false, 800, 500 ) {
            Manager = manager;
            Extension = extension;
            Favorites = manager.GetConfig().Favorites;
            IsSource = isSource;

            RecentTab = new( this, "Recent", manager.GetConfig().RecentItems );
            FavoritesTab = new( this, "Favorites", manager.GetConfig().Favorites );
            if( isSource ) PenumbraTab = new( this );
        }

        public void Invoke( SelectResult result ) {
            if( IsSource ) Manager.SetSource( result );
            else Manager.SetReplace( result );
        }

        public virtual void Play( string path ) { }

        public override void DrawBody() {
            using var _ = ImRaii.PushId( $"{Manager.Id}/{Name}" );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            DrawGameTabs();
            DrawGamePath();
            if( IsSource ) {
                DrawLocalPath();
                PenumbraTab.Draw();
            }
            RecentTab.Draw();
            FavoritesTab.Draw();
        }

        // ============= GAME =================

        public void DrawGameTabs() {
            if( GetTabs().Count == 0 ) return;
            using var _ = ImRaii.PushId( "Game" );

            using var tabItem = ImRaii.TabItem( "Game Items" );
            if( !tabItem ) return;

            using var tabBar = ImRaii.TabBar( "Tabs" );
            if( !tabBar ) return;
            foreach( var tab in GetTabs() ) tab.Draw();
        }

        // =========== LOCAL ================

        private void DrawLocalPath() {
            using var _ = ImRaii.PushId( "Local" );

            using var tabItem = ImRaii.TabItem( "Local File" );
            if( !tabItem ) return;

            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 4, 3 ) );

            ImGui.TextDisabled( $".{Extension} file located on your computer, eg: C:/Users/me/Downloads/awesome.{Extension}" );
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() + 4 );

            ImGui.Text( "Path" );
            ImGui.SameLine();
            ImGui.InputText( "##Input", ref LocalPathInput, 255 );

            ImGui.SameLine();
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                var browse = ImGui.Button( FontAwesomeIcon.Search.ToIconString() );
                if( browse ) {
                    FileDialogManager.OpenFileDialog( "Select a File", $".{Extension},.*", ( bool ok, string res ) => {
                        if( !ok ) return;
                        Invoke( new SelectResult( SelectResultType.Local, "[LOCAL] " + res, res ) );
                    } );
                }
            }

            ImGui.SameLine();
            if( ImGui.Button( "SELECT" ) ) {
                Invoke( new SelectResult( SelectResultType.Local, "[LOCAL] " + LocalPathInput, LocalPathInput ) );
            }
        }

        // ============== GAME FILE =============

        public void DrawGamePath() {
            using var _ = ImRaii.PushId( "Path" );

            using var tabItem = ImRaii.TabItem( "Game Path" );
            if( !tabItem ) return;

            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 4, 3 ) );

            ImGui.TextDisabled( $"In-game .{Extension} file, eg: vfx/common/eff/wp_astro1h.{Extension}" );
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() + 4 );

            ImGui.Text( "Path" );
            ImGui.SameLine();
            ImGui.InputText( "##Input", ref GamePathInput, 255 );

            ImGui.SameLine();
            if( ImGui.Button( "SELECT" ) ) {
                var cleanedGamePath = GamePathInput.Replace( "\\", "/" );
                Invoke( new SelectResult( SelectResultType.GamePath, "[GAME] " + cleanedGamePath, cleanedGamePath ) );
            }
        }

        // ======== DRAWING UTILS ======

        public bool IsFavorite( SelectResult result ) => Favorites.Any( result.CompareTo );

        public void AddFavorite( SelectResult result ) {
            Favorites.Add( result );
            Plugin.Configuration.Save();
        }

        public void RemoveFavorite( SelectResult result ) {
            Favorites.RemoveAll( result.CompareTo );
            Plugin.Configuration.Save();
        }

        public bool DrawFavorite( string path, SelectResultType resultType, string resultName ) => DrawFavorite( SelectTabUtils.GetSelectResult( path, resultType, resultName ) );

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

        public void DrawPapsWithHeader( Dictionary<string, Dictionary<string, string>> items, SelectResultType resultType, string name ) {
            foreach( var (subName, subItems) in items ) {
                if( subItems.Count == 0 ) continue;

                using var _ = ImRaii.PushId( subName );

                if( ImGui.CollapsingHeader( subName, ImGuiTreeNodeFlags.DefaultOpen ) ) {
                    using var indent = ImRaii.PushIndent( 10f );
                    DrawPaps( subItems, resultType, $"{name} {subName}" );
                    ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
                }
            }
        }

        public void DrawPaps( Dictionary<string, string> items, SelectResultType resultType, string name ) {
            using var _ = ImRaii.PushId( name );

            foreach( var (suffix, path) in items ) {
                using var __ = ImRaii.PushId( suffix );

                DrawFavorite( path, resultType, $"{name} ({suffix})" );

                ImGui.Text( $"{suffix}:" );

                ImGui.SameLine();
                if( path.Contains( "action.pap" ) || path.Contains( "face.pap" ) ) SelectTabUtils.DisplayPathWarning( path, "Be careful about modifying this file, as it contains dozens of animations for every job" );
                else SelectTabUtils.DisplayPath( path );

                DrawPath( "", path, resultType, $"{name} ({suffix})" );
            }
        }

        public void DrawPaths( string label, IEnumerable<string> paths, SelectResultType resultType, string resultName, bool play = false ) {
            var idx = 0;
            foreach( var path in paths ) {
                using var _ = ImRaii.PushId( idx );
                DrawPath( $"{label} #{idx}", path, resultType, $"{resultName} #{idx}", play );
                idx++;
            }
        }

        public void DrawPath( string label, string path, SelectResultType resultType, string resultName, bool play = false ) =>
            DrawPath( label, path, path, resultType, resultName, play );

        public void DrawPath( string label, string path, string displayPath, SelectResultType resultType, string resultName, bool play = false ) {
            if( string.IsNullOrEmpty( path ) ) return;
            if( path.Contains( "BGM_Null" ) ) return;

            using var _ = ImRaii.PushId( label );

            if( !string.IsNullOrEmpty( label ) ) { // if this is blank, assume there is some custom logic to draw the path
                DrawFavorite( path, resultType, resultName );
                ImGui.Text( $"{label}:" );
                ImGui.SameLine();
                SelectTabUtils.DisplayPath( displayPath );
            }

            using var indent = ImRaii.PushIndent( 25f );

            if( ImGui.Button( "SELECT" ) ) Invoke( SelectTabUtils.GetSelectResult( path, resultType, resultName ) );
            ImGui.SameLine();
            SelectTabUtils.Copy( path );
            if( play ) Play( path );
        }

        public void DrawBgmSituation( string name, BgmSituationStruct situation ) {
            if( situation.IsSituation ) {
                DrawPath( "Daytime Bgm", situation.DayPath, SelectResultType.GameMusic, $"{name} / Day" );
                DrawPath( "Nighttime Bgm", situation.NightPath, SelectResultType.GameMusic, $"{name} / Night" );
                DrawPath( "Battle Bgm", situation.BattlePath, SelectResultType.GameMusic, $"{name} / Battle" );
                DrawPath( "Daybreak Bgm", situation.DaybreakPath, SelectResultType.GameMusic, $"{name} / Break" );
            }
            else DrawPath( "Bgm", situation.Path, SelectResultType.GameZone, name );
        }
    }
}
