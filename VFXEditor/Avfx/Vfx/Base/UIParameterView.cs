using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;

namespace VFXEditor.Avfx.Vfx {
    public class UIParameterView : UIBase {
        private readonly List<UIBase> Parameters;
        private readonly int[] Version = new int[4];
        private readonly UIFloat3 RevisedScale;
        private float ScaleCombined = 1.0f;

        public UIParameterView( AVFXBase avfx ) {
            var versionBytes = AVFXLib.Main.Util.IntTo4Bytes( avfx.Version.Value );
            for( var i = 0; i < versionBytes.Length; i++ ) {
                Version[i] = versionBytes[i];
            }
            RevisedScale = new UIFloat3( "Revised Scale", avfx.RevisedValuesScaleX, avfx.RevisedValuesScaleY, avfx.RevisedValuesScaleZ );
            ScaleCombined = Math.Max( avfx.RevisedValuesScaleX.Value, Math.Max( avfx.RevisedValuesScaleY.Value, avfx.RevisedValuesScaleZ.Value ) );

            Parameters = new List<UIBase> {
                new UIFloat3( "Revised Position", avfx.RevisedValuesPosX, avfx.RevisedValuesPosY, avfx.RevisedValuesPosZ ),
                new UIFloat3( "Revised Rotation", avfx.RevisedValuesRotX, avfx.RevisedValuesRotY, avfx.RevisedValuesRotZ ),
                new UIFloat3( "Revised Color", avfx.RevisedValuesR, avfx.RevisedValuesG, avfx.RevisedValuesB ),
                new UICheckbox( "Delay Fast Particle", avfx.IsDelayFastParticle ),
                new UICheckbox( "Fit Ground", avfx.IsFitGround ),
                new UICheckbox( "Transform Skip", avfx.IsTranformSkip ),
                new UICheckbox( "All Stop on Hide", avfx.IsAllStopOnHide ),
                new UICheckbox( "Can be Clipped Out", avfx.CanBeClippedOut ),
                new UICheckbox( "Clip Box Enabled", avfx.ClipBoxenabled ),
                new UIFloat3( "ClipBox Position", avfx.ClipBoxX, avfx.ClipBoxY, avfx.ClipBoxZ ),
                new UIFloat3( "ClipBox Size", avfx.ClipBoxsizeX, avfx.ClipBoxsizeY, avfx.ClipBoxsizeZ ),
                new UIFloat( "Bias Z Max Scale", avfx.BiasZmaxScale ),
                new UIFloat( "Bias Z Max Distance", avfx.BiasZmaxDistance ),
                new UICheckbox( "Camera Space", avfx.IsCameraSpace ),
                new UICheckbox( "Full Env Light", avfx.IsFullEnvLight ),
                new UICheckbox( "Clip Own Setting", avfx.IsClipOwnSetting ),
                new UIFloat( "Soft Particle Fade Range", avfx.SoftParticleFadeRange ),
                new UIFloat( "Sort Key Offset", avfx.SoftKeyOffset ),
                new UICombo<DrawLayer>( "Draw Layer", avfx.DrawLayerType ),
                new UICombo<DrawOrder>( "Draw Order", avfx.DrawOrderType ),
                new UICombo<DirectionalLightSource>( "Directional Light Source", avfx.DirectionalLightSourceType ),
                new UICombo<PointLightSouce>( "Point Light 1", avfx.PointLightsType1 ),
                new UICombo<PointLightSouce>( "Point Light 2", avfx.PointLightsType2 ),
                new UICheckbox( "Fade X", avfx.FadeXenabled ),
                new UICheckbox( "Fade Y", avfx.FadeYenabled ),
                new UICheckbox( "Fade Z", avfx.FadeZenabled ),
                new UIFloat3( "Fade Inner", avfx.FadeXinner, avfx.FadeYinner, avfx.FadeZinner ),
                new UIFloat3( "Fade Outer", avfx.FadeXouter, avfx.FadeYouter, avfx.FadeZouter ),
                new UICheckbox( "Global Fog", avfx.GlobalFogEnabled ),
                new UIFloat( "Global Fog Influence", avfx.GlobalFogInfluence ),
                new UICheckbox( "LTS Enabled", avfx.LTSEnabled )
            };
        }

        public override void Draw( string parentId = "" ) {
            var id = "##AVFX";
            ImGui.BeginChild( id + "/Child" );
            ImGui.Text( $"VFX Version: {Version[0]}.{Version[1]}.{Version[2]}.{Version[3]}" );
            if( ImGui.InputFloat( "Revised Scale (Combined)", ref ScaleCombined ) ) {
                RevisedScale.Literal1.GiveValue( ScaleCombined );
                RevisedScale.Literal2.GiveValue( ScaleCombined );
                RevisedScale.Literal3.GiveValue( ScaleCombined );
                RevisedScale.Value = new System.Numerics.Vector3( ScaleCombined, ScaleCombined, ScaleCombined );
            }
            RevisedScale.Draw( id );
            ImGui.Separator();

            DrawList( Parameters, id );
            ImGui.EndChild();
        }
    }
}
