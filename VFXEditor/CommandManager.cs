using System;
using System.Collections.Generic;

namespace VfxEditor {
    public class CommandManager {
        public static CommandManager Avfx => Plugin.AvfxManager.CurrentFile?.Command;
        public static CommandManager Tmb => Plugin.TmbManager.CurrentFile?.Command;
        public static CommandManager Pap => Plugin.PapManager.CurrentFile?.Command;

        private readonly List<ICommand> CommandBuffer = new();
        private int CommandIndex;
        private static readonly int MAX = 10;

        public void Add(ICommand command) {
            CommandBuffer.Add(command);
            while( CommandBuffer.Count > MAX ) CommandBuffer.RemoveAt( 0 );
            CommandIndex = CommandBuffer.Count - 1;
            command.Execute();
        }

        public bool CanUndo => CommandBuffer.Count > 0 && CommandIndex >= 0;

        public bool CanRedo => CommandBuffer.Count > 0 && CommandIndex < ( CommandBuffer.Count - 1 );

        /* 
         * 0 1 [2]
         *   undo 
         * 0 [1] 2
         *  undo 
         * [0] 1 2
         *  undo
         * 0 1 2 (-1)
         *  redo
         * [0] 1 2
         */

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
    }
}
