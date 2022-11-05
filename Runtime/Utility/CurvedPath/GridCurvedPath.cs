using System.Collections.Generic;
using GG.Core;
using UnityEngine;

namespace GG.Grid
{
    public static class GridCurvedPath
    {
        #region VARIABLES

        public static DataGridCurvedPathResult Result { get; private set; }
        
        private static DataGridCurvedPathRequest _request;
        private static readonly List<GridCell> _path = new();
        
        #endregion VARIABLES


        #region API

        public static DataGridCurvedPathResult CurvedPath(DataGridCurvedPathRequest request)
        {
            ClearCaches(request);
            RunCurvedPath();

            Result = new DataGridCurvedPathResult()
            {
                Path = _path
            };

            return Result;
        }

        #endregion API


        #region LOGIC

        private static void ClearCaches(DataGridCurvedPathRequest request)
        {
            _path.Clear();
            _request = request;
        }

        private static void RunCurvedPath()
        {
            Vector2 p1 = new(_request.Source.XCoord, _request.Source.YCoord);
            Vector2 p2 = new(_request.Arc1.XCoord, _request.Arc1.YCoord);
            Vector2 p3 = new(_request.Arc2.XCoord, _request.Arc2.YCoord);
            Vector2 p4 = new(_request.Destination.XCoord, _request.Destination.YCoord);
            List<Vector2> path = new List<Vector2>();

            float resolution = 0;
            resolution += Vector2.Distance(p1, p2);
            resolution += Vector2.Distance(p2, p3);
            resolution += Vector2.Distance(p3, p4);
            resolution *= _request.PathResolution;
            
            // Build path of points
            path.Add(p1);
            int intRes = Mathf.CeilToInt(resolution);
            for (int i = 1; i < intRes; i++)
            {
                float time = (float)i / intRes;
                Vector2 point = BezierCurve.GetPoint(time, p1, p2, p3, p4);
                path.Add(point);
            }
            path.Add(p4);
            
            // Now we need to loop thru raw points, and get the corresponding grid point
            foreach (Vector2 point in path)
            {
                Vector2Int pointInt = new Vector2Int(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.y));
                GridCell cell = _request.View.BaseCellAtIndex(_request.View.Grid.GetFlattenedIndexForCoords(pointInt.x, pointInt.y));
                if (!cell)
                    continue;
                
                if (!_path.Contains(cell))
                {
                    _path.Add(cell);
                }
            }
        }

        #endregion LOGIC
    }
}