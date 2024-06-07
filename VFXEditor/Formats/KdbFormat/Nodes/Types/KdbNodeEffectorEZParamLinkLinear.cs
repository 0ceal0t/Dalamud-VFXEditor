using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeEffectorEZParamLinkLinear : KdbNode {
        public override KdbNodeType Type => KdbNodeType.EffectorEZParamLinkLinear;

        public readonly ParsedDouble Unknown = new( "Unknown" );
        public readonly ParsedDouble Scale = new( "Scale" );
        public readonly ParsedDouble Offset = new( "Offset" );
        public readonly ParsedDouble2 Clamp = new( "Clamp" );
        public readonly ParsedBool EnableMin = new( "Enable Minimum" );
        public readonly ParsedBool EnableMax = new( "Enable Maximum" );

        public KdbNodeEffectorEZParamLinkLinear() : base() { }

        public KdbNodeEffectorEZParamLinkLinear( BinaryReader reader ) : this() { ReaderHeader( reader ); }

        public override void ReadBody( BinaryReader reader ) {
            Unknown.Read( reader );
            Scale.Read( reader );
            Offset.Read( reader );
            Clamp.Read( reader );
            EnableMin.Read( reader );
            EnableMax.Read( reader );
        }

        public override void WriteBody( BinaryWriter writer ) {
            Unknown.Write( writer );
            Scale.Write( writer );
            Offset.Write( writer );
            Clamp.Write( writer );
            EnableMin.Write( writer );
            EnableMax.Write( writer );
        }

        protected override void DrawBody( List<string> bones ) {
            Unknown.Draw();
            Scale.Draw();
            Offset.Draw();
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
