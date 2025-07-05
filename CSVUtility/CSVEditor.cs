using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class CSVEditor : EditorWindow
{
    // List to hold the grid data (rows and columns)
    private List<List<string>> grid = new List<List<string>>();
    // Scroll position for the grid view
    private Vector2 scrollPosition;
    // Path of the currently loaded CSV file
    private string filePath;

    // Add a menu item named "CSV Editor" to the "Tools" menu
    [MenuItem("RarePixel/CSV Utility/CSV Editor")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(CSVEditor), false, "CSV Editor");
    }

    // Called to draw the GUI for the editor window
    void OnGUI()
    {
        // Display title and introductory text
        GUILayout.Label("CSV File Editor", EditorStyles.boldLabel);
        GUILayout.Space(5);
        EditorGUILayout.HelpBox("Load a CSV file to view and edit its contents. Use the buttons below to manage the data.", MessageType.Info);
        GUILayout.Space(10);

        // --- Toolbar for file operations ---
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        if (GUILayout.Button("Load CSV", EditorStyles.toolbarButton))
        {
            LoadCSV();
        }

        // The Save button is disabled if no file is loaded
        GUI.enabled = !string.IsNullOrEmpty(filePath);
        if (GUILayout.Button("Save CSV", EditorStyles.toolbarButton))
        {
            SaveCSV();
        }
        GUI.enabled = true; // Re-enable GUI elements

        if (GUILayout.Button("Save CSV As...", EditorStyles.toolbarButton))
        {
            SaveCSVAs();
        }
        GUILayout.FlexibleSpace(); // Pushes subsequent buttons to the right
        EditorGUILayout.EndHorizontal();
        
        // Display the loaded file path
        if (!string.IsNullOrEmpty(filePath))
        {
            EditorGUILayout.LabelField("Current File:", filePath, EditorStyles.miniLabel);
        }
        else
        {
            EditorGUILayout.LabelField("Current File:", "No file loaded", EditorStyles.miniLabel);
        }


        GUILayout.Space(10);

        // --- Grid editing controls ---
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Row", GUILayout.Width(100)))
        {
            AddRow();
        }
        if (GUILayout.Button("Add Column", GUILayout.Width(100)))
        {
            AddColumn();
        }
        EditorGUILayout.EndHorizontal();
        
        GUILayout.Space(10);

        // --- Scrollable Grid View ---
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        // If the grid is empty, show a message
        if (grid.Count == 0)
        {
            EditorGUILayout.HelpBox("No data to display. Load a CSV file or add rows/columns to start.", MessageType.None);
        }
        else
        {
            // Begin drawing the grid layout
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            for (int i = 0; i < grid.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                // Button to remove the current row
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    if (EditorUtility.DisplayDialog("Confirm Delete", "Are you sure you want to delete this row?", "Yes", "No"))
                    {
                        RemoveRow(i);
                        // Exit the loop to avoid trying to draw the removed row
                        break; 
                    }
                }

                // Create text fields for each cell in the row
                for (int j = 0; j < grid[i].Count; j++)
                {
                    grid[i][j] = EditorGUILayout.TextField(grid[i][j], GUILayout.MinWidth(100));
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// Opens a file dialog to select and load a CSV file.
    /// </summary>
    private void LoadCSV()
    {
        // Open a file dialog to get the path for a .csv file
        string path = EditorUtility.OpenFilePanel("Open CSV File", "", "csv");
        if (string.IsNullOrEmpty(path)) return; // User cancelled the dialog

        filePath = path;
        grid.Clear(); // Clear existing data

        try
        {
            // Read all lines from the selected file
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                // Split the line by commas to get individual cell values
                // This is a simple parser. For complex CSVs (with quoted commas), a more robust parser would be needed.
                string[] values = line.Split(',');
                grid.Add(new List<string>(values));
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading CSV file: {e.Message}");
            EditorUtility.DisplayDialog("Error", "Could not load the CSV file. Check the console for more details.", "OK");
        }
        
        // Repaint the window to show the new data
        Repaint();
    }

    /// <summary>
    /// Saves the current grid data to the original file path.
    /// </summary>
    private void SaveCSV()
    {
        if (string.IsNullOrEmpty(filePath))
        {
            EditorUtility.DisplayDialog("Save Error", "No file path specified. Use 'Save As...' first.", "OK");
            return;
        }
        WriteToFile(filePath);
    }

    /// <summary>
    /// Opens a file dialog to save the current grid data to a new file.
    /// </summary>
    private void SaveCSVAs()
    {
        string path = EditorUtility.SaveFilePanel("Save CSV As...", "", "data.csv", "csv");
        if (string.IsNullOrEmpty(path)) return; // User cancelled

        filePath = path;
        WriteToFile(filePath);
    }

    /// <summary>
    /// Writes the grid data to a specified file path in CSV format.
    /// </summary>
    /// <param name="path">The file path to write to.</param>
    private void WriteToFile(string path)
    {
        try
        {
            StringBuilder sb = new StringBuilder();
            // Build the CSV string from the grid data
            foreach (var row in grid)
            {
                // Join the elements of the row with commas
                sb.AppendLine(string.Join(",", row));
            }

            // Write the string to the file
            File.WriteAllText(path, sb.ToString());
            
            // Show a success message and refresh the Asset Database
            EditorUtility.DisplayDialog("Save Successful", $"The data was saved to:\n{path}", "OK");
            AssetDatabase.Refresh();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saving CSV file: {e.Message}");
            EditorUtility.DisplayDialog("Error", "Could not save the CSV file. Check the console for more details.", "OK");
        }
    }

    /// <summary>
    /// Adds a new, empty row to the grid.
    /// </summary>
    private void AddRow()
    {
        List<string> newRow = new List<string>();
        int columnCount = (grid.Count > 0) ? grid[0].Count : 1;
        for (int i = 0; i < columnCount; i++)
        {
            newRow.Add(""); // Add empty strings for each cell
        }
        grid.Add(newRow);
    }

    /// <summary>
    /// Removes a row at a specific index.
    /// </summary>
    /// <param name="index">The index of the row to remove.</param>
    private void RemoveRow(int index)
    {
        if (index >= 0 && index < grid.Count)
        {
            grid.RemoveAt(index);
        }
    }

    /// <summary>
    /// Adds a new, empty column to the grid.
    /// </summary>
    private void AddColumn()
    {
        if (grid.Count == 0)
        {
            // If the grid is empty, add a single cell to start
            AddRow();
        }
        else
        {
            foreach (var row in grid)
            {
                row.Add(""); // Add an empty cell to each existing row
            }
        }
    }
}
