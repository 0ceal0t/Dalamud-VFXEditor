using Newtonsoft.Json;
using Penumbra.Api.Enums;
using Penumbra.Api.Helpers;
using Penumbra.Api.IpcSubscribers;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace VfxEditor.Interop.Penumbra {
    // Penumbra is required: all in-game previews go through it (see PenumbraPreview)
    public class PenumbraIpc {
        public bool PenumbraEnabled { get; private set; }
        public bool ModsEnabled { get; private set; } = true; // Penumbra's global "Enable Mods" toggle

        private readonly ApiVersion ApiVersionsSubscriber;
        private readonly GetModDirectory GetModDirectorySubscriber;
        private readonly GetModList GetModsSubscriber;
        private readonly ResolveDefaultPath ResolveDefaultPathSubscriber;
        private readonly GetPlayerMetaManipulations GetPlayerMetaManipulationsSubscriber;
        private readonly AddTemporaryModAll AddTemporaryModAllSubscriber;
        private readonly RemoveTemporaryModAll RemoveTemporaryModAllSubscriber;
        private readonly RedrawObject RedrawObjectSubscriber;
        private readonly GetEnabledState GetEnabledStateSubscriber;

        private readonly EventSubscriber InitializedSubscriber;
        private readonly EventSubscriber DisposedSubscriber;
        private readonly EventSubscriber<bool> EnabledChangeSubscriber;

        public PenumbraIpc() {
            ApiVersionsSubscriber = new( Dalamud.PluginInterface );
            GetModDirectorySubscriber = new( Dalamud.PluginInterface );
            GetModsSubscriber = new( Dalamud.PluginInterface );
            GetPlayerMetaManipulationsSubscriber = new( Dalamud.PluginInterface );
            ResolveDefaultPathSubscriber = new( Dalamud.PluginInterface );
            AddTemporaryModAllSubscriber = new( Dalamud.PluginInterface );
            RemoveTemporaryModAllSubscriber = new( Dalamud.PluginInterface );
            RedrawObjectSubscriber = new( Dalamud.PluginInterface );
            GetEnabledStateSubscriber = new( Dalamud.PluginInterface );

            InitializedSubscriber = Initialized.Subscriber( Dalamud.PluginInterface, EnablePenumbra );
            DisposedSubscriber = Disposed.Subscriber( Dalamud.PluginInterface, DisablePenumbra );
            EnabledChangeSubscriber = EnabledChange.Subscriber( Dalamud.PluginInterface, OnEnabledChange );

            if( !Dalamud.PluginInterface.InstalledPlugins.Where( x => x.InternalName.Equals( "Penumbra" ) && x.IsLoaded ).Any() ) return;
            try {
                ApiVersionsSubscriber.Invoke();
            }
            catch( Exception ) { return; }

            PenumbraEnabled = true;
            RefreshEnabledState();
        }

        private void RefreshEnabledState() {
            try {
                ModsEnabled = GetEnabledStateSubscriber.Invoke();
            }
            catch( Exception ) {
                ModsEnabled = true; // older Penumbra, assume enabled
            }
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
                return [.. GetModsSubscriber.Invoke().Select( x => x.Key )];
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

        // Temporary mods apply to every collection. Removing is done by registering nothing
        public bool SetTemporaryMod( string tag, Dictionary<string, string> paths, string meta, int priority ) {
            if( !PenumbraEnabled ) return false;
            try {
                if( paths.Count == 0 && string.IsNullOrEmpty( meta ) ) {
                    var removeResult = RemoveTemporaryModAllSubscriber.Invoke( tag, priority );
                    return removeResult == PenumbraApiEc.Success || removeResult == PenumbraApiEc.NothingChanged;
                }
                var result = AddTemporaryModAllSubscriber.Invoke( tag, paths, meta ?? string.Empty, priority );
                if( result != PenumbraApiEc.Success ) Dalamud.Error( $"Penumbra AddTemporaryModAll failed: {result}" );
                return result == PenumbraApiEc.Success;
            }
            catch( Exception e ) {
                Dalamud.Error( e, "Error setting Penumbra temporary mod" );
                return false;
            }
        }

        public bool Redraw( int objectIndex ) {
            if( !PenumbraEnabled ) return false;
            try {
                RedrawObjectSubscriber.Invoke( objectIndex, RedrawType.Redraw );
                return true;
            }
            catch( Exception e ) {
                Dalamud.Error( e, "Error redrawing with Penumbra" );
                return false;
            }
        }

        // These can be raised from Penumbra's thread, so only set state here; PenumbraRequiredWindow reacts on the next draw
        private void EnablePenumbra() {
            PenumbraEnabled = true;
            RefreshEnabledState();
            PenumbraPreview.Invalidate(); // temporary mods do not survive a Penumbra reload, re-register on next sync
        }

        private void DisablePenumbra() {
            PenumbraEnabled = false;
            PenumbraPreview.Invalidate();
        }

        private void OnEnabledChange( bool enabled ) {
            ModsEnabled = enabled;
        }

        public void Dispose() {
            InitializedSubscriber?.Dispose();
            DisposedSubscriber?.Dispose();
            EnabledChangeSubscriber?.Dispose();
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
