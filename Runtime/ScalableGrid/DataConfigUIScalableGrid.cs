using UnityEngine;

namespace GG.Grid
{
    [CreateAssetMenu(
        fileName = "DAT_UIScalableGrid", 
        menuName = "GG/UI/Grid/UI Scalable Grid")]
    public class DataConfigUIScalableGrid : DataConfigGridView
    {
        [Header("Scalable")]
        [SerializeField] internal float CellSpacing;
        [SerializeField] internal float BorderSpacing;
        [SerializeField] internal float CellAreaInputNormalizedSize = 1;
        
    }
}