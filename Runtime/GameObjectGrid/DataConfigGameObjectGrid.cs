using UnityEngine;

namespace GG.Grid
{
    [CreateAssetMenu(
        fileName = "DAT_UIScalableGrid", 
        menuName = "GG/UI/Grid/UI Scalable Grid")]
    public class DataConfigGameObjectGrid : DataConfigGridView
    {
        [Header("GameObject Input")]
        [SerializeField] public LayerMask GridLayers;
        [SerializeField] public float GridInputDistance = 1000;
    }
}