using AVFXLib.AVFX;
using AVFXLib.Models;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumina.Excel.GeneratedSheets;
using System.IO;

namespace VFXEditor
{

    public class TextureManager
    {
        public Plugin _plugin;
        public Dictionary<string, ImGuiScene.TextureWrap> PathToTex = new Dictionary<string, ImGuiScene.TextureWrap>();

        public TextureManager(Plugin plugin )
        {
            _plugin = plugin;
        }

        public void LoadTexture(string path )
        {
            var _path = path.Trim( '\u0000' );
            if( PathToTex.ContainsKey( path ) )
                return;

            Task.Run( () => {
                var tex = CreateTexture( _path );
                if(tex != null )
                {
                    PathToTex.Add( path, tex );
                }
            } );
        }

        public ImGuiScene.TextureWrap CreateTexture(string path )
        {
            var result = _plugin.PluginInterface.Data.FileExists( path );
            if( result )
            {
                try
                {
                    var texFile = _plugin.PluginInterface.Data.GetFile<VFXTexture>( path );
                    var texBind = _plugin.PluginInterface.UiBuilder.LoadImageRaw( Cleanup( texFile.ImageData), texFile.Header.Width, texFile.Header.Height, 4 );
                    return texBind;
                }
                catch( Exception e )
                {
                    PluginLog.LogError( e.ToString() );
                    PluginLog.LogError( "Could not find tex:" + path );
                    return null;
                }
            }
            else
            {
                PluginLog.LogError( "Could not find tex:" + path );
                return null;
            }
        }

        public byte[] Cleanup(byte[] data ) // gbr -> rgb
        {
            byte[] ret = new byte[data.Length];
            for(int i = 0; i < data.Length / 4; i++ )
            {
                var idx = i * 4;
                ret[idx + 0] = data[idx + 2];
                ret[idx + 1] = data[idx + 1];
                ret[idx + 2] = data[idx + 0];
                ret[idx + 3] = data[idx + 3];
            }
            return ret;
        }
    }
}
