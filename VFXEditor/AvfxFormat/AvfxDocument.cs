using ImGuiNET;
using OtterGui.Raii;
using System.IO;
using System.Numerics;
using VfxEditor.FileManager;
using VfxEditor.Select;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat {
    public class AvfxDocument : FileManagerDocument<AvfxFile, WorkspaceMetaRenamed> {
        private string SpawnPath => ReplacePath;
        private bool SpawnDisabled => string.IsNullOrEmpty( SpawnPath );

        public AvfxDocument( AvfxManager manager, string writeLocation ) : base( manager, writeLocation, "Vfx", "avfx" ) { }

        public AvfxDocument( AvfxManager manager, string writeLocation, string localPath, string name, SelectResult source, SelectResult replace ) :
            base( manager, writeLocation, localPath, name, source, replace, "Vfx", "avfx" ) { }

        public AvfxDocument( AvfxManager manager, string writeLocation, string localPath, WorkspaceMetaRenamed data ) :
            this( manager, writeLocation, localPath, data.Name, data.Source, data.Replace ) {
            CurrentFile.ReadRenamingMap( data.Renaming );
        }

        public override void CheckKeybinds() {
            base.CheckKeybinds();

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

        protected override AvfxFile FileFromReader( BinaryReader reader ) => new( reader );

        public void Import( string path ) { if( CurrentFile != null && File.Exists( path ) ) CurrentFile.Import( path ); }

        public void ShowExportDialog( AvfxNode node ) => CurrentFile.ShowExportDialog( node );

        public void OpenTemplate( string path ) =>
            SetSource( new SelectResult( SelectResultType.Local, "[TEMPLATE]", Path.Combine( Plugin.RootLocation, "Files", path ) ) );

        public override WorkspaceMetaRenamed GetWorkspaceMeta( string newPath ) => new() {
            Name = Name,
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source,
            Renaming = CurrentFile.GetRenamingMap()
        };

        // ========= DRAWING =============

        protected override bool ExtraInputColumn() => true;

        protected override void DrawSearchBarsColumn() {
            ImGui.SetColumnWidth( 1, ImGui.GetWindowWidth() - 265 );
            ImGui.PushItemWidth( ImGui.GetColumnWidth() - 100 );
            DisplaySourceBar();
            DisplayReplaceBar();
            ImGui.PopItemWidth();
        }

        protected override void DrawExtraColumn() {
            ImGui.SetColumnWidth( 3, 150 );

            if( ImGui.Button( $"Library", new Vector2( 80, 23 ) ) ) Plugin.LibraryManager.Show();

            // Spawn + eye
            if( !Plugin.SpawnExists() ) {
                using( var style = ImRaii.PushStyle( ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.5f, SpawnDisabled ) ) {
                    if( ImGui.Button( "Spawn", new Vector2( 50, 23 ) ) && !SpawnDisabled ) ImGui.OpenPopup( "SpawnPopup" );
                }
                UiUtils.Tooltip( "Select both a loaded VFX and a VFX to replace in order to use the spawn function" );
            }
            else if( ImGui.Button( "Remove" ) ) Plugin.RemoveSpawn();

            if( ImGui.BeginPopup( "SpawnPopup" ) ) {
                if( ImGui.Selectable( "On Ground" ) ) Plugin.SpawnOnGround( SpawnPath );
                if( !Plugin.InGpose && ImGui.Selectable( "On Self" ) ) Plugin.SpawnOnSelf( SpawnPath );
                if( ImGui.Selectable( "On Target" ) ) Plugin.SpawnOnTarget( SpawnPath );
                ImGui.EndPopup();
            }

            using var style2 = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 2, 4 ) );

            ImGui.SameLine();
            Plugin.Tracker.Vfx.DrawEye();
            UiUtils.Tooltip( "VFX path overlay" );
        }
    }
}
