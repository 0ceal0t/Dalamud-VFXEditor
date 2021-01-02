using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace VFXEditor.UI.VFX
{
    public class UITexture : UIBase
    {
        public AVFXTexture Texture;
        public UITextureView View;
        public int Idx;
        public string lastValue;
        public UIString Path;

        public Plugin _plugin;
        // =======================

        public UITexture(AVFXTexture texture, UITextureView view, Plugin plugin)
        {
            Texture = texture;
            View = view;
            _plugin = plugin;
            Init();
        }
        public override void Init()
        {
            base.Init();
            // ================
            UIString.Change bytesToPath = BytesToPath;
            Path = new UIString("Path", Texture.Path, changeFunction: bytesToPath);
            lastValue = Texture.Path.Value;
            _plugin.Manager.TexManager.LoadTexture( Texture.Path.Value );
        }

        public override void Draw(string parentId)
        {
            string id = parentId + "/Texture" + Idx;
            if (ImGui.CollapsingHeader("Texture " + Idx + id))
            {
                if (UIUtils.RemoveButton("Delete" + id))
                {
                    View.AVFX.removeTexture(Idx);
                    View.Init();
                    return;
                }
                Path.Draw(id);

                // jank change detection
                var newValue = Path.Literal.Value;
                if(newValue != lastValue)
                {
                    lastValue = newValue;
                    _plugin.Manager.TexManager.LoadTexture( newValue );
                }

                if( _plugin.Manager.TexManager.PathToTex.ContainsKey( newValue ) )
                {
                    var a = _plugin.Manager.TexManager.PathToTex[newValue];
                    ImGui.Image(a.ImGuiHandle, new Vector2(a.Width, a.Height));
                }
            }
        }

        public static void BytesToPath(LiteralString literal)
        {
            literal.GiveValue(literal.Value + "\u0000");
        }
    }
}
