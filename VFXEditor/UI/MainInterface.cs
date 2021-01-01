using System;
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
    public class MainInterface
    {
        private readonly Plugin _plugin;
        public bool Visible = false;
        public bool ShowDebugBar = false;
        public VFX.UIMain VFXMain = null;

        public MainInterface( Plugin plugin )
        {
            _plugin = plugin;
#if DEBUG
            Visible = true;
            ShowDebugBar = true;
#endif
        }

        public void RefreshAVFX()
        {
            VFXMain = new VFX.UIMain( _plugin.AVFX );
        }
        public void UnloadAVFX()
        {
            VFXMain = null;
        }

        public void Draw()
        {
            DrawDebugBar();
            if( !Visible )
                return;
            // =====================
            DrawStartInterface();
        }

        public void DrawDebugBar()
        {
            if( ShowDebugBar && ImGui.BeginMainMenuBar() )
            {
                if( ImGui.BeginMenu( "VFXEditor" ) )
                {
                    if( ImGui.MenuItem( "Toggle UI", "/VFXEditor", Visible ) )
                    {
                        Visible = !Visible;
                    }
                    ImGui.EndMenu();
                }
                ImGui.EndMainMenuBar();
            }
        }

        public bool DrawOnce = false;
        public void DrawStartInterface()
        {
            if( !DrawOnce )
            {
                ImGui.SetNextWindowSize( new Vector2( 800, 1000 ) );
                DrawOnce = true;
            }
#if DEBUG
            var ret = ImGui.Begin( _plugin.PluginDebugTitleStr, ref Visible );
#else
            var ret = ImGui.Begin( _plugin.Name, ref Visible );
#endif
            if( !ret )
                return;
            // ==================
            ImGui.BeginTabBar( "MainInterfaceTabs" );
            DrawFiles();
            DrawSettings();
            DrawHelp();
            ImGui.EndTabBar();

            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            ImGui.BeginChild( "MainChild" );
            if(VFXMain == null )
            {
                ImGui.Text( "Load a file..." );
            }
            else
            {
                ImGui.PushStyleColor( ImGuiCol.Button, new Vector4( 0.8f, 0.25f, 0.25f, 1 ) );
                if( ImGui.Button( "UPDATE" ) )
                {
                    _plugin.Manager.SaveTempFile( _plugin.AVFX );
                    _plugin.ResourceLoader.ReRender();
                }
                ImGui.PopStyleColor();
                ImGui.SameLine();
                if( ImGui.Button( "Export" ) )
                {
                    Task.Run( async () =>
                    {
                        var picker = new SaveFileDialog
                        {
                            Filter = "AVFX File (*.avfx)|*.avfx*|All files (*.*)|*.*",
                            Title = "Select a Save Location."
                        };
                        var result = await picker.ShowDialogAsync();
                        if( result == DialogResult.OK )
                        {
                            try
                            {
                                _plugin.Manager.SaveLocalFile( picker.FileName, _plugin.AVFX );
                            }
                            catch( Exception ex )
                            {
                                PluginLog.LogError( ex, "Could not select a save location file." );
                            }
                        }
                    } );
                }

                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
                ImGui.Separator();
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
                //================================
                VFXMain.Draw();
            }
            ImGui.EndChild();

            ImGui.End();
        }

        public string sourceString = "[NONE]";
        public string previewString = "[NONE]";
        public void DrawFiles()
        {
            var ret = ImGui.BeginTabItem( "Files##MainInterfaceTabs" );
            if( !ret )
                return;
            // ==========================
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Columns( 3, "MainInterfaceFileColumns", false );

            ImGui.SetColumnWidth( 0, 80 );
            ImGui.Text( "Source" );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Text( "Replace" );
            ImGui.NextColumn();

            ImGui.SetColumnWidth( 1, 500 );
            ImGui.PushItemWidth( ImGui.GetColumnWidth() );
            ImGui.InputText( "##MainInterfaceFiles-Source", ref sourceString, 255, ImGuiInputTextFlags.ReadOnly );
            ImGui.PushItemWidth( ImGui.GetColumnWidth() );
            ImGui.InputText( "##MainInterfaceFiles-Preview", ref previewString, 255, ImGuiInputTextFlags.ReadOnly );
            ImGui.NextColumn();

            ImGui.SetColumnWidth( 2, 200 );
            if( ImGui.Button( "Select##MainInterfaceFiles-SourceSelect" ) )
            {
                _plugin.SelectUI.Show();
            }
            if( ImGui.Button( "Select##MainInterfaceFiles-PreviewSelect" ) )
            {
                _plugin.PreviewUI.Show( showLocal: false );
            }
            ImGui.SameLine();
            if( ImGui.Button( "Reset##MainInterfaceFiles-PreviewRemove" ) )
            {
                _plugin.RemoveReplaceAVFX();
            }

            ImGui.Columns( 1 );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.EndTabItem();
        }

        public void DrawSettings()
        {
            var ret = ImGui.BeginTabItem( "Settings##MainInterfaceTabs" );
            if( !ret )
                return;
            // ==========================

            ImGui.EndTabItem();
        }

        public void DrawHelp()
        {
            var ret = ImGui.BeginTabItem( "Help##MainInterfaceTabs" );
            if( !ret )
                return;
            // ==========================

            ImGui.EndTabItem();
        }
    }
}