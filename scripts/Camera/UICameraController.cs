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

/**
 * The UICameraController redirects the output of the Camera's viewport to be drawn via a RenderTexture. The
 * RenderTexture is created using the current Screen resolution and then is drawn to Unity's GUI layer. If
 * the resolution changes during runtime, the render texture will also be updated. Should the Camera already
 * have a render texture specified, the screen does not generate a new one and does not change with screen
 * resolution changes.
 * 
 * @author Jean-Philippe Steinmetz
 */
public class UICameraController : MonoBehaviour
{
	/**
	 * Set to true to enable alpha blending when drawing the GUI, otherwise set to false.
	 */
	public bool alphaBlending = true;
	
	/**
	 * Indicates if the render texture has been generated.
	 */
	private bool generatedTexture = true;
	
	/**
	 * The render texture to use.
	 */
	private RenderTexture renderTexture = null;
	
	/**
	 * The screen's height used to generate the render texture.
	 */
	private float screenHeight = 0.0f;
	
	/**
	 * The screen's width used to generate the render texture.
	 */
	private float screenWidth = 0.0f;
	
	private void Start()
	{
		// If the camera already has a render texture set use that instead, do not generate one.
		if (this.camera.targetTexture != null)
		{
			renderTexture = this.camera.targetTexture;
			generatedTexture = false;
		}
		
		// If no render texture has been set we need to generate one
		if (renderTexture == null)
		{
			CreateRenderTexture();
		}
	}
	
	private void CreateRenderTexture()
	{
		// Create the render texture
		renderTexture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.Default);
		renderTexture.Create();
		this.camera.targetTexture = renderTexture;
		
		// Save the width and height so we can later update if it changes
		screenHeight = Screen.height;
		screenWidth = Screen.width;
	}
	
	private void OnGUI()
	{
		if (renderTexture == null) return;
		GUI.DrawTexture(new Rect(0, 0, renderTexture.width, renderTexture.height),
		                renderTexture,
		                ScaleMode.StretchToFill,
		                alphaBlending, 0);
	}
	
	private void Update()
	{
		// If the render texture was generated and the screen resolution has changed, we need to recreate it.
		if (!generatedTexture && Screen.height != screenHeight || Screen.width != screenWidth)
		{
			CreateRenderTexture();
		}
	}
}
