# Funcionamiento del Motor de Síntesis

## 1. Proceso de Búsqueda de Fonemas
El motor de síntesis utiliza un banco de voz para generar una voz sintetizada. A continuación, se describe el flujo de cómo trabaja el motor:

1. **Carga de la Base de Datos de Fonemas**:
   - Durante la inicialización, el motor carga los datos de fonemas desde un archivo CSV especificado en la configuración del cantante (`ovoice.cfg`).
   - El archivo `ovoice.cfg` contiene una línea con la clave `phmn_data`, que apunta a un archivo CSV principal que lista los fonemas y sus rutas asociadas.
   - Cada línea del archivo CSV principal tiene el siguiente formato:
     ```
     <fonema>,<ruta_del_archivo_csv>
     ```
     Por ejemplo:
     ```
     a,phonetics_folder/phonetic_a.csv
     k,phonetics_folder/phonetic_k.csv
     ```
   - Para cada fonema listado, el motor carga un archivo CSV adicional (por ejemplo, `phonetic_a.csv`) que contiene las muestras asociadas al fonema.

2. **Estructura de los Archivos CSV de Muestras**:
   - Cada archivo CSV de muestras tiene el siguiente formato:
     ```
     <Nota>,<Frecuencia>,<Ruta_del_Archivo>,<Inicio_Segmento>,<Fin_Segmento>
     ```
     Por ejemplo:
     ```
     C4,261.63,audios/a.wav,0.000,0.521
     D4,293.66,audios/a.wav,0.556,1.000
     ```
   - Estos datos se utilizan para construir una lista de objetos `PhonemeSample` que contienen:
     - `NoteName`: Nombre de la nota (por ejemplo, `C4`).
     - `Frequency`: Frecuencia de la nota en Hz.
     - `FilePath`: Ruta del archivo de audio.
     - `SegmentStart` y `SegmentEnd`: Tiempos de inicio y fin del segmento en el archivo de audio.

3. **Búsqueda del Fonema Más Cercano**:
   - Cuando se necesita sintetizar una nota, el motor busca el fonema más cercano en la base de datos utilizando el método `GetClosestSample`.
   - El flujo es el siguiente:
     1. Se verifica si el fonema existe en la base de datos cargada.
     2. Si no existe, se lanza una excepción y se muestra un mensaje de error al usuario.
     3. Si existe, se selecciona la muestra cuya frecuencia esté más cerca de la frecuencia objetivo calculada para la nota.

## 2. Proceso de Síntesis de Notas
El método principal para sintetizar notas es `SynthesizeNote`. A continuación, se describe su funcionamiento:

1. **Parámetros de Entrada**:
   - `NoteData`: Contiene información sobre la nota, como la letra (`Lyric`), el tono (`Pitch`), la transcripción fonética (`Transcription`) y la duración (`DurationTick`).
   - `ProjectData`: Contiene información del proyecto, como el tempo y la resolución de ticks (`PPQ`).

2. **Flujo de Ejecución**:
   - **Duración de la Nota**:
     - Se calcula la duración de la nota en segundos utilizando el tempo y los ticks.
   - **Transcripción de Fonemas**:
     - Este paso ya se debe haber hecho automáticamente con el propio `G2P` (Grapheme To Phoneme).
   - **Cálculo de Duración de Consonante y Vocal**:
     - Este proceso es necesario ya que debe calcular la duración de la vocal: `NoteDur`-`(<sub>1</sub>ConsonantDur)=VowelDur`.
   - **Normalización de Audio de Consonante**:
     - El motor normaliza el segmento de audio para tener un sonido final más estable e igualado en términos de volumen.
   - **Procesamiento de Consonantes**:
     - Cada consonante se procesa individualmente, ajustando tono y aplicando formantes.
   - **Normalización de Audio de Vocales**:
     - El motor normaliza el segmento de audio para tener un sonido final más estable e igualado en términos de volumen.
   - **Procesamiento de Vocales**:
     - La vocal principal se procesa ajustando su duración, tono y formantes.
     - **Procesamiento de Duración de Vocal**  
        - Este proceso funciona de manera similar a como lo hace **UTAU**: la vocal principal se estira o comprime en el tiempo para ajustarse a la duración de la nota.  
        - Además, durante este estiramiento se ajusta el **f0** (frecuencia fundamental) y se corrigen los **formantes**, para que la voz mantenga un timbre natural incluso cuando se modifica la duración o el tono.
   - **Concatenación de Segmentos**:
     - Los segmentos de audio generados para las consonantes y vocales se concatenan para formar la nota final.
   - **Normalización**:
     - El audio resultante se normaliza para ajustar su volumen.

3. **Errores y Excepciones**:
   - Si ocurre un error durante la síntesis, se captura la excepción y se devuelve un arreglo vacío de muestras de audio.

## 3. Métodos Clave
A continuación, se describen algunos de los métodos clave del motor:

### `LoadPhonemeDatabase`
- Carga la base de datos de fonemas desde un archivo CSV principal y los archivos CSV individuales asociados.
- Asocia cada fonema con una lista de muestras.

### `GetClosestSample`
- Busca la muestra más cercana para un fonema dado y un tono objetivo.

### `SynthesizeNote`
- Genera el audio para una nota específica.
- Procesa consonantes y vocales por separado y luego concatena los resultados.

### `ApplyPitch`
- Ajusta el tono de un segmento de audio utilizando una curva de tono.

### `ApplyCurvePitch`
- Aplica la curva de tono general al audio sintetizado.

### `NormalizeAudio`
- Normaliza el volumen del audio resultante.

## 4. Configuración del Cantante
El archivo de configuración del cantante (`ovoice.cfg`) contiene información clave como:
- Idioma (`lang`), importante para seleccionar el motor de síntesis.
- Ruta de la base de datos de fonemas (`phmn_data`).
- Ruta del diccionario de fonemas (`dictionary-path`).

## 5. Limitaciones y Consideraciones
- El motor está diseñado específicamente para el idioma japonés (`lang=ja`).
- Si no se encuentra un fonema, se utiliza un valor predeterminado seleccionado previamente en el editor (por ejemplo, la vocal `a`).
- La duración mínima de una nota es de 0.1 segundos para evitar problemas de síntesis.

## Extra
- El motor de síntesis usa librerías como **WORLD** y **NAudio**.  
- Se procura utilizar la menor cantidad posible de procesos de edición de audio para ofrecer una voz más realista.
---
