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
 * The GUIInputManager is responsible for determining the origin of input events in the GUI and notifying those
 * objects of any events. An origin is any game object that has a GUIObject component that has registered with this
 * manager. When an object has been determined to be the origin of an input event it is notified with one or more of
 * the following messages.
 * <p>
 * OnPress
 * OnRelease
 * OnRollOut
 * OnRollOver
 * 
 * @author Jean-Philippe Steinmetz
 */
public class GUIInputManager : MonoBehaviour
{
	/**
	 * The list of all GUI objects that have been registered for input events.
	 */
	private static List<GUIObject> allObjects = null;
	
	/**
	 * The list of GUI objects for each registered input event type.
	 */
	private static Dictionary<GUIEventType, List<GUIObject>> eventObjects = null;
	
	/**
	 * The last GUIObject that an event occurred with.
	 */
	private GUIObject lastObject = null;
	
	/**
	 * Indicates if the user is currently pressing a button or screen.
	 */
	private bool pressActive = false;
	
	/**
	 * The origin GUIObject where the press event began.
	 */
	private GUIObject pressOrigin = null;
	
	private bool useTouchInput = false;
	
	private List<bool> touchPressActives = new List<bool>();
	
	private List<GUIObject> touchLastObjects = null;
	
	private List<GUIObject> touchPressOrigins = null;
	
	private int lastTouchIndex = -1;
	
	private bool[] touchesDown = new bool[6];
	
	private void Awake()
	{
		allObjects = new List<GUIObject>();
		
		eventObjects = new Dictionary<GUIEventType, List<GUIObject>>();
		eventObjects.Add(GUIEventType.PRESS, new List<GUIObject>());
		eventObjects.Add(GUIEventType.RELEASE, new List<GUIObject>());
		eventObjects.Add(GUIEventType.ROLL_OUT, new List<GUIObject>());
		eventObjects.Add(GUIEventType.ROLL_OVER, new List<GUIObject>());
		//eventObjects.Add(GUIEventType.SWIPE_OFF, new List<GUIObject>());
		//eventObjects.Add(GUIEventType.SWIPE_ON, new List<GUIObject>());
		
		
		// should we use gui input or mouse input?
		useTouchInput = Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
	}
	
	private void UpdateTouch()
	{
		if (Input.touches.Length > 0) 
		{
			for (int i=0; i<Input.touches.Length; i++)
			{
				Dictionary<string, GUIObject> events = new Dictionary<string, GUIObject>();
				Touch touch = Input.GetTouch( i );
				
				GUIObject obj = GetObjectAt(touch.position);
				
				//consider touch is down if touch is not canceled or ended.
				bool isDown = touch.phase != TouchPhase.Canceled &&  touch.phase != TouchPhase.Ended;	
				
				if (obj != null)
				{
					if (touch.phase == TouchPhase.Began)
					{
						//only track last touch roll over/out
						lastTouchIndex = i;
						lastObject = obj;
						
						touchesDown[i] = true;
						
						events.Add("OnPress", obj);
					}
					else if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
					{
						//if touch was down
						if (touchesDown[i])
						{
							touchesDown[i] = false;
							
							events.Add("OnRelease", obj);
						}
					}
					else if (touch.phase == TouchPhase.Moved)
					{
						if (lastTouchIndex == i)
						{
							if (obj != lastObject)
							{
								if (lastObject != null)
								{
									//if touch was down
									if (touchesDown[i])
									{
										touchesDown[i] = false;
										
										events.Add("OnRollOut", lastObject);
									}
								}
								
								events.Add("OnRollOver", obj);
							}
						}
						else
						{
							//if touch was down
							if (touchesDown[i])
							{
								//release if not last touch
								touchesDown[i] = false;
								
								events.Add("OnRollOut", obj);
							}
						}
					}
					else if (touch.phase == TouchPhase.Stationary)
					{
						//release if not last touch
						if (lastTouchIndex != i)
						{
							//if touch was down
							if (touchesDown[i])
							{
								touchesDown[i] = false;
								
								events.Add("OnRollOut", obj);
							}
						}
					}
				}
				else
				{
					if (lastTouchIndex == i)
					{
						if (lastObject != null)
						{
							//if touch was down
							if (touchesDown[i])
							{
								touchesDown[i] = false;
								
								events.Add("OnRollOut", lastObject);
							}
						}
					}
				}
				
				lastObject = obj;
				
				//send message notifs:
				SendEventMessages( events );
			}
		}
	}
	
