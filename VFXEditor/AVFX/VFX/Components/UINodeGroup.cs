using System;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.AVFXLib;

namespace VfxEditor.AVFX.VFX {
    public class UINodeGroupSet {
        public readonly UINodeGroup<UIBinder> Binders;
        public readonly UINodeGroup<UIEmitter> Emitters;
        public readonly UINodeGroup<UIModel> Models;
        public readonly UINodeGroup<UIParticle> Particles;
        public readonly UINodeGroup<UIScheduler> Schedulers;
        public readonly UINodeGroup<UITexture> Textures;
        public readonly UINodeGroup<UITimeline> Timelines;
        public readonly UINodeGroup<UIEffector> Effectors;

        private readonly List<UINodeGroup> AllGroups = new();

        public UINodeGroupSet(AVFXMain avfx) {
            AllGroups = new() {
                ( Binders = new UINodeGroup<UIBinder>() ),
                ( Emitters = new UINodeGroup<UIEmitter>() ),
                ( Models = new UINodeGroup<UIModel>() ),
                ( Particles = new UINodeGroup<UIParticle>() ),
                ( Schedulers = new UINodeGroup<UIScheduler>() ),
                ( Textures = new UINodeGroup<UITexture>() ),
                ( Timelines = new UINodeGroup<UITimeline>() ),
                ( Effectors = new UINodeGroup<UIEffector>() )
            };

            avfx.Binders.ForEach( item => Binders.Add( new UIBinder( item ) ) );
            avfx.Emitters.ForEach( item => Emitters.Add( new UIEmitter( item, this ) ) );
            avfx.Models.ForEach( item => Models.Add( new UIModel( item ) ) );
            avfx.Particles.ForEach( item => Particles.Add( new UIParticle( item, this ) ) );
            avfx.Schedulers.ForEach( item => Schedulers.Add( new UIScheduler( item, this ) ) );
            avfx.Textures.ForEach( item => Textures.Add( new UITexture( item ) ) );
            avfx.Timelines.ForEach( item => Timelines.Add( new UITimeline( item, this ) ) );
            avfx.Effectors.ForEach( item => Effectors.Add( new UIEffector( item ) ) );
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

    public abstract class UINodeGroup {
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

    public class UINodeGroup<T> : UINodeGroup where T : UINode {
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
