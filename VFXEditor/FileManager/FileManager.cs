using Dalamud.Bindings.ImGui;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Data.Copy;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Select;
using VfxEditor.Ui.Export;
using VfxEditor.Utils;

namespace VfxEditor.FileManager {
    public abstract partial class FileManager<T, R, S> : FileManagerBase, IFileManager where T : FileManagerDocument<R, S> where R : FileManagerFile {
        public T ActiveDocument { get; protected set; }
        public R? File => ActiveDocument?.File;

        private int DOC_ID = 0;
        public override string NewWriteLocation => Path.Combine( Plugin.Configuration.WriteLocation, $"{Id}Temp{DOC_ID++}.{Extension}" ).Replace( '\\', '/' );

        private readonly FileManagerDocumentWindow<T, R, S> DocumentWindow;
        public readonly List<T> Documents = [];

        public FileManager( string title, string id ) : this( title, id, id.ToLower(), id, id ) { }

        public FileManager( string title, string id, string extension, string workspaceKey, string workspacePath ) : base( title, id, extension, workspaceKey, workspacePath ) {
            AddDocument();
            DocumentWindow = new( title, this );
        }

        // ===================

        public override void SetSource( SelectResult result ) {
            ActiveDocument?.SetSource( result );
            Plugin.Configuration.AddRecent( Configuration.RecentItems, result );
        }

        public override void SetReplace( SelectResult result ) {
            ActiveDocument?.SetReplace( result );
            Plugin.Configuration.AddRecent( Configuration.RecentItems, result );
        }

        private void CheckKeybinds() {
            if( !ImGui.IsWindowFocused( ImGuiFocusedFlags.RootAndChildWindows ) ) return;
            if( Plugin.Configuration.CopyKeybind.KeyPressed() ) CopyManager.Copy();
            if( Plugin.Configuration.PasteKeybind.KeyPressed() ) CopyManager.Paste();
            if( Plugin.Configuration.UndoKeybind.KeyPressed() ) CommandManager.Undo();
            if( Plugin.Configuration.RedoKeybind.KeyPressed() ) CommandManager.Redo();
            ActiveDocument?.CheckKeybinds();
        }

        // ====================

        protected abstract T GetNewDocument();

        public void AddDocument() {
            ActiveDocument = GetNewDocument();
            Documents.Add( ActiveDocument );
        }

        public void SelectDocument( T document ) {
            ActiveDocument = document;
        }

        public bool RemoveDocument( T document ) {
            Documents.Remove( document );
            document.Dispose();

            DraggingItem = null;
            DocumentWindow.Reset();

            ExportDialog.RemoveDocument( document );

            if( document == ActiveDocument ) {
                ActiveDocument = Documents[0];
                return true;
            }
            return false;
        }

        // ====================

        public IEnumerable<IFileDocument> GetDocuments() => Documents;

        public void WorkspaceImport( JObject meta, string loadLocation ) {
            var items = WorkspaceUtils.ReadFromMeta<S>( meta, WorkspaceKey );
            if( items == null || items.Length == 0 ) {
                AddDocument();
                return;
            }
            foreach( var item in items ) {
                var newDocument = GetWorkspaceDocument( item, Path.Combine( loadLocation, WorkspacePath ) );
                ActiveDocument = newDocument;
                Documents.Add( newDocument );
            }
        }

        protected abstract T GetWorkspaceDocument( S data, string localPath );

        public void WorkspaceExport( Dictionary<string, string> meta, string saveLocation ) {
            var rootPath = Path.Combine( saveLocation, WorkspacePath );
            Directory.CreateDirectory( rootPath );

            List<S> documentMeta = [];
            foreach( var (document, idx) in Documents.WithIndex() ) {
                document.WorkspaceExport( documentMeta, rootPath, $"{Id}Temp{idx}.{Extension}" );
            }

            WorkspaceUtils.WriteToMeta( meta, documentMeta.ToArray(), WorkspaceKey );
        }

        // ====================

        public bool FileExists( string path ) => IFileManager.FileExist( this, path );

        public bool GetReplacePath( string path, out string replacePath ) => IFileManager.GetReplacePath( this, path, out replacePath );

        public bool DoDebug( string path ) => path.Contains( $".{Extension}" );

        public virtual void Reset( ResetType type ) {
            Documents.ForEach( x => x.Dispose() );
            Documents.Clear();
            SourceSelect?.Hide();
            ReplaceSelect?.Hide();

            ActiveDocument = null;
            DraggingItem = null;
            DocumentWindow.Reset();

            if( type == ResetType.ToDefault ) AddDocument(); // Default document
        }

        public bool AcceptsExt( string path )
        {
            return path.EndsWith( Extension );
        }

        public void PenumbraImport( SelectResult selectedFile, SelectResult replacedFile )
        {
            SetSource( selectedFile );
            SetReplace( replacedFile );
            AddDocument();
        }
    }
}
