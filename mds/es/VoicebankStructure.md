# Okamy Voicebank(DB)
Información sobre la estructura de un Okamy Voicebank.

---

## 1. General
Un Okamy DB contiene toda la información de segmentación fonética de audios, transcripciones, información principal del DB (nombre, idioma, versión) y otros recursos como los Voice Colors.

---

## 2. Estructura de Carpetas
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

## 3. Archivos Principales

### 3.1 `ovoice.cfg`
Archivo de pares `clave=valor`.  
Claves: `name`, `version`, `dictionary-path`, `icon-sample`, `img-sample`, `samples`, `phmn_data`, `okamyextended`, `ave`, `lang`, `descripcion`, `editor-version` + futuras actualizaciones.

Ejemplo:
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
Secciones estándar: `vowels`, `consonants`, `breaths`, `aspirations`, `unvoiced`. Otras se consideran personalizadas.
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
Índice global de todos los fonemas:
```
phoneme,path
a,phonetics_folder/phonetic_a.csv
k,phonetics_folder/phonetic_k.csv
```
Se recrea durante “Rebuild Phoneme Files”.

---

### 3.4 `samples/`
| Archivo | Función |
|---------|---------|
| *.wav | Audio base |
| *.otrans | Transcripción de fonemas (después de `transcription:`) |
| *.osegment | Segmentos consolidados (una vez segmentado) |

> Importante: Los archivos `.otrans` y `.osegment` son eliminados en la construcción final del banco de voz para liberar espacio. Son solo archivos guía.  

Ejemplo `.otrans`:
```
transcription:
[h] [o] [l] [a]
```

`.osegment` ejemplo:
```
fonema,nota_midi,relativepath,pitch_promedio,inicio,fin
h,G3,196.00,samples/hola.wav,0.000,0.120
o,B3,246.50,samples/hola.wav,0.120,0.380
l,A3,220.10,samples/hola.wav,0.380,0.470
a,C4,261.60,samples/hola.wav,0.470,0.720
```

---

### 3.5 `phonetics_folder/`
Contiene un CSV por fonema (y sus variantes). Ver formatos más abajo.

---

### 3.6 Archivo de Expresiones `AVE` (Appened Voice Expression)
Ruta especificada en `ave` → `AVE.oe`.  
Sección `aveTable:` con líneas `Nombre Sufijo Color` genera vocales derivadas (ej. `a` + `_w` → `a_w`).

---

## 4. Ejemplo Mínimo
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

## 5. Recomendaciones Futuras
- Normalizador/migrador de cabeceras.  
- `VOICEBANK_FORMAT_VERSION` en `ovoice.cfg`.  
- Métricas extra: RMS, formantes, loudness.  
- Cache de conteos para reducir I/O.  
- Herramienta CLI de validación.

---

## 6. Extra: Archivos `.opit` y Refinamiento de Vocales
Esta sección documenta herramientas avanzadas opcionales que enriquecen el voicebank: extracción de características por frame (.opit) y refinamiento manual de segmentos vocálicos.

### 6.1 Archivos `.opit`
Archivo JSON generado automáticamente para cada `*.wav` en `samples/`. Contiene características por frame (ventana deslizante) para análisis, síntesis o entrenamiento.

Uso sugerido:
- Validar estabilidad tonal antes de segmentar.  
- Detectar frames no sonoros para limpiar datos.  
- Base para futuras funciones (Okamy DB AI, Autopitch, etc.).

### 8.2 Refinamiento de Vocales
Ajustar manualmente los tiempos de inicio/fin de ocurrencias vocálicas presentes en los CSV `phonetic_{vocal}.csv`.  
Sirve para re-segmentar mejor y encontrar la parte más estable de la vocal.

---

## 7. Glosario
| Término | Descripción |
|---------|-------------|
| ovoice.cfg | Archivo principal “cerebro” |
| dictionary.dict | Definición de fonemas y secciones |
| phmn_data.csv | Índice fonema → archivo CSV |
| .otrans | Transcripción por muestra |
| .osegment | Segmentación temporal por fonema |
| phonetics_folder | CSV de ocurrencias/características |
| AVE | Voice Colors, Vocal Modes, Appened Voice Expression |
| OE | Okamy Extended, identifica extensiones del banco de voz |

---

## Extra
Cualquier sugerencia de mejora es totalmente aceptada, puedes mandarla a nuestro servidor de [Discord](https://discord.gg/zkjrAJaUmK).
