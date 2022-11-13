using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using static VfxEditor.AvfxFormat2.Enums;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxMain : AvfxDrawable {
        public static AvfxMain FromStream( BinaryReader reader ) {
            var main = new AvfxMain();
            reader.ReadInt32();
            var size = reader.ReadInt32();
            main.Read( reader, size );
            return main;
        }

        // ============

        public readonly AvfxInt Version = new( "Version", "Ver" );
        public readonly AvfxBool IsDelayFastParticle = new( "Delay Fast Particle", "bDFP" );
        public readonly AvfxBool IsFitGround = new( "Fit Ground", "bFG" );
        public readonly AvfxBool IsTranformSkip = new( "Transform Skip", "bTS" );
        public readonly AvfxBool IsAllStopOnHide = new( "All Stop on Hide", "bASH" );
        public readonly AvfxBool CanBeClippedOut = new( "Can be Clipped Out", "bCBC" );
        public readonly AvfxBool ClipBoxenabled = new( "Clip Box Enabled", "bCul" );
        public readonly AvfxFloat ClipBoxX = new( "Clip Box X", "CBPx" );
        public readonly AvfxFloat ClipBoxY = new( "Clip Box Y", "CBPy" );
        public readonly AvfxFloat ClipBoxZ = new( "Clip Box Z", "CBPz" );
        public readonly AvfxFloat ClipBoxSizeX = new( "Clip Box Size X", "CBSx" );
        public readonly AvfxFloat ClipBoxSizeY = new( "Clip Box Size Y", "CBSy" );
        public readonly AvfxFloat ClipBoxSizeZ = new( "Clip Box Size Z", "CBSz" );
        public readonly AvfxFloat BiasZmaxScale = new( "Bias Z Max Scale", "ZBMs" );
        public readonly AvfxFloat BiasZmaxDistance = new( "Bias Z Max Distance", "ZBMd" );
        public readonly AvfxBool IsCameraSpace = new( "Camera Space", "bCmS" );
        public readonly AvfxBool IsFullEnvLight = new( "Full Env Light", "bFEL" );
        public readonly AvfxBool IsClipOwnSetting = new( "Clip Own Setting", "bOSt" );
        public readonly AvfxFloat NearClipBegin = new( "Near Clip Begin", "NCB" );
        public readonly AvfxFloat NearClipEnd = new( "Near Clip End", "NCE" );
        public readonly AvfxFloat FarClipBegin = new( "Far Clip Begin", "FCB" );
        public readonly AvfxFloat FarClipEnd = new( "Far Clip End", "FCE" );
        public readonly AvfxFloat SoftParticleFadeRange = new( "Soft Particle Fade Range", "SPFR" );
        public readonly AvfxFloat SoftKeyOffset = new( "Sort Key Offset", "SKO" );
        public readonly AvfxEnum<DrawLayer> DrawLayerType = new( "Draw Layer", "DwLy" );
        public readonly AvfxEnum<DrawOrder> DrawOrderType = new( "Draw Order", "DwOT" );
        public readonly AvfxEnum<DirectionalLightSource> DirectionalLightSourceType = new( "Directional Light Source", "DLST" );
        public readonly AvfxEnum<PointLightSouce> PointLightsType1 = new( "Point Light 1", "PL1S" );
        public readonly AvfxEnum<PointLightSouce> PointLightsType2 = new( "Point Light 2", "PL2S" );
        public readonly AvfxFloat RevisedValuesPosX = new( "Revised Pos X", "RvPx" );
        public readonly AvfxFloat RevisedValuesPosY = new( "Revised Pos Y", "RvPy" );
        public readonly AvfxFloat RevisedValuesPosZ = new( "Revised Pos Z", "RvPz" );
        public readonly AvfxFloat RevisedValuesRotX = new( "Revised Rot X", "RvRx" );
        public readonly AvfxFloat RevisedValuesRotY = new( "Revised Rot Y", "RvRy" );
        public readonly AvfxFloat RevisedValuesRotZ = new( "Revised Rot Z", "RvRz" );
        public readonly AvfxFloat RevisedValuesScaleX = new( "Revised Scale X", "RvSx" );
        public readonly AvfxFloat RevisedValuesScaleY = new( "Revised Scale Y", "RvSy" );
        public readonly AvfxFloat RevisedValuesScaleZ = new( "Revised Scale Z", "RvSz" );
        public readonly AvfxFloat RevisedValuesR = new( "Revised R", "RvR" );
        public readonly AvfxFloat RevisedValuesG = new( "Revised G", "RvG" );
        public readonly AvfxFloat RevisedValuesB = new( "Revised B", "RvB" );
        public readonly AvfxBool FadeEnabledX = new( "Fade Enabled X", "AFXe" );
        public readonly AvfxFloat FadeInnerX = new( "Fade Inner X", "AFXi" );
        public readonly AvfxFloat FadeOuterX = new( "Fade Outer X", "AFXo" );
        public readonly AvfxBool FadeEnabledY = new( "Fade Enabled Y", "AFYe" );
        public readonly AvfxFloat FadeInnerY = new( "Fade Inner Y", "AFYi" );
        public readonly AvfxFloat FadeOuterY = new( "Fade Outer Y", "AFYo" );
        public readonly AvfxBool FadeEnabledZ = new( "Fade Enabled Z", "AFZe" );
        public readonly AvfxFloat FadeInnerZ = new( "Fade Inner Z", "AFZi" );
        public readonly AvfxFloat FadeOuterZ = new( "Fade Outer Z", "AFZo" );
        public readonly AvfxBool GlobalFogEnabled = new( "Global Fog", "bGFE" );
        public readonly AvfxFloat GlobalFogInfluence = new( "Global Fog Influence", "GFIM" );
        public readonly AvfxBool LTSEnabled = new( "LTS Enabled", "bLTS" );

        public readonly UiNodeGroupSet NodeGroupSet;

        private readonly List<AvfxBase> Parsed;

        public readonly List<AvfxScheduler> Schedulers = new();
        public readonly List<AvfxTimeline> Timelines = new();
        public readonly List<AvfxEmitter> Emitters = new();
        public readonly List<AvfxParticle> Particles = new();
        public readonly List<AvfxEffector> Effectors = new();
        public readonly List<AvfxBinder> Binders = new();
        public readonly List<AvfxTexture> Textures = new();
        public readonly List<AvfxModel> Models = new();

        private readonly List<IUiBase> Display;
        private readonly int[] UiVersion = new int[4];
        private float ScaleCombined = 1.0f;

        public AvfxMain() : base( "AVFX" ) {
            Parsed = new List<AvfxBase> {
                Version,
                IsDelayFastParticle,
                IsFitGround,
                IsTranformSkip,
                IsAllStopOnHide,
                CanBeClippedOut,
                ClipBoxenabled,
                ClipBoxX,
                ClipBoxY,
                ClipBoxZ,
                ClipBoxSizeX,
                ClipBoxSizeY,
                ClipBoxSizeZ,
                BiasZmaxScale,
                BiasZmaxDistance,
                IsCameraSpace,
                IsFullEnvLight,
                IsClipOwnSetting,
                NearClipBegin,
                NearClipEnd,
                FarClipBegin,
                FarClipEnd,
                SoftParticleFadeRange,
                SoftKeyOffset,
                DrawLayerType,
                DrawOrderType,
                DirectionalLightSourceType,
                PointLightsType1,
                PointLightsType2,
                RevisedValuesPosX,
                RevisedValuesPosY,
                RevisedValuesPosZ,
                RevisedValuesRotX,
                RevisedValuesRotY,
                RevisedValuesRotZ,
                RevisedValuesScaleX,
                RevisedValuesScaleY,
                RevisedValuesScaleZ,
                RevisedValuesR,
                RevisedValuesG,
                RevisedValuesB,
                FadeEnabledX,
                FadeInnerX,
                FadeOuterX,
                FadeEnabledY,
                FadeInnerY,
                FadeOuterY,
                FadeEnabledZ,
                FadeInnerZ,
                FadeOuterZ,
                GlobalFogEnabled,
                GlobalFogInfluence,
                LTSEnabled
            };
            Version.SetValue( 0x20110913 );

            NodeGroupSet = new( this );

            Display = new() {
                new UiFloat3( "Revised Scale", RevisedValuesScaleX, RevisedValuesScaleY, RevisedValuesScaleZ ),
                new UiFloat3( "Revised Position", RevisedValuesPosX, RevisedValuesPosY, RevisedValuesPosZ ),
                new UiFloat3( "Revised Rotation", RevisedValuesRotX, RevisedValuesRotY, RevisedValuesRotZ ),
                new UiFloat3( "Revised Color", RevisedValuesR, RevisedValuesG, RevisedValuesB ),
                IsDelayFastParticle,
                IsFitGround,
                IsTranformSkip,
                IsAllStopOnHide,
                CanBeClippedOut,
                ClipBoxenabled,
                new UiFloat3( "Clip Box Position", ClipBoxX, ClipBoxY, ClipBoxZ ),
                new UiFloat3( "Clip Box Size", ClipBoxSizeX, ClipBoxSizeY, ClipBoxSizeZ ),
                BiasZmaxScale,
                BiasZmaxDistance,
                IsCameraSpace,
                IsFullEnvLight,
                IsClipOwnSetting,
                NearClipBegin,
                NearClipEnd,
                FarClipBegin,
                FarClipEnd,
                SoftParticleFadeRange,
                SoftKeyOffset,
                DrawLayerType,
                DrawOrderType,
                DirectionalLightSourceType,
                PointLightsType1,
                PointLightsType2,
                FadeEnabledX,
                FadeEnabledY,
                FadeEnabledZ,
                new UiFloat3( "Fade Inner", FadeInnerX, FadeInnerY, FadeInnerZ ),
                new UiFloat3( "Fade Outer", FadeOuterX, FadeOuterY, FadeOuterZ ),
                GlobalFogEnabled,
                GlobalFogEnabled,
                LTSEnabled
            };
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            Peek( reader, Parsed, size ); // read but then reset the position

            ReadNested( reader, ( BinaryReader _reader, string _name, int _size ) => {
                switch( _name ) {
                    case AvfxScheduler.NAME:
                        var Scheduler = new AvfxScheduler( NodeGroupSet );
                        Scheduler.Read( _reader, _size );
                        Schedulers.Add( Scheduler );
                        break;
                    case AvfxTimeline.NAME:
                        var Timeline = new AvfxTimeline( NodeGroupSet, false );
                        Timeline.Read( _reader, _size );
                        Timelines.Add( Timeline );
                        break;
                    case AvfxEmitter.NAME:
                        var Emitter = new AvfxEmitter( NodeGroupSet, false );
                        Emitter.Read( _reader, _size );
                        Emitters.Add( Emitter );
                        break;
                    case AvfxParticle.NAME:
                        var Particle = new AvfxParticle( NodeGroupSet, false );
                        Particle.Read( _reader, _size );
                        Particles.Add( Particle );
                        break;
                    case AvfxEffector.NAME:
                        var Effector = new AvfxEffector( false );
                        Effector.Read( _reader, _size );
                        Effectors.Add( Effector );
                        break;
                    case AvfxBinder.NAME:
                        var Binder = new AvfxBinder( false );
                        Binder.Read( _reader, _size );
                        Binders.Add( Binder );
                        break;
                    case AvfxTexture.NAME:
                        var Texture = new AvfxTexture();
                        Texture.Read( _reader, _size );
                        Textures.Add( Texture );
                        break;
                    case AvfxModel.NAME:
                        var Model = new AvfxModel();
                        Model.Read( _reader, _size );
                        Models.Add( Model );
                        break;
                }
            }, size );

            var versionBytes = BitConverter.GetBytes( Version.GetValue() );
            for( var i = 0; i < versionBytes.Length; i++ ) UiVersion[i] = versionBytes[i];
            ScaleCombined = Math.Max( RevisedValuesScaleX.GetValue(), Math.Max( RevisedValuesScaleY.GetValue(), RevisedValuesScaleZ.GetValue() ) );
        }

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Parsed, assigned );

        protected override void WriteContents( BinaryWriter writer ) {
            WriteNested( writer, Parsed );

            WriteLeaf( writer, "ScCn", 4, Schedulers.Count );
            WriteLeaf( writer, "TlCn", 4, Timelines.Count );
            WriteLeaf( writer, "EmCn", 4, Emitters.Count );
            WriteLeaf( writer, "PrCn", 4, Particles.Count );
            WriteLeaf( writer, "EfCn", 4, Effectors.Count );
            WriteLeaf( writer, "BdCn", 4, Binders.Count );
            WriteLeaf( writer, "TxCn", 4, Textures.Count );
            WriteLeaf( writer, "MdCn", 4, Models.Count );

            foreach( var item in Schedulers ) item.Write( writer );
            foreach( var item in Timelines ) item.Write( writer );
            foreach( var item in Emitters ) item.Write( writer );
            foreach( var item in Particles ) item.Write( writer );
            foreach( var item in Effectors ) item.Write( writer );
            foreach( var item in Binders ) item.Write( writer );
            foreach( var item in Textures ) item.Write( writer );
            foreach( var item in Models ) item.Write( writer );
        }

        public override void Draw( string parentId = "" ) {
            var id = "##AVFX";
            ImGui.BeginChild( id + "/Child" );

            ImGui.PushStyleColor( ImGuiCol.Text, ImGui.GetColorU32( ImGuiCol.TextDisabled ) );
            ImGui.TextWrapped( "Revised scale, position, and rotation only work on effects which are not attached to a binder. See the \"Binders\" tab for more information." );
            ImGui.PopStyleColor();

            if( ImGui.InputFloat( "Revised Scale (Combined)", ref ScaleCombined ) ) {
                CommandManager.Avfx.Add( new UiFloat3Command( RevisedValuesScaleX, RevisedValuesScaleY, RevisedValuesScaleZ, new Vector3( ScaleCombined ) ) );
            }

            IUiBase.DrawList( Display, id );
            ImGui.Text( $"VFX Version: {UiVersion[0]}.{UiVersion[1]}.{UiVersion[2]}.{UiVersion[3]}" );
            ImGui.EndChild();
        }
    }
}
