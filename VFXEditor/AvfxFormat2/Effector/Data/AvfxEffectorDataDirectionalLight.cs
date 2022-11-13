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
            Children = new() {
                Ambient,
                Color,
                Power,
                Rotation
            };

            Tabs.Add( Ambient );
            Tabs.Add( Color );
            Tabs.Add( Power );
            Tabs.Add( Rotation );
        }
    }
}
