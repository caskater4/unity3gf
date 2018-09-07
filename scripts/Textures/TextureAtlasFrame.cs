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
 * Represents the data for a particular frame of a sprite atlas.
 * 
 * @author Jean-Philippe Steinmetz
 */
public class TextureAtlasFrame
{
	/**
	 * The height of the frame within the sprite sheet.
	 */
	public float Height { get; set; }
	
	/**
	 * The name of the frame. This is often the original source file name.
	 */
	public string Name { get; set; }
	
	/**
	 * The position of the frame within the sprite sheet.
	 */
	public Vector2 Position { get; set; }
	
	/**
	 * The original height of the frame's source file.
	 */
	public float SourceHeight { get; set; }
	
	/**
	 * The original width of the frame's source file.
	 */
	public float SourceWidth { get; set; }
	
	/**
	 * Indicates whether the frame's original image has been rotated within the sprite sheet.
	 */
	public bool Rotated { get; set; }
	
	/**
	 * The width of the frame within the sprite sheet.
	 */
	public float Width { get; set; }
	
	/**
	 * Creates a new TextureAtlasFrame instance.
	 */
	public TextureAtlasFrame()
	{
	}
}
