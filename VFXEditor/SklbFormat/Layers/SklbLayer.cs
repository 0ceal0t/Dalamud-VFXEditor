using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.SklbFormat.Bones;
using VfxEditor.Ui.Components;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.SklbFormat.Layers {
    public class SklbLayer : IUiItem {
        public readonly SklbFile File;
        public readonly ParsedInt Id = new( "Id" );
        public readonly List<SklbLayerBone> Bones = new();
        private readonly ListView<SklbLayerBone> BonesView;

        public int Size => 4 + 2 + 2 * Bones.Count; // Id + numBones + boneIndices

        public SklbLayer( SklbFile file ) {
            File = file;
            BonesView = new( Bones, () => new( File ), () => CommandManager.Sklb );
        }

        public SklbLayer( SklbFile file, BinaryReader reader ) : this( file ) {
            Id.Read( reader );
            var numBones = reader.ReadInt16();
            for( var i = 0; i < numBones; i++ ) {
                Bones.Add( new( file, reader ) );
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

    public unsafe class SklbLayerBone : IUiItem {
        public readonly SklbFile File;
        public readonly ParsedBoneIndex Bone = new( "##Bone", -1 ); // Don't want the label to actually appear

        public SklbLayerBone( SklbFile file ) {
            File = file;
        }

        public SklbLayerBone( SklbFile file, BinaryReader reader ) {
            File = file;
            Bone.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            Bone.Write( writer );
        }

        public void Draw() {
            Bone.Draw( File.Bones.Bones );
        }
    }
}
