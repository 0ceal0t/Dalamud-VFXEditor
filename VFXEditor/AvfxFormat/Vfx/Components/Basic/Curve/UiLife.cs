using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Curve;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiLife : UiAssignableItem {
        public readonly AVFXLife Life;
        private readonly List<IUiBase> Parameters;

        public UiLife( AVFXLife life ) {
            Life = life;
            Parameters = new List<IUiBase> {
                new UiFloat( "Value", Life.Value ),
                new UiFloat( "Random Value", Life.ValRandom ),
                new UiCombo<RandomType>( "Random Type", Life.ValRandomType )
            };
        }

        public override void DrawAssigned( string parentId ) {
            var id = parentId + "/Life";
            IUiBase.DrawList( Parameters, id );
        }

        public override void DrawUnassigned( string id ) {
            if( ImGui.SmallButton( "+ Life" + id ) ) {
                AVFXBase.RecurseAssigned( Life, true );
            }
        }

        public override string GetDefaultText() => "Life";

        public override bool IsAssigned() => Life.IsAssigned();
    }
}
