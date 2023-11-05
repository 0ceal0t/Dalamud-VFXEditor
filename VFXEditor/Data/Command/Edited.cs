using System;
using System.Collections.Generic;

namespace VfxEditor.Data.Command {
    public class Edited : IDisposable {
        private static readonly Stack<Edited> Stack = new();

        public static void SetEdited() {
            foreach( var item in Stack ) item.EditState = true;
        }

        // =================

        protected bool EditState = false;
        private bool Disposed;

        public bool IsEdited => EditState;

        public Edited() {
            Stack.Push( this );
        }

        public void Dispose() {
            if( Disposed ) return;
            Stack.Pop();
            Disposed = true;
        }
    }
}
