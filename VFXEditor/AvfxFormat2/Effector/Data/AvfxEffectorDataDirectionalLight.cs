using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxEffectorDataDirectionalLight : AvfxData {
        public readonly AvfxCurveColor Ambient = new( "Ambient", "Amb" );
        public readonly AvfxCurveColor Color = new( "Color" );
        public readonly AvfxCurve Power = new( "Power", "Pow" );
        public readonly AvfxCurve3Axis Rotation = new( "Rotation", "Rot" );

        public AvfxEffectorDataDirectionalLight() : base() {
            Parsed = new() {
                Ambient,
                Color,
                Power,
                Rotation
            };

            DisplayTabs.Add( Ambient );
            DisplayTabs.Add( Color );
            DisplayTabs.Add( Power );
            DisplayTabs.Add( Rotation );
        }
    }
}
