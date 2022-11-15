using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxSimpleColors : AvfxBase {
        public readonly ParsedIntColor Color1 = new( "Color 1" );
        public readonly ParsedIntColor Color2 = new( "Color 2" );
        public readonly ParsedIntColor Color3 = new( "Color 3" );
        public readonly ParsedIntColor Color4 = new( "Color 4" );

        public AvfxSimpleColors() : base( "Cols" ) { }

        public override void ReadContents( BinaryReader reader, int size ) {
            Color1.Read( reader );
            Color2.Read( reader );
            Color3.Read( reader );
            Color4.Read( reader );
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            Color1.Write( writer );
            Color2.Write( writer );
            Color3.Write( writer );
            Color4.Write( writer );
        }
    }

    public class AvfxSimpleFrames : AvfxBase {
        public readonly ParsedShort Frame1 = new( "Frame 1" );
        public readonly ParsedShort Frame2 = new( "Frame 2" );
        public readonly ParsedShort Frame3 = new( "Frame 3" );
        public readonly ParsedShort Frame4 = new( "Frame 4" );

        public AvfxSimpleFrames() : base( "Frms" ) { }

        public override void ReadContents( BinaryReader reader, int size ) {
            Frame1.Read( reader );
            Frame2.Read( reader );
            Frame3.Read( reader );
            Frame4.Read( reader );
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            Frame1.Write( writer );
            Frame2.Write( writer );
            Frame3.Write( writer );
            Frame4.Write( writer );
        }
    }
}
