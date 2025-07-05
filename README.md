# Unity Utilities by RarePixel

![Unity Version](https://img.shields.io/badge/Unity-2019.4%2B-blue.svg)
![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)

A growing collection of powerful and easy-to-use **Unity Editor Utilities** developed by **RarePixel**, designed to simplify your workflow, enhance productivity, and make your game development smoother — right from within the Unity Editor.

This repository includes a modular set of editor tools for common tasks such as data management, CSV editing, and bulk asset handling, all built with Unity's native **IMGUI** system for maximum compatibility and stability.

---

## Included Utilities

### 🔹 [CSV Editor Utility](./CSVUtility/README_Unity_CSV_Editor.md)

An intuitive, spreadsheet-like editor window for directly editing `.csv` files inside the Unity Editor.

- Create, edit, and save CSVs with dynamic grids
- Add/remove rows and columns on the fly
- Perfect for localization, stats, and config data
- Built entirely using Unity IMGUI
- No external dependencies

➡️ [Read More »](./CSVUtility/README_Unity_CSV_Editor.md)

---

### 🔹 [ScriptableObject CSV Import/Export Utility](./CSVUtility/README_SO_CSV_Utility.md)

Bridge the gap between ScriptableObjects and CSVs. Easily export your game data to spreadsheets, and reimport for fast bulk editing.

- Export public fields of ScriptableObjects to CSV
- Import from CSV to generate ScriptableObject assets
- Supports basic types, enums, and GameObject references
- Reflection-based, no mapping setup required
- Fully configurable import/export paths

➡️ [Read More »](./CSVUtility/README_SO_CSV_Utility.md)

---

## Installation

Each utility is self-contained and can be installed independently:

1. Download the `.cs` script(s) from the utility you need.
2. Place them into an `Editor` folder inside your Unity project's `Assets` directory.
3. Unity will compile the scripts and make the utilities available via the top menu bar.

---

## Compatibility

- Unity 2019.4 LTS and above
- Compatible with Windows, macOS, and Linux
- IMGUI-based — no dependency on UI Toolkit or third-party libraries

---

## Contribution

We welcome contributions! If you have an idea for a new utility or an improvement to an existing one:

1. Fork the repository.
2. Create a feature branch:

   ```bash
   git checkout -b feature/NewUtility
   ```

3. Commit and push your changes.
4. Open a Pull Request and describe your addition.

---

## License

This repository is licensed under the **MIT License**.  
See [`LICENSE.md`](./LICENSE) for more details.

---

## Stay Connected

Follow RarePixel for more Unity tools, assets, and open-source projects.

📬 Feedback and suggestions welcome!
