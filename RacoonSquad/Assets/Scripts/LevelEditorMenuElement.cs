using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelEditorMenuElement : MonoBehaviour, IPointerEnterHandler
{
    public LevelEditor editor;
    public System.Action onHovered;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (onHovered.GetInvocationList().Length > 0) {
            onHovered.Invoke();
        }
    }
}
