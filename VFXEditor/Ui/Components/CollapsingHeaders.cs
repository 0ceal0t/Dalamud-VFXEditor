using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Ui.Components {
    public class CollapsingHeaders<T> where T : class, IUiItem {
        private readonly List<T> Items;

        private readonly string Id;
        private readonly Func<T, int, string> GetTextAction;
        private readonly Func<T> NewAction;
        private readonly Func<CommandManager> CommandAction;

        public CollapsingHeaders( string id, List<T> items, Func<T, int, string> getTextAction, Func<T> newAction, Func<CommandManager> commandAction ) {
            Id = id;
            Items = items;
            GetTextAction = getTextAction;
            NewAction = newAction;
            CommandAction = commandAction;
        }

        public void Draw() {
            var commandManager = CommandAction.Invoke();

            for( var idx = 0; idx < Items.Count; idx++ ) {
                var item = Items[idx];
                var text = GetTextAction == null ? $"{Id} {idx}" : GetTextAction.Invoke( item, idx );
                if( ImGui.CollapsingHeader( $"{text}###{idx}" ) ) {
                    using var _ = ImRaii.PushId( idx );
                    using var indent = ImRaii.PushIndent();

                    if( UiUtils.RemoveButton( "Delete", true ) ) { // REMOVE
                        commandManager.Add( new GenericRemoveCommand<T>( Items, item ) );
                        break;
                    }

                    item.Draw();
                }
            }

            if( ImGui.Button( "+ New" ) ) { // NEW
                commandManager.Add( new GenericAddCommand<T>( Items, NewAction.Invoke() ) );
            }
        }
    }
}
