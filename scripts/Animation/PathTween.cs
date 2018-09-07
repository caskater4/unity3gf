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
 * Animates the game object moving along a specified path, looping the animation when applicable.
 * 
 * @author Jean-Philippe Steinmetz
 */
public class PathTween : MonoBehaviour
{
	/**
	 * Set to true to automatically start the tween, otherwise set to false.
	 */
	public bool autoStart = true;
	
	/**
	 * The number of remaining times to loop the animation.
	 */
	private int loopCount = 0;
	
	/**
	 * The number of times to loop the animation. A value of 0 plays the animation once, 1 plays twice, and -1 loops
	 * the animation forever. Default value is 0.
	 */
	public int numLoops = 0;
	
	/**
	 * The list of points that make up the path of the animation tween where the first point is the starting position
	 * of the tween and the last point the final position.
	 */
	public Vector3[] path = null;
	
	/**
	 * The time at which playback was paused.
	 */
	private float pauseTime = -1;
	
	/**
	 * Indicates if the tween is currently playing.
	 */
	public bool playing
	{
		get;
		protected set;
	}
	
	/**
	 * The currrent amount of progress towards the completion of the tween as a percentage.
	 */
	protected float progress = 0.0f;
	
	/**
	 * The time that the animation began.
	 */
	private float startTime = -1.0f;
	
	/**
	 * The time, in seconds, that the tween will take to move the game object from the start of the path to the finish.
	 * Default value is 1.0.
	 */
	public float time = 1.0f;
	
	/**
	 * The amount of variation to apply to each point of the path. Typically only x and y variation is desirable.
	 * Default is no variation.
	 */
	public Vector3 variance = Vector3.zero;
	
	protected virtual void Start()
	{
		if (autoStart)
		{
			Play();
		}
	}
	
	/**
	 * Applies the variation to each point along the path.
	 */
	protected virtual Vector3[] ApplyVariation()
	{
		if (variance == Vector3.zero) return this.path;
		
		Vector3[] newPath = new Vector3[path.Length];
		
		for (int i = 0; i < this.path.Length; ++i)
		{
			// When applying variance make sure to scale it with the path
			newPath[i] = new Vector3(this.path[i].x + variance.x,
			                         this.path[i].y + variance.y,
			                         this.path[i].z + variance.z);
		}
		
		return newPath;
	}
	
	/**
	 * Pauses current playback of the tween animation.
	 */
	public void Pause()
	{
		pauseTime = Time.time;
		playing = false;
	}
	
	/**
	 * Begins playback of the tween animation.
	 */
	public void Play()
	{
		// Is this a new playback?
		if (startTime < 0)
		{
			loopCount = numLoops;
			progress = 0;
			startTime = Time.time;
		}
		else
		{
			// Update the startTime based on how much time has passed since we paused.
			startTime += Time.time - pauseTime;
			pauseTime = -1.0f;
		}
		
		playing = true;
	}
	
	/**
	 * Stops playing of the current tween animation.
	 * <p>
	 * This causes the animation to be reset on the next call to Play().
	 */
	public void Stop()
	{
		startTime = -1.0f;
		playing = false;
	}
	
	protected virtual void Update()
	{
		if (!playing) return;
		
		Vector3[] path = ApplyVariation();
		progress = (Time.time - startTime) / time;
		
		// Check to see if the animation has completed.
		if (progress > 1.0f)
		{
			// Are we looping?
			if (loopCount == 0)
			{
				progress = 1.0f;
			}
			else
			{
				// Wrap the percentage around
				progress = progress - 1.0f;
				
				// Decrement the counter
				if (loopCount > 0) --loopCount;
				
				// Update the start time
				startTime = Time.time;
			}
		}
		
		//Debug.Log("PercentComplete: " + percentComplete);
		// Move the game object to the correct position along the path
		
		iTween.PutOnPath(this.gameObject, path, progress);
	}
}
