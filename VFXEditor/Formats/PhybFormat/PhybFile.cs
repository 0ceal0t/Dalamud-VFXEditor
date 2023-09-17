using Dalamud.Interface;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Animations;
using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
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

        private bool SkeletonTabOpen = false;

        public PhybFile( BinaryReader reader, string sourcePath, bool verify ) : base( new( Plugin.PhybManager, () => Plugin.PhybManager.CurrentFile?.Updated() ) ) {
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

            if( verify ) Verified = FileUtils.CompareFiles( reader, ToBytes(), out var _ );

            Skeleton = new( this, Path.IsPathRooted( sourcePath ) ? null : sourcePath );
        }

        public override void Write( BinaryWriter writer ) {
            writer.BaseStream.Seek( 0, SeekOrigin.Begin );

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

            writer.BaseStream.Seek( offsetPos, SeekOrigin.Begin );
            writer.Write( ( int )collisionOffset );
            writer.Write( ( int )simOffset );
        }

        public override void Draw() {
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );

            var size = new Vector2( ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y - ImGui.GetFrameHeightWithSpacing() - ImGui.GetStyle().ItemSpacing.Y * 2f );
            if( SkeletonTabOpen ) {
                size = new Vector2( -1 );
            }
            else if( Plugin.Configuration.PhybSkeletonSplit ) {
                size = new Vector2( ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y / 2 );
            }


            using var style = ImRaii.PushStyle( ImGuiStyleVar.WindowPadding, new Vector2( 0, 0 ) );
            using( var child = ImRaii.Child( "Child", size, false ) ) {
                Version.Draw( CommandManager.Phyb );
                DataType.Draw( CommandManager.Phyb );

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
            }

            if( !SkeletonTabOpen ) {
                ImGui.Separator();
                using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                    if( ImGui.Button( Plugin.Configuration.PhybSkeletonSplit ? FontAwesomeIcon.AngleDoubleDown.ToIconString() : FontAwesomeIcon.AngleDoubleUp.ToIconString() ) ) {
                        Plugin.Configuration.PhybSkeletonSplit = !Plugin.Configuration.PhybSkeletonSplit;
                        Plugin.Configuration.Save();
                    }
                }

                if( Plugin.Configuration.PhybSkeletonSplit ) {
                    ImGui.SameLine();
                    Skeleton.Draw();
                }
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
