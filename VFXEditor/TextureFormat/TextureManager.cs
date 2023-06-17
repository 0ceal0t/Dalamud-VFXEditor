using Dalamud.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.IO;
using VfxEditor.Data;
using VfxEditor.FileManager;
using VfxEditor.TextureFormat.Textures;
using VfxEditor.Ui;
using VfxEditor.Utils;

namespace VfxEditor.TextureFormat {
    public partial class TextureManager : GenericDialog, IFileManager {
        private int TEX_ID = 0;
        private readonly ConcurrentDictionary<string, TextureReplace> PathToTextureReplace = new(); // Keeps track of imported textures which replace existing ones
        private readonly ConcurrentDictionary<string, PreviewTexture> PathToTexturePreview = new(); // Keeps track of ImGui handles for previewed images

        public TextureManager() : base( "Textures", false, 600, 400 ) { }

        public bool GetReplacePath( string gamePath, out string localPath ) {
            localPath = PathToTextureReplace.TryGetValue( gamePath, out var textureReplace ) ? textureReplace.LocalPath : null;
            return !string.IsNullOrEmpty( localPath );
        }

        public void WorkspaceImport( JObject meta, string loadLocation ) {
            var items = WorkspaceUtils.ReadFromMeta<WorkspaceMetaTex>( meta, "Tex" );
            if( items == null ) return;
            foreach( var item in items ) {
                var fullPath = WorkspaceUtils.ResolveWorkspacePath( item.RelativeLocation, Path.Combine( loadLocation, "Tex" ) );
                ImportAtex( fullPath, item.ReplacePath, item.Height, item.Width, item.Depth, item.MipLevels, item.Format );
            }
        }

        // import replacement texture from atex
        private bool ImportAtex( string localPath, string gamePath, int height, int width, int depth, int mips, TextureFormat format ) {
            var gameFileExtension = gamePath.Split( '.' )[^1].Trim( '\0' );
            var path = Path.Combine( Plugin.Configuration.WriteLocation, "TexTemp" + ( TEX_ID++ ) + "." + gameFileExtension );
            File.Copy( localPath, path, true );

            var replaceData = new TextureReplace( localPath, gamePath, height, width, depth, mips, format );

            return ReplaceAndRefreshTexture( replaceData, gamePath );
        }

        public CopyManager GetCopyManager() => null;

        public CommandManager GetCommandManager() => null;

        public string GetExportName() => "Textures";

        public void ToDefault() => Dispose();

        public void Dispose() {
            foreach( var entry in PathToTexturePreview ) {
                if( entry.Value.Wrap?.ImGuiHandle == null ) continue;
                try {
                    entry.Value.Wrap?.Dispose();
                }
                catch( Exception ) {
                    // Already disposed
                }
            }
            foreach( var entry in PathToTextureReplace ) {
                File.Delete( entry.Value.LocalPath );
            }
            PathToTexturePreview.Clear();
            PathToTextureReplace.Clear();
            TEX_ID = 0;
        }

        public bool DoDebug( string path ) => path.Contains( ".atex" ) || path.Contains( ".tex" );

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
