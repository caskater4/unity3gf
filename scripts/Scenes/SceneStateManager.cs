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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * The SceneStateManager is responsible for managing the state of a scene such that it can be saved and restored. At
 * any point the Save method can be called which takes a snapshot of the current state of the scene.
 * <p>
 * A scene is saved by broadcasting the OnSave event to every game object in the scene and saving the current state of
 * GlobalVars. Any GlobalVars variable whose name starts with an underscore ("_") will not be saved.
 * 
 * @author Jean-Philippe Steinmetz
 */
public class SceneStateManager : MonoBehaviour
{
	private void Awake()
	{
		if (this.gameObject.GetComponents<SceneStateManager>().Length > 1)
		{
			Debug.LogWarning("No more than one SceneStateManager component should exist at a time.");
		}
		
		string restoreScene = (string) GlobalVars.GetVariable("_restoreScene");
		if (restoreScene != null && Application.loadedLevelName == restoreScene)
		{
			// Load all saved global vars
			Hashtable data = (Hashtable) GlobalVars.GetVariable("_lastStateData");
			if (data != null)
			{
				foreach (string key in data.Keys)
				{
					GlobalVars.SetVariable(key, data[key]);
				}
			}
			
			// Now notify all game objects of the restoration
			GameObject[] gameObjects = (GameObject[]) GameObject.FindObjectsOfType(typeof(GameObject));
			foreach(GameObject gameObj in gameObjects)
			{
				gameObj.BroadcastMessage("OnRestore", SendMessageOptions.DontRequireReceiver);
			}
			
			// Remove the restore marker
			GlobalVars.SetVariable("_restoreScene", null);
		}
	}
	
	/**
	 * Reloads the current scene, restoring the scene to the last saved state.
	 */
	public static void Restore()
	{
		Debug.Log("Restoring...");
		
		// We mark the name of the current scene so that we don't restore at the wrong time
		GlobalVars.SetVariable("_restoreScene", Application.loadedLevelName);
		
		// Reload the scene
		Scene.ChangeScene(Application.loadedLevelName, true);
	}
	
	/**
	 * Saves the current state of the scene.
	 * <p>
	 * This has the effect of broadcasting OnSave to every game object in the scene and saving the current state of
	 * GlobalVars. Any GlobalVars variable whose name starts with an underscore ("_") will not be saved.
	 */
	public static void Save()
	{
		Debug.Log("Saving...");
		
		// Notify all game objects in the scene to save their state
		GameObject[] gameObjects = (GameObject[]) GameObject.FindObjectsOfType(typeof(GameObject));
		foreach(GameObject gameObj in gameObjects)
		{
			gameObj.BroadcastMessage("OnSave", SendMessageOptions.DontRequireReceiver);
		}
		
		// Now save the current state of all global vars, except the ones that start with an underscore
		Hashtable data = new Hashtable();
		string[] varNames = GlobalVars.VariableNames;
		foreach(string varName in varNames)
		{
			if (!varName.StartsWith("_"))
			{
				data[varName] = GlobalVars.GetVariable(varName);
			}
		}
		GlobalVars.SetVariable("_lastStateData", data);
		
		Debug.Log("Save Complete!");
	}
}