using ImGuiNET;
using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Ui.Export {
    public class ExportDialogCategory {
        public readonly IFileManager Manager;

        private bool ExportAll = false;
        private Dictionary<IFileDocument, bool> ToExport = new();

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
            var selectedCount = GetItemsToExport().Count();
            var totalCount = Manager.GetDocuments().Where( x => x.CanExport() ).Count();
            using var color = ImRaii.PushColor( ImGuiCol.Text, selectedCount == totalCount ? UiUtils.PARSED_GREEN : UiUtils.YELLOW_COLOR, selectedCount > 0 );
            if( ImGui.CollapsingHeader( $"{id} [{selectedCount}/{totalCount}]###{id}" ) ) {
                color.Pop();

                using var indent = ImRaii.PushIndent();

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

        public void Tick() {
            var items = Manager.GetDocuments();
            // Remove items there are no longer there
            ToExport = ToExport.Where( x => items.Contains( x.Key ) ).ToDictionary( x => x.Key, x => x.Value );
        }

        private bool DoExport( IFileDocument item ) => item.CanExport() && ( ToExport.TryGetValue( item, out var _checked ) ? _checked : ExportAll );
    }
}
