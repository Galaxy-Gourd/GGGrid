using System;
using GG.Core;
using GG.Input;
using GG.Operators;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GG.Grid
{
    public class GridInputUI : GridInput, ITickable
    {
        #region VARIABLES

        [Header("UI Input")]
        [SerializeField] private GridInputUIEventReceiver _eventReceiver;

        public TickGroup TickGroup => TickGroup.InputTransmission;
        public DataInputValuesGrid InputValues => _inputValues;
        public UnityEngine.Camera UICamera => _camera;

        private UISimulatedPointer _pointer;
        private UIScalableGrid _grid;
        private Canvas _overlay;
        private UnityEngine.Camera _camera;
        private DataInputValuesGrid _inputValues;
        private bool _active;

        #endregion VARIABLES
        
        
        #region INITIALIZATION

        private void Awake()
        {
            _grid = GetComponent<UIScalableGrid>();
            _overlay = GetComponentInParent<Canvas>();
        }

        public override void Init(int operatorIndex = 0, bool inputActiveAtStart = true)
        {
            base.Init(operatorIndex, inputActiveAtStart);

            _pointer = Modules.Get<ModuleInput>().GetPointerForOperator(operatorIndex);
            _camera = (Modules.Get<ModuleOperators>().GetOperator(operatorIndex) as OperatorPlayer).GameCamera.UICamera;
            _eventReceiver.Init(this);
        }

        private void OnEnable()
        {
            _active = true;
            TickRouter.Register(this);
            RegisterComponent(_grid);
        }

        private void OnDisable()
        {
            _active = false;
            TickRouter.Unregister(this);
            RemoveComponent(_grid);
        }

        public void SetActiveManual(bool b)
        {
            _active = b;
        }

        #endregion INITIALIZATION


        #region TICK

        void ITickable.Tick(float delta)
        {
            if (!_active)
                return;
            
            OnGridInput(_inputValues);
            
            _inputValues.DidEnterThisFrame = false;
            _inputValues.DidExitThisFrame = false;
            _inputValues.WasPressedThisFrame = false;
            _inputValues.WasAlternatePressedThisFrame = false;
            _inputValues.WasReleasedThisFrame = false;
            _inputValues.WasAlternateReleasedThisFrame = false;
        }

        #endregion TICK
 

        #region POINTER

        public Tuple<UIScalableGridCell, float> GetCellClosestToPosition(Vector3 targetPosition)//, bool useRelativePosition = true)
        {
            // We use screen space to measure distance
            Vector3 sourceSS = UICamera.WorldToScreenPoint(targetPosition);

            float closestDist = float.MaxValue;
            UIScalableGridCell closestCell = _grid.CellAtIndex(0);
            foreach (UIScalableGridCell cell in _grid.CellViews)
            {
                //
                // Vector2 adjustedTarget = useRelativePosition ?
                //     RectTransformUtility.CalculateRelativeRectTransformBounds(_overlay.transform, cell.transform).center :
                //     cell.CellRect.position;

                Vector3 targetSS = UICamera.WorldToScreenPoint(cell.CellRect.position);
                float distance = Vector3.Distance(sourceSS, targetSS);
                if (distance < closestDist)
                {
                    closestDist = distance;
                    closestCell = cell;
                }
            }

            return new Tuple<UIScalableGridCell, float>(closestCell, closestDist);
        }

        /// <summary>
        /// Returns the raw position of the pointer (including off-grid position)
        /// </summary>
        public Vector3 GetPointerPositionRaw()
        {
            return _pointer.Position;
        }

        public Vector3 GetPointerPositionScreen()
        {
            return _pointer.GetPointerPosForScreen();
        }

        /// <summary>
        /// Manually refreshes the active pointer position through the input struct
        /// </summary>
        public void RefreshPointerPositionManual()
        {
            ProliferateGridPointerPosition(GetPointerPositionRaw());
        }

        #endregion POINTER


        #region EVENTS

        internal void OnPointerEnter(PointerEventData eventData)
        {
            _inputValues.DidEnterThisFrame = true;
            _inputValues.IsInside = true;
        }

        internal void OnPointerExit(PointerEventData eventData)
        {
            _inputValues.DidExitThisFrame = true;
            _inputValues.IsInside = false;
            _inputValues.ActiveCell = null;
        }

        internal void OnPointerDown(PointerEventData eventData)
        {
            _inputValues.WasPressedThisFrame = eventData.button == PointerEventData.InputButton.Left;
            _inputValues.WasAlternatePressedThisFrame = eventData.button == PointerEventData.InputButton.Right;
            _inputValues.IsPressed = true;
        }

        internal void OnPointerUp(PointerEventData eventData)
        {
            _inputValues.WasReleasedThisFrame = eventData.button == PointerEventData.InputButton.Left;
            _inputValues.WasAlternateReleasedThisFrame = eventData.button == PointerEventData.InputButton.Right;
            _inputValues.IsPressed = false;
        }
        
        internal void OnPointerMove(PointerEventData eventData)
        {
            if (!_camera)
                return;
            
            ProliferateGridPointerPosition(GetAdjustedPP(eventData.position));
        }

        #endregion EVENTS


        #region UTILITY

        private void ProliferateGridPointerPosition(Vector3 position)
        {
            _inputValues.GridPointerPosition = position;
            Tuple<UIScalableGridCell, float> closestCell = 
                GetCellClosestToPosition(_inputValues.GridPointerPosition);
            _inputValues.ActiveCell = closestCell.Item1;
        }

        private Vector3 GetAdjustedPP(Vector2 position)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                _overlay.GetComponent<RectTransform>(), 
                position, 
                _camera, 
                out Vector3 pp);
            return pp;
        }

        #endregion UTILITY
    }
}