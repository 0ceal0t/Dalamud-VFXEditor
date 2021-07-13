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
using VFXEditor.Structs.Vfx;
using VFXEditor.UI;
using VFXEditor.UI.VFX;
using VFXSelect.UI;

namespace VFXEditor {
    public partial class Plugin {
        public bool Visible = false;

        public VFXSelectDialog SelectUI;
        public VFXSelectDialog PreviewUI;

        public DocDialog DocUI;
        public SettingsDialog SettingsUI;
        public ToolsDialog ToolsUI;
        public TexToolsDialog TexToolsUI;
        public PenumbraDialog PenumbraUI;
        public TextureDialog TextureUI;

        public BaseVfx SpawnVfx;

        public DateTime LastUpdate = DateTime.Now;

        public bool IsLoading = false;

        public void InitUI() {
            SelectUI = new VFXSelectDialog(
                Sheets, "File Select [SOURCE]",
                Configuration.RecentSelects,
                showSpawn: true,
                spawnVfxExists: () => SpawnExists(),
                removeSpawnVfx: () => RemoveSpawnVfx(),
                spawnOnGround: ( string path ) => SpawnOnGround( path ),
                spawnOnSelf: ( string path ) => SpawnOnSelf( path ),
                spawnOnTarget: ( string path ) => SpawnOnTarget( path )
            );
            PreviewUI = new VFXSelectDialog(
                Sheets, "File Select [TARGET]",
                Configuration.RecentSelects,
                showSpawn: true,
                spawnVfxExists: () => SpawnExists(),
                removeSpawnVfx: () => RemoveSpawnVfx(),
                spawnOnGround: ( string path ) => SpawnOnGround( path ),
                spawnOnSelf: ( string path ) => SpawnOnSelf( path ),
                spawnOnTarget: ( string path ) => SpawnOnTarget( path )
            );

            SelectUI.OnSelect += SetSourceVFX;
            PreviewUI.OnSelect += SetReplaceVFX;

            DocUI = new DocDialog( this );
            SettingsUI = new SettingsDialog( this );
            ToolsUI = new ToolsDialog( this );
            TextureUI = new TextureDialog( this );
            TexToolsUI = new TexToolsDialog( this );
            PenumbraUI = new PenumbraDialog( this );

#if DEBUG
            Visible = true;
#endif
        }

        public bool SpawnExists() {
            return SpawnVfx != null;
        }

        public void RemoveSpawnVfx() {
            SpawnVfx?.Remove();
            SpawnVfx = null;
        }

        public void SpawnOnGround( string path ) {
            SpawnVfx = new StaticVfx( this, path, PluginInterface.ClientState.LocalPlayer.Position );
        }

        public void SpawnOnSelf( string path ) {
            SpawnVfx = new ActorVfx( this, PluginInterface.ClientState.LocalPlayer, PluginInterface.ClientState.LocalPlayer, path );
        }

        public void SpawnOnTarget( string path ) {
            var t = PluginInterface.ClientState.Targets.CurrentTarget;
            if( t != null ) {
                SpawnVfx = new ActorVfx( this, t, t, path );
            }
        }

        public void DrawUI() {
            if( !Visible ) return;
            DrawMainInterface();
            if(!IsLoading) {
                SelectUI.Draw();
                PreviewUI.Draw();
                TexToolsUI.Draw();
                PenumbraUI.Draw();
                SettingsUI.Draw();
                ToolsUI.Draw();
                DocUI.Draw();
                TextureUI.Draw();
            }
        }

