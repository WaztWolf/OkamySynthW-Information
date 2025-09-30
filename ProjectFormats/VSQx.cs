using OkamySynthW.ExtraProperties;
using System;
using System.Threading;
using System.Xml;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Media;
using Avalonia;
using OkamySynthW.MapperEngine;
using OkamySynthW.Utils;
using OkamySynthW.EditorTools;
using System.Collections.Generic;
using Avalonia.Input;
using Avalonia.Layout;
using System.Linq;
using OkamySynthW.Models;
using OkamySynthW.Errors;

namespace OkamySynthW.ProjectFormats
{
    public partial class VSQX
    {
        private const double NoteHeight = 20;
        private const double BeatWidth = 40;
        private TransportControls transportControls;
        public void LoadVSQx(string filePath, LoadingProjectWindow loadingWindow, ProjectData projectData, MainWindow mainWindow, Dictionary<Button, List<Control>> noteElements)
        {
            try
            {
                Dispatcher.UIThread.InvokeAsync(() =>
            {
                loadingWindow.SetStatus("Cargando archivo...");
                loadingWindow.SetProgress(0);
            });
            Console.WriteLine("Cargando Archivo");
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);
            Console.WriteLine(filePath);

            MappingEngine MappingEngine = new MappingEngine();
            string configPath = "config.json";
            if (System.IO.File.Exists(configPath))
            {
                var json = System.IO.File.ReadAllText(configPath);
                using JsonDocument jsondoc = JsonDocument.Parse(json);
                var root = jsondoc.RootElement;
                if (root.TryGetProperty("PhonemeMappingPath", out JsonElement phonemeMappingPath))
                {
                    string mappingPath = phonemeMappingPath.GetString() ?? "";
                    MappingEngine.LoadSwmap(mappingPath);

                }
                else
                {
                    Console.WriteLine("There is no Mapping Path set in the config.json file.");
                }
            }

            JapaneseTextConverter converter = new JapaneseTextConverter();
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                loadingWindow.SetStatus("Cargando archivo VSQx...");
                loadingWindow.SetProgress(5);
                Thread.Sleep(100);
            });
            try
            {
                doc.Load(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar el archivo VSQx: {ex.Message}");
                return;
            }

            XmlNamespaceManager nsManager = new XmlNamespaceManager(doc.NameTable);
            nsManager.AddNamespace("v", "http://www.yamaha.co.jp/vocaloid/schema/vsq4/");

            var tempoNode = doc.SelectSingleNode("//v:masterTrack/v:tempo/v:v", nsManager);
            if (tempoNode != null && int.TryParse(tempoNode.InnerText, out int tempoValue))
            {
                Console.WriteLine($"Tempo encontrado: {tempoValue}");
                projectData.Tempo = tempoValue / 100.0;
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    mainWindow.FindControl<TextBox>("TempoTextBox").Text = projectData.Tempo.ToString();
                });

            }
            else
            {
                Console.WriteLine("Tempo no encontrado.");
            }

            var timeSigNode = doc.SelectSingleNode("//v:masterTrack/v:timeSig/v:timeSig", nsManager);
            if (timeSigNode != null)
            {
                var numeNode = timeSigNode["nu"];
                var denomiNode = timeSigNode["de"];
                if (numeNode != null && denomiNode != null)
                {
                    if (int.TryParse(numeNode.InnerText, out int numerator))
                        projectData.Numerator = numerator;
                    if (int.TryParse(denomiNode.InnerText, out int denominator))
                        projectData.Denominator = denominator;
                    Console.WriteLine($"Compas encontrado: {numerator}/{denominator}");
                }
            }
            else
            {
                projectData.Numerator = 4;
                projectData.Denominator = 4;
                Console.WriteLine("Compas no encontrado, usando 4/4 por defecto.");
            }

            Dispatcher.UIThread.InvokeAsync(() =>
            {
                loadingWindow.SetStatus("Leyendo Tempo");
                Thread.Sleep(100);
                loadingWindow.SetProgress(10);
            });
            var ppqNode = doc.SelectSingleNode("//v:masterTrack/v:resolution", nsManager);
            var ppq = ppqNode != null && int.TryParse(ppqNode.InnerText, out int ppqValue) ? ppqValue : 480;
            Console.WriteLine($"PPQ encontrado: {ppq}");

            var noteNodes = doc.SelectNodes("//v:vsTrack/v:vsPart/v:note", nsManager);
            if (noteNodes == null)
            {
                Console.WriteLine("No se encontraron notas en el archivo VSQx.");
                return;
            }
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                loadingWindow.SetStatus("Leyendo Notas");
                Thread.Sleep(100);
                mainWindow.FindControl<Canvas>("PianoRollCanvas").Children.Clear();
                noteElements.Clear();
                projectData.Notes.Clear();
                loadingWindow.SetProgress(15);
            });

            double lastNoteTick = 0;
            foreach (XmlNode noteNode in noteNodes)
            {
                int t = int.Parse(noteNode["t"].InnerText);
                int dur = int.Parse(noteNode["dur"].InnerText);
                int endTick = t + dur;
                if (endTick > lastNoteTick)
                    lastNoteTick = endTick;
            }
            double durationInBeats = lastNoteTick / (double)ppq;
            projectData.Beats = durationInBeats;
            projectData.Beats = durationInBeats; 
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                mainWindow.DrawGrid(projectData);
            });
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                loadingWindow.SetStatus("Calculando duracion total");
                loadingWindow.SetProgress(20);
                Thread.Sleep(100);
            });
            Console.WriteLine($"Duracion total en ticks: {lastNoteTick} ticks, que equivale a {durationInBeats} beats.");
            var ccNodes = doc.SelectNodes("//v:vsTrack/v:vsPart/v:cc", nsManager);
            foreach (XmlNode noteNode in noteNodes)
            {
                int notesCount = noteNodes.Count;
                int count = 0;
                int p = int.Parse(noteNode["t"].InnerText); // posicion de la nota en ticks, la inicial
                Console.WriteLine($"Nota encontrada en ticks: {p}"); // imprime la posicion de la nota en ticks
                int dur = int.Parse(noteNode["dur"].InnerText); // duracion de la nota en ticks
                Console.WriteLine($"Duracion de la nota: {dur} ticks"); // imprime la duracion de la nota en ticks
                int n = int.Parse(noteNode["n"].InnerText); // pitch de la nota
                Console.WriteLine($"Pitch de la nota: {n}"); // imprime el pitch de la nota
                string lyric = noteNode["y"]?.InnerText ?? "a"; // lyrics de la nota
                Console.WriteLine($"Lyric de la nota: {lyric}"); // imprime el lyric de la nota
                string phoneme = noteNode["p"]?.InnerText ?? ""; // phonemes de la nota
                Console.WriteLine($"Fonema de la nota: {phoneme}, actualizando al mapeo");// imprime el fonema de la nota
                string newPhnm = MappingEngine.Transcribe(lyric); // actualiza los fonemas
                bool protectedPhoneme = false; // proteccion del fonema inicial, falso
                string? lockedMode = noteNode["p"]?.Attributes?["lock"]?.Value; // consigue el lock
                string actualPhonemes = "";
                if (lockedMode == "1") // si lock=1(proteccion del fonema activado)
                {
                    protectedPhoneme = true; // se activa la proteccion del fonema
                    actualPhonemes = phoneme;
                } else {
                    protectedPhoneme = false;
                    actualPhonemes = newPhnm;
                }
                // Deteccion de texto en los diferentes lenguajes japoneses:
                // Hiragana: U+3040 a U+309F
                // Katakana: U+30A0 a U+30FF
                // Kanji (CJK Unified Ideographs): U+4E00 a U+9FBF
                // Katakana Extension: U+31F0 a U+31FF
                // Simbolos japoneses adicionales: U+3000 a U+303F (puede incluir signos de puntuacion japoneses)
                // detectar si el texto esta en japones o no

                int noteStartTick = p;
                int noteEndTick = p + dur;

                foreach (char c in lyric)
                {
                    if (c >= 0x3040 && c <= 0x309F || // Hiragana
                         c >= 0x30A0 && c <= 0x30FF || // Katakana
                         c >= 0x4E00 && c <= 0x9FBF || // Kanji
                         c >= 0x31F0 && c <= 0x31FF || // Katakana Extension
                         c >= 0x3000 && c <= 0x303F)   // Simbolos japoneses adicionales
                    {
                        lyric = converter.ConvertJapaneseTextToRomajiAsync(lyric).GetAwaiter().GetResult(); // convierte a romaji
                        // rompe el bucle
                        break;
                    }
                }

                double x = mainWindow.TickToX(p); // inicio de la nota en pixeles
                double length = mainWindow.TicksToWidth(dur); // ancho de la nota en pixels
                double y = mainWindow.GetYFromPitch(n);
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    loadingWindow.SetStatus($"Extrayendo informacion de nota: {noteNode}");
                    Thread.Sleep(100);
                });

                var note = new NoteData
                {
                    X = x,
                    Y = y,
                    Width = length,
                    StartTick = noteStartTick,
                    DurationTick = dur,
                    EndTick = noteEndTick,
                    Height = NoteHeight,
                    Color = "#DD0000",
                    Lyric = lyric,
                    transcription = actualPhonemes,
                    ProtectedPhonemeMode = protectedPhoneme,
                    Pitch = n,
                    NoteIndex = projectData.Notes.Count,
                };

                bool hasPitchAtStart = false;
                bool hasPitchAtEnd = false;

                double? pitchBefore = null;
                double? firstPitchInRange = null;
                double? lastPitchInRange = null;

                foreach (XmlNode ccNode in ccNodes)
                {
                    XmlNode? vNode = ccNode.SelectSingleNode("v[@id='P']");
                    if (vNode == null) continue;

                    int ccTick = int.Parse(ccNode["t"].InnerText);
                    Console.WriteLine($"CC Tick: {ccTick}"); // imprime el tick del CC
                    double ccPitch = double.Parse(ccNode["v"].InnerText); // en milis de semitonos
                    ccPitch = mainWindow.DenormalizeToMillicents(ccPitch) + n;
                    projectData.PitchPoints.Add(new PitchPoint(ccTick, ccPitch));
                }

                Dispatcher.UIThread.InvokeAsync(() => {
                    var noteButton = new Button
                    {
                        Width = note.Width,
                        Height = note.Height,
                        Background = new SolidColorBrush(Color.Parse(note.Color)),
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Tag = note
                    };

                    var lyricLabel = new TextBlock
                    {
                        Text = $"{note.Lyric} [{note.transcription}]",
                        Width = note.Width,
                        Height = NoteHeight,
                        TextAlignment = TextAlignment.Left,
                        Foreground = Brushes.Black,
                        VerticalAlignment = VerticalAlignment.Top
                    };

                    var resizeHandle = new Border
                    {
                        Width = 5,
                        Height = NoteHeight,
                        Background = Brushes.Gray,
                        Cursor = new Cursor(StandardCursorType.SizeWestEast)
                    };

                    mainWindow.ConfigureResizeHandle(resizeHandle, noteButton, lyricLabel);

                    noteButton.Click += (s, args) =>
                    {
                        mainWindow.EditNoteLyric(noteButton, lyricLabel);
                    };

                    var contextMenu = new ContextMenu();
                    var menuItem = new MenuItem { Header = "Note Property" };
                    menuItem.Click += (_, __) =>
                    {
                        Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            PitchRenderer pitchRenderer = new PitchRenderer(mainWindow.FindControl<Canvas>("PianoRollCanvas"), NoteHeight, projectData, BeatWidth, mainWindow.FindControl<ScrollViewer>("NoteScrollViewer"));
                            if (noteButton.Tag is NoteData data)
                            {
                                var notePropertyWindow = new NotePropertyWindow(data);
                                notePropertyWindow.NoteUpdated += updatedNote =>
                                {
                                    if (noteElements.TryGetValue(noteButton, out var related))
                                    {
                                        var label = related.OfType<TextBlock>().FirstOrDefault();
                                        if (label != null)
                                        {
                                            label.Text = $"{updatedNote.Lyric} [{updatedNote.transcription}]";
                                        }
                                    }
                                    pitchRenderer.UpdateCurve(mainWindow.FindControl<Canvas>("PianoRollCanvas"));
                                    noteButton.Tag = updatedNote;
                                };
                                notePropertyWindow.Show();
                            }
                        });
                    };

                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        loadingWindow.SetStatus("Configurando nota");
                        Thread.Sleep(100);
                    });

                    ButtonExtras.SetOriginalColor(noteButton, "#DD0000");

                    contextMenu.Items.Add(menuItem);
                    noteButton.ContextMenu = contextMenu;

                    Canvas.SetLeft(noteButton, note.X);
                    Canvas.SetTop(noteButton, note.Y);
                    Canvas.SetLeft(lyricLabel, note.X);
                    Canvas.SetTop(lyricLabel, note.Y + NoteHeight);
                    Canvas.SetLeft(resizeHandle, note.X + note.Width - resizeHandle.Width);
                    Canvas.SetTop(resizeHandle, note.Y);

                    mainWindow.FindControl<Canvas>("PianoRollCanvas").Children.Add(noteButton);
                    mainWindow.FindControl<Canvas>("PianoRollCanvas").Children.Add(lyricLabel);
                    mainWindow.FindControl<Canvas>("PianoRollCanvas").Children.Add(resizeHandle);

                    noteElements[noteButton] = new List<Control> { lyricLabel, resizeHandle };
                    projectData.Notes.Add(note);
                    count = count + 1;
                    loadingWindow.SetProgress(count/80);
                });
            }
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                loadingWindow.SetStatus("Reindexando notas");
                Thread.Sleep(100);
                PitchRenderer pitchRenderer = new PitchRenderer(mainWindow.FindControl<Canvas>("PianoRollCanvas"), NoteHeight, projectData, BeatWidth, mainWindow.FindControl<ScrollViewer>("NoteScrollViewer"));
                pitchRenderer.UpdateCurve(mainWindow.FindControl<Canvas>("PianoRollCanvas"));
            });
            
            mainWindow.ResolveOverlappingNotes();            
                Dispatcher.UIThread.InvokeAsync(() =>
            {
                transportControls = new TransportControls(
                    mainWindow.FindControl<Canvas>("PianoRollCanvas"), 
                    mainWindow.FindControl<Canvas>("timeBarCanvas"), 
                    mainWindow.FindControl<ScrollViewer>("NoteScrollViewer")
                );
                
                var tempoTextBox = mainWindow.FindControl<TextBox>("TempoTextBox");
                if (tempoTextBox != null && double.TryParse(tempoTextBox.Text, out double tempo))
                {
                    transportControls.SetTempo(tempo);
                }
                
                Console.WriteLine("Transport controls inicializados despues de cargar VSQx");
            });
            
            Console.WriteLine($"VSQx cargado desde {filePath}");
            }
            catch (Exception e)
            {
               Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var errorWindow = new ErrorWindow("Error", e.Message);
                    errorWindow.ShowDialog(mainWindow); 
                });
            }
        }

    }
}
