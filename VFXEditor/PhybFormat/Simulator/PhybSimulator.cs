using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Animations;
using ImGuiNET;
using OtterGui.Raii;
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
                ( PhybCollisionData item, int idx ) => item.Name.Value, () => new( File ), () => CommandManager.Phyb );

            CollisionConnectorSplitView = new( "Collision Connector", CollisionConnectors, false,
                ( PhybCollisionData item, int idx ) => item.Name.Value, () => new( File ), () => CommandManager.Phyb );

            ChainDropdown = new( "Chain", Chains,
                null, () => new( File ), () => CommandManager.Phyb );

            ConnectorSplitView = new( "Connector", Connectors, false,
                null, () => new( File ), () => CommandManager.Phyb );

            AttractSplitView = new( "Attract", Attracts, false,
                null, () => new( File ), () => CommandManager.Phyb );

            PinSplitView = new( "Pin", Pins, false,
                null, () => new( File ), () => CommandManager.Phyb );

            SpringSplitView = new( "Spring", Springs, false,
                null, () => new( File ), () => CommandManager.Phyb );

            PostAlignmentSplitView = new( "Post Alignment", PostAlignments, false,
                null, () => new( File ), () => CommandManager.Phyb );
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
            for( var i = 0; i < numCollisionData; i++ ) Collisions.Add( new PhybCollisionData( file, reader ) );

            reader.BaseStream.Seek( simulatorStartPos + collisionConnectorOffset + 4, SeekOrigin.Begin );
            for( var i = 0; i < numCollisionConnector; i++ ) CollisionConnectors.Add( new PhybCollisionData( file, reader ) );

            reader.BaseStream.Seek( simulatorStartPos + chainOffset + 4, SeekOrigin.Begin );
            for( var i = 0; i < numChain; i++ ) Chains.Add( new PhybChain( file, reader, simulatorStartPos ) );

            reader.BaseStream.Seek( simulatorStartPos + connectorOffset + 4, SeekOrigin.Begin );
            for( var i = 0; i < numConnector; i++ ) Connectors.Add( new PhybConnector( file, reader ) );

            reader.BaseStream.Seek( simulatorStartPos + attractOffset + 4, SeekOrigin.Begin );
            for( var i = 0; i < numAttract; i++ ) Attracts.Add( new PhybAttract( file, reader ) );

            reader.BaseStream.Seek( simulatorStartPos + pinOffset + 4, SeekOrigin.Begin );
            for( var i = 0; i < numPin; i++ ) Pins.Add( new PhybPin( file, reader ) );

            reader.BaseStream.Seek( simulatorStartPos + springOffset + 4, SeekOrigin.Begin );
            for( var i = 0; i < numSpring; i++ ) Springs.Add( new PhybSpring( file, reader ) );

            reader.BaseStream.Seek( simulatorStartPos + postAlignmentOffset + 4, SeekOrigin.Begin );
            for( var i = 0; i < numPostAlignment; i++ ) PostAlignments.Add( new PhybPostAlignment( file, reader ) );

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

        public void AddPhysicsObjects( MeshBuilder builder, Dictionary<string, Bone> boneMatrixes ) {
            foreach( var item in Collisions ) item.AddPhysicsObjects( builder, boneMatrixes );
            foreach( var item in CollisionConnectors ) item.AddPhysicsObjects( builder, boneMatrixes );
            foreach( var item in Chains ) item.AddPhysicsObjects( builder, boneMatrixes );
            foreach( var item in Connectors ) item.AddPhysicsObjects( builder, boneMatrixes );
            foreach( var item in Attracts ) item.AddPhysicsObjects( builder, boneMatrixes );
            foreach( var item in Pins ) item.AddPhysicsObjects( builder, boneMatrixes );
            foreach( var item in Springs ) item.AddPhysicsObjects( builder, boneMatrixes );
            foreach( var item in PostAlignments ) item.AddPhysicsObjects( builder, boneMatrixes );
        }
    }
}
