using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Windowing;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Data;
using VfxEditor.FileBrowser;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Formats.TextureFormat.Textures;
using VfxEditor.Formats.TextureFormat.Ui;
using VfxEditor.Interop.Penumbra;
using VfxEditor.Select;
using VfxEditor.Ui;
using VfxEditor.Ui.Export;
using VfxEditor.Utils;

namespace VfxEditor.Formats.TextureFormat {
    public class TextureManager : DalamudWindow, IFileManagerGroup, IFileManagerSelect {
        private static readonly string WORKSPACE_KEY = "Tex";

        private int TEX_ID = 0;
        public static string TempAtex => Path.Combine( Plugin.Configuration.WriteLocation, "temp_convert.atex" ).Replace( '\\', '/' );
        public static string TempEdit => Path.Combine( Plugin.Configuration.WriteLocation, "temp_edit.atex" ).Replace( '\\', '/' );

        public readonly WindowSystem WindowSystem = new();

        public readonly List<IDalamudTextureWrap> Wraps = [];
        public readonly List<IDalamudTextureWrap> WrapsToCleanup = [];

        private readonly List<TextureReplace> Textures = [];
        private readonly Dictionary<string, TexturePreview> Previews = [];
        private readonly HashSet<string> FailedPreviews = []; // Don't re-read and re-log broken textures every frame
        private readonly TextureView View;
        private readonly ManagerConfiguration Configuration;

        public TextureManager() : base( "Textures", false, new( 800, 500 ), Plugin.WindowSystem, isMainWindow: true ) {
            Configuration = Plugin.Configuration.GetManagerConfig( "Tex" );
            View = new( this, Textures );
        }

        public ManagerConfiguration GetConfig() => Configuration;

        public IEnumerable<IFileDocument> GetDocuments() => Textures;

        public string GetId() => "Textures";

        public string GetName() => "tex";

        public int GetWindowId() => 1;

        public bool AnyWindowsOpen() => IsOpen;

        public void ReplaceTexture( string importPath, string gamePath ) {
            var replace = new TextureReplace( gamePath, GetNewWriteLocation( gamePath ) );
            if( !replace.ImportFile( importPath ) ) return;
            Textures.Add( replace );
            PenumbraTempMod.MarkDirty();
        }

        public void RemoveReplace( TextureReplace replace ) {
            Textures.Remove( replace );
            View.ClearSelected();
            ExportDialog.RemoveDocument( replace );
            PenumbraTempMod.MarkDirty();
        }

        public void Import( SelectResult result ) {
            FileBrowserManager.OpenFileDialog( "Select a File", "Image files{.png,.tex,.atex,.dds},.*", ( ok, res ) => {
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
            if( FailedPreviews.Contains( gamePath ) ) return null;

            var gameFileExists = Dalamud.GameFileExists( gamePath );
            var penumbraFileExits = Plugin.PenumbraIpc.PenumbraFileExists( gamePath, out var penumbraPath );

            if( !gameFileExists && !penumbraFileExits ) return new TextureMissing( gamePath );

            try {
                var sourcePath = penumbraFileExits ? penumbraPath : gamePath;
                var data = Path.IsPathRooted( sourcePath ) ?
                    TextureDataFile.LoadFromLocal( sourcePath ) :
                    Dalamud.DataManager.GetFile<TextureDataFile>( sourcePath );

                if( !data.ValidFormat ) {
                    Dalamud.Error( $"Invalid format: {data.Header.Format} {gamePath}" );
                    FailedPreviews.Add( gamePath );
                    return null;
                }
                var newPreview = new TexturePreview( data, sourcePath, penumbraFileExits, gamePath );
                Previews[gamePath] = newPreview;
                return newPreview;
            }
            catch( Exception e ) {
                Dalamud.Error( e, $"Could not find tex: {gamePath}" );
                FailedPreviews.Add( gamePath );
                return null;
            }
        }

        public bool FileExists( string path ) => IFileManagerGroup.FileExist( this, path );

        public bool GetReplacePath( string path, out string replacePath ) => IFileManagerGroup.GetReplacePath( this, path, out replacePath );

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
            var items = WorkspaceUtils.ReadFromMeta<WorkspaceMetaTex>( meta, WORKSPACE_KEY );
            if( items == null ) return;

            foreach( var item in items ) {
                var fullPath = WorkspaceUtils.ResolveWorkspacePath( item.RelativeLocation, Path.Combine( loadLocation, WORKSPACE_KEY ) );
                var newReplace = new TextureReplace( GetNewWriteLocation( item.ReplacePath ), item );
                newReplace.ImportFile( fullPath );
                Textures.Add( newReplace );
            }

            var windows = WorkspaceUtils.GetWindowData( meta, WORKSPACE_KEY );
            SetMeta( windows?[0] );
        }

        public void WorkspaceExport( Dictionary<string, string> meta, string saveLocation, Dictionary<string, WorkspaceWindow[]> windows ) {
            var texRootPath = Path.Combine( saveLocation, WORKSPACE_KEY );
            Directory.CreateDirectory( texRootPath );

            var idx = 0;
            var texMeta = new List<WorkspaceMetaTex>();
            foreach( var texture in Textures ) {
                texMeta.Add( texture.WorkspaceExport( texRootPath, idx ) );
                idx++;
            }
            WorkspaceUtils.WriteToMeta( meta, texMeta.ToArray(), WORKSPACE_KEY );

            windows[WORKSPACE_KEY] = [ToMeta()];
        }

        // ================

        public void Reset( bool pluginClosing ) {
            Textures.Clear();
            Previews.Clear();
            FailedPreviews.Clear();

            // Weird async stuff, otherwise cleanup happens to the textures after they've been loaded
            WrapsToCleanup.Clear();
            WrapsToCleanup.AddRange( Wraps );
            Wraps.Clear();
            Dalamud.Framework.RunOnFrameworkThread( CleanupWraps );

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

        public WindowSystem GetWindowSystem() => WindowSystem;

        public bool CanImport( string path ) {
            return path.EndsWith( "atex", StringComparison.Ordinal );
        }

        public void PenumbraImport( SelectResult selectedFile, SelectResult replacedFile ) {
            ReplaceTexture( selectedFile.Path, replacedFile.Name );
        }
    }
}