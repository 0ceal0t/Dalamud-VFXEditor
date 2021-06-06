using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dalamud.Interface;
using Dalamud.Plugin;
using ImGuiNET;

namespace VFXSelect.UI
{
    public enum VFXSelectType
    {
        Local,
        GamePath,
        GameItem,
        GameStatus,
        GameAction,
        GameZone,
        GameEmote,
        GameGimmick,
        GameCutscene,
        GameNpc
    }
    public struct VFXSelectResult
    {
        public VFXSelectType Type;
        public string DisplayString;
        public string Path;

        public VFXSelectResult(VFXSelectType type, string displayString, string path)
        {
            Type = type;
            DisplayString = displayString;
            Path = path;
        }

        public static VFXSelectResult None() {
            var s = new VFXSelectResult();
            s.DisplayString = "[NONE]";
            s.Path = "";
            return s;
        }
    }

    public class VFXSelectDialog
    {
        public bool ShowLocal = true;

        public string Id;
        public event Action<VFXSelectResult> OnSelect;
        public bool Visible = false;

        private bool ShowSpawn = false;
        private Func<bool> SpawnVfxExists;
        private Action RemoveSpawnVfx;
        private Action<string> SpawnOnGround;
        private Action<string> SpawnOnSelf;
        private Action<string> SpawnOnTarget;

        public List<VFXSelectTab> GameTabs;
        public List<VFXSelectResult> RecentList;

        public VFXSelectDialog(
            SheetManager sheet, string id, List<VFXSelectResult> recentList,
            bool showSpawn = false,
            Func<bool> spawnVfxExists = null,
            Action removeSpawnVfx = null,
            Action<string> spawnOnGround = null,
            Action<string> spawnOnSelf = null,
            Action<string> spawnOnTarget = null
            ) {
            Id = id;
            RecentList = recentList;

            ShowSpawn = showSpawn;
            SpawnVfxExists = spawnVfxExists;
            RemoveSpawnVfx = removeSpawnVfx;
            SpawnOnGround = spawnOnGround;
            SpawnOnSelf = spawnOnSelf;
            SpawnOnTarget = spawnOnTarget;

            GameTabs = new List<VFXSelectTab>(new VFXSelectTab[]{
                new VFXItemSelect( id, "Item", sheet, this ),
                new VFXStatusSelect( id, "Status", sheet, this ),
                new VFXActionSelect( id, "Action", sheet, this ),
                new VFXActionSelect( id, "Non-Player Action", sheet, this, nonPlayer:true ),
                new VFXZoneSelect( id, "Zone", sheet, this ),
                new VFXNpcSelect( id, "Npc", sheet, this ),
                new VFXEmoteSelect( id, "Emote", sheet, this ),
                new VFXGimmickSelect( id, "Gimmick", sheet, this ),
                new VFXCutsceneSelect( id, "Cutscene", sheet, this ),
                new VFXMountSelect(id, "Mount", sheet, this),
                new VFXHousingSelect(id, "Housing", sheet, this)
            } );
        }

        public void Show(bool showLocal = true) {
            ShowLocal = showLocal;
            Visible = true;
        }

        public void Draw() {
            if( !Visible )
                return;
            ImGui.SetNextWindowSize( new Vector2( 800, 500 ), ImGuiCond.FirstUseEver );
            // ================
            var ret = ImGui.Begin(Id + "##" + Id, ref Visible );
            if( !ret )
                return;

            ImGui.BeginTabBar( "VFXSelectDialogTabs##" + Id );
            if( ShowLocal )
                DrawLocal();
            DrawGame();
            if( RecentList != null)
                DrawRecent();
            ImGui.EndTabBar();
            ImGui.End();
        }

        // =========== LOCAL ================
        private string localPathInput = "";
        public void DrawLocal() {
            var ret = ImGui.BeginTabItem( "Local File##Select-" + Id );
            if( !ret )
                return;
            // ==========================
            var id = "##Select/Local/" + Id;
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Text( ".avfx file located on your computer, eg: C:/Users/me/Downloads/awesome.avfx" );
            ImGui.Text( "Path" );
            ImGui.SameLine();
            ImGui.InputText( id + "Input", ref localPathInput, 255 );
            ImGui.SameLine();
            if( ImGui.Button( ( "Browse" + id ) ) ) {
                Task.Run( async () => {
                    var picker = new OpenFileDialog {
                        Filter = "AVFX File (*.avfx)|*.avfx*|All files (*.*)|*.*",
                        CheckFileExists = true,
                        Title = "Select AVFX File."
                    };
                    var result = await picker.ShowDialogAsync();
                    if( result == DialogResult.OK ) {
                        try {
                            localPathInput = picker.FileName;
                            Invoke( new VFXSelectResult( VFXSelectType.Local, "[LOCAL] " + localPathInput, localPathInput ) );
                        }
                        catch( Exception ex ){
                            PluginLog.LogError( ex, "Could not select the local file." );
                        }
                    }
                } );
            }
            ImGui.SameLine();
            if( ImGui.Button( "SELECT" + id ) ) {
                Invoke( new VFXSelectResult( VFXSelectType.Local, "[LOCAL] " + localPathInput, localPathInput ) );
            }

            ImGui.EndTabItem();
        }

