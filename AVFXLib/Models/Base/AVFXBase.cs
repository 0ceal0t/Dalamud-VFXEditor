using AVFXLib.AVFX;
using System;
using System.Collections.Generic;

namespace AVFXLib.Models {
    public class AVFXBase : Base {
        public LiteralInt Version = new( "Ver" );

        public LiteralBool IsDelayFastParticle = new( "bDFP" );
        public LiteralBool IsFitGround = new( "bFG" );
        public LiteralBool IsTranformSkip = new( "bTS" );
        public LiteralBool IsAllStopOnHide = new( "bASH" );
        public LiteralBool CanBeClippedOut = new( "bCBC" );
        public LiteralBool ClipBoxenabled = new( "bCul" );
        public LiteralFloat ClipBoxX = new( "CBPx" );
        public LiteralFloat ClipBoxY = new( "CBPy" );
        public LiteralFloat ClipBoxZ = new( "CBPz" );
        public LiteralFloat ClipBoxsizeX = new( "CBSx" );
        public LiteralFloat ClipBoxsizeY = new( "CBSy" );
        public LiteralFloat ClipBoxsizeZ = new( "CBSz" );
        public LiteralFloat BiasZmaxScale = new( "ZBMs" );
        public LiteralFloat BiasZmaxDistance = new( "ZBMd" );
        public LiteralBool IsCameraSpace = new( "bCmS" );
        public LiteralBool IsFullEnvLight = new( "bFEL" );
        public LiteralBool IsClipOwnSetting = new( "bOSt" );
        public LiteralFloat SoftParticleFadeRange = new( "SPFR" );
        public LiteralFloat SoftKeyOffset = new( "SKO" );
        public LiteralEnum<DrawLayer> DrawLayerType = new( "DwLy" );
        public LiteralEnum<DrawOrder> DrawOrderType = new( "DwOT" );
        public LiteralEnum<DirectionalLightSource> DirectionalLightSourceType = new( "DLST" );
        public LiteralEnum<PointLightSouce> PointLightsType1 = new( "PL1S" );
        public LiteralEnum<PointLightSouce> PointLightsType2 = new( "PL2S" );
        public LiteralFloat RevisedValuesPosX = new( "RvPx" );
        public LiteralFloat RevisedValuesPosY = new( "RvPy" );
        public LiteralFloat RevisedValuesPosZ = new( "RvPz" );
        public LiteralFloat RevisedValuesRotX = new( "RvRx" );
        public LiteralFloat RevisedValuesRotY = new( "RvRy" );
        public LiteralFloat RevisedValuesRotZ = new( "RvRz" );
        public LiteralFloat RevisedValuesScaleX = new( "RvSx" );
        public LiteralFloat RevisedValuesScaleY = new( "RvSy" );
        public LiteralFloat RevisedValuesScaleZ = new( "RvSz" );
        public LiteralFloat RevisedValuesR = new( "RvR" );
        public LiteralFloat RevisedValuesG = new( "RvG" );
        public LiteralFloat RevisedValuesB = new( "RvB" );
        public LiteralBool FadeXenabled = new( "AFXe" );
        public LiteralFloat FadeXinner = new( "AFXi" );
        public LiteralFloat FadeXouter = new( "AFXo" );
        public LiteralBool FadeYenabled = new( "AFYe" );
        public LiteralFloat FadeYinner = new( "AFYi" );
        public LiteralFloat FadeYouter = new( "AFYo" );
        public LiteralBool FadeZenabled = new( "AFZe" );
        public LiteralFloat FadeZinner = new( "AFZi" );
        public LiteralFloat FadeZouter = new( "AFZo" );
        public LiteralBool GlobalFogEnabled = new( "bGFE" );
        public LiteralFloat GlobalFogInfluence = new( "GFIM" );
        public LiteralBool LTSEnabled = new( "bLTS" );

