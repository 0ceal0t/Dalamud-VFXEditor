using AVFXLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace VFXEditor.UI.VFX
{
    public class UIBinderView : UIBase
    {
        public AVFXBase AVFX;
        List<UIBinder> Binders;
        public int Selected = -1;
        public string[] Options;

        public UIBinderView(AVFXBase avfx)
        {
            AVFX = avfx;
            Init();
        }
        public override void Init()
        {
            base.Init();
            Binders = new List<UIBinder>();
            Options = new string[AVFX.Binders.Count];
            int idx = 0;
            foreach( var binder in AVFX.Binders )
            {
                var item = new UIBinder( binder, this );
                item.Idx = idx;
                Options[idx] = item.GetDescText();
                Binders.Add( item );
                idx++;
            }
        }
        public void RefreshDesc( int idx )
        {
            Options[idx] = Binders[idx].GetDescText();
        }
        public override void Draw(string parentId = "")
        {
            string id = "##BIND";
            bool validSelect = UIUtils.ViewSelect( id, "Select a Binder", ref Selected, Options );
            ImGui.SameLine();
            if( ImGui.Button( "+ NEW" + id ) )
            {
                AVFX.addBinder();
                Init();
            }
            if( validSelect )
            {
                ImGui.SameLine();
                if( UIUtils.RemoveButton( "DELETE" + id ) )
                {
                    AVFX.removeBinder( Selected );
                    Init();
                    validSelect = false;
                }
            }
            ImGui.Separator();
            // ====================
            if( validSelect )
            {
                Binders[Selected].Draw( id );
            }
        }
    }
}
