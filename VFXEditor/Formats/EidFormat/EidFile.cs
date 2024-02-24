using Dalamud.Interface.Utility.Raii;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Animations;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.EidFormat.BindPoint;
using VfxEditor.FileManager;
using VfxEditor.Formats.EidFormat.Skeleton;
using VfxEditor.Interop.Havok.Ui;
using VfxEditor.Ui.Components;
using VfxEditor.Utils;

namespace VfxEditor.EidFormat {
    public class EidFile : FileManagerFile {
        public readonly List<EidBindPoint> BindPoints = new();
        public readonly CommandDropdown<EidBindPoint> Dropdown;

        private readonly short Version1;
        private readonly short Version2;
        private readonly uint Unk1;

        private bool NewData => Version1 == 0x3132;

        public readonly EidSkeletonView Skeleton;
        public bool BindPointsUpdated = true;
        private bool SkeletonTabOpen = false;

        public EidFile( BinaryReader reader, string sourcePath, bool verify ) : base() {
            reader.ReadInt32(); // magic 00656964
            Version1 = reader.ReadInt16();
            Version2 = reader.ReadInt16();
            var count = reader.ReadInt32();
            Unk1 = reader.ReadUInt32();

            for( var i = 0; i < count; i++ ) {
                BindPoints.Add( NewData ? new EidBindPointNew( reader ) : new EidBindPointOld( reader ) );
            }

            if( verify ) Verified = FileUtils.Verify( reader, ToBytes(), null );

            Dropdown = new( "Bind Point", BindPoints,
                ( EidBindPoint item, int idx ) => $"Bind Point {item.GetName()}", () => new EidBindPointNew() );

            Skeleton = new( this, Path.IsPathRooted( sourcePath ) ? null : sourcePath );
        }

        public override void Write( BinaryWriter writer ) {
            FileUtils.WriteMagic( writer, "eid" );
            writer.Write( Version1 );
            writer.Write( Version2 );
            writer.Write( BindPoints.Count );
            writer.Write( Unk1 );

            foreach( var bindPoint in BindPoints ) bindPoint.Write( writer );
        }

        public override void Draw() {
            var size = SkeletonView.CalculateSize( SkeletonTabOpen, Plugin.Configuration.EidSkeletonSplit );

            using var style = ImRaii.PushStyle( ImGuiStyleVar.WindowPadding, new Vector2( 0, 0 ) );
            using( var child = ImRaii.Child( "Child", size, false ) ) {
                using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
                if( !tabBar ) return;

                SkeletonTabOpen = false;

                using( var tab = ImRaii.TabItem( "Bind Points" ) ) {
                    if( tab ) Dropdown.Draw();
                }

                using( var tab = ImRaii.TabItem( "3D View" ) ) {
                    if( tab ) {
                        Skeleton.Draw();
                        SkeletonTabOpen = true;
                    }
                }
            }

            if( !SkeletonTabOpen ) Skeleton.DrawSplit( ref Plugin.Configuration.EidSkeletonSplit );
        }

        public void AddBindPoints( MeshBuilder mesh, Dictionary<string, Bone> boneMatrixes ) {
            BindPoints.ForEach( x => x.AddBindPoint( mesh, boneMatrixes ) );
        }

        public override void OnChange() {
            BindPointsUpdated = true;
        }
    }
}
