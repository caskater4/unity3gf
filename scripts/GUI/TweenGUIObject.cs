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
using System.Collections.Generic;

/**
 * Performs a tween of a specified GUIObject property.
 * 
 * @author Jean-Philippe Steinmetz
 */
[AddComponentMenu("GUI/Animation/Tween")]

public class TweenGUIObject : MonoBehaviour
{
#region INTERNAL_STRUCTURES
	
	/**
	 * Describes the different types of properties that can be tweened.
	 */
	public enum PropertyType
	{
		POSITION,
		SCALE, 
		ROTATION
	}
	
	private string[] propertyStringType = new string[] { "position", "scale", "rotation" };
	
#endregion
	
#region INSPECTOR_PROPERTIES
	
	public bool m_autoPlay = true;
	public float m_delay = 0;
	public iTween.EaseType m_easeType = iTween.EaseType.easeInQuad;
	public bool m_ignoreTimeScale = false;
	public string m_name = null;
	public PropertyType m_property = PropertyType.POSITION;
	public Vector2 m_propertyFrom = Vector2.zero;
	public Vector2 m_propertyTo = Vector2.zero;
	public float m_time = 0;
	public bool m_useCurrentValue = false;
	
	public bool isPlaying = false;
	
#endregion
	
#region VARIABLES
	
	protected GUIObject m_guiObject = null;
	protected bool m_pendingStart = false;
	
	protected Hashtable args;
	
	protected static List<TweenGUIObject> tweenGUIList = null;
	
#endregion
	
#region PROPERTIES
	
	/**
	 * A reference to the GUIObject component attached to this game object.
	 */
	protected GUIObject guiObject
	{
		get
		{
			if (m_guiObject	== null)
			{
				m_guiObject = (GUIObject) GetComponent<GUIObject>();
				
				if (m_guiObject == null)
				{
					Debug.LogError(this.gameObject.name + " is not a GUIObject!");
				}
			}
			
			return m_guiObject;
		}
	}
	
	/**
	 * Returns true if the animation is currently playing, otherwise false.
	 */
	public bool IsPlaying
	{
		get
		{
			return isPlaying;
		}
		protected set
		{
			isPlaying = value;
		}
	}
	
	/**
	 * The unique name of the tween.
	 */
	public string Name
	{
		get
		{
			return m_name;
		}
		set
		{
			m_name = value;
		}
	}
	
#endregion
	
#region INITIALIZERS
	
	protected virtual void Start()
	{
		if (tweenGUIList == null)
		{
			tweenGUIList = new List <TweenGUIObject>();
		}
		
		tweenGUIList.Add( this );
		
		if (m_autoPlay)
		{
			Play();
		}
	}
	
#endregion
	
#region FUNCTIONS
	
	/**
	 * Starts the tween animation.
	 */
	public void Play()
	{
		if (IsPlaying) return;
		
		//Debug.Log("Playing tween " + Name + " for " + this.gameObject.name);
		
		// Create the list of arguments to pass to iTween
		args = new Hashtable();
		args.Add("name", m_name);
		args.Add("from", 0.0f);
		args.Add("to", 1.0f);
		args.Add("time", m_time);
		args.Add("delay", m_delay);
		args.Add("easetype", m_easeType);
		args.Add("onstart", "OnGUITweenStart");
		args.Add("onstarttarget", this.gameObject);
		//pass this to tell callback who called it
		args.Add("onstartparams", this );
		args.Add("onupdate", "OnGUITweenUpdate");
		args.Add("onupdatetarget", this.gameObject);
		//pass this to tell callback who called it
		args.Add("onupdateparams", this );
		args.Add("oncomplete", "OnGUITweenComplete");
		args.Add("oncompletetarget", this.gameObject);
		//pass this to tell callback who called it
		args.Add("oncompleteparams", this );
		args.Add("ignoretimescale", m_ignoreTimeScale);
		
		//stops other tweens of same type:
		StopOtherTweens();
		
		// Start the tweening!
		iTween.ValueTo(this.gameObject, args);
		
		// We need to actually wait one frame before setting IsPlaying to true otherwise we get bad behavior.
		m_pendingStart = true;
	}
	
