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
    public class UIScheduleView : UIDropdownView<UIScheduler>
    {
        public AVFXBase AVFX;

        public UIScheduleView(AVFXBase avfx) : base( "##SCHED", "Select a Scheduler", allowNew:false, allowDelete:false )
        {
            AVFX = avfx;
            //=============
            foreach( var sched in AVFX.Schedulers ) {
                var item = new UIScheduler( sched, this );
                Items.Add( item );
            }
            SetupIdx();
        }

        public override UIScheduler OnNew() { return null; }
        public override void OnDelete( UIScheduler item ) {}
        public override byte[] OnExport( UIScheduler item ) {
            return new byte[0];
        }
        public override UIScheduler OnImport( AVFXNode node ) { return null; }
    }
}
