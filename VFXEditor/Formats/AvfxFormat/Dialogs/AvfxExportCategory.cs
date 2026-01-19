using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat.Dialogs {
    public abstract class AvfxExportCategory {
        public abstract HashSet<AvfxNode> GetSelectedNodes();
        public abstract void Reset();
        public abstract void Draw();
        public abstract bool Belongs( AvfxNode node );
        public abstract void Select( AvfxNode node );
    }

    public class ExportDialogCategory<T> : AvfxExportCategory where T : AvfxNode {
        public readonly HashSet<T> Selected = [];
        public NodeGroup<T> Group;
        public string HeaderText;

        public ExportDialogCategory( NodeGroup<T> group, string text ) {
            Group = group;
            Reset();
            Group.OnChange += Update;
            HeaderText = text;
        }

        public void Update() => Selected.RemoveWhere( x => !Group.Items.Contains( x ) );

        public override void Reset() => Selected.Clear();

        public override bool Belongs( AvfxNode node ) => node is T;

        public override void Select( AvfxNode node ) {
            if( node is not T item ) return;
            Selected.Add( item );
        }

        public override HashSet<AvfxNode> GetSelectedNodes() => [.. Selected];

        public override void Draw() {
            using var _ = ImRaii.PushId( HeaderText );

            var exportAll = Selected.Count == Group.Items.Count && Selected.Count > 0;
            if( ImGui.Checkbox( "##All", ref exportAll ) ) {
                Selected.Clear();
                if( exportAll ) {
                    foreach( var item in Group.Items ) Selected.Add( item );
                }
            }

            ImGui.SameLine();
            var selectedCount = Selected.Count;
            var totalCount = Group.Items.Count;
            using var color = ImRaii.PushColor( ImGuiCol.Text, selectedCount == totalCount ? UiUtils.PARSED_GREEN : UiUtils.DALAMUD_ORANGE, selectedCount > 0 );
            ImGui.SetNextItemOpen( false, ImGuiCond.FirstUseEver );
            if( ImGui.CollapsingHeader( $"{HeaderText} [{selectedCount}/{totalCount}]###{HeaderText}" ) ) {
                color.Pop();

                using var indent = ImRaii.PushIndent();

                foreach( var (item, idx) in Group.Items.WithIndex() ) {
                    using var __ = ImRaii.PushId( idx );

                    var nodeSelected = Selected.Contains( item );
                    if( ImGui.Checkbox( item.GetText(), ref nodeSelected ) ) {
                        if( nodeSelected ) Selected.Add( item );
                        else Selected.Remove( item );
                    }
                }
            }
        }
    }
}
