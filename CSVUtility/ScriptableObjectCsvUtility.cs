using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using System.Linq;
using System;
using System.Text.RegularExpressions;

/// <summary>
/// An editor window to export data from ScriptableObjects to a CSV file,
/// and import data from a CSV file to create new ScriptableObject assets.
/// Handles prefab references by storing their asset path.
/// </summary>
public class ScriptableObjectCsvUtility : EditorWindow
{
    // --- CONFIGURATION FIELDS ---
    private MonoScript scriptableObjectScript = null;
    private string exportSourceFolderPath = "Assets/ScriptableObjects";
    private string importDestinationFolderPath = "Assets/ImportedScriptableObjects";
    private string csvFilePath = "Assets/data.csv";
    
    // --- UI & STATUS ---
    private Vector2 scrollPosition;
    private string statusMessage = "Ready.";
    private MessageType statusMessageType = MessageType.Info;
    private int objectsFoundForExport = 0;

    /// <summary>
    /// Creates a menu item to open this editor window.
    /// </summary>
    [MenuItem("RarePixel/CSV Utility/Open CSV Utility Window")]
    public static void ShowWindow()
    {
        // Get existing open window or if none, make a new one.
        GetWindow<ScriptableObjectCsvUtility>("CSV Utility");
    }

    /// <summary>
    /// Called when the window is focused. Used to update the count of objects.
    /// </summary>
    void OnFocus()
    {
        UpdateFoundObjectsCount();
    }

    /// <summary>
    /// Draws the editor window GUI.
    /// </summary>
    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUILayout.LabelField("CSV Import/Export Utility", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Use this tool to export ScriptableObjects to a CSV file or import data from a CSV to create new ScriptableObject assets.", MessageType.None);

        // --- CONFIGURATION SECTION ---
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Configuration", EditorStyles.boldLabel);
        
        scriptableObjectScript = (MonoScript)EditorGUILayout.ObjectField(new GUIContent("SO Script", "The ScriptableObject script to process."), scriptableObjectScript, typeof(MonoScript), false);
        exportSourceFolderPath = EditorGUILayout.TextField(new GUIContent("Export Source Folder", "The folder to search for ScriptableObjects to export."), exportSourceFolderPath);
        importDestinationFolderPath = EditorGUILayout.TextField(new GUIContent("Import Destination Folder", "The folder where newly created ScriptableObjects will be saved."), importDestinationFolderPath);
        csvFilePath = EditorGUILayout.TextField(new GUIContent("CSV File Path", "The path to the CSV file for both reading and writing."), csvFilePath);

        // --- ACTIONS SECTION ---
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);

        // EXPORT BUTTON
        if (GUILayout.Button($"Export ({objectsFoundForExport} Objects Found)"))
        {
            ExportDataToCsv();
        }
        
        // IMPORT BUTTON
        if (GUILayout.Button("Import from CSV"))
        {
            if (EditorUtility.DisplayDialog("Confirm Import", 
                "This will create new ScriptableObject assets in the destination folder based on the CSV data. Are you sure you want to continue?", 
                "Yes, Import", "Cancel"))
            {
                ImportDataFromCsv();
            }
        }
        
        // --- STATUS SECTION ---
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Status", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(statusMessage, statusMessageType);

        EditorGUILayout.EndScrollView();

