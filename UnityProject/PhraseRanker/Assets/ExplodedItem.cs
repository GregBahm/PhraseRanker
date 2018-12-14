using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class ExplodedItem : MonoBehaviour
{
    public TextMesh TextMesh;
    public MeshRenderer Renderer;
    public SignitureItem Item;
    public float RandomSeed;
    public float Size;
    public float Param;
    public float PsuedoRank;
}