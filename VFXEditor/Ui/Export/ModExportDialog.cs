using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.Numerics;

namespace VfxEditor.Ui.Export {
    public abstract class ModExportDialog : GenericDialog {
        protected string ModName = "";
        protected string Author = "";
        protected string Version = "1.0.0";
        protected readonly Dictionary<string, bool> ToExport = new();

        public ModExportDialog( string id ) : base( id, false, 400, 300 ) {
            foreach( var manager in Plugin.Managers ) {
                if( manager == null ) continue;
                ToExport[manager.GetExportName()] = false;
            }
        }

        public override void DrawBody() {
            using var _ = ImRaii.PushId( Name );

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

            if( ImGui.Button( "Export" ) ) OnExport();
        }

        protected abstract void OnExport();
    }
}
