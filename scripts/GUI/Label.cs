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
 * Label is a GUI object that renders text directly to the screen. All text set to the component respects
 * localization.
 * 
 * @author Jean-Philippe Steinmetz
 */
[AddComponentMenu("GUI/Label")]
[ExecuteInEditMode]
public class Label : GUIObject
{
#region INTERNAL_STRUCTURES
	
	/**
	 * Describes the different font type options supported by Label.
	 */
	public enum FontTypes
	{
//		BITMAP,
		TRUETYPE
	}
	
#endregion
	
#region INSPECTOR PROPERTIES
	
	/**
	 * Do not modify this variable directly! Use the CharacterSize property instead.
	 */
	public float m_characterSize = 1.0f;
	
	/**
	 * The actual scale mode to apply to the character size.
	 */
	public ScaleModeTypes m_charScaleMode = ScaleModeTypes.NONE;
	
	/**
	 * Do not modify this variable directly! Use the Color property instead.
	 */
	public Color m_color = Color.white;
	
	/**
	 * Do not modify this variable directly! Use the FontBitmap property instead.
	 */
//	public tk2dFontData m_fontBitmap = null;
	
	/**
	 * Do not modify this variable directly! Use the FontTrueType property instead.
	 */
	public Font m_fontTrueType = null;
	
	/**
	 * Do not modify this variable directly! Use the FontType property instead.
	 */
	public FontTypes m_fontType = FontTypes.TRUETYPE;
	
	/**
	 * Do not modify this variable directly! Use the LineSpacing property instead.
	 */
	public float m_lineSpacing = 1.0f;
	
	/**
	 * Do not modify this variable directly! Use the TabSize property instead.
	 */
	public float m_tabSize = 4.0f;
	
	/**
	 * Do not modify this variable directly! Use the Text property instead.
	 */
	public string m_text = null;
	
	/**
	 * Do not modify this variable directly! Use the TextAnchor property instead.
	 */
	public TextAnchor m_textAnchor = TextAnchor.UpperLeft;
	
#endregion

#region VARIABLES
	
	/**
	 * The text mesh component attached to the game object when using TrueType fonts.
	 */
	protected TextMesh trueTypeTextMesh = null;
	
	/**
	 * The text mesh component attached to the game object when using Bitmap fonts.
	 */
//	protected tk2dTextMesh bitmapTextMesh = null;

#endregion
	
#region PROPERTIES
	
	/**
	 * The size of each character.
	 */
	public float CharacterSize
	{
		get
		{
//			if (FontType == FontTypes.BITMAP)
//			{
//				return bitmapTextMesh.scale.x;
//			}
//			else if (FontType == FontTypes.TRUETYPE)
			{
				return trueTypeTextMesh.characterSize;
			}
			
			return m_characterSize;
		}
		set
		{
//			if (FontType == FontTypes.BITMAP)
//			{
//				bitmapTextMesh.scale = new Vector3(value, value, value);
//			}
//			else if (FontType == FontTypes.TRUETYPE)
			{
				trueTypeTextMesh.characterSize = value;
			}
		}
	}
	
	/**
	 * The color to use when rendering the text. This property is unsupported by TrueType font types.
	 */
	public Color Color
	{
		get
		{
//			if (FontType == FontTypes.BITMAP)
//			{
//				return bitmapTextMesh.color;
//			}
//			else if (FontType == FontTypes.TRUETYPE)
			{ //used to use renderer.material
				return this.renderer.sharedMaterial.color;
			}
			
			return m_color;
		}
		set
		{
//			if (FontType == FontTypes.BITMAP)
//			{
//				bitmapTextMesh.color = value;
//			}
//			else if (FontType == FontTypes.TRUETYPE)
			{ //used to use renderer.material
				this.renderer.sharedMaterial.color = value;
			}
		}
	}
	
	/**
	 * The font to use when rendering the label when using bitmap fonts.
	 */
//	public tk2dFontData FontBitmap
//	{
//		get
//		{
//			if (FontType != FontTypes.BITMAP) return null;
//			return bitmapTextMesh.font;
//		}
//		set
//		{
//			if (FontType != FontTypes.BITMAP) return;
//			bitmapTextMesh.font = value;
//			
//			// Assign the font's material
//			//used to use renderer.material
//			this.renderer.sharedMaterial = value.material;
//		}
//	}
	
