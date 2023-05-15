using Dalamud.Logging;
using ImGuiFileDialog;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

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

    public class PenumbraDialog : ModExportDialog {
        public PenumbraDialog() : base( "Penumbra" ) { }

        protected override void OnExport() {
            FileDialogManager.SaveFileDialog( "Select a Save Location", ".pmp,.*", ModName, "pmp", ( bool ok, string res ) => {
                if( !ok ) return;
                Export( res );
                Visible = false;
            } );
        }

        private void Export( string saveFile ) {
            try {
                var saveDir = Path.GetDirectoryName( saveFile );
                var tempDir = Path.Combine( saveDir, "VFXEDITOR_PENUMBRA_TEMP" );
                Directory.CreateDirectory( tempDir );

                var files = new Dictionary<string, string>();
                foreach( var manager in Plugin.Managers ) {
                    if( manager == null ) continue;
                    if( !ToExport.TryGetValue( manager.GetExportName(), out var exportItem ) || !exportItem ) continue;
                    manager.PenumbraExport( tempDir, files );
                }

                var meta = new PenumbraMeta {
                    Name = ModName,
                    Author = Author,
                    Description = "Exported from VFXEditor",
                    Version = Version
                };

                var mod = new PenumbraMod {
                    Files = files
                };

                File.WriteAllText( Path.Combine( tempDir, "meta.json" ), JsonConvert.SerializeObject( meta ) );
                File.WriteAllText( Path.Combine( tempDir, "default_mod.json" ), JsonConvert.SerializeObject( mod ) );

                if( File.Exists( saveFile ) ) File.Delete( saveFile );
                ZipFile.CreateFromDirectory( tempDir, saveFile );
                Directory.Delete( tempDir, true );
                PluginLog.Log( "Exported To: " + saveFile );
            }
            catch( Exception e ) {
                PluginLog.Error( e, "Could not export to Penumbra" );
            }
        }
    }
}
