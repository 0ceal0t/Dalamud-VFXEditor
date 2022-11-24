using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Dialogs;
using VfxEditor.Utils;

namespace VfxEditor.FileManager {
    public class FileManagerDocumentWindow<T, S, R> : GenericDialog where T : FileManagerDocument<R, S> where R : FileManagerFile {
        private FileManagerWindow<T, S, R> Manager;
        private T SelectedDocument = null;

        public FileManagerDocumentWindow( string name, FileManagerWindow<T, S, R> manager ) : base( $"{name} [DOCUMENTS]", false, 600, 400 ) {
            Manager = manager;
        }

        public override void DrawBody() {
            var id = $"##{Name}";
            var footerHeight = ImGui.GetFrameHeightWithSpacing();

            if( ImGui.Button( "+ NEW" + id ) ) Manager.AddDocument();
            ImGui.SameLine();
            ImGui.Text( "Create documents in order to replace multiple files simultaneously" );

            ImGui.BeginChild( id + "/Child", new Vector2( 0, -footerHeight ), true );
            ImGui.Columns( 2, id + "/Columns", false );

            var idx = 0;
            foreach( var document in Manager.Documents ) {
                if( ImGui.Selectable( document.SourceDisplay + id + idx, document == SelectedDocument, ImGuiSelectableFlags.SpanAllColumns ) ) {
                    SelectedDocument = document;
                }
                if( ImGui.IsItemHovered() ) {
                    ImGui.BeginTooltip();
                    ImGui.Text( "Replace path: " + document.ReplaceDisplay );
                    ImGui.Text( "Write path: " + document.WritePath );
                    ImGui.EndTooltip();
                }
                idx++;
            }
            ImGui.NextColumn();

            foreach( var document in Manager.Documents ) {
                ImGui.Text( document.ReplaceDisplay );
            }

            ImGui.Columns( 1 );
            ImGui.EndChild();

            if( UiUtils.DisabledButton( $"Open{id}", SelectedDocument != null ) ) {
                Manager.SelectDocument( SelectedDocument );
            }
            ImGui.SameLine();
            if( UiUtils.DisabledRemoveButton( $"Delete{id}", SelectedDocument != null && Manager.Documents.Count > 1 ) ) {
                Manager.RemoveDocument( SelectedDocument );
                SelectedDocument = Manager.ActiveDocument;
            }
        }
    }
}
