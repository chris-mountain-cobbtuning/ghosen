using System.ComponentModel;

namespace ghosen
{
  public class ArbIdPair : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    public uint RequestId { get; set; }
    public uint ResponseId { get; set; }

    public string RequestIdHex
    {
      get => $"0x{RequestId:X3}";
      set
      {
        if (TryParseHex(value, out var parsed))
          RequestId = parsed;
      }
    }

    public string ResponseIdHex
    {
      get => $"0x{ResponseId:X3}";
      set
      {
        if (TryParseHex(value, out var parsed))
          ResponseId = parsed;
      }
    }

    public ArbIdPair() { }

    public ArbIdPair(uint requestId, uint responseId)
    {
      RequestId = requestId;
      ResponseId = responseId;
    }

    private static bool TryParseHex(string input, out uint result)
    {
      result = 0;
      if (string.IsNullOrWhiteSpace(input))
        return false;
      input = input.Trim();
      if (input.StartsWith("0x") || input.StartsWith("0X"))
        input = input.Substring(2);
      return uint.TryParse(input, System.Globalization.NumberStyles.HexNumber, null, out result);
    }

    public override string ToString() => $"({RequestIdHex}, {ResponseIdHex})";
  }
}
