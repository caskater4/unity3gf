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
 * SoundSource is a wrapper for AudioSource that enables localized audio content.
 * <p>
 * SoundSource instances also feature names. A name is an identifier intended to be unique for each instance of the
 * script per game object. If no name is specified one is generated automatically at runtime. Note that even though
 * names are intended to be unique there is no enforcement of two instances of the script being given the same
 * name.
 * 
 * @author Jean-Philippe Steinmetz
 */
public class SoundSource : MonoBehaviour
{
	/**
	 * The filename of the audio clip to play.
	 */
	public string audioClip = null;
	
	/**
	 * A unique name identifying the sound source.
	 */
	public string name = null;
	
	/**
	 * Un- / Mutes the AudioSource. Mute sets the volume=0, Un-Mute restore the original volume.
	 */
	public bool mute = false;
	
	/**
	 * Bypass effects (Applied from filter components or global listener filters)
	 */
	public bool bypassEffects = false;
	
	/**
	 * If set to true, the audio source will automatically start playing on awake
	 */
	public bool playOnAwake = true;
	
	/**
	 * Is the audio clip looping?
	 */
	public bool loop = false;
	
	/**
	 * Sets the priority of the audio source.
	 */
	public int priority = 128;
	
	/**
	 * The volume of the audio source.
	 */
	public float volume = 1.0f;
	
	/**
	 * The pitch of the audio source.
	 */
	public float pitch = 1.0f;
	
#region 3DSoundSettings
	/**
	 * Sets how much the 3d engine has an effect on the channel.
	 */
	public float panLevel = 1.0f;
	
	/**
	 * Sets the spread angle a 3d stereo or multichannel sound in speaker space.
	 */
	public float spread = 0.0f;
	
	/**
	 * Sets the Doppler scale for this audio source.
	 */
	public float dopplerLevel = 1.0f;
	
	/**
	 * Within the Min distance the AudioSource will cease to grow louder in volume.
	 */
	public float minDistance = 1.0f;
	
	/**
	 * (Logarithmic rolloff) MaxDistance is the distance a sound stops attenuating at.
	 */
	public float maxDistance = 500.0f;
	
	/**
	 * Sets/Gets how the audio source attenuates over distance.
	 */
	public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;
#endregion
	
#region 2DSoundSettings
	/**
	 * Sets a channels pan position linearly. Only works for 2D clips.
	 */
	public float pan2D = 0.0f;
	
	/**
	 * Sets a channels pan position linearly. Only works for 2D clips.
	 */
	public float pan
	{
		get
		{
			return pan2D;
		}
		set
		{
			pan2D = value;
		}
	}
#endregion
	
	/**
	 * This makes the audio source not take into account the volume of the audio listener.
	 */
	public bool ignoreListenerVolume
	{
		get
		{
			if (!source)
			{
				Debug.LogError("Sound not set up.");
				return false;
			}
			
			return source.ignoreListenerVolume;
		}
		set
		{
			if (!source)
			{
				Debug.LogError("Sound not set up.");
				return;
			}
			
			source.ignoreListenerVolume = value;
		}
	}
	
	/**
	 * Is the clip playing right now (Read Only)?
	 */
	public bool isPlaying
	{
		get
		{
			if (!source)
			{
				return false;
			}
			
			return source.isPlaying;
		}
	}
	
	/**
	 * Playback position in seconds.
	 */
	public float time
	{
		get
		{
			if (!source)
			{
				Debug.LogError("Sound not set up.");
				return 0;
			}
			
			return source.time;
		}
		set
		{
			if (!source)
			{
				Debug.LogError("Sound not set up.");
				return;
			}
			
			source.time = value;
		}
	}

	/**
	 * Playback position in PCM samples.
	 */
	public int timeSamples
	{
		get
		{
			if (!source)
			{
				Debug.LogError("Sound not set up.");
				return 0;
			}
			
			return source.timeSamples;
		}
		set
		{
			if (!source)
			{
				Debug.LogError("Sound not set up.");
				return;
			}
			
			source.timeSamples = value;
		}
	}
	
	public float Volume
	{
		get
		{
			if (!source)
			{
				Debug.LogError("Sound not set up.");
				return volume;
			}
			
			return source.volume;
		}
		set
		{
			if (!source)
			{
				Debug.LogError("Sound not set up.");
				volume = value;
				return;
			}
			
			source.volume = volume = value;
		}
	}
	
	/**
	 * The underlying audio source being wrapped.
	 */
	protected AudioSource source = null;
	
	/**
	 * This method is called when the script instance is being loaded.
	 */
	protected virtual void Awake()
	{
		// Generate a unique name if one has not been specified
		if (name == null || name.Length == 0)
		{
			SoundSource[] sources = this.gameObject.GetComponents<SoundSource>();
			name = "sound-" + sources.Length;
		}
		
		if (playOnAwake)
		{
			Play();
		}
	}
	
	/**
	 * Pauses playing the clip.
	 */
	public void Pause()
	{
		if (!source)
		{
			Debug.LogError("Sound not set up.");
			return;
		}
		
		source.Pause();
	}
	
	/**
	 * Plays the clip. 
	 */
	public void Play()
	{
		Play(0);
	}
	
	/**
	 * Plays the clip.
	 * <p>
	 * Delay allows a source to be played later at a specific sample-accurate point in time. Note that the delay must
	 * be set according to the reference output rate of 44.1 Khz, meaning that Play(44100) will delay the playing by
	 * exactly 1 sec. Note: To obtain sample accuracy with an AudioClip with a different samplerate (than 44.1 khz) you
	 * have to do the math yourselves. Delaying an audiosource with an attached AudioClip with samplerate of, say, 32
	 * khz, with 16k samples(.5 sec) is done by Play(22050). ((44100/32000) * 16000 = 22050). 
	 * 
	 * @param delay The amount to delay playback.
	 */
	public void Play(ulong delay)
	{
		// Set up the audio clip if not already done
		if (!source)
		{
			Setup(audioClip);
		}
		
		if (!source)
		{
			Debug.LogError("Sound not set up.");
			return;
		}
		
		source.Play(delay);
	}
	
