using Dalamud.Logging;
using ImGuiFileDialog;
using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Ui;

namespace VfxEditor.Penumbra {
    public class PenumbraDialog : GenericDialog {
        private struct PenumbraMod {
            public string Name;
            public string Author;
            public string Description;
#nullable enable
            public string? Version;
            public string? Website;
#nullable disable
            public Dictionary<string, string> FileSwaps;
        }

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
            FileDialogManager.SaveFolderDialog( "Select a Save Location", ModName, ( bool ok, string res ) => {
                if( !ok ) return;
                Export( res );
                Visible = false;
            } );
        }

        private void Export( string modFolder ) {
            try {
                var mod = new PenumbraMod {
                    Name = ModName,
                    Author = Author,
                    Description = "Exported from VFXEditor",
                    Version = Version,
                    Website = null,
                    FileSwaps = new Dictionary<string, string>()
                };

                Directory.CreateDirectory( modFolder );
                File.WriteAllText( Path.Combine( modFolder, "meta.json" ), JsonConvert.SerializeObject( mod ) );

                foreach( var manager in Plugin.Managers ) {
                    if( manager == null ) continue;
                    if( !ToExport.TryGetValue( manager.GetExportName(), out var exportItem ) || !exportItem ) continue;
                    manager.PenumbraExport( modFolder );
                }

                PluginLog.Log( "Exported To: " + modFolder );
            }
            catch( Exception e ) {
                PluginLog.Error( e, "Could not export to Penumbra" );
            }
        }
    }
}
