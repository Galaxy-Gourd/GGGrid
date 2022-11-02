namespace GG.Grid
{
    public interface IGridFloodFillNavigable : IGridCell
    {
        /// <summary>
        /// Overwrite to implement custom flood fill move penalties for your grid cell implementation
        /// </summary>
        int FloodFillMovePenalty(string key);
    }
}