using System;
using System.Collections.Generic;
using Editroid.ROM;

namespace Editroid
{
    /// <summary>
    /// Examines possibility and cost of modifications to a structure.
    /// </summary>
    class StructAnalizer
    {
        Structure structure;
        public StructAnalizer(Structure structure) {
            this.structure = structure;
        }

        public int CostOfAddingTile(int x, int y) {
            if (structure.Data[x, y] != Structure.EmptyTile)
                return 0;
            if (structure.Data.IsRowEmpty(y))
                return getCostOfAddingTileToEmptyRow(y);
            return getCostOfExtendingRow(x, y);
        }

        /// <summary>
        /// Gets the number of extra bytes a struct will need to add a tile to
        /// the specified coordinate, assuming the row already exists.
        /// </summary>
        private int getCostOfExtendingRow(int x, int y) {
            int firstTileX = structure.Data.GetFirstTileX(y);
            if (x < firstTileX) 
                return firstTileX - x;
            
            int lastTileX = structure.Data.GetLastTileX(y);
            if (x > lastTileX)
                return x - lastTileX;

            // firstTileX < X < lastTileX. The tile already exists, so:
            return 0;
        }

        /// <summary>
        /// Gets the number of extra bytes a struct will need to add a tile
        /// to the specified row, assuming the row does not already exist.
        /// </summary>
        private int getCostOfAddingTileToEmptyRow(int y) {
            int lastRow = structure.LastRow;
            if (y <= lastRow) return 0; // Row already exists

            const int BytesPerAddedRow = 2;
            int rowsAdded = y - lastRow;
            return BytesPerAddedRow * rowsAdded;
        }




    }
}
