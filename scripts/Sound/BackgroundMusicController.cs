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
using System.Collections;
using UnityEngine;

/**
 * BackgroundMusicController manages the background music of the application, providing a mechanism for
 * seamlessly transitioning from one sound to another. This is accomplished via the static Play() and Stop()
 * functions.
 * <p>
 * In order to use the controller place an instance of the script on a static GameObject that is does not
 * get destroyed on scene changes. Two SoundSource components are required with the names 'backgroundA' and
 * 'backgroundB' respectively.
 * 
 * @author Jean-Philippe Steinmetz
 */
[RequireComponent(typeof(SoundSource))]
public class BackgroundMusicController : MonoBehaviour
{
	/**
	 * The types of fades supported.
	 */
	public enum FadeTypes
	{
		CROSSFADE,
		HARDSTOP,
		FADEINOUT
	}
	
	private static BackgroundMusicController Instance = null;
	
	/**
	 * The current sound source playing the background music.
	 */
	public static SoundSource CurrentSource
	{
		get;
		private set;
	}
	
	/**
	 * The speed, in seconds, to perform a fade.
	 */
	public float fadeSpeed = 1.0f;
	
	/**
	 * The current sound source's desired peak volume level.
	 */
	private float currentSourceVolume = 1.0f;
	
	/**
	 * The sound source for channel A.
	 */
	private SoundSource sourceA = null;
	
	/**
	 * The desired peak volume level for channel A.
	 */
	private float sourceAVolume = 1.0f;
	
	/**
	 * The sound source for channel B.
	 */
	private SoundSource sourceB = null;
	
	/**
	 * The desired peak volume level for channel B.
	 */
	private float sourceBVolume = 1.0f;
	
	private bool paused = false;
	
	protected void Awake()
	{
		if (Instance)
		{
			Debug.LogWarning("There should only be one instance of BackgroundMusicController!");
			return;
		}
		
		Instance = this;
		
		sourceA = SoundSource.Find(this.gameObject, "backgroundA");
		if (sourceA == null)
		{
			Debug.LogError("No SoundSource component found named: backgroundA");
		}
		
		sourceB = SoundSource.Find(this.gameObject, "backgroundB");
		if (sourceB == null)
		{
			Debug.LogError("No SoundSource component found named: backgroundB");
		}
		
		sourceAVolume = sourceA.volume;
		sourceBVolume = sourceB.volume;
		CurrentSource = sourceA;
		currentSourceVolume = sourceAVolume;
	}
	
	private void PlayCrossFade(string path)
	{
		PlayCrossFade(path, fadeSpeed);
	}
	
	private void PlayCrossFade(string path, float speed)
	{
		// If the current playing background music is the same don't do anything
		if (CurrentSource.isPlaying && path == CurrentSource.audioClip)
		{
			return;
		}
		
		// Set up the new clip
		CurrentSource = CurrentSource == sourceA ? sourceB : sourceA;
		currentSourceVolume = CurrentSource == sourceA ? sourceAVolume : sourceBVolume;
		CurrentSource.Play(path);
		CurrentSource.Volume = 0.0f;
		
		// Create the list of arguments to pass to iTween
		Hashtable args = new Hashtable();
		args.Add("name", "bgcrossfade");
		args.Add("from", 0.0f);
		args.Add("to", 1.0f);
		args.Add("time", speed);
		args.Add("easetype", iTween.EaseType.linear);
		args.Add("onupdate", "OnBgCrossFadeUpdate");
		args.Add("onupdatetarget", this.gameObject);
		args.Add("oncomplete", "OnBgCrossFadeComplete");
		args.Add("oncompletetarget", this.gameObject);
		
		// Start the tweening!
		iTween.ValueTo(this.gameObject, args);
	}
	
	private void PlayHardStop(string path)
	{
		// If the current playing background music is the same don't do anything
		if (CurrentSource.isPlaying && path == CurrentSource.audioClip)
		{
			return;
		}
		
		// Stop the existing music
		if (CurrentSource.isPlaying)
		{
			CurrentSource.Stop();
		}
		
		// Now play the new music
		CurrentSource.Play(path);
		CurrentSource.Volume = currentSourceVolume;
	}
	