	/**
	 * The font to use when rendering the label when using TrueType fonts.
	 */
	public Font FontTrueType
	{
		get
		{
			if (FontType != FontTypes.TRUETYPE) return null;
			return trueTypeTextMesh.font;
		}
		set
		{
			if (FontType != FontTypes.TRUETYPE) return;
			trueTypeTextMesh.font = value;
			
			if (value != null)
			{
				// Make a copy of the material and assign it to the renderer
				//used to be renderer.material
				this.renderer.sharedMaterial = new Material(value.material);
				this.renderer.sharedMaterial.color = this.Color;
			}
		}
	}
	
	/**
	 * The type of font to use when rendering.
	 */
	public FontTypes FontType
	{
		get
		{
			return m_fontType;
		}
		set
		{
			m_fontType = value;
			
			// Create the necessary text mesh if missing
//			if (value == FontTypes.BITMAP)
//			{
//				// Clear out the TrueType mesh if it exists
//				if (trueTypeTextMesh != null)
//				{
//					DestroyImmediate(trueTypeTextMesh);
//				}
//				
//				if (bitmapTextMesh == null)
//				{
//					bitmapTextMesh = GetComponent<tk2dTextMesh>();
//					
//					// Create the text mesh if it doesn't exist
//					if (bitmapTextMesh == null)
//					{
//						bitmapTextMesh = (tk2dTextMesh) this.gameObject.AddComponent(typeof(tk2dTextMesh));
//					}
//				}
//			}
//			else if (value == FontTypes.TRUETYPE)
			{
				// Clear out the Bitmap mesh if it exists
//				if (bitmapTextMesh != null)
//				{
//					DestroyImmediate(bitmapTextMesh);
//					DestroyImmediate(GetComponent<MeshFilter>());
//				}
				
				if (trueTypeTextMesh == null)
				{
					trueTypeTextMesh = GetComponent<TextMesh>();
					
					// Create a text mesh if one doesn't exist
					if (trueTypeTextMesh == null)
					{
						trueTypeTextMesh = (TextMesh) this.gameObject.AddComponent(typeof(TextMesh));
					}
				}
			}
		}
	}
	
	/**
	 * How much space will be in-between lines of text.
	 */
	public float LineSpacing
	{
		get
		{
//			if (FontType == FontTypes.BITMAP)
//			{
//				return bitmapTextMesh.lineSpacing;
//			}
//			else if (FontType == FontTypes.TRUETYPE)
			{
				return trueTypeTextMesh.lineSpacing;
			}
			
			return m_lineSpacing;
		}
		set
		{
//			if (FontType == FontTypes.BITMAP)
//			{
//				bitmapTextMesh.lineSpacing = value;
//			}
//			else if (FontType == FontTypes.TRUETYPE)
			{
				trueTypeTextMesh.lineSpacing = value;
			}
		}
	}
	
	/**
	 * How much space will be inserted for a tab '\t' character. This is a multiplum of the 'spacebar' character 
	 * offset. This feature is not implemented for Bitmap font types.
	 */
	public float TabSize
	{
		get
		{
			if (FontType == FontTypes.TRUETYPE)
			{
				return trueTypeTextMesh.tabSize;
			}
			
			return m_tabSize;
		}
		set
		{
			if (FontType == FontTypes.TRUETYPE)
			{
				trueTypeTextMesh.tabSize = value;
			}
		}
	}
	
	/**
	 * The text to display.
	 */
	public string Text
	{
		get
		{
			return m_text;
		}
		set
		{
			m_text = value;
		}
	}
	
