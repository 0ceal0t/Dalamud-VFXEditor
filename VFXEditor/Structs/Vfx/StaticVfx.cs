using System;
using System.Numerics;

namespace VFXEditor.Structs.Vfx {
    public unsafe class StaticVfx : BaseVfx {

        public StaticVfx( string path, Vector3 position ) : base( path ) {
            Vfx = ( VfxStruct* )VfxEditor.ResourceLoader.StaticVfxCreate( path, "Client.System.Scheduler.Instance.VfxObject" );
            VfxEditor.ResourceLoader.StaticVfxRun( ( IntPtr )Vfx, 0.0f, 0xFFFFFFFF );

            UpdatePosition( position );
            Update();
        }

        public override void Remove() {
            VfxEditor.ResourceLoader.StaticVfxRemove( ( IntPtr )Vfx );
        }
    }
}