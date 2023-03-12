using System;
using System.Collections.Generic;

namespace VfxEditor.AvfxFormat {
    public class UiNodeGroupSet {
        public readonly UiNodeGroup<AvfxEffector> Effectors;
        public readonly UiNodeGroup<AvfxEmitter> Emitters;
        public readonly UiNodeGroup<AvfxTexture> Textures;
        public readonly UiNodeGroup<AvfxModel> Models;
        public readonly UiNodeGroup<AvfxParticle> Particles;
        public readonly UiNodeGroup<AvfxBinder> Binders;
        public readonly UiNodeGroup<AvfxTimeline> Timelines;
        public readonly UiNodeGroup<AvfxScheduler> Schedulers;

        private readonly List<UiNodeGroup> AllGroups;

        public UiNodeGroupSet( AvfxMain main ) {
            Effectors = new UiNodeGroup<AvfxEffector>( main.Effectors );
            Emitters = new UiNodeGroup<AvfxEmitter>( main.Emitters );
            Textures = new UiNodeGroup<AvfxTexture>( main.Textures );
            Models = new UiNodeGroup<AvfxModel>( main.Models );
            Particles = new UiNodeGroup<AvfxParticle>( main.Particles );
            Binders = new UiNodeGroup<AvfxBinder>( main.Binders );
            Timelines = new UiNodeGroup<AvfxTimeline>( main.Timelines );
            Schedulers = new UiNodeGroup<AvfxScheduler>( main.Schedulers );

            AllGroups = new() {
                Binders,
                Emitters,
                Models,
                Particles,
                Schedulers,
                Textures,
                Timelines,
                Effectors
            };
        }

        public void Initialize() => AllGroups.ForEach( group => group.Initialize() );

        public Dictionary<string, string> GetRenamingMap() {
            Dictionary<string, string> ret = new();
            AllGroups.ForEach( group => group.PopulateWorkspaceMeta( ret ) );
            return ret;
        }

        public void ReadRenamingMap( Dictionary<string, string> renamingMap ) {
            AllGroups.ForEach( group => group.ReadWorkspaceMeta( renamingMap ) );
        }

        public void PreImport() => AllGroups.ForEach( group => group.PreImport() );

        public void PostImport() => AllGroups.ForEach( group => group.PostImport() );

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

        public abstract void Initialize();
        public abstract void PreImport();
        public abstract void PostImport();
        public abstract void Dispose();
        public abstract void PopulateWorkspaceMeta( Dictionary<string, string> meta );
        public abstract void ReadWorkspaceMeta( Dictionary<string, string> meta );
    }

    public class UiNodeGroup<T> : UiNodeGroup where T : AvfxNode {
        public readonly List<T> Items;

        public Action OnInit;
        public Action OnChange;
        public Action OnImportFinish;

        public bool IsInitialized { get; private set; } = false;
        public int PreImportSize { get; private set; } = 0;
        public bool ImportInProgress { get; private set; } = false;

        public UiNodeGroup( List<T> items ) {
            Items = items;
        }

        public override void Initialize() {
            if( IsInitialized ) return;
            IsInitialized = true;
            UpdateIdx();
            OnInit?.Invoke();
            OnInit = null;
        }

        public void UpdateIdx() {
            for( var i = 0; i < Items.Count; i++ ) Items[i].SetIdx( i );
        }

        public void TriggerOnChange() => OnChange?.Invoke();

        public override void PreImport() {
            ImportInProgress = true;
            PreImportSize = Items.Count;
        }

        public override void PostImport() {
            OnImportFinish?.Invoke();
            ImportInProgress = false;
            OnImportFinish = null;
        }

        public override void PopulateWorkspaceMeta( Dictionary<string, string> meta ) {
            Items.ForEach( item => IUiWorkspaceItem.PopulateWorkspaceMeta( item, meta ) );
        }
        public override void ReadWorkspaceMeta( Dictionary<string, string> meta ) {
            Items.ForEach( item => IUiWorkspaceItem.ReadWorkspaceMeta( item, meta ) );
        }

        public void AddAndUpdate( T item ) {
            Items.Add( item );
            UpdateIdx();
            TriggerOnChange();
        }

        public void AddAndUpdate( T item, int idx ) {
            Items.Insert( idx, item );
            UpdateIdx();
            TriggerOnChange();
        }

        public void RemoveAndUpdate( T item ) {
            item.SetIdx( -1 );
            Items.Remove( item );
            UpdateIdx();
            TriggerOnChange();
        }

        public override void Dispose() {
            OnInit = null;
            OnChange = null;
            OnImportFinish = null;
        }
    }
}
