using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Formats.AtchFormat.Entry {
    public class AtchEntry : IUiItem {
        public readonly ParsedString Name = new( "Name" );
        public readonly ParsedBool Accessory = new( "Accessory" );
        public readonly List<AtchEntryState> States = new();

        public string WeaponName => AtchFile.WeaponNames.TryGetValue( Name.Value, out var weaponName ) ? weaponName : "";

        public AtchEntry() { }

        public AtchEntry( BinaryReader reader ) : this() {
            Name.Value = FileUtils.Reverse( FileUtils.ReadString( reader ) );
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
            Name.Draw( CommandManager.Atch, 3, Name.Name, 0, ImGuiInputTextFlags.None );
            Accessory.Draw( CommandManager.Atch );

            for( var idx = 0; idx < States.Count; idx++ ) {
                var state = States[idx];
                if( ImGui.CollapsingHeader( $"State {idx} ({state.Bone.Value})" ) ) {
                    using var _ = ImRaii.PushId( idx );
                    using var indent = ImRaii.PushIndent();

                    state.Draw();
                }
            }
        }
    }
}
