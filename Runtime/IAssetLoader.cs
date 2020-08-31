using System.Threading.Tasks;
using UnityEngine;

// ReSharper disable CheckNamespace

namespace GameLovers.AddressablesExtensions
{
	/// <summary>
	/// This interface allows to wrap the asset loading scheme into an object reference
	/// </summary>
	public interface IAssetLoader
	{
		/// <summary>
		/// Loads any asset of the given <typeparamref name="T"/> in the given <paramref name="path"/>.
		/// To help the execution of this method is recommended to request the asset path from an <seealso cref="AddressableConfig"/>.
		/// This method can be controlled in an async method and returns the asset loaded
		/// </summary>
		Task<T> LoadAssetAsync<T>(string path);

		/// <summary>
		/// Loads and instantiates the prefab in the given <paramref name="path"/> with the given <paramref name="parent"/>
		/// and the given <paramref name="instantiateInWorldSpace"/> to preserve the instance transform relative to world
		/// space or relative to the parent.
		/// To help the execution of this method is recommended to request the asset path from an <seealso cref="AddressableConfig"/>.
		/// This method can be controlled in an async method and returns the prefab instantiated
		/// </summary>
		Task<GameObject> InstantiatePrefabAsync(string path, Transform parent, bool instantiateInWorldSpace);

		/// <summary>
		/// Loads and instantiates the prefab in the given <paramref name="path"/> with the given <paramref name="position"/>,
		/// the given <paramref name="rotation"/> & the given <paramref name="parent"/>.
		/// To help the execution of this method is recommended to request the asset path from an <seealso cref="AddressableConfig"/>.
		/// This method can be controlled in an async method and returns the prefab instantiated
		/// </summary>
		Task<GameObject> InstantiatePrefabAsync(string path, Vector3 position, Quaternion rotation, Transform parent);

		/// <summary>
		/// Unloads the given <paramref name="asset"/> from the game memory.
		/// If <typeparamref name="T"/> is of <seealso cref="GameObject"/> type, then will also destroy it
		/// </summary>
		void UnloadAsset<T>(T asset);
	}
}