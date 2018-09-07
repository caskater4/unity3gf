/**
 * Copyright (c) 2011-2012, Jean-Philippe Steinmetz
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of Jean-Philippe Steinmetz nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL JEAN-PHILIPPE STEINMETZ BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections;
using UnityEngine;

/**
 * SceneLoader is a script to be attached to a game object that performs the actual work of loading new scenes.
 * The script has one public method, Load, which when called loads a new scene asynchronously. If desired, an
 * intermediate loading scene is displayed. The loading scene to display is determined by Scene.LoadingScene.
 * <p>
 * When changing scenes the SceneLoader should never be called directly. Instead, the Scene class should be used
 * to invoke change requests.
 * 
 * @author Jean-Philippe Steinmetz
 */
public class SceneLoader : MonoBehaviour
{
	private AsyncOperation loadOperation = null;
	private string nextScene = null;
	private bool readyToLoad = false;
	
	private void Start()
	{
		Scene.Loader = this;
	}
	
	/**
	 * Loads the scene with the given name, displaying the intermediate loading scene if specified.
	 * 
	 * @param scene The name of the scene to load.
	 * @param showLoadScene Set to true to display the loader while scene is being retrieved.
	 */
	public void Load(string scene, bool showLoadScene)
	{
		if (scene == null || scene.Length <= 0) return;
		
		nextScene = scene;
		
		// If we're not showing the load screen, load the scene immediately
		if (!showLoadScene)
		{
			loadOperation = Application.LoadLevelAsync(nextScene);
			return;
		}
		
		loadOperation = Application.LoadLevelAsync(Scene.LoadingScene);
		readyToLoad = true;
	}
	
	private void Update()
	{
		// Wait for the current load operationt to finish
		if (loadOperation == null || !loadOperation.isDone)
		{
			return;
		}
		
		// Clean up the load related vars when done
		if (loadOperation != null && loadOperation.isDone)
		{
			loadOperation = null;
		}
		
		// Are we loading a new scene?
		if (readyToLoad)
		{
			loadOperation = Application.LoadLevelAsync(nextScene);
			readyToLoad = false;
		}
	}
}