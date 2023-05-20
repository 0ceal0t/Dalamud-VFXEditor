using Dalamud.Interface;
using ImGuiNET;
using OtterGui.Raii;
using System.Numerics;
using VfxEditor.Ui;
using VfxEditor.Utils;

namespace VfxEditor.FileManager {
    public class FileManagerDocumentWindow<T, R, S> : GenericDialog where T : FileManagerDocument<R, S> where R : FileManagerFile {
        private readonly FileManagerWindow<T, R, S> Manager;
        private static bool ShowSourceColumn => Plugin.Configuration.DocumentPopoutShowSource;

        public FileManagerDocumentWindow( string name, FileManagerWindow<T, R, S> manager ) : base( $"{name} [DOCUMENTS]", false, 600, 400 ) {
            Manager = manager;
        }

        public override void DrawBody() {
            using var _ = ImRaii.PushId( "Documents" );

            if( ImGui.Button( "+ NEW" ) ) Manager.AddDocument();

            ImGui.SameLine();
            if( ImGui.Checkbox( "Show source column", ref Plugin.Configuration.DocumentPopoutShowSource ) ) Plugin.Configuration.Save();

            using var style = ImRaii.PushStyle( ImGuiStyleVar.WindowPadding, new Vector2( 0, 0 ) );
            using var child = ImRaii.Child( "Child", new Vector2( -1 ), true );
            using var table = ImRaii.Table( "##Table", ShowSourceColumn ? 3 : 2, ImGuiTableFlags.RowBg );
            style.Pop();

            ImGui.TableSetupColumn( "##Column1", ImGuiTableColumnFlags.WidthStretch );
            if( ShowSourceColumn ) ImGui.TableSetupColumn( "##Column2", ImGuiTableColumnFlags.WidthStretch );
            ImGui.TableSetupColumn( "##Column3", ImGuiTableColumnFlags.WidthFixed, 25 );

            for( var idx = 0; idx < Manager.Documents.Count; idx++ ) {
                var document = Manager.Documents[idx];
                using var __ = ImRaii.PushId( idx );

                ImGui.TableNextRow();
                ImGui.TableNextColumn();

                var active = document == Manager.ActiveDocument;
                using( var color = ImRaii.PushColor( ImGuiCol.Text, UiUtils.YELLOW_COLOR, active ) ) {
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
    }
}
