using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Dalamud.Plugin;

namespace VFXEditor.UI.VFX
{
    public class UITexture : UIBase
    {
        public AVFXTexture Texture;
        public UITextureView View;
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
            if( _plugin.Configuration.PreviewTextures )
            {
                _plugin.Manager.TexManager.LoadTexture( Texture.Path.Value );
            }
        }

        public override void Draw( string parentId )
        {
        }
        public override void DrawSelect( string parentId, ref UIBase selected )
        {
            if( !Assigned )
            {
                return;
            }
            if( ImGui.Selectable( "Texture " + Idx + parentId, selected == this ) )
            {
                selected = this;
            }
        }
        public override void DrawBody( string parentId )
        {
            string id = parentId + "/Texture" + Idx;
            if( UIUtils.RemoveButton( "Delete" + id ) )
            {
                PluginLog.Log( View.AVFX.Textures.Count().ToString() );
                PluginLog.Log( Idx.ToString() );
                View.AVFX.removeTexture( Idx );
                View.Init();
                return;
            }
            Path.Draw( id );

            // jank change detection
            var newValue = Path.Literal.Value;
            if( newValue != lastValue )
            {
                lastValue = newValue;
                if( _plugin.Configuration.PreviewTextures )
                {
                    _plugin.Manager.TexManager.LoadTexture( newValue );
                }
            }
            if( _plugin.Configuration.PreviewTextures )
            {
                if( _plugin.Manager.TexManager.PathToTex.ContainsKey( newValue ) )
                {
                    var a = _plugin.Manager.TexManager.PathToTex[newValue];
                    ImGui.Image( a.ImGuiHandle, new Vector2( a.Width, a.Height ) );
                }
            }
        }

        public static void BytesToPath(LiteralString literal)
        {
            literal.GiveValue(literal.Value + "\u0000");
        }
    }
}
