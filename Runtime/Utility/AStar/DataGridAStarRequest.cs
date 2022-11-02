using GG.Grid;

namespace GG.Grid
{
    public struct DataGridAStarRequest
    {
        public Grid Grid;
        public IGridView View;
        public IGridCellAStarNavigable Source;
        public IGridCellAStarNavigable Destination;
    }
}