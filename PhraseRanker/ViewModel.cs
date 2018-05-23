using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhraseFighter
{
  public abstract class ViewModel : INotifyPropertyChanged
  {
    protected void NotifyPropertyChanged(string property)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
    public event PropertyChangedEventHandler PropertyChanged;
  }

  public class EloFight : ViewModel
  {
    public SignitureItem LeftItem { get; }
    public SignitureItem RightItem { get; }
  }

  public class MainViewModel : ViewModel
  {
    private readonly Random _rand;
    private List<SignitureItem> _items;

    private EloFight _currentFight;
    public EloFight CurrentFight { get { return _currentFight; } }

    public MainViewModel(IEnumerable<SignitureItem> items)
    {
      _rand = new Random();
      _items = items.ToList();
    }

    public void GrantLeftWin()
    {
      ResolveFight(CurrentFight.LeftItem, CurrentFight.RightItem);
    }

    public void GrantRightWin()
    {
      ResolveFight(CurrentFight.RightItem, CurrentFight.LeftItem);
    }

    private void ResolveFight(SignitureItem winner, SignitureItem loser)
    {
      int newWinnerElo = (winner.Elo * winner.Challenges + loser.Elo + 400) / (winner.Challenges + 1);
      int newLoserElo = (loser.Elo * loser.Challenges + loser.Elo + 400) / (loser.Challenges + 1);

      winner.Elo = newWinnerElo;
      loser.Elo = newLoserElo;
      winner.Challenges += 1;
      loser.Challenges += 1;

      _items.Add(winner);
      _items.Add(loser);
      GetNewFight();
    }

    private void GetNewFight()
    {
      int minFights = _items.Min(item => item.Challenges);
      SignitureItem fighterA = _items.First(item => item.Challenges == minFights);
      _items.Remove(fighterA);
      int random = _rand.Next(0, _items.Count);
      SignitureItem challenger = _items[random];
      _items.RemoveAt(random);

    }

    void Save(string path)
    {
      throw new NotImplementedException();
    }
  }
}
