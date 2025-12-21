using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Ui.Components.Tables;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types.Source {
    public enum LinkType {
        None = 0,
        Bone = 2,
    }

    public abstract class KdbNodeSource : KdbNode {
        public readonly List<KdbSourceBoneRow> Bones = [];
        protected readonly CommandTable<KdbSourceBoneRow> BoneTable;

        public KdbNodeSource() : base() {
            BoneTable = new( "Bones", true, false, Bones, [
                ( "Bone", ImGuiTableColumnFlags.None, -1 ),
                ( "Weight", ImGuiTableColumnFlags.None, -1 ),
            ],
            () => new() );
        }

        public override void ReadBody( BinaryReader reader ) {
            var boneCount = reader.ReadUInt32();
            var bonePosition = reader.BaseStream.Position + reader.ReadUInt32();
            reader.ReadUInt32(); // weight count, same as bone count
            var weightPosition = reader.BaseStream.Position + reader.ReadUInt32();

            ReadSourceBody( reader );

            for( var i = 0; i < boneCount; i++ ) Bones.Add( new() );
            reader.BaseStream.Position = bonePosition;
            foreach( var bone in Bones ) bone.Name.Read( reader );
            reader.BaseStream.Position = weightPosition;
            foreach( var bone in Bones ) bone.Weight.Read( reader );
        }

        protected abstract void ReadSourceBody( BinaryReader reader );

        public override void WriteBody( BinaryWriter writer ) {
            var placeHolderPos = writer.BaseStream.Position;
            writer.Write( 0 );
            writer.Write( 0 );
            writer.Write( 0 );
            writer.Write( 0 );

            WriteSourceBody( writer );

            var bonePosition = writer.BaseStream.Position;
            foreach( var bone in Bones ) bone.Name.Write( writer );
            var weightPosition = writer.BaseStream.Position;
            foreach( var bone in Bones ) bone.Weight.Write( writer );

            var savePos = writer.BaseStream.Position;
            writer.BaseStream.Position = placeHolderPos;
            writer.Write( Bones.Count );
            writer.Write( ( uint )( bonePosition - writer.BaseStream.Position ) );
            writer.Write( Bones.Count );
            writer.Write( ( uint )( weightPosition - writer.BaseStream.Position ) );
            writer.BaseStream.Position = savePos;
        }

        protected abstract void WriteSourceBody( BinaryWriter writer );

        public override void UpdateBones( List<string> boneList ) {
            foreach( var bone in Bones ) bone.Name.Guess( boneList );
        }
    }
}
