using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.MdlFormat.Utils;
using VfxEditor.Ui.Components.SplitViews;

namespace VfxEditor.Formats.MdlFormat.Bone {
    public class MdlBoneTables {
        public readonly List<MdlBoneTable> Tables = [];
        private readonly UiSplitView<MdlBoneTable> BoneTableView;

        public MdlBoneTables() {
            BoneTableView = new( "Bone Table", Tables, false );
        }

        public MdlBoneTables( BinaryReader reader, int count, MdlFileData data ) : this() {
            for( var i = 0; i < count; i++ ) Tables.Add( new( reader, data.BoneStrings ) );
        }

        public virtual void Write( BinaryWriter writer, MdlWriteData data ) {
            foreach( var table in Tables ) table.Write( writer, data );
        }

        public void Draw() => BoneTableView.Draw();
    }
}
