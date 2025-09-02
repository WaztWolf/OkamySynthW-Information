using System.Collections.Generic;
using OkamySynthW.Models;

namespace OkamySynthW.Presets.VibratoPresets
{
    public static class DefaultVibratos
    {
        public static readonly VibratoPresetModel Default = new VibratoPresetModel
        {
            Name = "Default",
            VibratoCrescendo = false,
            VibratoCrescendoFinal = 0,
            VibratoDecrescendo = false,
            VibratoDecrescendoStart = 0,
            VibratoDepth = 0.5,
            VibratoFrequency = 5,
            VibratoStartTime = 0,
            VibratoEndTime = 1,
            VibratoWave = VibratoPresetModel.WaveType.Sine
        };
        public static readonly VibratoPresetModel SoftModel = new VibratoPresetModel
        {
            Name = "Soft",
            VibratoCrescendo = true,
            VibratoCrescendoFinal = 0.2,
            VibratoDecrescendo = false,
            VibratoDecrescendoStart = 0.8,
            VibratoDepth = 0.1,
            VibratoFrequency = 5,
            VibratoStartTime = 0,
            VibratoEndTime = 1,
            VibratoWave = VibratoPresetModel.WaveType.Sine
        };

        public static readonly VibratoPresetModel StrongModel = new VibratoPresetModel
        {
            Name = "Strong",
            VibratoCrescendo = true,
            VibratoCrescendoFinal = 0.6,
            VibratoDecrescendo = true,
            VibratoDecrescendoStart = 0.5,
            VibratoDepth = 0.5,
            VibratoFrequency = 8,
            VibratoStartTime = 0,
            VibratoEndTime = 1,
            VibratoWave = VibratoPresetModel.WaveType.Triangle
        };

        public static List<VibratoPresetModel> GetAll() => new List<VibratoPresetModel> { SoftModel, StrongModel };
    }
}
