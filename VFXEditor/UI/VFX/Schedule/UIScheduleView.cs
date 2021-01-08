using AVFXLib.AVFX;
using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIScheduleView : UIDropdownView
    {
        public AVFXBase AVFX;
        List<UIScheduler> Schedulers;

        public UIScheduleView(AVFXBase avfx) : base( "##SCHED", "Select a Scheduler", allowNew:false, allowDelete:false )
        {
            AVFX = avfx;
            Init();
        }
        public override void Init()
        {
            base.Init();
            Schedulers = new List<UIScheduler>();
            Options = new string[AVFX.Schedulers.Count];
            int idx = 0;
            foreach( var sched in AVFX.Schedulers )
            {
                var item = new UIScheduler( sched, this );
                item.Idx = idx;
                Options[idx] = item.GetDescText();
                Schedulers.Add( item );
                idx++;
            }
        }

        public override void OnNew() { }
        public override void OnDelete( int idx ) { }
        public override void OnDraw( int idx )
        {
            Schedulers[idx].Draw( id );
        }
        public override byte[] OnExport( int idx ){ return new byte[0]; }
        public override void RefreshDesc( int idx )
        {
            Options[idx] = Schedulers[idx].GetDescText();
        }
        public override void OnImport( AVFXNode node ) {}
    }
}
