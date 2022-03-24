using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using VFXEditor.Avfx.Vfx;
using VFXEditor.FileManager;
using VFXEditor.Helper;
using VFXSelect;

namespace VFXEditor.Avfx {
    public partial class AvfxDocument : FileManagerDocument<AvfxFile, WorkspaceMetaAvfx> {
        private DateTime LastUpdate = DateTime.Now;

        public AvfxDocument( string writeLocation ) : base( writeLocation, "Vfx", "VFX" ) {
        }
        public AvfxDocument( string writeLocation, string localPath, SelectResult source, SelectResult replace ) : base( writeLocation, localPath, source, replace, "Vfx", "VFX" ) {
        }
        public AvfxDocument( string writeLocation, string localPath, WorkspaceMetaAvfx data ) : this( writeLocation, localPath, data.Source, data.Replace ) {
            CurrentFile.ReadRenamingMap( data.Renaming );
        }

        protected override void LoadLocal( string localPath ) {
            CurrentFile = AvfxHelper.GetLocalFile( localPath, out var avfx ) ? new AvfxFile( avfx ) : CurrentFile;
        }

        protected override void LoadGame( string gamePath ) {
            CurrentFile = AvfxHelper.GetGameFile( gamePath, out var avfx ) ? new AvfxFile( avfx ) : CurrentFile;
        }

        protected override void Update() {
            if( CurrentFile == null ) return;
            if( Plugin.Configuration?.LogAllFiles == true ) PluginLog.Log( "Wrote VFX file to {0}", WriteLocation );
            File.WriteAllBytes( WriteLocation, CurrentFile.ToBytes() );
        }

        protected override bool GetVerified() => CurrentFile.GetVerified();

        protected override void DrawBody() { }

