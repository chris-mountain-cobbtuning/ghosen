# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run

```bash
dotnet build ghosen.sln              # Build all projects
dotnet build ghosen.sln -c Release   # Release build
dotnet run --project ghosen/ghosen.csproj  # Run the app
dotnet test                          # Run tests (ghosen.Tests, MSTest)
```

Target framework: .NET 5.0 with Windows Desktop (WPF). Output goes to `bin/Debug/` and `bin/Release/`.

## What This Is

ghosen is a WPF desktop tool for automotive ECU diagnostic analysis. It processes CAN bus log files, reassembles ISO-TP transport frames, parses ISO 14229 (UDS) diagnostic messages, and extracts firmware data blocks.

## Architecture

**Data processing pipeline:**
```
Drag-and-drop file → Plugin.ParseLines() → ISO-TP reassembly → ISO 14229 parsing → FileExtractor → binary chunks saved to disk
```

**Key layers (each in its own namespace/directory under `ghosen/`):**

- **Plugin system** (`GhosenPlugin/` + `Plugins/`): `IPluginV1` abstract base in `GhosenPlugin` defines the interface. Six plugin implementations parse different CAN log formats (Candump, Kvaser binary/CSV/text, Vehicle Spy, SavvyCAN). Plugins are dynamically loaded from DLLs via `PluginLoader.cs`.

- **ISO-TP** (`ghosen/ISO-TP/`): `Framing.cs` handles ISO 15765-2 transport protocol — parses Single/First/Consecutive/Flow frames and reassembles multi-frame messages via `ISO_TP_Session.ProcessFrames()`.

- **ISO 14229 / UDS** (`ghosen/ISO14229/`): `Services.cs` enumerates UDS service types (includes Subaru-specific custom modes). `Messages.cs` has a factory `Create()` method that produces specialized message types from `Messages/` subdirectory (RequestDownload, TransferData, RoutineControl with SIMOS18 subclasses, etc.).

- **FileExtractor** (`ghosen/FileExtractor/`): Tracks RequestDownload→TransferData→RequestTransferExit sequences to extract firmware blocks with memory addresses and compression metadata.

- **UI** (`MainWindow.xaml.cs` + `ViewModels/MainWindowViewModel.cs`): MVVM with Fody/PropertyChanged weaving. Drag-and-drop drives the full pipeline. Optional `ArbIdFilter` (e.g., 0x7E0/0x7E8) filters messages before processing.

- **Utils** (`Utils/`): Hex string ↔ byte array conversion utilities.

## Conventions

- Plugins are separate .csproj projects under `Plugins/` that reference `GhosenPlugin`. To add a new format, create a new plugin implementing `IPluginV1`.
- CAN message types use `ArbitrationId` struct for 11-bit CAN IDs.
- PropertyChanged notifications are handled automatically by Fody — no manual `OnPropertyChanged` calls needed in ViewModels.
