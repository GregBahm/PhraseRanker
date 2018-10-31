using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEditor;
using UnityEngine;

public class ExplodedScript : MonoBehaviour
{
    public TextAsset SourceXml;
    public GameObject TextMeshPrefab;

    [Range(0, 1)]
    public float High;
    [Range(0, 1)]
    public float Low;
    public float SizeRamp;
    public Color HighColor;
    public Color MidColor;
    public Color LowColor;
    public float ColorRamp;
    public Color BackgroundColor;

    private int _itemsCount;
    private List<ExplodedItem> _explodedItems;

    [Range(0, 1)]
    public float HeightScale;

    public float Margin;
    public float Slant;
    [Range(0, 1)]
    public float FadePower;
    [Range(0, 360)]
    public float RotationalOffset;
    public float Stagger;
    public float RampColor;
    public float ScaleColor;

    void Start ()
    {
        SignitureItem[] items = LoadFromXml(SourceXml.text).ToArray();
        items = CullItems(items);
        _itemsCount = items.Length;
        _explodedItems = CreateExplodedItems(items);
	}

    private SignitureItem[] CullItems(SignitureItem[] baseItems)
    {
        int count = baseItems.Length;
        float theshold = count * .75f;
        foreach (SignitureItem item in baseItems)
        {
            item.Rank = count - item.Rank;
        }
        return baseItems.Where(item => item.Rank < theshold).ToArray();
    }

    [MenuItem("Do/Screenshot")]
    public static void TakeScreenshot()
    {
        string outputPath = @"D:\PhraseRanker\Screenshot.png";
        ScreenCapture.CaptureScreenshot(outputPath, 4);
    }

    private List<ExplodedItem> CreateExplodedItems(SignitureItem[] items)
    {
        List<ExplodedItem> ret = new List<ExplodedItem>();
        foreach (SignitureItem item in items)
        {
            ExplodedItem explodedItem = InitializeExplodedItem(item);
            ret.Add(explodedItem);
        }
        return ret;
    }

    private ExplodedItem InitializeExplodedItem(SignitureItem item)
    {
        GameObject pivotA = new GameObject();
        pivotA.transform.parent = transform;


        GameObject pivotB = new GameObject();
        pivotB.transform.parent = pivotA.transform;

        GameObject newObj = Instantiate(TextMeshPrefab);
        newObj.transform.parent = pivotB.transform;
        ExplodedItem explodedItem = newObj.GetComponent<ExplodedItem>();
        explodedItem.Item = item;
        explodedItem.RandomSeed = UnityEngine.Random.value;
        explodedItem.TextMesh.text = GetTextFor(item);
        return explodedItem;
    }

    private string GetTextFor(SignitureItem item)
    {
        string quotesRemoved = item.Phrase.Substring(1, item.Phrase.Length - 2).Replace(".", "");
        return quotesRemoved;
    }

    void Update ()
    {
        float totalSize = EstablishSizes();
        EstablishParams(totalSize);
        foreach (ExplodedItem item in _explodedItems)
        {
            item.TextMesh.color = GetColorFor(item.Item, item.Param);
            UpdateItemPosition(item);
        }
    }

    private void EstablishParams(float totalSize)
    {
        float total = 0;
        for (int i = 0; i < _explodedItems.Count; i++)
        {
            ExplodedItem item = _explodedItems[i];
            float current = total + (item.Size / 2);
            total += item.Size;
            item.Param = current / totalSize;
        }
    }

    private float EstablishSizes()
    {
        float ret = 0;
        for (int i = 0; i < _explodedItems.Count; i++)
        {
            ExplodedItem item = _explodedItems[i];
            float rawRank = (float)item.Item.Rank / _itemsCount;
            item.Size = GetItemHeight(rawRank);
            ret += item.Size;
        }
        return ret;
    }

    private void UpdateItemPosition(ExplodedItem item)
    {
        float angle = (item.Param * 360) + RotationalOffset;
        angle = angle % 360 - 180;
        float stagger = Stagger * item.RandomSeed;
        float margin = Margin;
        if(Mathf.Abs(angle) > 90)
        {
            item.TextMesh.anchor = TextAnchor.MiddleRight;
            item.transform.parent.localPosition = new Vector3(-margin, 0, 0);
            item.transform.localPosition = new Vector3(stagger, 0, 0);
            item.transform.parent.localRotation = Quaternion.Euler(0, Slant, 0);
            angle += 180;
        }
        else
        {
            item.TextMesh.anchor = TextAnchor.MiddleRight;
            item.transform.parent.localPosition = new Vector3(margin, 0, 0);
            item.transform.localPosition = new Vector3(stagger, 0, 0);
            item.transform.parent.localRotation = Quaternion.Euler(0, 180 - Slant, 0);
        }
        item.transform.parent.parent.localRotation = Quaternion.Euler(0, 0, angle);
        item.transform.localScale = new Vector3(item.Size, item.Size, item.Size);
    }

    private float GetItemHeight(float rawRank)
    {
        float sizeRampedRank = Mathf.Pow(rawRank, SizeRamp);
        float size = Mathf.Lerp(Low, High, sizeRampedRank);
        return size;
    }
    
    private Color GetColorFor(SignitureItem item, float param)
    {
        float rawRank = (float)item.Rank / _itemsCount;
        float colorRampedRank = Mathf.Pow(rawRank, ColorRamp);
        float lowColorRamp = Mathf.Clamp01(colorRampedRank * 2);
        float highColorRamp = Mathf.Clamp01(colorRampedRank * 2 - 1);
        Color color = Color.Lerp(LowColor, MidColor, lowColorRamp);
        color = Color.Lerp(color, HighColor, highColorRamp);

        float offsetParam = (param + RotationalOffset / 360) % 1;
        float backgroundColorFade = Mathf.Abs(offsetParam - .5f) * 2;
        backgroundColorFade = Mathf.Lerp(1, backgroundColorFade, FadePower);
        color = Color.Lerp(BackgroundColor, color, backgroundColorFade);
        color = new Color(
            Mathf.Pow(color.r, RampColor) * ScaleColor, 
            Mathf.Pow(color.g, RampColor) * ScaleColor, 
            Mathf.Pow(color.b, RampColor) * ScaleColor,
            1);
        return color;
    }

    public static IEnumerable<SignitureItem> LoadFromXml(string text)
    {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(text);
        List<SignitureItem> ret = new List<SignitureItem>();
        foreach (XmlElement item in doc.DocumentElement.ChildNodes)
        {
            SignitureItem loadedItem = SignitureItem.FromXml(item);
            ret.Add(loadedItem);
        }
        return ret.OrderBy(item => item.StartTime);
    }
}
