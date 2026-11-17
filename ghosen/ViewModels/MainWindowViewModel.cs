using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace ghosen.ViewModels
{
  public class MainWindowViewModel : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;
    public ObservableCollection<string> Strings { get; set; }
    public ObservableCollection<IPluginV1> Plugins { get; set; }
    public ObservableCollection<ArbIdPair> ArbIdPairs { get; set; }

    public MainWindowViewModel()
    {
      Strings = new ObservableCollection<string>();
      Plugins = new ObservableCollection<IPluginV1>();
      ArbIdPairs = new ObservableCollection<ArbIdPair>
      {
        new ArbIdPair(0x7E0, 0x7E8),
        new ArbIdPair(0x7E1, 0x7E9),
        new ArbIdPair(0x7DF, 0x7E8),
      };
      var pl = new PluginLoader(".");
      pl.Reload();
      foreach (var item in pl.Plugins)
      {
        Plugins.Add(item);
      }
    }

    public Plugins.ArbIdFilter BuildFilter()
    {
      var ids = new List<uint>();
      foreach (var pair in ArbIdPairs)
      {
        if (!ids.Contains(pair.RequestId))
          ids.Add(pair.RequestId);
        if (!ids.Contains(pair.ResponseId))
          ids.Add(pair.ResponseId);
      }
      return new Plugins.ArbIdFilter(ids);
    }
  }
}
