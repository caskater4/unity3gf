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
 * HideObjectGUITrigger is a type of {@link GUITrigger} that when activated sets the Visible property of the
 * target's GUIObject component to false. This has the effect of deactivating the associated GameObjects.
 * 
 * @author Jean-Philippe Steinmetz
 */
[AddComponentMenu("GUI/Triggers/Hide Object(s)")]
public class HideObjectGUITrigger : ToggleVisibleGUITrigger
{
	/**
	 * Sets the visible property of each target game object to false.
	 */
	protected override void OnActivate()
	{
		// If no targets have been selected, assume we meant ourself
		if (targets == null || targets.Length == 0)
		{
			//Debug.Log("Hiding " + this.gameObject.name);
			GUIObject targetObj = (GUIObject) GetComponent<GUIObject>();
			targetObj.Visible = false;
			return;
		}
		
		foreach (GameObject target in targets)
		{
			if (target == null) continue;
			
			//Debug.Log("Hiding " + target.name);
			GUIObject targetObj = (GUIObject) target.GetComponent<GUIObject>();
			targetObj.Visible = false;
		}
	}
}