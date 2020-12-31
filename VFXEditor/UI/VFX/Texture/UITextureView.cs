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
        List<UITexture> Textures;

        public UITextureView(AVFXBase avfx)
        {
            AVFX = avfx;
            Init();
        }
        public override void Init()
        {
            base.Init();
            Textures = new List<UITexture>();
            foreach (var texture in AVFX.Textures)
            {
                Textures.Add(new UITexture(texture, this));
            }
        }

        public override void Draw(string parentId = "")
        {
            string id = "##TEX";
            int tIdx = 0;
            foreach (var texture in Textures)
            {
                texture.Idx = tIdx;
                texture.Draw(id);
                tIdx++;
            }
            if (ImGui.Button("+ Texture" + id))
            {
                AVFX.addTexture();
                Init();
            }
        }
    }
}
