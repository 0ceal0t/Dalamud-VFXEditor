using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VFXEditor.Helper;

namespace VFXEditor.TMB.Items {
    public class C012 : TMBItem {
        private int Duration = 30;
        private int Unk_3 = 0;
        private string Path = "";
        private short BindPoint1 = 1;
        private short BindPoint2 = 0xFF;
        private short BindPoint3 = 2;
        private short BindPoint4 = 0xFF;
        private int Unk_4;
        private int Unk_5;
        private Vector3 Scale = new( 1 );
        private Vector3 Rotation = new( 0 );
        private Vector3 Position = new( 0 );
        private Vector4 RGBA = new( 1 );

        public static readonly string DisplayName = "VFX (C012)";
        public override string GetDisplayName() => DisplayName;
        public override string GetName() => "C012";

        public C012() { }
        public C012( BinaryReader reader ) {
            var startPos = reader.BaseStream.Position; // [C012] + 8

            ReadInfo( reader );
            Duration = reader.ReadInt32();
            Unk_3 = reader.ReadInt32();

            var offset = reader.ReadInt32(); // path offset: [C012] + offset + 8 = path
            var savePos = reader.BaseStream.Position;
            reader.BaseStream.Seek( startPos + offset, SeekOrigin.Begin );
            Path = FileHelper.ReadString( reader );
            reader.BaseStream.Seek( savePos, SeekOrigin.Begin );

            BindPoint1 = reader.ReadInt16();
            BindPoint2 = reader.ReadInt16();
            BindPoint3 = reader.ReadInt16();
            BindPoint4 = reader.ReadInt16();

            var pairs = ReadPairs( 4, reader, startPos );
            Scale = ListToVec3( pairs[0] );
            Rotation = ListToVec3( pairs[1] );
            Position = ListToVec3( pairs[2] );
            RGBA = ListToVec4( pairs[3] );

            Unk_4 = reader.ReadInt32();
            Unk_5 = reader.ReadInt32();
        }

        public override int GetSize() => 0x48;
        public override int GetExtraSize() => 4 * ( 3 + 3 + 3 + 4 ); // scale(3) + rotation(3) + position(3) + rgba(4)

        public override void Write( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, Dictionary<string, int> stringPositions, int stringPos ) {
            var startPos = ( int )entryWriter.BaseStream.Position + entryPos;
            var endPos = stringPositions[Path] + stringPos;
            var offset = endPos - startPos - 8;

            WriteInfo( entryWriter );
            entryWriter.Write( Duration );
            entryWriter.Write( Unk_3 );

            entryWriter.Write( offset );

            entryWriter.Write( BindPoint1 );
            entryWriter.Write( BindPoint2 );
            entryWriter.Write( BindPoint3 );
            entryWriter.Write( BindPoint4 );

            var pairs = new List<List<float>> {
                Vec3ToList( Scale ),
                Vec3ToList( Rotation ),
                Vec3ToList( Position ),
                Vec4ToList( RGBA )
            };
            WritePairs( entryWriter, extraWriter, extraPos, startPos, pairs );

            entryWriter.Write( Unk_4 );
            entryWriter.Write( Unk_5 );
        }

        public override void Draw( string id ) {
            DrawInfo( id );
            ImGui.InputInt( $"Duration{id}", ref Duration );
            ImGui.InputInt( $"Unknown 3{id}", ref Unk_3 );
            ImGui.InputInt( $"Unknown 4{id}", ref Unk_4 );
            ImGui.InputInt( $"Unknown 5{id}", ref Unk_5 );
            ImGui.InputText( $"Path{id}", ref Path, 255 );
            FileHelper.ShortInput( $"Bind Point 1{id}", ref BindPoint1 );
            FileHelper.ShortInput( $"Bind Point 2{id}", ref BindPoint2 );
            FileHelper.ShortInput( $"Bind Point 3{id}", ref BindPoint3 );
            FileHelper.ShortInput( $"Bind Point 4{id}", ref BindPoint4 );

            ImGui.InputFloat3( $"Scale{id}", ref Scale );
            ImGui.InputFloat3( $"Rotation{id}", ref Rotation );
            ImGui.InputFloat3( $"Position{id}", ref Position );
            ImGui.InputFloat4( $"RGBA{id}", ref RGBA );
        }

        public override void PopulateStringList( List<string> stringList ) {
            AddString( Path, stringList );
        }
    }
}