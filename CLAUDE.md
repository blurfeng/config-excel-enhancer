# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Collaboration Rules
- Always reply in Chinese
- Before modifying code, briefly explain the approach first; do not jump straight to code
- When multiple implementation options exist, list them and let me choose rather than picking one yourself
- NEVER perform any git operation automatically (commit, push, branch, reset, etc.); the user handles these. Even without an explicit instruction, do not proactively propose or perform them on the user's behalf

## Commands

```bash
# Build
dotnet build ConfigExcelEnhancer.csproj

# Run (development)
dotnet run --project ConfigExcelEnhancer.csproj

# Release build
dotnet build -c Release ConfigExcelEnhancer.csproj
```

There are no automated tests — this is a UI-heavy tool tested manually via the application.

## What This Is

**ConfigExcelEnhancer** is a .NET 8 Windows Forms desktop utility for game developers using the [Luban](https://github.com/focus-creative-games/luban) config framework. Its main jobs:
1. Read enum definitions from Luban XML schema files and inject Excel data-validation rules into matching columns
2. Generate C# template classes from JSON data files
3. Apply Excel design/formatting templates across multiple files
4. Execute Luban `.bat` export scripts with real-time output

## Architecture

```
Core/         — Pure processing logic, no UI dependencies
UI/           — Windows Forms tabs, one per feature area
Models/       — Plain data classes (AppSettings, EnumInfo, etc.)
Utils/        — SettingsManager, LocalStateManager, ProgressBarHelper
```

**MainForm** owns the tab lifecycle and blocks tab switching during async operations. Each tab (`HomeTab`, `EnumTab`, `LubanTab`, `TemplateTab`, `TableDesignTab`, `ExcelExportTab`, `SettingsTab`) is self-contained and communicates back through events.

**Key Core classes:**
- `EnumScanner` — parses Luban XML (both `<var>` new format and `<option>` legacy format) to extract enum definitions
- `ValidationUpdater` — the heart of the tool; manages the `__enum_data` hidden sheet, DefinedNames, and cell validation rules in Excel workbooks via ClosedXML. Only rewrites files when the enum schema has actually changed.
- `TemplateExporter` / `TemplateRenderer` — Jinja-like placeholder substitution (`$ClassName`, `$TableAccessor`, etc.) for C# code generation
- `TablesClassParser` — extracts table metadata from Luban-generated `Tables.cs`
- `LubanRunner` — runs `.bat` scripts as child processes with async stdout/stderr streaming
- `FunctionLibrary` — contains Excel COM interop for formula refresh (sets `fullCalcOnLoad` since ClosedXML doesn't evaluate formulas)

## Persistence

Two persisted stores. When adding a new config field, decide which one it belongs to using the rule below.

- `settings.json` — `AppSettings`, persisted by `SettingsManager`. **Committed to version control / shared across the team.** **Paths stored as relative** to `LocalState.ProjectRoot` for portability across machines (register any new path field in `SettingsManager.TransformPaths`). See `memory/project_relative_paths.md`.
- `local_state.json` — `LocalState`, persisted by `LocalStateManager` to `LocalApplicationData` (per-exe-location). **Machine-private, NOT version-controlled.** Stores **absolute** paths (no relativization).

### AppSettings vs. LocalState — where to put a new config field

Put it in **`AppSettings`** when it's **stable, project-level config the whole team should share** — e.g. XML/Excel directories, the class→Excel associations of list mode (`ExcelExportClassConfigs`), batch target folder. These are the durable agreements about how the project exports.

Put it in **`LocalState`** when it's **machine-private, transient, or an "current operation / scratch selection"** that would be meaningless (or noisy) to share — e.g. `EnumForceRewrite`, window size, run-state caches, the per-developer Excel naming preference (`ExcelExportNameConvention` / `ExcelExportNamePrefix` / `ExcelExportNameSuffix`), and the single-export mode's current selection (`ExcelExportSingleXmlFile` / `ExcelExportSingleClassName` / `ExcelExportSingleTargetFolder`). Rule of thumb: if storing it in `settings.json` would create git diff noise every time a developer does a routine personal action, it belongs in `LocalState`.

> Path fields in `AppSettings` must be registered in `SettingsManager.TransformPaths` (relative ↔ absolute). Path fields in `LocalState` are stored absolute and need no transform.

## Important Conventions

- **Non-destructive writes**: `ValidationUpdater` compares schema hashes before writing; files unchanged on disk if enum schema hasn't changed (avoids git noise).
- **GBK encoding**: `Program.cs` calls `Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)` at startup — necessary for reading Chinese-content config files.
- **ClosedXML** (not Excel COM) is the primary Excel library; COM is only used in `FunctionLibrary` for formula recalculation.
- **Async task pattern**: Long operations run on background tasks; MainForm disables tab switching via an `IsExecuting` flag during execution.
- Template placeholder syntax uses `$` prefix (e.g. `$ClassName`). See `memory/project_template_export.md` for full syntax and `Tables.cs` parsing logic.