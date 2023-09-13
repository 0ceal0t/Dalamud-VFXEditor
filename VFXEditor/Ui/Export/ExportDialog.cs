using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.FileManager.Interfaces;

namespace VfxEditor.Ui.Export {
    public abstract class ExportDialog : GenericDialog {
        protected string ModName = "";
        protected string Author = "";
        protected string Version = "1.0.0";

        protected readonly Dictionary<string, bool> ToExport = new();

        protected readonly List<ExportDialogCategory> Categories = new();

        public ExportDialog( string id ) : base( id, false, 600, 500 ) {
            foreach( var manager in Plugin.Managers ) {
                if( manager == null ) continue;
                ToExport[manager.GetId()] = false;

                Categories.Add( new ExportDialogCategory( manager ) );
            }
        }

        public override void DrawBody() {
            using var _ = ImRaii.PushId( Name );

            ImGui.InputText( "Mod Name", ref ModName, 255 );
            ImGui.InputText( "Author", ref Author, 255 );
            ImGui.InputText( "Version", ref Version, 255 );

            var footerHeight = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();

            using( var child = ImRaii.Child( "Child", new Vector2( 0, -footerHeight ), true ) ) {
                foreach( var category in Categories ) category.Draw();
            }

            if( ImGui.Button( "Export" ) ) OnExport();
        }

        protected abstract void OnExport();

        public void RemoveDocument( IFileDocument document ) => Categories.ForEach( x => x.RemoveDocument( document ) );
    }
}
