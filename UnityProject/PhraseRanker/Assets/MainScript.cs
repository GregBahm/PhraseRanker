using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class MainScript : MonoBehaviour
{
    public TextAsset SourceXml;
    private SignitureItem[] _items;
    public TMP_Text TextMesh;

    public float High;
    public float Low;
    public float SizeRamp;
    public Color HighColor;
    public Color MidColor;
    public Color LowColor;
    public float ColorRamp;

    private StringBuilder _stringBuilder;

	void Start ()
    {
        _items = LoadFromXml(SourceXml.text).ToArray();
        _stringBuilder = new StringBuilder();
	}
	
	void Update ()
    {
        TextMesh.text = GetText();
	}

    private string GetText()
    {
        _stringBuilder.Clear();
        foreach (SignitureItem item in _items)
        {
            string newLine = AddTextFor(item);
            _stringBuilder.AppendLine(newLine);
        }
        return _stringBuilder.ToString();
    }

    private string AddTextFor(SignitureItem item)
    {
        float rawRank = (float)item.Rank / _items.Length;
        float sizeRampedRank = Mathf.Pow(rawRank, SizeRamp);
        float size = Mathf.Lerp(Low, High, sizeRampedRank);

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

[Serializable]
public class SignitureItem
{
    public DateTime StartTime;
    public DateTime EndTime;
    public string Phrase;
    public int Rank;

    public SignitureItem(DateTime startTime, DateTime endTime, string phrase, int rank)
    {
        StartTime = startTime;
        EndTime = endTime;
        Phrase = phrase;
        Rank = rank;
    }

    public static SignitureItem FromXml(XmlElement element)
    {
        XmlNode startTimeNode = element.SelectSingleNode("StartTime");
        long startTimeTicks = Convert.ToInt64(startTimeNode.InnerText);
        DateTime startTime = new DateTime(startTimeTicks);

        XmlNode endTimeNode = element.SelectSingleNode("EndTime");
        long endTimeTicks = Convert.ToInt64(endTimeNode.InnerText);
        DateTime endTime = new DateTime(endTimeTicks);

        XmlNode rankNode = element.SelectSingleNode("Rank");
        int rank = Convert.ToInt32(rankNode.InnerText);

        XmlNode phraseNode = element.SelectSingleNode("Phrase");
        string phrase = phraseNode.InnerText;

        return new SignitureItem(startTime, endTime, phrase, rank);
    }
}
