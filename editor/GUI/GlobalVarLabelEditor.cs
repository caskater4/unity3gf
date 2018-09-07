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
 * Provides a property inspector GUI for editing GlobalVarLabel instances.
 * 
 * @author Jean-Philippe Steinmetz
 */
[CustomEditor(typeof(GlobalVarLabel))]
public class GlobalVarLabelEditor : Editor
{
//	private tk2dFont[] allBmFontImporters = null;
	private bool showAlignment = true;
	
	public override void OnInspectorGUI()
	{
		GlobalVarLabel label = target as GlobalVarLabel;
		
//		if (label.m_fontType == Label.FontTypes.BITMAP)
//		{
//			// maybe cache this if its too slow later
//			if (allBmFontImporters == null) allBmFontImporters = tk2dEditorUtility.GetOrCreateIndex().GetFonts();
//			
//			if (label.m_fontBitmap == null)
//			{
//				label.m_fontBitmap = allBmFontImporters[0].data;
//			}
//		}
		
		GUI.changed = false;
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Is Visible");
		label.m_visible = EditorGUILayout.Toggle(label.m_visible);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		label.FontType = label.m_fontType = (Label.FontTypes) EditorGUILayout.EnumPopup("Font Type", label.m_fontType);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Variable Name");
		label.m_variableName = EditorGUILayout.TextField(label.m_variableName);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Prefix");
		label.m_prefix = EditorGUILayout.TextField(label.m_prefix);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Suffix");
		label.m_suffix = EditorGUILayout.TextField(label.m_suffix);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		label.m_characterSize = EditorGUILayout.FloatField("Character Size", label.m_characterSize);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		label.m_lineSpacing = EditorGUILayout.FloatField("Line Spacing", label.m_lineSpacing);
		EditorGUILayout.EndHorizontal();
		
		if (label.m_fontType == Label.FontTypes.TRUETYPE)
		{
			EditorGUILayout.BeginHorizontal();
			label.m_tabSize = EditorGUILayout.FloatField("Tab Size", label.m_tabSize);
			EditorGUILayout.EndHorizontal();
		}
		
		EditorGUILayout.BeginHorizontal();
		label.m_textAnchor = (TextAnchor) EditorGUILayout.EnumPopup("Anchor", label.m_textAnchor);
		EditorGUILayout.EndHorizontal();
		
//		if (label.m_fontType == Label.FontTypes.BITMAP)
//		{
//			int currId = -1;
//			string[] fontNames = new string[allBmFontImporters.Length];
//			for (int i = 0; i < allBmFontImporters.Length; ++i)
//			{
//				fontNames[i] = allBmFontImporters[i].name;
//				if (allBmFontImporters[i].data == label.m_fontBitmap)
//				{
//					currId = i;
//				}
//			}
//			
//			int newId = EditorGUILayout.Popup("Font", currId, fontNames);
//			if (newId != currId)
//			{
//				label.m_fontBitmap = allBmFontImporters[newId].data;
//				label.renderer.material = allBmFontImporters[newId].material;
//			}
//		}
//		else if (label.m_fontType == Label.FontTypes.TRUETYPE)
		{
			EditorGUILayout.BeginHorizontal();
			label.m_fontTrueType = (Font) EditorGUILayout.ObjectField("Font", label.m_fontTrueType, typeof(Font));
			EditorGUILayout.EndHorizontal();
		}
		
		EditorGUILayout.BeginHorizontal();
		label.m_color = EditorGUILayout.ColorField("Color", label.m_color);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		label.m_depth = EditorGUILayout.IntField("Depth", label.m_depth);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		label.m_position = EditorGUILayout.Vector2Field("Position", label.m_position);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		label.m_positionUnits = (GUIObject.PositionUnitTypes) EditorGUILayout.EnumPopup("Position Units", label.m_positionUnits);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		showAlignment = EditorGUILayout.Foldout(showAlignment, "Screen Alignment");
		EditorGUILayout.EndHorizontal();
		
		if (showAlignment)
		{
			EditorGUILayout.BeginHorizontal();
			label.m_horizontalAlignment = (GUIObject.HorizontalAlignmentTypes) EditorGUILayout.EnumPopup("Horizontal",
			                                                                                             label.m_horizontalAlignment);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			label.m_verticalAlignment = (GUIObject.VerticalAlignmentTypes) EditorGUILayout.EnumPopup("Vertical",
			                                                                                         label.m_verticalAlignment);
			EditorGUILayout.EndHorizontal();
		}
		
		EditorGUILayout.BeginHorizontal();
		label.m_anchor = (GUIObject.AnchorTypes) EditorGUILayout.EnumPopup("Anchor Point", label.m_anchor);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		label.m_charScaleMode = (GUIObject.ScaleModeTypes) EditorGUILayout.EnumPopup("Scale Mode", label.m_charScaleMode);
		EditorGUILayout.EndHorizontal();
		
		if (GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}
}
