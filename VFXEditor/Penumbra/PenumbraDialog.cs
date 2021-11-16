using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

using Dalamud.Logging;
using ImGuiFileDialog;
using ImGuiNET;
using Newtonsoft.Json;

using VFXEditor.Dialogs;
using VFXEditor.Helper;

namespace VFXEditor.Penumbra {

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

        public PenumbraDialog() : base( "Penumbra" ) {
            Size = new Vector2( 400, 200 );
        }

        public string Name = "";
        public string Author = "";
        public string Version = "1.0.0";
        public bool ExportVfx = true;
        public bool ExportTex = true;
        public bool ExportTmb = true;
        public bool ExportPap = true;

        public override void DrawBody() {
            var id = "##Penumbra";
            var footerHeight = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();

            ImGui.BeginChild( id + "/Child", new Vector2( 0, -footerHeight ), true );

            ImGui.InputText( "Mod Name" + id, ref Name, 255 );
            ImGui.InputText( "Author" + id, ref Author, 255 );
            ImGui.InputText( "Version" + id, ref Version, 255 );

            ImGui.Checkbox( "Export Vfx", ref ExportVfx );
            ImGui.Checkbox( "Export Textures", ref ExportTex );
            ImGui.Checkbox( "Export Tmb", ref ExportTmb );
            ImGui.Checkbox( "Export Pap", ref ExportPap );

            ImGui.EndChild();

            if( ImGui.Button( "Export" + id ) ) {
                SaveDialog();
            }
        }

        private void SaveDialog() {
            FileDialogManager.SaveFolderDialog( "Select a Save Location", Name, ( bool ok, string res ) => {
                if( !ok ) return;
                Export( res );
                Visible = false;
            } );
        }

        private void Export(string modFolder ) {
            try {
                var mod = new PenumbraMod {
                    Name = Name,
                    Author = Author,
                    Description = "Exported from VFXEditor",
                    Version = Version,
                    Website = null,
                    FileSwaps = new Dictionary<string, string>()
                };

                Directory.CreateDirectory( modFolder );
                var modConfig = Path.Combine( modFolder, "meta.json" );
                var configString = JsonConvert.SerializeObject( mod );
                File.WriteAllText( modConfig, configString );

                Plugin.AvfxManager.PenumbraExport( modFolder, ExportVfx );
                Plugin.TextureManager.PenumbraExport( modFolder, ExportTex );
                Plugin.TmbManager.PenumbraExport( modFolder, ExportTmb );
                Plugin.PapManager.PenumbraExport( modFolder, ExportPap );

                PluginLog.Log( "Exported To: " + modFolder );
            }
            catch( Exception e ) {
                PluginLog.Error( e, "Could not export to Penumbra" );
            }
        }
    }
}
