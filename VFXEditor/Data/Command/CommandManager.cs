using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using VfxEditor.Data;
using VfxEditor.FileManager;
using VfxEditor.Utils.Stacks;

namespace VfxEditor {
    public class CommandManager {
        public static readonly List<CommandManager> CommandManagers = new();

        public static CommandManager Avfx => Plugin.AvfxManager?.GetCurrentCommandManager();
        public static CommandManager Pap => Plugin.PapManager?.GetCurrentCommandManager();
        public static CommandManager Scd => Plugin.ScdManager?.GetCurrentCommandManager();
        public static CommandManager Eid => Plugin.EidManager?.GetCurrentCommandManager();
        public static CommandManager Uld => Plugin.UldManager?.GetCurrentCommandManager();
        public static CommandManager Phyb => Plugin.PhybManager?.GetCurrentCommandManager();
        public static CommandManager Sklb => Plugin.SklbManager?.GetCurrentCommandManager();
        public static CommandManager Atch => Plugin.AtchManager?.GetCurrentCommandManager();
        public static CommandManager Skp => Plugin.SkpManager?.GetCurrentCommandManager();
        public static CommandManager Shpk => Plugin.ShpkManager?.GetCurrentCommandManager();
        public static CommandManager Shcd => Plugin.ShcdManager?.GetCurrentCommandManager();

        private readonly UndoRedoStack<ICommand> Commands = new( 25 );

        public readonly CopyManager Copy;
        private readonly FileManagerFile Data;
        private readonly Action OnChangeAction;

        public CommandManager( FileManagerFile data, FileManagerBase manager, Action onChangeAction ) {
            CommandManagers.Add( this );
            Data = data;
            Copy = manager.GetCopyManager();
            OnChangeAction = onChangeAction;
        }

        public void Add( ICommand command ) {
            command.Execute();
            Commands.Add( command );

            Data.SetUnsaved();
            OnChangeAction?.Invoke();
        }

        public bool CanUndo => Commands.CanUndo;

        public bool CanRedo => Commands.CanRedo;

        public void Undo() {
            if( !Commands.Undo( out var item ) ) return;
            item.Undo();

            Data.SetUnsaved();
            OnChangeAction?.Invoke();
        }

        public void Redo() {
            if( !Commands.Redo( out var item ) ) return;
            item.Redo();

            Data.SetUnsaved();
            OnChangeAction?.Invoke();
        }

        public unsafe void Draw() {
            using( var dimUndo = ImRaii.PushColor( ImGuiCol.Text, *ImGui.GetStyleColorVec4( ImGuiCol.TextDisabled ), !CanUndo ) ) {
                if( ImGui.MenuItem( "Undo" ) ) Undo();
            }

            using var dimRedo = ImRaii.PushColor( ImGuiCol.Text, *ImGui.GetStyleColorVec4( ImGuiCol.TextDisabled ), !CanRedo );
            if( ImGui.MenuItem( "Redo" ) ) Redo();
        }

        public static void DrawDisabled() {
            ImGui.MenuItem( "Undo" );
            ImGui.MenuItem( "Redo" );
        }

        public void Dispose() {
            Commands.Clear();
        }

        public static void DisposeAll() {
            CommandManagers.ForEach( x => x.Dispose() );
        }
    }
}
