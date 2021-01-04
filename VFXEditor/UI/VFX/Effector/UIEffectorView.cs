using AVFXLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace VFXEditor.UI.VFX
{
    public class UIEffectorView : UIBase
    {
        public AVFXBase AVFX;
        List<UIEffector> Effectors;
        public int Selected = -1;
        public string[] Options;

        public UIEffectorView(AVFXBase avfx)
        {
            AVFX = avfx;
            Init();
        }
        public override void Init()
        {
            base.Init();
            Effectors = new List<UIEffector>();
            Options = new string[AVFX.Effectors.Count];
            int idx = 0;
            foreach( var effector in AVFX.Effectors )
            {
                var item = new UIEffector( effector, this );
                item.Idx = idx;
                Options[idx] = item.GetDescText();
                Effectors.Add( item );
                idx++;
            }
        }
        public void RefreshDesc( int idx )
        {
            Options[idx] = Effectors[idx].GetDescText();
        }
        public override void Draw(string parentId = "")
        {
            string id = "##EFFECT";
            bool validSelect = UIUtils.ViewSelect( id, "Select an Effector", ref Selected, Options );
            ImGui.SameLine();
            if( ImGui.Button( "+ NEW" + id ) )
            {
                AVFX.addEffector();
                Init();
            }
            if( validSelect )
            {
                ImGui.SameLine();
                if( UIUtils.RemoveButton( "DELETE" + id ) )
                {
                    AVFX.removeEffector( Selected );
                    Init();
                    validSelect = false;
                }
            }
            ImGui.Separator();
            // ====================
            if( validSelect )
            {
                Effectors[Selected].Draw( id );
            }
        }
    }
}
