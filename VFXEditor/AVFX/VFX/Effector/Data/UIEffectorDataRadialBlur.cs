using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Effector;

namespace VfxEditor.AVFX.VFX {
    public class UIEffectorDataRadialBlur : UIData {
        public UIParameters Parameters;

        public UIEffectorDataRadialBlur( AVFXEffectorDataRadialBlur data ) {
            Tabs.Add( Parameters = new UIParameters( "Parameters" ) );
            Parameters.Add( new UIFloat( "Fade Start Distance", data.FadeStartDistance ) );
            Parameters.Add( new UIFloat( "Fade End Distance", data.FadeEndDistance ) );
            Parameters.Add( new UICombo<ClipBasePoint>( "Fade Base Point", data.FadeBasePointType ) );

            Tabs.Add( new UICurve( data.Length, "Length" ) );
            Tabs.Add( new UICurve( data.Strength, "Strength" ) );
            Tabs.Add( new UICurve( data.Gradation, "Gradation" ) );
            Tabs.Add( new UICurve( data.InnerRadius, "Inner Radius" ) );
            Tabs.Add( new UICurve( data.OuterRadius, "Outer Radius" ) );
        }
    }
}
