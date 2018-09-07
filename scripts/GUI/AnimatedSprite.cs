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
 * An AnimatedSprite is a type of {@link Sprite} that renders a sequence of textures over time to create an animation.
 * The class assumes that a sequence is a set of discrete textures stored within in an atlas. The name of each texture
 * is sequential starting from 0 such that the first file is named texture0 and the last named textureN.
 * 
 * @author Jean-Philippe Steinmetz
 */
[AddComponentMenu("GUI/Animated Sprite")]
[ExecuteInEditMode]
public class AnimatedSprite : Sprite
{
	/**
	 * Set to true to start playback immediately, otherwise set to false.
	 */
	public bool autoStart = true;
	
	/**
	 * The speed at which to render frames per second.
	 */
	public float frameRate = 30.0f;
	
	/**
	 * The number of frames in the sequence.
	 */
	public int numFrames = 1;
	
	/**
	 * The number of times the animation will loop. 0 plays the animation once, 1 plays the animation twice (looping
	 * once), and -1 loops the animation infinitely.
	 */
	public int loops = 0;
	
	/**
	 * Set to true to have the animated sprite play in reverse.
	 */
	public bool playBackwards = false;
	
	/**
	 * The base name of the texture to retrieve from the atlas.
	 */
	private string m_baseTextureName = null;
	
	/**
	 * The current frame being rendered.
	 */
	private int m_curFrame = 0;
	
	/**
	 * The number of remaining times animation will be looped.
	 */
	private int m_loopCount = 0;
	
	/**
	 * Internal variable for Playhead property. See Playhead for details.
	 */
	private float m_playhead = 0.0f;
	
	/**
	 * The length of the animation, in seconds.
	 */
	public float Duration
	{
		get;
		protected set;
	}
	
	/**
	 * Indicates if playback of the animation is currently paused.
	 */
	public bool IsPaused
	{
		get;
		protected set;
	}
	
	/**
	 * Indicates if playback of the animation is currently playing.
	 */
	public bool IsPlaying
	{
		get;
		protected set;
	}
	
	/**
	 * The position, in seconds, of the playback head.
	 */
	public float Playhead
	{
		get
		{
			return m_playhead;
		}
		set
		{
			if (value < 0)
			{
				m_playhead = 0;
			}
			else if (value > Duration)
			{
				m_playhead = Duration;
			}
			else
			{
				m_playhead = value;
			}
		}
	}
	
	protected override void Awake()
	{
		base.Awake();
		
		if (atlas == null || atlas.Length == 0)
		{
			Debug.LogError(this.gameObject.name + " requires atlas to be set.");
			return;
		}
		
		// We store the texture name so that it is easier to apply frame numbers for
		// searching later. We also want to make sure that the first frame is always
		// loaded at start.
		m_baseTextureName = texture;
		Texture = m_baseTextureName + m_curFrame;
	}
	
	protected override void Start()
	{
		base.Start();
		
		// Calculate the length of the animation
		Duration = (float) numFrames / frameRate;
		
		if (autoStart)
		{
			Play();
		}
	}
	
	/**
	 * Pauses playback of the animation.
	 */
	public void Pause()
	{
		if (!IsPlaying || IsPaused) return;
		
		IsPaused = true;
		
		// Notify any local listeners
		this.SendMessage("OnAnimPause", "AnimatedSprite", SendMessageOptions.DontRequireReceiver);
	}
	
	/**
	 * Begins playback of the animation.
	 */
	public void Play()
	{
		// Are we resuming or starting fresh?
		if (!IsPlaying)
		{
			// Reset playback properties
			m_loopCount = loops;
			Playhead = 0;
			
			// Notify any local listeners
			this.SendMessage("OnAnimStart", "AnimatedSprite", SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			// Notify any local listeners
			this.SendMessage("OnAnimResume", "AnimatedSprite", SendMessageOptions.DontRequireReceiver);
		}
		
		IsPlaying = true;
		IsPaused = false;
	}
	
	/**
	 * Stops the animation and begins playback from the beginning.
	 */
	public void Restart()
	{
		Stop();
		Play();
	}
	
	/**
	 * Stops playback of the animation, rewinding to the beginning.
	 */
	public void Stop()
	{
		IsPlaying = IsPaused = false;
		
		// Reset all playback properties
		m_loopCount = 0;
		Playhead = 0;
		
		// Notify any local listeners
		this.SendMessage("OnAnimStop", "AnimatedSprite", SendMessageOptions.DontRequireReceiver);
	}
	
	protected override void Update()
	{
		// Only advanced the playhead when we are playing and not currently paused
		if (IsPlaying && !IsPaused)
		{
			// Did we finish the sequence?
			if (Playhead + Time.deltaTime >= Duration)
			{
				// Notify any local listeners
				this.SendMessage("OnAnimEnd", "AnimatedSprite", SendMessageOptions.DontRequireReceiver);
				
				// Are we looping?
				if (m_loopCount	!= 0)
				{
					// Calculate the wrap around time of the loop
					Playhead = Playhead + Time.deltaTime - Duration;
					
					// Notify any local listeners
					this.SendMessage("OnAnimStart", "AnimatedSprite", SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					IsPlaying = IsPaused = false;
				}
				
				if (m_loopCount	> 0) m_loopCount--;
			}
			else
			{
				Playhead += Time.deltaTime;
			}
		}
		
		// Calculate the current frame
		int curFrame = (int) (Playhead * frameRate);
		
		// If we are playing backwards, invert the frame number
		if (playBackwards)
		{
			curFrame = numFrames - curFrame;
			if (curFrame < 0) curFrame *= -1;
		}
		
		// Make sure we never exceed the number of the last frame (N-1)
		if (curFrame >= numFrames) curFrame = numFrames - 1;
		
		if (curFrame != m_curFrame)
		{
			//Debug.Log("Frame: " + curFrame + " / " + numFrames);
			Texture = m_baseTextureName + curFrame;
			m_curFrame = curFrame;
		}
		
		base.Update();
	}
}
