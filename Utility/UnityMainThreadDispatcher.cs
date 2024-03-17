/*
Copyright 2015 Pim de Witte All Rights Reserved.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;

namespace GrimbaHack.Utility;

/// Author: Pim de Witte (pimdewitte.com) and contributors, https://github.com/PimDeWitte/UnityMainThreadDispatcher
/// <summary>
/// A thread-safe class which holds a queue with actions to execute on the next Update() method. It can be used to make calls to the main thread for
/// things such as UI Manipulation in Unity. It was developed for use in combination with the Firebase Unity plugin, which uses separate threads for event handling
/// </summary>
public class UnityMainThreadDispatcher : MonoBehaviour {

	private static readonly Queue<Action> ExecutionQueue = new();

	public void Update() {
		lock(ExecutionQueue) {
			while (ExecutionQueue.Count > 0) {
				ExecutionQueue.Dequeue().Invoke();
			}
		}
	}

	/// <summary>
	/// Locks the queue and adds the IEnumerator to the queue
	/// </summary>
	/// <param name="action">IEnumerator function that will be executed from the main thread.</param>
	[HideFromIl2Cpp]
	public void Enqueue(IEnumerator action) {
		lock (ExecutionQueue) {
			ExecutionQueue.Enqueue (() => {
				StartCoroutine(action.WrapToIl2Cpp());
			});
		}
	}

	/// <summary>
	/// Locks the queue and adds the Action to the queue
	/// </summary>
	/// <param name="action">function that will be executed from the main thread.</param>
	[HideFromIl2Cpp]
	public void Enqueue(Action action)
	{
		Enqueue(ActionWrapper(action));
	}

	/// <summary>
	/// Locks the queue and adds the Action to the queue, returning a Task which is completed when the action completes
	/// </summary>
	/// <param name="action">function that will be executed from the main thread.</param>
	/// <returns>A Task that can be awaited until the action completes</returns>
	[HideFromIl2Cpp]
	public Task EnqueueAsync(Action action)
	{
		var tcs = new TaskCompletionSource<bool>();

		void WrappedAction() {
			try
			{
				action();
				tcs.TrySetResult(true);
			} catch (Exception ex)
			{
				tcs.TrySetException(ex);
			}
		}

		Enqueue(ActionWrapper(WrappedAction));
		return tcs.Task;
	}

	[HideFromIl2Cpp]
	private IEnumerator ActionWrapper(Action a)
	{
		a();
		yield return null;
	}

	private static UnityMainThreadDispatcher _instance = null;

	public static bool Exists() {
		return _instance != null;
	}

	public static UnityMainThreadDispatcher Instance
	{
		get
		{
			if (!Exists ()) {
				throw new Exception ("UnityMainThreadDispatcher could not find the UnityMainThreadDispatcher object. Please ensure you have added the MainThreadExecutor Prefab to your scene.");
			}
			return _instance;
		}
	}

	private void Awake() {
		lock (ExecutionQueue)
		{
		}
		if (_instance == null) {
			_instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
	}

	private void OnDestroy() {
		_instance = null;
	}
}