using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.Data.Command;
using VfxEditor.Data.Copy;
using VfxEditor.FileManager;
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

        public static void Draw() {
            if( Current == null ) {
                using var disabled = ImRaii.Disabled();
                ImGui.MenuItem( "Undo" );
                ImGui.MenuItem( "Redo" );
                return;
            }

            Current.DrawMenu();
        }

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

        protected unsafe void DrawMenu() {
            using( var dimUndo = ImRaii.PushColor( ImGuiCol.Text, *ImGui.GetStyleColorVec4( ImGuiCol.TextDisabled ), !CanUndo ) ) {
                if( ImGui.MenuItem( "Undo" ) ) UndoInternal();
            }

            using var dimRedo = ImRaii.PushColor( ImGuiCol.Text, *ImGui.GetStyleColorVec4( ImGuiCol.TextDisabled ), !CanRedo );
            if( ImGui.MenuItem( "Redo" ) ) RedoInternal();
        }

        public void Dispose() => Commands.Clear();
    }
}
