namespace VfxEditor.Select.Rows {
    public class XivMountSelected {
        public XivMount Mount;

        public int Count;
        public int VfxId;
        public string ImcPath;

        public bool VfxExists;

        public XivMountSelected( Lumina.Data.Files.ImcFile file, XivMount mount ) {
            Mount = mount;

            Count = file.Count;
            var imcData = file.GetVariant( mount.Variant - 1 );
            ImcPath = file.FilePath;
            VfxId = imcData.VfxId;
            VfxExists = !( VfxId == 0 );
        }

        public string GetVFXPath() {
            if( !VfxExists ) return "";
            return Mount.GetVfxPath( VfxId );
        }
    }
}
