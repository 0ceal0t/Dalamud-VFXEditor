using Dalamud.Interface.Utility.Raii;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Animations;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.FileManager;
using VfxEditor.Formats.PhybFormat.Extended;
using VfxEditor.Interop.Havok.Ui;
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

        public readonly PhybSkeletonView Skeleton;
        public bool PhysicsUpdated = true;
        private bool SkeletonTabOpen = false;

        public readonly PhybExtended Extended;

        public PhybFile( BinaryReader reader, string sourcePath, bool verify ) : base() {
            Version.Read( reader );

            if( Version.Value > 0 ) DataType.Read( reader );

            var collisionOffset = reader.ReadUInt32();
            var simOffset = reader.ReadUInt32();

            reader.BaseStream.Position = collisionOffset;
            Collision = new( this, reader, collisionOffset == simOffset );

            // New to Dawntrail
            List<(int, int)> ignoreRange = null;
            var diff = 0;
            var ephbPos = reader.BaseStream.Length;

            reader.BaseStream.Position = reader.BaseStream.Length <= 0x18 ? 0 : reader.BaseStream.Length - 0x18;
            if( reader.ReadUInt32() == PhybExtended.MAGIC_PACK ) {
                Extended = new( reader, out ephbPos, out var ephbSize );
                ignoreRange = [(( int )ephbPos, ( int )reader.BaseStream.Length)];
                diff = ephbSize - Extended.Table.Export().SerializeToBinary().Length;
            }

            reader.BaseStream.Position = simOffset;
            Simulation = new( this, reader, simOffset == ephbPos );

            if( verify ) {
                Verified = FileUtils.Verify( reader, ToBytes(), ignoreRange, diff );
                if( Verified == VerifiedStatus.VERIFIED && Extended != null ) Verified = VerifiedStatus.PARTIAL;
            }

            Skeleton = new( this, Path.IsPathRooted( sourcePath ) ? null : sourcePath );
        }

        public override void Write( BinaryWriter writer ) {
            writer.BaseStream.Position = 0;

            if( Version.Value == 0 ) {
                writer.Write( 0 );
                writer.Write( 0x0C );
                writer.Write( 0x0C );
                return;
            }

            Version.Write( writer );
            DataType.Write( writer );

            var offsetPos = writer.BaseStream.Position; // coming back here later
            writer.Write( 0 ); // placeholders
            writer.Write( 0 );

            var collisionOffset = writer.BaseStream.Position;
            Collision.Write( writer );

            var simOffset = writer.BaseStream.Position;
            var simWriter = new SimulationWriter();
            Simulation.Write( simWriter );
            simWriter.WriteTo( writer );

            writer.BaseStream.Position = offsetPos;
            writer.Write( ( int )collisionOffset );
            writer.Write( ( int )simOffset );

            writer.BaseStream.Position = writer.BaseStream.Length;
            Extended?.Write( writer );
        }

        public override void Draw() {
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );

            var size = SkeletonView.CalculateSize( SkeletonTabOpen, Plugin.Configuration.PhybSkeletonSplit );

            using var style = ImRaii.PushStyle( ImGuiStyleVar.WindowPadding, new Vector2( 0, 0 ) );
            using( var child = ImRaii.Child( "Child", size, false ) ) {
                Version.Draw();
                DataType.Draw();

                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );

                using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
                if( !tabBar ) return;

                SkeletonTabOpen = false;

                using( var tab = ImRaii.TabItem( "Collision" ) ) {
                    if( tab ) Collision.Draw();
                }

                using( var tab = ImRaii.TabItem( "Simulation" ) ) {
                    if( tab ) Simulation.Draw();
                }

                using( var tab = ImRaii.TabItem( "3D View" ) ) {
                    if( tab ) {
                        Skeleton.Draw();
                        SkeletonTabOpen = true;
                    }
                }

                if( Extended != null ) {
                    using var tab = ImRaii.TabItem( "Extended" );
                    if( tab ) Extended.Draw();
                }
            }

            if( !SkeletonTabOpen ) Skeleton.DrawSplit( ref Plugin.Configuration.PhybSkeletonSplit );
        }

        public void AddPhysicsObjects( MeshBuilders meshes, Dictionary<string, Bone> boneMatrixes ) {
            Collision.AddPhysicsObjects( meshes, boneMatrixes );
            Simulation.AddPhysicsObjects( meshes, boneMatrixes );
        }

        public override void OnChange() {
            PhysicsUpdated = true;
        }
    }
}
