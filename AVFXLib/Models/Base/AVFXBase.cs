using AVFXLib.AVFX;
using AVFXLib.Main;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXBase : Base
    {
        public LiteralInt Version = new LiteralInt( "Ver" );

        public LiteralBool IsDelayFastParticle = new LiteralBool("bDFP");
        public LiteralBool IsFitGround = new LiteralBool("bFG");
        public LiteralBool IsTranformSkip = new LiteralBool("bTS");
        public LiteralBool IsAllStopOnHide = new LiteralBool("bASH");
        public LiteralBool CanBeClippedOut = new LiteralBool("bCBC");
        public LiteralBool ClipBoxenabled = new LiteralBool("bCul");
        public LiteralFloat ClipBoxX = new LiteralFloat("CBPx");
        public LiteralFloat ClipBoxY = new LiteralFloat("CBPy");
        public LiteralFloat ClipBoxZ = new LiteralFloat("CBPz");
        public LiteralFloat ClipBoxsizeX = new LiteralFloat("CBSx");
        public LiteralFloat ClipBoxsizeY = new LiteralFloat("CBSy");
        public LiteralFloat ClipBoxsizeZ = new LiteralFloat("CBSz");
        public LiteralFloat BiasZmaxScale = new LiteralFloat("ZBMs");
        public LiteralFloat BiasZmaxDistance = new LiteralFloat("ZBMd");
        public LiteralBool IsCameraSpace = new LiteralBool("bCmS");
        public LiteralBool IsFullEnvLight = new LiteralBool("bFEL");
        public LiteralBool IsClipOwnSetting = new LiteralBool("bOSt");
        public LiteralFloat SoftParticleFadeRange = new LiteralFloat("SPFR");
        public LiteralFloat SoftKeyOffset = new LiteralFloat("SKO");
        public LiteralEnum<DrawLayer> DrawLayerType = new LiteralEnum<DrawLayer>("DwLy");
        public LiteralEnum<DrawOrder> DrawOrderType = new LiteralEnum<DrawOrder>("DwOT");
        public LiteralEnum<DirectionalLightSource> DirectionalLightSourceType = new LiteralEnum<DirectionalLightSource>("DLST");
        public LiteralEnum<PointLightSouce> PointLightsType1 = new LiteralEnum<PointLightSouce>("PL1S");
        public LiteralEnum<PointLightSouce> PointLightsType2 = new LiteralEnum<PointLightSouce>("PL2S");
        public LiteralFloat RevisedValuesPosX = new LiteralFloat("RvPx");
        public LiteralFloat RevisedValuesPosY = new LiteralFloat("RvPy");
        public LiteralFloat RevisedValuesPosZ = new LiteralFloat("RvPz");
        public LiteralFloat RevisedValuesRotX = new LiteralFloat("RvRx");
        public LiteralFloat RevisedValuesRotY = new LiteralFloat("RvRy");
        public LiteralFloat RevisedValuesRotZ = new LiteralFloat("RvRz");
        public LiteralFloat RevisedValuesScaleX = new LiteralFloat("RvSx");
        public LiteralFloat RevisedValuesScaleY = new LiteralFloat("RvSy");
        public LiteralFloat RevisedValuesScaleZ = new LiteralFloat("RvSz");
        public LiteralFloat RevisedValuesR = new LiteralFloat("RvR");
        public LiteralFloat RevisedValuesG = new LiteralFloat("RvG");
        public LiteralFloat RevisedValuesB = new LiteralFloat("RvB");
        public LiteralBool FadeXenabled = new LiteralBool("AFXe");
        public LiteralFloat FadeXinner = new LiteralFloat("AFXi");
        public LiteralFloat FadeXouter = new LiteralFloat("AFXo");
        public LiteralBool FadeYenabled = new LiteralBool("AFYe");
        public LiteralFloat FadeYinner = new LiteralFloat("AFYi");
        public LiteralFloat FadeYouter = new LiteralFloat("AFYo");
        public LiteralBool FadeZenabled = new LiteralBool("AFZe");
        public LiteralFloat FadeZinner = new LiteralFloat("AFZi");
        public LiteralFloat FadeZouter = new LiteralFloat("AFZo");
        public LiteralBool GlobalFogEnabled = new LiteralBool("bGFE");
        public LiteralFloat GlobalFogInfluence = new LiteralFloat("GFIM");
        public LiteralBool LTSEnabled = new LiteralBool("bLTS");

        public List<AVFXSchedule> Schedulers = new List<AVFXSchedule>();
        public List<AVFXTimeline> Timelines = new List<AVFXTimeline>();
        public List<AVFXEmitter> Emitters = new List<AVFXEmitter>();
        public List<AVFXParticle> Particles = new List<AVFXParticle>();
        public List<AVFXEffector> Effectors = new List<AVFXEffector>();
        public List<AVFXBinder> Binders = new List<AVFXBinder>();
        public List<AVFXTexture> Textures = new List<AVFXTexture>();
        public List<AVFXModel> Models = new List<AVFXModel>();

        List<Base> Attributes;

        public AVFXBase() : base("AVFX")
        {
            Attributes = new List<Base>(new Base[]{
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
            });
        }

        public override void Read(AVFXNode node) {
            Assigned = true;
            ReadAVFX( Version, node );
            ReadAVFX(Attributes, node);

            foreach (AVFXNode item in node.Children)
            {
                switch (item.Name)
                {
                    case AVFXSchedule.NAME:
                        AVFXSchedule Scheduler = new AVFXSchedule();
                        Scheduler.Read(item);
                        Schedulers.Add(Scheduler);
                        break;
                    case AVFXTimeline.NAME:
                        AVFXTimeline Timeline = new AVFXTimeline();
                        Timeline.Read(item);
                        Timelines.Add(Timeline);
                        break;
                    case AVFXEmitter.NAME:
                        AVFXEmitter Emitter = new AVFXEmitter();
                        Emitter.Read(item);
                        Emitters.Add(Emitter);
                        break;
                    case AVFXParticle.NAME:
                        AVFXParticle Particle = new AVFXParticle();
                        Particle.Read(item);
                        Particles.Add(Particle);
                        break;
                    case AVFXEffector.NAME:
                        AVFXEffector Effector = new AVFXEffector();
                        Effector.Read(item);
                        Effectors.Add(Effector);
                        break;
                    case AVFXBinder.NAME:
                        AVFXBinder Binder = new AVFXBinder();
                        Binder.Read(item);
                        Binders.Add(Binder);
                        break;
                    case AVFXTexture.NAME:
                        AVFXTexture Texture = new AVFXTexture();
                        Texture.Read(item);
                        Textures.Add(Texture);
                        break;
                    case AVFXModel.NAME:
                        AVFXModel Model = new AVFXModel();
                        Model.Read(item);
                        Models.Add(Model);
                        break;
                }
            }
        }

        // ==== ADD/REMOVE ======
        public void AddTimeline( AVFXTimeline item )
        {
            Timelines.Add( item );
        }
        public void RemoveTimeline(int idx)
        {
            Timelines.RemoveAt(idx);
        }
        public void RemoveTimeline( AVFXTimeline item ) {
            Timelines.Remove( item );
        }
        //
        public void AddEffector( AVFXEffector item )
        {
            Effectors.Add( item );
        }
        public void RemoveEffector(int idx)
        {
            Effectors.RemoveAt(idx);
        }
        public void RemoveEffector( AVFXEffector item ) {
            Effectors.Remove( item );
        }
        //
        public void AddEmitter( AVFXEmitter item )
        {
            Emitters.Add( item );
        }
        public void RemoveEmitter(int idx)
        {
            Emitters.RemoveAt(idx);
        }
        public void RemoveEmitter( AVFXEmitter item ) {
            Emitters.Remove( item );
        }
        //
        public void AddParticle( AVFXParticle item )
        {
            Particles.Add( item );
        }
        public void RemoveParticle(int idx)
        {
            Particles.RemoveAt(idx);
        }
        public void RemoveParticle( AVFXParticle item ) {
            Particles.Remove( item );
        }
        //
        public void AddBinder( AVFXBinder item )
        {
            Binders.Add( item );
        }
        public void RemoveBinder(int idx)
        {
            Binders.RemoveAt(idx);
        }
        public void RemoveBinder( AVFXBinder item ) {
            Binders.Remove( item );
        }
        //
        public void AddTexture( AVFXTexture item )
        {
            Textures.Add( item );
        }
        public AVFXTexture AddTexture()
        {
            AVFXTexture texture = new AVFXTexture();
            texture.ToDefault();
            Textures.Add(texture);
            return texture;
        }
        public void RemoveTexture(int idx)
        {
            Textures.RemoveAt(idx);
        }
        public void RemoveTexture( AVFXTexture item ) {
            Textures.Remove( item );
        }
        //
        public void AddModel( AVFXModel item )
        {
            Models.Add( item );
        }
        public AVFXModel AddModel()
        {
            AVFXModel model = new AVFXModel();
            model.ToDefault();
            Models.Add( model );
            return model;
        }
        public void RemoveModel(int idx )
        {
            Models.RemoveAt( idx );
        }
        public void RemoveModel( AVFXModel item ) {
            Models.Remove( item );
        }
        // ======= EXPORT =======
        public override AVFXNode ToAVFX()
        {
            AVFXNode baseAVFX = new AVFXNode("AVFX");

            baseAVFX.Children.Add(new AVFXLeaf("Ver", 4, new byte[] { 0x13, 0x09, 0x11, 0x20 })); //?
            PutAVFX(baseAVFX, Attributes);

            baseAVFX.Children.Add(new AVFXLeaf("ScCn", 4, BitConverter.GetBytes(Schedulers.Count())));
            baseAVFX.Children.Add(new AVFXLeaf("TlCn", 4, BitConverter.GetBytes(Timelines.Count())));
            baseAVFX.Children.Add(new AVFXLeaf("EmCn", 4, BitConverter.GetBytes(Emitters.Count())));
            baseAVFX.Children.Add(new AVFXLeaf("PrCn", 4, BitConverter.GetBytes(Particles.Count())));
            baseAVFX.Children.Add(new AVFXLeaf("EfCn", 4, BitConverter.GetBytes(Effectors.Count())));
            baseAVFX.Children.Add(new AVFXLeaf("BdCn", 4, BitConverter.GetBytes(Binders.Count())));
            baseAVFX.Children.Add(new AVFXLeaf("TxCn", 4, BitConverter.GetBytes(Textures.Count())));
            baseAVFX.Children.Add(new AVFXLeaf("MdCn", 4, BitConverter.GetBytes(Models.Count())));

            // SCHEDULE
            foreach (AVFXSchedule schedElem in Schedulers)
            {
                baseAVFX.Children.Add(schedElem.ToAVFX());
            }
            // TIMELINES
            foreach (AVFXTimeline tmlnElem in Timelines)
            {
                baseAVFX.Children.Add(tmlnElem.ToAVFX());
            }
            // EMITTERS
            foreach (AVFXEmitter emitterElem in Emitters)
            {
                baseAVFX.Children.Add(emitterElem.ToAVFX());
            }
            // PARTICLES
            foreach (AVFXParticle particleElement in Particles)
            {
                baseAVFX.Children.Add(particleElement.ToAVFX());
            }
            // EFFECTORS
            foreach (AVFXEffector effectorElem in Effectors)
            {
                baseAVFX.Children.Add(effectorElem.ToAVFX());
            }
            // BINDERS
            foreach (AVFXBinder bindElem in Binders)
            {
                baseAVFX.Children.Add(bindElem.ToAVFX());
            }
            // TEXTURES
            foreach (AVFXTexture texElem in Textures)
            {
                baseAVFX.Children.Add(texElem.ToAVFX());
            }
            // MODELS
            foreach (AVFXModel modelElem in Models)
            {
                baseAVFX.Children.Add(modelElem.ToAVFX());
            }
            return baseAVFX;
        }

        public AVFXBase Clone()
        {
            AVFXNode node = ToAVFX();
            AVFXBase ret = new AVFXBase();
            ret.Read(node);
            return ret;
        }
    }
}