        // ============= GAME =================
        public void DrawGame() {
            var ret = ImGui.BeginTabItem( "Game File##Select/" + Id );
            if( !ret )
                return;
            // ==========================
            ImGui.BeginTabBar( "GameSelectTabs##" + Id );
            DrawGamePath();
            foreach(var tab in GameTabs ) {
                tab.Draw();
            }
            ImGui.EndTabBar();
            ImGui.EndTabItem();
        }

        // ============== GAME FILE ================
        private string gamePathInput = "";
        public void DrawGamePath() {
            var ret = ImGui.BeginTabItem( "Game File##Select/" + Id );
            if( !ret )
                return;
            // ==========================
            var id = "##Select/GamePath/" + Id;
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Text( "In-game .avfx file, eg: vfx/common/eff/wp_astro1h.avfx" );
            ImGui.Text( "Path" );
            ImGui.SameLine();
            ImGui.InputText( id + "Input", ref gamePathInput, 255 );
            ImGui.SameLine();
            if( ImGui.Button( "SELECT" + id ) )
            {
                Invoke( new VFXSelectResult( VFXSelectType.GamePath, "[GAME] " + gamePathInput, gamePathInput ) );
            }

            ImGui.EndTabItem();
        }
        // ======== RECENT ========
        public VFXSelectResult RecentSelected;
        public bool IsRecentSelected = false;
        public void DrawRecent() {
            var ret = ImGui.BeginTabItem( "Recent##Select/" + Id );
            if( !ret )
                return;
            var id = "##Recent/" + Id;

            float footerHeight = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();
            ImGui.BeginChild( id + "/Child", new Vector2( 0, -footerHeight ), true );
            int idx = 0;
            foreach(var item in RecentList ) {
                if( item.Type == VFXSelectType.Local && !ShowLocal )
                    continue;

                if( ImGui.Selectable(item.DisplayString + id + idx, RecentSelected.Equals(item)) ) {
                    IsRecentSelected = true;
                    RecentSelected = item;
                }
                idx++;
            }
            ImGui.EndChild();
            if( IsRecentSelected ) {
                if( ImGui.Button( "SELECT" + id ) ) {
                    Invoke( RecentSelected );
                }
            }

            ImGui.EndTabItem();
        }

        // ======== UTIL ==========
        public static bool Matches(string item, string query ) {
            return item.ToLower().Contains( query.ToLower() );
        }
        public void Invoke( VFXSelectResult result ) {
            OnSelect?.Invoke( result );
        }
        public void DisplayPath(string path ) {
            ImGui.PushStyleColor( ImGuiCol.Text, new Vector4( 0.8f, 0.8f, 0.8f, 1 ) );
            ImGui.TextWrapped( path );
            ImGui.PopStyleColor();
        }
        public void Copy(string copyPath, string id = "" ) {
            ImGui.PushStyleColor( ImGuiCol.Button, new Vector4( 0.15f, 0.15f, 0.15f, 1 ) );
            if(ImGui.Button("Copy##" + id ) )
            {
                ImGui.SetClipboardText( copyPath );
            }
            ImGui.PopStyleColor();
        }
        public void Spawn(string spawnPath, string id = "") {
            if( !ShowSpawn ) return;
            ImGui.SameLine();
            if(SpawnVfxExists()) {
                if(ImGui.Button("Remove##" + id)) {
                    RemoveSpawnVfx();
                }
            }
            else {
                if(ImGui.Button("Spawn##" + id ) ) {
                    ImGui.OpenPopup( "Spawn_Popup##" + id );
                }
                if( ImGui.BeginPopup( "Spawn_Popup##" + id ) ) {
                    if( ImGui.Selectable( "On Ground" ) ) {
                        SpawnOnGround( spawnPath );
                    }
                    if( ImGui.Selectable( "On Self" ) ) {
                        SpawnOnSelf( spawnPath );
                    }
                    if( ImGui.Selectable( "On Target" ) ) {
                        SpawnOnTarget( spawnPath );
                    }
                    ImGui.EndPopup();
                }
            }
        }
        public static void DisplayVisible(int count, out int preItems, out int showItems, out int postItems, out float itemHeight) {
            float childHeight = ImGui.GetWindowSize().Y - ImGui.GetCursorPosY();
            var scrollY = ImGui.GetScrollY();
            var style = ImGui.GetStyle();
            itemHeight = ImGui.GetTextLineHeight() + style.ItemSpacing.Y;
            preItems = ( int )Math.Floor( scrollY / itemHeight );
            showItems = ( int )Math.Ceiling( childHeight / itemHeight );
            postItems = count - showItems - preItems;

        }
    }
}
