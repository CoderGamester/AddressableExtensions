using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

// ReSharper disable once CheckNamespace

namespace GameLovers.AddressablesExtensions
{
	/// <summary>
	/// The asset loader to use with Addressables
	/// </summary>
	public class AddressablesAssetLoader : IAssetLoader, ISceneLoader
	{
		/// <inheritdoc />
		public async Task<T> LoadAssetAsync<T>(string path)
		{			
			var operation = Addressables.LoadAssetAsync<T>(path);

			await operation.Task;

			if (operation.Status != AsyncOperationStatus.Succeeded)
			{
				throw operation.OperationException;
			}
			
			return operation.Result;
		}

		/// <inheritdoc />
		public async Task<GameObject> InstantiatePrefabAsync(string path, Transform parent, bool instantiateInWorldSpace)
		{
			return await InstantiatePrefabAsync(path, new InstantiationParameters(parent, instantiateInWorldSpace));
		}

		/// <inheritdoc />
		public async Task<GameObject> InstantiatePrefabAsync(string path, Vector3 position, Quaternion rotation, Transform parent)
		{
			return await InstantiatePrefabAsync(path, new InstantiationParameters(position, rotation, parent));
		}

		/// <inheritdoc />
		public void UnloadAsset<T>(T asset)
		{
			Addressables.Release(asset);

			var gameObject = asset as GameObject;
			if (gameObject != null)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
		}

		/// <inheritdoc />
		public async Task<Scene> LoadSceneAsync(string path, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true)
		{
			var operation = Addressables.LoadSceneAsync(path, loadMode, activateOnLoad);
		
			await operation.Task;

			if (operation.Status != AsyncOperationStatus.Succeeded)
			{
				throw operation.OperationException;
			
			}
		
			return operation.Result.Scene;

		}

		/// <inheritdoc />
		public async Task UnloadSceneAsync(Scene scene)
		{
			var operation = SceneManager.UnloadSceneAsync(scene);

			await AsyncOperation(operation);
		}

		private async Task AsyncOperation(AsyncOperation operation)
		{
			while (!operation.isDone)
			{
				await Task.Delay(100);
			}
		}
	
		private async Task<GameObject> InstantiatePrefabAsync(string path, InstantiationParameters instantiateParameters = new InstantiationParameters())
		{
			var operation = Addressables.InstantiateAsync(path, instantiateParameters);

			await operation.Task;

			if (operation.Status != AsyncOperationStatus.Succeeded)
			{
				throw operation.OperationException;
			}
			
			return operation.Result;
		}
	}
}