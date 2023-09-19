using Dalamud.Interface;
using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Ui.Nodes;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat {
    public interface IUiNodeView<T> : IUiItem where T : AvfxNode {
        public NodeGroup<T> GetGroup();
        public string GetDefaultPath();

        public bool IsAllowedNew();
        public bool IsAllowedDelete();

        public T Read( BinaryReader reader, int size );

        public T GetSelected();
        public void ResetSelected();
        public void SetSelected( T selected );

        public static void DrawControls( IUiNodeView<T> view, AvfxFile file ) {
            var allowNew = view.IsAllowedNew();
            var allowDelete = view.IsAllowedDelete();
            var selected = view.GetSelected();
            var group = view.GetGroup();

            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 4, 4 ) ) ) {
                using var font = ImRaii.PushFont( UiBuilder.IconFont );

                if( allowNew && ImGui.Button( FontAwesomeIcon.Plus.ToIconString() ) ) ImGui.OpenPopup( "NewPopup" );

                if( selected != null && allowDelete ) {
                    if( allowNew ) ImGui.SameLine();
                    if( ImGui.Button( FontAwesomeIcon.Save.ToIconString() ) ) {
                        file.ShowExportDialog( selected );
                    }

                    ImGui.SameLine();
                    if( ImGui.Button( FontAwesomeIcon.BookBookmark.ToIconString() ) ) {
                        file.AddToNodeLibrary( selected );
                    }

                    // Tooltip
                    ImGui.PopFont();
                    UiUtils.Tooltip( "Add to node library" );
                    ImGui.PushFont( UiBuilder.IconFont );

                    ImGui.SameLine();
                    if( UiUtils.RemoveButton( FontAwesomeIcon.Trash.ToIconString() ) ) {
                        CommandManager.Avfx.Add( new UiNodeViewRemoveCommand<T>( view, group, selected ) );
                    }
                }
            }

            // ===== NEW =====

            using var popup = ImRaii.Popup( "NewPopup" );
            if( !popup ) return;

            if( ImGui.Selectable( "Default" ) ) file.Import( view.GetDefaultPath() );

            if( ImGui.Selectable( "Import" ) ) file.ShowImportDialog();

            if( selected != null && ImGui.Selectable( "Duplicate" ) ) {
                using var ms = new MemoryStream();
                using var writer = new BinaryWriter( ms );
                using var reader = new BinaryReader( ms );
                selected.Write( writer );
                reader.BaseStream.Seek( 0, SeekOrigin.Begin );

                var size = ms.Length;
                file.Import( reader, ( int )size, false, string.IsNullOrEmpty( selected.Renamed ) ? null : new List<string>( new[] { selected.Renamed } ) );
            }
        }
    }
}
