using System.Collections.Generic;
using System.IO;

namespace VfxEditor.AVFXLib.Curve {
    public class AVFXCurve : AVFXBase {
        public readonly AVFXEnum<CurveBehavior> PreBehavior = new( "BvPr" );
        public readonly AVFXEnum<CurveBehavior> PostBehavior = new( "BvPo" );
        public readonly AVFXEnum<RandomType> Random = new( "RanT" );
        public readonly AVFXCurveKeys Keys = new();

        private readonly List<AVFXBase> Children;

        public AVFXCurve( string name ) : base( name ) {
            Children = new List<AVFXBase> {
                PreBehavior,
                PostBehavior,
                Random,
                Keys
            };
        }

        /* KeyC size key_count
         * children
         * Keys size key_data
         */

        public override void ReadContents( BinaryReader reader, int size ) => ReadNested( reader, Children, size );

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Children, assigned );

        protected override void WriteContents( BinaryWriter writer ) {
            WriteLeaf( writer, "KeyC", 4, Keys.Keys.Count );
            WriteNested( writer, Children );
        }
    }
}
