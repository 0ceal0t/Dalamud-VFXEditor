using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using VfxEditor.FilePicker;

namespace VfxEditor.Ui.Export {
    [Serializable]
    public class PenumbraMod {
        public string Name = "";
        public int Priority = 0;
        public Dictionary<string, string> Files = new();
        public Dictionary<string, string> FileSwaps = new();
        public List<object> Manipulations = new();
        public List<PenumbraMod> Options;
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
        public PenumbraDialog() : base( "Penumbra" ) { }

        protected override void OnExport() {
            FilePickerManager.SaveFileDialog( "Select a Save Location", ".pmp,.*", ModName, "pmp", ( bool ok, string res ) => {
                if( !ok ) return;
                Export( res );
                Hide();
            } );
        }

        private void Export( string saveFile ) {
            try {
                var saveDir = Path.GetDirectoryName( saveFile );
                var tempDir = Path.Combine( saveDir, "VFXEDITOR_PENUMBRA_TEMP" );
                Directory.CreateDirectory( tempDir );

                var filesOut = new Dictionary<string, string>();

                foreach( var category in Categories ) {
                    foreach( var item in category.GetItemsToExport() ) item.PenumbraExport( tempDir, filesOut );
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
