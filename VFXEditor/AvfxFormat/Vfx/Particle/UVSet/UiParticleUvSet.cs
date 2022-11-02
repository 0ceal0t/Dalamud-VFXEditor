using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Particle;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiParticleUvSet : UiItem {
        public AVFXParticleUVSet UvSet;
        public UiParticle Particle;
        public UiCurve2Axis Scale;
        public UiCurve2Axis Scroll;
        public UiCurve Rotation;
        private readonly List<UiItem> Curves = new();
        private readonly List<IUiBase> Parameters;

        public UiParticleUvSet( AVFXParticleUVSet uvSet, UiParticle particle ) {
            UvSet = uvSet;
            Particle = particle;

            Parameters = new List<IUiBase> {
                new UiCombo<TextureCalculateUV>( "Calculate UV", UvSet.CalculateUVType )
            };

            Curves.Add( Scale = new UiCurve2Axis( UvSet.Scale, "Scale" ) );
            Curves.Add( Scroll = new UiCurve2Axis( UvSet.Scroll, "Scroll" ) );
            Curves.Add( Rotation = new UiCurve( UvSet.Rot, "Rotation" ) );
            Curves.Add( new UiCurve( UvSet.RotRandom, "Rotation Random" ) );
        }

        public override void DrawInline( string parentId ) {
            var id = parentId + "/UV";
            IUiBase.DrawList( Parameters, id );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            DrawListTabs( Curves, parentId );
        }

        public override string GetDefaultText() => $"UV {Idx}";
    }
}
