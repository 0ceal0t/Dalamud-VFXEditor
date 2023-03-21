using Lumina.Data.Files;
using System.Linq;

namespace VfxEditor.Select2.Vfx.Mount {
    public class MountRowSelected {
        public readonly MountRow Mount;
        public readonly string ImcPath;
        public readonly int VfxId;
        public readonly bool VfxExists;

        public MountRowSelected( ImcFile file, MountRow mount ) {
            Mount = mount;
            ImcPath = file.FilePath;
            var vfxIds = file.GetParts().Select( x => x.Variants[mount.Variant - 1] ).Where( x => x.VfxId != 0 ).Select( x => ( int )x.VfxId ).ToList();
            VfxId = vfxIds.Count > 0 ? vfxIds[0] : 0;
            VfxExists = VfxId > 0;
        }

        public string GetVfxPath() => VfxExists ? Mount.GetVfxPath( VfxId ) : "";
    }
}
