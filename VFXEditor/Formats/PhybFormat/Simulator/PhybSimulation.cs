using HelixToolkit.SharpDX.Animations;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.Formats.PhybFormat.Simulator;
using VfxEditor.PhybFormat.Utils;

namespace VfxEditor.PhybFormat.Simulator {
    public class PhybSimulation : IPhysicsObject {
        public readonly PhybFile File;

        public readonly List<PhybSimulator> Simulators = [];

        private readonly PhybSimulatorDropdown SimulatorDropdown;

        public PhybSimulation( PhybFile file, BinaryReader reader, bool isEmpty ) {
            File = file;

            if( !isEmpty ) {
                var startPos = reader.BaseStream.Position;
                var numSimulators = reader.ReadUInt32();
                for( var i = 0; i < numSimulators; i++ ) Simulators.Add( new PhybSimulator( file, reader, startPos ) );
            }

            SimulatorDropdown = new( file, this, Simulators );
        }

        public void Write( SimulationWriter writer ) => writer.Write( Simulators );

        public void Draw() {
            SimulatorDropdown.Draw();
        }

        public void AddPhysicsObjects( MeshBuilders meshes, Dictionary<string, Bone> boneMatrixes ) {
            foreach( var item in Simulators ) item.AddPhysicsObjects( meshes, boneMatrixes );
        }
    }
}
