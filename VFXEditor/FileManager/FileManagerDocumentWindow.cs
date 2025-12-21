using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Numerics;
using VfxEditor.Ui;
using VfxEditor.Utils;

namespace VfxEditor.FileManager {
    public class FileManagerDocumentWindow<T, R, S> : DalamudWindow where T : FileManagerDocument<R, S> where R : FileManagerFile {
        private T DraggingItem;
        private readonly FileManager<T, R, S> Manager;
        private static bool ShowSourceColumn => Plugin.Configuration.DocumentPopoutShowSource;

        public FileManagerDocumentWindow( string name, FileManager<T, R, S> manager ) : base( $"{name} [DOCUMENTS]", false, new( 600, 400 ), manager.WindowSystem ) {
            Manager = manager;
        }

        public override void DrawBody() {
            using var _ = ImRaii.PushId( "Documents" );

            if( UiUtils.IconButton( FontAwesomeIcon.Plus, "New" ) ) Manager.AddDocument();

            ImGui.SameLine();
            if( ImGui.Checkbox( "Show source column", ref Plugin.Configuration.DocumentPopoutShowSource ) ) Plugin.Configuration.Save();

            using var style = ImRaii.PushStyle( ImGuiStyleVar.WindowPadding, new Vector2( 0, 0 ) );
            using var child = ImRaii.Child( "Child", new Vector2( -1 ), true );
            using var table = ImRaii.Table( "##Table", ShowSourceColumn ? 3 : 2, ImGuiTableFlags.RowBg );
            style.Pop();

            ImGui.TableSetupColumn( "##Column1", ImGuiTableColumnFlags.WidthStretch );
            if( ShowSourceColumn ) ImGui.TableSetupColumn( "##Column2", ImGuiTableColumnFlags.WidthStretch );
            ImGui.TableSetupColumn( "##Column3", ImGuiTableColumnFlags.WidthFixed, 25 );

            foreach( var (document, idx) in Manager.Documents.WithIndex() ) {
                using var __ = ImRaii.PushId( idx );

                ImGui.TableNextRow();
                ImGui.TableNextColumn();

                var active = document == Manager.ActiveDocument;
                using( var color = ImRaii.PushColor( ImGuiCol.Text, UiUtils.DALAMUD_ORANGE, active ) ) {
                    ImGui.SetCursorPosX( ImGui.GetCursorPosX() + 10 );

                    if( ShowSourceColumn ) {
                        ImGui.Text( document.SourceDisplay );
                        ImGui.TableNextColumn();
                    }

                    if( ImGui.Selectable( document.DisplayName, false, ImGuiSelectableFlags.SpanAllColumns ) ) Manager.SelectDocument( document );

                    ImGui.TableNextColumn();
                    if( document.Unsaved ) {
                        var pos = ImGui.GetCursorScreenPos();
                        var drawList = ImGui.GetWindowDrawList();
                        var height = ImGui.GetFrameHeightWithSpacing();

                        drawList.AddCircleFilled( pos + new Vector2( 12.5f, ( height / 2f ) - 4 ), 4, ImGui.GetColorU32( ImGuiCol.Text ) );
                    }
                }

                if( UiUtils.DrawDragDrop( Manager.Documents, document, document.DisplayName, ref DraggingItem, "DOCUMENTS-WINDOW", false ) ) break;

                if( ImGui.IsItemClicked( ImGuiMouseButton.Right ) ) ImGui.OpenPopup( "DocumentPopup" );

                using var popup = ImRaii.Popup( "DocumentPopup" );
                if( popup ) {
                    var deleteDisabled = Manager.Documents.Count < 2;
                    using( var disabled = ImRaii.Disabled( deleteDisabled ) ) {
                        if( UiUtils.IconSelectable( FontAwesomeIcon.Trash, "Delete" ) && !deleteDisabled ) Manager.RemoveDocument( document );
                    }
                    document.DrawRename();
                }
            }
        }

        public void Reset() {
            DraggingItem = null;
        }
    }
}
