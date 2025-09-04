using OkamySynthW.Models;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Drawing;

namespace OkamySynthW.Models
{
    [Serializable]
    public class NoteData
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string Color { get; set; } = "#FFFFFF";
        public string Lyric { get; set; } = "a"; //default
        public int Pitch { get; set; }
        public string transcription { get; set; } = "a"; //default
        public bool ProtectedPhonemeMode { get; set; } = false;
        public bool vibratoEnabled { get; set; } = false;
        public bool vibratoCrescendo { get; set; } = false;
        public double vibratoCrescendoFinal { get; set; } = 0.2; // 0.2 de la nota
        public bool vibratoDecrescendo { get; set; } = false;
        public double vibratoDecrescendoStart { get; set; } = 0.8; // 0.8 de la nota
        public double VibratoDepth { get; set; }
        public double VibratoFrequency { get; set; }
        public double VibratoStartTime { get; set; } = 0; //inicio de la nota
        public double VibratoEndTime { get; set; } = 1;   //final de la nota
        public bool vibratoRandomness { get; set; } = false; //aleatoriedad 
        public double VibratoRandomnessAmount { get; set; } = 0.000; // MAX = 1x, 5050 = 0.500x, MIN = 0.000x
        public enum VibratoWaveType { Sine, Triangle, Square, Saw }
        public VibratoWaveType VibratoWave { get; set; } = VibratoWaveType.Sine;
        public double NoteIndex { get; set; } = 1;
        public double StartTick { get; set; }
        public double DurationTick { get; set; }
        public double EndTick { get; set; } 
        public List<SerializablePoint> PitchPointsVisual { get; set; } = new();
    }

    [Serializable]
    public class Notes
    {
        public List<NoteData> NoteList { get; set; } = new List<NoteData>(); // Lista de notas
    }
}
