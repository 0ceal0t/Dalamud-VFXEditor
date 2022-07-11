using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using System;
using System.IO;
using System.Numerics;

using VFXEditor.AVFX.VFX;
using VFXEditor.FileManager;
using VFXEditor.Helper;
using VFXSelect;

namespace VFXEditor.AVFX {
    public partial class AVFXDocument : FileManagerDocument<AVFXFile, WorkspaceMetaAvfx> {
        private DateTime LastUpdate = DateTime.Now;

        public AVFXDocument( string writeLocation ) : base( writeLocation, "Vfx", "VFX" ) {
        }
        public AVFXDocument( string writeLocation, string localPath, SelectResult source, SelectResult replace ) : base( writeLocation, localPath, source, replace, "Vfx", "VFX" ) {
        }
        public AVFXDocument( string writeLocation, string localPath, WorkspaceMetaAvfx data ) : this( writeLocation, localPath, data.Source, data.Replace ) {
            CurrentFile.ReadRenamingMap( data.Renaming );
        }

        protected override void LoadLocal( string localPath ) {
            if( File.Exists( localPath ) ) {
                try {
                    using var br = new BinaryReader( File.Open( localPath, FileMode.Open ) );
                    CurrentFile = new AVFXFile( br );
                    UIHelper.OkNotification( "VFX file loaded" );
                }
                catch( Exception e ) {
                    PluginLog.Error( "Error Reading File", e );
                    PluginLog.Error( e.ToString() );
                    UIHelper.ErrorNotification( "Error reading file" );
                }
            }
        }

        protected override void LoadGame( string gamePath ) {
            if( Plugin.DataManager.FileExists( gamePath ) ) {
                try {
                    var file = Plugin.DataManager.GetFile( gamePath );
                    using var ms = new MemoryStream( file.Data );
                    using var br = new BinaryReader( ms );
                    CurrentFile = new AVFXFile( br );
                    UIHelper.OkNotification( "VFX file loaded" );
                }
                catch( Exception e ) {
                    PluginLog.Error( "Error Reading File", e );
                    PluginLog.Error( e.ToString() );
                    UIHelper.ErrorNotification( "Error reading file" );
                }
            }
        }

        protected override void Update() {
            if( CurrentFile == null ) return;
            if( Plugin.Configuration?.LogDebug == true ) PluginLog.Log( "Wrote VFX file to {0}", WriteLocation );
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
            ImGui.SameLine(); UIHelper.HelpMarker( "The source of the new VFX. For example, if you wanted to replace the Fire animation with that of Blizzard, Blizzard would be the data source" );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Text( "VFX Being Replaced" );
            ImGui.SameLine(); UIHelper.HelpMarker( "The VFX which is being replaced. For example, if you wanted to replace the Fire animation with that of Blizzard, Fire would be the preview vfx" );
            ImGui.NextColumn();

            // ======= SEARCH BARS =========
            var sourceString = SourceDisplay;
            var previewString = ReplaceDisplay;
            ImGui.SetColumnWidth( 1, ImGui.GetWindowWidth() - 255 );
            ImGui.PushItemWidth( ImGui.GetColumnWidth() - 100 );

            // Remove
            ImGui.PushFont( UiBuilder.IconFont );
            ImGui.PushStyleColor( ImGuiCol.Button, UIHelper.RED_COLOR );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Times}##MainInterfaceFiles-SourceRemove", new Vector2( 30, 23 ) ) ) {
                RemoveSource();
            }
            ImGui.PopStyleColor();
            ImGui.PopFont();
            // Input
            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            ImGui.InputText( "##MainInterfaceFiles-Source", ref sourceString, 255, ImGuiInputTextFlags.ReadOnly );
            // Search
            ImGui.SameLine();
            ImGui.PushFont( UiBuilder.IconFont );
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Search}", new Vector2( 30, 23 ) ) ) {
                SourceShow();
            }
            ImGui.PopFont();

            // Remove
            ImGui.PushFont( UiBuilder.IconFont );
            ImGui.PushStyleColor( ImGuiCol.Button, UIHelper.RED_COLOR );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Times}##MainInterfaceFiles-PreviewRemove", new Vector2( 30, 23 ) ) ) {
                RemoveReplace();
            }
            ImGui.PopStyleColor();
            ImGui.PopFont();
            // Input
            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            ImGui.InputText( "##MainInterfaceFiles-Preview", ref previewString, 255, ImGuiInputTextFlags.ReadOnly );
            // Search
            ImGui.SameLine();
            ImGui.PushFont( UiBuilder.IconFont );
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Search}##MainInterfaceFiles-PreviewSelect", new Vector2( 30, 23 ) ) ) {
                ReplaceShow();
            }
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

            // ======= SPAWN + EYE =========
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
            UIHelper.Tooltip( "VFX path overlay" );

            ImGui.Columns( 1 );
            ImGui.Separator();
            ImGui.EndChild();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( CurrentFile == null ) {
                DisplayBeginHelpText();
            }
            else {
                if( UIHelper.OkButton( "UPDATE" ) ) {
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
                        UIHelper.WriteBytesDialog( ".avfx", CurrentFile.ToBytes(), "avfx" );
                    }
                    ImGui.EndPopup();
                }

                // ======== VERIFY ============
                ImGui.SameLine();
                UIHelper.ShowVerifiedStatus( Verified );

                // ======== NODE LIBRARY =======
                ImGui.SameLine();
                ImGui.SetCursorPosX( ImGui.GetCursorPosX() + ImGui.GetContentRegionAvail().X - 37 );
                ImGui.PushFont( UiBuilder.IconFont );
                if( ImGui.Button( $"{( char )FontAwesomeIcon.Book}" ) ) {
                    AVFXManager.NodeLibrary.Show();
                }
                ImGui.PopFont();
                UIHelper.Tooltip( "VFX node library" );

                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

                CurrentFile.Draw();
            }
        }

        public void Import( string path ) {
            if( CurrentFile != null && File.Exists( path ) ) CurrentFile.Import( path );
        }

        protected override void SourceShow() => AVFXManager.SourceSelect.Show();

        protected override void ReplaceShow() => AVFXManager.ReplaceSelect.Show();

        private void OpenTemplate( string path ) {
            var newResult = new SelectResult {
                DisplayString = "[NEW]",
                Type = SelectResultType.Local,
                Path = Path.Combine( Plugin.RootLocation, "Files", path )
            };
            SetSource( newResult );
        }
    }
}
