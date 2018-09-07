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
 * DestroyObjectGUITrigger is a type of {@link GUITrigger} that destroys the instances from a list of specified
 * game objects when the set event types are triggered. If no objects are listed the trigger will destroy the
 * owner instead.
 * 
 * @author Jean-Philippe Steinmetz
 */
[AddComponentMenu("GUI/Triggers/Destroy Object(s)")]
public class DestroyObjectGUITrigger : GUITrigger
{
	/**
	 * The list of game objects to destroy.
	 */
	public GameObject[] gameObjects = null;
	
	protected override void OnActivate()
	{
		if (gameObjects != null && gameObjects.Length > 0)
		{
			foreach (GameObject go in gameObjects)
			{
				if (go == null) continue;
				Debug.Log("Destroying: " + go.name);
				Destroy(go);
			}
		}
		else
		{
			// If no objects were specified, assume we meant ourself.
			Debug.Log("Destroying: " + this.gameObject.name);
			Destroy(this.gameObject);
		}
	}
}