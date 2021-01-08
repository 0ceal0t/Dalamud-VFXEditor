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
    public class UIEffectorView : UIDropdownView
    {
        public AVFXBase AVFX;
        List<UIEffector> Effectors;

        public UIEffectorView(AVFXBase avfx) : base( "##EFFCT", "Select an Effector" )
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

        public override void OnNew()
        {
            AVFX.addEffector();
        }
        public override void OnDelete( int idx )
        {
            AVFX.removeEffector( idx );
        }
        public override void OnDraw( int idx )
        {
            Effectors[idx].Draw( id );
        }
        public override byte[] OnExport( int idx )
        {
            return Effectors[idx].Effector.toAVFX().toBytes();
        }
        public override void RefreshDesc( int idx )
        {
            Options[idx] = Effectors[idx].GetDescText();
        }
        public override void OnImport( AVFXNode node ) {
            AVFXEffector item = new AVFXEffector();
            item.read( node );
            AVFX.addEffector( item );
        }
    }
}
