using System.Collections.Generic;
using System.IO;

namespace VFXEditor.AVFXLib {
    public class AVFXGenericData : AVFXBase {
        protected List<AVFXBase> Children;

        public AVFXGenericData() : base( "Data" ) {
        }

        public override void ReadContents( BinaryReader reader, int size ) => ReadNested( reader, Children, size );

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Children, assigned );

        protected override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Children );
    }
}
