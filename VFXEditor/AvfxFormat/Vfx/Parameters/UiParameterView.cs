using ImGuiNET;
using System;
using System.Collections.Generic;
using VfxEditor.AVFXLib;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat.Vfx {
    public unsafe class UiParameterView : IUiBase {
        private readonly List<IUiBase> Parameters;
        private readonly int[] Version = new int[4];
        private readonly UiFloat3 RevisedScale;
        private float ScaleCombined = 1.0f;

        public UiParameterView( AVFXMain avfx ) {
            var versionBytes = BitConverter.GetBytes( avfx.Version.GetValue() );
            for( var i = 0; i < versionBytes.Length; i++ ) {
                Version[i] = versionBytes[i];
            }
            RevisedScale = new UiFloat3( "Revised Scale", avfx.RevisedValuesScaleX, avfx.RevisedValuesScaleY, avfx.RevisedValuesScaleZ );
            ScaleCombined = Math.Max( avfx.RevisedValuesScaleX.GetValue(), Math.Max( avfx.RevisedValuesScaleY.GetValue(), avfx.RevisedValuesScaleZ.GetValue() ) );

            Parameters = new List<IUiBase> {
                new UiFloat3( "Revised Position", avfx.RevisedValuesPosX, avfx.RevisedValuesPosY, avfx.RevisedValuesPosZ ),
                new UiFloat3( "Revised Rotation", avfx.RevisedValuesRotX, avfx.RevisedValuesRotY, avfx.RevisedValuesRotZ ),
                new UiFloat3( "Revised Color", avfx.RevisedValuesR, avfx.RevisedValuesG, avfx.RevisedValuesB ),
                new UiCheckbox( "Delay Fast Particle", avfx.IsDelayFastParticle ),
                new UiCheckbox( "Fit Ground", avfx.IsFitGround ),
                new UiCheckbox( "Transform Skip", avfx.IsTranformSkip ),
                new UiCheckbox( "All Stop on Hide", avfx.IsAllStopOnHide ),
                new UiCheckbox( "Can be Clipped Out", avfx.CanBeClippedOut ),
                new UiCheckbox( "Clip Box Enabled", avfx.ClipBoxenabled ),
                new UiFloat3( "ClipBox Position", avfx.ClipBoxX, avfx.ClipBoxY, avfx.ClipBoxZ ),
                new UiFloat3( "ClipBox Size", avfx.ClipBoxsizeX, avfx.ClipBoxsizeY, avfx.ClipBoxsizeZ ),
                new UiFloat( "Bias Z Max Scale", avfx.BiasZmaxScale ),
                new UiFloat( "Bias Z Max Distance", avfx.BiasZmaxDistance ),
                new UiCheckbox( "Camera Space", avfx.IsCameraSpace ),
                new UiCheckbox( "Full Env Light", avfx.IsFullEnvLight ),
                new UiCheckbox( "Clip Own Setting", avfx.IsClipOwnSetting ),
                new UiFloat( "Near Clip Begin", avfx.NearClipBegin ),
                new UiFloat( "Near Clip End", avfx.NearClipEnd ),
                new UiFloat( "Far Clip Begin", avfx.FarClipBegin ),
                new UiFloat( "Far Clip End", avfx.FarClipEnd ),
                new UiFloat( "Soft Particle Fade Range", avfx.SoftParticleFadeRange ),
                new UiFloat( "Sort Key Offset", avfx.SoftKeyOffset ),
                new UiCombo<DrawLayer>( "Draw Layer", avfx.DrawLayerType ),
                new UiCombo<DrawOrder>( "Draw Order", avfx.DrawOrderType ),
                new UiCombo<DirectionalLightSource>( "Directional Light Source", avfx.DirectionalLightSourceType ),
                new UiCombo<PointLightSouce>( "Point Light 1", avfx.PointLightsType1 ),
                new UiCombo<PointLightSouce>( "Point Light 2", avfx.PointLightsType2 ),
                new UiCheckbox( "Fade X", avfx.FadeXenabled ),
                new UiCheckbox( "Fade Y", avfx.FadeYenabled ),
                new UiCheckbox( "Fade Z", avfx.FadeZenabled ),
                new UiFloat3( "Fade Inner", avfx.FadeXinner, avfx.FadeYinner, avfx.FadeZinner ),
                new UiFloat3( "Fade Outer", avfx.FadeXouter, avfx.FadeYouter, avfx.FadeZouter ),
                new UiCheckbox( "Global Fog", avfx.GlobalFogEnabled ),
                new UiFloat( "Global Fog Influence", avfx.GlobalFogInfluence ),
                new UiCheckbox( "LTS Enabled", avfx.LTSEnabled )
            };
        }

        public void DrawInline( string parentId = "" ) {
            var id = "##AVFX";
            ImGui.BeginChild( id + "/Child" );

            ImGui.PushStyleColor( ImGuiCol.Text, ImGui.GetColorU32( ImGuiCol.TextDisabled ) );
            ImGui.TextWrapped( "Revised scale, position, and rotation only work on effects which are not attached to a binder. See the \"Binders\" tab for more information." );
            ImGui.PopStyleColor();

            if( ImGui.InputFloat( "Revised Scale (Combined)", ref ScaleCombined ) ) {
                RevisedScale.Literal1.SetValue( ScaleCombined );
                RevisedScale.Literal2.SetValue( ScaleCombined );
                RevisedScale.Literal3.SetValue( ScaleCombined );
                RevisedScale.Value = new System.Numerics.Vector3( ScaleCombined, ScaleCombined, ScaleCombined );
            }
            RevisedScale.DrawInline( id );

            IUiBase.DrawList( Parameters, id );
            ImGui.Text( $"VFX Version: {Version[0]}.{Version[1]}.{Version[2]}.{Version[3]}" );
            ImGui.EndChild();
        }
    }
}
