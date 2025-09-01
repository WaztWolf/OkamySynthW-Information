# Okamy Voicebank(DB)
Information about the structure of an Okamy Voicebank.

---

## 1. General
An Okamy DB contains all the information on phonetic segmentation of audios, transcriptions, main DB information (name, language, version), and other resources such as Voice Colors.

---

## 2. Folder Structure
```
voicebank-root/
├─ ovoice.cfg
├─ dictionary.dict
├─ phmn_data.csv
├─ samples/
│  ├─ ejemplo.wav
│  ├─ ejemplo.otrans
│  └─ ejemplo.osegment
├─ phonetics_folder/
│  ├─ phonetic_a.csv
│  ├─ phonetic_e.csv
│  └─ ...
├─ AVE.oe (opcional)
├─ recursos (icon / portrait)
└─ otros (extendido)
```

---

## 3. Main Files

### 3.1 `ovoice.cfg`
Key=value pair file.  
Keys: `name`, `version`, `dictionary-path`, `icon-sample`, `img-sample`, `samples`, `phmn_data`, `okamyextended`, `ave`, `lang`, `description`, `editor-version` + future updates.

Example:
```
name=MiVoz
version=1.0.0
dictionary-path=dictionary.dict
samples=samples
phmn_data=phmn_data.csv
lang=es
descripcion=Banco de ejemplo
editor-version=1.0.0
```

---

### 3.2 `dictionary.dict`
Standard sections: `vowels`, `consonants`, `breaths`, `aspirations`, `unvoiced`. Others are considered custom.
```
# DUID = 000012
vowels:
    a
    e
    a#h
consonants:
    k
    s
breaths:
    br1
    br2
unvoiced:
    h
```

### 3.3 `phmn_data.csv`
Global index of all phonemes:
```
phoneme,path
a,phonetics_folder/phonetic_a.csv
k,phonetics_folder/phonetic_k.csv
```
It is recreated during “Rebuild Phoneme Files”.

---

### 3.4 `samples/`
| File | Function |
|---------|---------|
| *.wav | Base audio |
| *.otrans | Phoneme transcription (after `transcription:`) |
| *.osegment | Consolidated segments (once segmented) |

> Important: `.otrans` and `.osegment` files are removed in the final voicebank build to save space. They are guide files only.  

Example `.otrans`:
```
transcription:
[h] [o] [l] [a]
```

Example `.osegment`:
```
fonema,nota_midi,relativepath,pitch_promedio,inicio,fin
h,G3,196.00,samples/hola.wav,0.000,0.120
o,B3,246.50,samples/hola.wav,0.120,0.380
l,A3,220.10,samples/hola.wav,0.380,0.470
a,C4,261.60,samples/hola.wav,0.470,0.720
```

---

### 3.5 `phonetics_folder/`
Contains a CSV per phoneme (and its variants). See formats below.

---

### 3.6 Expression File `AVE` (Appended Voice Expression)
Path specified in `ave` → `AVE.oe`.  
`aveTable:` section with lines `Name Suffix Color` generates derived vowels (e.g., `a` + `_w` → `a_w`).

---

## 4. Minimal Example
```
ovoice.cfg
dictionary.dict
phmn_data.csv
samples/
  hola.wav
  hola.otrans
  hola.osegment
phonetics_folder/
  phonetic_h.csv
  phonetic_o.csv
  phonetic_l.csv
  phonetic_a.csv
```
---

## 5. Future Recommendations
- Header normalizer/migrator.  
- `VOICEBANK_FORMAT_VERSION` in `ovoice.cfg`.  
- Extra metrics: RMS, formants, loudness.  
- Count cache to reduce I/O.  
- CLI validation tool.

---

## 6. Extra: `.opit` Files and Vowel Refinement
This section documents optional advanced tools that enrich the voicebank: frame-based feature extraction (`.opit`) and manual refinement of vowel segments.

### 6.1 `.opit` Files
Automatically generated JSON file for each `*.wav` in `samples/`. Contains frame-by-frame features (sliding window) for analysis, synthesis, or training.

Suggested use:
- Validate tonal stability before segmenting.  
- Detect non-sounding frames to clean data.  
- Base for future features (Okamy DB AI, Autopitch, etc.).

### 6.2 Vowel Refinement
Manually adjust start/end times of vowel occurrences in `phonetic_{vowel}.csv` files.  
Useful for re-segmenting better and finding the most stable part of the vowel.

---

## 7. Glossary
| Term | Description |
|---------|-------------|
| ovoice.cfg | Main “brain” file |
| dictionary.dict | Phoneme definitions and sections |
| phmn_data.csv | Phoneme → CSV file index |
| .otrans | Sample transcription |
| .osegment | Temporal segmentation per phoneme |
| phonetics_folder | CSV of occurrences/features |
| AVE | Voice Colors, Vocal Modes, Appended Voice Expression |
| OE | Okamy Extended, identifies voicebank extensions |

---

## Extra
Any suggestions for improvement are fully welcome; you can send them to our [Discord](https://discord.gg/zkjrAJaUmK).
