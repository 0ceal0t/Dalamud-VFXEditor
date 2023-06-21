using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using VfxEditor.PhybFormat.Collision.Capsule;
using VfxEditor.PhybFormat.Collision.Ellipsoid;
using VfxEditor.PhybFormat.Collision.NormalPlane;
using VfxEditor.PhybFormat.Collision.Sphere;
using VfxEditor.PhybFormat.Collision.ThreePointPlane;
using VfxEditor.Ui.Components;
using VfxEditor.Utils;

namespace VfxEditor.PhybFormat.Collision {
    public class PhybCollision {
        public readonly PhybFile File;

        public readonly List<PhybCapsule> Capsules = new();
        public readonly List<PhybEllipsoid> Ellipsoids = new();
        public readonly List<PhybNormalPlane> NormalPlanes = new();
        public readonly List<PhybThreePointPlane> ThreePointPlanes = new();
        public readonly List<PhybSphere> Spheres = new();

        private readonly SimpleDropdown<PhybCapsule> CapsuleDropdown;
        private readonly SimpleDropdown<PhybEllipsoid> EllipsoidDropdown;
        private readonly SimpleDropdown<PhybNormalPlane> NormalPlaneDropdown;
        private readonly SimpleDropdown<PhybThreePointPlane> ThreePointPlaneDropdown;
        private readonly SimpleDropdown<PhybSphere> SphereDropdown;

        public PhybCollision( PhybFile file, BinaryReader reader ) {
            File = file;

            var numCapsules = reader.ReadByte();
            var numEllipsoids = reader.ReadByte();
            var numNormalPlanes = reader.ReadByte();
            var numThreePointPlanes = reader.ReadByte();
            var numSpheres = reader.ReadByte();
            reader.ReadBytes( 3 ); // padding: 0xCC, 0xCC, 0xCC

            for( var i = 0; i < numCapsules; i++ ) Capsules.Add( new PhybCapsule( file, reader ) );
            for( var i = 0; i < numEllipsoids; i++ ) Ellipsoids.Add( new PhybEllipsoid( file, reader ) );
            for( var i = 0; i < numNormalPlanes; i++ ) NormalPlanes.Add( new PhybNormalPlane( file, reader ) );
            for( var i = 0; i < numThreePointPlanes; i++ ) ThreePointPlanes.Add( new PhybThreePointPlane( file, reader ) );
            for( var i = 0; i < numSpheres; i++ ) Spheres.Add( new PhybSphere( file, reader ) );

            CapsuleDropdown = new( "Capsule", Capsules,
                ( PhybCapsule item, int idx ) => item.Name.Value, () => new PhybCapsule( File ), () => CommandManager.Phyb );
            EllipsoidDropdown = new( "Ellipsoid", Ellipsoids,
                ( PhybEllipsoid item, int idx ) => item.Name.Value, () => new PhybEllipsoid( File ), () => CommandManager.Phyb );
            NormalPlaneDropdown = new( "Normal Plane", NormalPlanes,
                ( PhybNormalPlane item, int idx ) => item.Name.Value, () => new PhybNormalPlane( File ), () => CommandManager.Phyb );
            ThreePointPlaneDropdown = new( "Three-Point Plane", ThreePointPlanes,
                ( PhybThreePointPlane item, int idx ) => item.Name.Value, () => new PhybThreePointPlane( File ), () => CommandManager.Phyb );
            SphereDropdown = new( "Sphere", Spheres,
                ( PhybSphere item, int idx ) => item.Name.Value, () => new PhybSphere( File ), () => CommandManager.Phyb );
        }

        public void Write( BinaryWriter writer ) {
            writer.Write( ( byte )Capsules.Count );
            writer.Write( ( byte )Ellipsoids.Count );
            writer.Write( ( byte )NormalPlanes.Count );
            writer.Write( ( byte )ThreePointPlanes.Count );
            writer.Write( ( byte )Spheres.Count );
            writer.Write( ( byte )0xCC );
            writer.Write( ( byte )0xCC );
            writer.Write( ( byte )0xCC );

            foreach( var item in Capsules ) item.Write( writer );
            foreach( var item in Ellipsoids ) item.Write( writer );
            foreach( var item in NormalPlanes ) item.Write( writer );
            foreach( var item in ThreePointPlanes ) item.Write( writer );
            foreach( var item in Spheres ) item.Write( writer );
        }

        public void Draw() {
            using var _ = ImRaii.PushId( "Collision" );

            if( Ellipsoids.Count > 0 || NormalPlanes.Count > 0 || ThreePointPlanes.Count > 0 ) {
                ImGui.TextColored( UiUtils.RED_COLOR, "ELLIPSOID/NORMAL PLANE/THREE-POINT PLANE FOUND!" );
            }

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Capsules" ) ) {
                if( tab ) CapsuleDropdown.Draw();
            }

            using( var tab = ImRaii.TabItem( "Ellipsoids" ) ) {
                if( tab ) EllipsoidDropdown.Draw();
            }

            using( var tab = ImRaii.TabItem( "Normal Planes" ) ) {
                if( tab ) NormalPlaneDropdown.Draw();
            }

            using( var tab = ImRaii.TabItem( "Three-Point Planes" ) ) {
                if( tab ) ThreePointPlaneDropdown.Draw();
            }

            using( var tab = ImRaii.TabItem( "Spheres" ) ) {
                if( tab ) SphereDropdown.Draw();
            }
        }
    }
}
