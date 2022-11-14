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
            Color1.Read( reader, size );
            Color2.Read( reader, size );
            Color3.Read( reader, size );
            Color4.Read( reader, size );
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
        public readonly ParsedInt Frame1 = new( "Frame 1", size: 2 );
        public readonly ParsedInt Frame2 = new( "Frame 2", size: 2 );
        public readonly ParsedInt Frame3 = new( "Frame 3", size: 2 );
        public readonly ParsedInt Frame4 = new( "Frame 4", size: 2 );

        public AvfxSimpleFrames() : base( "Frms" ) { }

        public override void ReadContents( BinaryReader reader, int size ) {
            Frame1.Read( reader, 2 );
            Frame2.Read( reader, 2 );
            Frame3.Read( reader, 2 );
            Frame4.Read( reader, 2 );
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
