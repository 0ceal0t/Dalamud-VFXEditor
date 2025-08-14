using Dalamud.Interface.Utility.Raii;
using HelixToolkit.SharpDX.Core.Animations;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using VfxEditor.PhybFormat.Collision.Capsule;
using VfxEditor.PhybFormat.Collision.Ellipsoid;
using VfxEditor.PhybFormat.Collision.NormalPlane;
using VfxEditor.PhybFormat.Collision.Sphere;
using VfxEditor.PhybFormat.Collision.ThreePointPlane;
using VfxEditor.Ui.Components.SplitViews;
using VfxEditor.Utils;

namespace VfxEditor.PhybFormat.Collision {
    public class PhybCollision : IPhysicsObject {
        public readonly PhybFile File;

        public readonly List<PhybCapsule> Capsules = [];
        public readonly List<PhybEllipsoid> Ellipsoids = [];
        public readonly List<PhybNormalPlane> NormalPlanes = [];
        public readonly List<PhybThreePointPlane> ThreePointPlanes = [];
        public readonly List<PhybSphere> Spheres = [];

        private readonly CommandSplitView<PhybCapsule> CapsuleSplitView;
        private readonly CommandSplitView<PhybEllipsoid> EllipsoidSplitView;
        private readonly CommandSplitView<PhybNormalPlane> NormalPlaneSplitView;
        private readonly CommandSplitView<PhybThreePointPlane> ThreePointPlaneSplitView;
        private readonly CommandSplitView<PhybSphere> SphereDropdown;

        public bool IsEmpty => Capsules.Count + Ellipsoids.Count + NormalPlanes.Count + ThreePointPlanes.Count + Spheres.Count == 0;

        public PhybCollision( PhybFile file, BinaryReader reader, bool isEmpty ) {
            File = file;

            if( !isEmpty ) {
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
            }

            CapsuleSplitView = new( "Capsule", Capsules, false,
                ( PhybCapsule item, int idx ) => item.Name.Value, () => new( File ), ( PhybCapsule _, bool _ ) => File.OnChange() );

            EllipsoidSplitView = new( "Ellipsoid", Ellipsoids, false,
                ( PhybEllipsoid item, int idx ) => item.Name.Value, () => new( File ), ( PhybEllipsoid _, bool _ ) => File.OnChange() );

            NormalPlaneSplitView = new( "Normal Plane", NormalPlanes, false,
                ( PhybNormalPlane item, int idx ) => item.Name.Value, () => new( File ), ( PhybNormalPlane _, bool _ ) => File.OnChange() );

            ThreePointPlaneSplitView = new( "Three-Point Plane", ThreePointPlanes, false,
                ( PhybThreePointPlane item, int idx ) => item.Name.Value, () => new( File ), ( PhybThreePointPlane _, bool _ ) => File.OnChange() );

            SphereDropdown = new( "Sphere", Spheres, false,
                ( PhybSphere item, int idx ) => item.Name.Value, () => new PhybSphere( File ), ( PhybSphere _, bool _ ) => File.OnChange() );
        }

        public void Write( BinaryWriter writer ) {
            if( IsEmpty ) return;

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
                ImGui.TextColored( UiUtils.RED_COLOR, "[ELLIPSOID/NORMAL PLANE/THREE-POINT PLANE]" );
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 1 );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Capsules" ) ) {
                if( tab ) CapsuleSplitView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Ellipsoids" ) ) {
                if( tab ) EllipsoidSplitView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Normal Planes" ) ) {
                if( tab ) NormalPlaneSplitView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Three-Point Planes" ) ) {
                if( tab ) ThreePointPlaneSplitView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Spheres" ) ) {
                if( tab ) SphereDropdown.Draw();
            }
        }

        public void AddPhysicsObjects( MeshBuilders meshes, Dictionary<string, Bone> boneMatrixes ) {
            foreach( var item in Capsules ) item.AddPhysicsObjects( meshes, boneMatrixes );
            foreach( var item in Ellipsoids ) item.AddPhysicsObjects( meshes, boneMatrixes );
            foreach( var item in NormalPlanes ) item.AddPhysicsObjects( meshes, boneMatrixes );
            foreach( var item in ThreePointPlanes ) item.AddPhysicsObjects( meshes, boneMatrixes );
            foreach( var item in Spheres ) item.AddPhysicsObjects( meshes, boneMatrixes );
        }
    }
}
