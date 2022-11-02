using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using System;
using System.IO;
using System.Numerics;

using VfxEditor.AvfxFormat.Vfx;
using VfxEditor.Data;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat {
    public partial class AvfxDocument : FileManagerDocument<AvfxFile, WorkspaceMetaAvfx> {
        private DateTime LastUpdate = DateTime.Now;
        private string SpawnPath => Replace.Path;
        private bool SpawnDisabled => string.IsNullOrEmpty( SpawnPath );

        public AvfxDocument( string writeLocation ) : base( writeLocation, "Vfx", "VFX" ) {
        }
        public AvfxDocument( string writeLocation, string localPath, SelectResult source, SelectResult replace ) : base( writeLocation, localPath, source, replace, "Vfx", "VFX" ) {
        }
        public AvfxDocument( string writeLocation, string localPath, WorkspaceMetaAvfx data ) : this( writeLocation, localPath, data.Source, data.Replace ) {
            CurrentFile.ReadRenamingMap( data.Renaming );
        }

        public override void Update() {
            if( ( DateTime.Now - LastUpdate ).TotalSeconds > 0.5 ) { // only allow updates every 1/2 second
                UpdateFile();
                Reload();
                VfxEditor.ResourceLoader.ReRender();
                LastUpdate = DateTime.Now;
            }
        }

        public override void CheckKeybinds() {
            if( VfxEditor.Configuration.CopyVfxKeybind.KeyPressed() ) CopyManager.Copy();
            if( VfxEditor.Configuration.PasteVfxKeybind.KeyPressed() ) CopyManager.Paste();

            if( VfxEditor.Configuration.SpawnOnSelfKeybind.KeyPressed() ) {
                VfxEditor.RemoveSpawn();
                if( !SpawnDisabled ) VfxEditor.SpawnOnSelf( SpawnPath );
            }
            if( VfxEditor.Configuration.SpawnOnGroundKeybind.KeyPressed() ) {
                VfxEditor.RemoveSpawn();
                if( !SpawnDisabled ) VfxEditor.SpawnOnGround( SpawnPath );

            }
            if( VfxEditor.Configuration.SpawnOnTargetKeybind.KeyPressed() ) {
                VfxEditor.RemoveSpawn();
                if( !SpawnDisabled ) VfxEditor.SpawnOnTarget( SpawnPath );

            }
        }

        protected override void LoadLocal( string localPath ) {
            if( File.Exists( localPath ) ) {
                try {
                    using var br = new BinaryReader( File.Open( localPath, FileMode.Open ) );
                    CurrentFile = new AvfxFile( br );
                    UiUtils.OkNotification( "VFX file loaded" );
                }
                catch( Exception e ) {
                    PluginLog.Error(e, "Error Reading File", e );
                    UiUtils.ErrorNotification( "Error reading file" );
                }
            }
        }

        protected override void LoadGame( string gamePath ) {
            if( VfxEditor.DataManager.FileExists( gamePath ) ) {
                try {
                    var file = VfxEditor.DataManager.GetFile( gamePath );
                    using var ms = new MemoryStream( file.Data );
                    using var br = new BinaryReader( ms );
                    CurrentFile = new AvfxFile( br );
                    UiUtils.OkNotification( "VFX file loaded" );
                }
                catch( Exception e ) {
                    PluginLog.Error( e, "Error Reading File" );
                    UiUtils.ErrorNotification( "Error reading file" );
                }
            }
        }

        protected override void UpdateFile() {
            if( CurrentFile == null ) return;
            if( VfxEditor.Configuration?.LogDebug == true ) PluginLog.Log( "Wrote VFX file to {0}", WriteLocation );
            File.WriteAllBytes( WriteLocation, CurrentFile.ToBytes() );
        }

        protected override void ExportRaw() {
            UiUtils.WriteBytesDialog( ".avfx", CurrentFile.ToBytes(), "avfx" );
        }

        protected override bool IsVerified() => CurrentFile.IsVerified;

        protected override void DrawBody() { }

        public override void Draw() {
            ImGui.Columns( 3, "MainInterfaceFileColumns", false );

            // ======== INPUT TEXT =========
            ImGui.SetColumnWidth( 0, 150 );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            UiUtils.HelpMarker( "The source of the new VFX. For example, if you wanted to replace the Fire animation with that of Blizzard, Blizzard would be the loaded VFX" ); ImGui.SameLine();
            ImGui.Text( "Loaded VFX" );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            UiUtils.HelpMarker( "The VFX which is being replaced. For example, if you wanted to replace the Fire animation with that of Blizzard, Fire would be the replaced VFX" ); ImGui.SameLine();
            ImGui.Text( "VFX Being Replaced" );
            ImGui.NextColumn();

            // ======= SEARCH BARS =========
            ImGui.SetColumnWidth( 1, ImGui.GetWindowWidth() - 265 );
            ImGui.PushItemWidth( ImGui.GetColumnWidth() - 100 );

            DisplaySearchBars();

            ImGui.PopItemWidth();

            // ======= TEMPLATES =========
            ImGui.NextColumn();
            ImGui.SetColumnWidth( 3, 150 );

            if( ImGui.Button( $"Library", new Vector2( 80, 23 ) ) ) {
                AvfxManager.NodeLibrary.Show();
            }

            // ======= SPAWN + EYE =========
            if( !VfxEditor.SpawnExists() ) {
                if( SpawnDisabled ) {
                    ImGui.PushStyleVar( ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.5f );
                }
                if( ImGui.Button( "Spawn", new Vector2( 50, 23 ) ) && !SpawnDisabled ) {
                    ImGui.OpenPopup( "Spawn_Popup" );
                }
                if( SpawnDisabled ) {
                    ImGui.PopStyleVar();
                    UiUtils.Tooltip( "Select both a loaded VFX and a VFX to replace in order to use the spawn function" );
                }
            }
            else {
                if( ImGui.Button( "Remove" ) ) VfxEditor.RemoveSpawn();
            }
            if( ImGui.BeginPopup( "Spawn_Popup" ) ) {
                if( ImGui.Selectable( "On Ground" ) ) VfxEditor.SpawnOnGround( SpawnPath );
                if( ImGui.Selectable( "On Self" ) ) VfxEditor.SpawnOnSelf( SpawnPath );
                if( ImGui.Selectable( "On Taget" ) ) VfxEditor.SpawnOnTarget( SpawnPath );
                ImGui.EndPopup();
            }

            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 6 );
            ImGui.PushFont( UiBuilder.IconFont );
            if( ImGui.Button( $"{( !VfxEditor.VfxTracker.Enabled ? ( char )FontAwesomeIcon.Eye : ( char )FontAwesomeIcon.Times )}###MainInterfaceFiles-MarkVfx", new Vector2( 28, 23 ) ) ) {
                VfxEditor.VfxTracker.Toggle();
                if( !VfxEditor.VfxTracker.Enabled ) {
                    VfxEditor.VfxTracker.Reset();
                    VfxEditor.PluginInterface.UiBuilder.DisableCutsceneUiHide = false;
                }
                else {
                    VfxEditor.PluginInterface.UiBuilder.DisableCutsceneUiHide = true;
                }
            }
            ImGui.PopFont();
            UiUtils.Tooltip( "VFX path overlay" );

            ImGui.Columns( 1 );
            ImGui.Separator();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( CurrentFile == null ) DisplayBeginHelpText();
            else {
                DisplayFileControls();

                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
                CurrentFile.Draw();
            }
        }

        public void Import( string path ) {
            if( CurrentFile != null && File.Exists( path ) ) CurrentFile.Import( path );
        }

        public void ShowExportDialog( UiNode node ) => CurrentFile.ShowExportDialog( node );

        protected override void SourceShow() => AvfxManager.SourceSelect.Show();

        protected override void ReplaceShow() => AvfxManager.ReplaceSelect.Show();

        public void OpenTemplate( string path ) {
            var newResult = new SelectResult {
                DisplayString = "[NEW]",
                Type = SelectResultType.Local,
                Path = Path.Combine( VfxEditor.RootLocation, "Files", path )
            };
            SetSource( newResult );
        }
    }
}
