namespace Editroid
{
    /// <summary>
    /// Represents the showPhysics that are associated with a graphic tile.
    /// </summary>
    public enum Physics : byte
    {
        /// <summary>Indicates there is no physics associated with a tile, such as when referring to a location off screen.</summary>
        None = 0,
        /// <summary>An impassable tile.</summary>
        Solid,
        /// <summary>An intangible tile.</summary>
        Air,
        /// <summary>T tile that can be destroyed.</summary>
        Breakable,
        /// <summary>T tile that acts as a door marker.</summary>
        Door,
        /// <summary>T tile that acts as a door marker from one adjacent horizontal room to another.</summary>
        DoorHorizontal,
        /// <summary>Tile that acts as a door bubble placeholder. These are placed dynamically by the game engine.</summary>
        DoorBubble,
        /// <summary>
        /// Tiles that simulate a slope going up and right.
        /// </summary>
        SlopeRight,
        /// <summary>
        /// Tiles that simulate a slope going up and left.
        /// </summary>
        SlopeLeft,
        /// <summary>
        /// Tiles that represent the solid tile at the top of a slope
        /// </summary>
        SlopeTop,
    }
}
