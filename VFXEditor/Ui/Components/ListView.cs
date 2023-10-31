using Dalamud.Interface;
using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using VfxEditor.Data.Command.ListCommands;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Ui.Components {
    public class ListView<T> where T : class, IUiItem {
        private readonly List<T> Items;
        private readonly Func<T> NewAction;
        private readonly bool DeleteButtonFirst;

        public ListView( List<T> items, Func<T> newAction, bool deleteButtonFirst = false ) {
            Items = items;
            NewAction = newAction;
            DeleteButtonFirst = deleteButtonFirst;
        }

        public void Draw() {
            for( var idx = 0; idx < Items.Count; idx++ ) {
                var item = Items[idx];
                using var _ = ImRaii.PushId( idx );

                if( DeleteButtonFirst ) {
                    using var font = ImRaii.PushFont( UiBuilder.IconFont );
                    if( UiUtils.RemoveButton( FontAwesomeIcon.Trash.ToIconString() ) ) {
                        CommandManager.Add( new ListRemoveCommand<T>( Items, item ) );
                        break;
                    }

                    using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );
                    ImGui.SameLine();
                }

                item.Draw();

                if( !DeleteButtonFirst ) {
                    using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );
                    ImGui.SameLine();

                    using var font = ImRaii.PushFont( UiBuilder.IconFont );
                    if( UiUtils.RemoveButton( FontAwesomeIcon.Trash.ToIconString() ) ) {
                        CommandManager.Add( new ListRemoveCommand<T>( Items, item ) );
                        break;
                    }
                }
            }

            if( UiUtils.IconButton( FontAwesomeIcon.Plus, "New" ) ) {
                CommandManager.Add( new ListAddCommand<T>( Items, NewAction.Invoke() ) );
            }
        }
    }
}
