namespace GG.Grid
{
    public struct DataGridAStarResult
    {
        /// <summary>
        /// Ordered array of indices representing the path to the destination (includes source and target)
        /// If destination is unreachable, this array is empty
        /// </summary>
        public int[] Path;
    }
}