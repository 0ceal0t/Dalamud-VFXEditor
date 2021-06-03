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
using VFXEditor.UI.VFX;
using VFXSelect.UI;

namespace VFXEditor.UI
{
    public class MainInterface
    {
        private readonly Plugin Plugin;
        public bool Visible = false;

        public UIMain VFXMain;
        public VFXSelectDialog SelectUI;
        public VFXSelectDialog PreviewUI;
        public TexToolsDialog TexToolsUI;
        public PenumbraDialog PenumbraUI;
        public DocDialog DocUI;
        public TextureDialog TextureUI;
        public VFXManipulator VFXManip;

        public BaseVfx SpawnVfx;

        private string IconText;
        private string StatusText;
        private Vector4 StatusColor;

        private string RawInputValue = "";
        private string RawTexInputValue = "";

        public MainInterface( Plugin plugin )
        {
            Plugin = plugin;
            SelectUI = new VFXSelectDialog( Plugin.Manager.Sheets, "File Select [SOURCE]", Plugin.Configuration.RecentSelects );
            PreviewUI = new VFXSelectDialog( Plugin.Manager.Sheets, "File Select [TARGET]", Plugin.Configuration.RecentSelects );

            SelectUI.OnSelect += Plugin.SelectAVFX;
            PreviewUI.OnSelect += Plugin.ReplaceAVFX;

            TexToolsUI = new TexToolsDialog( Plugin );
            PenumbraUI = new PenumbraDialog( Plugin );
            DocUI = new DocDialog( Plugin );
            TextureUI = new TextureDialog( Plugin );
            VFXManip = new VFXManipulator( Plugin );

#if DEBUG
            Visible = true;
#endif
        }
        public void RefreshAVFX() {
            VFXMain = new UIMain( Plugin.AVFX, Plugin );
        }
        public void UnloadAVFX() {
            VFXMain = null;
        }

