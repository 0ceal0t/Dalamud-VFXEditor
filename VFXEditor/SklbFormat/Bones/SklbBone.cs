using FFXIVClientStructs.Havok;
using OtterGui.Raii;
using System;
using System.Numerics;
using System.Runtime.InteropServices;
using VfxEditor.Parsing;

namespace VfxEditor.SklbFormat.Bones {
    public unsafe class SklbBone {
        public SklbBone Parent;

        public readonly int Id;

        public readonly ParsedString Name = new( "Name" );
        public readonly ParsedFloat4 Position = new( "Position" );
        public readonly ParsedFloat4 Rotation = new( "Rotation" );
        public readonly ParsedFloat4 Scale = new( "Scale " );
        public readonly ParsedInt LockTranslation = new( "Lock Translation" );

        public Vector4 Pos => Position.Value;
        public Vector4 Rot => Rotation.Value;
        public Vector4 Scl => Scale.Value;

        public SklbBone( int id ) {
            Id = id;
        }

        public SklbBone( hkaBone bone, hkQsTransformf pose, int id ) : this( id ) {
            Name.Value = bone.Name.String;
            var pos = pose.Translation;
            var rot = pose.Rotation;
            var scl = pose.Scale;
            Position.Value = new( pos.X, pos.Y, pos.Z, pos.W );
            Rotation.Value = new( rot.X, rot.Y, rot.Z, rot.W );
            Scale.Value = new( scl.X, scl.Y, scl.Z, scl.W );
            LockTranslation.Value = bone.LockTranslation;
        }

        public void DrawBody() {
            using var _ = ImRaii.PushId( Id );

            Name.Draw( CommandManager.Sklb );
            Position.Draw( CommandManager.Sklb );
            Rotation.Draw( CommandManager.Sklb );
            Scale.Draw( CommandManager.Sklb );
            LockTranslation.Draw( CommandManager.Sklb );
        }

        public void ToHavok( out hkaBone bone, out hkQsTransformf pose, out IntPtr nameHandle ) {
            nameHandle = Marshal.StringToHGlobalAnsi( Name.Value );

            var namePtr = new hkStringPtr {
                StringAndFlag = ( byte* )nameHandle
            };

            bone = new() {
                Name = namePtr,
                LockTranslation = ( byte )LockTranslation.Value
            };

            pose = new();

            var pos = new hkVector4f {
                X = Position.Value.X,
                Y = Position.Value.Y,
                Z = Position.Value.Z,
                W = Position.Value.W
            };
            pose.Translation = pos;

            var rot = new hkQuaternionf {
                X = Rotation.Value.X,
                Y = Rotation.Value.Y,
                Z = Rotation.Value.Z,
                W = Rotation.Value.W
            };
            pose.Rotation = rot;

            var scl = new hkVector4f {
                X = Scale.Value.X,
                Y = Scale.Value.Y,
                Z = Scale.Value.Z,
                W = Scale.Value.W
            };
            pose.Scale = scl;
        }
    }
}
