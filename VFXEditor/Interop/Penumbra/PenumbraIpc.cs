using Dalamud.Plugin.Ipc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace VfxEditor.Interop.Penumbra {
    public class PenumbraIpc : IDisposable {
        public bool PenumbraEnabled { get; private set; }

        private readonly ICallGateSubscriber<(int Breaking, int Features)> ApiVersionsSubscriber;
        private readonly ICallGateSubscriber<string> GetModDirectorySubscriber;
        private readonly ICallGateSubscriber<IList<(string, string)>> GetModsSubscriber;
        private readonly ICallGateSubscriber<string, string> ResolveDefaultPathSubscriber;

        private readonly ICallGateSubscriber<string> GetPlayerMetaManipulationsSubscriber;

        private readonly ICallGateSubscriber<object> InitializedSubscriber;
        private readonly ICallGateSubscriber<object> DisposedSubscriber;

        public PenumbraIpc() {
            ApiVersionsSubscriber = Dalamud.PluginInterface.GetIpcSubscriber<(int, int)>( "Penumbra.ApiVersions" );
            InitializedSubscriber = Dalamud.PluginInterface.GetIpcSubscriber<object>( "Penumbra.Initialized" );
            DisposedSubscriber = Dalamud.PluginInterface.GetIpcSubscriber<object>( "Penumbra.Disposed" );

            GetModDirectorySubscriber = Dalamud.PluginInterface.GetIpcSubscriber<string>( "Penumbra.GetModDirectory" );
            GetModsSubscriber = Dalamud.PluginInterface.GetIpcSubscriber<IList<(string, string)>>( "Penumbra.GetMods" );

            GetPlayerMetaManipulationsSubscriber = Dalamud.PluginInterface.GetIpcSubscriber<string>( "Penumbra.GetPlayerMetaManipulations" );

            ResolveDefaultPathSubscriber = Dalamud.PluginInterface.GetIpcSubscriber<string, string>( "Penumbra.ResolveDefaultPath" );

            InitializedSubscriber.Subscribe( EnablePenumbra );
            DisposedSubscriber.Subscribe( DisablePenumbra );

            if( !Dalamud.PluginInterface.InstalledPlugins.Where( x => x.InternalName.Equals( "Penumbra" ) ).Any() ) return;
            try {
                ApiVersionsSubscriber.InvokeFunc();
            }
            catch( Exception ) { return; }

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
            if( !PenumbraEnabled ) return [];
            try {
                return GetModsSubscriber.InvokeFunc().Select( x => x.Item1 ).ToList();
            }
            catch( Exception ) { return []; }
        }

        public string GetModDirectory() {
            if( !PenumbraEnabled ) return "";
            try {
                return GetModDirectorySubscriber.InvokeFunc();

            }
            catch( Exception ) { return ""; }
        }

        public MetaManipulation[] GetPlayerMetaManipulations() {
            if( !PenumbraEnabled ) return [];
            try {
                FromCompressedBase64<MetaManipulation[]>( GetPlayerMetaManipulationsSubscriber.InvokeFunc(), out var data );
                return data ?? [];
            }
            catch( Exception ) { return []; }
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

        // https://github.com/Ottermandias/OtterGui/blob/4673e93f5165108a7f5b91236406d527f16384a5/Functions.cs#L154
#nullable enable
        public static byte FromCompressedBase64<T>( string base64, out T? data ) {
            var version = byte.MaxValue;
            try {
                var bytes = Convert.FromBase64String( base64 );
                using var compressedStream = new MemoryStream( bytes );
                using var zipStream = new GZipStream( compressedStream, CompressionMode.Decompress );
                using var resultStream = new MemoryStream();
                zipStream.CopyTo( resultStream );
                bytes = resultStream.ToArray();
                version = bytes[0];
                var json = Encoding.UTF8.GetString( bytes, 1, bytes.Length - 1 );
                data = JsonConvert.DeserializeObject<T>( json );
            }
            catch {
                data = default;
            }

            return version;
        }
#nullable disable
    }
}
