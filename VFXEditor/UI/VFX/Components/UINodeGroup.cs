using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace VFXEditor.UI.VFX {
    public class UINodeGroup {
        public static UINodeGroup<UIBinder> Binders;
        public static uint BinderColor = ImGui.GetColorU32( new Vector4( 0.17f, 0.54f, 0.36f, 1 ) );

        public static UINodeGroup<UIEmitter> Emitters;
        public static uint EmitterColor = ImGui.GetColorU32( new Vector4( 0.62f, 0.37f, 0.20f, 1 ) );

        public static UINodeGroup<UIModel> Models;
        public static uint ModelColor = ImGui.GetColorU32( new Vector4( 0.49f, 0.17f, 0.19f, 1 ) );

        public static UINodeGroup<UIParticle> Particles;
        public static uint ParticleColor = ImGui.GetColorU32( new Vector4( 0.42f, 0.34f, 0.68f, 1 ) );

        public static UINodeGroup<UIScheduler> Schedulers;
        public static uint SchedColor = ImGui.GetColorU32( new Vector4( 0.69f, 0.12f, 0.36f, 1 ) );

        public static UINodeGroup<UITexture> Textures;
        public static uint TextureColor = ImGui.GetColorU32( new Vector4( 0.40f, 0.69f, 0.12f, 1 ) );

        public static UINodeGroup<UITimeline> Timelines;
        public static uint TimelineColor = ImGui.GetColorU32( new Vector4( 0.29f, 0.53f, 0.69f, 1 ) );

        public static UINodeGroup<UIEffector> Effectors;
        public static uint EffectorColor = ImGui.GetColorU32( new Vector4( 0.26f, 0.24f, 0.82f, 1 ) );

        public static void SetupGroups() {
            Binders = new UINodeGroup<UIBinder>();
            Emitters = new UINodeGroup<UIEmitter>();
            Models = new UINodeGroup<UIModel>();
            Particles = new UINodeGroup<UIParticle>();
            Schedulers = new UINodeGroup<UIScheduler>();
            Textures = new UINodeGroup<UITexture>();
            Timelines = new UINodeGroup<UITimeline>();
            Effectors = new UINodeGroup<UIEffector>();
        }
        public static void InitGroups() {
            Binders?.Init();
            Emitters?.Init();
            Models?.Init();
            Particles?.Init();
            Schedulers?.Init();
            Textures?.Init();
            Timelines?.Init();
            Effectors?.Init();
        }
        public static void PreImportGroups() {
            Binders.PreImport();
            Emitters.PreImport();
            Models.PreImport();
            Particles.PreImport();
            Schedulers.PreImport();
            Textures.PreImport();
            Timelines.PreImport();
            Effectors.PreImport();
        }
    }

    public class UINodeGroup<T> : UINodeGroup where T : UINode {

        public List<T> Items = new List<T>();
        public Action OnInit;
        public Action OnChange;
        public bool IsInit = false;
        public int PreImportSize = 0;

        public UINodeGroup() {
        }

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

        public void Update() {
            OnChange?.Invoke();
        }

        public void Init() {
            UpdateIdx();
            IsInit = true;
            OnInit?.Invoke();
            OnInit = null;
        }

        public void UpdateIdx() {
            for( int i = 0; i < Items.Count; i++ ) {
                Items[i].Idx = i;
            }
        }

        public void PreImport() {
            PreImportSize = Items.Count;
        }
    }
}
