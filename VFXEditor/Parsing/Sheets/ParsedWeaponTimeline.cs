using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Parsing.Sheets {
    public class ParsedWeaponTimeline : ParsedSimpleBase<ushort> {
        private static bool IsInitialized = false;
        private static readonly Dictionary<ushort, string> Timelines = new();

        private static void Init() {
            if( IsInitialized ) return;
            IsInitialized = true;

            foreach( var item in Plugin.DataManager.GetExcelSheet<WeaponTimeline>() ) {
                Timelines[( ushort )item.RowId] = item.File.ToString();
            }
        }

        // =================

        public readonly string Name;
        private string CurrentTimeline => Timelines.TryGetValue( Value, out var timeline ) ? timeline : "";

        public ParsedWeaponTimeline( string name ) {
            Name = name;
            Init();
        }

        public override void Read( BinaryReader reader, int size ) => Read( reader, 0 );

        public override void Read( BinaryReader reader ) {
            Value = reader.ReadUInt16();
        }

        public override void Write( BinaryWriter writer ) => writer.Write( Value );

        public override void Draw( CommandManager manager ) {
            // Copy/Paste
            var copy = manager.Copy;
            if( copy.IsCopying ) copy.Ints[Name] = Value;
            if( copy.IsPasting && copy.Ints.TryGetValue( Name, out var val ) ) {
                copy.PasteCommand.Add( new ParsedSimpleCommand<ushort>( this, ( ushort )val ) );
            }

            DrawCombo( manager );

            ImGui.SameLine();
            ImGui.Text( Name );
        }

        private void DrawCombo( CommandManager manager ) {
            using var combo = ImRaii.Combo( "##Combo", $"[{Value}] {CurrentTimeline}" );
            if( !combo ) return;

            foreach( var option in Timelines ) {
                var selected = option.Key == Value;

                if( ImGui.Selectable( $"[{option.Key}] {option.Value}", selected ) ) {
                    manager.Add( new ParsedSimpleCommand<ushort>( this, option.Key ) );
                }

                if( selected ) ImGui.SetItemDefaultFocus();
            }
        }
    }
}
