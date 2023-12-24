using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.SklbFormat.Bones;
using VfxEditor.Utils;

namespace VfxEditor.Formats.SklbFormat.Bones {
    public class SklbBoneListView {
        private readonly List<SklbBone> Bones;
        private SklbBone DraggingItem;

        public SklbBoneListView( List<SklbBone> bones ) {
            Bones = bones;
        }

        public void Draw() {
            using var child = ImRaii.Child( "Child" );

            foreach( var (bone, idx) in Bones.WithIndex() ) {
                var text = "[" + $"{idx}".PadLeft( 3, '0' ) + $"]  {bone.Name.Value}";
                ImGui.TreeNodeEx( text, ImGuiTreeNodeFlags.Framed | ImGuiTreeNodeFlags.Bullet | ImGuiTreeNodeFlags.FramePadding | ImGuiTreeNodeFlags.NoTreePushOnOpen );
                if( UiUtils.DrawDragDrop( Bones, bone, text, ref DraggingItem, "BONE-LIST", true ) ) break;
            }
        }
    }
}
