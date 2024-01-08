using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using VfxEditor.FileBrowser;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Ui.Export.Categories;

namespace VfxEditor.Ui.Export {
    [Serializable]
    public class PenumbraNamedItem {
        public string Name = "";
        public string Description = "";
        public int Priority = 0;
    }

    [Serializable]
    public class PenumbraMod : PenumbraNamedItem {
        public Dictionary<string, string> Files = new();
        public Dictionary<string, string> FileSwaps = new();
        public List<object> Manipulations = new();
    }

    [Serializable]
    public class PenumbraGroup : PenumbraNamedItem {
        public string Type = "Single"; // Single / Multi
        public uint DefaultSettings = 0; // Bitmask of 32 defaults
        public List<PenumbraOption> Options = new();
    }

    [Serializable]
    public class PenumbraOption : PenumbraNamedItem {
        public Dictionary<string, string> Files = new();
        public Dictionary<string, string> FileSwaps = new();
        public List<object> Manipulations = new();
    }

    [Serializable]
    public class PenumbraMeta {
        public int FileVersion = 3;
        public string Name = "";
        public string Author = "";
        public string Description = "";
        public string Version = "";
        public string Website = "";
        public List<string> ModTags = new();
    }

    public class PenumbraDialog : ExportDialog {
        // Temp
        private readonly ExportDialogCategorySet ToExport = new();




        public PenumbraDialog() : base( "Penumbra" ) { }

        protected override void OnExport() {
            FileBrowserManager.SaveFileDialog( "Select a Save Location", ".pmp,.*", ModName, "pmp", ( bool ok, string res ) => {
                if( !ok ) return;
                Export( res );
                Hide();
            } );
        }

        // Temp
        protected override void OnDraw() => ToExport.Draw();

        protected override void OnRemoveDocument( IFileDocument document ) => ToExport.RemoveDocument( document );

        protected override void OnReset() => ToExport.Reset();





        private void Export( string saveFile ) {
            try {
                var saveDir = Path.GetDirectoryName( saveFile );
                var tempDir = Path.Combine( saveDir, "VFXEDITOR_PENUMBRA_TEMP" );
                Directory.CreateDirectory( tempDir );

                var filesOut = new Dictionary<string, string>();

                // Temp
                foreach( var category in ToExport.Categories ) {
                    foreach( var item in category.GetItemsToExport() ) item.PenumbraExport( tempDir, "", filesOut );
                }




                var meta = new PenumbraMeta {
                    Name = ModName,
                    Author = Author,
                    Description = "Exported from VFXEditor",
                    Version = Version
                };

                var mod = new PenumbraMod {
                    Files = filesOut
                };

                File.WriteAllText( Path.Combine( tempDir, "meta.json" ), JsonConvert.SerializeObject( meta ) );
                File.WriteAllText( Path.Combine( tempDir, "default_mod.json" ), JsonConvert.SerializeObject( mod ) );

                if( File.Exists( saveFile ) ) File.Delete( saveFile );
                ZipFile.CreateFromDirectory( tempDir, saveFile );
                Directory.Delete( tempDir, true );
                Dalamud.Log( $"Exported To: {saveFile}" );
            }
            catch( Exception e ) {
                Dalamud.Error( e, "Could not export to Penumbra" );
            }
        }
    }
}
