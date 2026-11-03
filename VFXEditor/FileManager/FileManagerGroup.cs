using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Interop.Penumbra;
using VfxEditor.Select;
using VfxEditor.Utils;

namespace VfxEditor.FileManager {
    public abstract class FileManagerGroup<M, D, F, S> : FileManagerGroupBase, IFileManagerGroup where M : FileManager<D, F, S> where D : FileManagerDocument<F, S> where F : FileManagerFile {
        public readonly List<M> Managers = [];
        public List<D> Documents => [.. Managers.SelectMany( x => x.Documents )];

        public M LastFocusedManager { get; protected set; } = null;

        private (M, D)? Dragging = null;

        public FileManagerGroup( string title, string formatName ) :
            this( title, formatName, formatName.ToLower(), formatName, formatName ) { }

        public FileManagerGroup( string title, string formatName, string extension, string workspaceKey, string workspacePath ) :
            base( title, formatName, extension, workspaceKey, workspacePath ) {

            LastFocusedManager = AddManager();
        }

        protected abstract M GetNewManager();

        public M AddManager() {
            var newManager = GetNewManager();
            Managers.Add( newManager );
            return newManager;
        }

        public override void NewWindow() => AddManager().Show();

        public override void ToNewWindow( FileManagerBase currentManager, IFileDocument document ) {
            if( ( currentManager is M m ) && ( document is D d ) ) {
                m.RemoveDocument( d, dispose: false );
                var newManager = AddManager();
                newManager.Show();
                newManager.InsertDocument( d );
            }
        }

        public override void AddDefaultDocument() {
            Managers[0].AddDocument();
        }

        public void RemoveManager( M manager ) {
            if( Managers.Count == 1 ) return; // can't remove the last manager
            if( LastFocusedManager == manager ) LastFocusedManager = null;
            manager.Reset( false );
            Managers.Remove( manager );
            WindowSystem.RemoveWindow( manager );

            PenumbraTempMod.MarkDirty();
        }

        public IEnumerable<IFileDocument> GetDocuments() => Managers.Select( x => x.GetDocuments() ).SelectMany( x => x );

        // ======================

        public void WorkspaceImport( JObject meta, string loadLocation ) {
            var items = WorkspaceUtils.ReadFromMeta<S>( meta, WorkspaceKey );
            if( items == null || items.Length == 0 ) return;


            foreach( var item in items ) {
                var windowIdx = item switch {
                    WorkspaceMetaBasic basic => basic.WindowIndex,
                    WorkspaceMetaRenamed renamed => renamed.WindowIndex,
                    _ => null
                } ?? 0;
                while( windowIdx >= Managers.Count ) NewWindow();
                Managers[windowIdx].WorkspaceImport( item, loadLocation, WorkspacePath );
            }

            var windows = WorkspaceUtils.GetWindowData( meta, WorkspaceKey );
            while( ( windows?.Length - 1 ) >= Managers.Count ) NewWindow();

            foreach( var (manager, idx) in Managers.WithIndex() ) {
                manager.Show();
                manager.SetMeta( windows?[idx] );
            }
        }

        public void WorkspaceExport( Dictionary<string, string> meta, string saveLocation, Dictionary<string, WorkspaceWindow[]> windows ) {
            var rootPath = Path.Combine( saveLocation, WorkspacePath );
            Directory.CreateDirectory( rootPath );

            List<S> documentMeta = [];

            var idx = 0;
            foreach( var (manager, managerIdx) in Managers.WithIndex() ) {
                foreach( var document in manager.Documents ) {
                    document.WorkspaceExport( documentMeta, rootPath, $"{FormatName}Temp{idx}.{Extension}", managerIdx );
                    idx++;
                }
            }
            WorkspaceUtils.WriteToMeta( meta, documentMeta.ToArray(), WorkspaceKey );

            windows[WorkspaceKey] = [.. Managers.Select( x => x.ToMeta() )];
        }

        // ====================

        public bool DoDebug( string path ) => path.Contains( $".{Extension}" );

        public bool FileExists( string path ) => IFileManagerGroup.FileExist( this, path );

        public bool GetReplacePath( string path, out string replacePath ) => IFileManagerGroup.GetReplacePath( this, path, out replacePath );

        public override void SetLastFocusedManager( FileManagerBase manager ) {
            if( manager is M m ) LastFocusedManager = m;
        }

        public virtual void Reset( bool pluginClosing ) {
            Dragging = null;
            WindowId = 0;
            Managers.ForEach( x => x.Reset( pluginClosing ) );
            Managers.Clear();
            WindowSystem.RemoveAllWindows();
            LastFocusedManager = AddManager();
        }

        public bool AnyWindowsOpen() => Managers.Any( x => x.IsOpen );

        public void Show() {
            Managers.ForEach( x => x.Show() );
        }

        public override void Draw() => WindowSystem.Draw();

        public override void DrawCloseWindow( FileManagerBase manager ) {
            using var disable = ImRaii.Disabled( Managers.Count == 1 );
            if( ImGui.MenuItem( "Close Window" ) ) {
                if( Managers.Contains( manager ) ) RemoveManager( ( M )manager );
            }
        }

        public override unsafe bool DrawDragDrop( FileManagerBase _manager, IFileDocument? _document, string? targetDocumentName ) {
            // Allow nullable document to, for example, create a target in windows that have no documents
            if( ( _manager is M targetManager ) && ( _document is D || _document == null ) ) {
                var targetDocument = ( _document is D d ) ? d : null;

                if( targetDocument != null ) {
                    if( ImGui.BeginDragDropSource( ImGuiDragDropFlags.None ) ) {
                        ImGui.SetDragDropPayload( $"{Title}-DRAGDROP", null, 0 );
                        Dragging = (targetManager, targetDocument);
                        ImGui.Text( targetDocumentName ?? "..." );
                        ImGui.EndDragDropSource();
                    }
                }

                if( ImGui.BeginDragDropTarget() ) {
                    if( Dragging != null && ImGui.AcceptDragDropPayload( $"{Title}-DRAGDROP" ).Handle != null ) {
                        var sourceManager = Dragging!.Value.Item1;
                        var sourceDocument = Dragging!.Value.Item2;

                        if( sourceDocument != targetDocument ) {
                            if( sourceManager == targetManager && targetDocument != null ) { // Re-ordering
                                targetManager.MoveDocumentAfter( sourceDocument, targetDocument );
                            }
                            else { // Moving it to another manager
                                sourceManager.RemoveDocument( sourceDocument, dispose: false );
                                if( targetDocument == null ) {
                                    targetManager.InsertDocument( sourceDocument );
                                }
                                else {
                                    targetManager.MoveDocumentAfter( sourceDocument, targetDocument );
                                }
                            }
                        }

                        Dragging = null;
                        return true;
                    }
                    ImGui.EndDragDropTarget();
                }
            }
            return false;
        }

        public bool CanImport( string path ) {
            return LastFocusedManager.CanImport( path );
        }

        public void PenumbraImport( SelectResult selectedFile, SelectResult replacedFile ) {
            LastFocusedManager.PenumbraImport( selectedFile, replacedFile );
        }
    }
}