        public void DrawMainInterface() {
            ImGui.SetNextWindowSize( new Vector2( 800, 1000 ), ImGuiCond.FirstUseEver );
            if( !ImGui.Begin( Name + (string.IsNullOrEmpty(CurrentWorkspaceLocation) ? "" : " - " + CurrentWorkspaceLocation) + "###VFXEditor", ref Visible, ImGuiWindowFlags.MenuBar ) ) return;

            if(IsLoading) {
                ImGui.Text( "Loading...." );
                ImGui.End();
                return;
            }

            DrawHeader();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( CurrentDocument.Main == null ) {
                ImGui.Text( @"Select a source VFX file to begin..." );
            }
            else {
                ImGui.PushStyleColor( ImGuiCol.Button, new Vector4( 0.10f, 0.80f, 0.10f, 1.0f ) );
                if( ImGui.Button( "UPDATE" ) ) {
                    if( ( DateTime.Now - LastUpdate ).TotalSeconds > 0.5 ) { // only allow updates every 1/2 second
                        DocManager.Save();
                        ResourceLoader.ReRender();
                        LastUpdate = DateTime.Now;
                    }
                }
                ImGui.PopStyleColor();
                // ===== EXPORT ======
                ImGui.SameLine();
                ImGui.PushFont( UiBuilder.IconFont );
                if( ImGui.Button( $"{( char )FontAwesomeIcon.FileDownload}" ) ) {
                    ImGui.OpenPopup( "Export_Popup" );
                }
                ImGui.PopFont();

                if( ImGui.BeginPopup( "Export_Popup" ) ) {
                    if( ImGui.Selectable( ".AVFX" ) ) {
                        var node = CurrentDocument.Main.AVFX.ToAVFX();
                        SaveDialog( "AVFX File (*.avfx)|*.avfx*|All files (*.*)|*.*", node.ToBytes(), "avfx" );
                    }
                    if( ImGui.Selectable( "TexTools Mod" ) ) {
                        TexToolsUI.Show();
                    }
                    if( ImGui.Selectable( "Penumbra Mod" ) ) {
                        PenumbraUI.Show();
                    }
                    if( ImGui.Selectable( "Export last import (raw)" ) ) {
                        SaveDialog( "TXT files (*.txt)|*.txt|All files (*.*)|*.*", LastImportNode.ExportString( 0 ), "txt" );
                    }
                    ImGui.EndPopup();
                }

                // ======== VERIFY ============
                ImGui.SameLine();
                if(Configuration.Config.VerifyOnLoad) {
                    ImGui.SameLine();
                    ImGui.PushFont( UiBuilder.IconFont );

                    var verified = CurrentDocument.Main.Verified;
                    Vector4 color = verified switch
                    {
                        VerifiedStatus.OK => new Vector4( 0.15f, 0.90f, 0.15f, 1.0f ),
                        VerifiedStatus.ISSUE => new Vector4( 0.90f, 0.15f, 0.15f, 1.0f ),
                        _ => new Vector4( 0.7f, 0.7f, 0.7f, 1.0f )
                    };

                    string icon = verified switch
                    {
                        VerifiedStatus.OK => $"{( char )FontAwesomeIcon.Check}",
                        VerifiedStatus.ISSUE => $"{( char )FontAwesomeIcon.Times}",
                        _ => $"{( char )FontAwesomeIcon.Question}"
                    };

                    string text = verified switch
                    {
                        VerifiedStatus.OK => "Verified",
                        VerifiedStatus.ISSUE => "Parsing Issues",
                        _ => "Unverified"
                    };

                    ImGui.TextColored( color, icon );
                    ImGui.PopFont();
                    ImGui.SameLine();
                    ImGui.TextColored( color, text );
                }

                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
                CurrentDocument.Main.Draw();
            }
            ImGui.End();
        }

