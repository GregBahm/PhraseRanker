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

    public float High;
    public float Low;
    public float SizeRamp;
    public Color HighColor;
    public Color MidColor;
    public Color LowColor;
    public float ColorRamp;

    private int _itemsCount;
    private List<ExplodedItem> _explodedItems;

    [Range(0, 1)]
    public float HeightScale;

    public float Margin;
    public float Slant;

    void Start ()
    {
        SignitureItem[] items = LoadFromXml(SourceXml.text).ToArray();
        _itemsCount = items.Length;
        _explodedItems = CreateExplodedItems(items);
	}

    [MenuItem("Do/Screenshot")]
    public static void TakeScreenshot()
    {
        string outputPath = @"C:\Users\Lisa\Documents\PhraseRanker\Screenshot.png";
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
        GameObject newObj = Instantiate(TextMeshPrefab);
        newObj.transform.parent = transform;
        ExplodedItem explodedItem = newObj.GetComponent<ExplodedItem>();
        explodedItem.Item = item;
        explodedItem.RandomSeed = UnityEngine.Random.value;
        return explodedItem;
    }

    void Update ()
    {
        float offset = 0;
        for (int i = 0; i < _explodedItems.Count; i++)
        {
            ExplodedItem item = _explodedItems[i];
            offset += UpdateItemText(item, offset);
        }
        float offsetTotal = offset;
        offset = 0;
        for (int i = 0; i < _explodedItems.Count; i++)
        {
            ExplodedItem item = _explodedItems[i];
            offset += UpdateItemPosition(item, offset, offsetTotal);
        }
    }

    private float UpdateItemPosition(ExplodedItem item, float offset, float offsetTotal)
    {
        float rawRank = (float)item.Item.Rank / _itemsCount;
        float height = GetItemHeight(rawRank);
        float param = (offset + height / 2) / offsetTotal;
        float angle = (param * 360) - 180;
        float margin = Margin;
        if(Mathf.Abs(angle) > 90)
        {
            item.TextMesh.alignment = TMPro.TextAlignmentOptions.Right;
            angle += 180;
        }
        else
        {
            item.TextMesh.alignment = TMPro.TextAlignmentOptions.Left;
        }
        item.transform.localRotation = Quaternion.Euler(0, 0, angle);
        item.Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, margin);
        return height;
    }

    private float UpdateItemText(ExplodedItem item, float currentOffset)
    {
        float rawRank = (float)item.Item.Rank / _itemsCount;
        float height = GetItemHeight(rawRank);
        
        item.TextMesh.text = GetTextFor(item.Item, rawRank);
        return height;
    }

    private float GetItemHeight(float rawRank)
    {
        float sizeRampedRank = Mathf.Pow(rawRank, SizeRamp);
        float size = Mathf.Lerp(Low, High, sizeRampedRank);
        return size;
    }

    private string GetTextFor(SignitureItem item, float rawRank)
    {
        float size = GetItemHeight(rawRank);

        float colorRampedRank = Mathf.Pow(rawRank, ColorRamp);
        float lowColorRamp = Mathf.Clamp01(colorRampedRank * 2);
        float highColorRamp = Mathf.Clamp01(colorRampedRank * 2 - 1);
        Color color = Color.Lerp(LowColor, MidColor, lowColorRamp);
        color = Color.Lerp(color, HighColor, highColorRamp);
        string colorString = GetHexString(color);

        string ret = "<color=" + colorString + "><size=" + size + ">" + item.Phrase + "</size></color>";
        return ret;
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

    private static string ComponentToHex(byte c)
    {
        string hex = c.ToString();
        return hex.Length == 1 ? "0" + hex : hex;
    }

    private static string GetHexString(Color color)
    {
        byte r = (byte)(byte.MaxValue * color.r);
        byte g = (byte)(byte.MaxValue * color.g);
        byte b = (byte)(byte.MaxValue * color.b);
        return "#" + r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
    }

}
