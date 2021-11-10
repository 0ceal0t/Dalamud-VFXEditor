using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Text;
using Dalamud.Interface;
using ImGuiFileDialog;
using ImGuiNET;
using VFXEditor.Data;
using VFXEditor.Structs.Vfx;
using VFXEditor.Tmb;
using VFXEditor.UI;
using VFXEditor.UI.VFX;
using VFXSelect.UI;

namespace VFXEditor {
    public partial class Plugin {
        private bool Visible = false;

        private VFXSelectDialog SelectUI;
        private VFXSelectDialog PreviewUI;

        private DocDialog DocUI;
        private SettingsDialog SettingsUI;
        private ToolsDialog ToolsUI;
        private TexToolsDialog TexToolsUI;
        private PenumbraDialog PenumbraUI;
        private TextureDialog TextureUI;

        private DateTime LastUpdate = DateTime.Now;
        private bool IsLoading = false;

        public void InitUI() {
            SelectUI = new VFXSelectDialog(
                "File Select [SOURCE]",
                Configuration.Config.RecentSelects,
                showSpawn: true,
                spawnVfxExists: () => SpawnExists(),
                removeSpawnVfx: () => RemoveSpawnVfx(),
                spawnOnGround: ( string path ) => SpawnOnGround( path ),
                spawnOnSelf: ( string path ) => SpawnOnSelf( path ),
                spawnOnTarget: ( string path ) => SpawnOnTarget( path )
            );
            PreviewUI = new VFXSelectDialog(
                "File Select [TARGET]",
                Configuration.Config.RecentSelects,
                showSpawn: true,
                spawnVfxExists: () => SpawnExists(),
                removeSpawnVfx: () => RemoveSpawnVfx(),
                spawnOnGround: ( string path ) => SpawnOnGround( path ),
                spawnOnSelf: ( string path ) => SpawnOnSelf( path ),
                spawnOnTarget: ( string path ) => SpawnOnTarget( path )
            );

            SelectUI.OnSelect += SetSourceVFX;
            PreviewUI.OnSelect += SetReplaceVFX;

            DocUI = new DocDialog();
            SettingsUI = new SettingsDialog();
            ToolsUI = new ToolsDialog();
            TextureUI = new TextureDialog();
            TexToolsUI = new TexToolsDialog();
            PenumbraUI = new PenumbraDialog();

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
            SpawnVfx = new StaticVfx( this, path, ClientState.LocalPlayer.Position );
        }

        public void SpawnOnSelf( string path ) {
            SpawnVfx = new ActorVfx( this, ClientState.LocalPlayer, ClientState.LocalPlayer, path );
        }

        public void SpawnOnTarget( string path ) {
            var t = TargetManager.Target;
            if( t != null ) {
                SpawnVfx = new ActorVfx( this, t, t, path );
            }
        }

        public void Draw() {
            if( !Visible ) return;

            CopyManager.PreDraw();
            DrawMainInterface();
            Tracker.Draw();
            if( !IsLoading ) {
                SelectUI.Draw();
                PreviewUI.Draw();
                TexToolsUI.Draw();
                PenumbraUI.Draw();
                SettingsUI.Draw();
                ToolsUI.Draw();
                DocUI.Draw();
                TextureUI.Draw();
                Tmb.TmbManager.Manager?.Draw(); // TODO
            }
        }