	private void SendEventMessages( Dictionary<string,GUIObject> events)
	{
		// If any events were found, notify each object
		foreach (KeyValuePair<string, GUIObject> evt in events)
		{
			//Debug.Log("[" + Time.time + "] Firing " + evt.Key + " on " + evt.Value.gameObject.name);
			evt.Value.SendMessage(evt.Key, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	private void UpdateMouse()
	{
		Dictionary<string, GUIObject> events = new Dictionary<string, GUIObject>();
		Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		
		// Get the top most object underneath the current mouse position
		GUIObject obj = GetObjectAt(mousePosition);
		
		// When there is a new press action we want to mark the origin object
		if (!pressActive && Input.GetMouseButtonDown(0))
		{
			pressOrigin = obj;
		}
		
		if (obj != null)
		{
			// Do we have an OnPress event?
			if (Input.GetMouseButtonDown(0) && pressOrigin == obj)
			{
				events.Add("OnPress", obj);
			}
			// Do we have an OnRelease event?
			else if (Input.GetMouseButtonUp(0) && pressOrigin == obj)
			{
				events.Add("OnRelease", obj);
			}
			else if (lastObject != obj)
			{
				if (lastObject != null)
				{
					events.Add("OnRollOut", lastObject);
				}
				
				events.Add("OnRollOver", obj);
			}
		}
		else if (lastObject != null)
		{
			events.Add("OnRollOut", lastObject);
		}
		
		lastObject = obj;
		pressActive = Input.GetMouseButtonDown(0);
		pressOrigin = Input.GetMouseButtonUp(0) ? null : pressOrigin;
		
		// If any events were found, notify each object
		foreach (KeyValuePair<string, GUIObject> evt in events)
		{
			//Debug.Log("[" + Time.time + "] Firing " + evt.Key + " on " + evt.Value.gameObject.name);
			evt.Value.SendMessage(evt.Key, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	
	/**
	 * Checks for new input events every tick, notifying affect objects as necessary.
	 */
	private void Update()
	{
		if (useTouchInput)
		{
			UpdateTouch();
		}
		else
		{
			UpdateMouse();
		}
	}
	
	/**
	 * Returns the top most object found lying underneath the given coordinates.
	 * 
	 * @param coords The x and y coordinates to search for objects.
	 * @return The object found to lie directly beneath the specified coordinates, otherwise null.
	 */
	private static GUIObject GetObjectAt(Vector2 coords)
	{
		return GetObjectAt(coords.x, coords.y);
	}
	
	/**
	 * Returns the top most object found lying underneath the given coordinates.
	 * 
	 * @param x The x coordinate.
	 * @param y The y coordinate.
	 * @return The object found to lie directly beneath the specified coordinates, otherwise null.
	 */
	private static GUIObject GetObjectAt(float x, float y)
	{
		GUIObject candidate = null;
		
		// Go through each registered object to determine if the position falls within their bounding box.
		// If it does, the object is a possible candidate. The true candidate is the object with the lowest
		// depth.
		foreach (GUIObject obj in allObjects)
		{
			if (x >= obj.BoundingBox.min.x && x <= obj.BoundingBox.max.x &&
			    y >= obj.BoundingBox.min.y && y <= obj.BoundingBox.max.y)
			{
				// We have a candidate! Finally make sure the depth is lower than our current option.
				if (candidate == null || obj.Depth < candidate.Depth)
				{
					candidate = obj;
				}
			}
		}
		
		return candidate;
	}
	
	/**
	 * Registers the provided object to be considered as an origin of input events for all types.
	 * 
	 * @param obj The object to register for all input event types.
	 */
	public static void Register(GUIObject obj)
	{
		Register(obj, GUIEventType.PRESS);
		Register(obj, GUIEventType.RELEASE);
		Register(obj, GUIEventType.ROLL_OUT);
		Register(obj, GUIEventType.ROLL_OVER);
		//Register(obj, GUIEventType.SWIPE_OFF);
		//Register(obj, GUIEventType.SWIPE_ON);
	}
	
	/**
	 * Registers the provided object to be considered as an origin of input events of a particular type.
	 * 
	 * @param obj The object to register for the event type.
	 * @param type The event type to register the object for.
	 */
	public static void Register(GUIObject obj, GUIEventType type)
	{
		// Add the object to allObjects if not done so yet
		if (!allObjects.Contains(obj))
		{
			allObjects.Add(obj);
		}
		
		// No need to register an object that's already been registered.
		if (eventObjects[type].Contains(obj))
		{
			return;
		}
		
		eventObjects[type].Add(obj);
	}
	
	/**
	 * Removes the provided object from consideration when determining the origin of events for any type of input.
	 * 
	 * @param obj The object to remove registration of all event types.
	 */
	public static void UnRegister(GUIObject obj)
	{
		UnRegister(obj, GUIEventType.PRESS);
		UnRegister(obj, GUIEventType.RELEASE);
		UnRegister(obj, GUIEventType.ROLL_OUT);
		UnRegister(obj, GUIEventType.ROLL_OVER);
		//UnRegister(obj, GUIEventType.SWIPE_OFF);
		//UnRegister(obj, GUIEventType.SWIPE_ON);
	}
	
	/**
	 * Removes the provided object from consideration when determining the origin of events for a particular
	 * type of input.
	 * 
	 * @param obj The object to remove registration of the event type.
	 * @param type The event type to remove registration of the object for.
	 */
	public static void UnRegister(GUIObject obj, GUIEventType type)
	{
		if (eventObjects != null && eventObjects.ContainsKey(type) && eventObjects[type].Contains(obj))
		{
			eventObjects[type].Remove(obj);
		}
		
		// Remove the object from allObjects but only if no other event registrations exist
		if (allObjects != null && allObjects.Contains(obj) &&
		    !eventObjects[GUIEventType.PRESS].Contains(obj) &&
		    !eventObjects[GUIEventType.RELEASE].Contains(obj) &&
		    !eventObjects[GUIEventType.ROLL_OUT].Contains(obj) &&
		    !eventObjects[GUIEventType.ROLL_OVER].Contains(obj)/* &&
			!eventObjects[GUIEventType.SWIPE_OFF].Contains(obj) &&
			!eventObjects[GUIEventType.SWIPE_ON].Contains(obj)*/)
		{
			allObjects.Remove(obj);
		}
	}
}
