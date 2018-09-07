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
 * Provides a property inspector GUI for editing Button instances.
 * 
 * @author Jean-Philippe Steinmetz
 */
[CustomEditor(typeof(ToggleButton))]
public class ToggleButtonEditor : Editor
{
	private bool showAlignment = true;
	private bool usingAtlas = false;
	
	public override void OnInspectorGUI()
	{
		bool dirty = false;
		ToggleButton button = target as ToggleButton;
		
		if (button.atlas != null && button.atlas.Length > 0)
		{
			usingAtlas = true;
		}
		else
		{
			usingAtlas = false;
		}
		
		GUI.changed = false;
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Is Visible");
		button.m_visible = EditorGUILayout.Toggle(button.m_visible);
		button.loadOnAwake = EditorGUILayout.Toggle("Load On Awake", button.loadOnAwake);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Is Active");
		button.isActive = EditorGUILayout.Toggle(button.isActive);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Atlas");
		button.atlas = EditorGUILayout.TextField(button.atlas);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Inactive Texture");
		button.texture = EditorGUILayout.TextField(button.texture);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Active Texture");
		button.activeTexture = EditorGUILayout.TextField(button.activeTexture);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Over Texture");
		button.overTexture = EditorGUILayout.TextField(button.overTexture);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Down Texture");
		button.downTexture = EditorGUILayout.TextField(button.downTexture);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Up Texture");
		button.upTexture = EditorGUILayout.TextField(button.upTexture);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Event Duration");
		button.eventDuration = EditorGUILayout.FloatField(button.eventDuration);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Depth");
		button.m_depth = EditorGUILayout.IntField(button.m_depth);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		button.m_position = EditorGUILayout.Vector2Field("Position", button.m_position);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		button.m_positionUnits = (GUIObject.PositionUnitTypes) EditorGUILayout.EnumPopup("Position Units", button.m_positionUnits);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		showAlignment = EditorGUILayout.Foldout(showAlignment, "Screen Alignment");
		EditorGUILayout.EndHorizontal();
		
		if (showAlignment)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Horizontal");
			button.m_horizontalAlignment = (GUIObject.HorizontalAlignmentTypes) EditorGUILayout.EnumPopup(button.m_horizontalAlignment);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Vertical");
			button.m_verticalAlignment = (GUIObject.VerticalAlignmentTypes) EditorGUILayout.EnumPopup(button.m_verticalAlignment);
			EditorGUILayout.EndHorizontal();
		}
		
		EditorGUILayout.BeginHorizontal();
		button.m_anchor = (GUIObject.AnchorTypes) EditorGUILayout.EnumPopup("Anchor Point", button.m_anchor);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		button.m_scale = EditorGUILayout.Vector2Field("Scale", button.m_scale);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		button.m_scaleMode = (GUIObject.ScaleModeTypes) EditorGUILayout.EnumPopup("Scale Mode", button.m_scaleMode);
		EditorGUILayout.EndHorizontal();
		
		if (!usingAtlas)
		{
			EditorGUILayout.BeginHorizontal();
			button.uvOffset = EditorGUILayout.Vector2Field("UV Offset", button.uvOffset);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			button.uvScale = EditorGUILayout.Vector2Field("UV Scale", button.uvScale);
			EditorGUILayout.EndHorizontal();
		}
		
		if (GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}
}
