using FFXIVClientStructs.FFXIV.Client.System.String;
using System;
using System.Numerics;

namespace VfxEditor.Interop.Structs.Vfx {
    public unsafe class StaticVfx : BaseVfx {

        public StaticVfx( string path, Vector3 position, float rotation ) : base( path ) {
            Vfx = Plugin.ResourceLoader.StaticVfxCreate(
                ( new Utf8String( path ) ).StringPtr,
                ( new Utf8String( "Client.System.Scheduler.Instance.VfxObject" ) ).StringPtr
            );
            Plugin.ResourceLoader.StaticVfxRun( Vfx, 0.0f, 0xFFFFFFFF );

            UpdatePosition( position );
            UpdateRotation( rotation );
            Update();
        }

        public override void Remove() => Plugin.ResourceLoader.StaticVfxRemove( Vfx );
    }
}