	private void PlayFadeInOut(string path)
	{
		PlayFadeInOut(path, fadeSpeed);
	}
	
	private void PlayFadeInOut(string path, float speed)
	{
		// If the current playing background music is the same don't do anything
		if (CurrentSource.isPlaying && path == CurrentSource.audioClip)
		{
			return;
		}
		
		// Set up the new clip
		CurrentSource = CurrentSource == sourceA ? sourceB : sourceA;
		currentSourceVolume = CurrentSource == sourceA ? sourceAVolume : sourceBVolume;
		CurrentSource.Play(path);
		CurrentSource.Volume = 0.0f;
		
		// Create the list of arguments to pass to iTween
		Hashtable args = new Hashtable();
		args.Add("name", "bgcrossfade");
		args.Add("from", 0.0f);
		args.Add("to", 1.0f);
		args.Add("time", speed);
		args.Add("easetype", iTween.EaseType.linear);
		args.Add("oncomplete", "OnBgFadeInOutComplete");
		args.Add("oncompletetarget", this.gameObject);
		args.Add("onupdate", "OnBgFadeInOutUpdate");
		args.Add("onupdatetarget", this.gameObject);
		
		// Start the tweening!
		iTween.ValueTo(this.gameObject, args);
	}
	
	private void OnBgCrossFadeComplete()
	{
		// Stop the old sound source that should now have a volume of zero
		SoundSource oldSource = CurrentSource == sourceA ? sourceB : sourceA;
		
		if (oldSource.isPlaying)
		{
			oldSource.Stop();
		}
	}
	
	private void OnBgCrossFadeUpdate(float progress)
	{
		CurrentSource.Volume = progress * currentSourceVolume;
		
		SoundSource oldSource = CurrentSource == sourceA ? sourceB : sourceA;
		float oldSourceVolume = CurrentSource == sourceA ? sourceBVolume : sourceAVolume;
		
		if (oldSource.isPlaying)
		{
			oldSource.Volume = (1.0f - progress) * oldSourceVolume;
		}
	}
	
	private void OnBgFadeInOutComplete()
	{
		CurrentSource.Volume = currentSourceVolume;
		
		// Stop the old sound source that should now have a volume of zero
		SoundSource oldSource = CurrentSource == sourceA ? sourceB : sourceA;
		
		if (oldSource.isPlaying)
		{
			oldSource.Stop();
		}
	}
	
	private void OnBgFadeInOutUpdate(float progress)
	{
		SoundSource oldSource = CurrentSource == sourceA ? sourceB : sourceA;
		float oldSourceVolume = CurrentSource == sourceA ? sourceBVolume : sourceAVolume;
		
		// Fading out the old source
		if (oldSource.isPlaying && progress < 0.5f)
		{
			oldSource.Volume = (1.0f - (progress * 2)) * oldSourceVolume;
		}
		// Fading in the new source
		else
		{
			CurrentSource.Volume = ((progress % 0.5f) * 2) * currentSourceVolume;
		}
	}
	
	private void _Stop()
	{
		CurrentSource.Stop();
	}
	
	private void _Pause()
	{
		CurrentSource.Pause();		
	}
	
	private void _Unpause()
	{
		CurrentSource.Play();
	}
	
	public static void Play(string path, FadeTypes fade)
	{
		switch (fade)
		{
		case FadeTypes.CROSSFADE:	Instance.PlayCrossFade(path); break;
		case FadeTypes.FADEINOUT:	Instance.PlayFadeInOut(path); break;
		case FadeTypes.HARDSTOP:	Instance.PlayHardStop(path); break;
		default: 					Debug.LogError("Invalid fade type: " + fade); break;
		}
	}
	
	public static void Play(string path, FadeTypes fade, float speed)
	{
		switch (fade)
		{
		case FadeTypes.CROSSFADE:	Instance.PlayCrossFade(path, speed); break;
		case FadeTypes.FADEINOUT:	Instance.PlayFadeInOut(path, speed); break;
		case FadeTypes.HARDSTOP:	Instance.PlayHardStop(path); break;
		default: 					Debug.LogError("Invalid fade type: " + fade); break;
		}
	}
	
	public static void Stop()
	{
		Instance._Stop();
	}	
}
