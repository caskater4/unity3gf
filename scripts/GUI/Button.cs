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
using System.Collections;

/**
 * Button is a type of Sprite that displays different textures when a user presses on the sprite itself.
 * <p>
 * When the user presses on a button, OnPress and OnRelease messages are sent to the game object.
 * 
 * @author Jean-Philippe Steinmetz
 */
[AddComponentMenu("GUI/Button")]
[ExecuteInEditMode]
public class Button : Sprite
{
	/**
	 * The length of time, in seconds, that an event's state will be shown.
	 */
	public float eventDuration = 0.1f;
	
	/**
	 * The texture to display when the button is rolled over.
	 */
	public string overTexture = "";
	
	/**
	 * The texture to display when the button is pressed.
	 */
	public string downTexture = "";
	
	/**
	 * The texture to display when the button is released.
	 */
	public string upTexture = "";
	
	public string offTexture = "";
	
	protected Texture m_downTexture = null;
	protected Texture m_offTexture = null;
	protected Texture m_overTexture = null;
	protected Texture m_upTexture = null;
	private float m_eventStartTime = -1.0f;
	private bool m_pressed = false;
	
	/**
	 * Indicates if the button is currently being pressed.
	 */
	public bool IsPressed
	{
		get
		{
			return m_pressed;
		}
	}
	
	protected override void Start()
	{
		base.Start();
		
		offTexture = texture;
		
		if (m_atlas == null)
		{
			m_offTexture = m_texture;
			
			// Load the down texture
			if (m_downTexture == null)
			{
				m_downTexture = PreloadTexture( downTexture );
			}
			
			// Load the over texture
			if (m_overTexture == null)
			{
				m_overTexture = PreloadTexture( overTexture );
			}
			
			// Load the up texture
			if (m_upTexture == null)
			{
				m_upTexture = PreloadTexture( upTexture );
			}
		}
	}
	
	protected virtual void OnPress()
	{
		if (m_downTexture != null)
		{
			m_texture = m_downTexture;
		}
		else if (downTexture != null && downTexture.Length > 0)
		{
			Texture = downTexture;
		}
		
		m_eventStartTime = Time.time;
		m_pressed = true;
		//Debug.Log("Button(" + this.gameObject.name + "): OnPress! " + m_eventStartTime);
	}
	
	protected virtual void OnRelease()
	{
		if (m_upTexture	!= null)
		{
			m_texture = m_upTexture;
		}
		else if (upTexture != null && upTexture.Length > 0)
		{
			Texture = upTexture;
		}
		
		m_eventStartTime = Time.time;
		m_pressed = false;
		//Debug.Log("Button(" + this.gameObject.name + "): OnRelease! " + m_eventStartTime);
	}
	
	protected virtual void OnRollOut()
	{
		if (m_atlas == null)
		{
			m_texture = m_offTexture;
		}
		
		Texture = offTexture;
		m_eventStartTime = -1.0f;
		m_pressed = false;
		//Debug.Log("Button(" + this.gameObject.name + "): OnRollOut!");
	}
	
	protected virtual void OnRollOver()
	{
		if (m_overTexture != null)
		{
			m_texture = m_overTexture;
		}
		else if (overTexture != null && overTexture.Length > 0)
		{
			Texture = overTexture;
		}
		//Debug.Log("Button(" + this.gameObject.name + "): OnRollOver!");
	}
	
	protected override void Update()
	{
		if (!m_pressed && m_eventStartTime > 0 && m_eventStartTime + eventDuration < Time.time)
		{
			OnRollOut();
		}
		
		base.Update();
	}
}
