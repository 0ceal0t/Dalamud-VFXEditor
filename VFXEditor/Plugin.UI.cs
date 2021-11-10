using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Text;
using Dalamud.Interface;
using ImGuiFileDialog;
using ImGuiNET;
using VFXEditor.Data;
using VFXEditor.Document;
using VFXEditor.Texture;
using VFXEditor.Tmb;
using VFXEditor.Tracker;
using VFXEditor.UI;
using VFXEditor.UI.Vfx;
using VFXSelect.UI;

namespace VFXEditor {
    public partial class Plugin {
        private bool Visible = false;

        private VFXSelectDialog SelectUI;
        private VFXSelectDialog PreviewUI;

        private ToolsDialog ToolsUI;
        private TexToolsDialog TexToolsUI;
        private PenumbraDialog PenumbraUI;

        private DateTime LastUpdate = DateTime.Now;
        private bool IsLoading = false;

        public void InitUI() {
            SelectUI = new VFXSelectDialog(
                "File Select [SOURCE]",
                Configuration.RecentSelects,
                showSpawn: true,
                spawnVfxExists: () => SpawnExists(),
                removeSpawnVfx: () => RemoveSpawnVfx(),
                spawnOnGround: ( string path ) => SpawnOnGround( path ),
                spawnOnSelf: ( string path ) => SpawnOnSelf( path ),
                spawnOnTarget: ( string path ) => SpawnOnTarget( path )
            );
            PreviewUI = new VFXSelectDialog(
                "File Select [TARGET]",
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

            ToolsUI = new ToolsDialog();
            TexToolsUI = new TexToolsDialog();
            PenumbraUI = new PenumbraDialog();

#if DEBUG
            Visible = true;
#endif
        }

        public void Draw() {
            if( !Visible ) return;

            CopyManager.PreDraw();
            DrawMainInterface();
            VfxTracker.Draw();
            if( !IsLoading ) {
                SelectUI.Draw();
                PreviewUI.Draw();
                TexToolsUI.Draw();
                PenumbraUI.Draw();
                Configuration.Draw();
                ToolsUI.Draw();
                DocumentManager.Draw();
                TextureManager.Draw();
                TmbManager.Draw();
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

            if( DocumentManager.ActiveDocument.Main == null ) {
                ImGui.Text( @"Select a source VFX file to begin..." );
            }
            else {
                if( UIUtils.OkButton( "UPDATE" ) ) {
                    if( ( DateTime.Now - LastUpdate ).TotalSeconds > 0.5 ) { // only allow updates every 1/2 second
                        DocumentManager.Save();
                        ResourceLoader.ReRender();
                        LastUpdate = DateTime.Now;
                    }
                }

                ImGui.SameLine();
                if( ImGui.Button( "Reload" ) ) { // load resource
                    if( File.Exists( DocumentManager.ActiveDocument.WriteLocation ) )
                        ResourceLoader.ReloadPath( DocumentManager.ActiveDocument.Replace.Path, true );
                }
                ImGui.SameLine();
                UIUtils.HelpMarker( "Manually reload the resource. Only do this after pressing the UPDATE button." );

                // ===== EXPORT ======
                ImGui.SameLine();
                ImGui.PushFont( UiBuilder.IconFont );
                if( ImGui.Button( $"{( char )FontAwesomeIcon.FileDownload}" ) ) {
                    ImGui.OpenPopup( "Export_Popup" );
                }
                ImGui.PopFont();

                if( ImGui.BeginPopup( "Export_Popup" ) ) {
                    if( ImGui.Selectable( ".AVFX" ) ) {
                        var node = DocumentManager.ActiveDocument.Main.AVFX.ToAVFX();
                        WriteBytesDialog( ".avfx", node.ToBytes(), "avfx" );
                    }
                    if( ImGui.Selectable( "TexTools Mod" ) ) {
                        TexToolsUI.Show();
                    }
                    if( ImGui.Selectable( "Penumbra Mod" ) ) {
                        PenumbraUI.Show();
                    }
                    if( ImGui.Selectable( "Export last import (raw)" ) ) {
                        WriteBytesDialog( ".txt", AvfxHelper.LastImportNode.ExportString( 0 ), "txt" );
                    }
                    ImGui.EndPopup();
                }

                // ======== VERIFY ============
                if( Configuration.VerifyOnLoad ) {
                    ImGui.SameLine();
                    UIUtils.ShowVerifiedStatus( DocumentManager.ActiveDocument.Main.Verified );
                }

                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
                DocumentManager.ActiveDocument.Main.Draw();
            }

            ImGui.End();
        }

        public void DrawHeader() {
            if( ImGui.BeginMenuBar() ) {
                if( ImGui.BeginMenu( "File##Menu" ) ) {
                    ImGui.TextDisabled( "Workspace" );
                    ImGui.SameLine();
                    UIUtils.HelpMarker( "A workspace allows you to save multiple vfx replacements at the same time, as well as any imported textures or item renaming (such as particles or emitters)" );

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
                if( ImGui.MenuItem( "Documents##Menu" ) ) DocumentManager.Show();
                if( ImGui.MenuItem( "Textures##Menu" ) ) TextureManager.Show();
                if( ImGui.MenuItem( "Settings##Menu" ) ) Configuration.Show();
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
            ImGui.SetColumnWidth( 0, 140 );
            ImGui.Text( "Loaded VFX" );
            ImGui.SameLine(); UIUtils.HelpMarker( "The source of the new VFX. For example, if you wanted to replace the Fire animation with that of Blizzard, Blizzard would be the data source" );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Text( "VFX Being Replaced" );
            ImGui.SameLine(); UIUtils.HelpMarker( "The VFX which is being replaced. For example, if you wanted to replace the Fire animation with that of Blizzard, Fire would be the preview vfx" );
            ImGui.NextColumn();

            // ======= SEARCH BARS =========
            var sourceString = DocumentManager.ActiveDocument.Source.DisplayString;
            var previewString = DocumentManager.ActiveDocument.Replace.DisplayString;
            ImGui.SetColumnWidth( 1, ImGui.GetWindowWidth() - 255 );
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
            var previewSpawn = DocumentManager.ActiveDocument.Replace.Path;
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
            if( ImGui.Button( $"{( !VfxTracker.Enabled ? ( char )FontAwesomeIcon.Eye : ( char )FontAwesomeIcon.Times )}###MainInterfaceFiles-MarkVfx", new Vector2( 28, 23 ) ) ) {
                VfxTracker.Toggle();
                if( !VfxTracker.Enabled ) {
                    VfxTracker.Reset();
                    PluginInterface.UiBuilder.DisableCutsceneUiHide = false;
                }
                else {
                    PluginInterface.UiBuilder.DisableCutsceneUiHide = true;
                }
            }
            ImGui.PopFont();

            ImGui.SameLine(); UIUtils.HelpMarker( @"Use the eye icon to enable or disable the VFX overlay. This will show you the positions of most VFXs in the game world, along with their file paths. Note that you may need to enter and exit your current zone to see all of the VFXs" );

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

        public static void WriteBytesDialog( string filter, string data, string ext ) {
            WriteBytesDialog( filter, Encoding.ASCII.GetBytes( data ), ext );
        }

        public static void WriteBytesDialog( string filter, byte[] data, string ext ) {
            FileDialogManager.SaveFileDialog( "Select a Save Location", filter, "", ext, ( bool ok, string res ) => {
                if( ok ) File.WriteAllBytes( res, data );
            } );
        }

        public static void OpenUrl( string url ) {
            Process.Start( new ProcessStartInfo {
                FileName = url,
                UseShellExecute = true
            } );
        }
    }
}