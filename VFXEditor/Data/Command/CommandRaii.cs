using System;

namespace VfxEditor.Data.Command {
    public struct CommandRaii : IDisposable {
        private bool Disposed;

        public CommandRaii( CommandManager command ) {
            Disposed = false;
            CommandManager.Push( command );
        }

        public void Dispose() {
            if( Disposed ) return;
            CommandManager.Pop();
            Disposed = true;
        }
    }
}
