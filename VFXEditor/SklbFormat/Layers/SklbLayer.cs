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
        private readonly CollapsingHeaders<SklbLayerBone> BonesView;

        public int Size => 4 + 2 + 2 * Bones.Count; // Id + numBones + boneIndices

        public SklbLayer( SklbFile file ) {
            File = file;
            BonesView = new( "Bone", Bones, ( SklbLayerBone item, int idx ) => item.BoneIndex.GetText( File.Bones.Bones ), () => new( file ), () => CommandManager.Sklb );
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
        public ParsedBoneIndex BoneIndex = new( "Bone Index", -1 );

        public SklbLayerBone( SklbFile file ) {
            File = file;
        }

        public SklbLayerBone( SklbFile file, BinaryReader reader ) {
            File = file;
            BoneIndex.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            BoneIndex.Write( writer );
        }

        public void Draw() {
            BoneIndex.Draw( File.Bones.Bones );
        }
    }
}
