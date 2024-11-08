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
			_importers = GetAllImporters();
		}
		
		[DidReloadScripts]
		public static void OnCompileScripts()
		{
			if(AssetsImporter.AutoImportOnRefresh)
			{
				_importers = GetAllImporters();
			}
		}

		[MenuItem(TOGGLE_PATH)]
		private static void ToggleAutoImport()
		{
			AssetsImporter.AutoImportOnRefresh = !AssetsImporter.AutoImportOnRefresh;
			Menu.SetChecked(TOGGLE_PATH, AssetsImporter.AutoImportOnRefresh);
		}

		[MenuItem(TOGGLE_PATH, true)]
		private static bool ValidateAutoImport()
		{
			Menu.SetChecked(TOGGLE_PATH, AssetsImporter.AutoImportOnRefresh);
			return true;
		}

		[MenuItem("Tools/Assets Importer/Get All Importers")]
		private static void ImportAllImporters()
		{
			_importers = GetAllImporters();
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
		{		if (_importers == null)
			{
				// Not yet initialized. Will initialized as soon has all scripts finish compiling
				return;
			}

			var typeCheck = typeof(IScriptableObjectImporter);
			var tool = (AssetsImporter) target;

			GUILayout.Toggle(AssetsImporter.AutoImportOnRefresh, "Toggle Auto Import on Refresh (Post Script Compilation)");
			
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
				//EditorGUILayout.PrefixLabel(importer.Type.Name, GUIStyle.);
				EditorGUILayout.LabelField(importer.Type.Name, EditorStyles.boldLabel);

				if (GUILayout.Button(string.IsNullOrEmpty(importer.AssetsFolderPath) ? "Set Path" : "Update Path"))
				{
					var scriptableObject = GetScriptableObject(importer);

					var path = EditorUtility.OpenFolderPanel("Select Folder Path", scriptableObject.AssetsFolderPath,"");
					scriptableObject.AssetsFolderPath = path.Substring(path.IndexOf("Assets/", StringComparison.Ordinal));
					importer.AssetsFolderPath = scriptableObject.AssetsFolderPath;

					importer.Importer.Import();
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
				}
				
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.LabelField(string.IsNullOrEmpty(importer.AssetsFolderPath) ? "< NO PATH SET>" : importer.AssetsFolderPath);
				EditorGUILayout.BeginHorizontal();
				
				if (GUILayout.Button("Import"))
				{
					importer.Importer.Import();
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
				}
				if(typeCheck.IsAssignableFrom(importer.Type) && GUILayout.Button("Select Object"))
				{
					Selection.activeObject = GetScriptableObject(importer);
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
						importers.Add(new ImportData
						{
							Type = type,
							Importer = Activator.CreateInstance(type) as IAssetConfigsImporter
						});
					}
				}
			}

			return importers;
		}

		private static AssetConfigsScriptableObject GetScriptableObject(ImportData data)
		{
			var scriptableObjectType = data.Importer.ScriptableObjectType;
			var assets = AssetDatabase.FindAssets($"t:{scriptableObjectType?.Name}");
			var scriptableObject = assets.Length > 0 ? 
				                       AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assets[0]), scriptableObjectType) :
				                       CreateInstance(scriptableObjectType);

			if (assets.Length == 0 && scriptableObjectType != null)
			{
				AssetDatabase.CreateAsset(scriptableObject, $"Assets/{scriptableObjectType.Name}.asset");
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}

			return scriptableObject as AssetConfigsScriptableObject;
		}

		private class ImportData
		{
			public Type Type;
			public IAssetConfigsImporter Importer;
			public string AssetsFolderPath;
		}
	}
}