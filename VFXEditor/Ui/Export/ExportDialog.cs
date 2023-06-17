using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VfxEditor.FileManager;

namespace VfxEditor.Ui.Export {
    public abstract class ExportDialog : GenericDialog {
        protected string ModName = "";
        protected string Author = "";
        protected string Version = "1.0.0";

        protected readonly Dictionary<string, bool> ToExport = new();

        protected readonly List<ExportDialogCategory> Categories = new();

        public ExportDialog( string id ) : base( id, false, 400, 300 ) {
            foreach( var manager in Plugin.Managers ) {
                if( manager == null ) continue;
                ToExport[manager.GetExportName()] = false;

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

    public class ExportDialogCategory {
        public readonly IFileManager Manager;

        private bool ExportAll = false;
        private readonly Dictionary<IFileDocument, bool> ToExport = new();

        public ExportDialogCategory( IFileManager manager ) {
            Manager = manager;
        }

        public void Draw() {
            var id = Manager.GetExportName();
            using var _ = ImRaii.PushId( id );

            if( ImGui.Checkbox( "##All", ref ExportAll ) ) {
                foreach( var key in ToExport.Keys ) ToExport[key] = ExportAll;
            }

            ImGui.SameLine();

            if( ImGui.CollapsingHeader( id ) ) {
                using var indent = ImRaii.PushIndent( 5f );

                var items = Manager.GetExportDocuments();
                if( items.Count() == 0 ) return;

                using var table = ImRaii.Table( "##Table", 3, ImGuiTableFlags.RowBg );
                if( !table ) return;

                ImGui.TableSetupColumn( "##Check", ImGuiTableColumnFlags.WidthFixed, 20 );
                ImGui.TableSetupColumn( "##Source", ImGuiTableColumnFlags.WidthStretch );
                ImGui.TableSetupColumn( "##Replace", ImGuiTableColumnFlags.WidthStretch );

                var idx = 0;
                foreach( var item in items ) {
                    using var __ = ImRaii.PushId( idx );

                    var isChecked = DoExport( item );

                    ImGui.TableNextRow();

                    ImGui.TableNextColumn();
                    if( ImGui.Checkbox( "##Check", ref isChecked ) ) {
                        if( !isChecked && ExportAll ) ExportAll = false;
                    }

                    ImGui.TableNextColumn();
                    ImGui.Text( item.GetExportSource() );

                    ImGui.TableNextColumn();
                    ImGui.Text( item.GetExportReplace() );

                    ToExport[item] = isChecked;

                    idx++;
                }
            }
        }

        public IEnumerable<IFileDocument> GetItemsToExport() => Manager.GetExportDocuments().Where( DoExport );

        private bool DoExport( IFileDocument item ) => ToExport.TryGetValue( item, out var _checked ) ? _checked : ExportAll;

        public void RemoveDocument( IFileDocument document ) {
            if( ToExport.ContainsKey( document ) ) ToExport.Remove( document );
        }
    }
}
