using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace VfxEditor.Parsing.Color {
    public class ParsedSheetColor : ParsedSimpleBase<uint> {
        private static bool IsInitialized = false;
        private static readonly Dictionary<uint, Vector4> Colors = new();

        private static void Init() {
            if( IsInitialized ) return;
            IsInitialized = true;

            foreach( var item in Plugin.DataManager.GetExcelSheet<UIColor>() ) {
                var bytes = BitConverter.GetBytes( item.UIForeground );
                Colors[item.RowId] = new( bytes[3] / 255f, bytes[2] / 255f, bytes[1] / 255f, bytes[0] / 255f );
            }
        }

        // =================

        public readonly string Name;
        private Vector4 CurrentColor => Colors.TryGetValue( Value, out var color ) ? color : new Vector4( 1, 1, 1, 1 );

        public ParsedSheetColor( string name ) {
            Name = name;
            Init();
        }

        public override void Read( BinaryReader reader, int size ) => Read( reader, 0 );

        public override void Read( BinaryReader reader ) {
            Value = reader.ReadUInt32();
        }

        public override void Write( BinaryWriter writer ) => writer.Write( Value );

        public override void Draw( CommandManager manager ) {
            // Copy/Paste
            var copy = manager.Copy;
            if( copy.IsCopying ) copy.Ints[Name] = ( int )Value;
            if( copy.IsPasting && copy.Ints.TryGetValue( Name, out var val ) ) {
                copy.PasteCommand.Add( new ParsedSimpleCommand<uint>( this, ( uint )val ) );
            }

            DrawCombo( manager );

            ImGui.SameLine();
            ImGui.Text( Name );
        }

        private void DrawCombo( CommandManager manager ) {
            using var color = ImRaii.PushColor( ImGuiCol.Text, CurrentColor );
            var text = Colors.ContainsKey( Value ) ? $"----{Value}----" : "[NONE]";

            using var combo = ImRaii.Combo( "", text );
            if( !combo ) return;

            foreach( var option in Colors ) {
                var selected = option.Key == Value;

                using var _ = ImRaii.PushColor( ImGuiCol.Text, option.Value );
                if( ImGui.Selectable( $"----{option.Key}----", selected ) ) {
                    manager.Add( new ParsedSimpleCommand<uint>( this, option.Key ) );
                }

                if( selected ) ImGui.SetItemDefaultFocus();
            }
        }
    }
}
