using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dalamud.Interface;
using Dalamud.Plugin;
using ImGuiNET;

namespace VFXEditor.UI
{
    public enum VFXSelectType
    {
        Local,
        GamePath,
        GameItem,
        GameStatus,
        GameAction
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
    }

    public class VFXSelectDialog
    {
        public Plugin _plugin;
        public bool ShowLocal = true;
        public bool ShowGamePath = true;
        public bool ShowGameItems = true;
        public bool ShowGameStatus = true;
        public bool ShowGameActions = true;
        public bool ShowGameNonPlayerActions = true;
        public bool ShowRecent = true;

        public string Id;
        public event Action<VFXSelectResult> OnSelect;
        public bool Visible = false;

        public VFXActionSelect NonPlayerActionSelect;
        public VFXActionSelect ActionSelect;
        public VFXStatusSelect StatusSelect;
        public VFXItemSelect ItemSelect;

        public VFXSelectDialog(Plugin plugin, string id)
        {
            _plugin = plugin;
            Id = id;
            // ===========================
            NonPlayerActionSelect = new VFXActionSelect( id, "GameNPAction", _plugin.Manager.NonPlayerActions, _plugin, this );
            ActionSelect = new VFXActionSelect( id, "GameAction", _plugin.Manager.Actions, _plugin, this );
            StatusSelect = new VFXStatusSelect( id, "GameStatus", _plugin.Manager.Status, _plugin, this );
            ItemSelect = new VFXItemSelect( id, "GameItem", _plugin.Manager.Items, _plugin, this );
        }

        public void Show(
            bool showLocal = true,
            bool showGamePath = true,
            bool showGameItems = true,
            bool showGameStatus = true,
            bool showGameActions = true,
            bool showGameNonPlayerActions = true,
            bool showRecent = true
        )
        {
            ShowLocal = showLocal;
            ShowGamePath = showGamePath;
            ShowGameItems = showGameItems;
            ShowGameStatus = showGameStatus;
            ShowGameActions = showGameActions;
            ShowGameNonPlayerActions = showGameNonPlayerActions;
            ShowRecent = showRecent;
            // ======================
            Visible = true;
        }

        public bool DrawOnce = false;
        public void Draw()
        {
            if( !Visible )
                return;
            if( !DrawOnce )
            {
                ImGui.SetNextWindowSize( new Vector2( 800, 500 ) );
                DrawOnce = true;
            }
            // ================
            var ret = ImGui.Begin("File Select##" + Id, ref Visible );
            if( !ret )
                return;

            ImGui.BeginTabBar( "VFXSelectDialogTabs##" + Id );
            if( ShowLocal )
                DrawLocal();
            if( ShowGamePath )
                DrawGamePath();
            if( ShowGameItems )
                DrawGameItems();
            if( ShowGameStatus )
                DrawGameStatus();
            if( ShowGameActions )
                DrawGameActions();
            if( ShowGameNonPlayerActions )
                DrawGameNonPlayerActions();
            if( ShowRecent )
                DrawRecent();
            ImGui.EndTabBar();

            ImGui.End();
        }

        // =========== LOCAL ================
        private string localPathInput = "";
        public void DrawLocal()
        {
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
            if( ImGui.Button( ( "Browse" + id ) ) )
            {
                Task.Run( async () =>
                {
                    var picker = new OpenFileDialog
                    {
                        Filter = "AVFX File (*.avfx)|*.avfx*|All files (*.*)|*.*",
                        CheckFileExists = true,
                        Title = "Select AVFX File."
                    };
                    var result = await picker.ShowDialogAsync();
                    if( result == DialogResult.OK )
                    {
                        try
                        {
                            localPathInput = picker.FileName;
                        }
                        catch( Exception ex )
                        {
                            PluginLog.LogError( ex, "Could not select the local file." );
                        }
                    }
                } );
            }
            ImGui.SameLine();
            if( ImGui.Button( "SELECT" + id ) )
            {
                Invoke( new VFXSelectResult( VFXSelectType.Local, "[LOCAL] " + localPathInput, localPathInput ) );
            }

            ImGui.EndTabItem();
        }

