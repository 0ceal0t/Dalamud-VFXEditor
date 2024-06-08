using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Utils;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types.Effector {
    public class KdbNodeEffectorEZParamLinkLinear : KdbNode {
        public override KdbNodeType Type => KdbNodeType.EffectorEZParamLinkLinear;

        public readonly ParsedDouble InputOffset = new( "Input Offset" );
        public readonly ParsedDouble Scale = new( "Scale" );
        public readonly ParsedDouble OutputOffset = new( "Output Offset" );
        public readonly ParsedDouble2 Clamp = new( "Clamp" );
        public readonly ParsedByteBool EnableMin = new( "Enable Minimum" );
        public readonly ParsedByteBool EnableMax = new( "Enable Maximum" );

        public KdbNodeEffectorEZParamLinkLinear() : base() { }

        public KdbNodeEffectorEZParamLinkLinear( BinaryReader reader ) : this() { ReaderHeader( reader ); }

        public override void ReadBody( BinaryReader reader ) {
            InputOffset.Read( reader );
            Scale.Read( reader );
            OutputOffset.Read( reader );
            Clamp.Read( reader );
            EnableMin.Read( reader );
            EnableMax.Read( reader );
            reader.ReadBytes( 6 ); // padding
        }

        public override void WriteBody( BinaryWriter writer ) {
            InputOffset.Write( writer );
            Scale.Write( writer );
            OutputOffset.Write( writer );
            Clamp.Write( writer );
            EnableMin.Write( writer );
            EnableMax.Write( writer );
            FileUtils.Pad( writer, 6 );
        }

        protected override void DrawBody( List<string> bones ) {
            InputOffset.Draw();
            Scale.Draw();
            OutputOffset.Draw();
            Clamp.Draw();
            EnableMin.Draw();
            EnableMax.Draw();
        }

        protected override List<KdbSlot> GetInputSlots() => [
            new( ConnectionType.Input ),
        ];

        protected override List<KdbSlot> GetOutputSlots() => [
            new( ConnectionType.Output ),
        ];
    }
}
