using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.Data.Sheets {
    public abstract class SheetLoader<T,S> { // T = Not Selected, S = Selected
        public DataManager Manager;
        public Plugin _plugin;

        public List<T> Items = new List<T>();
        public bool Loaded = false;
        public bool Waiting = false;

        public abstract void OnLoad();
        public abstract bool SelectItem( T item, out S selectedItem );

        public SheetLoader(DataManager manager, Plugin plugin ) {
            Manager = manager;
            _plugin = plugin;
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
