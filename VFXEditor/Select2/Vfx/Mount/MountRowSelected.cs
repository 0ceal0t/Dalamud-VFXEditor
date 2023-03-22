using Lumina.Data.Files;
using System.Linq;
using VfxEditor.Select2.Shared.Mount;

namespace VfxEditor.Select2.Vfx.Mount
{
    public class MountRowSelected {
        public readonly string ImcPath;
        public readonly int VfxId;
        public readonly bool VfxExists;
        public readonly string VfxPath;

        public MountRowSelected( ImcFile file, MountRow mount ) {
            ImcPath = file.FilePath;
            var vfxIds = file.GetParts().Select( x => x.Variants[mount.Variant - 1] ).Where( x => x.VfxId != 0 ).Select( x => ( int )x.VfxId ).ToList();
            VfxId = vfxIds.Count > 0 ? vfxIds[0] : 0;
            VfxExists = VfxId > 0;
            VfxPath = VfxExists ? mount.GetVfxPath( VfxId ) : "";
        }
    }
}
