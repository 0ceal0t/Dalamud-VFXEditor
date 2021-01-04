using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIModelView : UIBase
    {
        List<UIBase> Models = new List<UIBase>();
        UISplitView ModelSplit;

        public UIModelView(AVFXBase avfx)
        {
            foreach (var model in avfx.Models)
            {
                Models.Add(new UIModel(model));
            }
            ModelSplit = new UISplitView( Models );
        }

        public override void Draw(string parentId = "")
        {
            string id = "##MDL";
            ModelSplit.Draw( id );
        }
    }
}
