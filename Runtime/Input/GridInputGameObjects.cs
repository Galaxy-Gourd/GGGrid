using System;
using GG.Camera;
using GG.Core;
using GG.Input;
using GG.Operators;
using UnityEngine;

namespace GG.Grid
{
    public class GridInputGameObjects : GridInput, ITickable
    {
        #region VARIABLES

        public TickGroup TickGroup => TickGroup.InputTransmission;
        internal OperatorPlayer Operator { get; private set; }
        internal IGameObjectGrid Grid { get; private set; }
        public Vector3 RayHitPosition => _raycaster.InputValues.GridPointerPosition;

        private readonly LayerMask _cellLayer;
        private readonly float _raycastDistance;
        private GridInputRaycaster _raycaster;

        #endregion VARIABLES
        
        
        #region INITIALIZATION

        private void Awake()
        {
            Grid = GetComponent<IGameObjectGrid>();
        }

        public override void Init(int operatorIndex = 0, bool inputActiveAtStart = true)
        {
            base.Init(operatorIndex, inputActiveAtStart);

            Operator = Modules.Get<ModuleOperators>().GetOperator(operatorIndex) as OperatorPlayer;
            _raycaster = GetComponent<GridInputRaycaster>();
            _raycaster.Init(this);
        }

        private void OnEnable()
        {
            TickRouter.Register(this);
            RegisterComponent(Grid);
        }

        private void OnDisable()
        {
            TickRouter.Unregister(this);
            RemoveComponent(Grid);
        }

        #endregion INITIALIZATION


        #region TICK

        void ITickable.Tick(float delta)
        {
            if (!_raycaster || !_inputEnabled)
                return;
            
            OnGridInput(_raycaster.InputValues);
            _raycaster.ResetValues();
        }

        #endregion TICK
    }
}