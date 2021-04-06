using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using AVFXLib.Models;
using System.Runtime.InteropServices;

namespace VFXEditor.Data.Texture {
    public class GradientManager {
        public static GradientManager _Manager;

        public TextureManager Manager;
        public Plugin _plugin;

        public static readonly int Width = 150;
        public static readonly int Height = 50;

        public struct GradData {
            public ImGuiScene.TextureWrap Wrap;
        }

        Dictionary<AVFXCurve, GradData> CurveToGradient = new Dictionary<AVFXCurve, GradData>();

        public GradientManager(Plugin plugin, TextureManager manager ) {
            _Manager = this;
            Manager = manager;
            _plugin = plugin;
        }

        public GradData UpdateGradient( AVFXCurve curve ) {
            if( CurveToGradient.ContainsKey( curve ) ) {
                CurveToGradient[curve].Wrap.Dispose();
            }
            return AddGradient( curve );
        }

        public GradData AddGradient( AVFXCurve curve ) {
            LinearGradientBrush _brush = new LinearGradientBrush( new Rectangle( 0, 0, Width, Height ), Color.Black, Color.Black, LinearGradientMode.Horizontal );
            ColorBlend cb = new ColorBlend();

            float[] Positions = new float[curve.Keys.Count];
            Color[] Colors = new Color[curve.Keys.Count];
            float startTime = curve.Keys[0].Time;
            float endTime = curve.Keys[curve.Keys.Count - 1].Time;
            for(int i = 0; i < curve.Keys.Count; i++ ) {
                var item = curve.Keys[i];
                Positions[i] = (float) ( item.Time - startTime ) / ( endTime - startTime );
                Colors[i] = Color.FromArgb( 255, (int) (item.Z * 255), (int) (item.Y * 255), (int) (item.X * 255) );
            }
            cb.Positions = Positions;
            cb.Colors = Colors;

            _brush.InterpolationColors = cb;

            using( Bitmap bitmap = new Bitmap( Width, Height ) )
            using( Graphics graphics = Graphics.FromImage( bitmap ) )
            {
                graphics.FillRectangle( _brush, new Rectangle( 0, 0, Width, Height ) );
                _brush.Dispose();

                var bitmapData = bitmap.LockBits( new Rectangle( 0, 0, Width, Height ) , ImageLockMode.ReadWrite, bitmap.PixelFormat);
                var length = bitmapData.Stride * bitmapData.Height;
                byte[] bytes = new byte[length];
                Marshal.Copy( bitmapData.Scan0, bytes, 0, length );
                bitmap.UnlockBits( bitmapData );

                var wrap = _plugin.PluginInterface.UiBuilder.LoadImageRaw( bytes, Width, Height, 4 );
                var res = new GradData {
                    Wrap = wrap
                };
                CurveToGradient[curve] = res;
                return res;
            }
        }

        public void Dispose() {
            foreach(var item in CurveToGradient.Values ) {
                item.Wrap.Dispose();
            }
        }
    }
}
