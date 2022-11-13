using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxEffectorDataCameraQuake : AvfxData {
        public readonly AvfxCurve Attenuation = new( "Attenuation", "Att" );
        public readonly AvfxCurve RadiusOut = new( "Radius Out", "RdO" );
        public readonly AvfxCurve RadiusIn = new( "Radius In", "RdI" );
        public readonly AvfxCurve3Axis Rotation = new( "Rotation", "Rot" );
        public readonly AvfxCurve3Axis Position = new( "Position", "Pos" );

        public AvfxEffectorDataCameraQuake() : base() {
            Children = new() {
                Attenuation,
                RadiusOut,
                RadiusIn,
                Rotation,
                Position
            };

            Tabs.Add( Attenuation );
            Tabs.Add( RadiusOut );
            Tabs.Add( RadiusIn );
            Tabs.Add( Rotation );
            Tabs.Add( Position );
        }
    }
}
