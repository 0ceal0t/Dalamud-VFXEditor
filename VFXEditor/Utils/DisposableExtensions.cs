using System;
using System.Collections.Generic;

namespace VfxEditor.Utils {
    public static class DisposableExtensions {
        // Disposes every item, logging (rather than throwing on) failures so one bad item doesn't stop the rest
        public static void DisposeAll<T>( this ICollection<T> items, bool clear = true ) where T : IDisposable {
            foreach( var item in items ) {
                try {
                    item?.Dispose();
                }
                catch( Exception e ) {
                    Dalamud.Error( e, $"Failed to dispose {typeof( T ).Name}" );
                }
            }
            if( clear ) items.Clear();
        }
    }
}