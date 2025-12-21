using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.Havok.Animation.Rig;
using FFXIVClientStructs.Havok.Common.Base.Container.String;
using FFXIVClientStructs.Havok.Common.Base.Math.QsTransform;
using FFXIVClientStructs.Havok.Common.Base.Math.Quaternion;
using FFXIVClientStructs.Havok.Common.Base.Math.Vector;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using VfxEditor.Parsing;

namespace VfxEditor.SklbFormat.Bones {
    public unsafe class SklbBone {
        public SklbBone Parent;

        public readonly int Id;

        public readonly ParsedString Name = new( "Name" );
        public readonly ParsedFloat4 Position = new( "Position", new Vector4( 0, 0, 0, 1 ) );
        public readonly ParsedQuat Rotation = new( "Rotation" );
        public readonly ParsedFloat4 Scale = new( "Scale", new Vector4( 1, 1, 1, 1 ) );
        public readonly ParsedInt LockTranslation = new( "Lock Translation" );

        public Vector4 Pos => Position.Value;
        public Quaternion Rot => Rotation.Quaternion;
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
            Rotation.Quaternion = new( rot.X, rot.Y, rot.Z, rot.W );
            Scale.Value = new( scl.X, scl.Y, scl.Z, scl.W );
            LockTranslation.Value = bone.LockTranslation;
        }

        public void DrawBody( int boneIdx ) {
            using var _ = ImRaii.PushId( Id );

            Name.Draw();
            ImGui.SameLine();
            ImGui.TextDisabled( $"[ ID: {boneIdx} ]" );

            Position.Draw();
            Rotation.Draw();
            Scale.Draw();
            LockTranslation.Draw();
        }

        public void ToHavok( HashSet<nint> handles, out hkaBone bone, out hkQsTransformf pose ) {
            var nameHandle = Marshal.StringToHGlobalAnsi( Name.Value );
            handles.Add( nameHandle );

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

            var quat = Rotation.Quaternion;
            var rot = new hkQuaternionf {
                X = ( float )quat.X,
                Y = ( float )quat.Y,
                Z = ( float )quat.Z,
                W = ( float )quat.W
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

        public bool IsChildOf( SklbBone parent ) {
            if( parent == null || Parent == null ) return false;
            if( Parent == parent ) return true;
            return Parent.IsChildOf( parent );
        }

        public void MakeChildOf( SklbBone parent ) {
            if( parent == null ) {
                Parent = null;
                return;
            }
            if( parent == this ) return;
            if( parent.IsChildOf( this ) ) return; // would be cyclical
            Parent = parent;
        }
    }
}
