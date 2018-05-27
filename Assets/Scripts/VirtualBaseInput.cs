using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualBaseInput : BaseInput
{
    public RectTransform m_fakeMousePosition;

    public override Vector2 mousePosition
    {
        get { return m_fakeMousePosition.anchoredPosition; }
    }
}