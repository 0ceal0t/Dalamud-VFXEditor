using Dalamud.Bindings.ImGui;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types.Source {
    public class KdbSourceBoneRow : IUiItem {
        public readonly ParsedFnvHash Name = new( "##Bone" );
        public readonly ParsedDouble Weight = new( "##Weight" );

        public void Draw() {
            ImGui.TableNextColumn();
            ImGui.SetNextItemWidth( 200 );
            Name.Draw();

            ImGui.TableNextColumn();
            ImGui.SetNextItemWidth( 200 );
            Weight.Draw();
        }
    }
}
