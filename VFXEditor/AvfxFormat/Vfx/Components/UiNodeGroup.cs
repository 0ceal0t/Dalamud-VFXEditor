using System;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.AVFXLib;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiNodeGroupSet {
        public readonly UiNodeGroup<UiBinder> Binders;
        public readonly UiNodeGroup<UiEmitter> Emitters;
        public readonly UiNodeGroup<UiModel> Models;
        public readonly UiNodeGroup<UiParticle> Particles;
        public readonly UiNodeGroup<UiScheduler> Schedulers;
        public readonly UiNodeGroup<UiTexture> Textures;
        public readonly UiNodeGroup<UiTimeline> Timelines;
        public readonly UiNodeGroup<UiEffector> Effectors;

        private readonly List<UiNodeGroup> AllGroups = new();

        public UiNodeGroupSet(AVFXMain avfx) {
            AllGroups = new() {
                ( Binders = new UiNodeGroup<UiBinder>() ),
                ( Emitters = new UiNodeGroup<UiEmitter>() ),
                ( Models = new UiNodeGroup<UiModel>() ),
                ( Particles = new UiNodeGroup<UiParticle>() ),
                ( Schedulers = new UiNodeGroup<UiScheduler>() ),
                ( Textures = new UiNodeGroup<UiTexture>() ),
                ( Timelines = new UiNodeGroup<UiTimeline>() ),
                ( Effectors = new UiNodeGroup<UiEffector>() )
            };

            avfx.Binders.ForEach( item => Binders.Add( new UiBinder( item ) ) );
            avfx.Emitters.ForEach( item => Emitters.Add( new UiEmitter( item, this ) ) );
            avfx.Models.ForEach( item => Models.Add( new UiModel( item ) ) );
            avfx.Particles.ForEach( item => Particles.Add( new UiParticle( item, this ) ) );
            avfx.Schedulers.ForEach( item => Schedulers.Add( new UiScheduler( item, this ) ) );
            avfx.Textures.ForEach( item => Textures.Add( new UiTexture( item ) ) );
            avfx.Timelines.ForEach( item => Timelines.Add( new UiTimeline( item, this ) ) );
            avfx.Effectors.ForEach( item => Effectors.Add( new UiEffector( item ) ) );
        }

        public void Init() => AllGroups.ForEach( group => group.Init() );

        public Dictionary<string, string> GetRenamingMap() {
            Dictionary<string, string> ret = new();
            AllGroups.ForEach( group => group.PopulateWorkspaceMeta( ret ) );
            return ret;
        }

        public void ReadRenamingMap( Dictionary<string, string> renamingMap ) {
            AllGroups.ForEach( group => group.ReadWorkspaceMeta( renamingMap ) );
        }

        public void PreImport() => AllGroups.ForEach( group => group.PreImport() );

        public void Dispose() => AllGroups.ForEach( group => group.Dispose() );
    }

    public abstract class UiNodeGroup {
        public const uint BinderColor = 0xFF5C8A2B;
        public const uint EmitterColor = 0xFF335E9E;
        public const uint ModelColor = 0xFF302B7D;
        public const uint ParticleColor = 0xFFAD576B;
        public const uint SchedColor = 0xFF5C1FB0;
        public const uint TextureColor = 0xFF1FB066;
        public const uint TimelineColor = 0xFFB0874A;
        public const uint EffectorColor = 0xFFD13D42;

        public abstract void Init();
        public abstract void PreImport();
        public abstract void Dispose();
        public abstract void PopulateWorkspaceMeta( Dictionary<string, string> meta );
        public abstract void ReadWorkspaceMeta( Dictionary<string, string> meta );
    }

    public class UiNodeGroup<T> : UiNodeGroup where T : UiNode {
        public readonly List<T> Items = new();

        public Action OnInit;
        public Action OnChange;

        public bool IsInitialized { get; private set; } = false;
        public int PreImportSize { get; private set; } = 0;

        public void Remove( T item ) {
            item.Idx = -1;
            Items.Remove( item );
            UpdateIdx();
            Update();
        }

        public void Add( T item ) {
            item.Idx = Items.Count;
            Items.Add( item );
        }

        public void Update() => OnChange?.Invoke();

        public override void Init() {
            UpdateIdx();
            IsInitialized = true;
            OnInit?.Invoke();
            OnInit = null;
        }

        public void UpdateIdx() {
            for( var i = 0; i < Items.Count; i++ ) {
                Items[i].Idx = i;
            }
        }

        public override void PreImport() {
            PreImportSize = Items.Count;
        }

        public override void PopulateWorkspaceMeta( Dictionary<string, string> meta ) {
            Items.ForEach( item => item.PopulateWorkspaceMeta( meta ) );
        }
        public override void ReadWorkspaceMeta( Dictionary<string, string> meta ) {
            Items.ForEach( item => item.ReadWorkspaceMeta( meta ) );
        }

        public override void Dispose() {
            OnInit = null;
            OnChange = null;
        }
    }
}
