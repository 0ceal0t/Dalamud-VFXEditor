using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.Ui.Components.Tables;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types.Source {
    public abstract class KdbNodeSource : KdbNode {
        public readonly List<KdbSourceBoneRow> Bones = [];
        protected readonly CommandTable<KdbSourceBoneRow> BoneTable;

        public KdbNodeSource() : base() {
            BoneTable = new( "Bones", true, false, Bones, [
                ( "Bone", ImGuiTableColumnFlags.None, -1 ),
                ( "Weight", ImGuiTableColumnFlags.None, -1 ),
            ],
            () => new() );
        }

        public override void UpdateBones( List<string> boneList ) {
            foreach( var bone in Bones ) bone.Name.Guess( boneList );
        }
    }
}
