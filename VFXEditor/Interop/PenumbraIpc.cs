using Dalamud.Plugin.Ipc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Interop {
    public class PenumbraIpc : IDisposable {
        public bool PenumbraEnabled { get; private set; }

        private readonly ICallGateSubscriber<(int Breaking, int Features)> ApiVersionsSubscriber;
        private readonly ICallGateSubscriber<string> GetModDirectorySubscriber;
        private readonly ICallGateSubscriber<IList<(string, string)>> GetModsSubscriber;
        private readonly ICallGateSubscriber<string, string> ResolveDefaultPathSubscriber;

        private readonly ICallGateSubscriber<object> InitializedSubscriber;
        private readonly ICallGateSubscriber<object> DisposedSubscriber;

        public PenumbraIpc() {
            ApiVersionsSubscriber = Dalamud.PluginInterface.GetIpcSubscriber<(int, int)>( "Penumbra.ApiVersions" );
            InitializedSubscriber = Dalamud.PluginInterface.GetIpcSubscriber<object>( "Penumbra.Initialized" );
            DisposedSubscriber = Dalamud.PluginInterface.GetIpcSubscriber<object>( "Penumbra.Disposed" );

            GetModDirectorySubscriber = Dalamud.PluginInterface.GetIpcSubscriber<string>( "Penumbra.GetModDirectory" );
            GetModsSubscriber = Dalamud.PluginInterface.GetIpcSubscriber<IList<(string, string)>>( "Penumbra.GetMods" );

            ResolveDefaultPathSubscriber = Dalamud.PluginInterface.GetIpcSubscriber<string, string>( "Penumbra.ResolveDefaultPath" );

            InitializedSubscriber.Subscribe( EnablePenumbra );
            DisposedSubscriber.Subscribe( DisablePenumbra );

            if( !Dalamud.PluginInterface.InstalledPlugins.Where( x => x.InternalName.Equals( "Penumbra" ) ).Any() ) return;
            try {
                ApiVersionsSubscriber.InvokeFunc();
            }
            catch( Exception ) {
                return;
            }

            PenumbraEnabled = true;
        }

        public string ResolveDefaultPath( string path ) {
            if( !PenumbraEnabled ) return "";

            try {
                return ResolveDefaultPathSubscriber.InvokeFunc( path );
            }
            catch( Exception ) { return ""; }
        }

        public List<string> GetMods() {
            var ret = new List<string>();
            if( !PenumbraEnabled ) return ret;

            try {
                var mods = GetModsSubscriber.InvokeFunc();
                foreach( var m in mods ) {
                    var (modPath, modName) = m;
                    ret.Add( modPath );
                }
            }
            catch( Exception ) { }
            return ret;
        }

        public string GetModDirectory() {
            if( !PenumbraEnabled ) return "";

            try {
                return GetModDirectorySubscriber.InvokeFunc();

            }
            catch( Exception ) {
                return "";
            }
        }

        public bool PenumbraFileExists( string path, out string localPath ) {
            if( !PenumbraEnabled ) {
                localPath = path;
                return false;
            }

            localPath = ResolveDefaultPath( path );
            if( path.Equals( localPath ) ) return false;
            if( !string.IsNullOrEmpty( localPath ) ) return true;
            return false;
        }

        private void EnablePenumbra() {
            PenumbraEnabled = true;
        }

        private void DisablePenumbra() {
            PenumbraEnabled = false;
        }

        public void Dispose() {
            InitializedSubscriber.Unsubscribe( EnablePenumbra );
            DisposedSubscriber.Unsubscribe( DisablePenumbra );
        }
    }
}
