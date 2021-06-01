using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXSelect.Data.Rows;

namespace VFXSelect.Data.Sheets {
    public abstract class SheetLoader<T,S> { // T = Not Selected, S = Selected
        public SheetManager Manager;
        public DalamudPluginInterface PluginInterface;

        public List<T> Items = new List<T>();
        public bool Loaded = false;
        public bool Waiting = false;

        public abstract void OnLoad();
        public abstract bool SelectItem( T item, out S selectedItem );

        public SheetLoader( SheetManager manager, DalamudPluginInterface pluginInterface ) {
            Manager = manager;
            PluginInterface = pluginInterface;
        }

        public void Load() {
            if( Waiting ) return;
            Waiting = true;
            PluginLog.Log( "Loading " + typeof(T).Name );
            Task.Run( async () => {
                try {
                    OnLoad();
                }
                catch(Exception e ) {
                    PluginLog.LogError( "Error Loading " + typeof( T ).Name, e );
                }
                Loaded = true;
            } );
        }
    }
}
