using System;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.TmbFormat.Tmfcs {
    public class TmfcRow {
        public const int Size = 0x18;

        public readonly ParsedUInt Unk1 = new( "Unknown 1" );
        public readonly ParsedFloat Time = new( "Time" );
        public readonly ParsedFloat Value = new( "Value" );
        public readonly ParsedFloat TangentIn = new( "Tangent In" );
        public readonly ParsedFloat TangentOut = new( "Tangent Out" );

        // The initial value, recompute it on each write
        private readonly float InitInverseDuration;

        public TmfcRow( BinaryReader reader ) {
            Unk1.Read( reader );
            Time.Read( reader );
            InitInverseDuration = reader.ReadSingle();
            Value.Read( reader );
            TangentIn.Read( reader );
            TangentOut.Read( reader );
        }

        public void Write( BinaryWriter writer, TmfcRow? nextRow ) {
            Unk1.Write( writer );
            Time.Write( writer );
            writer.Write( GetInverseDuration( nextRow ) );
            Value.Write( writer );
            TangentIn.Write( writer );
            TangentOut.Write( writer );
        }

        private float GetInverseDuration( TmfcRow? nextRow ) {
            if( nextRow == null ) return 0f;
            var duration = nextRow.Time.Value - Time.Value;
            if( duration <= 0f ) return 0f;
            // Give some leniency for the initial value so floating point weirdness doesn't mess with verification
            var expected = 1f / duration;
            if( Math.Abs( InitInverseDuration - expected ) <= Math.Abs( expected ) * 1e-5f ) return InitInverseDuration;
            return expected;
        }

        public void Draw() {
            Unk1.Draw();
            Time.Draw();
            Value.Draw();
            TangentIn.Draw();
            TangentOut.Draw();
        }
    }
}