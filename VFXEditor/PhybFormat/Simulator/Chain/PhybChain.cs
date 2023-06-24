using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Animations;
using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.PhybFormat.Simulator.CollisionData;
using VfxEditor.PhybFormat.Utils;
using VfxEditor.Ui.Components;

namespace VfxEditor.PhybFormat.Simulator.Chain {
    public enum ChainType {
        Sphere,
        Capsule
    }

    public class PhybChain : PhybPhysicsData, IPhysicsObject {
        public readonly ParsedFloat Dampening = new( "Dampening" );
        public readonly ParsedFloat MaxSpeed = new( "Max Speed" );
        public readonly ParsedFloat Friction = new( "Friction" );
        public readonly ParsedFloat CollisionDampening = new( "Collision Dampening" );
        public readonly ParsedFloat RepulsionStrength = new( "Repulsion Strength" );
        public readonly ParsedFloat3 LastBoneOffset = new( "Last Bone Offset" );
        public readonly ParsedEnum<ChainType> Type = new( "Type" );

        public readonly List<PhybCollisionData> Collisions = new();
        public readonly List<PhybNode> Nodes = new();

        private readonly SimpleSplitview<PhybCollisionData> CollisionSplitView;
        private readonly SimpleSplitview<PhybNode> NodeSplitView;

        public PhybChain( PhybFile file ) : base( file ) {
            CollisionSplitView = new( "Collision Object", Collisions, false,
                null, () => new( file ), () => CommandManager.Phyb );

            NodeSplitView = new( "Node", Nodes, false,
                null, () => new( file ), () => CommandManager.Phyb );
        }

        public PhybChain( PhybFile file, BinaryReader reader, long simulatorStartPos ) : this( file ) {
            var numCollisions = reader.ReadUInt16();
            var numNodes = reader.ReadUInt16();

            foreach( var parsed in Parsed ) parsed.Read( reader );

            var collisionOffset = reader.ReadUInt32();
            var nodeOffset = reader.ReadUInt32();

            var resetPos = reader.BaseStream.Position;

            reader.BaseStream.Seek( simulatorStartPos + collisionOffset + 4, SeekOrigin.Begin );
            for( var i = 0; i < numCollisions; i++ ) Collisions.Add( new PhybCollisionData( file, reader ) );

            reader.BaseStream.Seek( simulatorStartPos + nodeOffset + 4, SeekOrigin.Begin );
            for( var i = 0; i < numNodes; i++ ) Nodes.Add( new PhybNode( file, reader ) );

            reader.BaseStream.Seek( resetPos, SeekOrigin.Begin );
        }

        protected override List<ParsedBase> GetParsed() => new() {
            Dampening,
            MaxSpeed,
            Friction,
            CollisionDampening,
            RepulsionStrength,
            LastBoneOffset,
            Type,
        };

        public override void Draw() {
            using var _ = ImRaii.PushId( "Chain" );

            foreach( var parsed in Parsed ) parsed.Draw( CommandManager.Phyb );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

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

        public void AddPhysicsObjects( MeshBuilder builder, Dictionary<string, Bone> boneMatrixes ) {

        }
    }
}
