using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using System;
using System.IO;
using System.Numerics;
using VfxEditor.Data;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat {
    public partial class AvfxDocument : FileManagerDocument<AvfxFile, WorkspaceMetaAvfx> {
        private DateTime LastUpdate = DateTime.Now;
        private string SpawnPath => Replace.Path;
        private bool SpawnDisabled => string.IsNullOrEmpty( SpawnPath );

        public AvfxDocument( string writeLocation ) : base( writeLocation, "Vfx" ) { }
        public AvfxDocument( string writeLocation, string localPath, SelectResult source, SelectResult replace ) : base( writeLocation, localPath, source, replace, "Vfx" ) { }
        public AvfxDocument( string writeLocation, string localPath, WorkspaceMetaAvfx data ) : this( writeLocation, localPath, data.Source, data.Replace ) {
            CurrentFile.ReadRenamingMap( data.Renaming );
        }

        public override void Update() {
            if( ( DateTime.Now - LastUpdate ).TotalSeconds <= 0.5 ) return;
            UpdateFile();
            Reload();
            Plugin.ResourceLoader.ReRender();
            LastUpdate = DateTime.Now;
        }

        public override void CheckKeybinds() {
            if( Plugin.Configuration.CopyKeybind.KeyPressed() ) CopyManager.Avfx.Copy();
            if( Plugin.Configuration.PasteKeybind.KeyPressed() ) CopyManager.Avfx.Paste();
            if( Plugin.Configuration.UndoKeybind.KeyPressed() ) CommandManager.Avfx?.Undo();
            if( Plugin.Configuration.RedoKeybind.KeyPressed() ) CommandManager.Avfx?.Redo();

            if( Plugin.Configuration.SpawnOnSelfKeybind.KeyPressed() ) {
                Plugin.RemoveSpawn();
                if( !SpawnDisabled ) Plugin.SpawnOnSelf( SpawnPath );
            }
            if( Plugin.Configuration.SpawnOnGroundKeybind.KeyPressed() ) {
                Plugin.RemoveSpawn();
                if( !SpawnDisabled ) Plugin.SpawnOnGround( SpawnPath );

            }
            if( Plugin.Configuration.SpawnOnTargetKeybind.KeyPressed() ) {
                Plugin.RemoveSpawn();
                if( !SpawnDisabled ) Plugin.SpawnOnTarget( SpawnPath );
            }
        }

        protected override string GetExtensionWithoutDot() => "avfx";

        protected override AvfxFile FileFromReader( BinaryReader reader ) => new( reader );

        protected override void SourceShow() => AvfxManager.SourceSelect.Show();

        protected override void ReplaceShow() => AvfxManager.ReplaceSelect.Show();

        public void Import( string path ) { if( CurrentFile != null && File.Exists( path ) ) CurrentFile.Import( path ); }

        public void ShowExportDialog( AvfxNode node ) => CurrentFile.ShowExportDialog( node );

        public void OpenTemplate( string path ) {
            var newResult = new SelectResult {
                DisplayString = "[NEW]",
                Type = SelectResultType.Local,
                Path = Path.Combine( Plugin.RootLocation, "Files", path )
            };
            SetSource( newResult );
        }

        public override WorkspaceMetaAvfx GetWorkspaceMeta( string newPath ) => new() {
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source,
            Renaming = CurrentFile.GetRenamingMap()
        };

        // ========= DRAWING =============

        protected override bool ExtraInputColumn() => true;

        protected override void DrawInputTextColumn() {
            ImGui.SetColumnWidth( 0, 150 );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            UiUtils.HelpMarker( "The source of the new VFX. For example, if you wanted to replace the Fire animation with that of Blizzard, Blizzard would be the loaded VFX" ); ImGui.SameLine();
            ImGui.Text( "Loaded VFX" );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            UiUtils.HelpMarker( "The VFX which is being replaced. For example, if you wanted to replace the Fire animation with that of Blizzard, Fire would be the replaced VFX" ); ImGui.SameLine();
            ImGui.Text( "VFX Being Replaced" );
        }

        protected override void DrawSearchBarsColumn() {
            ImGui.SetColumnWidth( 1, ImGui.GetWindowWidth() - 265 );
            ImGui.PushItemWidth( ImGui.GetColumnWidth() - 100 );
            DisplaySearchBars();
            ImGui.PopItemWidth();
        }

        protected override void DrawExtraColumn() {
            ImGui.SetColumnWidth( 3, 150 );

            if( ImGui.Button( $"Library", new Vector2( 80, 23 ) ) ) AvfxManager.NodeLibrary.Show();

            // Spawn + eye
            if( !Plugin.SpawnExists() ) {
                if( SpawnDisabled ) ImGui.PushStyleVar( ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.5f );
                if( ImGui.Button( "Spawn", new Vector2( 50, 23 ) ) && !SpawnDisabled ) ImGui.OpenPopup( "Spawn_Popup" );
                if( SpawnDisabled ) {
                    ImGui.PopStyleVar();
                    UiUtils.Tooltip( "Select both a loaded VFX and a VFX to replace in order to use the spawn function" );
                }
            }
            else {
                if( ImGui.Button( "Remove" ) ) Plugin.RemoveSpawn();
            }

            if( ImGui.BeginPopup( "Spawn_Popup" ) ) {
                if( ImGui.Selectable( "On Ground" ) ) Plugin.SpawnOnGround( SpawnPath );
                if( ImGui.Selectable( "On Self" ) ) Plugin.SpawnOnSelf( SpawnPath );
                if( ImGui.Selectable( "On Taget" ) ) Plugin.SpawnOnTarget( SpawnPath );
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
            UiUtils.Tooltip( "VFX path overlay" );
        }

        protected override void DrawBody() {
            ImGui.Separator();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( CurrentFile == null ) DisplayBeginHelpText();
            else {
                DisplayFileControls();
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
                CurrentFile.Draw( "##Avfx" );
            }
        }
    }
}