        public void Draw() {
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
            if( !ImGui.Begin( Plugin.Name, ref Visible ) ) return;

            ImGui.BeginTabBar( "MainInterfaceTabs" );
            DrawFiles();
            DrawExtract();
            DrawSettings();
            DrawHelp();
            ImGui.EndTabBar();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( VFXMain == null ) {
                ImGui.Text( @"Select a source VFX file to begin..." );
            }
            else {
                ImGui.PushStyleColor( ImGuiCol.Button, new Vector4( 0.10f, 0.80f, 0.10f, 1.0f ) );
                if( ImGui.Button( "UPDATE" ) ) {
                    if((DateTime.Now - LastUpdate).TotalSeconds > 0.5  ) { // only allow updates every 1/2 second
                        Plugin.Doc.Save();
                        Plugin.ResourceLoader.ReRender();
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
                        var node = Plugin.AVFX.ToAVFX();
                        SaveDialog( "AVFX File (*.avfx)|*.avfx*|All files (*.*)|*.*", node.ToBytes(), "avfx" );
                    }
                    if(ImGui.Selectable("TexTools Mod" ) ) {
                        TexToolsUI.Show();
                    }
                    if(ImGui.Selectable("Penumbra Mod" ) ) {
                        PenumbraUI.Show();
                    }
                    if( ImGui.Selectable( "Export last import (raw)" ) ) {
                        SaveDialog( "TXT files (*.txt)|*.txt|All files (*.*)|*.*", Plugin.Manager.LastImportNode.ExportString( 0 ), "txt" );
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
                ImGui.Text( $"{Plugin.Manager.TexManager.GamePathReplace.Count} Texture(s)" );
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

        public void SetStatus(bool status ) {
            if( status ) {
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
            if( !ImGui.BeginTabItem( "Files##MainInterfaceTabs" ) ) return;

            ImGui.PushStyleColor( ImGuiCol.ChildBg, new Vector4( 0.18f, 0.18f, 0.22f, 0.4f ) );
            ImGui.SetCursorPos( ImGui.GetCursorPos() - new Vector2( 5, 5 ) );
            ImGui.BeginChild( "Child##MainInterface", new Vector2( ImGui.GetWindowWidth() - 0, 60 ) );
            ImGui.SetCursorPos( ImGui.GetCursorPos() + new Vector2( 5, 5 ) );
            ImGui.PopStyleColor();

            ImGui.Columns( 3, "MainInterfaceFileColumns", false );

            ImGui.SetColumnWidth( 0, 80 );
            ImGui.Text( "Data Source:" );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Text( "Preview On:" );
            ImGui.NextColumn();

            // ======= SEARCH BARS =========
            string sourceString = Plugin.SourceString;
            string previewString = Plugin.ReplaceString;
            ImGui.SetColumnWidth( 1, ImGui.GetWindowWidth() - 210 );
            ImGui.PushItemWidth( ImGui.GetColumnWidth() - 80 );

            ImGui.InputText( "##MainInterfaceFiles-Source", ref sourceString, 255, ImGuiInputTextFlags.ReadOnly );

            ImGui.SameLine();
            ImGui.PushFont( UiBuilder.IconFont );
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Search}", new Vector2( 30, 23 ) ) ) {
                SelectUI.Show();
            }
            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            ImGui.PushStyleColor( ImGuiCol.Button, new Vector4( 0.80f, 0.10f, 0.10f, 1.0f ) );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Times}##MainInterfaceFiles-SourceRemove", new Vector2( 30, 23 ) ) ) {
                Plugin.RemoveSourceAVFX();
            }
            ImGui.PopStyleColor();
            ImGui.PopFont();


            ImGui.InputText( "##MainInterfaceFiles-Preview", ref previewString, 255, ImGuiInputTextFlags.ReadOnly );

            ImGui.SameLine();
            ImGui.PushFont( UiBuilder.IconFont );
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Search}##MainInterfaceFiles-PreviewSelect", new Vector2( 30, 23 ) ) ) {
                PreviewUI.Show( showLocal: false );
            }
            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            ImGui.PushStyleColor( ImGuiCol.Button, new Vector4( 0.80f, 0.10f, 0.10f, 1.0f ) );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Times}##MainInterfaceFiles-PreviewRemove", new Vector2( 30, 23 ) ) ) {
                Plugin.RemoveReplaceAVFX();
            }
            ImGui.PopStyleColor();
            ImGui.PopFont();

            ImGui.PopItemWidth();

            // ======= MANAGE + OVERLAY =========
            ImGui.NextColumn();
            ImGui.SetColumnWidth( 3, 200 );

            ImGui.PushFont( UiBuilder.IconFont );
            ImGui.PushStyleColor( ImGuiCol.Button, new Vector4( 0.10f, 0.80f, 0.10f, 1.0f ) );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.FileMedical}", new Vector2( 28, 23 ) ) ) {
                ImGui.OpenPopup( "New_Popup1" );
            }
            ImGui.PopStyleColor();
            ImGui.PopFont();

            if( ImGui.BeginPopup( "New_Popup1" ) ) {
                if( ImGui.Selectable( "Blank" ) ) {
                    OpenTemplate( @"default_vfx.avfx" );
                }
                if( ImGui.Selectable( "Weapon" ) ) {
                    OpenTemplate( @"default_weapon.avfx" );
                }
                ImGui.EndPopup();
            }

            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 6 );
            ImGui.PushStyleColor( ImGuiCol.Button, new Vector4( 0.20f, 0.20f, 0.20f, 1.0f ) );
            if(ImGui.Button( "Docs", new Vector2( 52, 23 ) ) ) {
                DocUI.Show();
            }
            ImGui.PopStyleColor();

            // =======SPAWN + MANIP =========
            ImGui.PushFont( UiBuilder.IconFont );
            if( ImGui.Button( $"{( !Plugin.Tracker.Enabled ? ( char )FontAwesomeIcon.Eye : ( char )FontAwesomeIcon.EyeSlash )}##MainInterfaceFiles-MarkVfx", new Vector2( 28, 23 ) ) ) {
                Plugin.Tracker.Enabled = !Plugin.Tracker.Enabled;
                if( !Plugin.Tracker.Enabled ) {
                    Plugin.Tracker.Reset();
                    Plugin.PluginInterface.UiBuilder.DisableCutsceneUiHide = false;
                }
                else {
                    Plugin.PluginInterface.UiBuilder.DisableCutsceneUiHide = true;
                }
            }
            ImGui.PopFont();

            ImGui.SameLine();
            string previewSpawn = Plugin.Doc.ActiveDoc.Replace.Path;
            bool spawnDisabled = string.IsNullOrEmpty( previewSpawn );
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 6 );
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
                    SpawnVfx = new StaticVfx( Plugin, previewSpawn, Plugin.PluginInterface.ClientState.LocalPlayer.Position);
                }
                if( ImGui.Selectable( "On Self" ) ) {
                    SpawnVfx = new ActorVfx( Plugin, Plugin.PluginInterface.ClientState.LocalPlayer, Plugin.PluginInterface.ClientState.LocalPlayer, previewSpawn );
                }
                if (ImGui.Selectable("On Taget" ) ) {
                    var t = Plugin.PluginInterface.ClientState.Targets.CurrentTarget;
                    if(t != null ) {
                        SpawnVfx = new ActorVfx( Plugin, t, t, previewSpawn );
                    }
                }
                ImGui.EndPopup();
            }
            ImGui.SameLine();
            ImGui.PushFont( UiBuilder.IconFont );
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 6 );
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

        public void DrawExtract() {
            if( !ImGui.BeginTabItem( "Extract##MainInterfaceTabs" ) ) return;
            // ======= AVFX =========
            ImGui.Text( "Extract a raw .avfx file" );
            ImGui.InputText( "Path##RawExtract", ref RawInputValue, 255 );
            ImGui.SameLine();
            if( ImGui.Button( "Extract##RawExtract" ) ) {
                bool result = Plugin.PluginInterface.Data.FileExists( RawInputValue );
                if( result ) {
                    try {
                        var file = Plugin.PluginInterface.Data.GetFile( RawInputValue );
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
                bool result = Plugin.PluginInterface.Data.FileExists( RawTexInputValue );
                if( result ) {
                    try {
                        var file = Plugin.PluginInterface.Data.GetFile( RawTexInputValue );
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
            if( !ImGui.BeginTabItem( "Settings##MainInterfaceTabs" ) ) return;

            ImGui.Text( "Changes to the temp file location may require a restart to take effect" );
            ImGui.InputText( "Temp file location", ref Configuration.Config.WriteLocation, 255 );
            ImGui.SetNextItemWidth( 200 );
            ImGui.Checkbox( "Verify on load##Settings", ref Configuration.Config.VerifyOnLoad );
            ImGui.SameLine();
            ImGui.Checkbox( "Load textures##Settings", ref Configuration.Config.PreviewTextures );
            ImGui.SameLine();
            ImGui.Checkbox( "Log all files##Settings", ref Configuration.Config.LogAllFiles );
            ImGui.SameLine();
            ImGui.Checkbox( "Hide with UI##Settings", ref Configuration.Config.HideWithUI );
            ImGui.SameLine();
            ImGui.SetNextItemWidth( 135 );
            if( ImGui.InputInt( "Recent VFX Limit##Settings", ref Configuration.Config.SaveRecentLimit ) ) {
                Configuration.Config.SaveRecentLimit = Math.Max( Configuration.Config.SaveRecentLimit, 0 );
            }
            ImGui.Checkbox( "Live Overlay Limit by Distance##Settings", ref Configuration.Config.OverlayLimit );

            if( ImGui.Button( "Save##Settings" ) ) {
                Configuration.Config.Save();
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Separator();
            ImGui.EndTabItem();
        }

        public void DrawHelp() {
            if( !ImGui.BeginTabItem( "Help##MainInterfaceTabs" ) ) return;
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
            Plugin.SelectAVFX( newResult );
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