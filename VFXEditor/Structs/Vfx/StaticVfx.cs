using System.Numerics;

namespace VFXEditor.Structs.Vfx {
    public unsafe class StaticVfx : BaseVfx {

        public StaticVfx( Plugin plugin, string path, Vector3 position ) : base( plugin, path ) {
            Vfx = Plugin.ResourceLoader.StaticVfxCreate( path, "Client.System.Scheduler.Instance.VfxObject" );
            Plugin.ResourceLoader.StaticVfxRun( Vfx, 0.0f, 0xFFFFFFFF );

            UpdatePosition( position );
            Update();
        }

        public override void Remove() {
            Plugin.ResourceLoader.StaticVfxRemove( Vfx );
        }
    }
}