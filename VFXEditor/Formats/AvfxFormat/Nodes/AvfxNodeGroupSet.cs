using VfxEditor.Ui.Nodes;

namespace VfxEditor.AvfxFormat.Nodes {
    public class AvfxNodeGroupSet : NodeGroupSet {
        public const uint BinderColor = 0xFF5C8A2B;
        public const uint EmitterColor = 0xFF335E9E;
        public const uint ModelColor = 0xFF302B7D;
        public const uint ParticleColor = 0xFFAD576B;
        public const uint SchedColor = 0xFF5C1FB0;
        public const uint TextureColor = 0xFF1FB066;
        public const uint TimelineColor = 0xFFB0874A;
        public const uint EffectorColor = 0xFFD13D42;

        public readonly NodeGroup<AvfxEffector> Effectors;
        public readonly NodeGroup<AvfxEmitter> Emitters;
        public readonly NodeGroup<AvfxTexture> Textures;
        public readonly NodeGroup<AvfxModel> Models;
        public readonly NodeGroup<AvfxParticle> Particles;
        public readonly NodeGroup<AvfxBinder> Binders;
        public readonly NodeGroup<AvfxTimeline> Timelines;
        public readonly NodeGroup<AvfxScheduler> Schedulers;

        public AvfxNodeGroupSet( AvfxMain main ) {
            Effectors = new NodeGroup<AvfxEffector>( main.Effectors );
            Emitters = new NodeGroup<AvfxEmitter>( main.Emitters );
            Textures = new NodeGroup<AvfxTexture>( main.Textures );
            Models = new NodeGroup<AvfxModel>( main.Models );
            Particles = new NodeGroup<AvfxParticle>( main.Particles );
            Binders = new NodeGroup<AvfxBinder>( main.Binders );
            Timelines = new NodeGroup<AvfxTimeline>( main.Timelines );
            Schedulers = new NodeGroup<AvfxScheduler>( main.Schedulers );

            AllGroups.AddRange( new NodeGroup[] {
                Schedulers,
                Timelines,
                Emitters,
                Particles,
                Effectors,
                Binders,
                Textures,
                Models
            } );
        }
    }
}
