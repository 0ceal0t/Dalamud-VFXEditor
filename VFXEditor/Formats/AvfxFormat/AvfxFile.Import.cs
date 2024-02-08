using System.Collections.Generic;
using System.IO;
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
            using var reader = new BinaryReader( File.Open( path, FileMode.Open ) );

            if( Path.GetExtension( path ) == ".vfxedit" ) { // OLD METHOD
                var dataSize = reader.BaseStream.Length;

                if( dataSize < 8 ) return;
                reader.ReadInt32(); // first name
                var firstSize = reader.ReadInt32();
                var hasDependencies = dataSize > ( firstSize + 8 + 4 );

                reader.BaseStream.Position = 0; // reset position
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
                reader.BaseStream.Position = renamedOffset;
                List<string> renames = [];

                for( var i = 0; i < numberOfItems; i++ ) {
                    renames.Add( FileUtils.ReadString( reader ) );
                }

                reader.BaseStream.Position = dataOffset;
                Import( reader, dataSize, hasDependencies, renames );
            }
        }

        public void Import( BinaryReader reader, int size, bool hasDependencies, List<string> renames ) {
            NodeGroupSet.PreImport( hasDependencies );

            List<NodePosition> models = [];
            List<NodePosition> textures = [];
            List<NodePosition> binders = [];
            List<NodePosition> effectors = [];
            List<NodePosition> particles = [];
            List<NodePosition> emitters = [];
            List<NodePosition> timelines = [];

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

            var commands = new List<ICommand>();
            // Import items in a specific order
            ImportGroup( models, reader, ModelView, NodeGroupSet.Models, commands );
            ImportGroup( textures, reader, TextureView, NodeGroupSet.Textures, commands );
            ImportGroup( binders, reader, BinderView, NodeGroupSet.Binders, commands );
            ImportGroup( effectors, reader, EffectorView, NodeGroupSet.Effectors, commands );
            ImportGroup( particles, reader, ParticleView, NodeGroupSet.Particles, commands );
            ImportGroup( emitters, reader, EmitterView, NodeGroupSet.Emitters, commands );
            ImportGroup( timelines, reader, TimelineView, NodeGroupSet.Timelines, commands );

            Command.AddAndExecute( new CompoundCommand( commands ) ); // doesn't actually execute anything

            NodeGroupSet.PostImport();
        }

        private static void ImportGroup<T>( List<NodePosition> positions, BinaryReader reader, IUiNodeView<T> view, NodeGroup<T> group, List<ICommand> commands ) where T : AvfxNode {
            foreach( var pos in positions ) {
                reader.BaseStream.Position = pos.Position;
                var item = view.Read( reader, pos.Size );
                if( !string.IsNullOrEmpty( pos.Renamed ) ) item.Renamed = pos.Renamed;
                group.AddAndUpdate( item ); // triggers Idx update as well
                commands.Add( new AvfxNodeViewAddCommand<T>( view, group, item ) );
            }
        }
    }
}
