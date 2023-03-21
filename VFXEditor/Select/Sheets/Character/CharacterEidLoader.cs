namespace VfxEditor.Select.Sheets {
    public class CharacterEidLoader : SheetLoader<XivCharacterEid> {
        public override void OnLoad() {
            foreach( var item in SheetManager.RaceAnimationIds ) {
                Items.Add( new( item.Key, item.Value.SkeletonId ) );
            }
        }
    }
}
