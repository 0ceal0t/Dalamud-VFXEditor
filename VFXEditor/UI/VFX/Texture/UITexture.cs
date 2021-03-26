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
    public class UITexture : UINode
    {
        public TextureManager Manager;
        public UITextureView View;
        public AVFXTexture Texture;
        public string lastValue;
        public UIString Path;
        public UINodeGraphView NodeView;

        public UITexture(AVFXTexture texture, UITextureView view)
        {
            View = view;
            Texture = texture;
            _Color = TextureColor;
            NodeView = new UINodeGraphView( this );
            Manager = view.Manager;
            // ================
            UIString.Change bytesToPath = BytesToPath;
            Path = new UIString( "Path", Texture.Path, changeFunction: bytesToPath );
            lastValue = Texture.Path.Value;
            if( view.GetPreviewTexture() ) {
                Manager.LoadTexture( Texture.Path.Value );
            }
        }
        public override void DrawBody( string parentId )
        {
            string id = parentId + "/Texture";
            NodeView.Draw( id );
            Path.Draw( id );

            var newValue = Path.Literal.Value;
            if( newValue != lastValue )
            {
                lastValue = newValue;
                if( View.GetPreviewTexture() )
                {
                    Manager.LoadTexture( newValue );
                }
            }
            if( View.GetPreviewTexture() )
            {
                if( Manager.PathToTex.ContainsKey( newValue ) )
                {
                    var t = Manager.PathToTex[newValue];
                    ImGui.Image( t.Wrap.ImGuiHandle, new Vector2( t.Width, t.Height ) );
                    if(ImGui.Button("Export" + id ) )
                    {
                        SaveDialog( t );
                    }
                }
            }
        }

        public static void SaveDialog(TexData t)
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
                                bmp.SetPixel( j, i, Color.FromArgb( a, r, g, b ) );
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

        public override string GetText() {
            return "Texture " + Idx;
        }

        public override byte[] toBytes() {
            return Texture.toAVFX().toBytes();
        }
    }
}
