using UnityEngine;

public class GraphicIgnoreRaycast : MonoBehaviour, ICanvasRaycastFilter
{
    public bool allowRaycast;

    public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        return allowRaycast;
    }
}