        public void DrawHeader() {
            if(ImGui.BeginMenuBar()) {
                if(ImGui.BeginMenu("File##Menu")) {
                    ImGui.TextDisabled( "Workspace" );
                    ImGui.SameLine();
                    HelpMarker( "A workspace allows you to save multiple vfx replacements at the same time, as well as any imported textures or item renaming (such as particles or emitters)" );

                    if(ImGui.MenuItem("New##Menu")) {
                        NewWorkspace();
                    }
                    if( ImGui.MenuItem( "Open##Menu" ) ) {
                        OpenWorkspace();
                    }
                    if( ImGui.MenuItem( "Save##Menu" ) ) {
                        SaveWorkspace();
                    }
                    if( ImGui.MenuItem( "Save As##Menu" ) ) {
                        SaveAsWorkspace();
                    }
                    ImGui.EndMenu();
                }
                if( ImGui.MenuItem( "Documents##Menu" ) ) {
                    DocUI.Show();
                }
                if( ImGui.MenuItem( "Textures##Menu" ) ) {
                    TextureUI.Show();
                }
                if( ImGui.MenuItem( "Settings##Menu" ) ) {
                    SettingsUI.Show();
                }
                if( ImGui.MenuItem( "Tools##Menu" ) ) {
                    ToolsUI.Show();
                }
                if( ImGui.BeginMenu( "Help##Menu" ) ) {
                    if( ImGui.MenuItem( "Report an Issue##Menu" ) ) {
                        Process.Start( "https://github.com/0ceal0t/Dalamud-VFXEditor/issues" );
                    }
                    if( ImGui.MenuItem( "Basic Guide##Menu" ) ) {
                        Process.Start( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki/Basic-Guide" );
                    }
                    if( ImGui.MenuItem( "Github##Menu" ) ) {
                        Process.Start( "https://github.com/0ceal0t/Dalamud-VFXEditor" );
                    }
                    ImGui.EndMenu();
                }
                ImGui.EndMenuBar();
            }

            ImGui.SetCursorPos( ImGui.GetCursorPos() - new Vector2( 5, 5 ) );
            ImGui.BeginChild( "Child##MainInterface", new Vector2( ImGui.GetWindowWidth() - 0, 60 ) );
            ImGui.SetCursorPos( ImGui.GetCursorPos() + new Vector2( 5, 5 ) );

            ImGui.Columns( 3, "MainInterfaceFileColumns", false );

            // ======== INPUT TEXT =========
            ImGui.SetColumnWidth( 0, 95 );
            ImGui.Text( "Data Source" );
            ImGui.SameLine(); HelpMarker( "The source of the new VFX. For example, if you wanted to replace the Fire animation with that of Blizzard, Blizzard would be the data source" );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Text( "Preview On" );
            ImGui.SameLine(); HelpMarker( "The VFX which is being replaced. For example, if you wanted to replace the Fire animation with that of Blizzard, Fire would be the preview vfx" );
            ImGui.NextColumn();

            // ======= SEARCH BARS =========
            string sourceString = CurrentDocument.Source.DisplayString;
            string previewString = CurrentDocument.Replace.DisplayString;
            ImGui.SetColumnWidth( 1, ImGui.GetWindowWidth() - 210 );
            ImGui.PushItemWidth( ImGui.GetColumnWidth() - 100 );

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
                RemoveSourceVFX();
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
                RemoveReplaceVFX();
            }
            ImGui.PopStyleColor();
            ImGui.PopFont();

            ImGui.PopItemWidth();

            // ======= TEMPLATES =========
            ImGui.NextColumn();
            ImGui.SetColumnWidth( 3, 150 );

            if( ImGui.Button( $"Templates", new Vector2( 80, 23 ) ) ) {
                ImGui.OpenPopup( "New_Popup1" );
            }

            if( ImGui.BeginPopup( "New_Popup1" ) ) {
                if( ImGui.Selectable( "Blank" ) ) {
                    OpenTemplate( @"default_vfx.avfx" );
                }
                if( ImGui.Selectable( "Weapon" ) ) {
                    OpenTemplate( @"default_weapon.avfx" );
                }
                ImGui.EndPopup();
            }

            // =======SPAWN + EYE =========
            string previewSpawn = DocManager.ActiveDoc.Replace.Path;
            bool spawnDisabled = string.IsNullOrEmpty( previewSpawn );
            if( !SpawnExists() ) {
                if( spawnDisabled ) {
                    ImGui.PushStyleVar( ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.5f );
                }
                if( ImGui.Button( "Spawn", new Vector2( 50, 23 ) ) && !spawnDisabled ) {
                    ImGui.OpenPopup( "Spawn_Popup" );
                }
                if( spawnDisabled ) {
                    ImGui.PopStyleVar();
                }
            }
            else {
                if( ImGui.Button( "Remove" ) ) {
                    RemoveSpawnVfx();
                }
            }
            if( ImGui.BeginPopup( "Spawn_Popup" ) ) {
                if( ImGui.Selectable( "On Ground" ) ) {
                    SpawnOnGround( previewSpawn );
                }
                if( ImGui.Selectable( "On Self" ) ) {
                    SpawnOnSelf( previewSpawn );
                }
                if( ImGui.Selectable( "On Taget" ) ) {
                    SpawnOnTarget( previewSpawn );
                }
                ImGui.EndPopup();
            }

            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 6 );
            ImGui.PushFont( UiBuilder.IconFont );
            if( ImGui.Button( $"{( !Tracker.Enabled ? ( char )FontAwesomeIcon.Eye : ( char )FontAwesomeIcon.Times )}###MainInterfaceFiles-MarkVfx", new Vector2( 28, 23 ) ) ) {
                Tracker.Enabled = !Tracker.Enabled;
                if( !Tracker.Enabled ) {
                    Tracker.Reset();
                    PluginInterface.UiBuilder.DisableCutsceneUiHide = false;
                }
                else {
                    PluginInterface.UiBuilder.DisableCutsceneUiHide = true;
                }
            }
            ImGui.PopFont();

