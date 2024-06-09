using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing.Int;
using VfxEditor.Ui.Components.Tables;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types.TargetConstraint {
    public abstract class KdbNodeTargetConstraint : KdbNode {
        public readonly ParsedFnvHash Bone = new( "Bone" );

        public readonly List<KdbNodeTargetConstraintBoneRow> Bones = [];
        protected readonly CommandTable<KdbNodeTargetConstraintBoneRow> BoneTable;

        public KdbNodeTargetConstraint() : base() {
            BoneTable = new( "Bone", true, false, Bones, [
                ( "Bone", ImGuiTableColumnFlags.None, -1 ),
                ( "Unknown 1", ImGuiTableColumnFlags.None, -1 ),
                ( "Unknown 2", ImGuiTableColumnFlags.None, -1 ),
                ], () => new() );
        }

        public override void ReadBody( BinaryReader reader ) {
            Bone.Read( reader );

            var boneCount = reader.ReadUInt32();
            var bonePosition = reader.BaseStream.Position + reader.ReadUInt32();
            reader.ReadUInt32();
            var pos1 = reader.BaseStream.Position + reader.ReadUInt32();
            reader.ReadUInt32();
            var pos2 = reader.BaseStream.Position + reader.ReadUInt32();

            ReadTargetConstraintBody( reader );

            for( var i = 0; i < boneCount; i++ ) Bones.Add( new() );
            reader.BaseStream.Position = bonePosition;
            foreach( var bone in Bones ) bone.Bone.Read( reader );
            reader.BaseStream.Position = pos1;
            foreach( var bone in Bones ) bone.Unknown1.Read( reader );
            reader.BaseStream.Position = pos2;
            foreach( var bone in Bones ) bone.Unknown2.Read( reader );
        }

        protected abstract void ReadTargetConstraintBody( BinaryReader reader );

        public override void WriteBody( BinaryWriter writer ) {
            Bone.Write( writer );

            var placeHolderPos = writer.BaseStream.Position;
            writer.Write( 0 );
            writer.Write( 0 );
            writer.Write( 0 );
            writer.Write( 0 );
            writer.Write( 0 );
            writer.Write( 0 );

            WriteTargetConstraintBody( writer );

            var bonePosition = writer.BaseStream.Position;
            foreach( var bone in Bones ) bone.Bone.Write( writer );
            var pos1 = writer.BaseStream.Position;
            foreach( var bone in Bones ) bone.Unknown1.Write( writer );
            var pos2 = writer.BaseStream.Position;
            foreach( var bone in Bones ) bone.Unknown2.Write( writer );

            var savePos = writer.BaseStream.Position;
            writer.BaseStream.Position = placeHolderPos;
            writer.Write( Bones.Count );
            writer.Write( ( uint )( bonePosition - writer.BaseStream.Position ) );
            writer.Write( Bones.Count );
            writer.Write( ( uint )( pos1 - writer.BaseStream.Position ) );
            writer.Write( Bones.Count );
            writer.Write( ( uint )( pos2 - writer.BaseStream.Position ) );
            writer.BaseStream.Position = savePos;
        }

        protected abstract void WriteTargetConstraintBody( BinaryWriter writer );

        public override void UpdateBones( List<string> boneList ) {
            Bone.Guess( boneList );
            foreach( var bone in Bones ) bone.Bone.Guess( boneList );
        }

        protected override List<KdbSlot> GetInputSlots() => [];

        protected override List<KdbSlot> GetOutputSlots() => [];
    }
}
