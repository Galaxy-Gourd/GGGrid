namespace GG.Grid
{
    public class GridCellTestAStar : UIScalableGridCell
    {
        #region VARIABLES

        internal bool Impassible { get; set; }

        #endregion VARIABLES


        #region OVERRIDES

        public override bool IsAStarNavigable(string requestTag)
        {
            return !Impassible;
        }

        #endregion OVERRIDES
    }
}
