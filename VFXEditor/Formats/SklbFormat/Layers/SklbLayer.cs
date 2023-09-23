using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.SklbFormat.Bones;
using VfxEditor.Ui.Components;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.SklbFormat.Layers {
    public class SklbLayer : IUiItem {
        public bool IsSklb => File != null;
        public readonly SklbFile File;

        public readonly ParsedInt Id = new( "Id" );
        public readonly List<SklbLayerBone> Bones = new();
        private readonly ListView<SklbLayerBone> BonesView;

        public int Size => 4 + 2 + 2 * Bones.Count; // Id + numBones + boneIndices

        public SklbLayer( SklbFile file ) {
            File = file;
            BonesView = new( Bones, () => new( File ), () => IsSklb ? CommandManager.Sklb : CommandManager.Skp );
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
            Id.Draw( IsSklb ? CommandManager.Sklb : CommandManager.Skp );
            BonesView.Draw();
        }
    }

    public unsafe class SklbLayerBone : IUiItem {
        // For Sklb
        public bool IsSklb => File != null;
        public readonly SklbFile File;
        public readonly ParsedBoneIndex Bone = new( "##Bone", -1 ); // Don't want the label to actually appear

        // For Skp
        public readonly ParsedShort SkpBone = new( "##Bone", -1 );

        public SklbLayerBone( SklbFile file ) {
            File = file;
        }

        public SklbLayerBone( SklbFile file, BinaryReader reader ) : this( file ) {
            if( IsSklb ) Bone.Read( reader );
            else SkpBone.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            if( IsSklb ) Bone.Write( writer );
            else SkpBone.Write( writer );
        }

        public void Draw() {
            if( IsSklb ) Bone.Draw( File.Bones.Bones );
            else SkpBone.Draw( CommandManager.Skp );
        }
    }
}
