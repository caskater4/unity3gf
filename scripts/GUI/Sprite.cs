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
using System.Collections.Generic;
using System.Collections;

/**
 * A Sprite is a 2D texture that is displayed on the GUI layer of the scene. Sprite supports rendering textures as-is
 * or as sub-textures from an atlas.
 * <p>
 * When using an atlas there must exist the data file with the same name as the atlas' texture.
 * See {@link TextureAtlas} for supported atlas data formats.
 * <p>
 * This script requires MeshRenderer and MeshFilter components. If either is missing they will be created. If the
 * MeshFilter does not have a Mesh specified, one is generated for the sprite.
 * 
 * @author Jean-Philippe Steinmetz
 */
[AddComponentMenu("GUI/Sprite")]

[ExecuteInEditMode]
public class Sprite : GUIObject
{
	/**
	 * The name of the texture atlas to retrieve the texture from.
	 */
	public string atlas = "";
	
	/**
	 * The name of the texture to render on the sprite surface.
	 */
	public string texture = "";
	
	public bool loadOnAwake = true;
	
	private string currentTexture = "";
	
	/**
	 * The UV coordinate offset to use when rendering the texture. Setting this property will have no effect when using
	 * a texture atlas.
	 */
	public Vector2 uvOffset = Vector2.zero;
	
	/**
	 * The UV coordinate scale to use when rendering the texture. Setting this property will have no effect when using
	 * a texture atlas.
	 */
	public Vector2 uvScale = Vector2.one;
	
	/**
	 * The atlas containing a listing of all data for each frame within the atlas.
	 */
	protected TextureAtlas m_atlas = null;
	
	/**
	 * The underlying texture to render to the sprite surface.
	 */
	protected Texture m_texture = null;
	
	public bool isAtlasDirty = false;
	
	private bool isFirstUpdate = true;
	
	protected static List<Sprite> spriteList = null;
	
	protected static Dictionary<string, Texture> loadedTextures = null;
	
	protected static Dictionary<string, TextureAtlas> loadedAtlases = null;
	
	protected static bool isCoroutineStarted = false;
	
	
	/**
	 * The name or path of the texture to render on the sprite surface.
	 */
	public string Texture
	{
		get
		{
			return currentTexture;
		}
		set
		{
			currentTexture = value;
		}
	}
	
	public string Atlas
	{
		get
		{
			return atlas;
		}
		set
		{
			string newAtlas = value;
			if (!newAtlas.Equals(atlas))
			{
				isAtlasDirty = true;
			}
			atlas = newAtlas;
		}
	}
	
	protected override void Awake()
	{
		Texture = texture;
		
		base.Awake();
		
		if (this.gameObject.active && !isCoroutineStarted)
		{
			isCoroutineStarted = true;
			StartCoroutine( LoadOnNextFrame() );
		}
		
		if (loadedTextures == null)
		{
			loadedTextures = new Dictionary<string, Texture>();
			loadedAtlases = new Dictionary<string, TextureAtlas>();
			spriteList = new List<Sprite>();
		}
		
		if (loadOnAwake)
		{
			Load();
		}
		else
		{
			spriteList.Add( this );	
		}
	}
	
	protected IEnumerator LoadOnNextFrame()
	{
		yield return 0;
		
		for (int i=0; i<spriteList.Count; i++)
		{
			spriteList[i].Load();
			Debug.Log("   -late load");
		}
	}
	
	/**
	 * Loads the currently set texture and atlas (if set).
	 */
	public void Load()
	{		
		string textureToLoad = Texture;
		
		// If applicable, load the atlas data file
		if (atlas != null && atlas.Length > 0)
		{
			textureToLoad = atlas;
			
			//if atlas isn't loaded or atlas has been changed, load atlas
			if (m_atlas	== null || isAtlasDirty)
			{
				loadedAtlases.TryGetValue( atlas, out m_atlas );
				if (m_atlas == null)
				{
					LoadAtlas();
					if (m_atlas != null)
					{
						loadedAtlases.Add( atlas, m_atlas );
					}
				}
				
			}
		}
		
		if (textureToLoad != null && textureToLoad.Length > 0)
		{
			isAtlasDirty = false;
			
			loadedTextures.TryGetValue( textureToLoad, out m_texture );
			if (m_texture == null)
			{
				m_texture = PreloadTexture( textureToLoad );
				if (m_texture != null)
				{
					loadedTextures.Add( textureToLoad, m_texture );
				}
			}
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
			Debug.LogError("[" + this.gameObject.name + "]Atlas not found: " + atlas);
			return;
		}

		m_atlas = TextureAtlasUtil.GetTextureAtlas(asset, TextureAtlasFormat.COCOS2D);
	}
	
	/**
	 * Load the current texture frame from the atlas.
	 */
	private TextureAtlasFrame LoadAtlasFrame()
	{
		TextureAtlasFrame frame = m_atlas.GetFrame(Texture);
		
		if (frame == null)
		{
			Debug.LogError("[" + this.gameObject.name + "]Frame not found: " + Texture);
		}
		
		return frame;
	}
	
	protected override void Update()
	{

		// If for some reason our texture wasn't loaded at the start let's try to load it now
		if (isAtlasDirty || isFirstUpdate && m_texture == null && texture != null && texture.Length > 0)
		{
			Debug.Log("loading due to dirtiness or first update");
			Load();
		}
		
		if (m_atlas != null)
		{
			TextureAtlasFrame frame = LoadAtlasFrame();
			
			if (frame != null)
			{
				float offsetX = frame.Position.x / (float)m_texture.width;
				float offsetY = 1.0f - ((frame.Position.y + (float)frame.Height) / (float)m_texture.height);
				uvOffset = new Vector2(offsetX, offsetY);
				
				float scaleX = (float)frame.Width / (float)m_texture.width;
				float scaleY = (float)frame.Height / (float)m_texture.height;
				uvScale = new Vector2(scaleX, scaleY);
				
				Height = frame.Height;
				Width = frame.Width;
			}
		}
		else if (m_texture != null)
		{
			Height = m_texture.height;
			Width = m_texture.width;
		}
		
		// Assign the texture to the rendering material
		renderer.material.mainTexture = m_texture;
		
		// Set the offset and size of the renderer's main texture
		renderer.material.mainTextureOffset = uvOffset;
		renderer.material.mainTextureScale = uvScale;
		//used to be renderer.material
		
		base.Update();
	}
	
	protected static Texture PreloadTexture(string name)
	{
		Texture texture = null;
		// Load the texture
		if (name != null && name.Length > 0)
		{
			texture = (Texture) Asset.Load(name, typeof(Texture));
			if (texture == null)
			{
				Debug.LogError("Texture not found: " + name);
			}
			else
			{
				Debug.Log("Preloaded texture: " + name );
			}
		}
		return texture;
	}
}

