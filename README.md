# Toto Analyzer (.NET Console App)

## Description

Toto Analyzer is a .NET console application that downloads, parses and analyzes historical Bulgarian Sports Toto 6/49 data.

The application automatically downloads TXT and DOCX files from the official Sports Toto statistics website and performs statistical analysis using LINQ.

Official data source:
https://info.toto.bg/statistika/6x49

---

## Features

### Data Loading
- Downloading files using HttpClient
- TXT file parsing
- DOCX file parsing using OpenXML
- Data stored in IEnumerable collection

### Statistical Analysis (LINQ)
- Top N most frequent numbers
- Hot pairs (most common combinations)
- Distribution by ranges:
  - 1–10
  - 11–20
  - 21–30
  - 31–40
  - 41–49

### Console Visualizations
- ASCII Bar Chart
- Heat Map (7x7)

### Interactive Console Menu
- Period selection
- Input validation
- Interactive statistics menu

---

## Technologies Used

- C#
- .NET
- LINQ
- HttpClient
- OpenXML SDK

---

## Project Structure

```text
TotoAnalyzer/
│
├── Models/
│   └── Draw.cs
│
├── Services/
│   ├── DataLoader.cs
│   ├── Statistics.cs
│   └── Visualizer.cs
│
├── Program.cs
└── README.md
```

---

## Run Project

```bash
dotnet run
```

---

## Author
Irena Rizovska

Educational project developed for .NET programming coursework.
