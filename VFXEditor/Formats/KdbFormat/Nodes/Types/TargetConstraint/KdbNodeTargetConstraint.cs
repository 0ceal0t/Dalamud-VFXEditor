using Dalamud.Bindings.ImGui;
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
                ( "Weight", ImGuiTableColumnFlags.None, -1 ),
                ( "Unknown", ImGuiTableColumnFlags.None, -1 ),
                ], () => new() );
        }

        public override void ReadBody( BinaryReader reader ) {
            Bone.Read( reader );

            var boneCount = reader.ReadUInt32();
            var bonePosition = reader.BaseStream.Position + reader.ReadUInt32();
            reader.ReadUInt32();
            var weightPosition = reader.BaseStream.Position + reader.ReadUInt32();
            reader.ReadUInt32();
            var unkPos = reader.BaseStream.Position + reader.ReadUInt32();

            ReadTargetConstraintBody( reader );

            for( var i = 0; i < boneCount; i++ ) Bones.Add( new() );
            reader.BaseStream.Position = bonePosition;
            foreach( var bone in Bones ) bone.Bone.Read( reader );
            reader.BaseStream.Position = weightPosition;
            foreach( var bone in Bones ) bone.Weight.Read( reader );
            reader.BaseStream.Position = unkPos;
            foreach( var bone in Bones ) bone.Unknown.Read( reader );
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
            var weightPosition = writer.BaseStream.Position;
            foreach( var bone in Bones ) bone.Weight.Write( writer );
            var unkPos = writer.BaseStream.Position;
            foreach( var bone in Bones ) bone.Unknown.Write( writer );

            var savePos = writer.BaseStream.Position;
            writer.BaseStream.Position = placeHolderPos;
            writer.Write( Bones.Count );
            writer.Write( ( uint )( bonePosition - writer.BaseStream.Position ) );
            writer.Write( Bones.Count );
            writer.Write( ( uint )( weightPosition - writer.BaseStream.Position ) );
            writer.Write( Bones.Count );
            writer.Write( ( uint )( unkPos - writer.BaseStream.Position ) );
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
