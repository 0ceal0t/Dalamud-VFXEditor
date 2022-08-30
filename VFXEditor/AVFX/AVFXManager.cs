using ImGuiNET;
using VFXEditor.AVFX.VFX;
using VFXEditor.Data;
using VFXEditor.FileManager;
using VFXEditor.NodeLibrary;
using VFXEditor.Select.VFX;

namespace VFXEditor.AVFX {
    public class AVFXManager : FileManager<AVFXDocument, WorkspaceMetaAvfx, AVFXFile> {
        public static VFXSelectDialog SourceSelect { get; private set; }
        public static VFXSelectDialog ReplaceSelect { get; private set; }
        public static VFXNodeLibrary NodeLibrary { get; private set; }

        public static void Setup() {
            SourceSelect = new VFXSelectDialog(
                "File Select [SOURCE]",
                VfxEditor.Configuration.RecentSelects,
                true,
                SetSourceGlobal,
                showSpawn: true,
                spawnVfxExists: VfxEditor.SpawnExists,
                removeSpawnVfx: VfxEditor.RemoveSpawn,
                spawnOnGround: VfxEditor.SpawnOnGround,
                spawnOnSelf: VfxEditor.SpawnOnSelf,
                spawnOnTarget: VfxEditor.SpawnOnTarget
            );
            ReplaceSelect = new VFXSelectDialog(
                "File Select [TARGET]",
                VfxEditor.Configuration.RecentSelects,
                false,
                SetReplaceGlobal,
                showSpawn: true,
                spawnVfxExists: VfxEditor.SpawnExists,
                removeSpawnVfx: VfxEditor.RemoveSpawn,
                spawnOnGround: VfxEditor.SpawnOnGround,
                spawnOnSelf: VfxEditor.SpawnOnSelf,
                spawnOnTarget: VfxEditor.SpawnOnTarget
            );
            NodeLibrary = new( VfxEditor.Configuration.VFXNodeLibraryItems, VfxEditor.Configuration.WriteLocation );
        }

        public static void SetSourceGlobal( SelectResult result ) {
            VfxEditor.AvfxManager?.SetSource( result );
            VfxEditor.Configuration.AddRecent( VfxEditor.Configuration.RecentSelects, result );
        }

        public static void SetReplaceGlobal( SelectResult result ) {
            VfxEditor.AvfxManager?.SetReplace( result );
            VfxEditor.Configuration.AddRecent( VfxEditor.Configuration.RecentSelects, result );
        }

        public static readonly string PenumbraPath = "VFX";

        // =================

        public AVFXManager() : base( title: "VFXEditor", id: "Vfx", tempFilePrefix: "VfxTemp", extension: "avfx", penumbaPath: PenumbraPath ) { }

        protected override AVFXDocument GetNewDocument() => new( LocalPath );

        protected override AVFXDocument GetImportedDocument( string localPath, WorkspaceMetaAvfx data ) => new( LocalPath, localPath, data );

        protected override void DrawMenu() {
            if( ImGui.BeginMenu( "Edit##Menu" ) ) {
                if( ImGui.MenuItem( "Copy##Menu" ) ) CopyManager.Copy();
                if( ImGui.MenuItem( "Paste##Menu" ) ) CopyManager.Paste();

                if( ImGui.BeginMenu( "Templates##Menu" ) ) {
                    if( ImGui.MenuItem( "Blank##Menu" ) ) ActiveDocument?.OpenTemplate( @"default_vfx.avfx" );
                    if( ImGui.MenuItem( "Weapon##Menu" ) ) ActiveDocument?.OpenTemplate( @"default_weapon.avfx" );
                    ImGui.EndMenu();
                }

                ImGui.EndMenu();
            }
        }

        public override void Dispose() {
            base.Dispose();
            SourceSelect.Hide();
            ReplaceSelect.Hide();
        }

        public override void DrawBody() {
            SourceSelect.Draw();
            ReplaceSelect.Draw();
            NodeLibrary.Draw();
            base.DrawBody();
        }

        public void Import( string path ) => ActiveDocument.Import( path );

        public void ShowExportDialog( UINode node ) => ActiveDocument.ShowExportDialog( node );
    }
}
