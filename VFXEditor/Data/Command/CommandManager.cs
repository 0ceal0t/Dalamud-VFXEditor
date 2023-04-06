using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Data;

namespace VfxEditor {
    public class CommandManager {
        public static readonly List<CommandManager> Managers = new();
        public static readonly List<string> FilesToCleanup = new();

        public static CommandManager Avfx => Plugin.AvfxManager?.GetCommandManager();
        public static CommandManager Tmb => Plugin.TmbManager?.GetCommandManager();
        public static CommandManager Pap => Plugin.PapManager?.GetCommandManager();
        public static CommandManager Scd => Plugin.ScdManager?.GetCommandManager();
        public static CommandManager Eid => Plugin.EidManager?.GetCommandManager();
        public static CommandManager Uld => Plugin.UldManager?.GetCommandManager();

        public static int Max => Plugin.Configuration.MaxUndoSize;
        private readonly List<ICommand> CommandBuffer = new();
        private int CommandIndex;

        public readonly CopyManager Copy;

        public CommandManager( CopyManager copy ) {
            Managers.Add( this );
            Copy = copy;
        }

        public void Add( ICommand command ) {
            var numberToRemove = CommandBuffer.Count - 1 - CommandIndex; // when a change is made, wipes out the previous undo
            if( numberToRemove > 0 ) CommandBuffer.RemoveRange( CommandBuffer.Count - numberToRemove, numberToRemove );

            CommandBuffer.Add( command );
            while( CommandBuffer.Count > Max ) CommandBuffer.RemoveAt( 0 );
            CommandIndex = CommandBuffer.Count - 1;
            command.Execute();
        }

        public bool CanUndo => CommandBuffer.Count > 0 && CommandIndex >= 0;

        public bool CanRedo => CommandBuffer.Count > 0 && CommandIndex < ( CommandBuffer.Count - 1 );

        public void Undo() {
            if( !CanUndo ) return;
            CommandBuffer[CommandIndex].Undo();
            CommandIndex--;
        }

        public void Redo() {
            if( !CanRedo ) return;
            CommandIndex++;
            CommandBuffer[CommandIndex].Redo();
        }

        public unsafe void Draw() {
            var dimUndo = !CanUndo;
            var dimRedo = !CanRedo;

            if( dimUndo ) ImGui.PushStyleColor( ImGuiCol.Text, *ImGui.GetStyleColorVec4( ImGuiCol.TextDisabled ) );
            if( ImGui.MenuItem( "Undo##Menu" ) ) Undo();
            if( dimUndo ) ImGui.PopStyleColor();

            if( dimRedo ) ImGui.PushStyleColor( ImGuiCol.Text, *ImGui.GetStyleColorVec4( ImGuiCol.TextDisabled ) );
            if( ImGui.MenuItem( "Redo##Menu" ) ) Redo();
            if( dimRedo ) ImGui.PopStyleColor();
        }

        public void Dispose() {
            CommandBuffer.Clear();
        }

        public static void DisposeAll() {
            Managers.ForEach( x => x.Dispose() );
            foreach( var file in FilesToCleanup ) {
                if( File.Exists( file ) ) File.Delete( file );
            }
            FilesToCleanup.Clear();
        }
    }
}
