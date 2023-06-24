using HelixToolkit.SharpDX.Core.Animations;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.PhybFormat.Simulator.PostAlignment;
using VfxEditor.PhybFormat.Utils;
using VfxEditor.Ui.Components;

namespace VfxEditor.PhybFormat.Simulator {
    public class PhybSimulation : IPhysicsObject {
        public readonly PhybFile File;

        public readonly List<PhybSimulator> Simulators = new();

        private readonly SimpleDropdown<PhybSimulator> SimulatorDropdown;

        public PhybSimulation( PhybFile file, BinaryReader reader, bool isEmpty ) {
            File = file;

            if( !isEmpty ) {
                var startPos = reader.BaseStream.Position;
                var numSimulators = reader.ReadUInt32();
                for( var i = 0; i < numSimulators; i++ ) Simulators.Add( new PhybSimulator( file, reader, startPos ) );
            }

            SimulatorDropdown = new( "Simulator", Simulators,
                null, () => new PhybSimulator( File ), () => CommandManager.Phyb );
        }

        public void Write( SimulationWriter writer ) {
            writer.Write( Simulators.Count );

            var placeholders = Simulators.Select( x => x.WriteHeader( writer ) ).ToList();
            var offsets = new List<List<int>> {
                Simulators.Select( x => WriteList( x.Collisions, writer ) ).ToList(),
                Simulators.Select( x => WriteList( x.CollisionConnectors, writer ) ).ToList(),
                Simulators.Select( x => WriteList( x.Chains, writer ) ).ToList(),
                Simulators.Select( x => WriteList( x.Connectors, writer ) ).ToList(),
                Simulators.Select( x => WriteList( x.Attracts, writer ) ).ToList(),
                Simulators.Select( x => WriteList( x.Pins, writer ) ).ToList(),
                Simulators.Select( x => WriteList( x.Springs, writer ) ).ToList(),
                Simulators.Select( x => WriteList( x.PostAlignments, writer ) ).ToList(),
            };

            var resetPos = writer.Position;

            for( var i = 0; i < placeholders.Count; i++ ) {
                writer.Seek( placeholders[i] );
                for( var j = 0; j < offsets.Count; j++ ) {
                    writer.Write( offsets[j][i] );
                }
            }

            writer.Seek( resetPos );
        }

        public void Draw() {
            SimulatorDropdown.Draw();
        }

        private static int WriteList<T>( List<T> items, SimulationWriter writer ) where T : PhybData {
            var offset = writer.Position;
            foreach( var item in items ) item.Write( writer );
            var defaultOffset = typeof( T ) == typeof( PhybPostAlignment ) ? 0xCCCCCCCC : 0;
            return items.Count == 0 ? ( int )defaultOffset : ( int )offset - 4;
        }

        public void AddPhysicsObjects( MeshBuilders meshes, Dictionary<string, Bone> boneMatrixes ) {
            foreach( var item in Simulators ) item.AddPhysicsObjects( meshes, boneMatrixes );
        }
    }
}
