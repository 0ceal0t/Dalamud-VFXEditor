using ImGuiFileDialog;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.Dialogs;

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

        public static SelectResult None() => new() {
            DisplayString = "[NONE]",
            Path = ""
        };
    }

    public abstract class SelectDialog : GenericDialog {
        private readonly string Name;
        private readonly string Extension;
        private readonly bool ShowLocal;
        private readonly List<SelectResult> RecentList;

        protected Action<SelectResult> OnSelect;
        protected abstract List<SelectTab> GetTabs();
        protected SelectResult RecentSelected;
        protected bool IsRecentSelected = false;

        public SelectDialog( string name, string extension, List<SelectResult> recentList, bool showLocal, Action<SelectResult> onSelect ) : base( name, startingWidth:800, startingHeight:500 ) {
            Name = name;
            Extension = extension;
            RecentList = recentList;
            ShowLocal = showLocal;
            OnSelect = onSelect;
        }

        public override void DrawBody() {
            var id = $"##{Name}";
            ImGui.BeginTabBar( $"Tabs{id}" );
            DrawGame( id );
            DrawGamePath( id );
            if( ShowLocal ) DrawLocal( id );
            if( RecentList != null ) DrawRecent( id );
            ImGui.EndTabBar();
        }

        // =========== LOCAL ================

        private string LocalPath = "";
        private void DrawLocal( string parentId ) {
            var id = $"{parentId}/Local";

            var ret = ImGui.BeginTabItem( "Local File" + id );
            if( !ret ) return;

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Text( $".{Extension} file located on your computer, eg: C:/Users/me/Downloads/awesome.{Extension}" );
            ImGui.Text( "Path" );
            ImGui.SameLine();
            ImGui.InputText( id + "-Input", ref LocalPath, 255 );
            ImGui.SameLine();
            if( ImGui.Button( ( "Browse" + id ) ) ) {
                FileDialogManager.OpenFileDialog( "Select a File", $".{Extension},.*", ( bool ok, string res ) => {
                    if( !ok ) return;
                    Invoke( new SelectResult( SelectResultType.Local, "[LOCAL] " + res, res ) );
                } );
            }
            ImGui.SameLine();
            if( ImGui.Button( "SELECT" + id ) ) Invoke( new SelectResult( SelectResultType.Local, "[LOCAL] " + LocalPath, LocalPath ) );

            ImGui.EndTabItem();
        }

        // ============= GAME =================

        public void DrawGame( string parentId ) {
            var id = $"{parentId}/Game";

            if( GetTabs().Count == 0 ) return;
            var ret = ImGui.BeginTabItem( "Game Items" + id );
            if( !ret ) return;

            ImGui.BeginTabBar( "Tabs" + id );
            foreach( var tab in GetTabs() ) tab.Draw( id );
            ImGui.EndTabBar();
            ImGui.EndTabItem();
        }

        // ============== GAME FILE =============

        private string GamePath = "";
        public void DrawGamePath( string parentId ) {
            var id = $"{parentId}/Path";

            var ret = ImGui.BeginTabItem( "Game Path" + id );
            if( !ret ) return;

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Text( $"In-game .{Extension} file, eg: vfx/common/eff/wp_astro1h.{Extension}" );
            ImGui.Text( "Path" );
            ImGui.SameLine();
            ImGui.InputText( id + "-Input", ref GamePath, 255 );
            ImGui.SameLine();
            if( ImGui.Button( "SELECT" + id ) ) {
                var cleanedGamePath = GamePath.Replace( "\\", "/" );
                Invoke( new SelectResult( SelectResultType.GamePath, "[GAME] " + cleanedGamePath, cleanedGamePath ) );
            }

            ImGui.EndTabItem();
        }

        // ======== RECENT ========

        public void DrawRecent( string parentId ) {
            var id = $"{parentId}/Recent";

            var ret = ImGui.BeginTabItem( "Recent##Select/" + Name );
            if( !ret ) return;

            var footerHeight = ImGui.GetFrameHeightWithSpacing();
            ImGui.BeginChild( id + "/Child", new Vector2( 0, -footerHeight ), true );

            var idx = 0;
            foreach( var item in RecentList ) {
                if( item.Type == SelectResultType.Local && !ShowLocal ) continue;

                if( ImGui.Selectable( item.DisplayString + id + idx, RecentSelected.Equals( item ) ) ) {
                    IsRecentSelected = true;
                    RecentSelected = item;
                }
                idx++;
            }
            ImGui.EndChild();

            // Disable button if nothing selected
            if( !IsRecentSelected ) ImGui.PushStyleVar( ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.5f );
            if( ImGui.Button( "SELECT" + id ) && IsRecentSelected ) Invoke( RecentSelected );
            if( !IsRecentSelected ) ImGui.PopStyleVar();

            ImGui.EndTabItem();
        }

        public void Invoke( SelectResult result ) => OnSelect?.Invoke( result );

        public virtual void Play( string playPath, string id ) { }
    }
}
