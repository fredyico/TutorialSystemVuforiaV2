using UnityEngine;

public class Button3D : MonoBehaviour
{
    public delegate void Button3DEventHandler(Button3D button);
    public event Button3DEventHandler OnClick;

    void OnMouseDown()
    {
        OnClick?.Invoke(this);
    }
}
