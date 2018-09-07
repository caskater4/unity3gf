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
using UnityEngine;

/**
 * AppMain acts as the application's main entry point and should be ordered as the very first script to execute in the
 * Script Execution Order menu. The script can be safely added to any scene, however it's Awake method will only
 * execute once during the entire lifetime of the application.
 * 
 * @author Jean-Philippe Steinmetz
 */
[ExecuteInEditMode]
public class AppMain : MonoBehaviour
{
	private static bool initialized = false;
	
	private void Awake()
	{
		// Do not proceed if we have already been initialized
		if (initialized)
		{
			// If global has already been created and initialized destroy this copy
			if (Application.isPlaying)
			{
				Destroy(this.gameObject);
			}
			
			return;
		}
		
		// Prevent the Global game object from being deleted on scene change
		DontDestroyOnLoad(this.gameObject);
		
		// Init all global variables
		GlobalVars.Reset();
		
		// Initialize target framerate:
		Application.targetFrameRate = 60;
		
		// Initialize our config storage manager
		AppConfigManager.Init();
		
		if (Application.isPlaying)
		{
			// Load saved configuration from disk
			AppConfigManager.Load();
		}
		
		// Initialize the Localization system
		Localization.Init();
		
		// Initialize the asset system
		Asset.Init();
		
		initialized = true;
	}
	
	private void OnApplicationQuit()
	{
		if (Application.isPlaying)
		{
			// Save the application state
			AppConfigManager.Save();
		}
	}
}
