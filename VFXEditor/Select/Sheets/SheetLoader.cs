using Dalamud.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VfxEditor.Select.Sheets {
    public abstract class SheetLoader<T, S> { // T = Not Selected, S = Selected
        public List<T> Items = new();
        public bool Loaded = false;
        public bool Waiting = false;

        public abstract void OnLoad();
        public abstract bool SelectItem( T item, out S selectedItem );

        public async void Load() {
            if( Waiting ) return;
            Waiting = true;
            PluginLog.Log( "Loading " + typeof( T ).Name );
            await Task.Run( () => {
                try {
                    OnLoad();
                }
                catch( Exception e ) {
                    PluginLog.Error( e, "Error Loading: " + typeof( T ).Name);
                }
                Loaded = true;
            } );
        }
    }
}
