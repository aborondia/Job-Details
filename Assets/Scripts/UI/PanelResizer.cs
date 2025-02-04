using UnityEngine;
using UnityEngine.UIElements;

public class PanelResizer : MonoBehaviour
{
    [SerializeField] private PanelSettings panelSettings;
    [SerializeField] private Vector2 defaultResolution = new Vector2(1194, 834);

    private void Start()
    {
        AdjustResolution((int)this.defaultResolution.x, (int)this.defaultResolution.y);
    }

    private void AdjustResolution(int width, int height)
    {
        panelSettings.referenceResolution = new Vector2Int(width, height);

        Debug.Log($"Resolution adjusted to: {panelSettings.referenceResolution}");
    }
}
