using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.MdlFormat.Element {
    public class MdlEid : IUiItem {
        private readonly ParsedUInt ElementId = new( "Bind Point Id" );
        private readonly ParsedString ParentBone = new( "Parent Bone" ); // chara/weapon/w2366/obj/body/b0001/model/w2366b0001.mdl
        private readonly ParsedFloat3 Translation = new( "Translation" );
        private readonly ParsedFloat3 Rotation = new( "Rotation" );

        public MdlEid() { }

        public MdlEid( Dictionary<uint, string> strings, BinaryReader reader ) : this() {
            ElementId.Read( reader );
            ParentBone.Value = strings[reader.ReadUInt32()];
            Translation.Read( reader );
            Rotation.Read( reader );
        }

        public void Draw() {
            ElementId.Draw();
            ParentBone.Draw();
            Translation.Draw();
            Rotation.Draw();
        }
    }
}
