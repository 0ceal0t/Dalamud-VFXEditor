using ImGuiNET;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Data;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Select;
using VfxEditor.Utils;

namespace VfxEditor.FileManager {
    public abstract partial class FileManager<T, R, S> : FileManagerBase, IFileManager where T : FileManagerDocument<R, S> where R : FileManagerFile {
        public T ActiveDocument { get; protected set; } = null;
        public R CurrentFile => ActiveDocument?.CurrentFile;

        protected readonly string WindowTitle;
        private readonly string Extension;
        private readonly string WorkspaceKey;
        private readonly string WorkspacePath;

        public readonly ManagerConfiguration Configuration;
        public readonly CopyManager Copy = new();

        private int DOC_ID = 0;
        public override string NewWriteLocation => Path.Combine( Plugin.Configuration.WriteLocation, $"{Id}Temp{DOC_ID++}.{Extension}" ).Replace( '\\', '/' );

        private readonly FileManagerDocumentWindow<T, R, S> DocumentWindow;
        public readonly List<T> Documents = new();

        public SelectDialog SourceSelect { get; protected set; }
        public SelectDialog ReplaceSelect { get; protected set; }

        public FileManager( string windowTitle, string id ) : this( windowTitle, id, id.ToLower(), id, id ) { }

        public FileManager( string windowTitle, string id, string extension, string workspaceKey, string workspacePath ) : base( windowTitle, id ) {
            WindowTitle = windowTitle;
            Extension = extension;
            WorkspaceKey = workspaceKey;
            WorkspacePath = workspacePath;
            Configuration = Plugin.Configuration.GetManagerConfig( Id );
            AddDocument();
            DocumentWindow = new( windowTitle, this );
        }

        public override CopyManager GetCopyManager() => Copy;
        public override CommandManager GetCommandManager() => CurrentFile?.Command;
        public override ManagerConfiguration GetConfig() => Configuration;

        // ===================

        public override void SetSource( SelectResult result ) {
            ActiveDocument?.SetSource( result );
            Plugin.Configuration.AddRecent( Configuration.RecentItems, result );
        }

        public override void ShowSource() => SourceSelect?.Show();

        public override void SetReplace( SelectResult result ) {
            ActiveDocument?.SetReplace( result );
            Plugin.Configuration.AddRecent( Configuration.RecentItems, result );
        }

        public override void ShowReplace() => ReplaceSelect?.Show();

        private void CheckKeybinds() {
            if( !ImGui.IsWindowFocused( ImGuiFocusedFlags.RootAndChildWindows ) ) return;
            if( Plugin.Configuration.UpdateKeybind.KeyPressed() ) ActiveDocument?.Update();
            ActiveDocument?.CheckKeybinds();
        }

        // ====================

        public override void Unsaved() {
            if( ActiveDocument != null ) ActiveDocument.Unsaved = true;
        }

        protected abstract T GetNewDocument();

        public void AddDocument() {
            var newDocument = GetNewDocument();
            ActiveDocument = newDocument;
            Documents.Add( newDocument );
        }

        public void SelectDocument( T document ) {
            ActiveDocument = document;
        }

        public bool RemoveDocument( T document ) {
            var switchDoc = ( document == ActiveDocument );
            Documents.Remove( document );
            Plugin.CleanupExport( document );

            if( switchDoc ) {
                ActiveDocument = Documents[0];
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
                ActiveDocument = newDocument;
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

        public bool GetReplacePath( string path, out string replacePath ) => IFileManager.GetReplacePath( this, path, out replacePath );

        public bool DoDebug( string path ) => path.Contains( $".{Extension}" );

        public void ToDefault() {
            Dispose();
            AddDocument();
        }

        public virtual void Dispose() {
            Documents.ForEach( x => x.Dispose() );
            Documents.Clear();
            SourceSelect?.Hide();
            ReplaceSelect?.Hide();
            ActiveDocument = null;
            DOC_ID = 0;
        }
    }
}
