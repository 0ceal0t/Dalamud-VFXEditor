using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dalamud.Interface;
using Dalamud.Plugin;
using ImGuiNET;
using Newtonsoft.Json.Linq;

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
            VFXMain = new VFX.UIMain( _plugin.AVFX, _plugin );
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
            DrawRaw();
            DrawSettings();
            DrawHelp();
            ImGui.EndTabBar();
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( VFXMain == null )
            {
                ImGui.Text( "Load a file..." );
            }
            else
            {
                ImGui.PushStyleColor( ImGuiCol.Button, new Vector4( 0.15f, 0.90f, 0.15f, 1.0f ) );
                if( ImGui.Button( "UPDATE" ) )
                {
                    _plugin.Manager.SaveTempFile( _plugin.AVFX );
                    _plugin.ResourceLoader.ReRender();
                }
                ImGui.PopStyleColor();
                ImGui.SameLine();
                if( ImGui.Button( "Export" ) )
                {
                    ImGui.OpenPopup( "Export_Popup" );
                }
                // ======= EXPORT ==========
                if( ImGui.BeginPopup( "Export_Popup" ) )
                {
                    if( ImGui.Selectable( ".AVFX" ) )
                    {
                        var node = _plugin.AVFX.toAVFX();
                        SaveDialog( "AVFX File (*.avfx)|*.avfx*|All files (*.*)|*.*", node.toBytes(), "avfx" );
                    }
                    if( ImGui.Selectable( ".JSON" ) )
                    {
                        JObject json = ( JObject )_plugin.AVFX.toJSON();
                        SaveDialog( "JSON files (*.json)|*.json|All files (*.*)|*.*", json.ToString(), "json" );
                    }
                    if(ImGui.Selectable("TexTools Mod" ) )
                    {
                        _plugin.TexToolsUI.Show();
                    }
                    ImGui.EndPopup();
                }
#if DEBUG
                ImGui.SameLine();
                if( ImGui.Button( "[DEBUG] Export Raw" ) )
                {
                    SaveDialog( "TXT files (*.txt)|*.txt|All files (*.*)|*.*", _plugin.Manager.LastImportNode.exportString( 0 ), "txt" );
                }
#endif
                if( _plugin.Configuration.VerifyOnLoad )
                {
                    ImGui.SameLine();
                    ImGui.PushFont( UiBuilder.IconFont );
                    ImGui.TextColored( StatusColor, IconText );
                    ImGui.PopFont();
                    ImGui.SameLine();
                    ImGui.TextColored( StatusColor, StatusText );
                }

                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
                ImGui.Separator();
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
                //================================
                VFXMain.Draw();
            }
            ImGui.End();
        }

        public bool Status = false;
        public string IconText = "";
        public string StatusText = "";
        public Vector4 StatusColor = new Vector4();
        public void SetStatus(bool status )
        {
            Status = status;
            if( Status )
            {
                IconText = $"{( char )FontAwesomeIcon.Check}";
                StatusText = "Verified";
                StatusColor = new Vector4( 0.15f, 0.90f, 0.15f, 1.0f );
            }
            else
            {
                IconText = $"{( char )FontAwesomeIcon.Times}";
                StatusText = "Parsing Issue";
                StatusColor = new Vector4( 0.90f, 0.15f, 0.15f, 1.0f );
            }
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
            ImGui.SameLine();
            if( ImGui.Button( "New##MainInterfaceFiles-New" ) )
            {
                VFXSelectResult newResult = new VFXSelectResult();
                newResult.DisplayString = "[NEW]";
                newResult.Type = VFXSelectType.Local;
                newResult.Path = Path.Combine( _plugin.Location.ToString(), @"Templates\default_vfx.avfx" );
                //newResult.Path = @"D:\FFXIV\TOOLS\Dalamud-VFXEditor\VFXEditor\bin\Debug\net472\Templates\default_vfx.avfx";
                _plugin.SelectAVFX( newResult );
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

        public string RawInputValue = "";
        public void DrawRaw()
        {
            var ret = ImGui.BeginTabItem( "Raw##MainInterfaceTabs" );
            if( !ret )
                return;
            // =================
            ImGui.Text( "Extract a raw .avfx file from the game and save it locally" );
            ImGui.InputText( "Path##RawExtract", ref RawInputValue, 255 );
            ImGui.SameLine();
            if( ImGui.Button( "Extract##RawExtract" ) )
            {
                bool result = _plugin.PluginInterface.Data.FileExists( RawInputValue );
                if( result )
                {
                    try
                    {
                        var file = _plugin.PluginInterface.Data.GetFile( RawInputValue );
                        SaveDialog( "AVFX File (*.avfx)|*.avfx*|All files (*.*)|*.*", file.Data, "avfx" );
                    }
                    catch(Exception e )
                    {
                        PluginLog.LogError( "Could not read file" );
                        PluginLog.LogError( e.ToString() );
                    }
                }
            }
            ImGui.EndTabItem();
        }

        public void DrawSettings()
        {
            var ret = ImGui.BeginTabItem( "Settings##MainInterfaceTabs" );
            if( !ret )
                return;
            // ==========================
            bool verifyOnLoad = _plugin.Configuration.VerifyOnLoad;
            if(ImGui.Checkbox( "Verify on load##Settings", ref verifyOnLoad ) )
            {
                _plugin.Configuration.VerifyOnLoad = verifyOnLoad;
            }
            ImGui.SameLine();
            bool loadTextures = _plugin.Configuration.PreviewTextures;
            if( ImGui.Checkbox( "Load textures##Settings", ref loadTextures ) )
            {
                _plugin.Configuration.PreviewTextures = loadTextures;
            }
            if( ImGui.Button( "Save##Settings" ) )
            {
                _plugin.Configuration.Save();
            }
            ImGui.EndTabItem();
        }

        public void DrawHelp()
        {
            var ret = ImGui.BeginTabItem( "Help##MainInterfaceTabs" );
            if( !ret )
                return;
            // ==========================
            if( ImGui.Button( "Github" ) )
            {
                Process.Start( "https://github.com/0ceal0t/Dalamud-VFXEditor/issues" );
            }
            ImGui.TextWrapped( @"This plugin works by replacing an existing VFX with another one. It does not, however, actually modify any of the game's internal files.
If you want to make the modification permanent, you will need to create a mod using your platform of choice.

If you are having issues loading a VFX, please open a Github issue. Make sure to specify either the in-game path of the VFX file or attach the file directly."
        );
            ImGui.EndTabItem();
        }

        // ======= HELPERS ============
        public void SaveDialog( string filter, string data, string ext )
        {
            SaveDialog( filter, Encoding.ASCII.GetBytes(data), ext );
        }
        public void SaveDialog(string filter, byte[] data, string ext )
        {
            Task.Run( async () =>
            {
                var picker = new SaveFileDialog
                {
                    Filter = filter,
                    Title = "Select a Save Location.",
                    DefaultExt = ext,
                    AddExtension = true
                };
                var result = await picker.ShowDialogAsync();
                if( result == DialogResult.OK )
                {
                    try
                    {
                        File.WriteAllBytes( picker.FileName, data );
                    }
                    catch( Exception ex )
                    {
                        PluginLog.LogError( ex, "Could not save to: " + picker.FileName );
                    }
                }
            } );
        }
    }
}