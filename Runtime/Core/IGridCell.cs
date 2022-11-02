namespace GG.Grid
{
    public interface IGridCell
    {
        #region PROPERTIES

        int XCoord { get; }
        int YCoord { get; }
        int Index { get; }

        #endregion PROPERTIES
    }
}