using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
using VfxEditor.Data.Command.ListCommands;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Ui.Components {
    public class CommandListView<T> where T : class, IUiItem {
        private readonly List<T> Items;
        private readonly Func<T> NewAction;
        private readonly bool DeleteButtonFirst;

        public CommandListView( List<T> items, Func<T> newAction, bool deleteButtonFirst = false ) {
            Items = items;
            NewAction = newAction;
            DeleteButtonFirst = deleteButtonFirst;
        }

        public void Draw() {
            foreach( var (item, idx) in Items.WithIndex() ) {
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

                ImGui.SetNextItemWidth( UiUtils.GetOffsetInputSize( FontAwesomeIcon.Trash ) );
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
