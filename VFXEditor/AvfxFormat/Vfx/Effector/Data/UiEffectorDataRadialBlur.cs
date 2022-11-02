using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Effector;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiEffectorDataRadialBlur : UiData {
        public UiParameters Parameters;

        public UiEffectorDataRadialBlur( AVFXEffectorDataRadialBlur data ) {
            Tabs.Add( Parameters = new UiParameters( "Parameters" ) );
            Parameters.Add( new UiFloat( "Fade Start Distance", data.FadeStartDistance ) );
            Parameters.Add( new UiFloat( "Fade End Distance", data.FadeEndDistance ) );
            Parameters.Add( new UiCombo<ClipBasePoint>( "Fade Base Point", data.FadeBasePointType ) );

            Tabs.Add( new UiCurve( data.Length, "Length" ) );
            Tabs.Add( new UiCurve( data.Strength, "Strength" ) );
            Tabs.Add( new UiCurve( data.Gradation, "Gradation" ) );
            Tabs.Add( new UiCurve( data.InnerRadius, "Inner Radius" ) );
            Tabs.Add( new UiCurve( data.OuterRadius, "Outer Radius" ) );
        }
    }
}
