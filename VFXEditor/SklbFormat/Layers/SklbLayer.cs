using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.SklbFormat.Layers {
    public class SklbLayer : IUiItem {
        public readonly ParsedInt Id = new( "Id" );
        public readonly List<SklbLayerBone> Bones = new();
        private readonly CollapsingHeaders<SklbLayerBone> BonesView;

        public int Size => 4 + 2 + 2 * Bones.Count; // Id + numBones + boneIndices

        public SklbLayer() {
            BonesView = new( "Bone", Bones, null, () => new(), () => CommandManager.Sklb );
        }

        public SklbLayer( BinaryReader reader ) : this() {
            Id.Read( reader );
            var numBones = reader.ReadInt16();
            for( var i = 0; i < numBones; i++ ) {
                Bones.Add( new( reader ) );
            }
        }

        public void Write( BinaryWriter writer ) {
            Id.Write( writer );
            writer.Write( ( short )Bones.Count );
            Bones.ForEach( x => x.Write( writer ) );
        }

        public void Draw() {
            Id.Draw( CommandManager.Sklb );
            BonesView.Draw();
        }
    }

    public class SklbLayerBone : IUiItem {
        public ParsedShort BoneIndex = new( "Bone Index" );

        public SklbLayerBone() { }

        public SklbLayerBone( BinaryReader reader ) {
            BoneIndex.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            BoneIndex.Write( writer );
        }

        public void Draw() {
            BoneIndex.Draw( CommandManager.Sklb );
        }
    }
}
