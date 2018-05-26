using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PhraseFighter
{
    public class SignitureItem
    {
        private readonly DateTime _startTime;
        private readonly DateTime _endTime;
        public string Phrase { get; }
        public int Rank { get; set; }

        public SignitureItem(DateTime startTime, DateTime endTime, string phrase, int rank)
        {
            _startTime = startTime;
            _endTime = endTime;
            Phrase = phrase;
            Rank = rank;
        }

        public XmlElement ToXml(XmlDocument doc)
        {
            XmlElement ret = doc.CreateElement("Item");

            XmlElement startTime = doc.CreateElement("StartTime");
            startTime.InnerText = _startTime.Ticks.ToString();

            XmlElement endTime = doc.CreateElement("EndTime");
            endTime.InnerText = _endTime.Ticks.ToString();

            XmlElement rank = doc.CreateElement("Rank");
            rank.InnerText = Rank.ToString();

            XmlElement phrase = doc.CreateElement("Phrase");
            phrase.InnerText = Phrase;

            ret.AppendChild(startTime);
            ret.AppendChild(endTime);
            ret.AppendChild(rank);
            ret.AppendChild(phrase);

            return ret;
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

        public static List<SignitureItem> LoadFromRawText(string textFile)
        {
            List<SignitureItem> ret = new List<SignitureItem>();
            string[] rawLines = File.ReadAllLines(textFile);
            
            for (int i = 0; i < rawLines.Length - 1; i++)
            {
                string currentLine = rawLines[i];
                RawLine currentConverted = new RawLine(currentLine);
                string nextLine = rawLines[i + 1];
                RawLine nextConverted = new RawLine(nextLine);
                ret.Add(new SignitureItem(currentConverted.StartTime, nextConverted.StartTime, currentConverted.Phrase, 0));
            }
            return ret;
        }

        private class RawLine
        {
            public DateTime StartTime { get; }
            public string Phrase { get; }

            public RawLine(string line)
            {
                string[] split = line.Split(new string[] { "- " }, StringSplitOptions.RemoveEmptyEntries);
                string dateText = split[0];
                StartTime = Convert.ToDateTime(dateText);
                Phrase = split[1];
            }
        }
    }
}