	/**
	 * Plays the clip. 
	 */
	public void Play(string clip)
	{
		Play(clip, 0);
	}
	
	/**
	 * Plays the clip.
	 * <p>
	 * Delay allows a source to be played later at a specific sample-accurate point in time. Note that the delay must
	 * be set according to the reference output rate of 44.1 Khz, meaning that Play(44100) will delay the playing by
	 * exactly 1 sec. Note: To obtain sample accuracy with an AudioClip with a different samplerate (than 44.1 khz) you
	 * have to do the math yourselves. Delaying an audiosource with an attached AudioClip with samplerate of, say, 32
	 * khz, with 16k samples(.5 sec) is done by Play(22050). ((44100/32000) * 16000 = 22050). 
	 * 
	 * @param delay The amount to delay playback.
	 */
	public void Play(string clip, ulong delay)
	{
		audioClip = clip;
		Setup(audioClip);
		
		if (!source)
		{
			Debug.LogError("Sound not set up.");
			return;
		}
		
		source.Play(delay);
	}
	
	/**
	 * Plays an AudioClip.
	 * 
	 * @param clip The path to the clip to play.
	 */
	public void PlayOneShot(string clip)
	{
		PlayOneShot(clip, 1.0f);
	}
	
	/**
	 * Plays an AudioClip.
	 * <p>
	 * Scales the audio source volume by volumeScale.
	 * 
	 * @param clip The path to the clip to play.
	 * @param volumeScale The volume scale to play the clip at.
	 */
	public void PlayOneShot(string clip, float volumeScale)
	{
		Setup(clip, volumeScale);
		
		if (!source)
		{
			Debug.LogError("Sound not set up.");
			return;
		}
		
		source.PlayOneShot(source.clip, volumeScale);
	}
	
	/**
	 * Attempts to load and set up the internal audio source using the specified audio clip path.
	 * 
	 * @param clipPath The path to the localized audio asset.
	 */
	protected void Setup(string clipPath)
	{
		Setup(clipPath, this.volume);
	}
	
	/**
	 * Attempts to load and set up the internal audio source using the specified audio clip path.
	 * 
	 * @param clipPath The path to the localized audio asset.
	 * @param volume The volume level to set the audio source with.
	 */
	protected void Setup(string clipPath, float volume)
	{
		// First find the audio clip
		AudioClip clip = null;
		try {
			clip = (AudioClip) Asset.Load(clipPath, typeof(AudioClip));
		}
		catch ( UnityException e)
		{
			Debug.LogError("Can't find clip!");
		}
		
		if (!clip)
		{
			Debug.LogError("Audio asset not found: " + clipPath);
			return;
		}
		
		// Create the AudioSource but only if we haven't already done so previously
		if (source == null)
		{
			source = this.gameObject.AddComponent<AudioSource>();
		}
		
		// Now set all variables on the source
		source.clip = clip;
		source.mute = this.mute;
		source.bypassEffects = this.bypassEffects;
		source.playOnAwake = false;
		source.loop = this.loop;
		source.priority = this.priority;
		source.volume = volume;
		source.pitch = this.pitch;
		source.panLevel = this.panLevel;
		source.spread = this.spread;
		source.dopplerLevel = this.dopplerLevel;
		source.minDistance = this.minDistance;
		source.maxDistance = this.maxDistance;
		source.rolloffMode = this.rolloffMode;
		source.panLevel = this.pan2D;
	}
	
	/**
	 * Stops playing the clip.
	 */
	public void Stop()
	{
		if (!source)
		{
			Debug.LogError("Sound not set up.");
			return;
		}
		
		source.Stop();
	}
	
	/**
	 * Retrieves the first SoundSource instance attached to the specified object with the given name.
	 * 
	 * @param obj The GameObject to retrieve the SoundSource instance from.
	 * @param name The name of the SoundSource instance to retrieve.
	 * @return The first instance of SoundSource with the specified name, otherwise null.
	 */
	public static SoundSource Find(GameObject obj, string name)
	{
		SoundSource[] sources = obj.GetComponents<SoundSource>();
		
		foreach (SoundSource source in sources)
		{
			if (source.name	== name)
			{
				return source;
			}
		}
		
		return null;
	}
	
	/**
	 * Plays the clip at position. Automatically cleans up the audio source after it has finished playing.
	 * 
	 * @param clip The path to the localized audio asset to play.
	 * @param position The 3D position to set for the audio source when playing.
	 */
	public static void PlayClipAtPoint(string clip, Vector3 position)
	{
		PlayClipAtPoint(clip, position, 1.0f);
	}
	
	/**
	 * Plays the clip at position. Automatically cleans up the audio source after it has finished playing.
	 * 
	 * @param clip The path to the localized audio asset to play.
	 * @param position The 3D position to set for the audio source when playing.
	 * @param volume The volume level to play the audio source at.
	 */
	public static void PlayClipAtPoint(string clip, Vector3 position, float volume)
	{
		// First find the audio clip
		AudioClip audioClip = (AudioClip) Asset.Load(clip, typeof(AudioClip));
		
		if (!audioClip)
		{
			Debug.LogError("Audio asset not found: " + clip);
			return;
		}
		
		AudioSource.PlayClipAtPoint(audioClip, position, volume);
	}
}
