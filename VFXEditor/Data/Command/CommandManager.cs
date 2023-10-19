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

        private static int Max => Plugin.Configuration.MaxUndoSize;
        private readonly List<ICommand> CommandBuffer = new();
        private int CommandIndex;

        public readonly CopyManager Copy;
        public readonly FileManagerFile Data;

        private readonly Action OnChangeAction;

        public CommandManager( FileManagerFile data, FileManagerBase manager, Action onChangeAction ) {
            CommandManagers.Add( this );
            Data = data;
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
            Data.SetUnsaved();
            OnChangeAction?.Invoke();
        }

        public bool CanUndo => CommandBuffer.Count > 0 && CommandIndex >= 0;

        public bool CanRedo => CommandBuffer.Count > 0 && CommandIndex < ( CommandBuffer.Count - 1 );

        public void Undo() {
            if( !CanUndo ) return;
            CommandBuffer[CommandIndex].Undo();
            CommandIndex--;
            Data.SetUnsaved();
            OnChangeAction?.Invoke();
        }

        public void Redo() {
            if( !CanRedo ) return;
            CommandIndex++;
            CommandBuffer[CommandIndex].Redo();
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
