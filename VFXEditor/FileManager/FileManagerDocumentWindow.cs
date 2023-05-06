using ImGuiNET;
using System.Numerics;
using VfxEditor.Ui;
using VfxEditor.Utils;

namespace VfxEditor.FileManager {
    public class FileManagerDocumentWindow<T, R, S> : GenericDialog where T : FileManagerDocument<R, S> where R : FileManagerFile {
        private readonly FileManagerWindow<T, R, S> Manager;
        private T SelectedDocument = null;

        public FileManagerDocumentWindow( string name, FileManagerWindow<T, R, S> manager ) : base( $"{name} [DOCUMENTS]", false, 600, 400 ) {
            Manager = manager;
        }

        public override void DrawBody() {
            var id = $"##{Name}";
            var footerHeight = ImGui.GetFrameHeightWithSpacing();

            if( ImGui.Button( "+ NEW" + id ) ) Manager.AddDocument();
            ImGui.SameLine();
            ImGui.TextDisabled( "Create documents in order to replace multiple files simultaneously" );

            ImGui.PushStyleVar( ImGuiStyleVar.WindowPadding, new Vector2( 0, 0 ) );
            ImGui.BeginChild( id + "/Child", new Vector2( 0, -footerHeight ), true );

            if( ImGui.BeginTable( $"{id}/Table", 2, ImGuiTableFlags.RowBg ) ) {
                var idx = 0;
                foreach( var document in Manager.Documents ) {
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();

                    var showActive = document == Manager.ActiveDocument;

                    if( showActive ) ImGui.PushStyleColor( ImGuiCol.Text, UiUtils.YELLOW_COLOR );
                    ImGui.SetCursorPosX( ImGui.GetCursorPosX() + 5 );
                    if( ImGui.Selectable( $"{document.SourceDisplay}{id}{idx}", document == SelectedDocument, ImGuiSelectableFlags.SpanAllColumns ) ) {
                        SelectedDocument = document;
                    }
                    if( showActive ) ImGui.PopStyleColor();

                    if( ImGui.IsItemHovered() ) {
                        if( ImGui.IsMouseDoubleClicked( ImGuiMouseButton.Left ) ) Manager.SelectDocument( SelectedDocument );

                        ImGui.BeginTooltip();
                        ImGui.Text( "Replace path: " + document.ReplaceDisplay );
                        ImGui.Text( "Write path: " + document.WritePath );
                        ImGui.EndTooltip();
                    }

                    ImGui.TableNextColumn();

                    if( showActive ) ImGui.PushStyleColor( ImGuiCol.Text, UiUtils.YELLOW_COLOR );
                    ImGui.Text( document.ReplaceDisplay );
                    if( showActive ) ImGui.PopStyleColor();

                    idx++;
                }

                ImGui.EndTable();
            }

            ImGui.EndChild();
            ImGui.PopStyleVar( 1 );

            if( UiUtils.DisabledButton( $"Open{id}", SelectedDocument != null ) ) {
                Manager.SelectDocument( SelectedDocument );
            }
            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 2 );
            if( UiUtils.DisabledRemoveButton( $"Delete{id}", SelectedDocument != null && Manager.Documents.Count > 1 ) ) {
                Manager.RemoveDocument( SelectedDocument );
                SelectedDocument = Manager.ActiveDocument;
            }

            ImGui.SameLine();
            ImGui.TextDisabled( "Double-clicking can also be used to select items" );
        }
    }
}
