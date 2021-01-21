using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIModelView : UINodeSplitView<UIModel>
    {
        public Model3D Mdl3D;

        public UIModelView(AVFXBase avfx, Plugin plugin) : base(avfx, "##MDL")
        {
            Mdl3D = plugin.DXManager.Model;
            UINode._Models = new UINodeGroup<UIModel>();
            Group = UINode._Models;
            Group.Items = AVFX.Models.Select( item => new UIModel( item, this ) ).ToList();
        }

        public override void OnSelect( UIModel item ) {
            Mdl3D.LoadModel( item.Model );
        }

        public override void OnDelete( UIModel item ) {
            AVFX.removeModel( item.Model );
        }

        public override UIModel OnNew() {
            return new UIModel( AVFX.addModel(), this );
        }
    }
}
