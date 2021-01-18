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
        GameAction,
        GameZone,
        GameEmote,
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
        public Plugin _plugin;
        public bool ShowLocal = true;
        public bool ShowRecent = true;

        public string Id;
        public event Action<VFXSelectResult> OnSelect;
        public bool Visible = false;

        public VFXActionSelect NonPlayerActionSelect;
        public VFXActionSelect ActionSelect;
        public VFXStatusSelect StatusSelect;
        public VFXItemSelect ItemSelect;
        public VFXZoneSelect ZoneSelect;
        public VFXNpcSelect NpcSelect;
        public VFXEmoteSelect EmoteSelect;

        public VFXSelectDialog(Plugin plugin, string id)
        {
            _plugin = plugin;
            Id = id;
            NonPlayerActionSelect = new VFXActionSelect( id, "Non-Player Action", _plugin.Manager.NonPlayerActions, _plugin, this );
            ActionSelect = new VFXActionSelect( id, "Action", _plugin.Manager.Actions, _plugin, this );
            StatusSelect = new VFXStatusSelect( id, "Status", _plugin.Manager.Status, _plugin, this );
            ItemSelect = new VFXItemSelect( id, "Item", _plugin.Manager.Items, _plugin, this );
            ZoneSelect = new VFXZoneSelect( id, "Zone", _plugin.Manager.Zones, _plugin, this );
            NpcSelect = new VFXNpcSelect( id, "Npc", _plugin.Manager.Npcs, _plugin, this );
            EmoteSelect = new VFXEmoteSelect( id, "Emote", _plugin.Manager.Emotes, _plugin, this );
        }

        public void Show(bool showLocal = true, bool showRecent = true) {
            ShowLocal = showLocal;
            ShowRecent = showRecent;
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
            var ret = ImGui.Begin(Id + "##" + Id, ref Visible );
            if( !ret )
                return;

            ImGui.BeginTabBar( "VFXSelectDialogTabs##" + Id );
            if( ShowLocal )
                DrawLocal();
            DrawGame();
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
                            Invoke( new VFXSelectResult( VFXSelectType.Local, "[LOCAL] " + localPathInput, localPathInput ) );
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

        // ============= GAME =================
        public void DrawGame()
        {
            var ret = ImGui.BeginTabItem( "Game##Select/" + Id );
            if( !ret )
                return;
            // ==========================
            ImGui.BeginTabBar( "GameSelectTabs##" + Id );
            DrawGamePath();
            ItemSelect.Draw();
            StatusSelect.Draw();
            ActionSelect.Draw();
            NonPlayerActionSelect.Draw();
            ZoneSelect.Draw();
            NpcSelect.Draw();
            EmoteSelect.Draw();
            ImGui.EndTabBar();
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
            ImGui.BeginChild( id + "/Child", new Vector2( 0, -footerHeight ), true );
            int idx = 0;
            foreach(var item in _plugin.Configuration.RecentSelects )
            {
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
        public static void DisplayVisible(int count, out int preItems, out int showItems, out int postItems, out float itemHeight)
        {
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
