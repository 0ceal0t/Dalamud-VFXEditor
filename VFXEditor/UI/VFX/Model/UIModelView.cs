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
        public ModelPreview Mdl3D;
        public UIMain Main;

        public UIModelView(UIMain main, AVFXBase avfx, Plugin plugin) : base(avfx, "##MDL")
        {
            Main = main;
            Mdl3D = plugin.DXManager.ModelView;
            Group = UINodeGroup.Models;
            Group.Items = AVFX.Models.Select( item => new UIModel( item ) ).ToList();
        }

        public override void DrawNewButton( string parentId ) {
            if( ImGui.SmallButton( "+ New" + Id ) ) {
                Group.Add( OnNew() );
            }
            ImGui.SameLine();
            if( ImGui.SmallButton( "Import" + Id ) ) {
                Main.ImportDialog();
            }
        }

        public override void OnSelect( UIModel item ) {
            Mdl3D.LoadModel( item.Model );
        }

        public override void OnDelete( UIModel item ) {
            AVFX.RemoveModel( item.Model );
        }

        public override UIModel OnNew() {
            return new UIModel( AVFX.AddModel() );
        }

        public override UIModel OnImport( AVFXNode node ) {
            AVFXModel mdl = new AVFXModel();
            mdl.Read( node );
            AVFX.AddModel( mdl );
            return new UIModel( mdl );
        }
    }
}
