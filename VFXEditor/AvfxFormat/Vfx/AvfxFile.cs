using Dalamud.Logging;
using ImGuiFileDialog;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Linq;
using System.Text;
using VfxEditor.AVFXLib;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat.Vfx {
    public class AvfxFile {
        public readonly AVFXMain Avfx;
        public readonly CommandManager Command = new();

        public readonly UiParameterView ParameterView;
        public readonly UiEffectorView EffectorView;
        public readonly UiEmitterView EmitterView;
        public readonly UiModelView ModelView;
        public readonly UiParticleView ParticleView;
        public readonly UiTextureView TextureView;
        public readonly UiTimelineView TimelineView;
        public readonly UiScheduleView ScheduleView;
        public readonly UiBinderView BinderView;

        public readonly UiNodeGroupSet NodeGroupSet;

        public readonly ExportDialog ExportUI;

        private readonly bool Verified = true;
        public bool IsVerified => Verified;

        public AvfxFile( BinaryReader reader, bool checkOriginal = true ) {
            byte[] original = null;
            if( checkOriginal ) {
                var startPos = reader.BaseStream.Position;
                original = reader.ReadBytes( ( int )reader.BaseStream.Length );
                reader.BaseStream.Seek( startPos, SeekOrigin.Begin );
            }

            Avfx = AVFXMain.FromStream( reader );

            if( checkOriginal ) {
                using var ms = new MemoryStream();
                using var writer = new BinaryWriter( ms );
                Avfx.Write( writer );

                Verified = FileUtils.CompareFiles( original, ms.ToArray(), out var _ );
            }

            // ======================

            NodeGroupSet = new( Avfx );

            ParameterView = new UiParameterView( Avfx );

            ParticleView = new UiParticleView( this, Avfx, NodeGroupSet.Particles );
            BinderView = new UiBinderView( this, Avfx, NodeGroupSet.Binders );
            EmitterView = new UiEmitterView( this, Avfx, NodeGroupSet.Emitters );
            EffectorView = new UiEffectorView( this, Avfx, NodeGroupSet.Effectors );
            TimelineView = new UiTimelineView( this, Avfx, NodeGroupSet.Timelines );
            TextureView = new UiTextureView( this, Avfx, NodeGroupSet.Textures );
            ModelView = new UiModelView( this, Avfx, NodeGroupSet.Models );
            ScheduleView = new UiScheduleView( this, Avfx, NodeGroupSet.Schedulers );

            NodeGroupSet.Init();

            ExportUI = new ExportDialog( this );
        }

        public void Draw() {
            if( ImGui.BeginTabBar( "##MainTabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) ) {
                if( ImGui.BeginTabItem( "Parameters##Main" ) ) {
                    ParameterView.DrawInline();
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Scheduler##Main" ) ) {
                    ScheduleView.DrawInline();
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Timelines##Main" ) ) {
                    TimelineView.DrawInline();
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Emitters##Main" ) ) {
                    EmitterView.DrawInline();
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Particles##Main" ) ) {
                    ParticleView.DrawInline();
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Effectors##Main" ) ) {
                    EffectorView.DrawInline();
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Binders##Main" ) ) {
                    BinderView.DrawInline();
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Textures##Main" ) ) {
                    TextureView.DrawInline();
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Models##Main" ) ) {
                    ModelView.DrawInline();
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
            ExportUI.Draw();
        }

        public void Write( BinaryWriter writer ) => Avfx?.Write( writer );

        public void Dispose() => NodeGroupSet?.Dispose();

        // ========== WORKSPACE ===========

        public Dictionary<string, string> GetRenamingMap() => NodeGroupSet.GetRenamingMap();

        public void ReadRenamingMap( Dictionary<string, string> renamingMap ) => NodeGroupSet.ReadRenamingMap( renamingMap );

        // ========= EXPORT ==============

        public void AddToNodeLibrary( UiNode node ) {
            var newId = UiUtils.RandomString( 12 );
            var newPath = AvfxManager.NodeLibrary.GetPath( newId );
            Export( node, newPath, true );
            AvfxManager.NodeLibrary.Add( node.GetText(), newId, newPath );
        }

        public void Export( UiNode node, string path, bool exportDependencies) => Export( new List<UiNode> { node }, path, exportDependencies );

        public void Export( List<UiNode> nodes, string path, bool exportDependencies ) {
            using var writer = new BinaryWriter( File.Open( path, FileMode.Create ) );

            writer.Write( 2 ); // Magic :)
            writer.Write( exportDependencies );

            var placeholderPos = writer.BaseStream.Position;
            writer.Write( 0 ); // placeholder, number of items
            writer.Write( 0 ); // placeholder, data size
            writer.Write( 0 ); // placeholder, rename offset

            var finalNodes = nodes;
            var dataPos = writer.BaseStream.Position;
            if (exportDependencies) {
                finalNodes = ExportDependencies( nodes, writer );
            }
            else {
                // leave nodes as-is
                finalNodes.ForEach( n => n.Write( writer ) );
            }
            var size = writer.BaseStream.Position - dataPos;

            var renames = finalNodes.Select( n => n.Renamed );
            var renamedOffset = writer.BaseStream.Position;
            var numberOfItems = finalNodes.Count;

            foreach(var renamed in renames) {
                FileUtils.WriteString( writer, string.IsNullOrEmpty(renamed) ? "" : renamed, true );
            }
            var finalPos = writer.BaseStream.Position;

            // go back and fill out placeholders
            writer.BaseStream.Seek( placeholderPos, SeekOrigin.Begin );
            writer.Write( numberOfItems );
            writer.Write( (int) size );
            writer.Write( (int) renamedOffset );
            writer.BaseStream.Seek( finalPos, SeekOrigin.Begin );
        }

        private List<UiNode> ExportDependencies( List<UiNode> startNodes, BinaryWriter bw ) {
            var visited = new HashSet<UiNode>();
            var nodes = new List<UiNode>();
            foreach( var startNode in startNodes ) {
                RecurseChild( startNode, nodes, visited );
            }

            var IdxSave = new Dictionary<UiNode, int>(); // save these to restore afterwards, since we don't want to modify the current document
            foreach( var n in nodes ) {
                IdxSave[n] = n.Idx;
            }

            OrderByType<UiTimeline>( nodes );
            OrderByType<UiEmitter>( nodes );
            OrderByType<UiEffector>( nodes );
            OrderByType<UiBinder>( nodes );
            OrderByType<UiParticle>( nodes );
            OrderByType<UiTexture>( nodes );
            OrderByType<UiModel>( nodes );

            UpdateAllNodes( nodes );
            foreach( var n in nodes ) {
                n.Write( bw );
            }
            foreach( var n in nodes ) { // reset index
                n.Idx = IdxSave[n];
            }
            UpdateAllNodes( nodes );

            return nodes;
        }

        private void RecurseChild( UiNode node, List<UiNode> output, HashSet<UiNode> visited ) {
            if( visited.Contains( node ) ) return; // prevents infinite loop
            visited.Add( node );

            foreach( var n in node.Children ) {
                RecurseChild( n, output, visited );
            }
            if( output.Contains( node ) ) return; // make sure elements get added AFTER their children. This doesn't work otherwise, since we want each node to be AFTER its dependencies
            output.Add( node );
        }

        private static void OrderByType<T>( List<UiNode> items ) where T : UiNode {
            var i = 0;
            foreach( var node in items ) {
                if( node is T ) {
                    node.Idx = i;
                    i++;
                }
            }
        }

        private static void UpdateAllNodes( List<UiNode> nodes ) {
            foreach( var n in nodes ) {
                foreach( var s in n.Selectors ) {
                    s.UpdateNode();
                }
            }
        }

        public byte[] ToBytes() {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter( ms );
            Avfx.Write( writer );
            return ms.ToArray();
        }

        // ========= IMPORT ==============

        public void Import( string path ) {
            var ext = Path.GetExtension( path );
            using var reader = new BinaryReader( File.Open( path, FileMode.Open ) );

            if (ext == ".vfxedit") { // OLD METHOD
                var dataSize = reader.BaseStream.Length;

                if( dataSize < 8 ) return;
                reader.ReadInt32(); // first name
                var firstSize = reader.ReadInt32();
                var hasDependencies = dataSize > ( firstSize + 8 + 4 );

                reader.BaseStream.Seek( 0, SeekOrigin.Begin ); // reset position
                Import( reader, ( int )dataSize, hasDependencies, null );
            }
            else { // NEW METHOD
                reader.ReadInt32(); // magic
                var hasDependencies = reader.ReadBoolean();
                var numberOfItems = reader.ReadInt32();
                var dataSize = reader.ReadInt32();
                var renamedOffset = reader.ReadInt32();

                if( dataSize < 8 ) return;

                var dataOffset = reader.BaseStream.Position;
                reader.BaseStream.Seek( renamedOffset, SeekOrigin.Begin );
                List<string> renames = new();

                for( var i = 0; i < numberOfItems; i++ ) {
                    renames.Add( FileUtils.ReadString( reader ) );
                }

                reader.BaseStream.Seek( dataOffset, SeekOrigin.Begin );
                Import( reader, dataSize, hasDependencies, renames );
            }
        }

        private void Import( BinaryReader reader, int size, bool hasDependencies, List<string> renames ) {
            if( hasDependencies ) NodeGroupSet.PreImport();

            List<NodePosition> models = new();
            List<NodePosition> textures = new();
            List<NodePosition> binders = new();
            List<NodePosition> effectors = new();
            List<NodePosition> particles = new();
            List<NodePosition> emitters = new();
            List<NodePosition> timelines = new();

            var idx = 0;
            AVFXBase.ReadNested( reader, ( BinaryReader _reader, string _name, int _size ) => {
                var renamed = renames == null ? "" : renames[idx];
                switch( _name ) {
                    case "Modl":
                        models.Add( new NodePosition( _reader.BaseStream.Position, _size, renamed ) );
                        break;
                    case "Tex":
                        textures.Add( new NodePosition( _reader.BaseStream.Position, _size, renamed ) );
                        break;
                    case "Bind":
                        binders.Add( new NodePosition( _reader.BaseStream.Position, _size, renamed ) );
                        break;
                    case "Efct":
                        effectors.Add( new NodePosition( _reader.BaseStream.Position, _size, renamed ) );
                        break;
                    case "Ptcl":
                        particles.Add( new NodePosition( _reader.BaseStream.Position, _size, renamed ) );
                        break;
                    case "Emit":
                        emitters.Add( new NodePosition( _reader.BaseStream.Position, _size, renamed ) );
                        break;
                    case "TmLn":
                        timelines.Add( new NodePosition( _reader.BaseStream.Position, _size, renamed ) );
                        break;
                }
                _reader.ReadBytes( _size ); // skip it for now, we'll come back later
                idx++;
            }, size );

            // Import items in a specific order
            ImportGroup( models, reader, ModelView, hasDependencies );
            ImportGroup( textures, reader, TextureView, hasDependencies );
            ImportGroup( binders, reader, BinderView, hasDependencies );
            ImportGroup( effectors, reader, EffectorView, hasDependencies );
            ImportGroup( particles, reader, ParticleView, hasDependencies );
            ImportGroup( emitters, reader, EmitterView, hasDependencies );
            ImportGroup( timelines, reader, TimelineView, hasDependencies );
        }

        private static void ImportGroup<T>(List<NodePosition> positions, BinaryReader reader, IUiNodeView<T> view, bool hasDependencies) where T : UiNode {
            foreach( var pos in positions ) {
                view.Import( reader, pos.Position, pos.Size, pos.Renamed, hasDependencies );
            }
        }

        // =====================

        public void ShowExportDialog( UiNode node ) => ExportUI.ShowDialog( node );

        public void ShowImportDialog() {
            FileDialogManager.OpenFileDialog( "Select a File", "Partial VFX{.vfxedit2,.vfxedit},.*", ( bool ok, string res ) => {
                if( !ok ) return;
                try {
                    Import( res );
                }
                catch( Exception e ) {
                    PluginLog.Error( e, "Could not import data" );
                }
            } );
        }

        private struct NodePosition {
            public long Position;
            public int Size;
            public string Renamed;

            public NodePosition( long position, int size, string rename ) {
                Position = position;
                Size = size;
                Renamed = rename;
            }
        }
    }
}
