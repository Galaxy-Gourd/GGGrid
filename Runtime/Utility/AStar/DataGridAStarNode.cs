using GG.Grid;

namespace GG.Grid
{
    public class DataGridAStarNode
    {
        public IGridCellAStarNavigable Cell;
        public DataGridAStarNode FromNode;
        public int GCost; // Distance from starting node
        public int HCost; // Distance from end node
        public int FCost => GCost + HCost;
    }
}