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
    public class UIBinderView : UIDropdownView
    {
        public AVFXBase AVFX;
        List<UIBinder> Binders;

        public UIBinderView(AVFXBase avfx) : base( "##BIND", "Select a Binder" )
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

        public override void OnNew()
        {
            AVFX.addBinder();
        }
        public override void OnDelete( int idx )
        {
            AVFX.removeBinder( idx );
        }
        public override void OnDraw( int idx )
        {
            if( idx >= Binders.Count ) return;
            Binders[idx].Draw( id );
        }
        public override byte[] OnExport(int idx )
        {
            return Binders[idx].Binder.toAVFX().toBytes();
        }
        public override void RefreshDesc( int idx )
        {
            Options[idx] = Binders[idx].GetDescText();
        }
        public override void OnImport( AVFXNode node ) {
            AVFXBinder item = new AVFXBinder();
            item.read( node );
            AVFX.addBinder( item );
        }
    }
}
