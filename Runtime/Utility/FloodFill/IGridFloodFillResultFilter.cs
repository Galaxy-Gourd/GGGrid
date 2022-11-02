namespace GG.Grid
{
    /// <summary>
    /// Allows classes to filter the results of a flood fill
    /// </summary>
    public interface IGridFloodFillResultFilter
    {
        DataGridFloodFillResult Filter(DataGridFloodFillResult rawResults);
    }
}