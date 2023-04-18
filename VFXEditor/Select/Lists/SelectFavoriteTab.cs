using Dalamud.Interface;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Select.Lists {
    public class SelectFavoriteTab : SelectListTab, IDraggableList<SelectResult> {
        private SelectResult DraggingItem;
        private SelectResult EditingItem;
        private string EditingInput = "";

        public SelectFavoriteTab( SelectDialog dialog, string name, List<SelectResult> items ) : base( dialog, name, items ) { }

        protected override bool DrawRow( SelectResult item, string id, int idx ) {
            if( item == EditingItem ) {
                ImGui.TableNextColumn();
                ImGui.TableNextColumn();

                var checkSize = UiUtils.GetPaddedIconSize( FontAwesomeIcon.Check );
                var removeSize = UiUtils.GetPaddedIconSize( FontAwesomeIcon.Cross );
                ImGui.PushStyleVar( ImGuiStyleVar.ItemSpacing, new Vector2( 3, 4 ) );

                // Input
                var inputSize = ImGui.GetContentRegionAvail().X - checkSize - removeSize - 6;
                ImGui.SetNextItemWidth( inputSize );
                ImGui.InputText( $"{id}/Rename", ref EditingInput, 256 );

                ImGui.PushFont( UiBuilder.IconFont );
                ImGui.SameLine();
                if( ImGui.Button( $"{( char )FontAwesomeIcon.Check}" + id ) ) {
                    EditingItem.DisplayString = EditingInput;
                    Plugin.Configuration.Save();
                    EditingItem = null;
                }
                ImGui.SameLine();
                if( UiUtils.RemoveButton( $"{( char )FontAwesomeIcon.Cross}" + id ) ) {
                    EditingItem = null;
                }
                ImGui.PopFont();

                ImGui.PopStyleVar( 1 );

                return false;
            }
            return base.DrawRow( item, id, idx );
        }

        protected override bool PostRow( SelectResult item, string id, int idx ) {
            if( IDraggableList<SelectResult>.DrawDragDrop( this, item, $"{id}-FAVORITE" ) ) {
                Plugin.Configuration.Save();
                return true;
            }

            if( base.PostRow( item, id, idx ) ) return true;

            var itemId = $"{id}{idx}";
            if( ImGui.IsItemClicked( ImGuiMouseButton.Right ) ) ImGui.OpenPopup( $"{itemId}-context" );

            // Make sure to remove and then restore window padding if necessary
            ImGui.PopStyleVar( 1 );
            if( ImGui.BeginPopup( $"{itemId}-context" ) ) {
                if( ImGui.Selectable( $"Rename{itemId}" ) ) {
                    EditingItem = item;
                    EditingInput = item.DisplayString;
                }
                ImGui.EndPopup();
            }
            ImGui.PushStyleVar( ImGuiStyleVar.WindowPadding, new Vector2( 0, 2 ) );

            return false;
        }

        // For drag+drop

        public SelectResult GetDraggingItem() => DraggingItem;

        public void SetDraggingItem( SelectResult item ) => DraggingItem = item;

        public List<SelectResult> GetItems() => Items;

        public string GetDraggingText( SelectResult item ) => item?.DisplayString ?? string.Empty;
    }
}
