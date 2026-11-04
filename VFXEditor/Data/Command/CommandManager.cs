using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.Data.Command;
using VfxEditor.Data.Copy;
using VfxEditor.FileManager;
using VfxEditor.Utils;
using VfxEditor.Utils.Stacks;

namespace VfxEditor {
    public class CommandManager {
        public static CommandManager? Current => Stack.Count == 0 ? null : Stack.Peek();
        private static readonly Stack<CommandManager> Stack = new();

        public static void Push( CommandManager current ) {
            Stack.Push( current );
        }

        public static void Pop() {
            var item = Stack.Pop();
            if( item != null && CopyManager.IsPasting ) {
                item.AddAndExecute( new CompoundCommand( item.PasteCommands ) ); // Commit the changes
                item.PasteCommands.Clear();
            }
        }

        public static void Add( ICommand command ) => Current?.AddAndExecute( command );

        public static void Undo() => Current?.UndoInternal();

        public static void Redo() => Current?.RedoInternal();

        public static void Paste( ICommand command ) {
            if( Current == null ) return;
            if( !CopyManager.IsPasting ) return;
            Current.PasteCommands.Add( command );
        }

        // ======================

        private readonly UndoRedoStack<ICommand> Commands = new( 25 );

        private readonly FileManagerFile File;

        private readonly List<ICommand> PasteCommands = [];

        public CommandManager( FileManagerFile file ) {
            File = file;
        }

        public void AddAndExecute( ICommand command ) {
            Commands.Add( command );
            File.OnChange();
            Edited.SetEdited();
        }

        protected bool CanUndo => Commands.CanUndo;

        protected bool CanRedo => Commands.CanRedo;

        protected void UndoInternal() {
            if( !Commands.Undo( out var item ) ) return;
            item.Undo();
            File.OnChange();
        }

        protected void RedoInternal() {
            if( !Commands.Redo( out var item ) ) return;
            item.Redo();
            File.OnChange();
        }

        public void DrawButtons() {
            using var font = ImRaii.PushFont( UiBuilder.IconFont );

            using( var spacing = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                ImGui.SameLine();
            }
            using( ImRaii.Disabled( !CanUndo ) ) {
                if( ImGui.Button( FontAwesomeIcon.Undo.ToIconString(), new Vector2( 30, ImGui.GetFrameHeight() ) ) ) UndoInternal();
            }
            UiUtils.Tooltip( "Undo" );

            using( var spacing = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                ImGui.SameLine();
            }
            using( ImRaii.Disabled( !CanRedo ) ) {
                if( ImGui.Button( FontAwesomeIcon.Redo.ToIconString(), new Vector2( 30, ImGui.GetFrameHeight() ) ) ) RedoInternal();
            }
            UiUtils.Tooltip( "Redo" );
        }

        public void Dispose() => Commands.Clear();
    }
}