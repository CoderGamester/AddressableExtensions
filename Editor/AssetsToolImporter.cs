using GameLovers.AssetsImporter;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

// ReSharper disable once CheckNamespace

namespace GameLoversEditor.AssetsImporter
{
	/// <summary>
	/// Customizes the visual inspector of the importing tool <seealso cref="AssetsImporter"/>
	/// </summary>
	[CustomEditor(typeof(AssetsImporter))]
	public class AssetsToolImporter : Editor
	{
		private const string TOGGLE_PATH = "Tools/Assets Importer/Toggle Auto Import On Refresh";

		private static List<ImportData> _importers;
		
		private void Awake()
		{
			if (AssetsImporter.AutoUpdateOnRefresh)
			{
				_importers = GetAllImporters();
			}
		}
		
		[DidReloadScripts]
		public static void OnCompileScripts()
		{
			if(AssetsImporter.AutoUpdateOnRefresh)
			{
				_importers = GetAllImporters();
			}
		}

		[MenuItem(TOGGLE_PATH)]
		private static void ToggleAutoImport()
		{
			AssetsImporter.AutoUpdateOnRefresh = !AssetsImporter.AutoUpdateOnRefresh;
			Menu.SetChecked(TOGGLE_PATH, AssetsImporter.AutoUpdateOnRefresh);
		}

		[MenuItem(TOGGLE_PATH, true)]
		private static bool ValidateAutoImport()
		{
			Menu.SetChecked(TOGGLE_PATH, AssetsImporter.AutoUpdateOnRefresh);
			return true;
		}

		[MenuItem("Tools/Assets Importer/Get All Importers")]
		private static void UpdateAllImporters()
		{
			_importers = GetAllImporters();

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		[MenuItem("Tools/Assets Importer/Import Assets Data")]
		private static void ImportAllAssetsData()
		{
			_importers = GetAllImporters();
			
			foreach (var importer in _importers)
			{
				importer.Importer.Import();
			}
			
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
		
		/// <inheritdoc />
		public override void OnInspectorGUI()
		{
			var typeCheck = typeof(IAssetConfigsImporter);
			var tool = (AssetsImporter) target;

			AssetsImporter.AutoUpdateOnRefresh = GUILayout.Toggle(AssetsImporter.AutoUpdateOnRefresh, "Toggle Auto Update on Refresh (Post Script Compilation)");
			EditorGUILayout.HelpBox("Click on the 'Import All Importers' if you see any importer missing", MessageType.Info);

			if (GUILayout.Button("Update All Importers"))
			{
				UpdateAllImporters();
			}

			if (_importers == null)
			{
				// Not yet initialized. Will initialized as soon has all scripts finish compiling
				return;
			}

			if (GUILayout.Button("Import Assets Data"))
			{
				foreach (var importer in _importers)
				{
					importer.Importer.Import();
				}
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}

			foreach (var importer in _importers)
			{
				EditorGUILayout.Space();
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(importer.Type.Name, EditorStyles.boldLabel);

				if (GUILayout.Button(string.IsNullOrEmpty(importer.AssetsFolderPath) ? "Set Path" : "Update Path"))
				{
					var path = EditorUtility.OpenFolderPanel("Select Folder Path", Application.dataPath, "");

					if(string.IsNullOrEmpty(path))
					{
						return;
					}

					path = path.Substring(path.IndexOf("Assets/", StringComparison.Ordinal));
					importer.AssetsFolderPath = path;

					importer.Importer.Import(path);
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
					Repaint();
				}
				
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.LabelField(string.IsNullOrEmpty(importer.AssetsFolderPath) ? "< NO PATH SET>" : importer.AssetsFolderPath);
				EditorGUILayout.BeginHorizontal();
				
				if (importer.Importer is IAssetConfigsGeneratorImporter)
				{
					EditorGUILayout.LabelField("This is a Code Generator Importer. Click on 'Set Path' to generate the missing scripts", EditorStyles.boldLabel);
				}
				else
				{
					if(GUILayout.Button("Import"))
					{
						importer.Importer.Import();
						AssetDatabase.SaveAssets();
						AssetDatabase.Refresh();
					}
					if (typeCheck.IsAssignableFrom(importer.Type) && GUILayout.Button("Select Object") &&
						TryGetScriptableObject(importer.Importer.ScriptableObjectType, out var selectedObject))
					{
						Selection.activeObject = selectedObject;
					}
				}
				
				EditorGUILayout.EndHorizontal();
			}
		}
		
		private static List<ImportData> GetAllImporters()
		{
			var importerInterface = typeof(IAssetConfigsImporter);
			var importers = new List<ImportData>();
			
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (var type in assembly.GetTypes())
				{
					if (!type.IsAbstract && !type.IsInterface && importerInterface.IsAssignableFrom(type))
					{
						var importer = Activator.CreateInstance(type) as IAssetConfigsImporter;

						TryGetScriptableObject(importer.ScriptableObjectType, out var scriptableObject);
						importers.Add(new ImportData
						{
							Type = type,
							Importer = importer,
							AssetsFolderPath = scriptableObject?.AssetsFolderPath
						});
					}
				}
			}

			return importers;
		}

		private static bool TryGetScriptableObject(Type type, out AssetConfigsScriptableObject scriptableObject)
		{
			var assets = AssetDatabase.FindAssets($"t:{type?.Name}");
			var obj = assets.Length > 0 ?
				AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assets[0]), type) :
				CreateInstance(type);

			if (obj == null)
			{
				scriptableObject = null;

				return false;
			}

			scriptableObject = obj as AssetConfigsScriptableObject;

			if (assets.Length == 0 && type != null)
			{
				AssetDatabase.CreateAsset(scriptableObject, $"Assets/{type.Name}.asset");
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}

			return true;
		}

		private class ImportData
		{
			public Type Type;
			public IAssetConfigsImporter Importer;
			public string AssetsFolderPath;
		}
	}
}