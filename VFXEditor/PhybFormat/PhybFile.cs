using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Animations;
using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;
using VfxEditor.PhybFormat.Collision;
using VfxEditor.PhybFormat.Simulator;
using VfxEditor.PhybFormat.Skeleton;
using VfxEditor.PhybFormat.Utils;
using VfxEditor.Utils;

namespace VfxEditor.PhybFormat {
    public class MeshBuilders {
        public MeshBuilder Collision;
        public MeshBuilder Simulation;
        public MeshBuilder Spring;
    }

    public class PhybFile : FileManagerFile, IPhysicsObject {
        public readonly ParsedIntByte4 Version = new( "Version" );
        public readonly ParsedUInt DataType = new( "Data Type" );

        public readonly PhybCollision Collision;
        public readonly PhybSimulation Simulation;

        public readonly SkeletonView Skeleton;
        public bool PhysicsUpdated = true;

        public PhybFile( BinaryReader reader, string sourcePath, bool checkOriginal = true ) : base( new( Plugin.PhybManager ) ) {
            var original = checkOriginal ? FileUtils.GetOriginal( reader ) : null;

            Version.Read( reader );

            if( Version.Value > 0 ) {
                DataType.Read( reader );
            }

            var collisionOffset = reader.ReadUInt32();
            var simOffset = reader.ReadUInt32();

            reader.BaseStream.Seek( collisionOffset, SeekOrigin.Begin );
            Collision = new( this, reader, collisionOffset == simOffset );

            reader.BaseStream.Seek( simOffset, SeekOrigin.Begin );
            Simulation = new( this, reader, simOffset == reader.BaseStream.Length );

            if( checkOriginal ) Verified = FileUtils.CompareFiles( original, ToBytes(), out var _ );

            Skeleton = new( this, sourcePath );
        }

        public override void Write( BinaryWriter writer ) {
            writer.BaseStream.Seek( 0, SeekOrigin.Begin );

            Version.Write( writer );

            if( Version.Value > 0 ) {
                DataType.Write( writer );
            }

            var offsetPos = writer.BaseStream.Position; // coming back here later
            writer.Write( 0 ); // placeholders
            writer.Write( 0 );

            if( Version.Value == 0 ) return;

            var collisionOffset = writer.BaseStream.Position;
            Collision.Write( writer );

            var simOffset = writer.BaseStream.Position;
            var simWriter = new SimulationWriter();
            Simulation.Write( simWriter );
            simWriter.WriteTo( writer );

            writer.BaseStream.Seek( offsetPos, SeekOrigin.Begin );
            writer.Write( ( int )collisionOffset );
            writer.Write( ( int )simOffset );
        }

        public override void Draw() {
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );

            Version.Draw( CommandManager.Phyb );
            DataType.Draw( CommandManager.Phyb );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Collision" ) ) {
                if( tab ) Collision.Draw();
            }

            using( var tab = ImRaii.TabItem( "Simulation" ) ) {
                if( tab ) Simulation.Draw();
            }

            using( var tab = ImRaii.TabItem( "3D View" ) ) {
                if( tab ) Skeleton.Draw();
            }
        }

        public override void Dispose() {
            base.Dispose();
            Skeleton.Dispose();
        }

        public void AddPhysicsObjects( MeshBuilders meshes, Dictionary<string, Bone> boneMatrixes ) {
            Collision.AddPhysicsObjects( meshes, boneMatrixes );
            Simulation.AddPhysicsObjects( meshes, boneMatrixes );
        }

        public void Updated() {
            PhysicsUpdated = true;
        }
    }
}