            ImGui.SameLine(); HelpMarker( @"Use the eye icon to enable or disable the VFX overlay. This will show you the positions of most VFXs in the game world, along with their file paths. Note that you may need to enter and exit your current zone to see all of the VFXs" );

            ImGui.Columns( 1 );
            ImGui.Separator();
            ImGui.EndChild();
        }

        // ======= HELPERS ============
        public void OpenTemplate( string path ) {
            VFXSelectResult newResult = new VFXSelectResult();
            newResult.DisplayString = "[NEW]";
            newResult.Type = VFXSelectType.Local;
            newResult.Path = Path.Combine( TemplateLocation, "Files", path );
            SetSourceVFX( newResult );
        }

        // =========== HELPERS ===========
        public static void HelpMarker(string text) {
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            ImGui.TextDisabled( "(?)" );
            if(ImGui.IsItemHovered()) {
                ImGui.BeginTooltip();
                ImGui.PushTextWrapPos( ImGui.GetFontSize() * 35.0f );
                ImGui.TextUnformatted( text );
                ImGui.PopTextWrapPos();
                ImGui.EndTooltip();
            }
        }

        public static void ImportFileDialog(string filter, string title, Action<string> callback) {
            Task.Run( async () => {
                var picker = new OpenFileDialog
                {
                    Filter = filter,
                    CheckFileExists = true,
                    Title = title
                };
                var result = await picker.ShowDialogAsync();
                if(result == DialogResult.OK) {
                    callback( picker.FileName );
                }
            } );
        }

        public static void SaveDialog( string filter, string data, string ext ) {
            SaveDialog( filter, Encoding.ASCII.GetBytes( data ), ext );
        }

        public static void SaveDialog( string filter, byte[] data, string ext ) {
            SaveFileDialog( filter, "Select a Save Location.", ext,
                ( string path ) => {
                    try {
                        File.WriteAllBytes( path, data );
                    }
                    catch( Exception ex ) {
                        PluginLog.LogError( ex, "Could not save to: " + path );
                    }
                }
            );
        }

        public static void SaveFileDialog( string filter, string title, string defaultExt, Action<string> callback ) {
            Task.Run( async () => {
                var picker = new SaveFileDialog
                {
                    Filter = filter,
                    DefaultExt = defaultExt,
                    AddExtension = true,
                    Title = title
                };
                var result = await picker.ShowDialogAsync();
                if( result == DialogResult.OK ) {
                    callback( picker.FileName );
                }
            } );
        }

        public static void SaveFolderDialog( string filter, string title, Action<string> callback ) {
            Task.Run( async () => {
                var picker = new SaveFileDialog
                {
                    Filter = filter,
                    Title = title,
                    ValidateNames = false,
                    CheckFileExists = false,
                    FileName = "Select a Folder"
                };
                var result = await picker.ShowDialogAsync();
                if( result == DialogResult.OK ) {
                    callback( picker.FileName );
                }
            } );
        }
    }
}