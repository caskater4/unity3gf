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
 * ControlAnimSpriteGUITrigger is a type of {@link GUITrigger} that when activated starts, pauses, stops, or restarts,
 * playback of the {@link AnimatedSprite} attached to the game object.
 *
 * @author Jean-Philippe Steinmetz
 */
[AddComponentMenu("GUI/Triggers/Control Anim Sprite")]
public class ControlAnimSpriteGUITrigger : GUITrigger
{
	/**
	 * The different actions of control behavior the trigger can do.
	 */
	public enum ControlActions
	{
		PLAY,
		PAUSE,
		STOP,
		RESTART,
		GOTO
	}
	
	/**
	 * The control action to invoke on the attached AnimatedSprite.
	 */
	public ControlActions action = ControlActions.PLAY;
	
	/**
	 * Sets the playBackgrounds flag on the targeted sprite.
	 */
	public bool playBackwards = false;
	
	/**
	 * The time, in seconds, to set the playhead when using the GOTO control action.
	 */
	public float gotoPosition = 0.0f;
	
	/**
	 * The list of target game objects to control.
	 */
	public GameObject[] targets = null;
	
	/**
	 * Starts playback of the animated sprite.
	 */
	protected override void OnActivate()
	{
		if (targets == null || targets.Length == 0)
		{
			AnimatedSprite sprite = GetComponent<AnimatedSprite>();
			
			if (sprite != null)
			{
				OnActivate(sprite);
			}
			else
			{
				Debug.LogError(this.gameObject.name + " is not an AnimatedSprite!");
			}
		}
		else
		{
			foreach (GameObject go in targets)
			{
				AnimatedSprite sprite = go.GetComponent<AnimatedSprite>();
			
				if (sprite != null)
				{
					OnActivate(sprite);
				}
				else
				{
					Debug.LogError(go.name + " is not an AnimatedSprite!");
				}
			}
		}
	}
	
	private void OnActivate(AnimatedSprite sprite)
	{
		if (sprite == null) return;
		
		sprite.playBackwards = playBackwards;
		
		switch (action)
		{
		case ControlActions.PAUSE:		sprite.Pause(); break;
		case ControlActions.PLAY:		sprite.Play(); break;
		case ControlActions.RESTART:	sprite.Restart(); break;
		case ControlActions.STOP:		sprite.Stop(); break;
		case ControlActions.GOTO:		sprite.Playhead = gotoPosition; break;
		default:						Debug.LogError("Action not supported: " + action); break;
		}
	}
}
