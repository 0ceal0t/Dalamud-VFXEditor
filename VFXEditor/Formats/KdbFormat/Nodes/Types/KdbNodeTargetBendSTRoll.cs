using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeTargetBendSTRoll : KdbNode {
        public override KdbNodeType Type => KdbNodeType.TargetBendSTRoll;

        public KdbNodeTargetBendSTRoll() : base() { }

        public KdbNodeTargetBendSTRoll( BinaryReader reader ) : this() { ReaderHeader( reader ); }

        public override void ReadBody( BinaryReader reader ) { }

        protected override void DrawBody( List<string> bones ) {
            /*
             * target_node: Identifier,

    a: f64, // No idea about these 3
    b: f64, //
    c: f64, //

    #[br(count = 3)]
    vec1: Vec<f64>,

    #[br(count = 3)]
    vec2: Vec<f64>,

    #[br(count = 3)]
    vec3: Vec<f64>,

    d: f64,

    #[br(count = 4)]
    vec4: Vec<f64>,

    #[br(count = 4)]
    vec5: Vec<f64>,

    e: f64,

    j: u32,

    k: u8,

    #[br(count = 3)]
    padding: Vec<u8>
             */

        }

        protected override List<KdbSlot> GetInputSlots() => [
            new( ConnectionType.Roll ),
            new( ConnectionType.BendS ),
            new( ConnectionType.BendT ),
        ];

        protected override List<KdbSlot> GetOutputSlots() => [];
    }
}
