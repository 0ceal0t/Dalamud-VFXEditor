using System;
using VFXEditor.Avfx.Vfx;
using VFXEditor.FileManager;
using VFXSelect;
using VFXSelect.VFX;

namespace VFXEditor.Avfx {
    public class AvfxManager : FileManager<AvfxDocument, WorkspaceMetaAvfx, AvfxFile> {
        public static VFXSelectDialog SourceSelect { get; private set; }
        public static VFXSelectDialog ReplaceSelect { get; private set; }

        public static void Setup() {
            SourceSelect = new VFXSelectDialog(
                "File Select [SOURCE]",
                Plugin.Configuration.RecentSelects,
                true,
                SetSourceGlobal,
                showSpawn: true,
                spawnVfxExists: Plugin.SpawnExists,
                removeSpawnVfx: Plugin.RemoveSpawnVfx,
                spawnOnGround: Plugin.SpawnOnGround,
                spawnOnSelf: Plugin.SpawnOnSelf,
                spawnOnTarget: Plugin.SpawnOnTarget
            );
            ReplaceSelect = new VFXSelectDialog(
                "File Select [TARGET]",
                Plugin.Configuration.RecentSelects,
                false,
                SetReplaceGlobal,
                showSpawn: true,
                spawnVfxExists: Plugin.SpawnExists,
                removeSpawnVfx: Plugin.RemoveSpawnVfx,
                spawnOnGround: Plugin.SpawnOnGround,
                spawnOnSelf: Plugin.SpawnOnSelf,
                spawnOnTarget: Plugin.SpawnOnTarget
            );
        }

        public static void SetSourceGlobal( SelectResult result ) {
            Plugin.AvfxManager?.SetSource( result );
            Plugin.Configuration.AddRecent( result );
        }

        public static void SetReplaceGlobal( SelectResult result ) {
            Plugin.AvfxManager?.SetReplace( result );
            Plugin.Configuration.AddRecent( result );
        }

        public static readonly string PenumbraPath = "VFX";
        
        // =================

        public AvfxManager() : base( title: "VFXEditor", id: "Vfx", tempFilePrefix: "VfxTemp", extension: "avfx", penumbaPath: PenumbraPath ) { }

        protected override AvfxDocument GetNewDocument() => new( LocalPath );

        protected override AvfxDocument GetImportedDocument( string localPath, WorkspaceMetaAvfx data ) => new( LocalPath, localPath, data );

        public override void Dispose() {
            base.Dispose();
            SourceSelect.Hide();
            ReplaceSelect.Hide();
        }

        public override void DrawBody() {
            SourceSelect.Draw();
            ReplaceSelect.Draw();
            base.DrawBody();
        }
    }
}
