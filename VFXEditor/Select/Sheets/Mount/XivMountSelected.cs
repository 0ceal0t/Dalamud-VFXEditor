using System.Linq;

namespace VfxEditor.Select.Rows {
    public class XivMountSelected {
        public readonly XivMount Mount;
        public readonly string ImcPath;
        public readonly int VfxId;
        public readonly bool VfxExists;

        public XivMountSelected( Lumina.Data.Files.ImcFile file, XivMount mount ) {
            Mount = mount;
            ImcPath = file.FilePath;
            var vfxIds = file.GetParts().Select( x => x.Variants[mount.Variant - 1] ).Where( x => x.VfxId != 0 ).Select( x => ( int )x.VfxId ).ToList();
            VfxId = vfxIds.Count > 0 ? vfxIds[0] : 0;
            VfxExists = VfxId > 0;
        }

        public string GetVFXPath() => VfxExists ? Mount.GetVfxPath( VfxId ) : "";
    }
}
