using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using ImGuiFileDialog;
using ImGuiNET;
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

        public virtual void Play( string playPath, string id ) { }

        public override void DrawBody() {
            var id = $"##{Manager.Id}/Select";
            ImGui.BeginTabBar( $"Tabs{id}" );
            DrawGameTabs( id );
            DrawGamePath( id );
            if( IsSource ) {
                DrawLocalPath( id );
                PenumbraTab.Draw( id );
            }
            RecentTab.Draw( id );
            FavoritesTab.Draw( id );
            ImGui.EndTabBar();
        }

        // ============= GAME =================

        public void DrawGameTabs( string parentId ) {
            var id = $"{parentId}/Game";

            if( GetTabs().Count == 0 ) return;
            if( !ImGui.BeginTabItem( $"Game Items{id}" ) ) return;

            ImGui.BeginTabBar( $"Tabs{id}" );
            foreach( var tab in GetTabs() ) tab.Draw( id );
            ImGui.EndTabBar();
            ImGui.EndTabItem();
        }

        // =========== LOCAL ================

        private void DrawLocalPath( string parentId ) {
            var id = $"{parentId}/Local";
            if( !ImGui.BeginTabItem( $"Local File{id}" ) ) return;
            ImGui.PushStyleVar( ImGuiStyleVar.ItemSpacing, new Vector2( 4, 3 ) );

            ImGui.TextDisabled( $".{Extension} file located on your computer, eg: C:/Users/me/Downloads/awesome.{Extension}" );
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() + 4 );

            ImGui.Text( "Path" );
            ImGui.SameLine();
            ImGui.InputText( $"{id}-Input", ref LocalPathInput, 255 );

            ImGui.SameLine();
            ImGui.PushFont( UiBuilder.IconFont );
            var browse = ImGui.Button( $"{( char )FontAwesomeIcon.Search}{id}" );
            ImGui.PopFont();
            if( browse) {
                FileDialogManager.OpenFileDialog( "Select a File", $".{Extension},.*", ( bool ok, string res ) => {
                    if( !ok ) return;
                    Invoke( new SelectResult( SelectResultType.Local, "[LOCAL] " + res, res ) );
                } );
            }

            ImGui.SameLine();
            if( ImGui.Button( $"SELECT{id}" ) ) {
                Invoke( new SelectResult( SelectResultType.Local, "[LOCAL] " + LocalPathInput, LocalPathInput ) );
            }

            ImGui.PopStyleVar( 1 );
            ImGui.EndTabItem();
        }

        // ============== GAME FILE =============

        public void DrawGamePath( string parentId ) {
            var id = $"{parentId}/Path";
            if( !ImGui.BeginTabItem( $"Game Path{id}" ) ) return;
            ImGui.PushStyleVar( ImGuiStyleVar.ItemSpacing, new Vector2( 4, 3 ) );

            ImGui.TextDisabled( $"In-game .{Extension} file, eg: vfx/common/eff/wp_astro1h.{Extension}" );
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() + 4 );

            ImGui.Text( "Path" );
            ImGui.SameLine();
            ImGui.InputText(  $"{id}-Input", ref GamePathInput, 255 );

            ImGui.SameLine();
            if( ImGui.Button( $"SELECT{id}" ) ) {
                var cleanedGamePath = GamePathInput.Replace( "\\", "/" );
                Invoke( new SelectResult( SelectResultType.GamePath, "[GAME] " + cleanedGamePath, cleanedGamePath ) );
            }

            ImGui.PopStyleVar( 1 );
            ImGui.EndTabItem();
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

        public void DrawPapsWithHeader( Dictionary<string, Dictionary<string, string>> items, SelectResultType resultType, string name, string id ) {
            foreach( var (subName, subItems) in items ) {
                if( subItems.Count == 0 ) continue;

                if( ImGui.CollapsingHeader( subName, ImGuiTreeNodeFlags.DefaultOpen ) ) {
                    ImGui.Indent( 10 );
                    DrawPaps( subItems, resultType, $"{name} {subName}", $"{id}/{subName}" );
                    ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
                    ImGui.Unindent( 10 );
                }
            }
        }

        public void DrawPaps( Dictionary<string, string> items, SelectResultType resultType, string name, string id ) {
            foreach( var (suffix, path) in items ) {
                DrawFavorite( path, resultType, $"{name} ({suffix})" );
                ImGui.Text( $"{suffix}:" );
                ImGui.SameLine();
                if( path.Contains( "action.pap" ) || path.Contains( "face.pap" ) ) SelectTabUtils.DisplayPathWarning( path, "Be careful about modifying this file, as it contains dozens of animations for every job" );
                else SelectTabUtils.DisplayPath( path );

                DrawPath( "", path, $"{id}{suffix}", resultType, $"{name} ({suffix})" );
            }
        }

        public void DrawPaths( string label, IEnumerable<string> paths, string id, SelectResultType resultType, string resultName, bool play = false ) {
            var idx = 0;
            foreach( var path in paths ) {
                DrawPath( $"{label} #{idx}", path, $"{id}-{idx}", resultType, $"{resultName} #{idx}", play );
                idx++;
            }
        }

        public void DrawPath( string label, string path, string id, SelectResultType resultType, string resultName, bool play = false ) =>
            DrawPath( label, path, path, id, resultType, resultName, play );

        public void DrawPath( string label, string path, string displayPath, string id, SelectResultType resultType, string resultName, bool play = false ) {
            if( string.IsNullOrEmpty( path ) ) return;
            if( path.Contains( "BGM_Null" ) ) return;

            if( !string.IsNullOrEmpty( label ) ) { // if this is blank, assume there is some custom logic to draw the path
                DrawFavorite( path, resultType, resultName );
                ImGui.Text( $"{label}:" );
                ImGui.SameLine();
                SelectTabUtils.DisplayPath( displayPath );
            }

            ImGui.Indent( 25f );

            if( ImGui.Button( $"SELECT{id}" ) ) Invoke( SelectTabUtils.GetSelectResult( path, resultType, resultName ) );
            ImGui.SameLine();
            SelectTabUtils.Copy( path, id + "Copy" );
            if( play ) Play( path, id + "/Spawn" );

            ImGui.Unindent( 25f );
        }

        public void DrawBgmSituation( string name, string parentId, BgmSituationStruct situation ) {
            if( situation.IsSituation ) {
                DrawPath( "Daytime Bgm", situation.DayPath, $"{parentId}/Day", SelectResultType.GameMusic, $"{name} / Day" );
                DrawPath( "Nighttime Bgm", situation.NightPath, $"{parentId}/Night", SelectResultType.GameMusic, $"{name} / Night" );
                DrawPath( "Battle Bgm", situation.BattlePath, $"{parentId}/Battle", SelectResultType.GameMusic, $"{name} / Battle" );
                DrawPath( "Daybreak Bgm", situation.DaybreakPath, $"{parentId}/Break", SelectResultType.GameMusic, $"{name} / Break" );
            }
            else DrawPath( "Bgm", situation.Path, parentId, SelectResultType.GameZone, name );
        }
    }
}
