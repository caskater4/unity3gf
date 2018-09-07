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
using System.Collections;

/**
 * Provides a property inspector GUI for editing TweenGUIObject instances.
 * 
 * @author Jean-Philippe Steinmetz
 */
[CustomEditor(typeof(TweenGUIObject))]
public class TweenGUIObjectEditor : Editor
{
	private bool showPropertyFrom = true;
	
	public override void OnInspectorGUI()
	{
		TweenGUIObject tween = target as TweenGUIObject;
		
		if (tween.m_useCurrentValue)
		{
			showPropertyFrom = false;
		}
		else
		{
			showPropertyFrom = true;
		}
		
		// If no name is specified for the tween, generate one
		if  (tween.m_name == null || tween.m_name.Length == 0)
		{
			// How many tweens are on the game object?
			TweenGUIObject[] tweens = (TweenGUIObject[]) tween.gameObject.GetComponents<TweenGUIObject>();
			int count = 0;
			
			if (tweens != null)
			{
				count = tweens.Length;
			}
			
			tween.m_name = "Tween" + (count+1);
		}
		
		GUI.changed = false;
		
		EditorGUILayout.BeginHorizontal();
		tween.m_autoPlay = EditorGUILayout.Toggle("Play on Start", tween.m_autoPlay);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		tween.m_name = EditorGUILayout.TextField("Name", tween.m_name);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Property");
		tween.m_property = (TweenGUIObject.PropertyType) EditorGUILayout.EnumPopup(tween.m_property);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Ease Type");
		tween.m_easeType = (iTween.EaseType) EditorGUILayout.EnumPopup(tween.m_easeType);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		tween.m_delay = EditorGUILayout.FloatField("Delay", tween.m_delay);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		tween.m_time = EditorGUILayout.FloatField("Time", tween.m_time);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		tween.m_useCurrentValue = EditorGUILayout.Toggle("Use Current Value",
                                                         tween.m_useCurrentValue);
		EditorGUILayout.EndHorizontal();
		
		if (showPropertyFrom)
		{
			EditorGUILayout.BeginHorizontal();
			tween.m_propertyFrom = EditorGUILayout.Vector2Field("Property From", tween.m_propertyFrom);
			EditorGUILayout.EndHorizontal();
		}
		
		EditorGUILayout.BeginHorizontal();
		tween.m_propertyTo = EditorGUILayout.Vector2Field("Property To", tween.m_propertyTo);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		tween.m_ignoreTimeScale = EditorGUILayout.Toggle("Ignore Time Scale", tween.m_ignoreTimeScale);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.Toggle("Is Playing", tween.IsPlaying);
		EditorGUILayout.EndHorizontal();
		
		if (GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}
}
