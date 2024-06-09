using ImGuiNET;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types.TargetConstraint {
    public class KdbNodeTargetConstraintBoneRow : IUiItem {
        public readonly ParsedFnvHash Bone = new( "##Bone" );
        public readonly ParsedDouble Unknown1 = new( "##Unknown 1" );
        public readonly ParsedDouble3 Unknown2 = new( "##Unknown 2" );

        public void Draw() {
            ImGui.TableNextColumn();
            ImGui.SetNextItemWidth( 200 );
            Bone.Draw();

            ImGui.TableNextColumn();
            ImGui.SetNextItemWidth( 200 );
            Unknown1.Draw();

            ImGui.TableNextColumn();
            ImGui.SetNextItemWidth( 200 );
            Unknown2.Draw();
        }
    }
}
