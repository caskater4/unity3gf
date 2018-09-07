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
 * A Sprite3D is a two-dimensional texture that is displayed in three-dimensional space as a plane.
 * <p>
 * The texture to render to the surface of the plane is set via the texture property. If the desired texture is
 * contained within an atlas (aka: sprite sheet) then you must set the atlas property as well.
 * <p>
 * When using an atlas there must exist the data file with the same name as the atlas' texture.
 * See {@link TextureAtlas} for supported atlas data formats. Also, the offset and scale properties are ignored.
 * 
 * @author Jean-Philippe Steinmetz
 */
[ExecuteInEditMode]
public class Sprite3D : MonoBehaviour
{
	/**
	 * The name of the texture atlas to retrieve the texture from.
	 */
	public string atlas = null;
	
	/**
	 * The name of the texture to render on the sprite surface.
	 */
	public string texture = null;
	
	/**
	 * The UV coordinate offset to use when rendering the texture. Setting this property will have no effect when using
	 * a texture atlas.
	 */
	public Vector2 offset = Vector2.zero;
	
	/**
	 * The transform the sprite should always face.
	 */
	public Transform alwaysLookAt = null;
	
	/**
	 * The UV coordinate scale to use when rendering the texture. Setting this property will have no effect when using
	 * a texture atlas.
	 */
	public Vector2 scale = Vector2.one;
	
	/**
	 * The atlas containing a listing of all data for each frame within the atlas.
	 */
	protected TextureAtlas m_atlas = null;
	
	/**
	 * The underlying texture to render to the sprite surface.
	 */
	protected Texture m_texture = null;
	
	/**
	 * The name or path of the texture to render on the sprite surface.
	 */
	protected string m_texturePath = null;
	
	protected bool frameChanged = false;
	
	protected TextureAtlasFrame frame = null;
	
	protected virtual void Start()
	{
		string textureToLoad = texture;
		m_texturePath = texture;
		
		// If applicable, load the atlas data file
		if (atlas != null && atlas.Length > 0)
		{
			textureToLoad = atlas;
			LoadAtlas();
		}
		
		m_texture = (Texture) Asset.Load(textureToLoad, typeof(Texture));
		if (m_texture == null)
		{
			Debug.LogError("Texture not found: " + textureToLoad);
			return;
		}
	}
	
	/**
	 * Loads and parses the manifest file for the sprite sheet. Has the effect of populating
	 * m_frameData with all the necessary information contained in the manifest.
	 */
	private void LoadAtlas()
	{
		TextAsset asset = (TextAsset) Asset.Load(atlas, typeof(TextAsset));
		
		if (asset == null)
		{
			Debug.LogError("Atlas not found: " + atlas);
			return;
		}

		m_atlas = TextureAtlasUtil.GetTextureAtlas(asset, TextureAtlasFormat.COCOS2D);
		
		// Also make sure that the texture frame exists
		frame = m_atlas.GetFrame(m_texturePath);
		if (frame == null)
		{
			//Debug.LogError("Frame not found: " + m_texturePath);
		}
	}
	
	protected virtual void Update()
	{
		// When using an atlas, update the offset to reflect the uv coordinates of the texture frame
		// within the atlas
		if (m_atlas != null)
		{
			if(frameChanged) 
			{
				frame = m_atlas.GetFrame(m_texturePath);
				frameChanged = false;
			}
			
			if (frame != null)
			{
				float offsetX = frame.Position.x / (float)m_texture.width;
				float offsetY = 1.0f - ((frame.Position.y + (float)frame.Height) / (float)m_texture.height);
				offset.x = offsetX;
				offset.y = offsetY;
				
				float scaleX = (float)frame.Width / (float)m_texture.width;
				float scaleY = (float)frame.Height / (float)m_texture.height;
				scale.x = scaleX;
				scale.y = scaleY;
			}
			else
			{
//				Debug.LogError("Frame not found: " + m_texturePath + " on object: " + this.gameObject.name);
			}
		}
		
		// Assign the texture to the rendering material
		//used to be renderer.material
		Material mat = renderer.material;
		mat.mainTexture = m_texture;
		// Set the offset and size of the renderer's main texture
		mat.mainTextureOffset = offset;
		mat.mainTextureScale = scale;
		
		renderer.material = mat;
		
		if (alwaysLookAt != null)
		{
			transform.LookAt(alwaysLookAt);
		}
	}
}
