using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Data;
using VfxEditor.FileManager;

namespace VfxEditor {
    public class CommandManager {
        public static readonly List<CommandManager> CommandManagers = new();
        public static readonly List<string> FilesToCleanup = new();

        public static CommandManager Avfx => Plugin.AvfxManager?.GetCommandManager();
        public static CommandManager Pap => Plugin.PapManager?.GetCommandManager();
        public static CommandManager Scd => Plugin.ScdManager?.GetCommandManager();
        public static CommandManager Eid => Plugin.EidManager?.GetCommandManager();
        public static CommandManager Uld => Plugin.UldManager?.GetCommandManager();
        public static CommandManager Phyb => Plugin.PhybManager?.GetCommandManager();
        public static CommandManager Sklb => Plugin.SklbManager?.GetCommandManager();
        public static CommandManager Atch => Plugin.AtchManager?.GetCommandManager();
        public static CommandManager Skp => Plugin.SkpManager?.GetCommandManager();
        public static CommandManager Shpk => Plugin.ShpkManager?.GetCommandManager();

        public static int Max => Plugin.Configuration.MaxUndoSize;
        private readonly List<ICommand> CommandBuffer = new();
        private int CommandIndex;

        public readonly CopyManager Copy;
        public readonly FileManagerBase Manager;

        private readonly Action OnChangeAction;

        public CommandManager( FileManagerBase manager, Action onChangeAction = null ) {
            CommandManagers.Add( this );
            Manager = manager;
            Copy = manager.GetCopyManager();
            OnChangeAction = onChangeAction;
        }

        public void Add( ICommand command ) {
            var numberToRemove = CommandBuffer.Count - 1 - CommandIndex; // when a change is made, wipes out the previous undo
            if( numberToRemove > 0 ) CommandBuffer.RemoveRange( CommandBuffer.Count - numberToRemove, numberToRemove );

            CommandBuffer.Add( command );
            while( CommandBuffer.Count > Max ) CommandBuffer.RemoveAt( 0 );
            CommandIndex = CommandBuffer.Count - 1;
            command.Execute();
            Manager.Unsaved();
            OnChangeAction?.Invoke();
        }

        public bool CanUndo => CommandBuffer.Count > 0 && CommandIndex >= 0;

        public bool CanRedo => CommandBuffer.Count > 0 && CommandIndex < ( CommandBuffer.Count - 1 );

        public void Undo() {
            if( !CanUndo ) return;
            CommandBuffer[CommandIndex].Undo();
            CommandIndex--;
            Manager.Unsaved();
            OnChangeAction?.Invoke();
        }

        public void Redo() {
            if( !CanRedo ) return;
            CommandIndex++;
            CommandBuffer[CommandIndex].Redo();
            Manager.Unsaved();
            OnChangeAction?.Invoke();
        }

        public unsafe void Draw() {
            using( var dimUndo = ImRaii.PushColor( ImGuiCol.Text, *ImGui.GetStyleColorVec4( ImGuiCol.TextDisabled ), !CanUndo ) ) {
                if( ImGui.MenuItem( "Undo##Menu" ) ) Undo();
            }

            using var dimRedo = ImRaii.PushColor( ImGuiCol.Text, *ImGui.GetStyleColorVec4( ImGuiCol.TextDisabled ), !CanRedo );
            if( ImGui.MenuItem( "Redo##Menu" ) ) Redo();
        }

        public void Dispose() {
            CommandBuffer.Clear();
        }

        public static void DisposeAll() {
            CommandManagers.ForEach( x => x.Dispose() );
            foreach( var file in FilesToCleanup ) {
                if( File.Exists( file ) ) File.Delete( file );
            }
            FilesToCleanup.Clear();
        }
    }
}
