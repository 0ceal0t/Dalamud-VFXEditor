using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components.SplitViews;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Formats.PbdFormat {
    public class PbdDeformer : IUiItem {
        public readonly ParsedShort SkeletonId = new( "Skeleton Id" );
        public readonly ParsedFloat UnknownScale = new( "Unknown Scale" );

        public readonly ParsedBool Enabled = new( "Enabled", true );
        public readonly List<PbdBone> Bones = [];
        private readonly CommandSplitView<PbdBone> BoneView;

        public PbdDeformer() {
            BoneView = new( "Bone", Bones, false, ( PbdBone bone, int idx ) => bone.Name.Value, () => new() );
        }

        public PbdDeformer( BinaryReader reader ) : this() {
            SkeletonId.Read( reader );
            reader.ReadUInt16(); // connection index
            var offset = reader.ReadInt32();
            UnknownScale.Read( reader );

            if( offset == 0 ) {
                Enabled.Value = false;
                return;
            }

            var savePos = reader.BaseStream.Position;
            reader.BaseStream.Position = offset;

            var boneCount = reader.ReadInt32();
            var stringOffsets = new List<short>();
            for( var i = 0; i < boneCount; i++ ) stringOffsets.Add( reader.ReadInt16() );
            FileUtils.PadTo( reader, 4 );
            for( var i = 0; i < boneCount; i++ ) Bones.Add( new( reader ) ); // read matrixes
            for( var i = 0; i < boneCount; i++ ) {
                reader.BaseStream.Position = offset + stringOffsets[i];
                Bones[i].Name.Read( reader );
            }

            reader.BaseStream.Position = savePos;
        }

        public void Draw() {
            SkeletonId.Draw();
            UnknownScale.Draw();

            Enabled.Draw();
            if( Enabled.Value ) {
                ImGui.Separator();
                BoneView.Draw();
            }
        }

        public void Write( BinaryWriter writer, List<PbdConnection> connections, Dictionary<PbdDeformer, long> offsets ) {
            SkeletonId.Write( writer );
            writer.Write( ( ushort )connections.IndexOf( x => x.Item == this ) );

            if( Enabled.Value ) offsets[this] = writer.BaseStream.Position;
            writer.Write( 0 ); // placeholder

            UnknownScale.Write( writer );
        }

        public void WriteData( BinaryWriter writer ) {
            if( !Enabled.Value ) return;
            var startPosition = writer.BaseStream.Position;

            writer.Write( Bones.Count );
            for( var i = 0; i < Bones.Count; i++ ) writer.Write( ( ushort )0 ); // placeholder
            FileUtils.PadTo( writer, 4 );

            foreach( var bone in Bones ) bone.Write( writer ); // write matrixes

            var boneNamePositions = new Dictionary<string, long>();
            foreach( var bone in Bones ) {
                boneNamePositions[bone.Name.Value] = writer.BaseStream.Position;
                bone.Name.Write( writer );
            }
            FileUtils.PadTo( writer, 4 );

            var endPosition = writer.BaseStream.Position;
            writer.BaseStream.Position = startPosition + 4;
            foreach( var bone in Bones ) {
                writer.Write( ( short )( boneNamePositions[bone.Name.Value] - startPosition ) );
            }

            writer.BaseStream.Position = endPosition;
        }
    }
}
