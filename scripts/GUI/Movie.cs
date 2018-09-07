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
 * Movie is a GUI element that renders video files as a sprite. The types of video that can be played are what
 * is defined by MovieTexture.
 * 
 * @author Jean-Philippe Steinmetz
 */
[AddComponentMenu("GUI/Movie")]
public class Movie : GUIObject
{
	/**
	 * The name and path of the movie to play.
	 */
	public string movieName = null;
	
	/**
	 * Set to true to automatically play the movie, otherwise set to false.
	 */
	public bool autoplay = true;

#if !UNITY_IPHONE && !UNITY_ANDROID
	/**
	 * The total length of the movie in seconds.
	 */
	public float Duration
	{
		get
		{
			return m_texture.duration;
		}
	}
	
	public bool IsPlaying
	{
		get
		{
			if (m_texture == null) return false;
			return m_texture.isPlaying;
		}
	}
	
	public bool IsReadyToPlay
	{
		get
		{
			if (m_texture == null) return false;
			return m_texture.isReadyToPlay;
		}
	}
	
	/**
	 * The time at which the movie started.
	 */
	private float m_startTime = -1.0f;
	
	/**
	 * 
	 */
	private bool m_playInitiated = false;
#endif
	
#if !UNITY_IPHONE && !UNITY_ANDROID
	private AudioSource m_audioSource = null;
	private MovieTexture m_texture = null;
#endif	
	protected override void Start()
	{
		Debug.Log("Movie starting!");
		#if !UNITY_IPHONE && !UNITY_ANDROID
		int pIdx = movieName.LastIndexOf(".");
		string file = movieName.Substring(0, pIdx >= 0 ? pIdx : movieName.Length);
		m_texture = (MovieTexture) Asset.Load(file, typeof(MovieTexture));
		
		if (m_texture == null)
		{
			Debug.LogError("Unable to load movie: " + movieName);
			return;
		}
		
		// Assign the texture to the rendering material
		renderer.material.mainTexture = m_texture;
		
		// Assign the audio to our source
		if (m_texture.audioClip != null)
		{
			m_audioSource = GetComponent<AudioSource>();
			if (m_audioSource == null)
			{
				m_audioSource = (AudioSource) this.gameObject.AddComponent(typeof(AudioSource));
				m_audioSource.playOnAwake = false;
			}
			
			m_audioSource.clip = m_texture.audioClip;
		}
		
		// Update the dimensions
		Width = m_texture.width;
		Height = m_texture.height;
#endif
		
		base.Start();
		
		if (autoplay)
		{
			Play();
		}
	}
	
	/**
	 * Pauses playback of the movie. Playback cannot be paused on iOS and Android through this function.
	 */
	public void Pause()
	{
#if !UNITY_IPHONE && !UNITY_ANDROID
		Debug.Log("Pausing movie...");
		m_texture.Pause();
		if (m_audioSource != null)
		{
			m_audioSource.Pause();
		}
		
		m_playInitiated = false;
		
		SendMessage("OnAnimPause", "Movie", SendMessageOptions.DontRequireReceiver);
#endif
	}
	
	/**
	 * Begins playback of the movie.
	 */
	public void Play()
	{
#if UNITY_IPHONE || UNITY_ANDROID
		SendMessage("OnAnimStart", "Movie", SendMessageOptions.DontRequireReceiver);
		iPhoneUtils.PlayMovie(movieName, Color.black, iPhoneMovieControlMode.CancelOnTouch);
		OnComplete();
#else
		if (IsPlaying) return;
		
		Debug.Log("Playing movie...");
		m_texture.Play();
		if (m_audioSource != null)
		{
			m_audioSource.Play();
		}
		
		m_startTime = Time.time;
		m_playInitiated = true;
		
		SendMessage("OnAnimStart", "Movie", SendMessageOptions.DontRequireReceiver);
#endif
	}
	
	/**
	 * Stops playing the movie, and rewinds it to the beginning. Playback cannot be stopped on iOS and Android through
	 * this function.
	 */
	public void Stop()
	{
#if !UNITY_IPHONE && !UNITY_ANDROID
		Debug.Log("Stopping movie...");
		m_texture.Stop();
		if (m_audioSource != null)
		{
			m_audioSource.Stop();
		}
		
		m_startTime = -1.0f;
		m_playInitiated = false;
		
		SendMessage("OnAnimStop", "Movie", SendMessageOptions.DontRequireReceiver);
#endif
	}
	
	/**
	 * This method is called when playback of the movie has completed.
	 */
	protected virtual void OnComplete()
	{
		SendMessage("OnAnimEnd", "Movie", SendMessageOptions.DontRequireReceiver);
#if !UNITY_IPHONE && !UNITY_ANDROID
		Debug.Log("Movie finished.");
		Stop();
#endif
	}
	
#if !UNITY_IPHONE && !UNITY_ANDROID
	protected override void Update()
	{
		if (!IsPlaying)
		{
			// Sometimes calling Play() on the texture doesn't start right away since the texture wasn't quite ready.
			// In those cases we want to call play again
			if (m_playInitiated)
			{
				Play();
			}
			// Check to see if the movie has finished
			else if (m_startTime >= 0 && Time.time >= m_startTime + Duration)
			{
				OnComplete();
			}
		}
		else
		{
			// Release this so we don't end up spamming Play() multiple times.
			m_playInitiated = false;
		}
		
		base.Update();
	}
#endif
}
