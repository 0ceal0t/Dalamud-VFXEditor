using OtterGui.Raii;
using System;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    [Flags]
    public enum ConditionType1st {
        Attr = 0x01,
        Label = 0x02,
        Equal = 0x04,
        Bank = 0x08,
        ExternalId1st = 0x10,
        Unknown = 0x40
    }

    public enum ConditionType2nd {
        None = 0x00,
        Frame = 0x01,
        Volume = 0x02,
        Pan = 0x03,
        Count = 0x04,
        Priority = 0x05,
        ExternalId = 0x06,
        TypeMask = 0x0F,
        GT = 0x10,
        LT = 0x20,
        LE = 0x30,
        EQ = 0x40,
        NE = 0x50,
        CondMask = 0xF0
    }

    public enum JoinType {
        And,
        Or
    }

    public class AttributeExtendData {
        public readonly ParsedEnum<ConditionType1st> FirstCondition = new( "First Condition", size: 1 );
        public readonly ParsedEnum<ConditionType2nd> SecondCondition = new( "Second Condition", size: 1 );
        public readonly ParsedEnum<JoinType> JoinTypeSelect = new( "Join Type", size: 1 );
        public readonly ParsedByte NumberOfConditions = new( "Number of Conditions" );
        public readonly ParsedInt SelfArgument = new( "Self Argument" );
        public readonly ParsedInt TargetArgument = new( "Target Argument" );

        public readonly AttributeResultCommand Result = new();

        public AttributeExtendData() { }

        public void Read( BinaryReader reader ) {
            FirstCondition.Read( reader );
            SecondCondition.Read( reader );
            JoinTypeSelect.Read( reader );
            NumberOfConditions.Read( reader );
            SelfArgument.Read( reader );
            TargetArgument.Read( reader );

            Result.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            FirstCondition.Write( writer );
            SecondCondition.Write( writer );
            JoinTypeSelect.Write( writer );
            NumberOfConditions.Write( writer );
            SelfArgument.Write( writer );
            TargetArgument.Write( writer );

            Result.Write( writer );
        }

        public void Draw() {
            FirstCondition.Draw();
            SecondCondition.Draw();
            JoinTypeSelect.Draw();
            NumberOfConditions.Draw();
            SelfArgument.Draw();
            TargetArgument.Draw();

            using var _ = ImRaii.PushId( "Result" );
            Result.Draw();
        }
    }
}
