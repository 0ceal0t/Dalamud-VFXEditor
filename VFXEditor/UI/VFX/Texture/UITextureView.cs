using AVFXLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace VFXEditor.UI.VFX
{
    public class UITextureView : UIBase
    {
        public AVFXBase AVFX;
        public List<UITexture> Textures;
        public UITextureSplitView TexSplit;
        public Plugin _plugin;

        public UITextureView(AVFXBase avfx, Plugin plugin)
        {
            AVFX = avfx;
            _plugin = plugin;
            Init();
        }
        public override void Init()
        {
            base.Init();
            Textures = new List<UITexture>();
            foreach (var texture in AVFX.Textures)
            {
                Textures.Add(new UITexture(texture, this, _plugin));
            }
            TexSplit = new UITextureSplitView( Textures, this );
        }

        public override void Draw(string parentId = "")
        {
            string id = "##TEX";
            TexSplit.Draw( id );
        }
    }
}
