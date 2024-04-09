using System.Collections.Generic;
using GrimbaHack.Modules.ComboTrial.UI;
using nway.gameplay;

namespace GrimbaHack.UI.TrainingMode;

public class ComboExport
{
    public string Title;
    public List<List<ComboItem>> Combo = new();
    public List<int> Inputs = new();
    public string Character;
    public string Dummy;
    public List<FixedPoint> PlayerPosition = new();
    public List<FixedPoint> DummyPosition = new();
    public int CharacterId;
    public int DummyId;
    public int SuperMeter;
    public TrainingOptions.MZMeterLevel MzMeter;
}
public class ComboExportOld
{
    public string Title;
    public List<List<ComboItemOld>> Combo;
    public List<int> Inputs;
    public string Character;
    public string Dummy;
    public List<FixedPoint> PlayerPosition;
    public List<FixedPoint> DummyPosition;
    public int CharacterId;
    public int DummyId;
    public int SuperMeter;
    public TrainingOptions.MZMeterLevel MzMeter;
}
