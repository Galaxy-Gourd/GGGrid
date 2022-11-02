using System;
using GG.Core;
using GG.Input;
using UnityEngine;

namespace GG.Grid
{
    /// <summary>
    /// Raycasts to a grid and sends the results to GridInputGameObjects
    /// </summary>
    public class GridInputRaycaster : MonoBehaviour, IInputReceiver<DataInputValuesPointer>, ITickable
    {
        #region VARIABLES

        public TickGroup TickGroup => TickGroup.PhysicsRaycast;
        internal DataInputValuesGrid InputValues => _inputValues;

        private GridInputGameObjects _input;
        private DataInputValuesGrid _inputValues;
        private bool _didHitGrid;
        
        #endregion VARIABLES


        #region INITIALIZATION

        internal void Init(GridInputGameObjects input)
        {
            _input = input;
            _input.Operator.Pointer.RegisterReceiver(this);
        }

        private void OnEnable()
        {
            TickRouter.Register(this);
        }

        private void OnDisable()
        {
            TickRouter.Unregister(this);
            _input.Operator.Pointer.UnregisterReceiver(this);
        }

        #endregion INITIALIZATION


        #region TICK

        void ITickable.Tick(float delta)
        {
            RaycastToGrid();
        }

        internal void ResetValues()
        {
            _inputValues.DidEnterThisFrame = false;
            _inputValues.DidExitThisFrame = false;
            _inputValues.WasPressedThisFrame = false;
            _inputValues.WasAlternatePressedThisFrame = false;
            _inputValues.WasReleasedThisFrame = false;
            _inputValues.WasAlternateReleasedThisFrame = false;
        }

        #endregion TICK


        #region POINTER

        void IInputReceiver<DataInputValuesPointer>.ReceiveInput(DataInputValuesPointer inputData, float delta)
        {
            _inputValues.IsPressed = inputData.SelectIsPressed;
            _inputValues.WasPressedThisFrame = inputData.SelectStarted;
            _inputValues.WasReleasedThisFrame = inputData.SelectReleased;
            _inputValues.WasAlternatePressedThisFrame = inputData.SelectAlternateStarted;
            _inputValues.WasAlternateReleasedThisFrame = inputData.SelectAlternateReleased;
        }

        #endregion POINTER

        
        #region RAYCAST

        private void RaycastToGrid()
        {
            if (!_input.Operator.Pointer)
                return;
            
            // Raycast to grid
            Vector2 inputPos = _input.Operator.Pointer.GetPointerPosForViewportRaycast();
            Ray ray = _input.Operator.Pointer.Camera.ViewportPointToRay(inputPos);
            if (Physics.Raycast(ray, out RaycastHit hit, _input.Grid.InputDistance, _input.Grid.GridMask))
            {
                // We need to determine which cell we hit
                GameObjectGridCell hitCell = _input.Grid.GetClosestCellFromPoint(hit.point, hit.transform.gameObject);
                _inputValues.ActiveCell = hitCell;
                
                // Hit
                _inputValues.IsInside = true;
                _inputValues.GridPointerPosition = hit.point;
                if (!_didHitGrid)
                {
                    _inputValues.DidEnterThisFrame = true;
                }
                _didHitGrid = true;
            }
            else
            {
                // No hit
                _inputValues.IsInside = false;
                if (_didHitGrid)
                {
                    _inputValues.DidExitThisFrame = true;
                }
                _didHitGrid = false;
                _inputValues.ActiveCell = null;
            }
        }

        #endregion RAYCAST
    }
}