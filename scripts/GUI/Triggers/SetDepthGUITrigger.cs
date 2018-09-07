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
 * SetDepthGUITrigger is a type of {@link GUITrigger} that when activated sets the depth of a target GUI object. If no
 * target object is set, the trigger will assume the GUI object itself is the intended target.
 * 
 * @author Jean-Philippe Steinmetz
 */
public class SetDepthGUITrigger : GUITrigger
{
	/**
	 * The target game object to set the depth of. If null, sets the depth of itself.
	 */
	public GameObject[] targets = null;
	
	/**
	 * The depth to set on the target.
	 */
	public int depth = 0;
	
	protected override void Awake()
	{
		base.Awake();
		
		if (targets == null || targets.Length == 0)
		{
			targets = new GameObject[]{this.gameObject};
		}
	}
	
	protected override void OnActivate()
	{
        if (targets == null) return;

        foreach (GameObject target in targets)
        {
            if (target == null) continue;

            GUIObject guiObject = target.GetComponent<GUIObject>();
            if (guiObject == null)
            {
                Debug.LogError(target.name + " is not a GUI object!");
                return;
            }

            guiObject.Depth = depth;
        }
	}
}