        public List<AVFXSchedule> Schedulers = new();
        public List<AVFXTimeline> Timelines = new();
        public List<AVFXEmitter> Emitters = new();
        public List<AVFXParticle> Particles = new();
        public List<AVFXEffector> Effectors = new();
        public List<AVFXBinder> Binders = new();
        public List<AVFXTexture> Textures = new();
        public List<AVFXModel> Models = new();
        private readonly List<Base> Attributes;

        public AVFXBase() : base( "AVFX" ) {
            Attributes = new List<Base>( new Base[]{
                IsDelayFastParticle,
                IsFitGround,
                IsTranformSkip,
                IsAllStopOnHide,
                CanBeClippedOut,
                ClipBoxenabled,
                ClipBoxX,
                ClipBoxY,
                ClipBoxZ,
                ClipBoxsizeX,
                ClipBoxsizeY,
                ClipBoxsizeZ,
                BiasZmaxScale,
                BiasZmaxDistance,
                IsCameraSpace,
                IsFullEnvLight,
                IsClipOwnSetting,
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
                FadeXenabled,
                FadeXinner,
                FadeXouter,
                FadeYenabled,
                FadeYinner,
                FadeYouter,
                FadeZenabled,
                FadeZinner,
                FadeZouter,
                GlobalFogEnabled,
                GlobalFogInfluence,
                LTSEnabled
            } );
        }

        public override void Read( AVFXNode node ) {
            Assigned = true;
            ReadAVFX( Version, node );
            ReadAVFX( Attributes, node );

            foreach( var item in node.Children ) {
                switch( item.Name ) {
                    case AVFXSchedule.NAME:
                        var Scheduler = new AVFXSchedule();
                        Scheduler.Read( item );
                        Schedulers.Add( Scheduler );
                        break;
                    case AVFXTimeline.NAME:
                        var Timeline = new AVFXTimeline();
                        Timeline.Read( item );
                        Timelines.Add( Timeline );
                        break;
                    case AVFXEmitter.NAME:
                        var Emitter = new AVFXEmitter();
                        Emitter.Read( item );
                        Emitters.Add( Emitter );
                        break;
                    case AVFXParticle.NAME:
                        var Particle = new AVFXParticle();
                        Particle.Read( item );
                        Particles.Add( Particle );
                        break;
                    case AVFXEffector.NAME:
                        var Effector = new AVFXEffector();
                        Effector.Read( item );
                        Effectors.Add( Effector );
                        break;
                    case AVFXBinder.NAME:
                        var Binder = new AVFXBinder();
                        Binder.Read( item );
                        Binders.Add( Binder );
                        break;
                    case AVFXTexture.NAME:
                        var Texture = new AVFXTexture();
                        Texture.Read( item );
                        Textures.Add( Texture );
                        break;
                    case AVFXModel.NAME:
                        var Model = new AVFXModel();
                        Model.Read( item );
                        Models.Add( Model );
                        break;
                }
            }
        }

