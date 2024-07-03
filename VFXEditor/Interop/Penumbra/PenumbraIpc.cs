using Newtonsoft.Json;
using Penumbra.Api.Helpers;
using Penumbra.Api.IpcSubscribers;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace VfxEditor.Interop.Penumbra {
    public class PenumbraIpc {
        public bool PenumbraEnabled { get; private set; }

        private readonly ApiVersion ApiVersionsSubscriber;
        private readonly GetModDirectory GetModDirectorySubscriber;
        private readonly GetModList GetModsSubscriber;
        private readonly ResolveDefaultPath ResolveDefaultPathSubscriber;
        private readonly GetPlayerMetaManipulations GetPlayerMetaManipulationsSubscriber;

        private readonly EventSubscriber InitializedSubscriber;
        private readonly EventSubscriber DisposedSubscriber;

        public PenumbraIpc() {
            // TODO

            //ApiVersionsSubscriber = new( Dalamud.PluginInterface );
            //GetModDirectorySubscriber = new( Dalamud.PluginInterface );
            //GetModsSubscriber = new( Dalamud.PluginInterface );
            //GetPlayerMetaManipulationsSubscriber = new( Dalamud.PluginInterface );
            //ResolveDefaultPathSubscriber = new( Dalamud.PluginInterface );

            //InitializedSubscriber = Initialized.Subscriber( Dalamud.PluginInterface, EnablePenumbra );
            //DisposedSubscriber = Disposed.Subscriber( Dalamud.PluginInterface, DisablePenumbra );


            if( !Dalamud.PluginInterface.InstalledPlugins.Where( x => x.InternalName.Equals( "Penumbra" ) ).Any() ) return;
            try {
                ApiVersionsSubscriber.Invoke();
            }
            catch( Exception ) { return; }

            PenumbraEnabled = true;
        }

        public string ResolveDefaultPath( string path ) {
            if( !PenumbraEnabled ) return "";
            try {
                return ResolveDefaultPathSubscriber.Invoke( path );
            }
            catch( Exception ) { return ""; }
        }

        public List<string> GetMods() {
            if( !PenumbraEnabled ) return [];
            try {
                return GetModsSubscriber.Invoke().Select( x => x.Key ).ToList();
            }
            catch( Exception ) { return []; }
        }

        public string GetModDirectory() {
            if( !PenumbraEnabled ) return "";
            try {
                return GetModDirectorySubscriber.Invoke();

            }
            catch( Exception ) { return ""; }
        }

        public MetaManipulation[] GetPlayerMetaManipulations() {
            if( !PenumbraEnabled ) return [];
            try {
                FromCompressedBase64<MetaManipulation[]>( GetPlayerMetaManipulationsSubscriber.Invoke(), out var data );
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
            InitializedSubscriber.Dispose();
            DisposedSubscriber.Dispose();
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
