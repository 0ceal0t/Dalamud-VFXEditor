using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.KdbFormat.Components;
using VfxEditor.Formats.KdbFormat.Nodes;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.KdbFormat.UnknownB {
    public class UnknownBItem : IUiItem {
        public readonly ParsedFnvKdbNodes Node;
        public readonly ParsedUInt Unknown1 = new( "Unknown 1" );
        public readonly ParsedUInt Unknown2 = new( "Unknown 2" );
        public readonly ParsedDouble4 Unknown3 = new( "Unknown 3" );
        public readonly ParsedDouble4 Unknown4 = new( "Unknown 4" );
        public readonly ParsedDouble4 Unknown5 = new( "Unknown 5" );

        public UnknownBItem( List<KdbNode> nodes ) {
            Node = new( "Node", nodes );
        }

        public UnknownBItem( BinaryReader reader, List<KdbNode> nodes ) : this( nodes ) {
            Node.Read( reader );
            Unknown1.Read( reader );
            Unknown2.Read( reader );
            Unknown3.Read( reader );
            Unknown4.Read( reader );
            Unknown5.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            Node.Write( writer );
            Unknown1.Write( writer );
            Unknown2.Write( writer );
            Unknown3.Write( writer );
            Unknown4.Write( writer );
            Unknown5.Write( writer );
        }

        public void Draw() {
            Node.Draw();
            Unknown1.Draw();
            Unknown2.Draw();
            Unknown3.Draw();
            Unknown4.Draw();
            Unknown5.Draw();
        }
    }
}
