using System;
using System.Collections.Generic;

namespace VFXEditor.AVFX.VFX {
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
    }

    public class UINodeGroup<T> : UINodeGroup where T : UINode {
        public List<T> Items = new();
        public Action OnInit;
        public Action OnChange;
        public bool IsInitialized = false;
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

        public override void Dispose() {
            OnInit = null;
            OnChange = null;
        }
    }
}