	protected virtual void StopOtherTweens()
	{
		for (int i=0; i<tweenGUIList.Count; i++)
		{
			if (tweenGUIList[i] == null)
			{
				tweenGUIList.RemoveAt( i-- );
			}
			else
			{
				//if another tween is playing, and the tween is of this tweeen's same type, stop it
				if (tweenGUIList[i].gameObject == this.gameObject && tweenGUIList[i].IsPlaying && tweenGUIList[i].m_property == this.m_property)
				{
					tweenGUIList[i].Stop();
				}
			}
		}
	}
	
	/**
	 * Called by iTween when the animation finishes.
	 */
	protected void OnGUITweenComplete( object tweenGUIObject)
	{
		//if this message wasn't intended for this component, ie this instance wasn't source of tween, ignore:
		TweenGUIObject tweenSource = tweenGUIObject as TweenGUIObject;
		if (this != tweenSource ) 
		{
			return;
		}
		
		if (!IsPlaying) return;
		
		Stop();
		
		// Notify any local listeners
		this.SendMessage("OnAnimEnd", this.Name, SendMessageOptions.DontRequireReceiver);
	}
	
	/**
	 * Called by iTween when the animation starts.
	 */
	protected virtual void OnGUITweenStart( object tweenGUIObject )
	{
		//if this message wasn't intended for this component, ie this instance wasn't source of tween, ignore:
		TweenGUIObject tweenSource = tweenGUIObject as TweenGUIObject;
		if (this != tweenSource ) 
		{
			return;
		}
		
		if (m_pendingStart)
		{
			m_pendingStart = false;
			IsPlaying = true;
		}
		
		if (!IsPlaying) return;

		if (m_useCurrentValue)
		{
			if (m_property == PropertyType.POSITION)
			{
				m_propertyFrom = guiObject.Position;
			}
			else if (m_property == PropertyType.SCALE)
			{
				m_propertyFrom = guiObject.Scale;
			}
			else if (m_property == PropertyType.ROTATION)
			{
				Quaternion rot = Quaternion.FromToRotation( Vector3.back, Vector3.up ) * transform.rotation;
				m_propertyFrom.x = rot.eulerAngles.y;
			}
		}
		else
		{
			if (m_property == PropertyType.POSITION)
			{
				guiObject.Position = m_propertyFrom;
			}
			else if (m_property == PropertyType.SCALE)
			{
				guiObject.Scale = m_propertyFrom;
			}
			else if (m_property == PropertyType.ROTATION)
			{
				transform.rotation = Quaternion.FromToRotation( Vector3.up, Vector3.back ) * Quaternion.AngleAxis( m_propertyFrom.x, Vector3.up );
			}
		}
		
		// Notify any local listeners
		this.SendMessage("OnAnimStart", this.Name, SendMessageOptions.DontRequireReceiver);
	}
	
	/**
	 * Called by iTween to update the tween's progress.
	 */
	protected virtual void OnGUITweenUpdate( object paramArray )
	{
		//first item in array is value, followed by onupdateparams array items
		//in this case, we passed ourself, so check if this source call is from ourself
		//if not return
		TweenGUIObject tweenSource = ((object[])paramArray)[0] as TweenGUIObject;
		if (this != tweenSource)
		{
			return;
		}
		
		//first item in array is value, get value of progress:
		float progress = (float)((object[])paramArray)[1];
		
		if (!IsPlaying) return;
		
		UpdateTween( progress );
	}
	
	protected void UpdateTween( float progress )
	{
		Vector2 delta = m_propertyTo - m_propertyFrom;
		delta *= progress;
		
		if (m_property == PropertyType.POSITION)
		{
			guiObject.Position = m_propertyFrom + delta;
		}
		else if (m_property == PropertyType.SCALE)
		{
			guiObject.Scale = m_propertyFrom + delta;
		}
		else if (m_property == PropertyType.ROTATION)
		{
			transform.rotation = Quaternion.FromToRotation( Vector3.up, Vector3.back ) * Quaternion.AngleAxis( m_propertyFrom.x + delta.x, Vector3.up );
		}
	}
	
	/**
	 * Stops playback of the animation.
	 */
	public void Stop()
	{
		IsPlaying = false;
		m_pendingStart = false;
		
		// Notify any local listeners
		this.SendMessage("OnAnimStop", this.Name, SendMessageOptions.DontRequireReceiver);
	}
	
#endregion
}
