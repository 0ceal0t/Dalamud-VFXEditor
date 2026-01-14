using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types.Effector {
    public enum ParamLinkTangent {
        Step,
        Linear,
        Spline,
        Max
    }

    public class KdbNodeEffectorEZParamLink : KdbNode {
        public override KdbNodeType Type => KdbNodeType.EffectorEZParamLink;

        public readonly ParsedDouble2 Start = new( "Start" );
        public readonly ParsedDouble2 Middle = new( "Middle" );
        public readonly ParsedDouble2 End = new( "End" );

        public readonly ParsedEnum<ParamLinkTangent> StartTangent = new( "Start Tangent" );
        public readonly ParsedEnum<ParamLinkTangent> MiddleTangent = new( "Middle Tangent" );
        public readonly ParsedEnum<ParamLinkTangent> EndTangent = new( "End Tangent" );

        public readonly ParsedByteBool Unknown3 = new( "Unknown 3" );
        public readonly ParsedByteBool Unknown4 = new( "Unknown 4" );
        public readonly ParsedByteBool Unknown5 = new( "Unknown 5" );
        public readonly ParsedByteBool Unknown6 = new( "Unknown 6" );

        public KdbNodeEffectorEZParamLink() : base() { }

        public KdbNodeEffectorEZParamLink( BinaryReader reader ) : this() { ReaderHeader( reader ); }

        public override void ReadBody( BinaryReader reader ) {
            Start.Read( reader );
            Middle.Read( reader );
            End.Read( reader );

            StartTangent.Read( reader );
            MiddleTangent.Read( reader );
            EndTangent.Read( reader );

            Unknown3.Read( reader );
            Unknown4.Read( reader );
            Unknown5.Read( reader );
            Unknown6.Read( reader );
        }

        public override void WriteBody( BinaryWriter writer ) {
            Start.Write( writer );
            Middle.Write( writer );
            End.Write( writer );

            StartTangent.Write( writer );
            MiddleTangent.Write( writer );
            EndTangent.Write( writer );

            Unknown3.Write( writer );
            Unknown4.Write( writer );
            Unknown5.Write( writer );
            Unknown6.Write( writer );
        }

        protected override void DrawBody( List<string> bones ) {
            Start.Draw();
            Middle.Draw();
            End.Draw();

            StartTangent.Draw();
            MiddleTangent.Draw();
            EndTangent.Draw();

            Unknown3.Draw();
            Unknown4.Draw();
            Unknown5.Draw();
            Unknown6.Draw();
        }

        protected override List<KdbSlot> GetInputSlots() => [
            new( ConnectionType.Input ),
        ];

        protected override List<KdbSlot> GetOutputSlots() => [
            new( ConnectionType.Output ),
        ];
    }
}
