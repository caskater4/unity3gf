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
 * Scene provides properties and methods for the management of scenes within the game. The primary purpose
 * of the class is to provide global access for changing the currently loaded scene via the ChangeScene
 * method. When specified, an intermediate scene may be displayed during the loading of a new scene. This
 * intermediate scene can be set via the LoadingScene property.
 * <p>
 * In order for this class to function properly the Loader property must be set. It is expected that a
 * SceneLoader instance will set this property at the beginning of the game's runtime before the first
 * call to ChangeScene is made.
 * <p>
 * Note that the class contains only static properties and methods and should never be instantiated.
 * 
 * @author Jean-Philippe Steinmetz
 */
public class Scene
{
	/**
	 * The name of the current scene running within the application.
	 */
	public static string CurrentScene
	{
		get;
		private set;
	}
	
	/**
	 * The name of the scene to display during loading of new scenes.
	 */
	public static string LoadingScene
	{
		get
		{
			return (string) GlobalVars.GetVariable("loadingScene");
		}
		set
		{
			GlobalVars.SetVariable("loadingScene", value);
		}
	}
	
	/**
	 * The name of the next scene currently being loaded.
	 */
	public static string NextScene
	{
		get;
		private set;
	}
	
	/**
	 * The script responsible for the actual work of scene loading.
	 */
	public static SceneLoader Loader
	{
		get;
		set;
	}
	
	/**
	 * Throws an Exception as the class should never be instantiated.
	 */
	public Scene()
	{
		throw new Exception("This class should not be instantiated");
	}
	
	/**
	 * Attempts to load and switch to the specific scene. A loading scene is displayed while the scene is being
	 * loaded.
	 * 
	 * @param scene The name of the scene to load and switch to.
	 */
	public static void ChangeScene(string scene)
	{
		if (scene == null || scene.Length <= 0) return;
		
		ChangeScene(scene, true);
	}
	
	/**
	 * Attempts to load and switch to the specific scene. A loading scene is display if showLoadScene is set to true,
	 * otherwise the current scene remains until the new scene is ready.
	 * 
	 * @param scene The name of the scene to load and switch to.
	 * @param showLoadScene Set to true to display an intermediate scene while waiting for the scene to load, otherwise
	 * set to false.
	 */
	public static void ChangeScene(string scene, bool showLoadScene)
	{
		if (scene == null || scene.Length <= 0) return;
		
		// Set the next scene
		NextScene = scene;
		
		// Call the loader
		Loader.Load(scene, showLoadScene);
	}
}
