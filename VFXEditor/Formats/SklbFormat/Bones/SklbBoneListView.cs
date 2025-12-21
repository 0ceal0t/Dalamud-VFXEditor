using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
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

            using( var disabled = ImRaii.Disabled() ) {
                using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                    ImGui.Text( FontAwesomeIcon.InfoCircle.ToIconString() );
                }
                ImGui.SameLine();
                ImGui.Text( "Bones can be re-ordered by dragging them" );
            }

            foreach( var (bone, idx) in Bones.WithIndex() ) {
                var text = $"[{idx:D3}]  {bone.Name.Value}";
                ImGui.TreeNodeEx( text, ImGuiTreeNodeFlags.Framed | ImGuiTreeNodeFlags.Bullet | ImGuiTreeNodeFlags.FramePadding | ImGuiTreeNodeFlags.NoTreePushOnOpen );
                if( UiUtils.DrawDragDrop( Bones, bone, text, ref DraggingItem, "BONE-LIST", true ) ) break;
            }
        }
    }
}
