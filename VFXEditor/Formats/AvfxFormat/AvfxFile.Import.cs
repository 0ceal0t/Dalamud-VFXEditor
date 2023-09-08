using System.Collections.Generic;
using System.IO;
using VfxEditor.Ui.Nodes;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat {
    public partial class AvfxFile {
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

        public void Import( string path ) {
            var ext = Path.GetExtension( path );
            using var reader = new BinaryReader( File.Open( path, FileMode.Open ) );

            if( ext == ".vfxedit" ) { // OLD METHOD
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

        public void Import( BinaryReader reader, int size, bool hasDependencies, List<string> renames ) {
            NodeGroupSet.PreImport( hasDependencies );

            List<NodePosition> models = new();
            List<NodePosition> textures = new();
            List<NodePosition> binders = new();
            List<NodePosition> effectors = new();
            List<NodePosition> particles = new();
            List<NodePosition> emitters = new();
            List<NodePosition> timelines = new();

            var idx = 0;
            AvfxBase.ReadNested( reader, ( BinaryReader _reader, string _name, int _size ) => {
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

            var importCommand = new CompoundCommand();
            // Import items in a specific order
            ImportGroup( models, reader, ModelView, NodeGroupSet.Models, importCommand );
            ImportGroup( textures, reader, TextureView, NodeGroupSet.Textures, importCommand );
            ImportGroup( binders, reader, BinderView, NodeGroupSet.Binders, importCommand );
            ImportGroup( effectors, reader, EffectorView, NodeGroupSet.Effectors, importCommand );
            ImportGroup( particles, reader, ParticleView, NodeGroupSet.Particles, importCommand );
            ImportGroup( emitters, reader, EmitterView, NodeGroupSet.Emitters, importCommand );
            ImportGroup( timelines, reader, TimelineView, NodeGroupSet.Timelines, importCommand );

            CommandManager.Avfx.Add( importCommand ); // doesn't actually execute anything

            NodeGroupSet.PostImport();
        }

        private static void ImportGroup<T>( List<NodePosition> positions, BinaryReader reader, IUiNodeView<T> view, NodeGroup<T> group, CompoundCommand command ) where T : AvfxNode {
            foreach( var pos in positions ) {
                reader.BaseStream.Seek( pos.Position, SeekOrigin.Begin );
                var item = view.Read( reader, pos.Size );
                if( !string.IsNullOrEmpty( pos.Renamed ) ) item.Renamed = pos.Renamed;
                group.AddAndUpdate( item ); // triggers Idx update as well
                command.Add( new UiNodeViewAddCommand<T>( view, group, item ) );
            }
        }
    }
}
