# Unity ScriptableObject CSV Import/Export Utility

![Unity Version](https://img.shields.io/badge/Unity-2019.4%2B-blue.svg)
![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)

A powerful editor utility for Unity that bridges the gap between ScriptableObjects and CSV files. This tool allows you to bulk-export data from your ScriptableObject assets into a CSV file for easy editing in any spreadsheet software, and then import the data back to create new ScriptableObject assets.

It's perfect for managing large datasets, collaborating with team members who don't use Unity, or for streamlining data entry for things like item databases, level configurations, or character stats.

---

## Key Features

- **Export to CSV:** Automatically scan a folder for ScriptableObjects of a specific type and export their public fields to a CSV file.
- **Import from CSV:** Parse a CSV file to create and populate new ScriptableObject assets in a designated folder.
- **Reflection-Based:** Dynamically reads the fields of your ScriptableObject class, so no manual mapping is required. The CSV headers are generated from your field names.
- **Handles Prefab References:** Correctly exports and re-links `GameObject` fields by storing their asset path.
- **Type Conversion:** Supports primitives (int, float, string, bool), enums, and `GameObject` references during import.
- **Robust CSV Parsing:** Handles complex values that contain commas or quotes.
- **Configurable Paths:** Easily set the source folder for exports, destination folder for imports, and the CSV file path.
- **User-Friendly Interface:** Provides clear status updates, error messages, and a live count of objects found for export.

---

## Installation

1. **Download:** Download the `ScriptableObjectCsvUtility.cs` script from this repository.
2. **Create Editor Folder:** In your Unity project's `Assets` folder, create a new folder named `Editor`. If it already exists, you can skip this step.
3. **Place Script:** Place the `ScriptableObjectCsvUtility.cs` file inside the `Editor` folder.

Unity will automatically compile the script, and the tool will be ready to use.

---

## How to Use

### 1. Open the Utility Window

Navigate to the Unity menu bar and click:

```
RarePixel > CSV Utility > Open CSV Utility Window
```

### 2. Configure the Settings

Before importing or exporting, you need to configure the paths and the ScriptableObject type.

- **SO Script:** Drag your ScriptableObject C# script file (e.g., `ItemData.cs`) into this field.
- **Export Source Folder:** The project folder where your existing ScriptableObject assets are located (e.g., `Assets/Data/Items`).
- **Import Destination Folder:** The project folder where new ScriptableObject assets will be created during import (e.g., `Assets/Data/ImportedItems`).
- **CSV File Path:** The full path within your project for the CSV file to be read from or written to (e.g., `Assets/Data/gamedata.csv`).

### 3. Exporting Data

1. Assign your **SO Script** and set the **Export Source Folder**.
2. The utility will show you how many objects of that type it found.
3. Click the **Export** button.
4. A CSV file will be created at the specified path, with headers matching the public field names of your ScriptableObject.

### 4. Importing Data

1. Create a CSV file with a header row. The column headers **must exactly match** the public field names in your ScriptableObject script.
2. Fill the CSV with the data you want to import.
3. In the utility window, assign the **SO Script** and set the **Import Destination Folder**.
4. Ensure the **CSV File Path** points to your prepared file.
5. Click the **Import** button and confirm the action.
6. The utility will create a new `.asset` file for each row in the CSV.

---

## CSV Data Formatting

- **Header Row:** The first row of your CSV **must** be a header row containing the names of the public fields from your ScriptableObject class. The order of columns does not matter.
- **Asset Naming:** During import, the utility will try to name the new asset files based on a field named `name` or `id` in your CSV. If neither is found, it will use a unique GUID.
- **Prefab/GameObject References:** To reference a prefab, use its full asset path in the cell (e.g., `Assets/Prefabs/Player.prefab`).
- **Enums:** Use the string name of the enum value (e.g., `Common`, `Rare`, `Epic`).

**Example CSV for an `ItemData` ScriptableObject:**

```csv
id,itemName,itemType,value,iconPrefab
ITEM001,Health Potion,Consumable,50,Assets/Prefabs/Icons/PotionIcon.prefab
ITEM002,Iron Sword,Weapon,120,Assets/Prefabs/Icons/SwordIcon.prefab
```

---

## Contributing

Contributions are welcome! If you have ideas for improvements or have found a bug, please feel free to open an issue or a pull request.

1. Fork the repository.
2. Create a new feature branch:

   ```bash
   git checkout -b feature/AmazingFeature
   ```

3. Commit your changes:

   ```bash
   git commit -m "Add some AmazingFeature"
   ```

4. Push to the branch:

   ```bash
   git push origin feature/AmazingFeature
   ```

5. Open a Pull Request.

---

## License

This project is licensed under the MIT License.