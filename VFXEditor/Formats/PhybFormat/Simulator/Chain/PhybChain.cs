using HelixToolkit.SharpDX.Core.Animations;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Parsing;
using VfxEditor.PhybFormat.Simulator.CollisionData;
using VfxEditor.PhybFormat.Utils;
using VfxEditor.Ui.Components.SplitViews;

namespace VfxEditor.PhybFormat.Simulator.Chain {
    public enum ChainType {
        Sphere,
        Capsule
    }

    public class PhybChain : PhybData, IPhysicsObject {
        public readonly PhybSimulator Simulator;

        public readonly ParsedFloat Dampening = new( "Dampening" );
        public readonly ParsedFloat MaxSpeed = new( "Max Speed" );
        public readonly ParsedFloat Friction = new( "Friction" );
        public readonly ParsedFloat CollisionDampening = new( "Collision Dampening" );
        public readonly ParsedFloat RepulsionStrength = new( "Repulsion Strength" );
        public readonly ParsedFloat3 LastBoneOffset = new( "Last Bone Offset" );
        public readonly ParsedEnum<ChainType> Type = new( "Type" );

        public readonly List<PhybCollisionData> Collisions = [];
        public readonly List<PhybNode> Nodes = [];

        private readonly CommandSplitView<PhybCollisionData> CollisionSplitView;
        private readonly CommandSplitView<PhybNode> NodeSplitView;

        public PhybChain( PhybFile file, PhybSimulator simulator ) : base( file ) {
            Simulator = simulator;

            CollisionSplitView = new( "Collision Object", Collisions, false,
                null, () => new( file, simulator ), ( PhybCollisionData _, bool _ ) => File.OnChange() );

            NodeSplitView = new( "Node", Nodes, false,
                ( PhybNode item, int idx ) => $"Node {idx + 1}", () => new( file, simulator ), ( PhybNode _, bool _ ) => File.OnChange() );
        }

        public PhybChain( PhybFile file, PhybSimulator simulator, BinaryReader reader, long simulatorStartPos ) : this( file, simulator ) {
            var numCollisions = reader.ReadUInt16();
            var numNodes = reader.ReadUInt16();

            foreach( var parsed in Parsed ) parsed.Read( reader );

            var collisionOffset = reader.ReadUInt32();
            var nodeOffset = reader.ReadUInt32();

            var resetPos = reader.BaseStream.Position;

            reader.BaseStream.Position = simulatorStartPos + collisionOffset + 4;
            for( var i = 0; i < numCollisions; i++ ) Collisions.Add( new PhybCollisionData( file, simulator, reader ) );

            reader.BaseStream.Position = simulatorStartPos + nodeOffset + 4;
            for( var i = 0; i < numNodes; i++ ) Nodes.Add( new PhybNode( file, simulator, reader ) );

            reader.BaseStream.Position = resetPos;
        }

        protected override List<ParsedBase> GetParsed() => [
            Dampening,
            MaxSpeed,
            Friction,
            CollisionDampening,
            RepulsionStrength,
            LastBoneOffset,
            Type,
        ];

        public override void Draw() {
            using var _ = ImRaii.PushId( "Chain" );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Parameters" ) ) {
                if( tab ) {
                    using var child = ImRaii.Child( "Child", new Vector2( -1 ), false );
                    foreach( var parsed in Parsed ) parsed.Draw();
                }
            }

            using( var tab = ImRaii.TabItem( "Collision Objects" ) ) {
                if( tab ) CollisionSplitView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Nodes" ) ) {
                if( tab ) NodeSplitView.Draw();
            }
        }

        public override void Write( BinaryWriter writer ) { }

        public override void Write( SimulationWriter writer ) {
            writer.Write( ( ushort )Collisions.Count );
            writer.Write( ( ushort )Nodes.Count );

            foreach( var parsed in Parsed ) parsed.Write( writer );

            if( Collisions.Count == 0 ) writer.Write( 0 );
            else {
                writer.WritePlaceholder( writer.ExtraWriter.BaseStream.Position - 4 );
            }

            foreach( var item in Collisions ) item.Write( writer.ExtraWriter );

            if( Nodes.Count == 0 ) writer.Write( 0 );
            else {
                writer.WritePlaceholder( writer.ExtraWriter.BaseStream.Position - 4 );
            }

            foreach( var item in Nodes ) item.Write( writer.ExtraWriter );
        }

        public void AddPhysicsObjects( MeshBuilders meshes, Dictionary<string, Bone> boneMatrixes ) {
            foreach( var item in Collisions ) item.AddPhysicsObjects( meshes, boneMatrixes );
            foreach( var item in Nodes ) item.AddPhysicsObjects( meshes, boneMatrixes );
        }
    }
}
