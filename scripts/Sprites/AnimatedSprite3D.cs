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
 * An AnimatedSprite3D is a type of {@link Sprite3D} that renders a sequence of textures over time to create an
 * animation. This makes use of the texture atlasing feature of Sprite3D and is therefore required. Subsequently,
 * the offset and scale properties are not usable as they will be overriden at runtime.
 * 
 * @author Jean-Philippe Steinmetz
 * @author Katlan Merrill
 */
public class AnimatedSprite3D : Sprite3D
{
	
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
	 * The current frame being rendered.
	 */
	private int m_curFrame = 0;
	
	/**
	 * The number of remaining times animation will be looped.
	 */
	private int m_loopCount = 0;
	
	/**
	 * The time at which the animation started.
	 */
	private float m_startTime = -1.0f;
	
	/**
	 * The total time, in seconds, the animation takes to complete.
	 */
	private float m_totalTime = 0.0f;
	
	/**
	 * Whether or not the animated sprite is playing or not.
	 * */
	private bool isPlaying = false;
	
	/**
	 * Whether or not the sprite should play on start or not
	 * */
	public bool autoStart = false;
	
	/**
	 * The time at which the last pause call was made, so resuming can resume from the right time
	 * */
	private float pauseTime = -1.0f;
	
	/**
	 * Whether or not the sprite is currently paused or not
	 * */
	private bool isPaused = false;
	
	
	/**
	 * Read only queryable value for whether or not this is playing
	 * */
	public bool IsPlaying
	{
		get 
		{
			return isPlaying;
		}
	}
	
	/**
	 * Controls whether or not the playback is paused. Same as calling Pause() for true, and Play() for false.
	 * */
	public bool IsPaused
	{
		get
		{
			return isPaused;
		}
		set
		{
			//if setting isPaused from false to true, pause
			if (value)
			{
				if (!isPaused)
				{
					Pause();
				}
			}
			//if setting isPaused from true to false, play
			else
			{
				if (isPaused)
				{
					Play();
				}
			}
		}
	}
	
	
	
	
	protected override void Start()
	{
		base.Start();
		
		if (atlas == null || atlas.Length == 0)
		{
			Debug.LogError(this.gameObject.name + " requires atlas to be set.");
			return;
		}
		
		// We store the texture name so that it is easier to apply frame numbers for
		// searching later. We also want to make sure that the first frame is always
		// loaded at start.
		m_texturePath = texture + m_curFrame;
		
		m_loopCount = loops;
		
		if (autoStart)
		{
			Restart();
		}
		
		base.Start();
	}
	
	
	
	/**
	 * Reset and Play the animated Sprite from the start
	 * */
	public void Restart()
	{
		m_curFrame = 0;
		
		m_texturePath = texture + m_curFrame;
		
		// Mark the start time
		m_startTime = Time.time;
		
		// Calculate the length of the animation
		m_totalTime = (float) numFrames / frameRate;
		
		//start playing
		isPlaying = true;
	}
	
	/**
	 * Play the animated Sprite, resume if paused, restart otherwise
	 * */
	public void Play()
	{
		if (isPaused)
		{
			//resume from when we paused
			m_startTime = Time.time - (pauseTime - m_startTime);
		}
		else
		{
			//play from start:
			Restart();
		}
		
		//start playing
		isPlaying = true;
	}
	
	/**
	 * Stops the animation and resets
	 * */
	public void Stop()
	{
		isPlaying = false;
		
		m_curFrame = 0;
		
		m_texturePath = texture + m_curFrame;
		
		base.Update();
	}
	
	/**
	 * Pause the Animated Sprite
	 * */
	public void Pause()
	{
		isPlaying = false;
		
		isPaused = true;
		
		//remember when we paused for when we resume
		pauseTime = Time.time;
	}
	
	
	protected override void Update()
	{
		/**
		 * only update if playing
		 * */
		if (isPlaying)
		{
			// Do nothing if we haven't officially started yet
			if (m_startTime	> Time.time) return;
			
			float elapsedTime = Time.time - m_startTime;
			
			// Did we finish the sequence?
			if (elapsedTime > m_totalTime)
			{
				// Are we looping?
				if (m_loopCount	!= 0)
				{
					elapsedTime = elapsedTime - m_totalTime;
					m_startTime = Time.time - elapsedTime;
				}
				else
				{
					elapsedTime = m_totalTime;
					
					//stop updating if not looping
					isPlaying = false;
				}
				
				if (m_loopCount	> 0) m_loopCount--;
			}
			
			// Elapsed time should never be less than zero
			if (elapsedTime	< 0) elapsedTime = 0;
			
			// Calculate the current frame
			int curFrame = (int) (elapsedTime * frameRate);
			
			// Make sure we never exceed the number of the last frame (N-1)
			if (curFrame >= numFrames) curFrame = numFrames - 1;
			
			if (curFrame != m_curFrame)
			{
				//Debug.Log("Frame: " + curFrame + " / " + numFrames);
				m_texturePath = texture + curFrame;
				m_curFrame = curFrame;
				frameChanged = true;
			}
			
			base.Update();
		}
	}
}