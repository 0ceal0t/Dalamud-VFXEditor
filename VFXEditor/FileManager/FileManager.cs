using ImGuiNET;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Data.Copy;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Select;
using VfxEditor.Utils;

namespace VfxEditor.FileManager {
    public abstract partial class FileManager<T, R, S> : FileManagerBase, IFileManager where T : FileManagerDocument<R, S> where R : FileManagerFile {
        public T Document { get; protected set; } = null;
        public R File => Document?.File;

        private int DOC_ID = 0;
        public override string NewWriteLocation => Path.Combine( Plugin.Configuration.WriteLocation, $"{Id}Temp{DOC_ID++}.{Extension}" ).Replace( '\\', '/' );

        private readonly FileManagerDocumentWindow<T, R, S> DocumentWindow;
        public readonly List<T> Documents = new();

        public FileManager( string title, string id ) : this( title, id, id.ToLower(), id, id ) { }

        public FileManager( string title, string id, string extension, string workspaceKey, string workspacePath ) : base( title, id, extension, workspaceKey, workspacePath ) {
            AddDocument();
            DocumentWindow = new( title, this );
        }

        // ===================

        public override void SetSource( SelectResult result ) {
            Document?.SetSource( result );
            Plugin.Configuration.AddRecent( Configuration.RecentItems, result );
        }

        public override void SetReplace( SelectResult result ) {
            Document?.SetReplace( result );
            Plugin.Configuration.AddRecent( Configuration.RecentItems, result );
        }

        private void CheckKeybinds() {
            if( !ImGui.IsWindowFocused( ImGuiFocusedFlags.RootAndChildWindows ) ) return;
            if( Plugin.Configuration.CopyKeybind.KeyPressed() ) CopyManager.Copy();
            if( Plugin.Configuration.PasteKeybind.KeyPressed() ) CopyManager.Paste();
            if( Plugin.Configuration.UndoKeybind.KeyPressed() ) CommandManager.Undo();
            if( Plugin.Configuration.RedoKeybind.KeyPressed() ) CommandManager.Redo();
            Document?.CheckKeybinds();
        }

        // ====================

        protected abstract T GetNewDocument();

        public void AddDocument() {
            var newDocument = GetNewDocument();
            Document = newDocument;
            Documents.Add( newDocument );
        }

        public void SelectDocument( T document ) {
            Document = document;
        }

        public bool RemoveDocument( T document ) {
            Documents.Remove( document );

            DraggingItem = null;
            DocumentWindow.Reset();

            if( document == Document ) {
                Document = Documents[0];
                document.Dispose();
                return true;
            }

            document.Dispose();
            return false;
        }

        // ====================

        public IEnumerable<IFileDocument> GetDocuments() => Documents;

        public void WorkspaceImport( JObject meta, string loadLocation ) {
            var items = WorkspaceUtils.ReadFromMeta<S>( meta, WorkspaceKey );
            if( items == null ) {
                AddDocument();
                return;
            }
            foreach( var item in items ) {
                var newDocument = GetWorkspaceDocument( item, Path.Combine( loadLocation, WorkspacePath ) );
                Document = newDocument;
                Documents.Add( newDocument );
            }
            if( Documents.Count == 0 ) AddDocument();
        }

        protected abstract T GetWorkspaceDocument( S data, string localPath );

        public void WorkspaceExport( Dictionary<string, string> meta, string saveLocation ) {
            var rootPath = Path.Combine( saveLocation, WorkspacePath );
            Directory.CreateDirectory( rootPath );

            var documentIdx = 0;
            List<S> documentMeta = new();

            foreach( var document in Documents ) {
                document.WorkspaceExport( documentMeta, rootPath, $"{Id}Temp{documentIdx++}.{Extension}" );
            }

            WorkspaceUtils.WriteToMeta( meta, documentMeta.ToArray(), WorkspaceKey );
        }

        // ====================

        public bool FileExists( string path ) => IFileManager.FileExist( this, path );

        public bool GetReplacePath( string path, out string replacePath ) => IFileManager.GetReplacePath( this, path, out replacePath );

        public bool DoDebug( string path ) => path.Contains( $".{Extension}" );

        public void ToDefault() {
            Reset();
            AddDocument();
        }

        public virtual void Reset() {
            Documents.ForEach( x => x.Dispose() );
            Documents.Clear();
            SourceSelect?.Hide();
            ReplaceSelect?.Hide();

            Document = null;
            DraggingItem = null;
            DocumentWindow.Reset();

            DOC_ID = 0;
        }
    }
}
