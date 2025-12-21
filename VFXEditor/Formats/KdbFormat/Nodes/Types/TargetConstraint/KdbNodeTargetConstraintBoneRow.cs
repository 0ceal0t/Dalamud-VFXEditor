using Dalamud.Bindings.ImGui;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types.TargetConstraint {
    public class KdbNodeTargetConstraintBoneRow : IUiItem {
        public readonly ParsedFnvHash Bone = new( "##Bone" );
        public readonly ParsedDouble Weight = new( "##Weight" );
        public readonly ParsedDouble3 Unknown = new( "##Unknown" );

        public void Draw() {
            ImGui.TableNextColumn();
            ImGui.SetNextItemWidth( 200 );
            Bone.Draw();

            ImGui.TableNextColumn();
            ImGui.SetNextItemWidth( 200 );
            Weight.Draw();

            ImGui.TableNextColumn();
            ImGui.SetNextItemWidth( 200 );
            Unknown.Draw();
        }
    }
}
