using System.Collections.Generic;
using System.IO;

using VFXEditor.AVFXLib.Binder;
using VFXEditor.AVFXLib.Effector;
using VFXEditor.AVFXLib.Emitter;
using VFXEditor.AVFXLib.Model;
using VFXEditor.AVFXLib.Particle;
using VFXEditor.AVFXLib.Scheduler;
using VFXEditor.AVFXLib.Texture;
using VFXEditor.AVFXLib.Timeline;

namespace VFXEditor.AVFXLib {
    public class AVFXMain : AVFXBase {
        public static AVFXMain FromStream( BinaryReader reader ) {
            var main = new AVFXMain();

            reader.ReadInt32(); // AVFX
            var size = reader.ReadInt32();
            main.Read( reader, size );

            return main;
        }

        // ==================================

        public readonly AVFXInt Version = new( "Ver" );
        public readonly AVFXBool IsDelayFastParticle = new( "bDFP" );
        public readonly AVFXBool IsFitGround = new( "bFG" );
        public readonly AVFXBool IsTranformSkip = new( "bTS" );
        public readonly AVFXBool IsAllStopOnHide = new( "bASH" );
        public readonly AVFXBool CanBeClippedOut = new( "bCBC" );
        public readonly AVFXBool ClipBoxenabled = new( "bCul" );
        public readonly AVFXFloat ClipBoxX = new( "CBPx" );
        public readonly AVFXFloat ClipBoxY = new( "CBPy" );
        public readonly AVFXFloat ClipBoxZ = new( "CBPz" );
        public readonly AVFXFloat ClipBoxsizeX = new( "CBSx" );
        public readonly AVFXFloat ClipBoxsizeY = new( "CBSy" );
        public readonly AVFXFloat ClipBoxsizeZ = new( "CBSz" );
        public readonly AVFXFloat BiasZmaxScale = new( "ZBMs" );
        public readonly AVFXFloat BiasZmaxDistance = new( "ZBMd" );
        public readonly AVFXBool IsCameraSpace = new( "bCmS" );
        public readonly AVFXBool IsFullEnvLight = new( "bFEL" );
        public readonly AVFXBool IsClipOwnSetting = new( "bOSt" );
        public readonly AVFXFloat NearClipBegin = new( "NCB" );
        public readonly AVFXFloat NearClipEnd = new( "NCE" );
        public readonly AVFXFloat FarClipBegin = new( "FCB" );
        public readonly AVFXFloat FarClipEnd = new( "FCE" );
        public readonly AVFXFloat SoftParticleFadeRange = new( "SPFR" );
        public readonly AVFXFloat SoftKeyOffset = new( "SKO" );
        public readonly AVFXEnum<DrawLayer> DrawLayerType = new( "DwLy" );
        public readonly AVFXEnum<DrawOrder> DrawOrderType = new( "DwOT" );
        public readonly AVFXEnum<DirectionalLightSource> DirectionalLightSourceType = new( "DLST" );
        public readonly AVFXEnum<PointLightSouce> PointLightsType1 = new( "PL1S" );
        public readonly AVFXEnum<PointLightSouce> PointLightsType2 = new( "PL2S" );
        public readonly AVFXFloat RevisedValuesPosX = new( "RvPx" );
        public readonly AVFXFloat RevisedValuesPosY = new( "RvPy" );
        public readonly AVFXFloat RevisedValuesPosZ = new( "RvPz" );
        public readonly AVFXFloat RevisedValuesRotX = new( "RvRx" );
        public readonly AVFXFloat RevisedValuesRotY = new( "RvRy" );
        public readonly AVFXFloat RevisedValuesRotZ = new( "RvRz" );
        public readonly AVFXFloat RevisedValuesScaleX = new( "RvSx" );
        public readonly AVFXFloat RevisedValuesScaleY = new( "RvSy" );
        public readonly AVFXFloat RevisedValuesScaleZ = new( "RvSz" );
        public readonly AVFXFloat RevisedValuesR = new( "RvR" );
        public readonly AVFXFloat RevisedValuesG = new( "RvG" );
        public readonly AVFXFloat RevisedValuesB = new( "RvB" );
        public readonly AVFXBool FadeXenabled = new( "AFXe" );
        public readonly AVFXFloat FadeXinner = new( "AFXi" );
        public readonly AVFXFloat FadeXouter = new( "AFXo" );
        public readonly AVFXBool FadeYenabled = new( "AFYe" );
        public readonly AVFXFloat FadeYinner = new( "AFYi" );
        public readonly AVFXFloat FadeYouter = new( "AFYo" );
        public readonly AVFXBool FadeZenabled = new( "AFZe" );
        public readonly AVFXFloat FadeZinner = new( "AFZi" );
        public readonly AVFXFloat FadeZouter = new( "AFZo" );
        public readonly AVFXBool GlobalFogEnabled = new( "bGFE" );
        public readonly AVFXFloat GlobalFogInfluence = new( "GFIM" );
        public readonly AVFXBool LTSEnabled = new( "bLTS" );

