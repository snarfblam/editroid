using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid
{
	/// <summary>
	/// This class contains constants that are found within raw ROM data
	/// </summary>
	public static class RomValues
	{
		/// <summary>
		/// This value marks the end of a room's list of objects.
		/// </summary>
		public static byte EndOfObjects = 0xFD;
		/// <summary>
		/// This value marks the end of a room's data. This usually follows enemy data.
		/// </summary>
		public static byte EndOfScreenData = 0xFF;
		/// <summary>
		/// This value marks the end of a structs's data.
		/// </summary>
		public static byte EndOfStructData = 0xFF;
        /////// <summary>
        /////// This value identifies door data currentLevelItems in enemey data.
        /////// </summary>
        ////public static byte DoorMarker = 0x02;
        /////// <summary>
        /////// This value identifies door data currentLevelItems in gameItem data.
        /////// </summary>
        ////public static byte DoorMarkerItemData = 0x02;

        /////// <summary>
        /////// This value identifies enemy data currentLevelItems in enemy data and gameItem data.
        /////// </summary>
        ////public static byte EnemyMarker = 0x01;
        /////// <summary>
        /////// This value identifies enemy data currentLevelItems for enemies that respawn in enemy data.
        /////// </summary>
        ////public static byte RespawnEnemyMarker = 0x07;
        /////// <summary>
        /////// This value identifies the Tourian access bridge in enemy data.
        /////// </summary>
        ////public static byte TourianAccessBridgeMarker = 0x06;

        public enum EnemyIdentifiers : byte
        {
            Enemy = 0x01,
            Door = 0x02,
            Elevator = 0x04,
            Bridge = 0x06,
            RespawnEnemy = 0x07
        }

	}
}
