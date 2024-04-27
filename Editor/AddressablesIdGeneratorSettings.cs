using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace

namespace GameLoversEditor.AssetsImporter
{
	/// <summary>
	/// Scriptable Object with the Addressables Id Generator settings
	/// </summary>
	[CreateAssetMenu(fileName = "AddressablesIdGenerator Settings", menuName = "ScriptableObjects/Editor/AddressablesIdGenerator")]
	public class AddressablesIdGeneratorSettings : ScriptableObject
	{
		public string ScriptFilename = "AddressableId";
		public string Namespace = "Game.Ids";
		public string AddressableLabel = "GenerateIds";

		[MenuItem("Tools/AddressableIds Generator/Select Settings.asset")]
		public static AddressablesIdGeneratorSettings SelectSheetImporter()
		{
			var settings = AssetDatabase.FindAssets($"t:{nameof(AddressablesIdGeneratorSettings)}");
			var scriptableObject = settings.Length > 0 ?
									   AssetDatabase.LoadAssetAtPath<AddressablesIdGeneratorSettings>(AssetDatabase.GUIDToAssetPath(settings[0])) :
									   CreateInstance<AddressablesIdGeneratorSettings>();

			if (settings.Length == 0)
			{
				AssetDatabase.CreateAsset(scriptableObject, $"Assets/{nameof(AddressablesIdGeneratorSettings)}.asset");
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}

			Selection.activeObject = scriptableObject;

			return scriptableObject;
		}
	}
}