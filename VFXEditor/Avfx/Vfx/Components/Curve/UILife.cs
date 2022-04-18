using ImGuiNET;
using System;
using System.Collections.Generic;
using VFXEditor.AVFXLib;
using VFXEditor.AVFXLib.Curve;

namespace VFXEditor.Avfx.Vfx {
    public class UILife : UIItem {
        public readonly AVFXLife Life;
        private readonly List<UIBase> Parameters;

        public UILife( AVFXLife life ) {
            Life = life;
            Parameters = new List<UIBase> {
                new UIFloat( "Value", Life.Value ),
                new UIFloat( "Random Value", Life.ValRandom ),
                new UICombo<RandomType>( "Random Type", Life.ValRandomType )
            };
        }

        public override void Draw( string parentId ) {
            if( ImGui.TreeNode( "Life" + parentId ) ) {
                DrawBody( parentId );
                ImGui.TreePop();
            }
        }

        public override void DrawBody( string parentId ) {
            var id = parentId + "/Life";
            DrawList( Parameters, id );
        }

        public override string GetDefaultText() => "Life";

        public override bool IsAssigned() => Life.IsAssigned();
    }
}