        public override void Draw() {
            ImGui.SetCursorPos( ImGui.GetCursorPos() - new Vector2( 5, 5 ) );
            ImGui.BeginChild( "Child##MainInterface", new Vector2( ImGui.GetWindowWidth() - 0, 60 ) );
            ImGui.SetCursorPos( ImGui.GetCursorPos() + new Vector2( 5, 5 ) );

            ImGui.Columns( 3, "MainInterfaceFileColumns", false );

            // ======== INPUT TEXT =========
            ImGui.SetColumnWidth( 0, 140 );
            ImGui.Text( "Loaded VFX" );
            ImGui.SameLine(); UiHelper.HelpMarker( "The source of the new VFX. For example, if you wanted to replace the Fire animation with that of Blizzard, Blizzard would be the data source" );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Text( "VFX Being Replaced" );
            ImGui.SameLine(); UiHelper.HelpMarker( "The VFX which is being replaced. For example, if you wanted to replace the Fire animation with that of Blizzard, Fire would be the preview vfx" );
            ImGui.NextColumn();

            // ======= SEARCH BARS =========
            var sourceString = SourceDisplay;
            var previewString = ReplaceDisplay;
            ImGui.SetColumnWidth( 1, ImGui.GetWindowWidth() - 255 );
            ImGui.PushItemWidth( ImGui.GetColumnWidth() - 100 );

            ImGui.InputText( "##MainInterfaceFiles-Source", ref sourceString, 255, ImGuiInputTextFlags.ReadOnly );

            ImGui.SameLine();
            ImGui.PushFont( UiBuilder.IconFont );
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Search}", new Vector2( 30, 23 ) ) ) {
                SourceShow();
            }
            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            ImGui.PushStyleColor( ImGuiCol.Button, UiHelper.RED_COLOR );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Times}##MainInterfaceFiles-SourceRemove", new Vector2( 30, 23 ) ) ) {
                RemoveSource();
            }
            ImGui.PopStyleColor();
            ImGui.PopFont();

            ImGui.InputText( "##MainInterfaceFiles-Preview", ref previewString, 255, ImGuiInputTextFlags.ReadOnly );

            ImGui.SameLine();
            ImGui.PushFont( UiBuilder.IconFont );
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Search}##MainInterfaceFiles-PreviewSelect", new Vector2( 30, 23 ) ) ) {
                ReplaceShow();
            }
            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            ImGui.PushStyleColor( ImGuiCol.Button, UiHelper.RED_COLOR );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Times}##MainInterfaceFiles-PreviewRemove", new Vector2( 30, 23 ) ) ) {
                RemoveReplace();
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
            var previewSpawn = Replace.Path;
            var spawnDisabled = string.IsNullOrEmpty( previewSpawn );
            if( !Plugin.SpawnExists() ) {
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
                    Plugin.RemoveSpawnVfx();
                }
            }
            if( ImGui.BeginPopup( "Spawn_Popup" ) ) {
                if( ImGui.Selectable( "On Ground" ) ) {
                    Plugin.SpawnOnGround( previewSpawn );
                }
                if( ImGui.Selectable( "On Self" ) ) {
                    Plugin.SpawnOnSelf( previewSpawn );
                }
                if( ImGui.Selectable( "On Taget" ) ) {
                    Plugin.SpawnOnTarget( previewSpawn );
                }
                ImGui.EndPopup();
            }

            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 6 );
            ImGui.PushFont( UiBuilder.IconFont );
            if( ImGui.Button( $"{( !Plugin.VfxTracker.Enabled ? ( char )FontAwesomeIcon.Eye : ( char )FontAwesomeIcon.Times )}###MainInterfaceFiles-MarkVfx", new Vector2( 28, 23 ) ) ) {
                Plugin.VfxTracker.Toggle();
                if( !Plugin.VfxTracker.Enabled ) {
                    Plugin.VfxTracker.Reset();
                    Plugin.PluginInterface.UiBuilder.DisableCutsceneUiHide = false;
                }
                else {
                    Plugin.PluginInterface.UiBuilder.DisableCutsceneUiHide = true;
                }
            }
            ImGui.PopFont();

            ImGui.SameLine(); UiHelper.HelpMarker( @"Use the eye icon to enable or disable the VFX overlay. This will show you the positions of most VFXs in the game world, along with their file paths. Note that you may need to enter and exit your current zone to see all of the VFXs" );

            ImGui.Columns( 1 );
            ImGui.Separator();
            ImGui.EndChild();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( CurrentFile == null ) {
                ImGui.Text( @"Select a source VFX file to begin..." );
            }
            else {
                if( UiHelper.OkButton( "UPDATE" ) ) {
                    if( ( DateTime.Now - LastUpdate ).TotalSeconds > 0.5 ) { // only allow updates every 1/2 second
                        Update();
                        Reload();
                        Plugin.ResourceLoader.ReRender();
                        LastUpdate = DateTime.Now;
                    }
                }

                // ===== EXPORT ======
                ImGui.SameLine();
                ImGui.PushFont( UiBuilder.IconFont );
                if( ImGui.Button( $"{( char )FontAwesomeIcon.FileDownload}" ) ) {
                    ImGui.OpenPopup( "Export_Popup" );
                }
                ImGui.PopFont();

                if( ImGui.BeginPopup( "Export_Popup" ) ) {
                    if( ImGui.Selectable( ".AVFX" ) ) {
                        UiHelper.WriteBytesDialog( ".avfx", CurrentFile.ToBytes(), "avfx" );
                    }
                    if( ImGui.Selectable( "Export last import (raw)" ) ) {
                        UiHelper.WriteBytesDialog( ".txt", AvfxHelper.LastImportNode.ExportString( 0 ), "txt" );
                    }
                    ImGui.EndPopup();
                }

                // ======== VERIFY ============
                ImGui.SameLine();
                UiHelper.ShowVerifiedStatus( Verified );

                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

                CurrentFile.Draw();
            }
        }

        protected override void SourceShow() => AvfxManager.SourceSelect.Show();

        protected override void ReplaceShow() => AvfxManager.ReplaceSelect.Show();

        private void OpenTemplate( string path ) {
            var newResult = new SelectResult {
                DisplayString = "[NEW]",
                Type = SelectResultType.Local,
                Path = Path.Combine( Plugin.TemplateLocation, "Files", path )
            };
            SetSource( newResult );
        }
    }
}
