using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Curve;

namespace VfxEditor.AVFX.VFX {
    public class UILife : UIAssignableItem {
        public readonly AVFXLife Life;
        private readonly List<IUIBase> Parameters;

        public UILife( AVFXLife life ) {
            Life = life;
            Parameters = new List<IUIBase> {
                new UIFloat( "Value", Life.Value ),
                new UIFloat( "Random Value", Life.ValRandom ),
                new UICombo<RandomType>( "Random Type", Life.ValRandomType )
            };
        }

        public override void DrawAssigned( string parentId ) {
            var id = parentId + "/Life";
            IUIBase.DrawList( Parameters, id );
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
