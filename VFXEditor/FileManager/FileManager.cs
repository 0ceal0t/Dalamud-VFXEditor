using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Data;
using VfxEditor.Data.Command;
using VfxEditor.Data.Copy;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Select;
using VfxEditor.Ui.Export;
using VfxEditor.Utils;

namespace VfxEditor.FileManager {
    public abstract partial class FileManager<D, F, S> : FileManagerBase, IFileManager where D : FileManagerDocument<F, S> where F : FileManagerFile {
        public D ActiveDocument { get; protected set; }
        public F? ActiveFile => ActiveDocument?.File;

        private int DOC_ID = 0;
        public override string NewWriteLocation => Path.Combine( Plugin.Configuration.WriteLocation, $"{FormatName}Temp{DOC_ID++}.{Extension}" ).Replace( '\\', '/' );

        private readonly FileManagerDocumentWindow<D, F, S> DocumentWindow;
        public readonly List<D> Documents = [];

        private bool UseWindowColor = false;
        private Vector4 WindowColor = new( 1, 0, 0, 1 ); // TODO: pick a random one

        public FileManager( FileManagerGroupBase group ) : base( group ) {
            DocumentWindow = new( Title, this );
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

        protected abstract D GetNewDocument();

        public void AddDocument() {
            ActiveDocument = GetNewDocument();
            Documents.Add( ActiveDocument );
        }

        public void SelectDocument( D document ) {
            ActiveDocument = document;
        }

        public void RemoveDocument( D document, bool dispose = true ) {
            if( !Documents.Contains( document ) ) return;
            DocumentWindow.Reset();

            Documents.Remove( document );
            if( dispose ) document.Dispose();
            ExportDialog.RemoveDocument( document );

            if( ActiveDocument == document ) ActiveDocument = Documents.Count > 0 ? Documents[0] : null;

            if( ActiveDocument == null ) {
                SourceSelect?.Hide();
                ReplaceSelect?.Hide();
            }
        }

        public void MoveDocumentAfter( D document, D targetDocument ) {
            if( !Documents.Contains( targetDocument ) ) return;
            Documents.Remove( document );
            Documents.Insert( Documents.IndexOf( targetDocument ), document );
        }

        public void InsertDocument( D document, int idx ) {
            if( Documents.Contains( document ) ) return;
            Documents.Insert( idx, document );
            ActiveDocument = document;
        }

        public void InsertDocument( D document ) {
            if( Documents.Contains( document ) ) return;
            Documents.Add( document );
            ActiveDocument = document;
        }

        public IEnumerable<IFileDocument> GetDocuments() => Documents;

        public void WorkspaceImport( S item, string loadLocation, string path ) {
            var newDocument = GetWorkspaceDocument( item, Path.Combine( loadLocation, path ) );
            ActiveDocument = newDocument;
            Documents.Add( newDocument );
        }

        protected abstract D GetWorkspaceDocument( S data, string localPath );

        public virtual void Reset( bool pluginClosing ) {
            Documents.ForEach( x => x.Dispose() );
            Documents.Clear();
            SourceSelect?.Hide();
            ReplaceSelect?.Hide();

            ActiveDocument = null;
            DocumentWindow.Reset();
        }

        public bool CanImport( string path ) => path.EndsWith( Extension );

        public void PenumbraImport( SelectResult selectedFile, SelectResult replacedFile ) {
            SetSource( selectedFile );
            SetReplace( replacedFile );
            AddDocument();
        }

        public override WorkspaceWindow ToMeta() => base.ToMeta() with {
            UseWindowColor = UseWindowColor,
            WindowColor = WindowColor
        };

        public override void SetMeta( WorkspaceWindow? meta ) {
            base.SetMeta( meta );
            UseWindowColor = meta?.UseWindowColor ?? UseWindowColor;
            WindowColor = meta?.WindowColor ?? WindowColor;
        }
    }
}
