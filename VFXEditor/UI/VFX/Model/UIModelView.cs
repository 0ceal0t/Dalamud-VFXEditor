using AVFXLib.AVFX;
using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.Data.DirectX;

namespace VFXEditor.UI.VFX
{
    public class UIModelView : UINodeSplitView<UIModel>
    {
        public Model3D Mdl3D;
        public UIMain Main;

        public UIModelView(UIMain main, AVFXBase avfx, Plugin plugin) : base(avfx, "##MDL")
        {
            Main = main;
            Mdl3D = plugin.DXManager.Model;
            Group = UINode._Models;
            Group.Items = AVFX.Models.Select( item => new UIModel( main, item, this ) ).ToList();
        }

        public override void OnSelect( UIModel item ) {
            Mdl3D.LoadModel( item.Model );
        }

        public override void OnDelete( UIModel item ) {
            AVFX.removeModel( item.Model );
        }

        public override UIModel OnNew() {
            return new UIModel( Main, AVFX.addModel(), this );
        }

        public override UIModel OnImport( AVFXNode node ) {
            AVFXModel mdl = new AVFXModel();
            mdl.read( node );
            AVFX.addModel( mdl );
            return new UIModel( Main, mdl, this );
        }
    }
}
