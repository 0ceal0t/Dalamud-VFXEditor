using AVFXLib.Models;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.Vfx {
    public class UIParameterView : UIBase {
        public AVFXBase AVFX;
        public List<UIBase> Attributes = new();
        // ======================
        public int[] Version = new int[4];
        public UIFloat3 RevisedScale;
        public float ScaleCombined = 1.0f;

        public UIParameterView( AVFXBase avfx ) {
            AVFX = avfx;
            // =====================
            var versionBytes = AVFXLib.Main.Util.IntTo4Bytes( AVFX.Version.Value );
            for( var i = 0; i < versionBytes.Length; i++ ) {
                Version[i] = versionBytes[i];
            }
            RevisedScale = new UIFloat3( "Revised Scale", AVFX.RevisedValuesScaleX, AVFX.RevisedValuesScaleY, AVFX.RevisedValuesScaleZ );
            ScaleCombined = Math.Max( AVFX.RevisedValuesScaleX.Value, Math.Max( AVFX.RevisedValuesScaleY.Value, AVFX.RevisedValuesScaleZ.Value ) );

            Attributes.Add( new UIFloat3( "Revised Position", AVFX.RevisedValuesPosX, AVFX.RevisedValuesPosY, AVFX.RevisedValuesPosZ ) );
            Attributes.Add( new UIFloat3( "Revised Rotation", AVFX.RevisedValuesRotX, AVFX.RevisedValuesRotY, AVFX.RevisedValuesRotZ ) );
            Attributes.Add( new UIFloat3( "Revised Color", AVFX.RevisedValuesR, AVFX.RevisedValuesG, AVFX.RevisedValuesB ) );
            Attributes.Add( new UICheckbox( "Delay Fast Particle", AVFX.IsDelayFastParticle ) );
            Attributes.Add( new UICheckbox( "Fit Ground", AVFX.IsFitGround ) );
            Attributes.Add( new UICheckbox( "Transform Skip", AVFX.IsTranformSkip ) );
            Attributes.Add( new UICheckbox( "All Stop on Hide", AVFX.IsAllStopOnHide ) );
            Attributes.Add( new UICheckbox( "Can be Clipped Out", AVFX.CanBeClippedOut ) );
            Attributes.Add( new UICheckbox( "Clip Box Enabled", AVFX.ClipBoxenabled ) );
            Attributes.Add( new UIFloat3( "ClipBox Position", AVFX.ClipBoxX, AVFX.ClipBoxY, AVFX.ClipBoxZ ) );
            Attributes.Add( new UIFloat3( "ClipBox Size", AVFX.ClipBoxsizeX, AVFX.ClipBoxsizeY, AVFX.ClipBoxsizeZ ) );
            Attributes.Add( new UIFloat( "Bias Z Max Scale", AVFX.BiasZmaxScale ) );
            Attributes.Add( new UIFloat( "Bias Z Max Distance", AVFX.BiasZmaxDistance ) );
            Attributes.Add( new UICheckbox( "Camera Space", AVFX.IsCameraSpace ) );
            Attributes.Add( new UICheckbox( "Full Env Light", AVFX.IsFullEnvLight ) );
            Attributes.Add( new UICheckbox( "Clip Own Setting", AVFX.IsClipOwnSetting ) );
            Attributes.Add( new UIFloat( "Soft Particle Fade Range", AVFX.SoftParticleFadeRange ) );
            Attributes.Add( new UIFloat( "Sort Key Offset", AVFX.SoftKeyOffset ) );
            Attributes.Add( new UICombo<DrawLayer>( "Draw Layer", AVFX.DrawLayerType ) );
            Attributes.Add( new UICombo<DrawOrder>( "Draw Order", AVFX.DrawOrderType ) );
            Attributes.Add( new UICombo<DirectionalLightSource>( "Directional Light Source", AVFX.DirectionalLightSourceType ) );
            Attributes.Add( new UICombo<PointLightSouce>( "Point Light 1", AVFX.PointLightsType1 ) );
            Attributes.Add( new UICombo<PointLightSouce>( "Point Light 2", AVFX.PointLightsType2 ) );
            Attributes.Add( new UICheckbox( "Fade X", AVFX.FadeXenabled ) );
            Attributes.Add( new UICheckbox( "Fade Y", AVFX.FadeYenabled ) );
            Attributes.Add( new UICheckbox( "Fade Z", AVFX.FadeZenabled ) );
            Attributes.Add( new UIFloat3( "Fade Inner", AVFX.FadeXinner, AVFX.FadeYinner, AVFX.FadeZinner ) );
            Attributes.Add( new UIFloat3( "Fade Outer", AVFX.FadeXouter, AVFX.FadeYouter, AVFX.FadeZouter ) );
            Attributes.Add( new UICheckbox( "Global Fog", AVFX.GlobalFogEnabled ) );
            Attributes.Add( new UIFloat( "Global Fog Influence", AVFX.GlobalFogInfluence ) );
            Attributes.Add( new UICheckbox( "LTS Enabled", AVFX.LTSEnabled ) );
        }

        public override void Draw( string parentId = "" ) {
            var id = "##AVFX";
            ImGui.BeginChild( id + "/Child" );
            ImGui.Text( "VFX Version: " + Version[0] + "." + Version[1] + "." + Version[2] + "." + Version[3] );
            if( ImGui.InputFloat( "Revised Scale (Combined)", ref ScaleCombined ) ) {
                RevisedScale.Literal1.GiveValue( ScaleCombined );
                RevisedScale.Literal2.GiveValue( ScaleCombined );
                RevisedScale.Literal3.GiveValue( ScaleCombined );
                RevisedScale.Value = new System.Numerics.Vector3( ScaleCombined, ScaleCombined, ScaleCombined );
            }
            RevisedScale.Draw( id );
            ImGui.Separator();

            DrawList( Attributes, id );
            ImGui.EndChild();
        }
    }
}
