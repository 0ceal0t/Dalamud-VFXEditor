using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Ui.Export.Categories {
    public class ExportDialogCategory {
        public readonly IFileManagerGroup Group;

        private bool ExportAll = false;
        private readonly Dictionary<IFileDocument, bool> ToExport = [];

        public ExportDialogCategory( IFileManagerGroup group ) {
            Group = group;
        }

        public void Draw() {
            var id = Group.GetId();
            using var _ = ImRaii.PushId( id );

            if( ImGui.Checkbox( "##All", ref ExportAll ) ) {
                foreach( var key in ToExport.Keys ) ToExport[key] = ExportAll;
            }

            ImGui.SameLine();
            var selectedCount = GetItemsToExport().Count();
            var totalCount = Group.GetDocuments().Where( x => x.CanExport() ).Count();
            using var color = ImRaii.PushColor( ImGuiCol.Text, selectedCount == totalCount ? UiUtils.PARSED_GREEN : UiUtils.DALAMUD_ORANGE, selectedCount > 0 );
            if( ImGui.CollapsingHeader( $"{id} [{selectedCount}/{totalCount}]###{id}" ) ) {
                color.Pop();

                using var indent = ImRaii.PushIndent();

                var items = Group.GetDocuments();
                if( !items.Any() ) return;

                using var table = ImRaii.Table( "##Table", 3, ImGuiTableFlags.RowBg | ImGuiTableFlags.NoSavedSettings );
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

        public IEnumerable<IFileDocument> GetItemsToExport() => Group.GetDocuments().Where( DoExport );

        public void Reset() => ToExport.Clear();

        public void RemoveDocument( IFileDocument document ) => ToExport.Remove( document );

        private bool DoExport( IFileDocument item ) => item.CanExport() && ( ToExport.TryGetValue( item, out var _checked ) ? _checked : ExportAll );

        public void WorkspaceImport( Dictionary<string, string> files, Dictionary<IFileManagerGroup, int> offsets ) {
            if( !files.TryGetValue( Group.GetId(), out var data ) ) return;
            var indexes = JsonConvert.DeserializeObject<int[]>( data );
            var documents = Group.GetDocuments().ToList();
            foreach( var index in indexes ) ToExport[documents[index + ( offsets.TryGetValue( Group, out var offset ) ? offset : 0 )]] = true;
        }

        public void WorkspaceExport( Dictionary<string, string> files ) {
            var indexes = new List<int>();
            foreach( var (item, idx) in Group.GetDocuments().WithIndex() ) {
                if( DoExport( item ) ) indexes.Add( idx );
            }
            files[Group.GetId()] = JsonConvert.SerializeObject( indexes.ToArray() );
        }
    }
}
