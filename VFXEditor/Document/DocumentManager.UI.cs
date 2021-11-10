using ImGuiNET;
using System.Numerics;
using VFXEditor.UI;

namespace VFXEditor.Document {
    public partial class DocumentManager {
        private ReplaceDoc SelectedDocument = null;

        public override void OnDraw() {
            var id = "##Doc";
            var footerHeight = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();

            ImGui.BeginChild( id + "/Child", new Vector2( 0, -footerHeight ), true );

            ImGui.Columns( 2, id + "/Columns", false );

            var idx = 0;
            foreach( var doc in Documents ) {
                if( ImGui.Selectable( doc.Source.DisplayString + id + idx, doc == SelectedDocument, ImGuiSelectableFlags.SpanAllColumns ) ) {
                    SelectedDocument = doc;
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

            foreach( var doc in Documents ) {
                ImGui.Text( doc.Replace.DisplayString );
            }

            ImGui.Columns( 1 );
            ImGui.EndChild();

            if( ImGui.Button( "+ NEW" + id ) ) {
                CreateNewDocument();
            }

            if( SelectedDocument != null ) {
                var deleteDisabled = ( Documents.Count == 1 );

                ImGui.SameLine( ImGui.GetWindowWidth() - 105 );
                if( ImGui.Button( "Select" + id ) ) {
                    SelectDocument( SelectedDocument );
                }
                if( !deleteDisabled ) {
                    ImGui.SameLine( ImGui.GetWindowWidth() - 55 );
                    if( UIUtils.RemoveButton( "Delete" + id ) ) {
                        RemoveDocument( SelectedDocument );
                        SelectedDocument = ActiveDocument;
                    }
                }
            }
        }
    }
}
