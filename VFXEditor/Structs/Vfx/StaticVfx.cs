using System;
using System.Numerics;

namespace VFXEditor.Structs.Vfx {
    public unsafe class StaticVfx : BaseVfx {

        public StaticVfx( string path, Vector3 position, float rotation ) : base( path ) {
            Vfx = ( VfxStruct* )VfxEditor.ResourceLoader.StaticVfxCreate( path, "Client.System.Scheduler.Instance.VfxObject" );
            VfxEditor.ResourceLoader.StaticVfxRun( ( IntPtr )Vfx, 0.0f, 0xFFFFFFFF );

            UpdatePosition( position );
            UpdateRotation( new Vector3(0, 0, rotation ) );
            Update();
        }

        public override void Remove() {
            VfxEditor.ResourceLoader.StaticVfxRemove( ( IntPtr )Vfx );
        }
    }
}