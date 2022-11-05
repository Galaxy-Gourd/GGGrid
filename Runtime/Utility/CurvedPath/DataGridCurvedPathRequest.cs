using System.Collections.Generic;

namespace GG.Grid
{
    public struct DataGridCurvedPathRequest
    {
        public IGridView View;
        public GridCell Source;
        public GridCell Arc1;
        public GridCell Arc2;
        public GridCell Destination;
        public float PathResolution;
    }
}