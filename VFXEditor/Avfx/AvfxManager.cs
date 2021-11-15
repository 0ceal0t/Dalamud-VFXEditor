using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.Avfx.Vfx;
using VFXEditor.Data;
using VFXEditor.FileManager;
using VFXEditor.Helper;
using VFXEditor.Textools;
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
                ( SelectResult result ) => SetSourceGlobal( result ),
                showSpawn: true,
                spawnVfxExists: () => Plugin.SpawnExists(),
                removeSpawnVfx: () => Plugin.RemoveSpawnVfx(),
                spawnOnGround: ( string path ) => Plugin.SpawnOnGround( path ),
                spawnOnSelf: ( string path ) => Plugin.SpawnOnSelf( path ),
                spawnOnTarget: ( string path ) => Plugin.SpawnOnTarget( path )
            );
            ReplaceSelect = new VFXSelectDialog(
                "File Select [TARGET]",
                Plugin.Configuration.RecentSelects,
                false,
                ( SelectResult result ) => SetReplaceGlobal( result ),
                showSpawn: true,
                spawnVfxExists: () => Plugin.SpawnExists(),
                removeSpawnVfx: () => Plugin.RemoveSpawnVfx(),
                spawnOnGround: ( string path ) => Plugin.SpawnOnGround( path ),
                spawnOnSelf: ( string path ) => Plugin.SpawnOnSelf( path ),
                spawnOnTarget: ( string path ) => Plugin.SpawnOnTarget( path )
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

        public void PenumbraExport( string modFolder, bool exportVfx, bool exportAll ) {
            if( !exportVfx ) return;

            if( !exportAll ) {
                ActiveDocument.PenumbraExport( modFolder );
                return;
            }
            foreach( var document in Documents ) {
                document.PenumbraExport( modFolder );
            }
        }

        public void TextoolsExport( BinaryWriter writer, bool exportVfx, bool exportAll, List<TTMPL_Simple> simpleParts, ref int modOffset ) {
            if( !exportVfx ) return;

            if( !exportAll ) {
                ActiveDocument.TextoolsExport( writer, simpleParts, ref modOffset );
                return;
            }
            foreach( var document in Documents ) {
                document.TextoolsExport( writer, simpleParts, ref modOffset );
            }
        }
    }
}
