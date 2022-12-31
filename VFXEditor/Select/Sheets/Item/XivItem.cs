using System;
using System.Collections.Generic;
using System.Text;

namespace VfxEditor.Select.Rows {
    public struct XivItemIds {
        public int PrimaryId;
        public int PrimaryVar;
        public int SecondaryId;
        public int SecondaryVar;

        public XivItemIds( ulong modelDataRaw ) {
            /*
             * Gear: [Id, Var, -, -] / [-,-,-,-]
             * Weapon: [Id, Var, Id, -] / [Id, Var, Id, -]
             */
            var b = BitConverter.GetBytes( modelDataRaw );
            PrimaryId = BitConverter.ToInt16( b, 0 ); // primary key
            PrimaryVar = BitConverter.ToInt16( b, 2 ); // primary variant (weapon if != 0)
            SecondaryId = BitConverter.ToInt16( b, 4 ); // secondary key
            SecondaryVar = BitConverter.ToInt16( b, 6 ); // secondary variant
        }

        public static ulong ToItemsId( int primaryId, int primaryVar, int secondaryId, int secondaryVar ) {
            List<byte> bytes = new();
            bytes.AddRange( BitConverter.GetBytes( (short)primaryId ) );
            bytes.AddRange( BitConverter.GetBytes( ( short )primaryVar ) );
            bytes.AddRange( BitConverter.GetBytes( ( short )secondaryId ) );
            bytes.AddRange( BitConverter.GetBytes( ( short )secondaryVar ) );
            return (ulong) BitConverter.ToInt64(bytes.ToArray());
        }
    }

    public abstract class XivItem {
        public string Name;
        public XivItemIds Ids;
        public XivItemIds SecondaryIds;
        public bool HasModel;
        public int Variant;

        public string RootPath;
        public string VfxRootPath;
        public string ImcPath;

        public int RowId;
        public ushort Icon;

        public XivItem( Lumina.Excel.GeneratedSheets.Item item ) {
            Name = item.Name.ToString();
            RowId = ( int )item.RowId;
            Icon = item.Icon;

            Ids = new XivItemIds( item.ModelMain );
            SecondaryIds = new XivItemIds( item.ModelSub );
            HasModel = ( Ids.PrimaryId != 0 );
        }

        public string GetVfxPath( int idx ) => $"{VfxRootPath}{idx.ToString().PadLeft( 4, '0' )}.avfx";
    }
}
