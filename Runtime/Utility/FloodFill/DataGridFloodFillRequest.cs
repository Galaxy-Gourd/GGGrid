
namespace GG.Grid
{
    /// <summary>
    /// Defines the data needed to reauest a grid flood fill
    /// </summary>
    public struct DataGridFloodFillRequest
    {
        public string Key;  // We can use this to interpret results differently if needed
        public Grid Grid;
        public IGridView View;
        public IGridFloodFillNavigable[] SourceCells;
        public int Range;
        public bool ApplyMovePenalty;
        public IGridFloodFillResultFilter Filter;
    }
}