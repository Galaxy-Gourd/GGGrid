using UnityEngine;

namespace GG.Grid
{
    [RequireComponent(typeof(RectTransform))]
    public class UIScalableGridCell : GridCell
    {
        #region VARIABLES

        internal RectTransform CellRect => _rect;
        
        protected RectTransform _rect;

        #endregion VARIABLES


        #region INITIALIZATION

        protected virtual void Awake()
        {
            _rect = GetComponent<RectTransform>();
        }

        #endregion INITIALIZATION
    }
}