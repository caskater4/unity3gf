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
 * SwapDepthsGUITrigger is a type of {@link GUITrigger} that when activated swaps depths with a target GUI object.
 * This has the effect of causing the object to be drawn on above or below the target object if the depth was initially
 * a higher value (or lower).
 * 
 * @author Jean-Philippe Steinmetz
 */
public class SwapDepthsGUITrigger : GUITrigger
{
	/**
	 * The target game object to swap depths with.
	 */
	public GameObject target = null;
	
	protected override void OnActivate()
	{
		if (target == null) return;
		
		GUIObject guiObject = GetComponent<GUIObject>();
		if (guiObject == null)
		{
			Debug.LogError(this.name + " is not a GUI object!");
			return;
		}
		
		GUIObject targetGuiObject = target.GetComponent<GUIObject>();
		if (targetGuiObject == null)
		{
			Debug.LogError(target.name + " is not a GUI object!");
			return;
		}
		
		int tmp = guiObject.Depth;
		guiObject.Depth = targetGuiObject.Depth;
		targetGuiObject.Depth = tmp;
	}
}