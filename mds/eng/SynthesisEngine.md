# Synthesis Engine Operation

## 1. Phoneme Search Process
The synthesis engine uses a voice bank to generate a synthesized voice. The following describes the workflow of how the engine works:

1. **Loading the Phoneme Database**:
   - During initialization, the engine loads phoneme data from a CSV file specified in the singer configuration (`ovoice.cfg`).
   - The `ovoice.cfg` file contains a line with the key `phmn_data`, which points to a main CSV file listing the phonemes and their associated paths.
   - Each line in the main CSV file has the following format:
     ```
     <phoneme>,<path_to_csv_file>
     ```
     For example:
     ```
     a,phonetics_folder/phonetic_a.csv
     k,phonetics_folder/phonetic_k.csv
     ```
   - For each listed phoneme, the engine loads an additional CSV file (e.g., `phonetic_a.csv`) containing the samples associated with that phoneme.

2. **Structure of Sample CSV Files**:
   - Each sample CSV file has the following format:
     ```
     <Note>,<Frequency>,<File_Path>,<Segment_Start>,<Segment_End>
     ```
     For example:
     ```
     C4,261.63,audios/a.wav,0.000,0.521
     D4,293.66,audios/a.wav,0.556,1.000
     ```
   - These data are used to build a list of `PhonemeSample` objects containing:
     - `NoteName`: Note name (e.g., `C4`).
     - `Frequency`: Note frequency in Hz.
     - `FilePath`: Path to the audio file.
     - `SegmentStart` and `SegmentEnd`: Start and end times of the segment in the audio file.

3. **Search for the Closest Phoneme**:
   - When a note needs to be synthesized, the engine searches for the closest phoneme in the database using the `GetClosestSample` method.
   - The workflow is as follows:
     1. Check if the phoneme exists in the loaded database.
     2. If it does not exist, an exception is thrown and an error message is shown to the user.
     3. If it exists, the sample whose frequency is closest to the target frequency calculated for the note is selected.

## 2. Note Synthesis Process
The main method for synthesizing notes is `SynthesizeNote`. Its operation is described below:

1. **Input Parameters**:
   - `NoteData`: Contains information about the note, such as the lyric (`Lyric`), pitch (`Pitch`), phonetic transcription (`Transcription`), and duration (`DurationTick`).
   - `ProjectData`: Contains project information, such as tempo and tick resolution (`PPQ`).

2. **Execution Flow**:
   - **Note Duration**:
     - The note duration in seconds is calculated using tempo and ticks.
   - **Phoneme Transcription**:
     - This step should already have been done automatically by the `G2P` (Grapheme To Phoneme).
   - **Consonant and Vowel Duration Calculation**:
     - This process is necessary to calculate the vowel duration: `NoteDur` - `ConsonantDur` = `VowelDur`.
   - **Consonant Audio Normalization**:
     - The engine normalizes the audio segment to achieve a more stable and balanced final sound in terms of volume.
   - **Consonant Processing**:
     - Each consonant is processed individually, adjusting pitch and applying formants.
   - **Vowel Audio Normalization**:
     - The engine normalizes the audio segment to achieve a more stable and balanced final sound in terms of volume.
   - **Vowel Processing**:
     - The main vowel is processed by adjusting its duration, pitch, and formants.
     - **Vowel Duration Processing**  
        - This process works similarly to **UTAU**: the main vowel is stretched or compressed in time to fit the note duration.  
        - Additionally, during this stretching the **f0** (fundamental frequency) is adjusted and **formants** are corrected, so that the voice maintains a natural timbre even when duration or pitch are modified.
   - **Segment Concatenation**:
     - The generated audio segments for consonants and vowels are concatenated to form the final note.
   - **Normalization**:
     - The resulting audio is normalized to adjust its volume.

3. **Errors and Exceptions**:
   - If an error occurs during synthesis, the exception is caught and an empty array of audio samples is returned.

## 3. Key Methods
Below are some of the key methods of the engine:

### `LoadPhonemeDatabase`
- Loads the phoneme database from a main CSV file and the associated individual CSV files.
- Associates each phoneme with a list of samples.

### `GetClosestSample`
- Searches for the closest sample for a given phoneme and target pitch.

### `SynthesizeNote`
- Generates the audio for a specific note.
- Processes consonants and vowels separately and then concatenates the results.

### `ApplyPitch`
- Adjusts the pitch of an audio segment using a pitch curve.

### `ApplyCurvePitch`
- Applies the overall pitch curve to the synthesized audio.

### `NormalizeAudio`
- Normalizes the volume of the resulting audio.

## 4. Singer Configuration
The singer configuration file (`ovoice.cfg`) contains key information such as:
- Language (`lang`), important for selecting the synthesis engine.
- Path to the phoneme database (`phmn_data`).
- Path to the phoneme dictionary (`dictionary-path`).

## 5. Limitations and Considerations
- The engine is specifically designed for the Japanese language (`lang=ja`).
- If a phoneme is not found, a default value previously selected in the editor is used (for example, the vowel `a`).
- The minimum duration of a note is 0.1 seconds to avoid synthesis problems.

## Extra
- The synthesis engine uses libraries such as **WORLD** and **NAudio**.  
- It aims to use the least amount of audio editing processes to deliver a more realistic voice.
