using UnityEngine;
using UnityEngine.EventSystems;

namespace GG.Grid
{
    /// <summary>
    /// Receives UI events and sends them to GridInputUI
    /// </summary>
    public class GridInputUIEventReceiver : MonoBehaviour, 
                                            IPointerEnterHandler, 
                                            IPointerExitHandler, 
                                            IPointerDownHandler, 
                                            IPointerUpHandler, 
                                            IPointerMoveHandler
    {
        #region VARIABLES

        private GridInputUI _input;

        #endregion VARIABLES
        
        
        #region INITIALIZATION

        internal void Init(GridInputUI input)
        {
            _input = input;
        }

        #endregion INITIALIZATION
        
        
        #region EVENTS

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            _input.OnPointerEnter(eventData);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            _input.OnPointerExit(eventData);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            _input.OnPointerDown(eventData);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            _input.OnPointerUp(eventData);
        }

        void IPointerMoveHandler.OnPointerMove(PointerEventData eventData)
        {
            _input.OnPointerMove(eventData);
        }
        
        #endregion EVENTS
    }
}