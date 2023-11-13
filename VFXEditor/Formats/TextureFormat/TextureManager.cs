using Dalamud.Interface.Internal;
using Dalamud.Interface.Windowing;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.FileBrowser;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Formats.TextureFormat.Textures;
using VfxEditor.Formats.TextureFormat.Ui;
using VfxEditor.Select;
using VfxEditor.Ui;
using VfxEditor.Utils;

namespace VfxEditor.Formats.TextureFormat {
    public class TextureManager : DalamudWindow, IFileManager {
        private int TEX_ID = 0;
        public static string TempAtex => Path.Combine( Plugin.Configuration.WriteLocation, $"temp_convert.atex" ).Replace( '\\', '/' );

        public readonly WindowSystem WindowSystem = new();

        public readonly List<IDalamudTextureWrap> Wraps = new();
        public readonly List<IDalamudTextureWrap> WrapsToCleanup = new();

        private readonly List<TextureReplace> Textures = new();
        private readonly Dictionary<string, TexturePreview> Previews = new();
        private readonly TextureView View;
        private readonly ManagerConfiguration Configuration;

        public TextureManager() : base( "Textures", false, new( 800, 500 ), Plugin.WindowSystem ) {
            Configuration = Plugin.Configuration.GetManagerConfig( "Tex" );
            View = new( this, Textures );
        }

        public ManagerConfiguration GetConfig() => Configuration;

        public IEnumerable<IFileDocument> GetDocuments() => Textures;

        public string GetId() => "Textures";

        public bool IsWindowOpen() => IsOpen;

        public void ReplaceTexture( string importPath, string gamePath ) {
            var replace = new TextureReplace( gamePath, GetNewWriteLocation( gamePath ) );
            replace.ImportFile( importPath );
            Textures.Add( replace );
        }

        public void RemoveReplace( TextureReplace replace ) {
            Textures.Remove( replace );
            View.ClearSelected();
        }

        public void Import( SelectResult result ) {
            FileBrowserManager.OpenFileDialog( "Select a File", "Image files{.png,.tex,.atex,.dds},.*", ( bool ok, string res ) => {
                if( !ok ) return;
                try {
                    AddRecent( result );
                    ReplaceTexture( res, result.Path );
                }
                catch( Exception e ) {
                    Dalamud.Error( e, "Could not import data" );
                }
            } );
        }

        public void AddRecent( SelectResult result ) => Plugin.Configuration.AddRecent( Configuration.RecentItems, result );

        public override void DrawBody() {
            View.Draw();
            WindowSystem.Draw();
        }

        public string GetNewWriteLocation( string path ) => Path.Combine( Plugin.Configuration.WriteLocation, $"TexTemp{TEX_ID++}." + path.Split( '.' )[^1] ).Replace( '\\', '/' );

        // ====================

        public TextureDrawable GetTexture( string gamePath ) {
            gamePath = gamePath.Trim( '\0' );
            if( string.IsNullOrEmpty( gamePath ) || !gamePath.Contains( '/' ) ) return null;

            foreach( var texture in Textures ) {
                if( texture.GetReplacePath( gamePath, out var _ ) ) return texture;
            }

            if( Previews.TryGetValue( gamePath, out var preview ) ) return preview;

            var gameFileExists = Dalamud.GameFileExists( gamePath );
            var penumbraFileExits = Plugin.PenumbraIpc.PenumbraFileExists( gamePath, out var penumbraPath );

            if( !gameFileExists && !penumbraFileExits ) return new TextureMissing( gamePath );

            try {
                var data = penumbraFileExits ?
                    ( Path.IsPathRooted( penumbraPath ) ?
                        TextureDataFile.LoadFromLocal( penumbraPath ) :
                        Dalamud.DataManager.GetFile<TextureDataFile>( penumbraPath )
                    ) :
                    Dalamud.DataManager.GetFile<TextureDataFile>( gamePath );

                if( !data.ValidFormat ) {
                    Dalamud.Error( $"Invalid format: {data.Header.Format} {gamePath}" );
                    return null;
                }
                var newPreview = new TexturePreview( data, penumbraFileExits, gamePath );
                Previews[gamePath] = newPreview;
                return newPreview;
            }
            catch( Exception e ) {
                Dalamud.Error( e, $"Could not find tex: {gamePath}" );
                return null;
            }
        }

        public bool FileExists( string path ) => IFileManager.FileExist( this, path );

        public bool GetReplacePath( string path, out string replacePath ) => IFileManager.GetReplacePath( this, path, out replacePath );

        public bool DoDebug( string path ) => path.Contains( ".atex" ) || path.Contains( ".tex" );

        // Not already converted, file exists and can be converted, not already replaced
        public bool CanConvertToCustom( string path ) =>
            !string.IsNullOrEmpty( path ) && Dalamud.GameFileExists( path ) && !Plugin.PenumbraIpc.PenumbraFileExists( path, out var _ ) && !GetReplacePath( path, out var _ );

