using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class ExplodedItem : MonoBehaviour
{
    public TMP_Text TextMesh;
    public SignitureItem Item;
    public RectTransform Rect;
    public float RandomSeed;

    private void Start()
    {
        TextMesh = GetComponent<TMP_Text>();
        Rect = GetComponent<RectTransform>();
    }
}