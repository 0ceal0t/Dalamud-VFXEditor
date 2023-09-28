using System.Collections.Generic;

namespace VfxEditor.AvfxFormat {
    public class AvfxNodeGroupSet {
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

        private readonly List<AvfxNodeGroup> AllGroups = new();

        public AvfxNodeGroupSet( AvfxMain main ) {
            Effectors = new NodeGroup<AvfxEffector>( main.Effectors );
            Emitters = new NodeGroup<AvfxEmitter>( main.Emitters );
            Textures = new NodeGroup<AvfxTexture>( main.Textures );
            Models = new NodeGroup<AvfxModel>( main.Models );
            Particles = new NodeGroup<AvfxParticle>( main.Particles );
            Binders = new NodeGroup<AvfxBinder>( main.Binders );
            Timelines = new NodeGroup<AvfxTimeline>( main.Timelines );
            Schedulers = new NodeGroup<AvfxScheduler>( main.Schedulers );

            AllGroups.AddRange( new AvfxNodeGroup[] {
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

        public void Initialize() => AllGroups.ForEach( group => group.Initialize() );

        public Dictionary<string, string> GetRenamingMap() {
            Dictionary<string, string> ret = new();
            AllGroups.ForEach( group => group.GetRenamingMap( ret ) );
            return ret;
        }

        public void ReadRenamingMap( Dictionary<string, string> renamingMap ) {
            AllGroups.ForEach( group => group.ReadRenamingMap( renamingMap ) );
        }

        public void PreImport( bool hasDependencies ) => AllGroups.ForEach( group => group.PreImport( hasDependencies ) );

        public void PostImport() => AllGroups.ForEach( group => group.PostImport() );

        public void Dispose() => AllGroups.ForEach( group => group.Dispose() );
    }
}
