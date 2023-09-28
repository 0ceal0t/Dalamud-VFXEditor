using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.Numerics;

namespace VfxEditor.AvfxFormat.Dialogs {
    public abstract class ExportDialogCategory {
        public HashSet<AvfxNode> Selected;
        public abstract void Reset();
        public abstract void Draw();
        public abstract bool Belongs( AvfxNode node );
        public abstract void Select( AvfxNode node );
    }

    public class ExportDialogCategory<T> : ExportDialogCategory where T : AvfxNode {
        public NodeGroup<T> Group;
        public string HeaderText;

        public ExportDialogCategory( NodeGroup<T> group, string text ) {
            Group = group;
            Reset();
            Group.OnChange += Reset;
            HeaderText = text;
        }

        public override void Reset() {
            Selected = new();
        }

        public override bool Belongs( AvfxNode node ) => node is T;

        public override void Select( AvfxNode node ) => Selected.Add( node );

        public override void Draw() {
            ImGui.SetNextItemOpen( false, ImGuiCond.FirstUseEver );

            using var color = ImRaii.PushColor( ImGuiCol.Text, new Vector4( 0.10f, 0.90f, 0.10f, 1.0f ), Selected.Count > 0 );
            if( ImGui.CollapsingHeader( $"{HeaderText} ({Selected.Count} Selected / {Group.Items.Count})###ExportUI/{HeaderText}" ) ) {
                color.Pop();

                using var indent = ImRaii.PushIndent();

                foreach( var item in Group.Items ) {
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
