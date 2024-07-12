namespace VfxEditor.Select.Tabs.Items {
    public class ItemTabVfxAccessory : ItemTabVfx {
        public ItemTabVfxAccessory( SelectDialog dialog, string name ) : base( dialog, name, "Accessory-Vfx", ItemTabFilter.Accessory | ItemTabFilter.Glasses ) { }

        protected override bool IsDisabled() => !Plugin.PenumbraIpc.PenumbraEnabled;
    }
}
