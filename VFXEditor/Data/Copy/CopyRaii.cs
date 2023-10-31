using System;

namespace VfxEditor.Data.Copy {
    public struct CopyRaii : IDisposable {
        private bool Disposed;

        public CopyRaii( CopyManager command ) {
            Disposed = false;
            CopyManager.Push( command );
        }

        public void Dispose() {
            if( Disposed ) return;
            CopyManager.Pop();
            Disposed = true;
        }
    }
}
