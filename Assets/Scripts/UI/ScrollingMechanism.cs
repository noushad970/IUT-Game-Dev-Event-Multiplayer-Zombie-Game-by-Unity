using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ScrollingMechanism : MonoBehaviour
{
    public Transform parent;

    public RectTransform rect;

    private const float minY = -20f;

    private void Awake()
    {
    }

    private void Update()
    {
        float maxY = 60f * parent.childCount;

        Vector2 pos = rect.anchoredPosition;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        rect.anchoredPosition = pos;
    }
}