	/**
	 * How lines of text are anchored within the object.
	 */
	public TextAnchor TextAnchor
	{
		get
		{
//			if (FontType == FontTypes.BITMAP)
//			{
//				return bitmapTextMesh.anchor;
//			}
//			else if (FontType == FontTypes.TRUETYPE)
			{
				// For some reason the anchor is flipped vertically so we need to translate.
				switch (trueTypeTextMesh.anchor)
				{
					case TextAnchor.LowerCenter:	return TextAnchor.UpperCenter;
					case TextAnchor.LowerLeft:		return TextAnchor.UpperLeft;
					case TextAnchor.LowerRight:		return TextAnchor.UpperRight;
					case TextAnchor.UpperCenter:	return TextAnchor.LowerCenter;
					case TextAnchor.UpperLeft:		return TextAnchor.LowerLeft;
					case TextAnchor.UpperRight:		return TextAnchor.LowerRight;
				}
				
				return trueTypeTextMesh.anchor;
			}
			
			return m_textAnchor;
		}
		set
		{
//			if (FontType == FontTypes.BITMAP)
//			{
//				bitmapTextMesh.anchor = value;
//			}
//			else if (FontType == FontTypes.TRUETYPE)
			{
				// For some reason the anchor is flipped vertically so we need to translate.
				switch (value)
				{
					case TextAnchor.LowerCenter:	trueTypeTextMesh.anchor = TextAnchor.UpperCenter; break;
					case TextAnchor.LowerLeft:		trueTypeTextMesh.anchor = TextAnchor.UpperLeft; break;
					case TextAnchor.LowerRight:		trueTypeTextMesh.anchor = TextAnchor.UpperRight; break;
					case TextAnchor.UpperCenter:	trueTypeTextMesh.anchor = TextAnchor.LowerCenter; break;
					case TextAnchor.UpperLeft:		trueTypeTextMesh.anchor = TextAnchor.LowerLeft; break;
					case TextAnchor.UpperRight:		trueTypeTextMesh.anchor = TextAnchor.LowerRight; break;
					default:						trueTypeTextMesh.anchor = value; break;
				}
			}
		}
	}
#endregion

#region INITIALIZERS
	
	protected override void Awake()
	{
		base.Awake();
		
		UpdateInspectorProperties();
		
		ScaleCharacterSize();
	}
	
#endregion

#region FUNCTIONS
	
	/**
	 * Calculates and applies the actual scale of the character size with respect to device size and scale mode.
	 */
	private void ScaleCharacterSize()
	{
		if (m_charScaleMode == ScaleModeTypes.NONE ||
		    TargetScreenSize == Vector2.zero ||
		    (UICamera.pixelWidth == TargetScreenSize.x &&
		     UICamera.pixelHeight == TargetScreenSize.y))
		{
			return;
		}
		
		float size = m_characterSize;
		float screenScaleX = UICamera.pixelWidth / TargetScreenSize.x;
		float screenScaleY = UICamera.pixelHeight / TargetScreenSize.y;
		
		switch (m_charScaleMode)
		{
		case ScaleModeTypes.FILL:
			screenScaleX = screenScaleY = screenScaleX > screenScaleY ? screenScaleX : screenScaleY;
			break;
		case ScaleModeTypes.FIT:
			screenScaleX = screenScaleY = screenScaleX > screenScaleY ? screenScaleY : screenScaleX;
			break;
		case ScaleModeTypes.NONE:
			screenScaleX = 1.0f;
			screenScaleY = 1.0f;
			break;
		}
		
		size *= screenScaleX;
		
//		if (FontType == FontTypes.BITMAP)
//		{
//			bitmapTextMesh.scale = new Vector3(size, size, size);
//		}
//		else if (FontType == FontTypes.TRUETYPE)
		{
			trueTypeTextMesh.characterSize = size;
		}
	}
	
	private void UpdateInspectorProperties()
	{
		this.FontType = m_fontType;
		this.CharacterSize = m_characterSize;
//		this.FontBitmap = m_fontBitmap;
		this.FontTrueType = m_fontTrueType;
		this.Color = m_color;
		this.LineSpacing = m_lineSpacing;
		this.TabSize = m_tabSize;
		this.Text = m_text;
		this.TextAnchor = m_textAnchor;
		this.ScaleMode = ScaleModeTypes.NONE;
	}
	
	protected override void Update()
	{
		// If this is the editor, in edit mode, apply all inspector properties
		if (Application.isEditor && !Application.isPlaying)
		{
			UpdateInspectorProperties();
		}
		
		//ScaleCharacterSize();
		
		// To get the width and height we need to grab the mesh bounds and convert to screen
		// coordinates.
		Vector2 bSize = WorldToScreenPoint(MeshBounds.size);
		Width = bSize.x - (UICamera.pixelWidth / 2.0f);
		Height = bSize.y - (UICamera.pixelHeight / 2.0f);
		
		// Update the text in the appropriate mesh
		if (Text != null && Text.Length > 0)
		{			
			string translation = Localization.TranslateText(Text);
			
//			if (FontType == FontTypes.BITMAP)
//			{
//				bitmapTextMesh.maxChars = translation.Length;
//				bitmapTextMesh.text = translation;
//				
//				//Force BitmapTextMesh Rebuild
//				bitmapTextMesh.ForceBuild();
//			}
//			else if (FontType == FontTypes.TRUETYPE)
			{
				trueTypeTextMesh.text = translation;
			}
		}
		
		base.Update();
	}
	
#endregion
}
