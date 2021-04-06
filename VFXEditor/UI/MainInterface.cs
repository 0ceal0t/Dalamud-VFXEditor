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
using VFXEditor.Structs.Vfx;
using VFXEditor.UI.Graphics;

namespace VFXEditor.UI
{
    public class MainInterface
    {
        private readonly Plugin _plugin;
        public bool Visible = false;
        public bool ShowDebugBar = false;
        public VFX.UIMain VFXMain = null;

        public MainInterface MainUI;
        public VFXSelectDialog SelectUI;
        public VFXSelectDialog PreviewUI;
        public TexToolsDialog TexToolsUI;
        public PenumbraDialog PenumbraUI;
        public DocDialog DocUI;
        public TextureDialog TextureUI;

        public VFXManipulator VFXManip = null;
        public BaseVfx SpawnVfx = null;

        public MainInterface( Plugin plugin )
        {
            _plugin = plugin;
            SelectUI = new VFXSelectDialog( _plugin, "File Select [SOURCE]" );
            PreviewUI = new VFXSelectDialog( _plugin, "File Select [TARGET]" );
            SelectUI.OnSelect += _plugin.SelectAVFX;
            PreviewUI.OnSelect += _plugin.ReplaceAVFX;

            TexToolsUI = new TexToolsDialog( _plugin );
            PenumbraUI = new PenumbraDialog( _plugin );
            DocUI = new DocDialog( _plugin );
            TextureUI = new TextureDialog( _plugin );
            VFXManip = new VFXManipulator( _plugin );

            VFX.UINodeGraphView.InitTex( _plugin ); // load grid texture
#if DEBUG
            Visible = true;
            ShowDebugBar = true;
#endif
        }
        public void RefreshAVFX() {
            VFXMain = new VFX.UIMain( _plugin.AVFX, _plugin );
        }
        public void UnloadAVFX() {
            VFXMain = null;
        }

        public void Draw() {
            if( ShowDebugBar && ImGui.BeginMainMenuBar() ) {
                if( ImGui.BeginMenu( "VFXEditor" ) ) {
                    if( ImGui.MenuItem( "Toggle UI", "/VFXEditor", Visible ) ) {
                        Visible = !Visible;
                    }
                    ImGui.EndMenu();
                }
                ImGui.EndMainMenuBar();
            }
            if( Visible ) {
                DrawMainInterface();
            }
            SelectUI.Draw();
            PreviewUI.Draw();
            TexToolsUI.Draw();
            PenumbraUI.Draw();
            DocUI.Draw();
            TextureUI.Draw();
            VFXManip.Draw();
        }

        public DateTime LastUpdate = DateTime.Now;
        public void DrawMainInterface() {
            ImGui.SetNextWindowSize( new Vector2( 800, 1000 ), ImGuiCond.FirstUseEver );
#if DEBUG
            var ret = ImGui.Begin( _plugin.PluginDebugTitleStr, ref Visible );
#else
            var ret = ImGui.Begin( _plugin.Name, ref Visible );
#endif
            if( !ret ) return;
            // ==================
            ImGui.BeginTabBar( "MainInterfaceTabs" );
            DrawFiles();
            DrawRaw();
            DrawSettings();
            DrawHelp();
            ImGui.EndTabBar();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( VFXMain == null ) {
                ImGui.Text( "Select a source file to begin..." );
            }
            else {
                ImGui.PushStyleColor( ImGuiCol.Button, new Vector4( 0.10f, 0.80f, 0.10f, 1.0f ) );
                if( ImGui.Button( "UPDATE" ) ) {
                    if((DateTime.Now - LastUpdate).TotalSeconds > 0.5  ) { // only allow updates every 1/2 second
                        _plugin.Doc.Save();
                        _plugin.ResourceLoader.ReRender();
                        LastUpdate = DateTime.Now;
                    }
                }
                ImGui.PopStyleColor();
                // ===== EXPORT ======
                ImGui.SameLine();
                ImGui.PushFont( UiBuilder.IconFont );
                if( ImGui.Button( $"{( char )FontAwesomeIcon.Save}" ) ) {
                    ImGui.OpenPopup( "Export_Popup" );
                }
                ImGui.PopFont();

                if( ImGui.BeginPopup( "Export_Popup" ) ) {
                    if( ImGui.Selectable( ".AVFX" ) ) {
                        var node = _plugin.AVFX.toAVFX();
                        SaveDialog( "AVFX File (*.avfx)|*.avfx*|All files (*.*)|*.*", node.toBytes(), "avfx" );
                    }
                    if(ImGui.Selectable("TexTools Mod" ) ) {
                        TexToolsUI.Show();
                    }
                    if(ImGui.Selectable("Penumbra Mod" ) ) {
                        PenumbraUI.Show();
                    }
                    if( ImGui.Selectable( "Export last import (raw)" ) ) {
                        var node = _plugin.AVFX.toAVFX();
                        SaveDialog( "TXT files (*.txt)|*.txt|All files (*.*)|*.*", _plugin.Manager.LastImportNode.exportString( 0 ), "txt" );
                    }
                    ImGui.EndPopup();
                }
                // ======= TEXTURES ==========
                ImGui.SameLine();
                ImGui.PushFont( UiBuilder.IconFont );
                if( ImGui.Button( $"{( char )FontAwesomeIcon.Image}" ) ) {
                    TextureUI.Show();
                }
                ImGui.PopFont();
                ImGui.SameLine();
                ImGui.Text( $"{_plugin.Manager.TexManager.GamePathReplace.Count} Texture(s)" );
                ImGui.SameLine();
                // ======== VERIFY ============
                if( Configuration.Config.VerifyOnLoad ) {
                    ImGui.SameLine();
                    ImGui.PushFont( UiBuilder.IconFont );
                    ImGui.TextColored( StatusColor, IconText );
                    ImGui.PopFont();
                    ImGui.SameLine();
                    ImGui.TextColored( StatusColor, StatusText );
                }

                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
                VFXMain.Draw();
            }
            ImGui.End();
        }