        public bool ConvertToCustom( string path, out string newPath ) {
            newPath = path;
            if( !CanConvertToCustom( path ) ) return false;

            newPath = string.IsNullOrEmpty( Plugin.Configuration.CustomPathPrefix ) ? path : Plugin.Configuration.CustomPathPrefix + path.Split( "/", 2 )[1];
            if( FileExists( newPath ) ) {
                Dalamud.Log( $"{newPath} already converted" );
                return true;
            }

            Dalamud.DataManager.GetFile( path )?.SaveFile( TempAtex );
            ReplaceTexture( TempAtex, newPath );
            Dalamud.Log( $"Converted {path} -> {newPath}" );
            return true;
        }

        // ===================

        public void WorkspaceImport( JObject meta, string loadLocation ) {
            var items = WorkspaceUtils.ReadFromMeta<WorkspaceMetaTex>( meta, "Tex" );
            if( items == null ) return;
            foreach( var item in items ) {
                var fullPath = WorkspaceUtils.ResolveWorkspacePath( item.RelativeLocation, Path.Combine( loadLocation, "Tex" ) );
                var newReplace = new TextureReplace( GetNewWriteLocation( item.ReplacePath ), item );
                newReplace.ImportFile( fullPath );
                Textures.Add( newReplace );
            }
        }

        public void WorkspaceExport( Dictionary<string, string> meta, string saveLocation ) {
            var texRootPath = Path.Combine( saveLocation, "Tex" );
            Directory.CreateDirectory( texRootPath );

            var idx = 0;
            var texMeta = new List<WorkspaceMetaTex>();
            foreach( var texture in Textures ) {
                texMeta.Add( texture.WorkspaceExport( texRootPath, idx ) );
                idx++;
            }
            WorkspaceUtils.WriteToMeta( meta, texMeta.ToArray(), "Tex" );
        }

        // ================

        public void Reset( ResetType type ) {
            Textures.Clear();
            Previews.Clear();

            // Weird async stuff, otherwise cleanup happens to the textures after they've been loaded
            WrapsToCleanup.Clear();
            WrapsToCleanup.AddRange( Wraps );
            Wraps.Clear();
            if( Plugin.State == WorkspaceState.Loading ) Plugin.OnMainThread += CleanupWraps;
            else CleanupWraps();

            TEX_ID = 0;
        }

        public void CleanupWraps() {
            foreach( var wrap in WrapsToCleanup ) {
                try {
                    wrap?.Dispose();
                }
                catch( Exception ) { }
            }
            WrapsToCleanup.Clear();
        }

        // =======================

        public static void LoadLibrary() {
            // Set paths manually since TexImpNet can be dumb sometimes
            // Using the 32-bit version in all cases because net6, I guess
            var runtimeRoot = Path.Combine( Plugin.RootLocation, "runtimes" );

            var freeImgLib = TeximpNet.Unmanaged.FreeImageLibrary.Instance;
            var _32bitPath = Path.Combine( runtimeRoot, "win-x64", "native", "FreeImage.dll" );
            var _64bitPath = Path.Combine( runtimeRoot, "win-x86", "native", "FreeImage.dll" );
            freeImgLib.Resolver.SetOverrideLibraryName32( _32bitPath );
            freeImgLib.Resolver.SetOverrideLibraryName64( _32bitPath );
            Dalamud.Log( $"FreeImage TeximpNet paths: {_32bitPath} / {_64bitPath}" );
            Dalamud.Log( $"FreeImage Default name: {freeImgLib.DefaultLibraryName} Library loaded: {freeImgLib.IsLibraryLoaded}" );
            freeImgLib.LoadLibrary();
            Dalamud.Log( $"FreeImage Library path: {freeImgLib.LibraryPath} Library loaded: {freeImgLib.IsLibraryLoaded}" );

            var nvtLib = TeximpNet.Unmanaged.NvTextureToolsLibrary.Instance;
            var nv_32bitPath = Path.Combine( runtimeRoot, "win-x64", "native", "nvtt.dll" );
            var nv_64bitPath = Path.Combine( runtimeRoot, "win-x86", "native", "nvtt.dll" );
            nvtLib.Resolver.SetOverrideLibraryName32( nv_32bitPath );
            nvtLib.Resolver.SetOverrideLibraryName64( nv_32bitPath );
            Dalamud.Log( $"NVT TeximpNet paths: {nv_32bitPath} / {nv_64bitPath}" );
            Dalamud.Log( $"NVT Default name: {nvtLib.DefaultLibraryName} Library loaded: {nvtLib.IsLibraryLoaded}" );
            nvtLib.LoadLibrary();
            Dalamud.Log( $"NVT Library path: {nvtLib.LibraryPath} Library loaded: {nvtLib.IsLibraryLoaded}" );
        }

        public static void FreeLibrary() {
            TeximpNet.Unmanaged.FreeImageLibrary.Instance.FreeLibrary();
            TeximpNet.Unmanaged.NvTextureToolsLibrary.Instance.FreeLibrary();
            Dalamud.Log( $"FreeImage Library loaded: {TeximpNet.Unmanaged.FreeImageLibrary.Instance.IsLibraryLoaded}" );
            Dalamud.Log( $"NVTT Library loaded: {TeximpNet.Unmanaged.NvTextureToolsLibrary.Instance.IsLibraryLoaded}" );
        }

        public WindowSystem GetWindowSystem() => WindowSystem;
    }
}
