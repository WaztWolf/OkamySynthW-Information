using OkamySynthW.Models;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Drawing;

namespace OkamySynthW.Models
{
    [Serializable]
    [XmlRoot("ProjectData")]
    public class ProjectData
    {
        [XmlElement("projectProperties")]
        public ProjectProperties FakeProperties
        {
            get => new ProjectProperties { Tempo = this.Tempo, PPQ = this.PPQ, Singer = this.Singer, Beats = this.Beats, BeatsSubdivision = this.BeatsSubdivision, Numerator = this.Numerator, Denominator = this.Denominator, PitchPoints = this.PitchPoints, GenderPoints = this.GenderPoints, GrowlPoints = this.GrowlPoints, BreathinessPoints = this.BreathinessPoints };
            set
            {
                this.Tempo = value.Tempo;
                this.Singer = value.Singer;
                this.Beats = value.Beats;
                this.BeatsSubdivision = value.BeatsSubdivision;
                this.Denominator = value.Denominator;
                this.Numerator = value.Numerator;
                this.PPQ = value.PPQ;
                this.PitchPoints = value.PitchPoints ?? new List<PitchPoint>();
                this.GenderPoints = value.GenderPoints ?? new List<PitchPoint>();
                this.GrowlPoints = value.GrowlPoints ?? new List<PitchPoint>();
                this.BreathinessPoints = value.BreathinessPoints ?? new List<PitchPoint>();
            }
        }

        [XmlIgnore]
        public double Tempo { get; set; } = 120;
        [XmlIgnore]
        public string Singer { get; set; } = "";
        [XmlIgnore]
        public double Beats { get; set; } = 64;
        [XmlIgnore]
        public double BeatsSubdivision { get; set; } = 4;
        [XmlIgnore]
        public double Numerator { get; set; } = 4;// Ej: 4
        [XmlIgnore]
        public double Denominator { get; set; } = 4;
        [XmlIgnore]
        public double PPQ { get; set; } = 480;
        [XmlArray("PitchPoints")]
        [XmlArrayItem("PitchPointsData")]
        public List<PitchPoint> PitchPoints { get; set; } = new();
        [XmlArray("GenderPoints")]
        [XmlArrayItem("GenderPointsData")]
        public List<PitchPoint> GenderPoints { get; set; } = new();
        [XmlArray("GrowlPoints")]
        [XmlArrayItem("GrowlPointsData")]
        public List<PitchPoint> GrowlPoints { get; set; } = new();
        [XmlArray("BreathinessPoints")]
        [XmlArrayItem("BreathinessPointsData")]
        public List<PitchPoint> BreathinessPoints { get; set; } = new();
        [XmlArray("Notes")]
        [XmlArrayItem("NoteData")]
        public List<NoteData> Notes { get; set; } = new();
    }
    [Serializable]
    public class ProjectProperties
    {
        [XmlElement("Tempo")]
        public double Tempo { get; set; } = 120;

        [XmlElement("Singer")]
        public string Singer { get; set; } = "";

        [XmlElement("Beats")]
        public double Beats { get; set; } = 64;

        [XmlElement("BeatsSubdivision")]
        public double BeatsSubdivision { get; set; } = 4;

        [XmlElement("Denominator")]
        public double Denominator { get; set; } = 4;

        [XmlElement("Numerator")]
        public double Numerator { get; set; } = 4;

        [XmlElement("PPQ")]
        public double PPQ { get; set; } = 480;

        [XmlElement("PitchPoints")]
        public List<PitchPoint> PitchPoints { get; set; } = new();

        [XmlElement("GenderPoints")]
        public List<PitchPoint> GenderPoints { get; set; } = new();

        [XmlElement("GrowlPoints")]
        public List<PitchPoint> GrowlPoints { get; set; } = new();

        [XmlElement("BreathinessPoints")]
        public List<PitchPoint> BreathinessPoints { get; set; } = new();
    }

}
