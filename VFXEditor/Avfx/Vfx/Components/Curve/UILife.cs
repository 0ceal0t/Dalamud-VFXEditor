using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;

namespace VFXEditor.Avfx.Vfx {
    public class UILife : UIItem {
        public AVFXLife Life;
        private List<UIBase> Parameters;

        public UILife( AVFXLife life ) {
            Life = life;
            Init();
        }

        public override void Init() {
            base.Init();
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
    }
}
