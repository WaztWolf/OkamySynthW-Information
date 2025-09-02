using OkamySynthW.Models;

namespace OkamySynthW.Models
{
    public class VibratoPresetModel
    {
        public string Name { get; set; } = "Default";
        public bool VibratoCrescendo { get; set; } = false;
        public double VibratoCrescendoFinal { get; set; } = 0.2;
        public bool VibratoDecrescendo { get; set; } = false;
        public double VibratoDecrescendoStart { get; set; } = 0.8;
        public double VibratoDepth { get; set; } = 0.1;
        public double VibratoFrequency { get; set; } = 5;
        public double VibratoStartTime { get; set; } = 0;
        public double VibratoEndTime { get; set; } = 1;

        public enum WaveType { Sine, Triangle, Square, Saw }
        public WaveType VibratoWave { get; set; } = WaveType.Sine;
    }
}
