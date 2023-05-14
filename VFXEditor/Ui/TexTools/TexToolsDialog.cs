using Dalamud.Logging;
using ImGuiFileDialog;
using ImGuiNET;
using Newtonsoft.Json;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Numerics;
using VfxEditor.Ui;

namespace VfxEditor.TexTools {
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
        private string ModName = "";
        private string Author = "";
        private string Version = "1.0.0";
        private readonly Dictionary<string, bool> ToExport = new();

        public TexToolsDialog() : base( "TexTools", false, 400, 300 ) {
            foreach( var manager in Plugin.Managers ) {
                if( manager == null ) continue;
                ToExport[manager.GetExportName()] = false;
            }
        }

        public override void DrawBody() {
            using var _ = ImRaii.PushId( "TexTools" );

            var footerHeight = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();

            using( var child = ImRaii.Child( "Child", new Vector2( 0, -footerHeight ), true ) ) {
                ImGui.InputText( "Mod Name", ref ModName, 255 );
                ImGui.InputText( "Author", ref Author, 255 );
                ImGui.InputText( "Version", ref Version, 255 );

                foreach( var entry in ToExport ) {
                    var exportItem = entry.Value;
                    if( ImGui.Checkbox( $"Export {entry.Key}", ref exportItem ) ) ToExport[entry.Key] = exportItem;
                }
            }

            if( ImGui.Button( "Export" ) ) SaveDialog();
        }

        private void SaveDialog() {
            FileDialogManager.SaveFileDialog( "Select a Save Location", ".ttmp2,.*", ModName, "ttmp2", ( bool ok, string res ) => {
                if( !ok ) return;
                Export( res );
                Visible = false;
            } );
        }

        private void Export( string saveFile ) {
            try {
                var simpleParts = new List<TTMPL_Simple>();
                byte[] newData;
                var modOffset = 0;

                using( var ms = new MemoryStream() )
                using( var writer = new BinaryWriter( ms ) ) {
                    foreach( var manager in Plugin.Managers ) {
                        if( manager == null ) continue;
                        if( !ToExport.TryGetValue( manager.GetExportName(), out var exportItem ) || !exportItem ) continue;
                        manager.TextoolsExport( writer, simpleParts, ref modOffset );
                    }

                    newData = ms.ToArray();
                }

                var mod = new TTMPL {
                    TTMPVersion = "1.3s",
                    Name = ModName,
                    Author = Author,
                    Version = Version,
                    Description = null,
                    ModPackPages = null,
                    SimpleModsList = simpleParts.ToArray()
                };

                var saveDir = Path.GetDirectoryName( saveFile );
                var tempDir = Path.Combine( saveDir, "VFXEDITOR_TEXTOOLS_TEMP" );
                Directory.CreateDirectory( tempDir );

                var mdpPath = Path.Combine( tempDir, "TTMPD.mpd" );
                var mplPath = Path.Combine( tempDir, "TTMPL.mpl" );
                var mplString = JsonConvert.SerializeObject( mod );
                File.WriteAllText( mplPath, mplString );
                File.WriteAllBytes( mdpPath, newData );

                if( File.Exists( saveFile ) ) File.Delete( saveFile );
                ZipFile.CreateFromDirectory( tempDir, saveFile );
                Directory.Delete( tempDir, true );
                PluginLog.Log( "Exported To: " + saveFile );
            }
            catch( Exception e ) {
                PluginLog.Error( e, "Could not export to TexTools" );
            }
        }
    }
}
