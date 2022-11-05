namespace GG.Grid
{
    public interface IGridCellAStarNavigable : IGridCell
    {
        #region METHODS

        /// <summary>
        /// Returns false if this cell should not be used in calculating AStar paths
        /// </summary>
        /// <returns></returns>
        bool IsAStarNavigable(string requestTag);

        #endregion METHODS
    }
}