using Dalamud.Logging;
using ImGuiFileDialog;
using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Numerics;
using VfxEditor.Ui;

namespace VfxEditor.Penumbra {
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

    public class PenumbraDialog : GenericDialog {

        private string ModName = "";
        private string Author = "";
        private string Version = "1.0.0";
        private readonly Dictionary<string, bool> ToExport = new();

        public PenumbraDialog() : base( "Penumbra", false, 400, 300 ) {
            foreach( var manager in Plugin.Managers ) {
                if( manager == null ) continue;
                ToExport[manager.GetExportName()] = false;
            }
        }

        public override void DrawBody() {
            var id = "##Penumbra";
            var footerHeight = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();

            ImGui.BeginChild( id + "/Child", new Vector2( 0, -footerHeight ), true );

            ImGui.InputText( "Mod Name" + id, ref ModName, 255 );
            ImGui.InputText( "Author" + id, ref Author, 255 );
            ImGui.InputText( "Version" + id, ref Version, 255 );

            foreach( var entry in ToExport ) {
                var exportItem = entry.Value;
                if( ImGui.Checkbox( $"Export {entry.Key}{id}", ref exportItem ) ) ToExport[entry.Key] = exportItem;
            }

            ImGui.EndChild();

            if( ImGui.Button( "Export" + id ) ) SaveDialog();
        }

        private void SaveDialog() {
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
