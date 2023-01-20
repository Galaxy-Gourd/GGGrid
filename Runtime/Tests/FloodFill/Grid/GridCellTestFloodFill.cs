namespace GG.Grid
{
    public class GridCellTestFloodFill : UIScalableGridCell
    {
        #region VARIABLES

        internal bool Impassible { get; set; }

        #endregion VARIABLES


        #region OVERRIDES

        public override int FloodFillMovePenalty(string requestTag)
        {
            return Impassible ? -1 : 0;
        }

        #endregion OVERRIDES
    }
}
