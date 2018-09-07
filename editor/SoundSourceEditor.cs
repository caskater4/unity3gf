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

using UnityEditor;
using UnityEngine;

/**
 * Provides a property inspector GUI for editing SoundSource instances.
 * <p>
 * The editor generates and validates names to ensure uniqueness among all instances of SoundSource for a game object.
 * 
 * @author Jean-Philippe Steinmetz
 */
//[CustomEditor(typeof(SoundSource))]
public class SoundSourceEditor : Editor
{
	private bool show2DSettings = true;
	private bool show3DSettings = true;
	
	public override void OnInspectorGUI()
	{
		GUI.changed = false;
		
		SoundSource source = target as SoundSource;
		
		// Check if a name exists. If not, generate one. Otherwise validate the name.
		if (source.name == null || source.name.Length == 0)
		{
			SoundSource[] sources = source.gameObject.GetComponents<SoundSource>();
			source.name = "sound-" + sources.Length;
		}
		else
		{
			ValidateName();
		}
		
		// Draw the GUI
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Audio Clip");
		source.audioClip = EditorGUILayout.TextField(source.audioClip);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Name");
		source.name = EditorGUILayout.TextField(source.name);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Mute");
		source.mute = EditorGUILayout.Toggle(source.mute);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Bypass Effects");
		source.bypassEffects = EditorGUILayout.Toggle(source.bypassEffects);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Play On Awake");
		source.playOnAwake = EditorGUILayout.Toggle(source.playOnAwake);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Loop");
		source.loop = EditorGUILayout.Toggle(source.loop);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Priority");
		source.priority = EditorGUILayout.IntSlider(source.priority, 0, 255);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Volume");
		source.volume = EditorGUILayout.Slider(source.volume, 0, 1.0f);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Pitch");
		source.pitch = EditorGUILayout.Slider(source.pitch, -3.0f, 3.0f);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		show3DSettings = EditorGUILayout.Foldout(show3DSettings, "3D Sound Settings");
		EditorGUILayout.EndHorizontal();
		
		if (show3DSettings)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Pan Level");
			source.panLevel = EditorGUILayout.Slider(source.panLevel, 0.0f, 1.0f);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Spread");
			source.spread= EditorGUILayout.Slider(source.spread, 0.0f, 360.0f);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Doppler Level");
			source.dopplerLevel = EditorGUILayout.Slider(source.dopplerLevel, 0.0f, 50.0f);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Min Distance");
			source.minDistance = EditorGUILayout.FloatField(source.minDistance);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Max Distance");
			source.maxDistance = EditorGUILayout.FloatField(source.maxDistance);
			EditorGUILayout.EndHorizontal();
			/*
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Rolloff Mode");
			source.rolloffMode = EditorGUILayout.EnumPopup(source.rolloffMode);
			EditorGUILayout.EndHorizontal();
			*/
		}
		
		EditorGUILayout.BeginHorizontal();
		show2DSettings = EditorGUILayout.Foldout(show2DSettings, "2D Sound Settings");
		EditorGUILayout.EndHorizontal();
		
		if (show2DSettings)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Pan 2D");
			source.pan2D = EditorGUILayout.Slider(source.pan2D, -1.0f, 1.0f);
			EditorGUILayout.EndHorizontal();
		}
		
		if (GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}
	
	/**
	 * Checks all SoundSource instances in the game object to determine if the current given name is a duplicate. If a
	 * duplicate is found we change our name to add a number at the end.
	 */
	private void ValidateName()
	{
		SoundSource source = target as SoundSource;
		
		// If no name has been specified yet return
		if (source.name	== null || source.name.Length == 0)
		{
			return;
		}
		
		SoundSource[] sources = source.gameObject.GetComponents<SoundSource>();
		int count = 0;
		
		foreach (SoundSource src in sources)
		{
			if (src.name	== null || src.name.Length == 0)
			{
				continue;
			}
			
			// If the names are the same and the instance is not the same, we have a problem. Count how many
			// there are with the same name.
			if (source.name.ToLower() == src.name.ToLower() && source != src)
			{
				count++;
			}
		}
		
		// If there were sounds found with the same name, adjust our name to include a count
		if (count > 0)
		{
			source.name += count.ToString();
		}
	}
}
