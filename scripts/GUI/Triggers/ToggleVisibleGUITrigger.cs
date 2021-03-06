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
 * ToggleVisibleGUITrigger is a type of {@link GUITrigger} that when activated toggles the Visible property of the
 * target's GUIObject component. This has the effect of activating/deactivating the associated GameObject.
 * 
 * @author Jean-Philippe Steinmetz
 */
[AddComponentMenu("GUI/Triggers/Toggle Object(s) Visibility")]
[RequireComponent(typeof(GUIObject))]
public class ToggleVisibleGUITrigger : GUITrigger
{
	/**
	 * The list of GameObjects to toggle visibility of when the trigger is activated.
	 */
	public GameObject[] targets = null;
	
	protected override void Awake()
	{
		base.Awake();
		
		if (targets == null)
		{
			targets = new GameObject[]{this.gameObject};
		}
		
		foreach (GameObject target in targets)
		{
			if (target == null) continue;
			
			GUIObject targetObj = (GUIObject) target.GetComponent<GUIObject>();
			
			if (targetObj == null)
			{
				Debug.LogError(target.name + " is not a GUI object!");
			}
		}
	}
	
	/**
	 * Toggles the active property of each target game object.
	 */
	protected override void OnActivate()
	{
		if (targets == null) return;
		
		foreach (GameObject target in targets)
		{
			if (target == null) continue;
			
			GUIObject targetObj = (GUIObject) target.GetComponent<GUIObject>();
			targetObj.Visible = !targetObj.Visible;
		}
	}
}
