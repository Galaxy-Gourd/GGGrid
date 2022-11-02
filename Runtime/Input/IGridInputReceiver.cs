using GG.Core;

namespace GG.Grid
{
    /// <summary>
    /// Defines behavior for receiving grid input events.
    /// </summary>
    public interface IGridInputReceiver : ISystemComponent
    {
        /// <summary>
        /// Called when the pointer enters the bounds of the grid
        /// </summary>
        void DoGridPointerEnter();

        /// <summary>
        /// Called when the pointer exits the bounds of the grid
        /// </summary>
        void DoGridPointerExit();

        /// <summary>
        /// Called when the pointer enters the bounds of a cell
        /// </summary>
        void DoCellPointerEnter(GridCell cell);

        /// <summary>
        /// Called when the pointer exits the bounds of a cell
        /// </summary>
        void DoCellPointerExit(GridCell cell);
        
        /// <summary>
        /// Called when the pointer selects while inside the bounds of a cell
        /// </summary>
        void DoCellPointerSelect(GridCell cell);
        
        /// <summary>
        /// Called when the pointer select releases while a cell is active
        /// </summary>
        void DoCellPointerSelectRelease(GridCell cell);

        /// <summary>
        /// If the pointer was pressed down on the grid, this will be called when the pointer is released even if off-grid
        /// </summary>
        void DoGridPointerReleaseOffGrid();

        /// <summary>
        /// Called when the pointer alternate selects while inside the bounds of a cell
        /// </summary>
        void DoCellPointerAlternateSelect(GridCell cell);
        
        /// <summary>
        /// Called when the pointer alternate select releases while a cell is active
        /// </summary>
        void DoCellPointerAlternateSelectRelease(GridCell cell);
        
        /// <summary>
        /// Called when the pointer alternate selects anywhere over the grid view
        /// </summary>
        void DoAlternateSelect();
    }
}