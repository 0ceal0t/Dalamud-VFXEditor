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
        public AVFXBase AVFX;
        public List<UIBase> Models;
        public UIModelSplitView ModelSplit;

        public UIModelView(AVFXBase avfx)
        {
            AVFX = avfx;
            Init();
        }
        public override void Init()
        {
            base.Init();
            Models = new List<UIBase>();
            foreach( var model in AVFX.Models )
            {
                Models.Add( new UIModel( model, this ) );
            }
            ModelSplit = new UIModelSplitView( Models, this );
        }

        public override void Draw(string parentId = "")
        {
            string id = "##MDL";
            ModelSplit.Draw( id );
        }
    }
}
