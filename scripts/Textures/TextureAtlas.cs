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
using System.Collections.Generic;
using UnityEngine;

/**
 * TextureAtlas is a special type of texture that contains other textures.
 * 
 * @author Jean-Philippe Steinmetz
 */
public abstract class TextureAtlas
{
	/**
	 * The list of frames contained in the atlas.
	 */
	public List<TextureAtlasFrame> Frames
	{
		get;
		protected set;
	}
	
	/**
	 * The name of the sprite atlas.
	 */
	public string Name
	{
		get;
		protected set;
	}
	
	/**
	 * Creates a new TextureAtlas instance.
	 * <p>
	 * Extending classes using this constructor must make sure to call ParseAsset.
	 */
	protected TextureAtlas()
	{
	}
	
	/**
	 * Creates a new TextureAtlas instance.
	 * 
	 * @param asset The asset containing the sprite atlas data.
	 */
	public TextureAtlas(TextAsset asset)
	{
		ParseAsset(asset);
	}
	
	/**
	 * Returns the frame at the specified index.
	 * 
	 * @param index The index of the frame to retrieve.
	 * @return The frame at the given index, otherwise null.
	 */
	public TextureAtlasFrame GetFrame(int index)
	{
		if (index < 0 || index >= Frames.Count) return null;
		
		return Frames[index];
	}
	
	/**
	 * Returns the first frame with the specified name.
	 * 
	 * @param name The name of the frame to retrieve.
	 * @return The frame with the given name, otherwise null.
	 */
	public TextureAtlasFrame GetFrame(string name)
	{
		return GetFrame(name, 0);
	}
	
	/**
	 * Returns the first frame with the specified name. The search begins at the provided index.
	 * 
	 * @param name The name of the frame to retrieve.
	 * @param startIdx The index to begin the search from.
	 * @return The frame with the given name, otherwise null.
	 */
	public TextureAtlasFrame GetFrame(string name, int startIdx)
	{
		if (name == null) return null;
		
		if (startIdx < 0) startIdx = 0;
		
		for (; startIdx < Frames.Count; ++startIdx)
		{
			TextureAtlasFrame frame = Frames[startIdx];
			
			if (name == frame.Name)
			{
				return frame;
			}
		}
		
		return null;
	}
	
	/**
	 * Parses the provided asset into a list of TextureAtlasFrame objects.
	 * 
	 * @param asset The sprite atlas text file to parse.
	 */
	protected abstract void ParseAsset(TextAsset asset);
}