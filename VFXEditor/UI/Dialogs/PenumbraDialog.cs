using System;
using System.IO;
using System.Numerics;
using Dalamud.Plugin;
using ImGuiFileDialog;
using ImGuiNET;
using VFXEditor.Data;
using VFXEditor.Document;
using VFXEditor.External;

namespace VFXEditor.UI {
    public class PenumbraDialog : GenericDialog {
        public PenumbraDialog() : base( "Penumbra" ) {
            Size = new Vector2( 400, 200 );
        }

        public string Name = "";
        public string Author = "";
        public string Version = "1.0.0";
        public bool ExportAll = false;
        public bool ExportTex = true;

        public override void OnDraw() {
            var id = "##Penumbra";
            var footerHeight = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();

            ImGui.BeginChild( id + "/Child", new Vector2( 0, -footerHeight ), true );
            ImGui.InputText( "Mod Name" + id, ref Name, 255 );
            ImGui.InputText( "Author" + id, ref Author, 255 );
            ImGui.InputText( "Version" + id, ref Version, 255 );
            ImGui.Checkbox( "Export Textures", ref ExportTex );
            ImGui.SameLine();
            ImGui.Checkbox( "Export All Documents", ref ExportAll );
            if( !Plugin.DocumentManager.HasReplacePath( ExportAll ) ) {
                ImGui.TextColored( UIUtils.RED_COLOR, "Missing Replace Path" );
            }
            ImGui.EndChild();

            if( ImGui.Button( "Export" + id ) ) {
                SaveDialog();
            }
        }

        public void SaveDialog() {
            FileDialogManager.SaveFolderDialog( "Select a Save Location", Name, ( bool ok, string res ) => {
                if( !ok ) return;
                Penumbra.Export( Name, Author, Version, res, ExportAll, ExportTex );
                Visible = false;
            } );
        }
    }
}
