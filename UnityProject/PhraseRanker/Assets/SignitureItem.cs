using System;
using System.Xml;

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