        // ============== GAME FILE ================
        private string gamePathInput = "";
        public void DrawGamePath()
        {
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

        // =========== GAME ITEM =============
        public string gameItemsSearchInput = "";
        public XivItem SelectedItem = null;
        public XivSelectedItem LoadedItem = null;
        public void DrawGameItems()
        {
            var ret = ImGui.BeginTabItem( "Game Item##Select/" + Id );
            if( !ret )
                return;
            _plugin.Manager.LoadItems();
            if( !_plugin.Manager.ItemsLoaded )
            {
                ImGui.EndTabItem();
                return;
            }
            ItemSelect.Draw();
            ImGui.EndTabItem();
        }

        // =========== GAME STATUS ================
        public void DrawGameStatus()
        {
            var ret = ImGui.BeginTabItem( "Game Status##Select/" + Id );
            if( !ret )
                return;
            _plugin.Manager.LoadStatus();
            if( !_plugin.Manager.StatusLoaded )
            {
                ImGui.EndTabItem();
                return;
            }
            StatusSelect.Draw();
            ImGui.EndTabItem();
        }

        // =========== GAME ACTIONS =============
        public void DrawGameActions()
        {
            var ret = ImGui.BeginTabItem( "Game Action##Select/" + Id );
            if( !ret )
                return;
            _plugin.Manager.LoadActions();
            if( !_plugin.Manager.ActionsLoaded )
            {
                ImGui.EndTabItem();
                return;
            }
            ActionSelect.Draw();
            ImGui.EndTabItem();
        }

        // =========== GAME NON PLAYER ACTIONS =============
        public void DrawGameNonPlayerActions()
        {
            var ret = ImGui.BeginTabItem( "Game Non-Player Action##Select/" + Id );
            if( !ret )
                return;
            _plugin.Manager.LoadNonPlayerActions();
            if( !_plugin.Manager.NonPlayerActionsLoaded )
            {
                ImGui.EndTabItem();
                return;
            }
            NonPlayerActionSelect.Draw();
            ImGui.EndTabItem();
        }

        // ======== RECENT ========
        public VFXSelectResult RecentSelected;
        public bool IsRecentSelected = false;
        public void DrawRecent()
        {
            var ret = ImGui.BeginTabItem( "Recent##Select/" + Id );
            if( !ret )
                return;
            var id = "##Recent/" + Id;

            float footerHeight = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();
            ImGui.BeginChild( id + "/Child", new Vector2( 0, -footerHeight ), false );

            int idx = 0;
            foreach(var item in _plugin.Configuration.RecentSelects )
            {
                // skip local
                if( item.Type == VFXSelectType.Local && !ShowLocal )
                    continue;

                if( ImGui.Selectable(item.DisplayString + id + idx, RecentSelected.Equals(item)) ) {
                    IsRecentSelected = true;
                    RecentSelected = item;
                }
                idx++;
            }

            ImGui.EndChild();
            if( IsRecentSelected )
            {
                ImGui.Separator();
                if( ImGui.Button( "SELECT" + id ) )
                {
                    OnSelect?.Invoke( RecentSelected );
                }
            }

            ImGui.EndTabItem();
        }

        // ======== UTIL ==========
        public static bool Matches(string item, string query )
        {
            return item.ToLower().Contains( query.ToLower() );
        }
        public void Invoke( VFXSelectResult result )
        {
            OnSelect?.Invoke( result );
            _plugin.Configuration.AddRecent( result );
        }
        public void DisplayPath(string path )
        {
            ImGui.PushStyleColor( ImGuiCol.Text, new Vector4( 0.8f, 0.8f, 0.8f, 1 ) );
            ImGui.TextWrapped( path );
            ImGui.PopStyleColor();
        }
        public void Copy(string copyPath, string id = "" )
        {
            ImGui.PushStyleColor( ImGuiCol.Button, new Vector4( 0.15f, 0.15f, 0.15f, 1 ) );
            if(ImGui.Button("Copy##" + id ) )
            {
                ImGui.SetClipboardText( copyPath );
            }
            ImGui.PopStyleColor();
        }
    }
}
