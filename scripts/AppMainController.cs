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
using System.Collections;

/**
 * AppMainController is a script that controls the initial execution of the application. It is intended to be applied
 * to the main scene.
 * 
 * @author Jean-Philippe Steinmetz
 */
public class AppMainController : MonoBehaviour
{
	/**
	 * The name of the scene to load at the start.
	 */
	public string defaultScene = null;
	
	/**
	 * The name of the scene to display while loading other scenes.
	 */
	public string loaderScene = null;
	
	/**
	 * Indicates whether the loader scene should be displayed.
	 */
	public bool showLoadScene = true;
	
	/**
	 * The target screen resolution UI elements should be scaled with respect to. Default value is {0,0} which will
	 * take the native screen resolution at runtime (does no scaling).
	 */
	public Vector2 targetScreenSize = Vector2.zero;
	
	private void Start()
	{
		Scene.LoadingScene = loaderScene;		
		
		Scene.ChangeScene(defaultScene, showLoadScene);
		
		if (targetScreenSize != Vector2.zero)
		{
			GlobalVars.SetVariable("targetScreenSize", targetScreenSize);
		}
	}
}
