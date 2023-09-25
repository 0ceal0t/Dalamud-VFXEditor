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
        private readonly bool DeleteButtonFirst;

        public ListView( List<T> items, Func<T> newAction, Func<CommandManager> commandAction, bool deleteButtonFirst = false ) {
            Items = items;
            NewAction = newAction;
            CommandAction = commandAction;
            DeleteButtonFirst = deleteButtonFirst;
        }

        public void Draw() {
            var commandManager = CommandAction.Invoke();

            for( var idx = 0; idx < Items.Count; idx++ ) {
                var item = Items[idx];
                using var _ = ImRaii.PushId( idx );

                if( DeleteButtonFirst ) {
                    using var font = ImRaii.PushFont( UiBuilder.IconFont );
                    if( UiUtils.RemoveButton( FontAwesomeIcon.Trash.ToIconString() ) ) {
                        commandManager.Add( new GenericRemoveCommand<T>( Items, item ) );
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
                        commandManager.Add( new GenericRemoveCommand<T>( Items, item ) );
                        break;
                    }
                }
            }

            if( UiUtils.IconButton( FontAwesomeIcon.Plus, "New" ) ) {
                commandManager.Add( new GenericAddCommand<T>( Items, NewAction.Invoke() ) );
            }
        }
    }
}
