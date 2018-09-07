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
 * The ToggleButton is a type of button that when pressed toggles to a different displayed state. When the button
 * is pressed a second time it displays the original state and so on.
 * 
 * @author Jean-Philippe Steinmetz
 */
[AddComponentMenu("GUI/Toggle Button")]
[ExecuteInEditMode]
public class ToggleButton : Button
{
	/**
	 * The texture to display when the button is active.
	 */
	public string activeTexture = null;
	
	/**
	 * Indicates if the button is currently active.
	 */
	public bool isActive = false;
	
	private Texture m_activeTexture = null;
	private Texture m_inactiveTexture = null;
	
	protected override void Awake()
	{
		base.Awake();
		
		if (m_activeTexture == null && activeTexture != null && activeTexture.Length > 0)
		{
			m_activeTexture = (Texture) Asset.Load(activeTexture, typeof(Texture));
			if (m_activeTexture == null)
			{
				Debug.LogError("[" + this.gameObject.name + "]Texture not found: " + activeTexture);
			}
		}
	}
	
	protected override void Start()
	{
		base.Start();
		
		m_inactiveTexture = m_offTexture;
	}
	
	protected override void OnRelease()
	{
		base.OnRelease();
		
		// Switch the state of the button
		isActive = !isActive;
		
		// Set the correct off state texture
		if (isActive)
		{
			m_offTexture = m_activeTexture;
		}
		else
		{
			m_offTexture = m_inactiveTexture;
		}
	}
}