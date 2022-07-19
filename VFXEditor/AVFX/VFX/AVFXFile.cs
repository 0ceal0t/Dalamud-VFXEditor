using Dalamud.Logging;
using ImGuiFileDialog;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Linq;
using System.Text;
using VFXEditor.AVFXLib;
using VFXEditor.Helper;

namespace VFXEditor.AVFX.VFX {
    public class AVFXFile {
        public readonly AVFXMain Avfx;

        public readonly UIParameterView ParameterView;
        public readonly UIEffectorView EffectorView;
        public readonly UIEmitterView EmitterView;
        public readonly UIModelView ModelView;
        public readonly UIParticleView ParticleView;
        public readonly UITextureView TextureView;
        public readonly UITimelineView TimelineView;
        public readonly UIScheduleView ScheduleView;
        public readonly UIBinderView BinderView;

        public readonly UINodeGroupSet NodeGroupSet;

        public readonly ExportDialog ExportUI;

        public bool Verified { get; private set; } = true;

        public AVFXFile( BinaryReader reader, bool checkOriginal = true ) {
            var startPos = reader.BaseStream.Position;

            byte[] original = null;
            if( checkOriginal ) {
                original = reader.ReadBytes( ( int )reader.BaseStream.Length );
                reader.BaseStream.Seek( startPos, SeekOrigin.Begin );
            }

            Avfx = AVFXMain.FromStream( reader );

            if( checkOriginal ) {
                using var ms = new MemoryStream();
                using var writer = new BinaryWriter( ms );
                Avfx.Write( writer );

                var newData = ms.ToArray();

                for( var i = 0; i < Math.Min( newData.Length, original.Length ); i++ ) {
                    if( newData[i] != original[i] ) {
                        PluginLog.Log( $"Warning: files do not match at {i} {newData[i]} {original[i]}" );
                        Verified = false;
                        break;
                    }
                }
            }

            // ======================

            NodeGroupSet = new( Avfx );

            ParameterView = new UIParameterView( Avfx );

            ParticleView = new UIParticleView( this, Avfx, NodeGroupSet.Particles );
            BinderView = new UIBinderView( this, Avfx, NodeGroupSet.Binders );
            EmitterView = new UIEmitterView( this, Avfx, NodeGroupSet.Emitters );
            EffectorView = new UIEffectorView( this, Avfx, NodeGroupSet.Effectors );
            TimelineView = new UITimelineView( this, Avfx, NodeGroupSet.Timelines );
            TextureView = new UITextureView( this, Avfx, NodeGroupSet.Textures );
            ModelView = new UIModelView( this, Avfx, NodeGroupSet.Models );
            ScheduleView = new UIScheduleView( this, Avfx, NodeGroupSet.Schedulers );

            NodeGroupSet.Init();

            ExportUI = new ExportDialog( this );
        }

        public void Draw() {
            if( ImGui.BeginTabBar( "##MainTabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) ) {
                if( ImGui.BeginTabItem( "Parameters##Main" ) ) {
                    ParameterView.Draw();
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Scheduler##Main" ) ) {
                    ScheduleView.Draw();
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Timelines##Main" ) ) {
                    TimelineView.Draw();
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Emitters##Main" ) ) {
                    EmitterView.Draw();
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Particles##Main" ) ) {
                    ParticleView.Draw();
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Effectors##Main" ) ) {
                    EffectorView.Draw();
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Binders##Main" ) ) {
                    BinderView.Draw();
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Textures##Main" ) ) {
                    TextureView.Draw();
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Models##Main" ) ) {
                    ModelView.Draw();
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

        public void AddToNodeLibrary( UINode node ) {
            var newId = UIHelper.RandomString( 12 );
            var newPath = AVFXManager.NodeLibrary.GetPath( newId );
            Export( node, newPath, true );
            AVFXManager.NodeLibrary.Add( node.GetText(), newId, newPath );
        }

        public void Export( UINode node, string path, bool exportDependencies) => Export( new List<UINode> { node }, path, exportDependencies );

        public void Export( List<UINode> nodes, string path, bool exportDependencies ) {
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
                FileHelper.WriteString( writer, string.IsNullOrEmpty(renamed) ? "" : renamed, true );
            }
            var finalPos = writer.BaseStream.Position;

            // go back and fill out placeholders
            writer.BaseStream.Seek( placeholderPos, SeekOrigin.Begin );
            writer.Write( numberOfItems );
            writer.Write( (int) size );
            writer.Write( (int) renamedOffset );
            writer.BaseStream.Seek( finalPos, SeekOrigin.Begin );
        }

        private List<UINode> ExportDependencies( List<UINode> startNodes, BinaryWriter bw ) {
            var visited = new HashSet<UINode>();
            var nodes = new List<UINode>();
            foreach( var startNode in startNodes ) {
                RecurseChild( startNode, nodes, visited );
            }

            var IdxSave = new Dictionary<UINode, int>(); // save these to restore afterwards, since we don't want to modify the current document
            foreach( var n in nodes ) {
                IdxSave[n] = n.Idx;
            }

            OrderByType<UITimeline>( nodes );
            OrderByType<UIEmitter>( nodes );
            OrderByType<UIEffector>( nodes );
            OrderByType<UIBinder>( nodes );
            OrderByType<UIParticle>( nodes );
            OrderByType<UITexture>( nodes );
            OrderByType<UIModel>( nodes );

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

        private void RecurseChild( UINode node, List<UINode> output, HashSet<UINode> visited ) {
            if( visited.Contains( node ) ) return; // prevents infinite loop
            visited.Add( node );

            foreach( var n in node.Children ) {
                RecurseChild( n, output, visited );
            }
            if( output.Contains( node ) ) return; // make sure elements get added AFTER their children. This doesn't work otherwise, since we want each node to be AFTER its dependencies
            output.Add( node );
        }

        private static void OrderByType<T>( List<UINode> items ) where T : UINode {
            var i = 0;
            foreach( var node in items ) {
                if( node is T ) {
                    node.Idx = i;
                    i++;
                }
            }
        }

        private static void UpdateAllNodes( List<UINode> nodes ) {
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
                    renames.Add( FileHelper.ReadString( reader ) );
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

        private static void ImportGroup<T>(List<NodePosition> positions, BinaryReader reader, IUINodeView<T> view, bool hasDependencies) where T : UINode {
            foreach( var pos in positions ) {
                view.Import( reader, pos.Position, pos.Size, pos.Renamed, hasDependencies );
            }
        }

        // =====================

        public void ShowExportDialog( UINode node ) => ExportUI.ShowDialog( node );

        public void ShowImportDialog() {
            FileDialogManager.OpenFileDialog( "Select a File", "Partial VFX{.vfxedit2,.vfxedit},.*", ( bool ok, string res ) => {
                if( !ok ) return;
                try {
                    Import( res );
                }
                catch( Exception e ) {
                    PluginLog.Error( "Could not import data", e );
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
