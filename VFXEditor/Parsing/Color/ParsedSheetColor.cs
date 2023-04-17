using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public override void Write( BinaryWriter writer ) {
            writer.Write( Value );
        }

        public override void Draw( string id, CommandManager manager ) {
            // Copy/Paste
            var copy = manager.Copy;
            if( copy.IsCopying ) copy.Ints[Name] = ( int )Value;
            if( copy.IsPasting && copy.Ints.TryGetValue( Name, out var val ) ) {
                copy.PasteCommand.Add( new ParsedSimpleCommand<uint>( this, ( uint )val ) );
            }

            ImGui.PushStyleColor( ImGuiCol.Text, CurrentColor );
            var text = Colors.ContainsKey( Value ) ? $"----{Value}----" : "[NONE]";

            if( ImGui.BeginCombo( $"{id}{Name}", text ) ) {
                foreach( var option in Colors ) {
                    var selected = option.Key == Value;
                    ImGui.PushStyleColor( ImGuiCol.Text, option.Value );
                    if( ImGui.Selectable( $"----{option.Key}----{id}", selected ) ) {
                        manager.Add( new ParsedSimpleCommand<uint>( this, option.Key ) );
                    }
                    ImGui.PopStyleColor();

                    if( selected ) ImGui.SetItemDefaultFocus();
                }
                ImGui.EndCombo();
            }
            ImGui.PopStyleColor();

            ImGui.SameLine();
            ImGui.Text( Name );
        }
    }
}
