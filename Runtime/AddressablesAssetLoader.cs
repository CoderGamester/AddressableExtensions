using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

// ReSharper disable once CheckNamespace

namespace GameLovers.AssetsImporter
{
	/// <summary>
	/// The asset loader to use with Addressables
	/// </summary>
	public class AddressablesAssetLoader : IAssetLoader, ISceneLoader
	{			 
		/// <inheritdoc />
		public async UniTask<T> LoadAssetAsync<T>(object key, Action<T> onCompleteCallback = null)
		{
			var operation = Addressables.LoadAssetAsync<T>(key);

			await operation.ToUniTask();

			if (operation.Status != AsyncOperationStatus.Succeeded)
			{
				throw operation.OperationException;
			}

			onCompleteCallback?.Invoke(operation.Result);

			return operation.Result;
		}

		/// <inheritdoc />
		public async UniTask<GameObject> InstantiateAsync(object key, Transform parent, bool instantiateInWorldSpace, 
			Action<GameObject> onCompleteCallback = null)
		{
			var gameObject = await InstantiatePrefabAsync(key, new InstantiationParameters(parent, instantiateInWorldSpace));

			onCompleteCallback?.Invoke(gameObject);

			return gameObject;
		}

		/// <inheritdoc />
		public async UniTask<GameObject> InstantiateAsync(object key, Vector3 position, Quaternion rotation, Transform parent, 
			Action<GameObject> onCompleteCallback = null)
		{
			var gameObject = await InstantiatePrefabAsync(key, new InstantiationParameters(position, rotation, parent));

			onCompleteCallback?.Invoke(gameObject);

			return gameObject;
		}

		/// <inheritdoc />
		public async UniTask UnloadAssetAsync<T>(T asset, Action onCompleteCallback = null)
		{
			Addressables.Release(asset);
			// Force a garbage collection and unload unused assets
			GC.Collect();
			GC.WaitForPendingFinalizers();

			await Resources.UnloadUnusedAssets().ToUniTask();

			onCompleteCallback?.Invoke();
		}

		/// <inheritdoc />
		public async UniTask<Scene> LoadSceneAsync(string path, LoadSceneMode loadMode = LoadSceneMode.Single, 
			bool activateOnLoad = true,	Action<Scene> onCompleteCallback = null)
		{
			var operation = Addressables.LoadSceneAsync(path, loadMode, activateOnLoad);

			await operation.ToUniTask();

			if (operation.Status != AsyncOperationStatus.Succeeded)
			{
				throw operation.OperationException;

			}

			onCompleteCallback?.Invoke(operation.Result.Scene);

			return operation.Result.Scene;

		}

		/// <inheritdoc />
		public async UniTask UnloadSceneAsync(Scene scene, Action onCompleteCallback = null)
		{
			var operation = SceneManager.UnloadSceneAsync(scene);

			await AsyncOperation(operation);

			onCompleteCallback?.Invoke();
		}

		private async UniTask AsyncOperation(AsyncOperation operation)
		{
			while (!operation.isDone)
			{
				await UniTask.Yield();
			}
		}

		private async UniTask<GameObject> InstantiatePrefabAsync(object key, 
			InstantiationParameters instantiateParameters = new InstantiationParameters())
		{
			var operation = Addressables.InstantiateAsync(key, instantiateParameters);

			await operation.ToUniTask();

			if (operation.Status != AsyncOperationStatus.Succeeded)
			{
				throw operation.OperationException;
			}

			return operation.Result;
		}
	}
}