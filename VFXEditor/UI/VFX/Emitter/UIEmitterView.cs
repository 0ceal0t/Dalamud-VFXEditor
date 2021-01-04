using AVFXLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace VFXEditor.UI.VFX
{
    public class UIEmitterView : UIBase
    {
        public AVFXBase AVFX;
        List<UIEmitter> Emitters;
        public int Selected = -1;
        public string[] Options;

        public UIEmitterView(AVFXBase avfx)
        {
            AVFX = avfx;
            Init();
        }
        public override void Init()
        {
            base.Init();
            Emitters = new List<UIEmitter>();
            Options = new string[AVFX.Emitters.Count];
            int idx = 0;
            foreach( var emitter in AVFX.Emitters )
            {
                var item = new UIEmitter( emitter, this );
                item.Idx = idx;
                Options[idx] = item.GetDescText();
                Emitters.Add( item );
                idx++;
            }
        }
        public void RefreshDesc( int idx )
        {
            Options[idx] = Emitters[idx].GetDescText();
        }
        public override void Draw(string parentId = "")
        {
            string id = "##EMIT";
            bool validSelect = UIUtils.ViewSelect( id, "Select an Emitter", ref Selected, Options );
            ImGui.SameLine();
            if( ImGui.Button( "+ NEW" + id ) )
            {
                AVFX.addEmitter();
                Init();
            }
            if( validSelect )
            {
                ImGui.SameLine();
                if( UIUtils.RemoveButton( "DELETE" + id ) )
                {
                    AVFX.removeEmitter( Selected );
                    Init();
                    validSelect = false;
                }
            }
            ImGui.Separator();
            // ====================
            if( validSelect )
            {
                Emitters[Selected].Draw( id );
            }
        }
    }
}
