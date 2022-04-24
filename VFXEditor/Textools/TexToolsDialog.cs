using Dalamud.Logging;
using ImGuiFileDialog;
using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Numerics;
using VFXEditor.Dialogs;

namespace VFXEditor.TexTools {
    public struct TTMPL {
        public string TTMPVersion;
        public string Name;
        public string Author;
        public string Version;
#nullable enable
        public string? Description;
        public string? ModPackPages;
#nullable disable
        public TTMPL_Simple[] SimpleModsList;
    }

    public struct TTMPL_Simple {
        public string Name;
        public string Category;
        public string FullPath;
        public bool IsDefault;
        public int ModOffset;
        public int ModSize;
        public string DatFile;
#nullable enable
        public string? ModPackEntry;
#nullable disable
    }

    public class TexToolsDialog : GenericDialog {
        public TexToolsDialog() : base( "TexTools" ) {
            Size = new Vector2( 400, 200 );
        }

        private string Name = "";
        private string Author = "";
        private string Version = "1.0.0";
        private bool ExportVfx = true;
        private bool ExportTex = true;
        private bool ExportTmb = true;
        private bool ExportPap = true;

        public override void DrawBody() {
            var id = "##Textools";
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
            FileDialogManager.SaveFileDialog( "Select a Save Location", ".ttmp2,.*", Name, "ttmp2", ( bool ok, string res ) => {
                if( !ok ) return;
                Export( res );
                Visible = false;
            } );
        }

        private void Export( string saveLocation ) {
            try {
                var simpleParts = new List<TTMPL_Simple>();
                byte[] newData;
                var modOffset = 0;

                using( var ms = new MemoryStream() )
                using( var writer = new BinaryWriter( ms ) ) {
                    Plugin.AvfxManager.TextoolsExport( writer, ExportVfx, simpleParts, ref modOffset );
                    Plugin.TextureManager.TextoolsExport( writer, ExportTex, simpleParts, ref modOffset );
                    Plugin.TmbManager.TextoolsExport( writer, ExportTmb, simpleParts, ref modOffset );
                    Plugin.PapManager.TextoolsExport( writer, ExportPap, simpleParts, ref modOffset );

                    newData = ms.ToArray();
                }

                var mod = new TTMPL {
                    TTMPVersion = "1.3s",
                    Name = Name,
                    Author = Author,
                    Version = Version,
                    Description = null,
                    ModPackPages = null,
                    SimpleModsList = simpleParts.ToArray()
                };

                var saveDir = Path.GetDirectoryName( saveLocation );
                var tempDir = Path.Combine( saveDir, "VFXEDITOR_TEXTOOLS_TEMP" );
                Directory.CreateDirectory( tempDir );
                var mdpPath = Path.Combine( tempDir, "TTMPD.mpd" );
                var mplPath = Path.Combine( tempDir, "TTMPL.mpl" );
                var mplString = JsonConvert.SerializeObject( mod );
                File.WriteAllText( mplPath, mplString );
                File.WriteAllBytes( mdpPath, newData );

                if( File.Exists( saveLocation ) ) File.Delete( saveLocation );
                ZipFile.CreateFromDirectory( tempDir, saveLocation );
                Directory.Delete( tempDir, true );
                PluginLog.Log( "Exported To: " + saveLocation );
            }
            catch( Exception e ) {
                PluginLog.Error( e, "Could not export to TexTools" );
            }
        }
    }
}
