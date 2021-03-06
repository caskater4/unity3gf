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
 * Provides a property inspector GUI for editing Sprite instances.
 * 
 * @author Jean-Philippe Steinmetz
 */
[CustomEditor(typeof(Sprite))]
public class SpriteEditor : Editor
{
	private bool showAlignment = true;
	private bool usingAtlas = false;
	
	public override void OnInspectorGUI()
	{
		Sprite sprite = target as Sprite;
		
		if (sprite.atlas != null && sprite.atlas.Length > 0)
		{
			usingAtlas = true;
		}
		else
		{
			usingAtlas = false;
		}
		
		GUI.changed = false;
		
		EditorGUILayout.BeginHorizontal();
		sprite.m_visible = EditorGUILayout.Toggle("Is Visible", sprite.m_visible);
		sprite.loadOnAwake = EditorGUILayout.Toggle("Load On Awake", sprite.loadOnAwake);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		sprite.atlas = EditorGUILayout.TextField("Atlas", sprite.atlas);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		sprite.texture = EditorGUILayout.TextField("Texture", sprite.texture);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		sprite.m_depth = EditorGUILayout.IntField("Depth", sprite.m_depth);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		sprite.m_position = EditorGUILayout.Vector2Field("Position", sprite.m_position);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		sprite.m_positionUnits = (GUIObject.PositionUnitTypes) EditorGUILayout.EnumPopup("Position Units",
			sprite.m_positionUnits);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		showAlignment = EditorGUILayout.Foldout(showAlignment, "Screen Alignment");
		EditorGUILayout.EndHorizontal();
		
		if (showAlignment)
		{
			EditorGUILayout.BeginHorizontal();
			sprite.m_horizontalAlignment = (GUIObject.HorizontalAlignmentTypes) EditorGUILayout.EnumPopup("Horizontal",
				sprite.m_horizontalAlignment);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			sprite.m_verticalAlignment = (GUIObject.VerticalAlignmentTypes) EditorGUILayout.EnumPopup("Vertical",
				sprite.m_verticalAlignment);
			EditorGUILayout.EndHorizontal();
		}
		
		EditorGUILayout.BeginHorizontal();
		sprite.m_anchor = (GUIObject.AnchorTypes) EditorGUILayout.EnumPopup("Anchor Point", sprite.m_anchor);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		sprite.m_scale = EditorGUILayout.Vector2Field("Scale", sprite.m_scale);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		sprite.m_scaleMode = (GUIObject.ScaleModeTypes) EditorGUILayout.EnumPopup("Scale Mode", sprite.m_scaleMode);
		EditorGUILayout.EndHorizontal();
		
		if (!usingAtlas)
		{
			EditorGUILayout.BeginHorizontal();
			sprite.uvOffset = EditorGUILayout.Vector2Field("UV Offset", sprite.uvOffset);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			sprite.uvScale = EditorGUILayout.Vector2Field("UV Scale", sprite.uvScale);
			EditorGUILayout.EndHorizontal();
		}
		
		if (GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}
}
