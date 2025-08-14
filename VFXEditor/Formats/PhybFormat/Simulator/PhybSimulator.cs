using Dalamud.Interface.Utility.Raii;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Animations;
using Dalamud.Bindings.ImGui;
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
using VfxEditor.Ui.Components.SplitViews;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.PhybFormat.Simulator {
    public class PhybSimulator : IUiItem, IPhysicsObject {
        public readonly PhybFile File;

        public readonly PhybSimulatorParams Params;

        public readonly List<PhybCollisionData> Collisions = [];
        public readonly List<PhybCollisionData> CollisionConnectors = [];
        public readonly List<PhybChain> Chains = [];
        public readonly List<PhybConnector> Connectors = [];
        public readonly List<PhybAttract> Attracts = [];
        public readonly List<PhybPin> Pins = [];
        public readonly List<PhybSpring> Springs = [];
        public readonly List<PhybPostAlignment> PostAlignments = [];

        private readonly CommandSplitView<PhybCollisionData> CollisionSplitView;
        private readonly CommandSplitView<PhybCollisionData> CollisionConnectorSplitView;
        private readonly CommandDropdown<PhybChain> ChainDropdown;
        private readonly CommandSplitView<PhybConnector> ConnectorSplitView;
        private readonly CommandSplitView<PhybAttract> AttractSplitView;
        private readonly CommandSplitView<PhybPin> PinSplitView;
        private readonly CommandSplitView<PhybSpring> SpringSplitView;
        private readonly CommandSplitView<PhybPostAlignment> PostAlignmentSplitView;

        public PhybSimulator( PhybFile file ) {
            File = file;
            Params = new( file );

            CollisionSplitView = new( "Collision Object", Collisions, false,
                ( PhybCollisionData item, int idx ) => item.CollisionName.Value, () => new( File, this ), ( PhybCollisionData _, bool _ ) => File.OnChange() );

            CollisionConnectorSplitView = new( "Collision Connector", CollisionConnectors, false,
                ( PhybCollisionData item, int idx ) => item.CollisionName.Value, () => new( File, this ), ( PhybCollisionData _, bool _ ) => File.OnChange() );

            ChainDropdown = new( "Chain", Chains,
                null, () => new( File, this ), ( PhybChain _, bool _ ) => File.OnChange() );

            ConnectorSplitView = new( "Connector", Connectors, false,
                null, () => new( File, this ), ( PhybConnector _, bool _ ) => File.OnChange() );

            AttractSplitView = new( "Attract", Attracts, false,
                null, () => new( File, this ), ( PhybAttract _, bool _ ) => File.OnChange() );

            PinSplitView = new( "Pin", Pins, false,
                null, () => new( File, this ), ( PhybPin _, bool _ ) => File.OnChange() );

            SpringSplitView = new( "Spring", Springs, false,
                null, () => new( File, this ), ( PhybSpring _, bool _ ) => File.OnChange() );

            PostAlignmentSplitView = new( "Post Alignment", PostAlignments, false,
                null, () => new( File, this ), ( PhybPostAlignment _, bool _ ) => File.OnChange() );
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

            reader.BaseStream.Position = simulatorStartPos + collisionDataOffset + 4;
            for( var i = 0; i < numCollisionData; i++ ) Collisions.Add( new PhybCollisionData( file, this, reader ) );

            reader.BaseStream.Position = simulatorStartPos + collisionConnectorOffset + 4;
            for( var i = 0; i < numCollisionConnector; i++ ) CollisionConnectors.Add( new PhybCollisionData( file, this, reader ) );

            reader.BaseStream.Position = simulatorStartPos + chainOffset + 4;
            for( var i = 0; i < numChain; i++ ) Chains.Add( new PhybChain( file, this, reader, simulatorStartPos ) );

            reader.BaseStream.Position = simulatorStartPos + connectorOffset + 4;
            for( var i = 0; i < numConnector; i++ ) Connectors.Add( new PhybConnector( file, this, reader ) );

            reader.BaseStream.Position = simulatorStartPos + attractOffset + 4;
            for( var i = 0; i < numAttract; i++ ) Attracts.Add( new PhybAttract( file, this, reader ) );

            reader.BaseStream.Position = simulatorStartPos + pinOffset + 4;
            for( var i = 0; i < numPin; i++ ) Pins.Add( new PhybPin( file, this, reader ) );

            reader.BaseStream.Position = simulatorStartPos + springOffset + 4;
            for( var i = 0; i < numSpring; i++ ) Springs.Add( new PhybSpring( file, this, reader ) );

            reader.BaseStream.Position = simulatorStartPos + postAlignmentOffset + 4;
            for( var i = 0; i < numPostAlignment; i++ ) PostAlignments.Add( new PhybPostAlignment( file, this, reader ) );

            // reset
            reader.BaseStream.Position = resetPos;
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
            string boneName,
            MeshBuilder builder, Dictionary<string, Bone> boneMatrixes ) {

            if( !GetNode( chainId, nodeId, boneMatrixes, out var nodeBone, out var node ) ) return;
            if( !boneMatrixes.TryGetValue( boneName, out var _ ) ) return;

            var nodeStart = nodeBone.BindPose.TranslationVector;
            // var bonePos = Vector3.Transform( offset, bone.BindPose ).ToVector3();

            var axisOffset = new Vector3( node.ConeAxisOffset.Value.Y, node.ConeAxisOffset.Value.X, -node.ConeAxisOffset.Value.Z );
            var axisPos = Vector3.Transform( axisOffset, nodeBone.BindPose ).ToVector3();
            var norm = ( axisPos - nodeStart ).Normalized();

            // var boneDiff = bonePos - nodeStart;
            // var closest = norm * Vector3.Dot( boneDiff, norm );

            // var coneStart = nodeStart + closest;
            var coneStart = nodeStart + norm * 0.2f;
            var coneEnd = nodeStart;
            var distance = ( coneEnd - coneStart ).Length();
            var angle = node.ConeMaxAngle.Value;
            // tan(angle) = radius / distance
            var radius = Math.Tan( angle ) * distance;

            builder.AddCone( coneStart, coneEnd, radius, false, 10 );
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
