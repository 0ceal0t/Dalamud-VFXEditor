using System.Numerics;
using ImGuiFileDialog;
using ImGuiNET;
using VFXEditor.Data;
using VFXEditor.External;

namespace VFXEditor.UI
{

    public class TexToolsDialog : GenericDialog
    {
        public TexToolsDialog() : base( "TexTools") {
            Size = new Vector2( 400, 200 );
        }

        public string Name = "";
        public string Author = "";
        public string Version = "1.0.0";
        public bool ExportAll = false;
        public bool ExportTex = true;

        public override void OnDraw() {
            var id = "##Textools";
            var footerHeight = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();

            ImGui.BeginChild( id + "/Child", new Vector2( 0, -footerHeight ), true );
            ImGui.InputText( "Mod Name" + id, ref Name, 255 );
            ImGui.InputText( "Author" + id, ref Author, 255 );
            ImGui.InputText( "Version" + id, ref Version, 255 );
            ImGui.Checkbox( "Export Textures", ref ExportTex );
            ImGui.SameLine();
            ImGui.Checkbox( "Export All Documents", ref ExportAll );
            if( !DocumentManager.Manager.HasReplacePath( ExportAll ) ) {
                ImGui.TextColored( VFX.UIUtils.RED_COLOR, "Missing Replace Path" );
            }
            ImGui.EndChild();

            if( ImGui.Button( "Export" + id ) ) {
                SaveDialog();
            }
        }

        public void SaveDialog() {
            FileDialogManager.SaveFileDialog( "Select a Save Location", ".ttmp2,.*", Name, "ttmp2", ( bool ok, string res ) =>
             {
                 if( !ok ) return;
                 TexTools.Export( Name, Author, Version, res, ExportAll, ExportTex );
                 Visible = false;
             } );
        }
    }
}
