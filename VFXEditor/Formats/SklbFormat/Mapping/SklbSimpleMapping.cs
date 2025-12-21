using FFXIVClientStructs.Havok.Common.Base.Math.QsTransform;
using FFXIVClientStructs.Havok.Common.Base.Math.Quaternion;
using FFXIVClientStructs.Havok.Common.Base.Math.Vector;
using Dalamud.Bindings.ImGui;
using System.Numerics;
using VfxEditor.Interop.Structs.Animation;
using VfxEditor.Parsing;
using VfxEditor.SklbFormat.Bones;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.SklbFormat.Mapping {
    public unsafe class SklbSimpleMapping : IUiItem {
        public readonly SklbMapping Mapping;

        public readonly ParsedBoneIndex BoneA = new( "Mapped Skeleton Bone", -1 );
        public readonly ParsedBoneIndex BoneB = new( "This Skeleton Bone", -1 );
        public readonly ParsedInt Unk1 = new( "Unknown 1" );
        public readonly ParsedInt Unk2 = new( "Unknown 2" );
        public readonly ParsedInt Unk3 = new( "Unknown 3" );
        public readonly ParsedFloat4 Translation = new( "Translation" );
        public readonly ParsedQuat Rotation = new( "Rotation" );
        public readonly ParsedFloat4 Scale = new( "Scale", new Vector4( 1, 1, 1, 1 ) );

        public SklbSimpleMapping( SklbMapping mapping ) {
            Mapping = mapping;
        }

        public SklbSimpleMapping( SklbMapping mapping, SimpleMapping simpleMapping ) : this( mapping ) {
            BoneA.Value = simpleMapping.BoneA;
            BoneB.Value = simpleMapping.BoneB;
            Unk1.Value = simpleMapping.Unk1;
            Unk2.Value = simpleMapping.Unk2;
            Unk3.Value = simpleMapping.Unk3;

            var transform = simpleMapping.AFromBTransform;
            var pos = transform.Translation;
            var rot = transform.Rotation;
            var scale = transform.Scale;

            Translation.Value = new( pos.X, pos.Y, pos.Z, pos.W );
            Rotation.Quaternion = new( rot.X, rot.Y, rot.Z, rot.W );
            Scale.Value = new( scale.X, scale.Y, scale.Z, scale.W );
        }

        public void Draw() {
            if( ImGui.Checkbox( "Display Raw Indexes", ref Plugin.Configuration.SklbMappingIndexDisplay ) ) Plugin.Configuration.Save();

            if( Plugin.Configuration.SklbMappingIndexDisplay ) {
                BoneA.Draw();
                BoneB.Draw();
            }
            else {
                BoneA.Draw( Mapping.MappedSkeleton );
                BoneB.Draw( Mapping.Bones.Bones );
            }

            Unk1.Draw();
            Unk2.Draw();
            Unk3.Draw();
            Translation.Draw();
            Rotation.Draw();
            Scale.Draw();
        }

        public SimpleMapping ToHavok() {
            var transform = new hkQsTransformf();

            var pos = new hkVector4f {
                X = Translation.Value.X,
                Y = Translation.Value.Y,
                Z = Translation.Value.Z,
                W = Translation.Value.W
            };
            transform.Translation = pos;

            var rotation = Rotation.Quaternion;
            var rot = new hkQuaternionf {
                X = ( float )rotation.X,
                Y = ( float )rotation.Y,
                Z = ( float )rotation.Z,
                W = ( float )rotation.W
            };
            transform.Rotation = rot;

            var scl = new hkVector4f {
                X = Scale.Value.X,
                Y = Scale.Value.Y,
                Z = Scale.Value.Z,
                W = Scale.Value.W
            };
            transform.Scale = scl;

            var simpleMapping = new SimpleMapping() {
                BoneA = ( short )BoneA.Value,
                BoneB = ( short )BoneB.Value,
                Unk1 = Unk1.Value,
                Unk2 = Unk2.Value,
                Unk3 = Unk3.Value,
                AFromBTransform = transform
            };
            return simpleMapping;
        }
    }
}
