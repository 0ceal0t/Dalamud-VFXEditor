using System;
using System.Numerics;

namespace VfxEditor.Structs.Vfx {
    public unsafe class StaticVfx : BaseVfx {

        public StaticVfx( string path, Vector3 position, float rotation ) : base( path ) {
            Vfx = ( VfxStruct* )Plugin.ResourceLoader.StaticVfxCreate( path, "Client.System.Scheduler.Instance.VfxObject" );
            Plugin.ResourceLoader.StaticVfxRun( ( IntPtr )Vfx, 0.0f, 0xFFFFFFFF );

            UpdatePosition( position );
            UpdateRotation( new Vector3( 0, 0, rotation ) );
            Update();
        }

        public override void Remove() => Plugin.ResourceLoader.StaticVfxRemove( ( IntPtr )Vfx );
    }
}