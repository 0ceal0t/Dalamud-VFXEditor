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
using System.Collections.Concurrent;

namespace VFXEditor
{
    public struct TexData
    {
        public ImGuiScene.TextureWrap Wrap;
        public byte[] Data;
        public int Height;
        public int Width;
        public TextureFormat Format;
    }

    public class TextureManager
    {
        public Plugin _plugin;
        public ConcurrentDictionary<string, TexData> PathToTex = new ConcurrentDictionary<string, TexData>();

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
                if(tex.Wrap != null )
                {
                    PathToTex.TryAdd( path, tex );
                }
            } );
        }

        public TexData CreateTexture(string path, bool loadImage = true )
        {
            var result = _plugin.PluginInterface.Data.FileExists( path );
            if( result )
            {
                try
                {
                    var ret = new TexData();

                    var texFile = _plugin.PluginInterface.Data.GetFile<VFXTexture>( path );
                    ret.Format = texFile.Header.Format;
                    var data = Cleanup( texFile.ImageData );
                    ret.Data = data;
                    if( loadImage ) {
                        var texBind = _plugin.PluginInterface.UiBuilder.LoadImageRaw( data, texFile.Header.Width, texFile.Header.Height, 4 );
                        ret.Wrap = texBind;
                    }
                    ret.Width = texFile.Header.Width;
                    ret.Height = texFile.Header.Height;
                    return ret;
                }
                catch( Exception e )
                {
                    PluginLog.LogError( e, "Could not find tex:" + path );
                    return new TexData();
                }
            }
            else
            {
                PluginLog.LogError( "Could not find tex:" + path );
                return new TexData();
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
