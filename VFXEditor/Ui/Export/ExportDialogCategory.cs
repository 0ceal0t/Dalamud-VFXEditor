using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.FileManager.Interfaces;

namespace VfxEditor.Ui.Export {
    public class ExportDialogCategory {
        public readonly IFileManager Manager;

        private bool ExportAll = false;
        private readonly Dictionary<IFileDocument, bool> ToExport = new();

        public ExportDialogCategory( IFileManager manager ) {
            Manager = manager;
        }

        public void Draw() {
            var id = Manager.GetId();
            using var _ = ImRaii.PushId( id );

            if( ImGui.Checkbox( "##All", ref ExportAll ) ) {
                foreach( var key in ToExport.Keys ) ToExport[key] = ExportAll;
            }

            ImGui.SameLine();

            if( ImGui.CollapsingHeader( $"{id} ({GetItemsToExport().Count()}/{Manager.GetDocuments().Where( x => x.CanExport() ).Count()})###{id}" ) ) {
                using var indent = ImRaii.PushIndent( 10f );

                var items = Manager.GetDocuments();
                if( !items.Any() ) return;

                using var table = ImRaii.Table( "##Table", 3, ImGuiTableFlags.RowBg );
                if( !table ) return;

                ImGui.TableSetupColumn( "##Check", ImGuiTableColumnFlags.WidthFixed, 20 );
                ImGui.TableSetupColumn( "##Source", ImGuiTableColumnFlags.WidthStretch );
                ImGui.TableSetupColumn( "##Replace", ImGuiTableColumnFlags.WidthStretch );

                var idx = 0;
                foreach( var item in items ) {
                    using var __ = ImRaii.PushId( idx );

                    using var disabled = ImRaii.Disabled( !item.CanExport() );

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

        public IEnumerable<IFileDocument> GetItemsToExport() => Manager.GetDocuments().Where( DoExport );

        private bool DoExport( IFileDocument item ) => item.CanExport() && ( ToExport.TryGetValue( item, out var _checked ) ? _checked : ExportAll );

        public void RemoveDocument( IFileDocument document ) => ToExport.Remove( document );
    }
}