        public void DrawMainInterface() {
            ImGui.SetNextWindowSize( new Vector2( 800, 1000 ), ImGuiCond.FirstUseEver );
            if( !ImGui.Begin( Name + ( string.IsNullOrEmpty( CurrentWorkspaceLocation ) ? "" : " - " + CurrentWorkspaceLocation ) + "###VFXEditor", ref Visible, ImGuiWindowFlags.MenuBar ) ) {
                ImGui.End();
                return;
            }

            if( IsLoading ) {
                ImGui.Text( "Loading...." );
                ImGui.End();
                return;
            }

            DrawHeader();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( DocumentManager.CurrentActiveDoc.Main == null ) {
                ImGui.Text( @"Select a source VFX file to begin..." );
            }
            else {
                ImGui.PushStyleColor( ImGuiCol.Button, UIUtils.GREEN_COLOR );
                if( ImGui.Button( "UPDATE" ) ) {
                    if( ( DateTime.Now - LastUpdate ).TotalSeconds > 0.5 ) { // only allow updates every 1/2 second
                        DocumentManager.Manager.Save();
                        ResourceLoader.ReRender();
                        LastUpdate = DateTime.Now;
                    }
                }
                ImGui.PopStyleColor();

                ImGui.SameLine();
                if (ImGui.Button("Reload") ) { // load resource
                    if( File.Exists( DocumentManager.CurrentActiveDoc.WriteLocation ) )
                        ResourceLoader.ReloadPath( DocumentManager.CurrentActiveDoc.Replace.Path, true );
                }
                ImGui.SameLine();
                HelpMarker( "Manually reload the resource. Only do this after pressing the UPDATE button." );

                // ===== EXPORT ======
                ImGui.SameLine();
                ImGui.PushFont( UiBuilder.IconFont );
                if( ImGui.Button( $"{( char )FontAwesomeIcon.FileDownload}" ) ) {
                    ImGui.OpenPopup( "Export_Popup" );
                }
                ImGui.PopFont();

                if( ImGui.BeginPopup( "Export_Popup" ) ) {
                    if( ImGui.Selectable( ".AVFX" ) ) {
                        var node =  DocumentManager.CurrentActiveDoc.Main.AVFX.ToAVFX();
                        WriteBytesDialog( ".avfx", node.ToBytes(), "avfx" );
                    }
                    if( ImGui.Selectable( "TexTools Mod" ) ) {
                        TexToolsUI.Show();
                    }
                    if( ImGui.Selectable( "Penumbra Mod" ) ) {
                        PenumbraUI.Show();
                    }
                    if( ImGui.Selectable( "Export last import (raw)" ) ) {
                        WriteBytesDialog( ".txt", DataHelper.LastImportNode.ExportString( 0 ), "txt" );
                    }
                    ImGui.EndPopup();
                }

                // ======== VERIFY ============
                if( Configuration.Config.VerifyOnLoad ) {
                    ImGui.SameLine();
                    ImGui.PushFont( UiBuilder.IconFont );

                    var verified = DocumentManager.CurrentActiveDoc.Main.Verified;
                    var color = verified switch {
                        VerifiedStatus.OK => UIUtils.GREEN_COLOR,
                        VerifiedStatus.ISSUE => UIUtils.RED_COLOR,
                        _ => new Vector4( 0.7f, 0.7f, 0.7f, 1.0f )
                    };

                    var icon = verified switch {
                        VerifiedStatus.OK => $"{( char )FontAwesomeIcon.Check}",
                        VerifiedStatus.ISSUE => $"{( char )FontAwesomeIcon.Times}",
                        _ => $"{( char )FontAwesomeIcon.Question}"
                    };

                    var text = verified switch {
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
                DocumentManager.CurrentActiveDoc.Main.Draw();
            }

            ImGui.End();
        }

        public void DrawHeader() {
            if( ImGui.BeginMenuBar() ) {
                if( ImGui.BeginMenu( "File##Menu" ) ) {
                    ImGui.TextDisabled( "Workspace" );
                    ImGui.SameLine();
                    HelpMarker( "A workspace allows you to save multiple vfx replacements at the same time, as well as any imported textures or item renaming (such as particles or emitters)" );

                    if( ImGui.MenuItem( "New##Menu" ) ) NewWorkspace();
                    if( ImGui.MenuItem( "Open##Menu" ) ) OpenWorkspace();
                    if( ImGui.MenuItem( "Save##Menu" ) ) SaveWorkspace();
                    if( ImGui.MenuItem( "Save As##Menu" ) ) SaveAsWorkspace();
                    ImGui.EndMenu();
                }
                if( ImGui.BeginMenu( "Edit##Menu" ) ) {
                    if( ImGui.MenuItem( "Copy##Menu" ) ) CopyManager.Copy();
                    if( ImGui.MenuItem( "Paste##Menu" ) ) CopyManager.Paste();
                    ImGui.EndMenu();
                }
                if( ImGui.MenuItem( "Documents##Menu" ) ) DocUI.Show();
                if( ImGui.MenuItem( "Textures##Menu" ) ) TextureUI.Show();
                if( ImGui.MenuItem( "Settings##Menu" ) ) SettingsUI.Show();
                if( ImGui.MenuItem( "Tools##Menu" ) ) ToolsUI.Show();
                if( ImGui.BeginMenu( "Help##Menu" ) ) {
                    if( ImGui.MenuItem( "Report an Issue##Menu" ) ) OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor/issues" );
                    if( ImGui.MenuItem( "Guides##Menu" ) ) OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki" );
                    if( ImGui.MenuItem( "Github##Menu" ) ) OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor" );
                    ImGui.EndMenu();
                }
                if( ImGui.BeginMenu( "Tmb##Menu " ) ) TmbManager.Show();
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
            var sourceString = DocumentManager.CurrentActiveDoc.Source.DisplayString;
            var previewString = DocumentManager.CurrentActiveDoc.Replace.DisplayString;
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
            ImGui.PushStyleColor( ImGuiCol.Button, UIUtils.RED_COLOR );
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
            ImGui.PushStyleColor( ImGuiCol.Button, UIUtils.RED_COLOR );
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
            var previewSpawn = DocumentManager.CurrentActiveDoc.Replace.Path;
            var spawnDisabled = string.IsNullOrEmpty( previewSpawn );
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
            var newResult = new VFXSelectResult {
                DisplayString = "[NEW]",
                Type = VFXSelectType.Local,
                Path = Path.Combine( TemplateLocation, "Files", path )
            };
            SetSourceVFX( newResult );
        }

        public static void HelpMarker( string text ) {
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            ImGui.TextDisabled( "(?)" );
            if( ImGui.IsItemHovered() ) {
                ImGui.BeginTooltip();
                ImGui.PushTextWrapPos( ImGui.GetFontSize() * 35.0f );
                ImGui.TextUnformatted( text );
                ImGui.PopTextWrapPos();
                ImGui.EndTooltip();
            }
        }

        public static void WriteBytesDialog( string filter, string data, string ext ) {
            WriteBytesDialog( filter, Encoding.ASCII.GetBytes( data ), ext );
        }

        public static void WriteBytesDialog( string filter, byte[] data, string ext ) {
            FileDialogManager.SaveFileDialog( "Select a Save Location", filter, "", ext, ( bool ok, string res ) => {
                if( ok ) File.WriteAllBytes( res, data );
            } );
        }

        public static void OpenUrl(string url) {
            Process.Start( new ProcessStartInfo {
                FileName = url,
                UseShellExecute = true
            } );
        }
    }
}