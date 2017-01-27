namespace Windows.Enum
{
    public enum ModWorldTransform: int
    {
        /// <summary>
        /// Resets the current world transformation by using the identity matrix. If this mode is specified, ant XForm structure specified is ignored.
        /// </summary>
        Identity = 1,
        /// <summary>
        /// Multiplies the current transformation by the data in an XForm structure. (The specified XForm becomes the left multiplicand, and the current transformation becomes the right multiplicand.)
        /// </summary>
        LeftMultiply = 2,
        /// <summary>
        /// Multiplies the current transformation by the data in an XForm structure. (The specified XForm becomes the pight multiplicand, and the current transformation becomes the left multiplicand.)
        /// </summary>
        RightMultiply = 3,
    }
}
