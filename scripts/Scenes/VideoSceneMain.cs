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

using UnityEngine;

/**
 * Main controller script scenes that play back video.
 * 
 * @author Jean-Philippe Steinmetz
 */
public class VideoSceneMain : SceneMain
{
	/**
	 * The name of the global variable to check in order to automatically skip the movie.
	 */
	public string autoSkipGlobalVarName = null;
	
	/**
	 * The name of the next scene to load when auto skip is enabled.
	 */
	public string nextScene = null;
	
	/**
	 * Should the scene loader be shown?
	 */
	public bool showSceneLoader = true;
	
	protected override void Awake()
	{
		base.Awake();
		
		// Do we want to automatically skip the scene?
		if(autoSkipGlobalVarName != null)
		{
			object variable = GlobalVars.GetVariable(autoSkipGlobalVarName);
			if (variable != null && (bool)variable)
			{
				Debug.Log("Skipping!");
				Scene.ChangeScene(nextScene, showSceneLoader);
				return;
			}
		}
		
		// Disable the background music
		BackgroundMusicController.Stop();
	}
}
