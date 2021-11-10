using ImGuiNET;
using System.Numerics;
using VFXEditor.UI;

namespace VFXEditor.Document {
    public partial class DocumentManager {
        private ReplaceDoc SelectedDoc = null;
        public override void OnDraw() {
            var id = "##Doc";
            var footerHeight = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();

            ImGui.BeginChild( id + "/Child", new Vector2( 0, -footerHeight ), true );

            ImGui.Columns( 2, id + "/Columns", false );

            var idx = 0;
            foreach( var doc in CurrentDocs ) {
                if( ImGui.Selectable( doc.Source.DisplayString + id + idx, doc == SelectedDoc, ImGuiSelectableFlags.SpanAllColumns ) ) {
                    SelectedDoc = doc;
                }
                if( ImGui.IsItemHovered() ) {
                    ImGui.BeginTooltip();
                    ImGui.Text( "Replace path: " + doc.Replace.Path );
                    ImGui.Text( "Write path: " + doc.WriteLocation );
                    ImGui.EndTooltip();
                }
                idx++;
            }
            ImGui.NextColumn();

            foreach( var doc in CurrentDocs ) {
                ImGui.Text( doc.Replace.DisplayString );
            }

            ImGui.Columns( 1 );
            ImGui.EndChild();

            if( ImGui.Button( "+ NEW" + id ) ) {
                Manager.NewDoc();
            }

            if( SelectedDoc != null ) {
                var deleteDisabled = ( CurrentDocs.Count == 1 );

                ImGui.SameLine( ImGui.GetWindowWidth() - 105 );
                if( ImGui.Button( "Select" + id ) ) {
                    Manager.SelectDoc( SelectedDoc );
                }
                if( !deleteDisabled ) {
                    ImGui.SameLine( ImGui.GetWindowWidth() - 55 );
                    if( UIUtils.RemoveButton( "Delete" + id ) ) {
                        Manager.RemoveDoc( SelectedDoc );
                        SelectedDoc = CurrentActiveDoc;
                    }
                }
            }
        }
    }
}
