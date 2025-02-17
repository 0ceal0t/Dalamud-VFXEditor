using System;
using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;
using Dalamud.Interface;
using ImGuiNET;

namespace VfxEditor.Ui.Components {
    public class CommandSaveDropdown<T> : CommandDropdown<T> where T : class, IUiItem {

        protected readonly Action<T> OnSave;

        public CommandSaveDropdown(
            string id,
            List<T> items,
            Func<T, int, string> getTextAction,
            Func<T> newAction,
            Action<T> onSave,
            Action<T, bool> onChangeAction = null
        ) : base( id, items, getTextAction, newAction, onChangeAction ) {
            OnSave = onSave;
        }

        // doesn't need a CommandManager add for OnSaveAction as we won't need to undo/redo the save
        protected override void DrawControls() {
            if( NewAction == null ) return;
            DrawNewDeleteControls( OnNew, OnDelete, OnSave );
        }

        protected void DrawNewDeleteControls( Action onNew, Action<T> onDelete, Action<T> onSave ) {
            if( onNew != null ) {
                using var font = ImRaii.PushFont( UiBuilder.IconFont );
                ImGui.SameLine();
                if( ImGui.Button( FontAwesomeIcon.Plus.ToIconString() ) ) onNew();
            }

            // draw a save button and run onSave() for the selected item when clicked
            if( onSave != null) {
                using var font = ImRaii.PushFont( UiBuilder.IconFont );
                ImGui.SameLine();
                if( ImGui.Button( FontAwesomeIcon.Save.ToIconString() ) && Items.Contains( Selected ) ) {
                    onSave( Selected );
                }
            }

            using var disabled = ImRaii.Disabled( Selected == null );

            if( onDelete != null ) {
                using var font = ImRaii.PushFont( UiBuilder.IconFont );
                ImGui.SameLine();
                ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 4 );
                if( UiUtils.RemoveButton( FontAwesomeIcon.Trash.ToIconString() ) && Items.Contains( Selected ) ) {
                    onDelete( Selected );
                    Selected = null;
                }
            }
        }
    }
}
