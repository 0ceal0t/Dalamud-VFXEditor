using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
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

        public readonly ParsedByteBool Unknown1 = new( "Unknown 1" );
        public readonly ParsedByteBool Unknown2 = new( "Unknown 2" );
        public readonly ParsedByteBool Unknown3 = new( "Unknown 3" );
        public readonly ParsedByteBool Unknown4 = new( "Unknown 4" );

        public KdbNodeEffectorEZParamLink() : base() { }

        public KdbNodeEffectorEZParamLink( BinaryReader reader ) : this() { ReaderHeader( reader ); }

        public override void ReadBody( BinaryReader reader ) {
            Start.Read( reader );
            Middle.Read( reader );
            End.Read( reader );

            StartTangent.Read( reader );
            MiddleTangent.Read( reader );
            EndTangent.Read( reader );

            Unknown1.Read( reader );
            Unknown2.Read( reader );
            Unknown3.Read( reader );
            Unknown4.Read( reader );
        }

        protected override void DrawBody( List<string> bones ) {
            Start.Draw();
            Middle.Draw();
            End.Draw();

            StartTangent.Draw();
            MiddleTangent.Draw();
            EndTangent.Draw();

            Unknown1.Draw();
            Unknown2.Draw();
            Unknown3.Draw();
            Unknown4.Draw();
        }

        protected override List<KdbSlot> GetInputSlots() => [
            new( ConnectionType.Input ),
        ];

        protected override List<KdbSlot> GetOutputSlots() => [
            new( ConnectionType.Output ),
        ];
    }
}