        public void AddTimeline( AVFXTimeline item ) {
            Timelines.Add( item );
        }
        public void RemoveTimeline( int idx ) {
            Timelines.RemoveAt( idx );
        }
        public void RemoveTimeline( AVFXTimeline item ) {
            Timelines.Remove( item );
        }
        //
        public void AddEffector( AVFXEffector item ) {
            Effectors.Add( item );
        }
        public void RemoveEffector( int idx ) {
            Effectors.RemoveAt( idx );
        }
        public void RemoveEffector( AVFXEffector item ) {
            Effectors.Remove( item );
        }
        //
        public void AddEmitter( AVFXEmitter item ) {
            Emitters.Add( item );
        }
        public void RemoveEmitter( int idx ) {
            Emitters.RemoveAt( idx );
        }
        public void RemoveEmitter( AVFXEmitter item ) {
            Emitters.Remove( item );
        }
        //
        public void AddParticle( AVFXParticle item ) {
            Particles.Add( item );
        }
        public void RemoveParticle( int idx ) {
            Particles.RemoveAt( idx );
        }
        public void RemoveParticle( AVFXParticle item ) {
            Particles.Remove( item );
        }
        //
        public void AddBinder( AVFXBinder item ) {
            Binders.Add( item );
        }
        public void RemoveBinder( int idx ) {
            Binders.RemoveAt( idx );
        }
        public void RemoveBinder( AVFXBinder item ) {
            Binders.Remove( item );
        }
        //
        public void AddTexture( AVFXTexture item ) {
            Textures.Add( item );
        }
        public AVFXTexture AddTexture() {
            var texture = new AVFXTexture();
            texture.ToDefault();
            Textures.Add( texture );
            return texture;
        }
        public void RemoveTexture( int idx ) {
            Textures.RemoveAt( idx );
        }
        public void RemoveTexture( AVFXTexture item ) {
            Textures.Remove( item );
        }
        //
        public void AddModel( AVFXModel item ) {
            Models.Add( item );
        }
        public AVFXModel AddModel() {
            var model = new AVFXModel();
            model.ToDefault();
            Models.Add( model );
            return model;
        }
        public void RemoveModel( int idx ) {
            Models.RemoveAt( idx );
        }
        public void RemoveModel( AVFXModel item ) {
            Models.Remove( item );
        }

        public override AVFXNode ToAVFX() {
            var baseAVFX = new AVFXNode( "AVFX" );

            baseAVFX.Children.Add( new AVFXLeaf( "Ver", 4, new byte[] { 0x13, 0x09, 0x11, 0x20 } ) ); //?
            PutAVFX( baseAVFX, Attributes );

            baseAVFX.Children.Add( new AVFXLeaf( "ScCn", 4, BitConverter.GetBytes( Schedulers.Count ) ) );
            baseAVFX.Children.Add( new AVFXLeaf( "TlCn", 4, BitConverter.GetBytes( Timelines.Count ) ) );
            baseAVFX.Children.Add( new AVFXLeaf( "EmCn", 4, BitConverter.GetBytes( Emitters.Count ) ) );
            baseAVFX.Children.Add( new AVFXLeaf( "PrCn", 4, BitConverter.GetBytes( Particles.Count ) ) );
            baseAVFX.Children.Add( new AVFXLeaf( "EfCn", 4, BitConverter.GetBytes( Effectors.Count ) ) );
            baseAVFX.Children.Add( new AVFXLeaf( "BdCn", 4, BitConverter.GetBytes( Binders.Count ) ) );
            baseAVFX.Children.Add( new AVFXLeaf( "TxCn", 4, BitConverter.GetBytes( Textures.Count ) ) );
            baseAVFX.Children.Add( new AVFXLeaf( "MdCn", 4, BitConverter.GetBytes( Models.Count ) ) );

            foreach( var schedElem in Schedulers ) {
                baseAVFX.Children.Add( schedElem.ToAVFX() );
            }
            foreach( var tmlnElem in Timelines ) {
                baseAVFX.Children.Add( tmlnElem.ToAVFX() );
            }
            foreach( var emitterElem in Emitters ) {
                baseAVFX.Children.Add( emitterElem.ToAVFX() );
            }
            foreach( var particleElement in Particles ) {
                baseAVFX.Children.Add( particleElement.ToAVFX() );
            }
            foreach( var effectorElem in Effectors ) {
                baseAVFX.Children.Add( effectorElem.ToAVFX() );
            }
            foreach( var bindElem in Binders ) {
                baseAVFX.Children.Add( bindElem.ToAVFX() );
            }
            foreach( var texElem in Textures ) {
                baseAVFX.Children.Add( texElem.ToAVFX() );
            }
            foreach( var modelElem in Models ) {
                baseAVFX.Children.Add( modelElem.ToAVFX() );
            }
            return baseAVFX;
        }

        public AVFXBase Clone() {
            var node = ToAVFX();
            var ret = new AVFXBase();
            ret.Read( node );
            return ret;
        }
    }
}
