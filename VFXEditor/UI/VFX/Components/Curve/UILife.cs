using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UILife : UIItem {
        public AVFXLife Life;

        public UILife(AVFXLife life)
        {
            Life = life;
            Init();
        }
        public override void Init()
        {
            base.Init();
            //==========================
            Attributes.Add(new UIFloat("Value", Life.Value));
            Attributes.Add(new UIFloat("Random Value", Life.ValRandom));
            Attributes.Add(new UICombo<RandomType>("Random Type", Life.ValRandomType));
        }


        // ======= DRAW ================
        public override void Draw( string parentId )
        {
            if( ImGui.TreeNode( "Life" + parentId ) )
            {
                DrawBody( parentId );
                ImGui.TreePop();
            }
        }
        public override void DrawBody( string parentId )
        {
            var id = parentId + "/Life";
            // =====================
            DrawAttrs( id );
        }

        public override string GetDefaultText() {
            return "Life";
        }
    }
}
