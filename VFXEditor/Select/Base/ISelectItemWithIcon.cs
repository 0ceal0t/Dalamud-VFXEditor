namespace VfxEditor.Select.Base {
    public interface ISelectItemWithIcon : ISelectItem {
        public uint GetIconId();

        public static bool HasIcon( ISelectItem item, out uint iconId ) {
            if( item is ISelectItemWithIcon itemWithIcon ) {
                iconId = itemWithIcon.GetIconId();
                return true;
            }
            iconId = 0;
            return false;
        }
    }
}
