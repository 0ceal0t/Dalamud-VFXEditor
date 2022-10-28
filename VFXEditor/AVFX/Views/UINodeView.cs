using Dalamud.Interface;
using ImGuiNET;
using System.IO;
using VFXEditor.Utils;

namespace VFXEditor.AVFX.VFX {
    public interface IUINodeView<T> where T : UINode {
        public void OnDelete( T item );
        public T OnImport( BinaryReader reader, int size, bool has_dependencies = false );
        public void AddToGroup( T item );
        public void DeleteSelected();
        public void OnNew();

        public void Import( BinaryReader reader, long position, int size, string renamed, bool hasDependencies) {
            reader.BaseStream.Seek( position, SeekOrigin.Begin );
            var newItem = OnImport( reader, size, hasDependencies );
            if( !string.IsNullOrEmpty( renamed ) ) newItem.Renamed = renamed;
            AddToGroup( newItem );
        }

        public static void DrawControls( IUINodeView<T> nodeView, AVFXFile vfxFile, T selected, UINodeGroup<T> group, bool allowNew, bool allowDelete, string Id ) {
            ImGui.PushFont( UiBuilder.IconFont );
            if( allowNew ) {
                if( ImGui.Button( $"{( char )FontAwesomeIcon.Plus}" + Id ) ) {
                    ImGui.OpenPopup( "New_Popup" + Id );
                }
            }
            if( selected != null && allowDelete ) {
                ImGui.SameLine();
                ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 4 );
                if( ImGui.Button( $"{( char )FontAwesomeIcon.Save}" + Id ) ) {
                    vfxFile.ShowExportDialog( selected );
                }

                ImGui.SameLine();
                ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 4 );
                if( ImGui.Button( $"{( char )FontAwesomeIcon.BookMedical}" + Id ) ) {
                    vfxFile.AddToNodeLibrary( selected );
                }
                // Tooltip
                ImGui.PopFont();
                UiUtils.Tooltip( "Add to node library" );
                ImGui.PushFont( UiBuilder.IconFont );

                ImGui.SameLine();
                ImGui.SetCursorPosX( ImGui.GetCursorPosX() + 20 );
                if( UiUtils.RemoveButton( $"{( char )FontAwesomeIcon.Trash}" + Id ) ) {
                    group.Remove( selected );
                    selected.DeleteNode();
                    nodeView.OnDelete( selected );
                    nodeView.DeleteSelected();
                }
            }
            ImGui.PopFont();

            // ===== NEW =====
            if( ImGui.BeginPopup( "New_Popup" + Id ) ) {
                if( ImGui.Selectable( "Default" + Id ) ) {
                    nodeView.OnNew();
                }
                if( ImGui.Selectable( "Import" + Id ) ) {
                    vfxFile.ShowImportDialog();
                }
                if( selected != null && ImGui.Selectable( "Duplicate" + Id ) ) {
                    using var ms = new MemoryStream();
                    using var writer = new BinaryWriter( ms );
                    using var reader = new BinaryReader( ms );

                    selected.Write( writer );
                    reader.BaseStream.Seek( 0, SeekOrigin.Begin );
                    reader.ReadInt32(); // Name
                    var size = reader.ReadInt32();
                    var newNode = nodeView.OnImport( reader, size );
                    newNode.Renamed = selected.Renamed;
                    group.Add( newNode );
                }
                ImGui.EndPopup();
            }
        }
    }
}