        public bool Status = false;
        public string IconText = "";
        public string StatusText = "";
        public Vector4 StatusColor = new Vector4();
        public void SetStatus(bool status ) {
            Status = status;
            if( Status ) {
                IconText = $"{( char )FontAwesomeIcon.Check}";
                StatusText = "Verified";
                StatusColor = new Vector4( 0.15f, 0.90f, 0.15f, 1.0f );
            }
            else {
                IconText = $"{( char )FontAwesomeIcon.Times}";
                StatusText = "Parsing Issue";
                StatusColor = new Vector4( 0.90f, 0.15f, 0.15f, 1.0f );
            }
        }

        public void DrawFiles() {
            var ret = ImGui.BeginTabItem( "Files##MainInterfaceTabs" );
            if( !ret ) return;
            // ==========================
            ImGui.PushStyleColor( ImGuiCol.ChildBg, new Vector4( 0.18f, 0.18f, 0.22f, 0.4f ) );
            ImGui.SetCursorPos( ImGui.GetCursorPos() - new Vector2( 5, 5 ) );
            ImGui.BeginChild( "Child##MainInterface", new Vector2( ImGui.GetWindowWidth() - 0, 60 ) );
            ImGui.SetCursorPos( ImGui.GetCursorPos() + new Vector2( 5, 5 ) );
            ImGui.PopStyleColor();

            ImGui.Columns( 4, "MainInterfaceFileColumns", false );

            ImGui.SetColumnWidth( 0, 80 );
            ImGui.Text( "Data Source:" );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Text( "Preview On:" );
            ImGui.NextColumn();

            string sourceString = _plugin.SourceString;
            string previewString = _plugin.ReplaceString;
            ImGui.SetColumnWidth( 1, ImGui.GetWindowWidth() - 250 );
            ImGui.PushItemWidth( ImGui.GetColumnWidth() - 10 );
            ImGui.InputText( "##MainInterfaceFiles-Source", ref sourceString, 255, ImGuiInputTextFlags.ReadOnly );
            ImGui.InputText( "##MainInterfaceFiles-Preview", ref previewString, 255, ImGuiInputTextFlags.ReadOnly );
            ImGui.PopItemWidth();

            // ====== SEARCH + NEW ========
            ImGui.NextColumn();
            ImGui.SetColumnWidth( 2, 67 );
            ImGui.PushFont( UiBuilder.IconFont );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Search}" ) ) {
                SelectUI.Show();
            }
            ImGui.SameLine();
            ImGui.PushStyleColor( ImGuiCol.Button, new Vector4( 0.10f, 0.80f, 0.10f, 1.0f ) );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.FileMedical}", new Vector2( 25, 23 ) ) ) {
                ImGui.OpenPopup( "New_Popup1" );
            }
            ImGui.PopStyleColor();
            ImGui.PopFont();
            if( ImGui.BeginPopup( "New_Popup1") ) {
                if( ImGui.Selectable( "Blank") ) {
                    OpenTemplate( @"default_vfx.avfx" );
                }
                if( ImGui.Selectable( "Weapon") ) {
                    OpenTemplate( @"default_weapon.avfx" );
                }
                ImGui.EndPopup();
            }
            // ======= SEARCH + UNLINK ========
            ImGui.PushFont( UiBuilder.IconFont );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Search}##MainInterfaceFiles-PreviewSelect" ) ) {
                PreviewUI.Show( showLocal: false );
            }
            ImGui.SameLine();
            ImGui.PushStyleColor( ImGuiCol.Button, new Vector4( 0.80f, 0.10f, 0.10f, 1.0f ) );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Unlink}##MainInterfaceFiles-PreviewRemove" ) ) {
                _plugin.RemoveReplaceAVFX();
            }
            ImGui.PopStyleColor();
            ImGui.PopFont();
            // ======= MANAGE + OVERLAY =========
            ImGui.NextColumn();
            ImGui.SetColumnWidth( 3, 200 );
            ImGui.PushStyleColor( ImGuiCol.Button, new Vector4( 0.20f, 0.20f, 0.20f, 1.0f ) );
            if(ImGui.Button( "Manage" ) ) {
                DocUI.Show();
            }
            ImGui.PopStyleColor();
            ImGui.SameLine();
            ImGui.PushFont( UiBuilder.IconFont );
            if( ImGui.Button( $"{(!_plugin.Tracker.Enabled ? ( char )FontAwesomeIcon.Eye : ( char )FontAwesomeIcon.EyeSlash)}##MainInterfaceFiles-MarkVfx" ) ) {
                _plugin.Tracker.Enabled = !_plugin.Tracker.Enabled;
                if( !_plugin.Tracker.Enabled ) {
                    _plugin.Tracker.Reset();
                    _plugin.PluginInterface.UiBuilder.DisableCutsceneUiHide = false;
                }
                else {
                    _plugin.PluginInterface.UiBuilder.DisableCutsceneUiHide = true;
                }
            }
            ImGui.PopFont();
            // =======SPAWN + MANIP =========
            string previewSpawn = _plugin.Doc.ActiveDoc.Replace.Path;
            bool spawnDisabled = string.IsNullOrEmpty( previewSpawn );
            if(SpawnVfx == null ) {
                if( spawnDisabled ) {
                    ImGui.PushStyleVar( ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.5f );
                }
                if(ImGui.Button( "Spawn", new Vector2(52, 23) ) && !spawnDisabled ) {
                    ImGui.OpenPopup( "Spawn_Popup" );
                }
                if( spawnDisabled ) {
                    ImGui.PopStyleVar();
                }
            }
            else {
                if( ImGui.Button( "Remove" ) ) {
                    SpawnVfx?.Remove();
                    SpawnVfx = null;
                }
            }
            if( ImGui.BeginPopup( "Spawn_Popup" ) ) {
                if( ImGui.Selectable( "On Ground" ) ) {
                    SpawnVfx = new StaticVfx( _plugin, previewSpawn, _plugin.PluginInterface.ClientState.LocalPlayer.Position);
                }
                if( ImGui.Selectable( "On Self" ) ) {
                    SpawnVfx = new ActorVfx( _plugin, _plugin.PluginInterface.ClientState.LocalPlayer, _plugin.PluginInterface.ClientState.LocalPlayer, previewSpawn );
                }
                if (ImGui.Selectable("On Taget" ) ) {
                    var t = _plugin.PluginInterface.ClientState.Targets.CurrentTarget;
                    if(t != null ) {
                        SpawnVfx = new ActorVfx( _plugin, t, t, previewSpawn );
                    }
                }
                ImGui.EndPopup();
            }
            ImGui.SameLine();
            ImGui.PushFont( UiBuilder.IconFont );
            if( !VFXManip.CanBeEnabled ) {
                ImGui.PushStyleVar( ImGuiStyleVar.Alpha, 0.5f );
                ImGui.Button( $"{( char )FontAwesomeIcon.Cube}##MainInterfaceFiles-VfxManip" );
                ImGui.PopStyleVar();
            }
            else {
                if(ImGui.Button( $"{( char )FontAwesomeIcon.Cube}##MainInterfaceFiles-VfxManip" ) ) {
                    VFXManip.Enabled = !VFXManip.Enabled;
                }
            }
            ImGui.PopFont();


            ImGui.Columns( 1 );
            ImGui.Separator();
            ImGui.EndChild();
            ImGui.EndTabItem();
        }

        public string RawInputValue = "";
        public string RawTexInputValue = "";
        public void DrawRaw() {
            var ret = ImGui.BeginTabItem( "Extract##MainInterfaceTabs" );
            if( !ret ) return;
            // ======= AVFX =========
            ImGui.Text( "Extract a raw .avfx file" );
            ImGui.InputText( "Path##RawExtract", ref RawInputValue, 255 );
            ImGui.SameLine();
            if( ImGui.Button( "Extract##RawExtract" ) )
            {
                bool result = _plugin.PluginInterface.Data.FileExists( RawInputValue );
                if( result ) {
                    try {
                        var file = _plugin.PluginInterface.Data.GetFile( RawInputValue );
                        SaveDialog( "AVFX File (*.avfx)|*.avfx*|All files (*.*)|*.*", file.Data, "avfx" );
                    }
                    catch(Exception e ) {
                        PluginLog.LogError( "Could not read file" );
                        PluginLog.LogError( e.ToString() );
                    }
                }
            }
            // ===== ATEX ==========
            ImGui.Text( "Extract an .atex file" );
            ImGui.InputText( "Path##RawTexExtract", ref RawTexInputValue, 255 );
            ImGui.SameLine();
            if( ImGui.Button( "Extract##RawTexExtract" ) ) {
                bool result = _plugin.PluginInterface.Data.FileExists( RawTexInputValue );
                if( result ) {
                    try {
                        var file = _plugin.PluginInterface.Data.GetFile( RawTexInputValue );
                        SaveDialog( "ATEX File (*.atex)|*.atex*|All files (*.*)|*.*", file.Data, "atex" );
                    }
                    catch( Exception e ) {
                        PluginLog.LogError( "Could not read file" );
                        PluginLog.LogError( e.ToString() );
                    }
                }
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Separator();
            ImGui.EndTabItem();
        }

        public void DrawSettings() {
            var ret = ImGui.BeginTabItem( "Settings##MainInterfaceTabs" );
            if( !ret ) return;
            bool verifyOnLoad = Configuration.Config.VerifyOnLoad;
            if(ImGui.Checkbox( "Verify on load##Settings", ref verifyOnLoad ) )
            {
                Configuration.Config.VerifyOnLoad = verifyOnLoad;
            }
            ImGui.SameLine();
            bool loadTextures = Configuration.Config.PreviewTextures;
            if( ImGui.Checkbox( "Load textures##Settings", ref loadTextures ) )
            {
                Configuration.Config.PreviewTextures = loadTextures;
            }
            ImGui.SameLine();
            bool logAllFiles = Configuration.Config.LogAllFiles;
            if( ImGui.Checkbox( "Log all files##Settings", ref logAllFiles ) ) {
                Configuration.Config.LogAllFiles = logAllFiles;
            }
            ImGui.SameLine();
            bool hideWithUI = Configuration.Config.HideWithUI;
            if( ImGui.Checkbox( "Hide with UI##Settings", ref hideWithUI ) ) {
                Configuration.Config.HideWithUI = hideWithUI;
            }

            if( ImGui.Button( "Save##Settings" ) )
            {
                Configuration.Config.Save();
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Separator();
            ImGui.EndTabItem();
        }

        public void DrawHelp() {
            var ret = ImGui.BeginTabItem( "Help##MainInterfaceTabs" );
            if( !ret ) return;
            if( ImGui.Button( "Report an Issue" ) ) {
                Process.Start( "https://github.com/0ceal0t/Dalamud-VFXEditor/issues" );
            }
            ImGui.SameLine();
            if( ImGui.Button( "Guide" ) ) {
                Process.Start( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki/Basic-Guide" );
            }
            ImGui.TextWrapped( 
@"This plugin works by replacing an existing VFX with another one. It does not, however, actually modify any of the game's internal files.
If you want to make the modification permanent, you will need to create a mod using your platform of choice.

If you are having issues loading a VFX, please open a Github issue. Make sure to specify either the in-game path of the VFX file or attach the file directly."
            );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Separator();
            ImGui.EndTabItem();
        }

        public void Dispose() {
            SpawnVfx?.Remove();
        }

        // ======= HELPERS ============
        public void OpenTemplate(string path ) {
            VFXSelectResult newResult = new VFXSelectResult();
            newResult.DisplayString = "[NEW]";
            newResult.Type = VFXSelectType.Local;
            newResult.Path = Path.Combine( Plugin.TemplateLocation, "Files", path );
            _plugin.SelectAVFX( newResult );
        }
        public void SaveDialog( string filter, string data, string ext ) {
            SaveDialog( filter, Encoding.ASCII.GetBytes(data), ext );
        }
        public void SaveDialog(string filter, byte[] data, string ext ) {
            Task.Run( async () => {
                var picker = new SaveFileDialog {
                    Filter = filter,
                    Title = "Select a Save Location.",
                    DefaultExt = ext,
                    AddExtension = true
                };
                var result = await picker.ShowDialogAsync();
                if( result == DialogResult.OK ) {
                    try {
                        File.WriteAllBytes( picker.FileName, data );
                    }
                    catch( Exception ex ) {
                        PluginLog.LogError( ex, "Could not save to: " + picker.FileName );
                    }
                }
            } );
        }
    }
}