using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dalamud.Interface;
using Dalamud.Plugin;
using ImGuiNET;

namespace VFXEditor.UI
{

    public class TexToolsDialog : GenericDialog
    {
        public TexToolsDialog( Plugin plugin ) : base(plugin, "TexTools") {
            Size = new Vector2( 400, 200 );
        }

        public string Name = "";
        public string Author = "";
        public string Version = "1.0.0";
        public bool ExportAll = false;
        public bool ExportTex = true;

        public override void OnDraw() {
            var id = "##Textools";
            float footerHeight = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();

            ImGui.BeginChild( id + "/Child", new Vector2( 0, -footerHeight ), true );
            ImGui.InputText( "Mod Name" + id, ref Name, 255 );
            ImGui.InputText( "Author" + id, ref Author, 255 );
            ImGui.InputText( "Version" + id, ref Version, 255 );
            ImGui.Checkbox( "Export Textures", ref ExportTex );
            ImGui.SameLine();
            ImGui.Checkbox( "Export All Documents", ref ExportAll );
            if( !_plugin.Doc.HasReplacePath( ExportAll ) ) {
                ImGui.TextColored( new Vector4( 0.8f, 0.1f, 0.1f, 1.0f ), "Missing Replace Path" );
            }
            ImGui.EndChild();

            if( ImGui.Button( "Export" + id ) ) {
                SaveDialog();
            }
        }

        public void SaveDialog()
        {
            Task.Run( async () => {
                var picker = new SaveFileDialog {
                    Filter = "TexTools Mod (*.ttmp2)|*.ttmp2*|All files (*.*)|*.*",
                    Title = "Select a Save Location.",
                    DefaultExt = "ttmp2",
                    AddExtension = true
                };
                var result = await picker.ShowDialogAsync();
                if( result == DialogResult.OK ) {
                    try {
                        _plugin.TexToolsManager.Export( Name, Author, Version, picker.FileName, ExportAll, ExportTex );
                        Visible = false;
                    }
                    catch( Exception ex ) {
                        PluginLog.LogError( ex, "Could not select a mod location");
                    }
                }
            } );
        }
    }
}
