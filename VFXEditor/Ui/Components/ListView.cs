using Dalamud.Interface;
using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Ui.Components {
    public class ListView<T> where T : class, IUiItem {
        private readonly List<T> Items;
        private readonly Func<T> NewAction;
        private readonly Func<CommandManager> CommandAction;

        public ListView( List<T> items, Func<T> newAction, Func<CommandManager> commandAction ) {
            Items = items;
            NewAction = newAction;
            CommandAction = commandAction;
        }

        public void Draw() {
            var commandManager = CommandAction.Invoke();

            for( var idx = 0; idx < Items.Count; idx++ ) {
                var item = Items[idx];
                using var _ = ImRaii.PushId( idx );

                item.Draw();

                using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );
                using var font = ImRaii.PushFont( UiBuilder.IconFont );
                ImGui.SameLine();
                if( UiUtils.RemoveButton( FontAwesomeIcon.Trash.ToIconString() ) ) {
                    commandManager.Add( new GenericRemoveCommand<T>( Items, item ) );
                    break;
                }
            }

            if( UiUtils.IconButton( FontAwesomeIcon.Plus, "New" ) ) {
                commandManager.Add( new GenericAddCommand<T>( Items, NewAction.Invoke() ) );
            }
        }
    }
}
