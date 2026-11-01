using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Interop.Penumbra;
using VfxEditor.Select;
using VfxEditor.Ui.Export;
using VfxEditor.Utils;

namespace VfxEditor.FileManager {
    public abstract partial class FileManagerDocument<R, S> : IFileDocument where R : FileManagerFile {
        public R File { get; protected set; }
        protected VerifiedStatus Verified => File == null ? VerifiedStatus.UNKNOWN : File.Verified;
        public bool Unsaved => File != null && File.Unsaved;
        public bool LocalSource => Source != null && Source.Type == SelectResultType.Local;

        public string DisplayName => string.IsNullOrEmpty( Name ) ? ReplaceDisplay : Name;
        protected string Name = "";

        protected SelectResult Source;
        protected SelectResult Replace;
        public string SourceDisplay => Source == null ? "[NONE]" : Source.DisplayString;
        public string ReplaceDisplay => Replace == null ? "[NONE]" : Replace.DisplayString;
        public string ReplacePath => ( Disabled || Replace == null ) ? "" : Replace.Path;
        protected bool Disabled = false;

        private string SourceTextInput = "";
        private string ReplaceTextInput = "";

        public string WriteLocation { get; protected set; }

        public abstract string Id { get; }
        public abstract string Extension { get; }

        protected readonly FileManagerBase Manager;

        protected DateTime LastUpdate = DateTime.Now;

        public FileManagerDocument( FileManagerBase manager, string writeLocation ) {
            Manager = manager;
            WriteLocation = writeLocation;
        }

        public bool GetReplacePath( string path, out string replacePath ) {
            replacePath = null;
            if( File == null ) return false;

            replacePath = ReplacePath.ToLower().Equals( path.ToLower() ) ? WriteLocation : null;
            return !string.IsNullOrEmpty( replacePath );
        }

        public bool GetActiveReplacement( out string gamePath, out string localPath ) {
            gamePath = ReplacePath;
            localPath = WriteLocation;
            return File != null && !string.IsNullOrEmpty( gamePath ) && !string.IsNullOrEmpty( localPath );
        }

        protected abstract R FileFromReader( BinaryReader reader, bool verify );

        protected void LoadLocal( string path, bool verify ) {
            if( !System.IO.File.Exists( path ) ) {
                Dalamud.Error( $"Local file: [{path}] does not exist" );
                return;
            }

            if( !path.EndsWith( $".{Extension}" ) ) {
                Dalamud.Error( $"{path} is the wrong file type" );
                return;
            }

            try {
                using var reader = new BinaryReader( System.IO.File.Open( path, FileMode.Open ) );
                File?.Dispose();
                File = FileFromReader( reader, verify );
            }
            catch( Exception e ) {
                Dalamud.Error( e, $"Error reading file: {path}" );
                Dalamud.ErrorNotification( "Error reading file" );
            }
        }

        protected void LoadGame( string path, bool verify ) {
            if( !Dalamud.DataManager.FileExists( path ) ) {
                Dalamud.Error( $"Game file: [{path}] does not exist" );
                return;
            }

            if( !path.EndsWith( $".{Extension}" ) ) {
                Dalamud.Error( $"{path} is the wrong file type" );
                return;
            }

            try {
                var file = Dalamud.DataManager.GetFile( path );
                using var ms = new MemoryStream( file.Data );
                using var reader = new BinaryReader( ms );
                File?.Dispose();
                File = FileFromReader( reader, verify );
            }
            catch( Exception e ) {
                Dalamud.Error( e, $"Error Reading File at {path}" );
                Dalamud.ErrorNotification( $"Error Reading File at {path}" );
            }
        }

        // =================

        public void SetSource( SelectResult result ) {
            if( result == null ) return;
            Source = result;
            SourceTextInput = "";

            if( result.Type == SelectResultType.Local ) LoadLocal( result.Path, true );
            else LoadGame( result.Path, true );

            if( File != null ) {
                WriteFile( WriteLocation );
            }
        }

        protected void RemoveSource() {
            File?.Dispose();
            File = null;
            Source = null;
            SourceTextInput = "";
        }

        public void SetReplace( SelectResult result ) {
            Replace = result;
            ReplaceTextInput = "";
            Plugin.AddCustomBackupLocation( Replace, WriteLocation );
        }

        protected void RemoveReplace() {
            Replace = null;
            ReplaceTextInput = "";
        }

        // =====================

        protected void WriteFile( string path ) {
            if( File == null ) return;
            if( Plugin.Configuration?.LogDebug == true ) Dalamud.Log( $"Wrote {Id} file to {path}" );
            System.IO.File.WriteAllBytes( path, File.ToBytes() );
        }

        protected void ExportRawDialog() => UiUtils.WriteBytesDialog( $".{Extension}", File.ToBytes(), Extension, "ExportedFile" );

        public void Update() {
            if( ( DateTime.Now - LastUpdate ).TotalSeconds <= 0.2 ) return;
            LastUpdate = DateTime.Now;

            File?.Update();

            var newWriteLocation = Manager.NewWriteLocation;
            WriteFile( newWriteLocation );
            WriteLocation = newWriteLocation;
            Plugin.AddCustomBackupLocation( Replace, WriteLocation );

            // Penumbra resolves the game path to the new temp file (new path -> new resource handle), then redraws
            PenumbraPreview.UpdateAndRedraw();
        }

        // =======================

        protected void LoadWorkspace( string localPath, string relativeLocation, string name, SelectResult source, SelectResult replace, bool disabled ) {
            Name = name ?? "";
            Source = source;
            Replace = replace;
            Disabled = disabled;

            SourceTextInput = "";
            ReplaceTextInput = "";

            LoadLocal( WorkspaceUtils.ResolveWorkspacePath( relativeLocation, localPath ), false );
            File?.Verified = VerifiedStatus.WORKSPACE;
            WriteFile( WriteLocation );
        }

        public string GetExportSource() => SourceDisplay;

        public string GetExportReplace() => DisplayName;

        public bool CanExport() => File != null && !string.IsNullOrEmpty( ReplacePath );

        public virtual void PenumbraExportManipulations( List<object> manipulationsOut ) { }

        public virtual void PenumbraExport( string modFolder, string groupOption, Dictionary<string, string> filesOut ) {
            var path = ReplacePath;
            if( string.IsNullOrEmpty( path ) || File == null ) return;
            var data = File.ToBytes();

            PenumbraUtils.WriteBytes( data, modFolder, groupOption, path, filesOut );
        }

        public void TextoolsExport( BinaryWriter writer, List<TTMPL_Simple> simplePartsOut, ref int modOffset ) {
            var path = ReplacePath;
            if( string.IsNullOrEmpty( path ) || File == null ) return;

            var modData = TexToolsUtils.CreateType2Data( File.ToBytes() );
            simplePartsOut.Add( TexToolsUtils.CreateModResource( path, modOffset, modData.Length ) );
            writer.Write( modData );
            modOffset += modData.Length;
        }

        public abstract S GetWorkspaceMeta( string newPath, int windowIdx );

        public void WorkspaceExport( List<S> meta, string rootPath, string newPath, int windowIdx ) {
            if( File == null ) return;

            var newFullPath = Path.Combine( rootPath, newPath );
            System.IO.File.WriteAllBytes( newFullPath, File.ToBytes() );
            meta.Add( GetWorkspaceMeta( newPath, windowIdx ) );
        }

        public virtual void Dispose() {
            File?.Dispose();
            File = null;
        }
    }
}