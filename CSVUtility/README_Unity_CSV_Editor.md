# Unity CSV Editor Utility

A simple and intuitive editor window for Unity that allows for the creation, viewing, and editing of CSV (Comma-Separated Values) files directly within the Unity Editor.

This tool is designed to streamline the workflow for developers who use CSV files for game data, such as localization, item databases, character stats, or level configurations. Avoid the hassle of switching between Unity and an external spreadsheet program.

---

## Features

- **Load & Edit**: Open any `.csv` file and display its contents in an easy-to-use grid.
- **Create from Scratch**: Start with an empty grid, add data, and save it as a new `.csv` file.
- **Save & Save As**: Overwrite the existing file or save the data to a new location.
- **Dynamic Grid**: Easily add or remove rows and columns on the fly.
- **Confirmation Dialogs**: Prevent accidental row deletion with a confirmation prompt.
- **Intuitive UI**: Clean, straightforward interface built with standard Unity IMGUI components.
- **Error Handling**: Provides feedback if a file fails to load or save.

---

## Installation

1. **Download**: Download the `CSVEditor.cs` script from this repository.
2. **Create Editor Folder**: In your Unity project's `Assets` folder, create a new folder named `Editor`. *(If it already exists, you can skip this step.)*
3. **Place Script**: Place the `CSVEditor.cs` file inside the `Editor` folder.
4. Unity will automatically compile the script, and you're ready to go!

---

## How to Use

### Open the Window

Navigate to the Unity menu bar and click:

```
RarePixel > CSV Utility > CSV Editor
```

### Load Data

Click the **Load CSV** button to open a file browser and select a `.csv` file.

### Edit Data

- Click into any cell to edit its text content.
- Click the **Add Row** or **Add Column** buttons to expand the grid.
- Click the **`-`** button at the beginning of a row to delete it.

### Save Data

- Click **Save CSV** to save changes to the currently loaded file.
- Click **Save CSV As...** to save the current grid data to a new file.

---

## Technical Notes

- This tool is built using Unity's **IMGUI** system, making it compatible with a wide range of Unity versions.
- The current CSV parsing logic uses a simple:

  ```csharp
  string.Split(',')
  ```

  This is fast and effective for basic CSVs but may not handle complex cases where cell values contain commas enclosed in quotes (e.g., `"item, with, comma"`). For advanced use cases, consider integrating a more robust CSV parsing library.

---

## Contributing

Contributions are welcome! If you have ideas for improvements or have found a bug, please:

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

5. Open a **Pull Request**.

Alternatively, open an issue with the `enhancement` or `bug` tag.

---

## License

This project is licensed under the **MIT License**.  
See the [`LICENSE.md`](LICENSE.md) file for details.