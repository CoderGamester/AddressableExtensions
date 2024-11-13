using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

// ReSharper disable CheckNamespace

namespace GameLovers.AssetsImporter
{
	/// <summary>
	/// This interface allows to wrap the scene loading scheme into an object reference
	/// </summary>
	public interface ISceneLoader
	{
		/// <summary>
		/// Loads any scene in the given <paramref name="path"/> with the given parameter configuration.
		/// To help the execution of this method is recommended to request the scene path from an <seealso cref="AddressableConfig"/>.
		/// Invokes <paramref name="onCompleteCallback"/> when the scene is loaded.
		/// This method can be controlled in an async method and returns the asset loaded
		/// </summary>
		UniTask<Scene> LoadSceneAsync(string path, LoadSceneMode loadMode = LoadSceneMode.Single, 
			bool activateOnLoad = true, Action<Scene> onCompleteCallback = null);

		/// <summary>
		/// Unloads the given <paramref name="scene"/> from the game memory.
		/// Invokes <paramref name="onCompleteCallback"/> when the scene is loaded.
		/// This method can be controlled in an async method
		/// </summary>
		UniTask UnloadSceneAsync(Scene scene, Action onCompleteCallback = null);
	}
}