using Dalamud.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.Data;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Formats.TextureFormat.Textures;
using VfxEditor.Formats.TextureFormat.Ui;
using VfxEditor.Ui;

namespace VfxEditor.Formats.TextureFormat {
    public class TextureManager : GenericDialog, IFileManager {
        // TODO: test custom paths
        // TODO: what if replace the same texture?
        // TODO: options for view layout?
        // TODO: image for if nothing?

        private int TEX_ID = 0;
        public string NewWriteLocation => Path.Combine( Plugin.Configuration.WriteLocation, $"TexTemp{TEX_ID++}.atex" ).Replace( '\\', '/' );

        private readonly List<TextureReplace> Textures = new();
        private readonly Dictionary<string, TexturePreview> Previews = new();
        private readonly TextureView View;
        private readonly ManagerConfiguration Configuration;

        public TextureManager() : base( "Textures", false, 800, 500 ) {
            View = new( Textures );
            Configuration = Plugin.Configuration.GetManagerConfig( "Tex" );
        }

        public CopyManager GetCopyManager() => null;
        public CommandManager GetCommandManager() => null;
        public ManagerConfiguration GetConfig() => Configuration;
        public IEnumerable<IFileDocument> GetDocuments() => Textures;
        public string GetId() => "Textures";

        public void ReplaceTexture( string importPath, string gamePath, ushort pngMip = 9, TextureFormat pngFormat = TextureFormat.DXT5 ) {
            var newReplace = new TextureReplace( gamePath, NewWriteLocation );
            newReplace.ImportFile( importPath, pngMip, pngFormat );
            Textures.Add( newReplace );
        }

        public void RemoveReplace( TextureReplace replace ) {
            replace.Dispose();
            Textures.Remove( replace );
            View.ClearSelected();
        }

        public override void DrawBody() => View.Draw();

        // ====================

        public TextureDrawable GetTexture( string gamePath ) {
            gamePath = gamePath.Trim( '\0' );
            if( string.IsNullOrEmpty( gamePath ) || !gamePath.Contains( '/' ) ) return null;

            foreach( var texture in Textures ) {
                if( texture.GetReplacePath( gamePath, out var _ ) ) return texture;
            }

            if( Previews.TryGetValue( gamePath, out var preview ) ) return preview;

            if( !Plugin.DataManager.FileExists( gamePath ) ) return null;

            try {
                var data = Plugin.DataManager.GetFile<TextureDataFile>( gamePath );
                if( !data.ValidFormat ) {
                    PluginLog.Error( $"Invalid format: {data.Header.Format} {gamePath}" );
                    return null;
                }
                var newPreview = new TexturePreview( data, gamePath );
                Previews[gamePath] = newPreview;
                return newPreview;
            }
            catch( Exception e ) {
                PluginLog.Error( e, $"Could not find tex: {gamePath}" );
                return null;
            }
        }

        public bool GetReplacePath( string path, out string replacePath ) => IFileManager.GetReplacePath( this, path, out replacePath );

        public bool DoDebug( string path ) => path.Contains( ".atex" ) || path.Contains( ".tex" );

        // ===================

        public void WorkspaceImport( JObject meta, string loadLocation ) => View.WorkspaceImport( meta, loadLocation );

        public void WorkspaceExport( Dictionary<string, string> meta, string saveLocation ) => View.WorkspaceExport( meta, saveLocation );

        // ================

        public void ToDefault() => Dispose();

        public void Dispose() {
            Textures.ForEach( x => x.Dispose() );
            Previews.Values.ToList().ForEach( x => x.Dispose() );
            Textures.Clear();
            Previews.Clear();
            TEX_ID = 0;
        }

        // =======================

        public static void Setup() {
            // Set paths manually since TexImpNet can be dumb sometimes
            // Using the 32-bit version in all cases because net6, I guess
            var runtimeRoot = Path.Combine( Plugin.RootLocation, "runtimes" );

            var freeImgLib = TeximpNet.Unmanaged.FreeImageLibrary.Instance;
            var _32bitPath = Path.Combine( runtimeRoot, "win-x64", "native", "FreeImage.dll" );
            var _64bitPath = Path.Combine( runtimeRoot, "win-x86", "native", "FreeImage.dll" );
            freeImgLib.Resolver.SetOverrideLibraryName32( _32bitPath );
            freeImgLib.Resolver.SetOverrideLibraryName64( _32bitPath );
            PluginLog.Log( $"FreeImage TeximpNet paths: {_32bitPath} / {_64bitPath}" );
            PluginLog.Log( $"FreeImage Default name: {freeImgLib.DefaultLibraryName} Library loaded: {freeImgLib.IsLibraryLoaded}" );
            freeImgLib.LoadLibrary();
            PluginLog.Log( $"FreeImage Library path: {freeImgLib.LibraryPath} Library loaded: {freeImgLib.IsLibraryLoaded}" );

            var nvtLib = TeximpNet.Unmanaged.NvTextureToolsLibrary.Instance;
            var nv_32bitPath = Path.Combine( runtimeRoot, "win-x64", "native", "nvtt.dll" );
            var nv_64bitPath = Path.Combine( runtimeRoot, "win-x86", "native", "nvtt.dll" );
            nvtLib.Resolver.SetOverrideLibraryName32( nv_32bitPath );
            nvtLib.Resolver.SetOverrideLibraryName64( nv_32bitPath );
            PluginLog.Log( $"NVT TeximpNet paths: {nv_32bitPath} / {nv_64bitPath}" );
            PluginLog.Log( $"NVT Default name: {nvtLib.DefaultLibraryName} Library loaded: {nvtLib.IsLibraryLoaded}" );
            nvtLib.LoadLibrary();
            PluginLog.Log( $"NVT Library path: {nvtLib.LibraryPath} Library loaded: {nvtLib.IsLibraryLoaded}" );
        }

        public static void BreakDown() {
            TeximpNet.Unmanaged.FreeImageLibrary.Instance.FreeLibrary();
            TeximpNet.Unmanaged.NvTextureToolsLibrary.Instance.FreeLibrary();
            PluginLog.Log( $"FreeImage Library loaded: {TeximpNet.Unmanaged.FreeImageLibrary.Instance.IsLibraryLoaded}" );
            PluginLog.Log( $"NVTT Library loaded: {TeximpNet.Unmanaged.NvTextureToolsLibrary.Instance.IsLibraryLoaded}" );
        }
    }
}
