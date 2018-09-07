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
 * GUITrigger is a base class component that responds to various events to provide out of the box functionality for
 * game objects. Events may originate from the {@link GUIInputManager}, the GUIObject itself, or from changes to
 * variables stored in GlobalVars. GUITrigger triggers must be placed on a game object that contains a
 * GUIObject script.
 * 
 * @author Jean-Philippe Steinmetz
 */
[RequireComponent(typeof(GUIObject))]
public abstract class GUITrigger : MonoBehaviour
{
	/**
	 * Indicates if the trigger will respond to OnAnimStart events.
	 */
	public bool activateOnAnimStart = false;
	
	/**
	 * Indicates if the trigger will respond to OnAnimEnd events.
	 */
	public bool activateOnAnimEnd = false;
	
	/**
	 * Indicates if the trigger will respond to OnPress events.
	 */
	public bool activateOnPress = false;
	
	/**
	 * Indicates if the trigger will respond to OnRelease events.
	 */
	public bool activateOnRelease = false;
	
	/**
	 * Indicates if the trigger will respond to OnRollOver events.
	 */
	public bool activateOnRollOver = false;
	
	/**
	 * Indicates if the trigger will respond to OnRollOut events.
	 */
	public bool activateOnRollOut = false;
	
	/**
	 * Indicates if the trigger will respond to changes to a GlobalVar variable.
	 */
	public bool activateOnGlobalVar = false;
	
	/**
	 * The name of the GlobalVar variable to watch for changes.
	 */
	public string globalVarName = null;
	
	/**
	 * The value of the GlobalVar variable that causes the trigger to activate.
	 */
	public string globalVarValue = null;
	
	/**
	 * The name of the animation to activate on.
	 */
	public string animName = "";
	
	/**
	 * The GUI object that the trigger is attached to.
	 */
	protected GUIObject guiObject = null;
	
	/**
	 * The last value of the GlobalVar variable that was checked.
	 */
	private string lastGlobalVarValue = null;
	
	protected bool enabled = true;
	
	protected virtual void Awake()
	{
		guiObject = (GUIObject) GetComponent<GUIObject>();
		
		// We want the desired value to be case-insensitive for easy comparison
		if(globalVarValue != null)
			globalVarValue = globalVarValue.ToLower();
	}
	
	protected virtual void Start()
	{
		// Register the object for each of the events specified as active
		if (activateOnPress)
		{
			GUIInputManager.Register(guiObject, GUIEventType.PRESS);
		}
		if (activateOnRelease)
		{
			GUIInputManager.Register(guiObject, GUIEventType.RELEASE);
		}
		if (activateOnRollOut)
		{
			GUIInputManager.Register(guiObject, GUIEventType.ROLL_OUT);
		}
		if (activateOnRollOver)
		{
			GUIInputManager.Register(guiObject, GUIEventType.ROLL_OVER);
		}
		
		// Immediately activate on global var if we can
		if (activateOnGlobalVar)
		{
			// Only fire OnActivate() for GlobalVar events if no other activation modes are set.
			if (!activateOnAnimEnd && !activateOnAnimStart && !activateOnPress && !activateOnRelease &&
			    !activateOnRollOut && !activateOnRollOver && CheckGlobalVar())
			{
				OnActivate();
			}
		}
	}
	
	protected virtual void OnDestroy()
	{
		// Unregister the object for each of the events specified as active
		if (activateOnPress)
		{
			GUIInputManager.UnRegister(guiObject, GUIEventType.PRESS);
		}
		if (activateOnRelease)
		{
			GUIInputManager.UnRegister(guiObject, GUIEventType.RELEASE);
		}
		if (activateOnRollOut)
		{
			GUIInputManager.UnRegister(guiObject, GUIEventType.ROLL_OUT);
		}
		if (activateOnRollOver)
		{
			GUIInputManager.UnRegister(guiObject, GUIEventType.ROLL_OVER);
		}
	}
	
