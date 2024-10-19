using ImGuiNET;
using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;
using VfxEditor.AtchFormat.Utils;

namespace VfxEditor.Formats.AtchFormat.Entry {
    public class AtchEntry : IUiItem {
        public int Size => 0x18;
        public readonly ParsedString Name = new( "Name" );
        public readonly ParsedBool Accessory = new( "Accessory" );
        public readonly List<AtchEntryState> States = [];

        public string WeaponName => AtchUtils.WeaponNames.TryGetValue( Name.Value, out var weaponName ) ? weaponName : "";

        public AtchEntry() { }

        public AtchEntry( BinaryReader reader ) : this()
        {
            Name.Value = FileUtils.Reverse( FileUtils.ReadString( reader ) );
        }

        public AtchEntry( AtchReader reader ) : this()
        {
            reader.UpdateStartPosition();
            Name.Value = reader.ReadName();
            Accessory.Value = reader.ReadAccessory();
            States.AddRange( reader.ReadOffsetState() );
        }

        public void ReadBody( BinaryReader reader, ushort numStates ) {
            for( var i = 0; i < numStates; i++ ) {
                States.Add( new( reader ) );
            }
        }

        public void Write( BinaryWriter writer ) {
            FileUtils.WriteString( writer, FileUtils.Reverse( Name.Value ), true );
        }

        public void WriteBody( BinaryWriter writer, int stringStartPos, BinaryWriter stringWriter, Dictionary<string, int> stringPos ) {
            States.ForEach( x => x.Write( writer, stringStartPos, stringWriter, stringPos ) );
        }

        public void Draw() {
            Name.Draw( 3, Name.Name, 0, ImGuiInputTextFlags.None );
            Accessory.Draw();

            for( var idx = 0; idx < States.Count; idx++ ) {
                var state = States[idx];
                if( ImGui.CollapsingHeader( $"State {idx} ({state.Bone.Value})###{idx}" ) ) {
                    using var _ = ImRaii.PushId( idx );
                    using var indent = ImRaii.PushIndent();

                    state.Draw();
                }
            }
        }

        public byte[] ToBytes()
        {
            var atchWriter = new AtchWriter(0, 0);
            atchWriter.WriteEntry( this );

            using var ms = new MemoryStream();
            using var writer = new BinaryWriter( ms );
            atchWriter.WriteTo( writer );
            atchWriter.Dispose();
            return ms.ToArray();
        }

        public void Log()
        {
            Dalamud.Log( $"Name: {Name.Value}" );
            Dalamud.Log( $"Accessory: {Accessory.Value}" );
            Dalamud.Log( $"State count: {States.Count}" );
        }
    }
}