        private readonly List<AVFXBase> Children;

        public readonly List<AVFXScheduler> Schedulers = new();
        public readonly List<AVFXTimeline> Timelines = new();
        public readonly List<AVFXEmitter> Emitters = new();
        public readonly List<AVFXParticle> Particles = new();
        public readonly List<AVFXEffector> Effectors = new();
        public readonly List<AVFXBinder> Binders = new();
        public readonly List<AVFXTexture> Textures = new();
        public readonly List<AVFXModel> Models = new();

        public AVFXMain() : base( "AVFX" ) {
            Children = new List<AVFXBase> {
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
                ClipBoxsizeX,
                ClipBoxsizeY,
                ClipBoxsizeZ,
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
            };

            Version.SetValue( 0x20110913 );
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            Peek( reader, Children, size ); // read but then reset the position

            ReadNested( reader, ( BinaryReader _reader, string _name, int _size ) => {
                switch( _name ) {
                    case AVFXScheduler.NAME:
                        var Scheduler = new AVFXScheduler();
                        Scheduler.Read( _reader, _size );
                        Schedulers.Add( Scheduler );
                        break;
                    case AVFXTimeline.NAME:
                        var Timeline = new AVFXTimeline();
                        Timeline.Read( _reader, _size );
                        Timelines.Add( Timeline );
                        break;
                    case AVFXEmitter.NAME:
                        var Emitter = new AVFXEmitter();
                        Emitter.Read( _reader, _size );
                        Emitters.Add( Emitter );
                        break;
                    case AVFXParticle.NAME:
                        var Particle = new AVFXParticle();
                        Particle.Read( _reader, _size );
                        Particles.Add( Particle );
                        break;
                    case AVFXEffector.NAME:
                        var Effector = new AVFXEffector();
                        Effector.Read( _reader, _size );
                        Effectors.Add( Effector );
                        break;
                    case AVFXBinder.NAME:
                        var Binder = new AVFXBinder();
                        Binder.Read( _reader, _size );
                        Binders.Add( Binder );
                        break;
                    case AVFXTexture.NAME:
                        var Texture = new AVFXTexture();
                        Texture.Read( _reader, _size );
                        Textures.Add( Texture );
                        break;
                    case AVFXModel.NAME:
                        var Model = new AVFXModel();
                        Model.Read( _reader, _size );
                        Models.Add( Model );
                        break;
                }
            }, size );
        }

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Children, assigned );

        protected override void WriteContents( BinaryWriter writer ) {
            WriteNested( writer, Children );

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

        public void AddTimeline( AVFXTimeline item ) {
            Timelines.Add( item );
        }

        public void RemoveTimeline( int idx ) {
            Timelines.RemoveAt( idx );
        }

        public void RemoveTimeline( AVFXTimeline item ) {
            Timelines.Remove( item );
        }

        public void AddEffector( AVFXEffector item ) {
            Effectors.Add( item );
        }

        public void RemoveEffector( int idx ) {
            Effectors.RemoveAt( idx );
        }

        public void RemoveEffector( AVFXEffector item ) {
            Effectors.Remove( item );
        }

        public void AddEmitter( AVFXEmitter item ) {
            Emitters.Add( item );
        }

        public void RemoveEmitter( int idx ) {
            Emitters.RemoveAt( idx );
        }

        public void RemoveEmitter( AVFXEmitter item ) {
            Emitters.Remove( item );
        }

        public void AddParticle( AVFXParticle item ) {
            Particles.Add( item );
        }

        public void RemoveParticle( int idx ) {
            Particles.RemoveAt( idx );
        }

        public void RemoveParticle( AVFXParticle item ) {
            Particles.Remove( item );
        }

        public void AddBinder( AVFXBinder item ) {
            Binders.Add( item );
        }

        public void RemoveBinder( int idx ) {
            Binders.RemoveAt( idx );
        }

        public void RemoveBinder( AVFXBinder item ) {
            Binders.Remove( item );
        }

        public void AddTexture( AVFXTexture item ) {
            Textures.Add( item );
        }

        public AVFXTexture AddTexture() {
            var texture = new AVFXTexture();
            Textures.Add( texture );
            return texture;
        }

        public void RemoveTexture( int idx ) {
            Textures.RemoveAt( idx );
        }

        public void RemoveTexture( AVFXTexture item ) {
            Textures.Remove( item );
        }

        public void AddModel( AVFXModel item ) {
            Models.Add( item );
        }

        public AVFXModel AddModel() {
            var model = new AVFXModel();
            Models.Add( model );
            return model;
        }

        public void RemoveModel( int idx ) {
            Models.RemoveAt( idx );
        }

        public void RemoveModel( AVFXModel item ) {
            Models.Remove( item );
        }
    }
}
