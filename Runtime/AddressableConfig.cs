using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

// ReSharper disable once CheckNamespace

namespace GameLovers.AssetsImporter
{
	/// <summary>
	/// Represents a configuration of an Addressable with all it's important data
	/// The Id is the int representation of the AddressableId generated by the AddressableIdsGenerator code generator
	/// </summary>
	[Serializable]
	public class AddressableConfig
	{
		public int Id;
		public string Address;
		public string Path;
		public string AssetFileType;
		public Type AssetType;
		public ReadOnlyCollection<string> Labels;

		public AddressableConfig(int id, string address, string path, Type assetType, string[] labels)
		{
			Id = id;
			Address = address;
			Path = path;
			AssetFileType = path.Substring(path.LastIndexOf('.') + 1);
			AssetType = assetType;
			Labels = new ReadOnlyCollection<string>(labels);
		}

		/// <summary>
		/// If this <see cref="AddressableConfig"/> is a config from a scene, it will returns the scene name.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// Thrown if this <see cref="AddressableConfig"/> is not a config from a scene
		/// </exception>
		public string GetSceneName()
		{
			if (AssetType != typeof(UnityEngine.SceneManagement.Scene))
			{
				throw new InvalidOperationException($"This {nameof(AddressableConfig)} is not of a " +
													$"{typeof(UnityEngine.SceneManagement.Scene)} config type. It's of {AssetType.Name} type.");
			}

			var index = Address.LastIndexOf("/", StringComparison.Ordinal);
			var typeIndex = Address.LastIndexOf(".", StringComparison.Ordinal);

			index = index < 0 ? 0 : index + 1;
			typeIndex = typeIndex < index ? Address.Length : typeIndex;

			return Address.Substring(index, typeIndex - index);
		}
	}

	/// <summary>
	/// Avoids boxing for Dictionary
	/// </summary>
	public class AddressableConfigComparer : IEqualityComparer<AddressableConfig>
	{
		/// <inheritdoc />
		public bool Equals(AddressableConfig x, AddressableConfig y)
		{
			return x.Id == y.Id;
		}

		/// <inheritdoc />
		public int GetHashCode(AddressableConfig config)
		{
			return config.Id;
		}
	}
}