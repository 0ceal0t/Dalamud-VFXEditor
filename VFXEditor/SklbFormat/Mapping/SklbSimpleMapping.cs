using FFXIVClientStructs.Havok;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.SklbFormat.Mapping {
    public class SklbSimpleMapping : IUiItem {
        public readonly ParsedShort BoneA = new( "Bone A", -1 );
        public readonly ParsedShort BoneB = new( "Bone B", -1 );
        public readonly ParsedInt Unk1 = new( "Unknown 1" );
        public readonly ParsedInt Unk2 = new( "Unknown 2" );
        public readonly ParsedFloat4 Translation = new( "Translation" );
        public readonly ParsedFloat4 Rotation = new( "Rotation", new( 0, 0, 0, 1 ) );
        public readonly ParsedFloat4 Scale = new( "Scale", new( 1, 1, 1, 1 ) );

        public SklbSimpleMapping() { }

        public SklbSimpleMapping( SimpleMapping simpleMapping ) {
            BoneA.Value = simpleMapping.BoneA;
            BoneB.Value = simpleMapping.BoneB;
            Unk1.Value = simpleMapping.Unk1;
            Unk2.Value = simpleMapping.Unk2;

            var transform = simpleMapping.AFromBTransform;
            var pos = transform.Translation;
            var rot = transform.Rotation;
            var scale = transform.Scale;

            Translation.Value = new( pos.X, pos.Y, pos.Z, pos.W );
            Rotation.Value = new( rot.X, rot.Y, rot.Z, rot.W );
            Scale.Value = new( scale.X, scale.Y, scale.Z, scale.W );
        }

        public void Draw() {
            BoneA.Draw( CommandManager.Sklb );
            BoneB.Draw( CommandManager.Sklb );
            Unk1.Draw( CommandManager.Sklb );
            Unk2.Draw( CommandManager.Sklb );
            Translation.Draw( CommandManager.Sklb );
            Rotation.Draw( CommandManager.Sklb );
            Scale.Draw( CommandManager.Sklb );
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

            var rot = new hkQuaternionf {
                X = Rotation.Value.X,
                Y = Rotation.Value.Y,
                Z = Rotation.Value.Z,
                W = Rotation.Value.W
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
                AFromBTransform = transform
            };
            return simpleMapping;
        }
    }
}
