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
		

			/* AssetReference types
			
				GameObject
				ScriptableObject
				Texture
				Texture3D
				Texture2D
				RenderTexture
				CustomRenderTexture
				CubeMap
				Material
				PhysicMaterial
				PhysicMaterial2D
				Sprite
				SpriteAtlas
				VideoClip
				AudioClip
				AudioMixer
				Avatar
				AnimatorController
				AnimatorOverrideController
				TextAsset
				Mesh
				Shader
				ComputeShader
				Flare
				NavMeshData
				TerrainData
				TerrainLayer
				Font
				Scene
				GUISkin
			 * */
			 
		/// <inheritdoc />
		public async Task<T> LoadAssetAsync<T>(object key, Action<T> onCompleteCallback = null)
		{
			var operation = Addressables.LoadAssetAsync<T>(key);

			await operation.Task;

			if (operation.Status != AsyncOperationStatus.Succeeded)
			{
				throw operation.OperationException;
			}

			onCompleteCallback?.Invoke(operation.Result);

			return operation.Result;
		}

		/// <inheritdoc />
		public async Task<GameObject> InstantiateAsync(object key, Transform parent, bool instantiateInWorldSpace, 
			Action<GameObject> onCompleteCallback = null)
		{
			var gameObject = await InstantiatePrefabAsync(key, new InstantiationParameters(parent, instantiateInWorldSpace));

			onCompleteCallback?.Invoke(gameObject);

			return gameObject;
		}

		/// <inheritdoc />
		public async Task<GameObject> InstantiateAsync(object key, Vector3 position, Quaternion rotation, Transform parent, 
			Action<GameObject> onCompleteCallback = null)
		{
			var gameObject = await InstantiatePrefabAsync(key, new InstantiationParameters(position, rotation, parent));

			onCompleteCallback?.Invoke(gameObject);

			return gameObject;
		}

		/// <inheritdoc />
		public void UnloadAsset<T>(T asset)
		{
			Addressables.Release(asset);
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
				await Task.Yield();
			}
		}

		private async Task<GameObject> InstantiatePrefabAsync(object key, InstantiationParameters instantiateParameters = new InstantiationParameters())
		{
			var operation = Addressables.InstantiateAsync(key, instantiateParameters);

			await operation.Task;

			if (operation.Status != AsyncOperationStatus.Succeeded)
			{
				throw operation.OperationException;
			}

			return operation.Result;
		}
	}
}