	/**
	 * Returns true if the GlobalVar event is ready to be activated, otherwise returns false.
	 * 
	 * @param ignoreOldValue Set to true to fire the event even if the GlobalVar value has not changed.
	 */
	private bool CheckGlobalVar(bool ignoreOldValue = false)
	{
		// The OnGlobalVar event only occurs if the variable changes from a non-target value to the target value.
		object globalVar = GlobalVars.GetVariable(globalVarName);
		string newValue = globalVar != null ? globalVar.ToString() : null;
		
		// Make sure we're comparing without case sensitivity
		if (newValue != null)
		{
			newValue = newValue.ToLower();
		}
		
		if (globalVarValue == newValue && (lastGlobalVarValue != newValue || ignoreOldValue))
		{
			lastGlobalVarValue = newValue;
			return true;
		}
		
		return false;
	}
	
	/**
	 * Called when the trigger has been activated due to a GUI event.
	 */
	protected abstract void OnActivate();
	
	/**
	 * Called when the GUI object has started it's animation. By default, calls OnActive().
	 */
	protected virtual void OnAnimStart(string name)
	{
		if (!activateOnAnimStart) return;
		if (animName != null && animName.Length > 0 && animName != name) return;
		
		// If GlobalVar activation is required, do not fire unless we've hit it
		if (activateOnGlobalVar	&& !CheckGlobalVar(true)) return;
		
		OnActivate();
	}
	
	/**
	 * Called when the GUI object has finished it's animation. By defualt, calls OnActivate().
	 */
	protected virtual void OnAnimEnd(string name)
	{
//		Debug.Log("OnAnimEnd: " + name + " ?= " + animName );
		
		if (!activateOnAnimEnd) return;
		if (animName != name) return;
		
		// If GlobalVar activation is required, do not fire unless we've hit it
		if (activateOnGlobalVar	&& !CheckGlobalVar(true)) return;
		
		OnActivate();
	}
	
	/**
	 * Called when the GUI object has been pressed. By default, calls OnActivate().
	 */
	protected virtual void OnPress()
	{
		if (!activateOnPress) return;
		
		// If GlobalVar activation is required, do not fire unless we've hit it
		if (activateOnGlobalVar	&& !CheckGlobalVar(true)) return;
		
		if(enabled) 
		{
			OnActivate();
		}
	}
	
	/**
	 * Called when the GUI object has been released. By default, calls OnActivate().
	 */
	protected virtual void OnRelease()
	{
		if (!activateOnRelease) return;
		
		// If GlobalVar activation is required, do not fire unless we've hit it
		if (activateOnGlobalVar	&& !CheckGlobalVar(true)) return;
		
		if(enabled) 
		{
			OnActivate();
		}
	}
	
	/**
	 * Called when the GUI object has been rolled out. By default, calls OnActivate().
	 */
	protected virtual void OnRollOut()
	{
		if (!activateOnRollOut) return;
		
		// If GlobalVar activation is required, do not fire unless we've hit it
		if (activateOnGlobalVar	&& !CheckGlobalVar(true)) return;
		
		OnActivate();
	}
	
	/**
	 * Called when the GUI object has been rolled over. By default, calls OnActivate().
	 */
	protected virtual void OnRollOver()
	{
		if (!activateOnRollOver) return;
		
		// If GlobalVar activation is required, do not fire unless we've hit it
		if (activateOnGlobalVar	&& !CheckGlobalVar(true)) return;
		
		OnActivate();
	}
	
	protected virtual void Update()
	{
		if (activateOnGlobalVar)
		{
			// Only fire OnActivate() for GlobalVar events if no other activation modes are set.
			if (!activateOnAnimEnd && !activateOnAnimStart && !activateOnPress && !activateOnRelease &&
			    !activateOnRollOut && !activateOnRollOver && CheckGlobalVar())
			{
				if(enabled) 
				{
					OnActivate();
				}
			}
		}
		
		// If at any point the last GlobalVar value changes from our last recorded copy, clear it out
		// so that the trigger can fire again.
		object globalVar = GlobalVars.GetVariable(globalVarName);
		string newValue = globalVar != null ? globalVar.ToString() : null;
		
		// Make sure we're comparing without case sensitivity
		if (newValue != null)
		{
			newValue = newValue.ToLower();
		}
		
		if (newValue != lastGlobalVarValue)
		{
			lastGlobalVarValue = null;
		}
	}
	
	public void Enable()
	{
		enabled = true;
	}
	
	public void Disable()
	{
		enabled = false;
	}
}
