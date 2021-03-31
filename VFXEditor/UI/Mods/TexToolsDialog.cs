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
        }

        public string Name = "";
        public string Author = "";
        public string Version = "1.0.0";
        public string SaveLocation = "";
        public string VFXPath = "";
        public bool ExportAll = false;
        public bool ExportTex = true;

        public override void OnDraw() {
            var id = "##Textools";
            float footerHeight = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();

            ImGui.BeginChild( id + "/Child", new Vector2( 0, -footerHeight ), true );
            ImGui.InputText( "Mod Name" + id, ref Name, 255 );
            ImGui.InputText( "Author" + id, ref Author, 255 );
            ImGui.InputText( "Version" + id, ref Version, 255 );
            ImGui.InputText( "Save Location" + id, ref SaveLocation, 255 );
            ImGui.SameLine( ImGui.GetWindowWidth() - 58 );
            if( ImGui.Button( "Browse" + id ) ) {
                SaveDialog();
            }
            ImGui.Checkbox( "Export Textures", ref ExportTex );
            ImGui.Separator();
            ImGui.Checkbox( "Export All", ref ExportAll );
            if( !ExportAll ) {
                ImGui.InputText( "VFX to Replace" + id, ref VFXPath, 255 );
                ImGui.SameLine( ImGui.GetWindowWidth() - 85 );
                if( ImGui.Button( "Use Preview" + id ) ) {
                    VFXPath = _plugin.ReplaceAVFXPath;
                }
            }
            ImGui.EndChild();

            if( ImGui.Button( "EXPORT" + id ) ) {
                _plugin.TexToolsManager.Export( Name, Author, Version, VFXPath, SaveLocation, ExportAll, ExportTex );
                Visible = false;
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
                        SaveLocation = picker.FileName;
                    }
                    catch( Exception ex ) {
                        PluginLog.LogError( ex, "Could not select a mod location");
                    }
                }
            } );
        }
    }
}
