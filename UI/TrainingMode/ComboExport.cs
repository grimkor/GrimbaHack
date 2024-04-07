using System.Collections.Generic;
using GrimbaHack.Modules.ComboTrial;
using nway.gameplay;

namespace GrimbaHack.UI.TrainingMode;

public class ComboExport
{
    public string Title;
    public List<List<ComboItem>> Combo;
    public List<uint> Inputs;
    public string Character;
    public string Dummy;
    public List<FixedPoint> PlayerPosition;
    public List<FixedPoint> DummyPosition;
    public int CharacterId;
    public int DummyId;
    public int SuperMeter;
    public TrainingOptions.MZMeterLevel MzMeter;
}
