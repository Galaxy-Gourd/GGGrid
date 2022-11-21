using UnityEngine;

namespace GG.Grid
{
    public struct DataInputValuesGrid
    {
        public GridCell ActiveCell;
        public Vector3 GridPointerPosition;
        
        public bool WasPressedThisFrame;
        public bool WasAlternatePressedThisFrame;
        public bool IsPressed;
        public bool WasReleasedThisFrame;
        public bool WasAlternateReleasedThisFrame;
        public bool DidEnterThisFrame;
        public bool IsInside;
        public bool DidExitThisFrame;
    }
}