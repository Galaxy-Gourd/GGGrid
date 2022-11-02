using System.Collections.Generic;

namespace GG.Grid
{
    /// <summary>
    /// Defines the data returned from a flood fill operation
    /// </summary>
    public struct DataGridFloodFillResult
    {
        /// <summary>
        /// We can use this to interpret results differently if needed
        /// </summary>
        public string Key;
        
        /// <summary>
        /// Indices of the cells that the request is able to visit. List orders tiles by distance from source; tiles in list index
        /// 0 are 1 square away, index 1 = 2 squares away, etc.
        /// </summary>
        public List<int[]> ValidIndices;

        /// <summary>
        /// 
        /// </summary>
        public List<int> OuterIndices;
    }
}