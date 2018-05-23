using System;
using System.Collections.Generic;
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
    private readonly string _phrase;
    public int Elo { get; set; }
    public int Challenges { get; set; }

    public SignitureItem(DateTime startTime, DateTime endTime, string phrase, int elo)
    {
      _startTime = startTime;
      _endTime = endTime;
      _phrase = phrase;
      Elo = elo;
    }

    public XmlElement ToXml(XmlDocument doc)
    {
      throw new NotImplementedException();
    }

    public static SignitureItem FromXml(XmlElement element)
    {
      throw new NotImplementedException();
    }
  }
}
