using AVFXLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using AVFXLib.AVFX;

namespace VFXEditor.UI.VFX
{
    public class UIEmitterView : UIDropdownView
    {
        public AVFXBase AVFX;
        List<UIEmitter> Emitters;

        public UIEmitterView(AVFXBase avfx) : base( "##EMIT", "Select an Emitter" )
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
        public override void OnNew()
        {
            AVFX.addEmitter();
        }
        public override void OnDelete( int idx )
        {
            AVFX.removeEmitter( idx );
        }
        public override void OnDraw( int idx )
        {
            Emitters[idx].Draw( id );
        }
        public override byte[] OnExport( int idx )
        {
            return Emitters[idx].Emitter.toAVFX().toBytes();
        }
        public override void RefreshDesc( int idx )
        {
            Options[idx] = Emitters[idx].GetDescText();
        }
        public override void OnImport( AVFXNode node ) {
            AVFXEmitter item = new AVFXEmitter();
            item.read( node );
            AVFX.addEmitter( item );
        }
    }
}
