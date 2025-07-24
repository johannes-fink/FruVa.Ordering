# 📘 Technische Dokumentation – Praktikum Johannes Fink

## 🧠 Allgemeine Informationen

### Motivation
Das Tool soll den Bestellprozess effizienter
und übersichtlicher machen. 
Es hilft, Daten einfach zu verwalten, 
Fehler zu reduzieren und Zeit sowie Geld zu sparen, 
indem es die Datenverwaltung optimiert.
Durch eine klare Struktur verbessert es die Nutzererfahrung 
und erleichtert die tägliche Arbeit.

### Programmiersprachen
- C# (Logik, MVVM)  
- SQL (Datenbankabfragen)  
- XAML (UI-Design)

### Verwendete Programme
- Microsoft Visual Studio  
- GitHub  
- Microsoft SQL Server Management Studio 21 (SSMS)  
- Excel (für Datenexport)

### Externe Pakete & Frameworks
- **CommunityToolkit.Mvvm** (MVVM Toolkit)  
- **log4net** (Logging)  
- **Microsoft.Extensions.DependencyInjection** (Dependency Injection)  
  - Automatisches Bereitstellen von Abhängigkeiten wie Services, Datenbankkontexten oder ViewModels  
- **Newtonsoft.Json** (JSON-Verarbeitung)  
  - Zur Übertragung von Daten zwischen Servern und Anwendungen  
- **Entity Framework Core** (Data Access)  
  - Arbeit mit Datenbanken über .NET-Objekte statt direkter SQL-Abfragen

## 📁 Projektstruktur

| Projektname               | Beschreibung                                    |
|--------------------------|------------------------------------------------|
| FruVa.Ordering.Ui         | UI-Projekt mit WPF/XAML und MVVM-Logik         |
| FruVa.Ordering.ApiAccess  | Zugriff auf externe APIs zur Datenbereitstellung|
| FruVa.Ordering.DataAccess | Datenbankzugriff über Entity Framework          |
| FruVa.Ordering.Tests      | Unittests zur Validierung der Geschäftslogik    |

### Warum mehrere Projekte?
- Bessere Übersichtlichkeit  
- Wiederverwendbarkeit (z. B. ApiAccess kann von anderen Anwendungen genutzt werden)
## FruVa.Ordering.UI
### 🧱 MVVM (Model-View-ViewModel)
- **Model:** Datenobjekte (z. B. Article)  
- **ViewModel:** Steuerlogik und Bindings (z. B. FilterWindowViewModel)  
- **View:** XAML-Dateien mit UI und Bindings

### 📦 Verwendete NuGet-Pakete
- **CommunityToolkit.Mvvm**  
  - Erleichtert MVVM mit `[ObservableProperty]` & `[RelayCommand]`  
- **log4net**  
  - Protokolliert Fehler und wichtige Ausgaben  
- **Microsoft.Extensions.DependencyInjection**  
  - Für saubere Abhängigkeitsinjektion von Services und Loggern  

## 🔌 API-Integration (FruVa.Ordering.ApiAccess)
- Schnittstelle zur Datenbereitstellung (Artikel, Empfänger)  
- Daten werden asynchron geladen und gefiltert angezeigt  

## 💾 Datenbankanbindung (Entity Framework Core  FruVa.Ordering.DataAccess )
- Datenmodelle als `DbSet<>` definiert  
- Zugriff über Repository-Pattern  
- Keine direkten SQL-Abfragen (ORM verwendet)  

## 🧪 Unit Tests (FruVa.Ordering.Tests)
- Testet die Verbindungsaufbau 
- Testet die Datenübertragung
 
## 🧰 Tool-Funktionen (FilterWindowViewModel-Beispiel)

### 🔍 Filterfunktion
- Dynamische Suche nach Artikel oder Empfänger  
- Unterstützung für Mehrfachsuche (z. B. „Tomate / Wuppertal GMBH Grossmarkt“)

### 📋 Auswahl & Zustandsspeicherung
- Checkbox-Auswahl via `IsChecked`  
- Synchronisation mit `SelectedOrderDetails`

### ✅ Commands
- `[RelayCommand]` für Aktionen wie Apply und Cancel  
- Automatische ICommand-Implementierung

### 🔁 Umschalten zwischen Artikel und Empfänger
- `IsArticleFilterEnabled` steuert Datenquelle, Titel und Auswahlmodus

### 📥 Asynchrones Laden der Daten
- `LoadLookupDataAsync()` lädt Daten parallel mit `Task.WhenAll()`

### 📊 Export als CSV / Excel
- Exportfunktion erzeugt Excel-kompatible CSV-Dateien  
- Erleichtert externe Auswertung und Archivierung

