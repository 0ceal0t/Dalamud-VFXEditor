using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Dalamud.Plugin;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace VFXEditor.UI.VFX
{
    public class UITexture : UIItem
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
        public override void DrawSelect( int idx, string parentId, ref UIItem selected )
        {
            if( ImGui.Selectable( GetText(idx) + parentId, selected == this ) )
            {
                selected = this;
            }
        }
        public override void DrawBody( string parentId )
        {
            string id = parentId + "/Texture";
            if( UIUtils.RemoveButton( "Delete" + id, small:true ) )
            {
                View.AVFX.removeTexture( Texture );
                View.TexSplit.OnDelete( this );
                return;
            }
            Path.Draw( id );

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
                    var t = _plugin.Manager.TexManager.PathToTex[newValue];
                    ImGui.Image( t.Wrap.ImGuiHandle, new Vector2( t.Width, t.Height ) );
                    if(ImGui.Button("Export" + id ) )
                    {
                        SaveDialog( t );
                    }
                }
            }
        }

        public void SaveDialog(TexData t)
        {
            Task.Run( async () =>
            {
                var picker = new SaveFileDialog
                {
                    Filter = "PNG Image (*.png)|*.png*|All files (*.*)|*.*",
                    Title = "Select a Save Location.",
                    DefaultExt = "png",
                    AddExtension = true
                };
                var result = await picker.ShowDialogAsync();
                if( result == DialogResult.OK )
                {
                    try
                    {
                        Bitmap bmp = new Bitmap( t.Width, t.Height );
                        for( int i = 0; i < t.Height; i++ )
                        {
                            for( int j = 0; j < t.Width; j++ )
                            {
                                int _idx = ( i * t.Width + j ) * 4;
                                int r = t.Data[_idx];
                                int g = t.Data[_idx + 1];
                                int b = t.Data[_idx + 2];
                                int a = t.Data[_idx + 3];
                                bmp.SetPixel( j, i, Color.FromArgb( r, g, b, a ) );
                            }
                        }
                        bmp.Save( picker.FileName, System.Drawing.Imaging.ImageFormat.Png );
                    }
                    catch( Exception ex )
                    {
                        PluginLog.LogError( ex, "Could not select an image location" );
                    }
                }
            } );
        }

        public static void BytesToPath(LiteralString literal)
        {
            literal.GiveValue(literal.Value + "\u0000");
        }

        public override string GetText( int idx ) {
            return "Texture " + idx;
        }
    }
}