        // Live update for object count as path is typed
        if (GUI.changed)
        {
            UpdateFoundObjectsCount();
        }
    }
    
    /// <summary>
    /// Updates the count of ScriptableObjects found in the export path.
    /// </summary>
    private void UpdateFoundObjectsCount()
    {
        Type soType = GetSelectedType();
        if (soType == null || string.IsNullOrEmpty(exportSourceFolderPath))
        {
            objectsFoundForExport = 0;
            return;
        }
        string[] guids = AssetDatabase.FindAssets($"t:{soType.Name}", new[] { exportSourceFolderPath });
        objectsFoundForExport = guids.Length;
        Repaint(); // Redraw the window to show the new count
    }

    #region Core Logic

    private void ExportDataToCsv()
    {
        Type soType = GetSelectedType();
        if (soType == null)
        {
            SetStatus("Please assign a ScriptableObject script in the 'SO Script' field.", MessageType.Error);
            return;
        }

        string[] guids = AssetDatabase.FindAssets($"t:{soType.Name}", new[] { exportSourceFolderPath });

        if (guids.Length == 0)
        {
            SetStatus($"No ScriptableObjects of type '{soType.Name}' found in '{exportSourceFolderPath}'.", MessageType.Warning);
            return;
        }

        List<ScriptableObject> scriptableObjects = new List<ScriptableObject>();
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            ScriptableObject so = AssetDatabase.LoadAssetAtPath(assetPath, soType) as ScriptableObject;
            if (so != null)
            {
                scriptableObjects.Add(so);
            }
        }

        if (scriptableObjects.Count == 0)
        {
             SetStatus($"Could not load any ScriptableObjects from folder: '{exportSourceFolderPath}'.", MessageType.Warning);
            return;
        }

        FieldInfo[] fields = soType.GetFields(BindingFlags.Public | BindingFlags.Instance);
        StringBuilder csvContent = new StringBuilder();
        csvContent.AppendLine(string.Join(",", fields.Select(f => f.Name)));

        foreach (var so in scriptableObjects)
        {
            var rowData = new List<string>();
            foreach(var field in fields)
            {
                object value = field.GetValue(so);
                string stringValue;

                // If the field is a GameObject (prefab), get its asset path.
                if (value is GameObject go)
                {
                    stringValue = AssetDatabase.GetAssetPath(go);
                }
                else
                {
                    stringValue = value?.ToString() ?? "";
                }
                rowData.Add(SanitizeCsvField(stringValue));
            }
            csvContent.AppendLine(string.Join(",", rowData));
        }

        WriteToFile(csvFilePath, csvContent.ToString(), $"Successfully exported {scriptableObjects.Count} objects to '{csvFilePath}'.", MessageType.Info);
    }

    private void ImportDataFromCsv()
    {
        EnsureDirectoryExists(importDestinationFolderPath);

        Type soType = GetSelectedType();
        if (soType == null)
        {
            SetStatus("Please assign a ScriptableObject script in the 'SO Script' field before importing.", MessageType.Error);
            return;
        }

        if (!soType.IsSubclassOf(typeof(ScriptableObject)))
        {
            SetStatus($"The assigned script '{soType.Name}' does not inherit from ScriptableObject.", MessageType.Error);
            return;
        }

        string[] lines;
        try
        {
            lines = File.ReadAllLines(csvFilePath);
        }
        catch (Exception e)
        {
            SetStatus($"Failed to read CSV file at '{csvFilePath}'. Error: {e.Message}", MessageType.Error);
            return;
        }

        if (lines.Length < 2)
        {
            SetStatus("CSV file is empty or contains only a header.", MessageType.Warning);
            return;
        }

        string[] headers = ParseCsvRow(lines[0]);
        int createdCount = 0;

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = ParseCsvRow(lines[i]);
            if (values.Length != headers.Length)
            {
                Debug.LogWarning($"Skipping row {i+1}: Mismatched column count. Expected {headers.Length}, got {values.Length}.");
                continue;
            }

            ScriptableObject newSo = CreateInstance(soType);

            for (int j = 0; j < headers.Length; j++)
            {
                FieldInfo field = soType.GetField(headers[j], BindingFlags.Public | BindingFlags.Instance);
                if (field != null)
                {
                    try
                    {
                        object convertedValue = ConvertValue(values[j], field.FieldType);
                        field.SetValue(newSo, convertedValue);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to set field '{headers[j]}' on row {i+1}. Value: '{values[j]}'. Error: {e.Message}");
                    }
                }
            }
            
            string assetName = Guid.NewGuid().ToString();
            int nameIndex = Array.IndexOf(headers, "name");
            if (nameIndex != -1 && !string.IsNullOrEmpty(values[nameIndex])) assetName = values[nameIndex];
            else
            {
                int idIndex = Array.IndexOf(headers, "id");
                if (idIndex != -1 && !string.IsNullOrEmpty(values[idIndex])) assetName = values[idIndex];
            }

            string assetPath = Path.Combine(importDestinationFolderPath, $"{assetName}.asset");
            string uniqueAssetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

            AssetDatabase.CreateAsset(newSo, uniqueAssetPath);
            createdCount++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        SetStatus($"Successfully imported and created {createdCount} ScriptableObjects in '{importDestinationFolderPath}'.", MessageType.Info);
    }
    
    #endregion

    #region Helper Methods

    private Type GetSelectedType()
    {
        if (scriptableObjectScript == null)
        {
            return null;
        }
        return scriptableObjectScript.GetClass();
    }

    private static object ConvertValue(string value, Type targetType)
    {
        if (string.IsNullOrEmpty(value)) return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        
        // If the target is a GameObject, load it from the asset path.
        if (targetType == typeof(GameObject))
        {
            return AssetDatabase.LoadAssetAtPath<GameObject>(value);
        }

        if (targetType.IsEnum) return Enum.Parse(targetType, value, true);
        
        return Convert.ChangeType(value, targetType);
    }

    private static string SanitizeCsvField(string field)
    {
        if (string.IsNullOrEmpty(field)) return "";
        if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }
        return field;
    }

    private static string[] ParseCsvRow(string row)
    {
        var pattern = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        var values = new Regex(pattern).Split(row);
        for (int i = 0; i < values.Length; i++)
        {
            if (values[i].StartsWith("\"") && values[i].EndsWith("\""))
            {
                values[i] = values[i].Substring(1, values[i].Length - 2).Replace("\"\"", "\"");
            }
        }
        return values;
    }

    private static void EnsureDirectoryExists(string path)
    {
        string directory = path.EndsWith("/") || Path.HasExtension(path) ? Path.GetDirectoryName(path) : path;
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    private void WriteToFile(string path, string content, string message, MessageType messageType)
    {
        try
        {
            EnsureDirectoryExists(path);
            File.WriteAllText(path, content);
            SetStatus(message, messageType);
        }
        catch (Exception e)
        {
            SetStatus($"Failed to write file at '{path}'. Error: {e.Message}", MessageType.Error);
            return;
        }
        AssetDatabase.Refresh();
    }

    private void SetStatus(string message, MessageType type)
    {
        statusMessage = message;
        statusMessageType = type;
        Debug.Log(message); // Also log to console for history
    }

    #endregion
}
