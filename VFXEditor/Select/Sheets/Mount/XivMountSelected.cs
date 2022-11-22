namespace VfxEditor.Select.Rows {
    public class XivMountSelected {
        public readonly XivMount Mount;
        public readonly int Count;
        public readonly int VfxId;
        public readonly string ImcPath;
        public readonly bool VfxExists;

        public XivMountSelected( Lumina.Data.Files.ImcFile file, XivMount mount ) {
            Mount = mount;
            Count = file.Count;
            var imcData = file.GetVariant( mount.Variant - 1 );
            ImcPath = file.FilePath;
            VfxId = imcData.VfxId;
            VfxExists = !( VfxId == 0 );
        }

        public string GetVFXPath() => VfxExists ? Mount.GetVfxPath( VfxId ) : "";
    }
}
