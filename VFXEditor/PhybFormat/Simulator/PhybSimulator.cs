using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Animations;
using ImGuiNET;
using OtterGui.Raii;
using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.PhybFormat.Simulator.Attract;
using VfxEditor.PhybFormat.Simulator.Chain;
using VfxEditor.PhybFormat.Simulator.CollisionData;
using VfxEditor.PhybFormat.Simulator.Connector;
using VfxEditor.PhybFormat.Simulator.Pin;
using VfxEditor.PhybFormat.Simulator.PostAlignment;
using VfxEditor.PhybFormat.Simulator.Spring;
using VfxEditor.PhybFormat.Utils;
using VfxEditor.Ui.Components;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.PhybFormat.Simulator {
    public class PhybSimulator : IUiItem, IPhysicsObject {
        public readonly PhybFile File;

        public readonly PhybSimulatorParams Params;

        public readonly List<PhybCollisionData> Collisions = new();
        public readonly List<PhybCollisionData> CollisionConnectors = new();
        public readonly List<PhybChain> Chains = new();
        public readonly List<PhybConnector> Connectors = new();
        public readonly List<PhybAttract> Attracts = new();
        public readonly List<PhybPin> Pins = new();
        public readonly List<PhybSpring> Springs = new();
        public readonly List<PhybPostAlignment> PostAlignments = new();

        private readonly SimpleSplitview<PhybCollisionData> CollisionSplitView;
        private readonly SimpleSplitview<PhybCollisionData> CollisionConnectorSplitView;
        private readonly SimpleDropdown<PhybChain> ChainDropdown;
        private readonly SimpleSplitview<PhybConnector> ConnectorSplitView;
        private readonly SimpleSplitview<PhybAttract> AttractSplitView;
        private readonly SimpleSplitview<PhybPin> PinSplitView;
        private readonly SimpleSplitview<PhybSpring> SpringSplitView;
        private readonly SimpleSplitview<PhybPostAlignment> PostAlignmentSplitView;

        public PhybSimulator( PhybFile file ) {
            File = file;
            Params = new( file );

            CollisionSplitView = new( "Collision Object", Collisions, false,
                ( PhybCollisionData item, int idx ) => item.CollisionName.Value, () => new( File, this ),
                () => CommandManager.Phyb, ( PhybCollisionData item ) => File.Updated() );

            CollisionConnectorSplitView = new( "Collision Connector", CollisionConnectors, false,
                ( PhybCollisionData item, int idx ) => item.CollisionName.Value, () => new( File, this ),
                () => CommandManager.Phyb, ( PhybCollisionData item ) => File.Updated() );

            ChainDropdown = new( "Chain", Chains,
                null, () => new( File, this ),
                () => CommandManager.Phyb, ( PhybChain item ) => File.Updated() );

            ConnectorSplitView = new( "Connector", Connectors, false,
                null, () => new( File, this ),
                () => CommandManager.Phyb, ( PhybConnector item ) => File.Updated() );

            AttractSplitView = new( "Attract", Attracts, false,
                null, () => new( File, this ),
                () => CommandManager.Phyb, ( PhybAttract item ) => File.Updated() );

            PinSplitView = new( "Pin", Pins, false,
                null, () => new( File, this ),
                () => CommandManager.Phyb, ( PhybPin item ) => File.Updated() );

            SpringSplitView = new( "Spring", Springs, false,
                null, () => new( File, this ),
                () => CommandManager.Phyb, ( PhybSpring item ) => File.Updated() );

            PostAlignmentSplitView = new( "Post Alignment", PostAlignments, false,
                null, () => new( File, this ),
                () => CommandManager.Phyb, ( PhybPostAlignment item ) => File.Updated() );
        }

        public PhybSimulator( PhybFile file, BinaryReader reader, long simulatorStartPos ) : this( file ) {
            File = file;

            var numCollisionData = reader.ReadByte();
            var numCollisionConnector = reader.ReadByte();
            var numChain = reader.ReadByte();
            var numConnector = reader.ReadByte();
            var numAttract = reader.ReadByte();
            var numPin = reader.ReadByte();
            var numSpring = reader.ReadByte();
            var numPostAlignment = reader.ReadByte();

            Params = new( file, reader );

            var collisionDataOffset = reader.ReadUInt32();
            var collisionConnectorOffset = reader.ReadUInt32();
            var chainOffset = reader.ReadUInt32();
            var connectorOffset = reader.ReadUInt32();
            var attractOffset = reader.ReadUInt32();
            var pinOffset = reader.ReadUInt32();
            var springOffset = reader.ReadUInt32();
            var postAlignmentOffset = reader.ReadUInt32();
            if( postAlignmentOffset == 0xCCCCCCCC ) postAlignmentOffset = 0; // ?

            var resetPos = reader.BaseStream.Position;

            reader.BaseStream.Seek( simulatorStartPos + collisionDataOffset + 4, SeekOrigin.Begin );
            for( var i = 0; i < numCollisionData; i++ ) Collisions.Add( new PhybCollisionData( file, this, reader ) );

            reader.BaseStream.Seek( simulatorStartPos + collisionConnectorOffset + 4, SeekOrigin.Begin );
            for( var i = 0; i < numCollisionConnector; i++ ) CollisionConnectors.Add( new PhybCollisionData( file, this, reader ) );

            reader.BaseStream.Seek( simulatorStartPos + chainOffset + 4, SeekOrigin.Begin );
            for( var i = 0; i < numChain; i++ ) Chains.Add( new PhybChain( file, this, reader, simulatorStartPos ) );

            reader.BaseStream.Seek( simulatorStartPos + connectorOffset + 4, SeekOrigin.Begin );
            for( var i = 0; i < numConnector; i++ ) Connectors.Add( new PhybConnector( file, this, reader ) );

            reader.BaseStream.Seek( simulatorStartPos + attractOffset + 4, SeekOrigin.Begin );
            for( var i = 0; i < numAttract; i++ ) Attracts.Add( new PhybAttract( file, this, reader ) );

            reader.BaseStream.Seek( simulatorStartPos + pinOffset + 4, SeekOrigin.Begin );
            for( var i = 0; i < numPin; i++ ) Pins.Add( new PhybPin( file, this, reader ) );

            reader.BaseStream.Seek( simulatorStartPos + springOffset + 4, SeekOrigin.Begin );
            for( var i = 0; i < numSpring; i++ ) Springs.Add( new PhybSpring( file, this, reader ) );

            reader.BaseStream.Seek( simulatorStartPos + postAlignmentOffset + 4, SeekOrigin.Begin );
            for( var i = 0; i < numPostAlignment; i++ ) PostAlignments.Add( new PhybPostAlignment( file, this, reader ) );

            // reset
            reader.BaseStream.Seek( resetPos, SeekOrigin.Begin );
        }

        public long WriteHeader( SimulationWriter writer ) {
            writer.Write( ( byte )Collisions.Count );
            writer.Write( ( byte )CollisionConnectors.Count );
            writer.Write( ( byte )Chains.Count );
            writer.Write( ( byte )Connectors.Count );
            writer.Write( ( byte )Attracts.Count );
            writer.Write( ( byte )Pins.Count );
            writer.Write( ( byte )Springs.Count );
            writer.Write( ( byte )PostAlignments.Count );

            Params.Write( writer );

            // Placeholders
            var placeholderPos = writer.Position;
            for( var i = 0; i < 8; i++ ) writer.Write( 0 );

            return placeholderPos;
        }

        public void Draw() {
            using var _ = ImRaii.PushId( "Simulator" );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Parameters" ) ) {
                if( tab ) Params.Draw();
            }

            using( var tab = ImRaii.TabItem( "Collision Objects" ) ) {
                if( tab ) CollisionSplitView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Collision Connectors" ) ) {
                if( tab ) CollisionConnectorSplitView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Chains" ) ) {
                if( tab ) ChainDropdown.Draw();
            }

            using( var tab = ImRaii.TabItem( "Connectors" ) ) {
                if( tab ) ConnectorSplitView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Attracts" ) ) {
                if( tab ) AttractSplitView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Pins" ) ) {
                if( tab ) PinSplitView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Springs" ) ) {
                if( tab ) SpringSplitView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Post Alignments" ) ) {
                if( tab ) PostAlignmentSplitView.Draw();
            }
        }

        public void AddPhysicsObjects( MeshBuilders meshes, Dictionary<string, Bone> boneMatrixes ) {
            foreach( var item in Collisions ) item.AddPhysicsObjects( meshes, boneMatrixes );
            foreach( var item in CollisionConnectors ) item.AddPhysicsObjects( meshes, boneMatrixes );
            foreach( var item in Chains ) item.AddPhysicsObjects( meshes, boneMatrixes );
            foreach( var item in Connectors ) item.AddPhysicsObjects( meshes, boneMatrixes );
            foreach( var item in Attracts ) item.AddPhysicsObjects( meshes, boneMatrixes );
            foreach( var item in Pins ) item.AddPhysicsObjects( meshes, boneMatrixes );
            foreach( var item in Springs ) item.AddPhysicsObjects( meshes, boneMatrixes );
            foreach( var item in PostAlignments ) item.AddPhysicsObjects( meshes, boneMatrixes );
        }

        public void ConnectNodes( int chainId1, int chainId2, int nodeId1, int nodeId2, float radius,
            MeshBuilder builder, Dictionary<string, Bone> boneMatrixes ) {

            if( !GetNode( chainId1, nodeId1, boneMatrixes, out var bone1, out var _ ) ) return;
            if( !GetNode( chainId2, nodeId2, boneMatrixes, out var bone2, out var _ ) ) return;

            var pos1 = bone1.BindPose.TranslationVector;
            var pos2 = bone2.BindPose.TranslationVector;

            builder.AddCylinder( pos1, pos2, radius * 2f, 10 );
        }

        public void ConnectNodeToBone( int chainId, int nodeId,
            string boneName, Vector3 offset,
            MeshBuilder builder, Dictionary<string, Bone> boneMatrixes ) {

            if( !GetNode( chainId, nodeId, boneMatrixes, out var nodeBone, out var node ) ) return;
            if( !boneMatrixes.TryGetValue( boneName, out var bone ) ) return;

            var nodeStart = nodeBone.BindPose.TranslationVector;
            var bonePos = Vector3.Transform( offset, bone.BindPose ).ToVector3();

            var axisOffset = new Vector3( node.ConeAxisOffset.Value.Y, node.ConeAxisOffset.Value.X, -node.ConeAxisOffset.Value.Z );
            var axisPos = Vector3.Transform( axisOffset, nodeBone.BindPose ).ToVector3();
            var norm = ( axisPos - nodeStart ).Normalized();
            var distance = ( bonePos - nodeStart ).Length();

            var coneStart = nodeStart + distance * norm;
            var coneEnd = nodeStart;
            var angle = node.ConeMaxAngle.Value;
            // tan(angle) = radius / distance
            var radius = Math.Tan( angle ) * distance;

            builder.AddCone( coneStart, coneEnd, radius, false, 10 );

            builder.AddSphere( bonePos, 0.01f, 10, 10 );
        }

        public bool GetNode( int chainId, int nodeId, Dictionary<string, Bone> boneMatrixes, out Bone bone, out PhybNode node ) {
            bone = default;
            node = null;

            nodeId -= 1; // 1-indexed?
            if( chainId >= Chains.Count || chainId < 0 ) return false;
            if( nodeId >= Chains[chainId].Nodes.Count || nodeId < 0 ) return false;

            node = Chains[chainId].Nodes[nodeId];

            if( !boneMatrixes.TryGetValue( node.BoneName.Value, out bone ) ) return false;

            return true;
        }
    }
}
