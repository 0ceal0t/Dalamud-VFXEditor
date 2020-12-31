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
    public class VFXSelectDialog
    {
        public Plugin _plugin;
        public bool ShowLocal = true;
        public bool ShowGamePath = true;
        public bool ShowGameItems = true;
        public string Id;
        
        public bool Visible = false;

        public VFXSelectDialog(Plugin plugin, string id)
        {
            _plugin = plugin;
            Id = id;
            // ===========================
        }

        public void Show(bool showLocal = true, bool showGamePath = true, bool showGameItems = true)
        {
            ShowLocal = showLocal;
            ShowGamePath = showGamePath;
            ShowGameItems = showGameItems;
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
                ImGui.SetNextWindowSize( new Vector2( 600, 500 ) );
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
            ImGui.EndTabBar();

            ImGui.End();
        }

        private string localPathInput = "";
        public void DrawLocal()
        {
            var ret = ImGui.BeginTabItem( "Local File##Select-" + Id );
            if( !ret )
                return;
            // ==========================
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Text( "Path" );
            ImGui.SameLine();
            ImGui.InputText( "##Select/Local/" + Id, ref localPathInput, 255 );
            ImGui.SameLine();
            if( ImGui.Button( ( "Browse##Select/Local/" + Id ) ) )
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
            if( ImGui.Button( "LOAD##Select/Local/" + Id ) )
            {
                bool result = _plugin.Manager.GetLocalFile( localPathInput, out var avfx );
                if( result )
                {
                    _plugin.MainUI.sourceString = "[LOCAL] " + localPathInput;
                    _plugin.LoadAVFX( avfx );
                }
            }

            ImGui.EndTabItem();
        }

        private string gamePathInput = "";
        public void DrawGamePath()
        {
            var ret = ImGui.BeginTabItem( "Game File##Select/" + Id );
            if( !ret )
                return;
            // ==========================
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Text( "Path" );
            ImGui.SameLine();
            ImGui.InputText( "##Select/GamePath/" + Id, ref gamePathInput, 255 );
            ImGui.SameLine();
            if( ImGui.Button( "LOAD##Select/GamePath/" + Id ) )
            {
                bool result = _plugin.Manager.GetGameFile( gamePathInput, out var avfx );
                if( result )
                {
                    _plugin.MainUI.sourceString = "[GAME] " + gamePathInput;
                    _plugin.LoadAVFX( avfx );
                }
            }

            ImGui.EndTabItem();
        }

        public string gameItemsSearchInput = "";
        public XivItem SelectedItem = null;
        public void DrawGameItems()
        {
            var ret = ImGui.BeginTabItem( "Game Item##Select/" + Id );
            if( !ret )
                return;
            // ==========================
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.InputText( "Search##Select/GameItemSearch/" + Id, ref gameItemsSearchInput, 255 );
            ImGui.Columns( 2, "##Select/GameItemColumns/" + Id, true );

            //
            ImGui.BeginChild("##Select/GameItemsTree/" + Id);
            foreach(var item in _plugin.Manager.Items )
            {
                if( !item.Name.Contains( gameItemsSearchInput ) )
                    continue;

                if(ImGui.Selectable(item.Name, SelectedItem == item ) )
                {
                    SelectedItem = item;
                }
            }
            ImGui.EndChild();
            ImGui.NextColumn();

            if( SelectedItem == null ) {
                ImGui.Text( "Select an item..." );
            }
            else
            {
                if( ImGui.Button( "LOAD##Select-GameItem/" + Id ) )
                {
                    // ....
                }
            }
            ImGui.Columns( 1 );

            ImGui.EndTabItem();
        }
    }
}
