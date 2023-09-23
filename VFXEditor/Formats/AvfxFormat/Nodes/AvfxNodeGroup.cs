using System;
using System.Collections.Generic;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.AvfxFormat {
    public abstract class AvfxNodeGroup {
        public abstract void Initialize();
        public abstract void PreImport( bool hasDependencies );
        public abstract void PostImport();
        public abstract void Dispose();
        public abstract void GetRenamingMap( Dictionary<string, string> meta );
        public abstract void ReadRenamingMap( Dictionary<string, string> meta );
    }

    public class NodeGroup<T> : AvfxNodeGroup where T : IWorkspaceUiItem {
        public readonly List<T> Items;

        public Action OnInit;
        public Action OnChange;
        public Action OnImportFinish;

        public bool IsInitialized { get; private set; } = false;
        public int PreImportSize { get; private set; } = 0;
        public bool ImportInProgress { get; private set; } = false;

        public NodeGroup( List<T> items ) {
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

        public override void PreImport( bool hasDependencies ) {
            ImportInProgress = true;
            PreImportSize = hasDependencies ? Items.Count : 0;
        }

        public override void PostImport() {
            OnImportFinish?.Invoke();
            ImportInProgress = false;
            OnImportFinish = null;
        }

        public override void GetRenamingMap( Dictionary<string, string> meta ) {
            Items.ForEach( item => IWorkspaceUiItem.GetRenamingMap( item, meta ) );
        }
        public override void ReadRenamingMap( Dictionary<string, string> meta ) {
            Items.ForEach( item => IWorkspaceUiItem.ReadRenamingMap( item, meta ) );
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
