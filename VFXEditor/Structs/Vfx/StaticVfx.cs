using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Actors.Types;
using Dalamud.Plugin;

using ImGuizmoNET;

namespace VFXEditor.Structs.Vfx {
    public class StaticVfx : BaseVfx {

        public StaticVfx( Plugin plugin, string path, Vector3 position) : base( plugin, path ) {
            Vfx = _Plugin.ResourceLoader.VfxCreate( path, "Client.System.Scheduler.Instance.VfxObject" );
            _Plugin.ResourceLoader.VfxRun( Vfx, 0.0f, 0xFFFFFFFF );

            UpdatePosition( position );
            Update();

            UpdateMatrix();
        }
        
        public override void Remove() {
            _Plugin.ResourceLoader.VfxRemove( Vfx );
        }
    }
}