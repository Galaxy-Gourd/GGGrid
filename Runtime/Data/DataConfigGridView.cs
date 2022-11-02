using System;
using GG.Core;
using UnityEngine;

namespace GG.Grid
{
    /// <summary>
    /// Holds data that can be used to influence construction of grid maps
    /// </summary>
    [CreateAssetMenu(
        fileName = "DAT_GridView", 
        menuName = "GG/UI/Grid/Standard View")]
    public class DataConfigGridView : DataConfig
    {
        #region PROPERTIES

        [Header("Grid")]
        [SerializeField] public int GridWidth;
        [SerializeField] public int GridHeight;
        
        [Header("View")]
        [SerializeField] public float GridCellSize;
        [SerializeField] public GameObject PrefabCellView;
 
        #endregion PROPERTIES
        
        
        #region VALIDATION

        internal Action OnValidated;
        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                OnValidated?.Invoke();
            }
        }

        #endregion VALIDATION
    }
}