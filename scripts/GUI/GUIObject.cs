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
 * GUIObject is the base class for all scripts or components that intend to provide GUI functionality to game objects.
 * 
 * @author Jean-Philippe Steinmetz
 */
public abstract class GUIObject : MonoBehaviour
{
#region STATICS
	/**
	 * The maximum depth that a GUI object can be.
	 */
	private static int MAX_DEPTH = 100;
	
	/**
	 * A reference to the UI's uiCamera component for faster reference.
	 */
	private static Camera uiCamera = null;
#endregion

#region INTERNAL STRUCTURES
	
	/**
	 * The various anchor types to use when scaling and positioning an object.
	 */
	public enum AnchorTypes
	{
		BottomLeft,
		BottomCenter,
		BottomRight,
		MiddleLeft,
		MiddleCenter,
		MiddleRight,
		TopLeft,
		TopCenter,
		TopRight
	}
	
	/**
	 * The types of alignment along the x axis that an object's position is to be calculated with respect to.
	 */
	public enum HorizontalAlignmentTypes
	{
		LEFT,
		CENTER,
		RIGHT
	}
	
	/**
	 * The types of alignment along the x axis that an object's position is to be calculated with respect to.
	 */
	public enum VerticalAlignmentTypes
	{
		TOP,
		MIDDLE,
		BOTTOM
	}
	
	/**
	 * The types of units to use when calculating a position relative to the screen.
	 */
	public enum PositionUnitTypes
	{
		PERCENTAGE,
		PIXELS
	}
	
	/**
	 * The method types to use when scaling an object from the target screen resolution to the actual screen
	 * resolution.
	 */
	public enum ScaleModeTypes
	{
		/**
		 * Does not perform any scaling.
		 */
		NONE,
		
		/**
		 * Scales the object, maintaining the aspect ratio, such that the entire screen is filled, potentially
		 * causing some of the object to be cropped out of view.
		 */
		FILL,
		
		/**
		 * Scales the object, maintaining the aspect ratio, such that the entire image fits within the screen.
	 	 * This can create a letterboxing around the object.
	 	 */
		FIT,
		
		/**
		 * Scales the object to fill the entire screen without cropping or letterboxing, taking the aspect ratio
	 	 * of the screen.
	 	 */
		STRETCH
	}
	
#endregion

#region INSPECTOR PROPERTIES
	
	/**
	 * Do not modify this variable directly! Use the Anchor property instead.
	 */
	public AnchorTypes m_anchor = AnchorTypes.MiddleCenter;
	
	/**
	 * Do not modify this variable directly! Use the Depth property instead.
	 */
	public int m_depth = 0;
	
	/**
	 * Do not modify this variable directly! Use the HorizontalAlignment property instead.
	 */
	public HorizontalAlignmentTypes m_horizontalAlignment = HorizontalAlignmentTypes.LEFT;
	
	/**
	 * Do not modify this variable directly! Use the Position property instead.
	 */
	public Vector2 m_position = Vector2.zero;
	
	/**
	 * Do not modify this variable directly! Use the PositionUnits property instead.
	 */
	public PositionUnitTypes m_positionUnits = PositionUnitTypes.PIXELS;
	
	/**
	 * Do not modify this variable directly! Use the Scale property instead.
	 */
	public Vector2 m_scale = Vector2.one;
	
	/**
	 * Do not modify this variable directly! Use the ScaleMode property instead.
	 */
	public ScaleModeTypes m_scaleMode = ScaleModeTypes.STRETCH;
	
	/**
	 * Do not modify this variable directly! Use the VerticalAlignment property instead.
	 */
	public VerticalAlignmentTypes m_verticalAlignment = VerticalAlignmentTypes.TOP;
	
	/**
	 * Do not modify this variable directly! Use the Visible property instead.
	 */
	public bool m_visible = true;
	
	public bool enabled = true;
	
#endregion
	
#region VARIABLES
	
	private float m_height = 0;
	private float m_lastViewHeight = 0;
	private float m_lastViewWidth = 0;
	private float m_width = 0;
	
#endregion

#region PROPERTIES
	/**
	 * The actual screen position of the object with respect to alignment and scale.
	 */
	protected Vector2 ActualPosition
	{
		get;
		set;
	}
	
	/**
	 * The actual scale of the object with respect to the device's screen size and the target screen size based upon
	 * the set ScaleMode for the object.
	 */
	protected Vector2 ActualScale
	{
		get;
		set;
	}
	
	/**
	 * The actual size (width and height) of the object with respect to the actual scale.
	 */
	protected Vector2 ActualSize
	{
		get
		{
			Vector2 scale = ActualScale;
			Vector2 size = new Vector2(Width * ActualScale.x, Height * ActualScale.y);
			return size;
		}
	}
	
	/**
	 * The anchor point to use that scale and position will be relative to for the object.
	 */
	public AnchorTypes Anchor
	{
		get
		{
			return m_anchor;
		}
		set
		{
			if (m_anchor == value) return;
			m_anchor = value;
			IsDirty = true;
		}
	}
	
	/**
	 * The axis-aligned bounding box (AABB) of the object in screen coordinates.
	 */
	public Bounds BoundingBox
	{
		get;
		private set;
	}
	
	/**
	 * The depth at which to render the object. Lower depths render on top of higher depths.
	 */
	public int Depth
	{
		get
		{
			return m_depth;
		}
		set
		{
			if (m_depth	== value) return;
			
			// Clamp the value between 0 and MAX_DEPTH
			if (value < 0) value = 0;
			if (value > MAX_DEPTH) value = MAX_DEPTH;
			
			m_depth = value;
			IsDirty = true;
		}
	}
	
	/**
	 * Used to indicate that the GUI object is dirty and should recalculate various components.
	 */
	private bool IsDirty
	{
		get;
		set;
	}
	
	/**
	 * The height of the object in screen coordinates.
	 */
	public float Height
	{
		get
		{
			return m_height;
		}
		protected set
		{
			if (m_height == value) return;
			m_height = value;
			IsDirty = true;
		}
	}
	
	/**
	 * The type of alignment along the x axis that the position is to be calculated with respect to.
	 */
	public HorizontalAlignmentTypes HorizontalAlignment
	{
		get
		{
			return m_horizontalAlignment;
		}
		set
		{
			if (m_horizontalAlignment == value) return;
			m_horizontalAlignment = value;
			IsDirty = true;
		}
	}
	
	/**
	 * The bounding box of the object's underlying mesh.
	 */
	protected Bounds MeshBounds
	{
		get
		{
			// Unity supports two types of mesh'. MeshFilter (for materials/textures) and the Text Mesh.
			// Unfortunately Text Mesh does not have sizing information. However both will have a MeshRenderer
			// so we can fall back to that we grabbing the bounds. Still, MeshFilter's bounds is more accurate
			// for some unknown reason.
			MeshFilter meshFilter = (MeshFilter) GetComponent<MeshFilter>();
			//used to use meshFilter.mesh
			if (meshFilter != null && meshFilter.mesh != null)
			{
				return meshFilter.mesh.bounds;
			}
				
			MeshRenderer renderer = (MeshRenderer) GetComponent<MeshRenderer>();
			if (renderer == null)
			{
				Debug.LogError(this.gameObject.name + " has no mesh!");
				return new Bounds(Vector3.zero, Vector3.zero);
			}
			
			return renderer.bounds;
		}
	}
	
	/**
	 * The position to place the sprite in screen coordinates. Default value is (0,0).
	 * <p>
	 * Extending classes should not use the property, use ActualPosition instead.
	 */
	public Vector2 Position
	{
		get
		{
			return m_position;
		}
		set
		{
			if (m_position == value) return;
			m_position = value;
			IsDirty = true;
		}
	}
	
	/**
	 * The units to use when calculating a GUIObject position relative to the screen.
	 */
	public PositionUnitTypes PositionUnits
	{
		get
		{
			return m_positionUnits;
		}
		set
		{
			if (m_positionUnits == value) return;
			m_positionUnits = value;
			IsDirty = true;
		}
	}
	
	/**
	 * The scale to be applied to the sprite. Default value is (1,1).
	 * <p>
	 * Extending classes should not use the property, use ActualScale instead.
	 */
	public Vector2 Scale
	{
		get
		{
			return m_scale;
		}
		set
		{
			if (m_scale	== value) return;
			m_scale = value;
			IsDirty = true;
		}
	}
	
	/**
	 * The method to use when scaling an object from the target screen resolution to the actual screen resolution.
	 */
	public ScaleModeTypes ScaleMode
	{
		get
		{
			return m_scaleMode;
		}
		set
		{
			if (m_scaleMode	== value) return;
			m_scaleMode = value;
			IsDirty = true;
		}
	}
	
	/**
	 * The target screen resolution UI objects should be scaled with respect to.
	 */
	protected Vector2 TargetScreenSize
	{
		get
		{
			Vector2 size = Vector2.zero;
			
			if (GlobalVars.Exists("targetScreenSize"))
			{
				size = (Vector2) GlobalVars.GetVariable("targetScreenSize");
			}
			
			if (size == Vector2.zero)
			{
				size = new Vector2(UICamera.pixelWidth, UICamera.pixelHeight);
			}
			
			return size;
		}
	}
	
	/**
	 * A reference to the UI camera component used to render GUI objects to the screen.
	 */
	protected static Camera UICamera
	{
		get
		{
			// Cache the component for faster reference.
			if (uiCamera == null)
			{
				GameObject uiCam = GameObject.Find("/CameraUI");
				if (uiCam != null)
				{
					uiCamera = uiCam.GetComponent<Camera>();
					if (uiCamera == null)
					{
						Debug.LogError("CameraUI is not a uiCamera!");
					}
				}
				else
				{
					Debug.LogError("An instance of CameraUI must exist to use GUI objects.");
				}
			}
			
			return uiCamera;
		}
	}
	
	/**
	 * The type of alignment along the y axis that the position is to be calculated with respect to.
	 */
	public VerticalAlignmentTypes VerticalAlignment
	{
		get
		{
			return m_verticalAlignment;
		}
		set
		{
			if (m_verticalAlignment == value) return;
			m_verticalAlignment = value;
			IsDirty = true;
		}
	}
	
	/**
	 * The width of the object in screen coordinates.
	 */
	public float Width
	{
		get
		{
			return m_width;
		}
		protected set
		{
			if (m_width	== value) return;
			m_width = value;
			IsDirty = true;
		}
	}
	
	/**
	 * Toggles the visibility of the object.
	 */
	public bool Visible
	{
		get
		{
			return m_visible;
		}
		set
		{
			if (!Application.isPlaying)
			{
				return;
			}
			
			m_visible = this.gameObject.active = value;
		}
	}

#endregion

#region INITIALIZERS

	/**
	 * Called when the game object instance is created. This occurs before Start.
	 */
	protected virtual void Awake()
	{
		Visible = m_visible;
	}
	
	/**
	 * Called when the game object has been first activated. This occurs after Awake and only if the object
	 * is active.
	 */
	protected virtual void Start()
	{
		
		if (!Application.isEditor || Application.isPlaying)
		{
			RegisterEventCallbacks();
		}
		
		IsDirty = true;
	}
	
	/**
	 * Called when the script or game object has been destroyed.
	 */
	protected virtual void OnDestroy()
	{
		if (!Application.isEditor || Application.isPlaying)
		{
			UnregisterEventCallbacks();
		}
	}
	
	/**
	 * Registers the object to be called back for various events.
	 */
	protected virtual void RegisterEventCallbacks()
	{
		GUIInputManager.Register(this);
	}
	
	/**
	 * Unregisters the object to be called back for various events.
	 */
	protected virtual void UnregisterEventCallbacks()
	{
		GUIInputManager.UnRegister(this);
	}

#endregion

#region FUNCTIONS
	/**
	 * Calculates the actual position of the object, in screen coordinates, with respect to alignment and scale.
	 */
	protected virtual void CalculateActualPosition()
	{
		float x = Position.x;
		float y = Position.y;
		
		// Convert position from percentage to pixels
		if (PositionUnits == PositionUnitTypes.PERCENTAGE)
		{
			x = (Position.x / 100.0f) * UICamera.pixelWidth;
			y = (Position.y / 100.0f) * UICamera.pixelHeight;
		}
		
		// Determined the scaled dimensions
		Vector2 actualScale = ActualScale;
		
		//x *= Mathf.Abs(actualScale.x);
		//y *= Mathf.Abs(actualScale.y);
		
		float scaledHeight = ScaleMode != ScaleModeTypes.NONE ? Height * Mathf.Abs(actualScale.y) : Height;
		float scaledWidth = ScaleMode != ScaleModeTypes.NONE ? Width * Mathf.Abs(actualScale.x) : Width;
		
		// Adjust x value based on horizontal alignment to the screen
		if (HorizontalAlignment == HorizontalAlignmentTypes.CENTER)
		{
			x += UICamera.pixelWidth / 2.0f;
		}
		else if (HorizontalAlignment == HorizontalAlignmentTypes.RIGHT)
		{
			x = UICamera.pixelWidth - x;
		}
		
		// Adjust y value based on vertical alignment to the screen
		if (VerticalAlignment == VerticalAlignmentTypes.MIDDLE)
		{
			y += UICamera.pixelHeight / 2.0f;
		}
		else if (VerticalAlignment == VerticalAlignmentTypes.TOP)
		{
			y = UICamera.pixelHeight - y;
		}
		
		// Now adjust the x value so that it reflects our desired pivot point
		if (Anchor == AnchorTypes.BottomCenter ||
			Anchor == AnchorTypes.MiddleCenter ||
			Anchor == AnchorTypes.TopCenter)
		{
			x -= scaledWidth / 2.0f;
		}
		else if (Anchor == AnchorTypes.BottomRight ||
				 Anchor == AnchorTypes.MiddleRight ||
				 Anchor == AnchorTypes.TopRight)
		{
			x -= scaledWidth;
		}
		
		// Finally adjust the y value so that it reflects our desired pivot point
		if (Anchor == AnchorTypes.MiddleCenter ||
			Anchor == AnchorTypes.MiddleLeft ||
			Anchor == AnchorTypes.MiddleRight)
		{
			y -= scaledHeight / 2.0f;
		}
		else if (Anchor == AnchorTypes.TopLeft ||
				 Anchor == AnchorTypes.TopCenter ||
				 Anchor == AnchorTypes.TopRight)
		{
			y -= scaledHeight;
		}
		
		ActualPosition = new Vector2(x,y);
	}
	
	/**
	 * Calculates the actual scale of the object, in screen coordinates, with respect to device size and scale mode.
	 */
	protected virtual void CalculateActualScale()
	{
		if (ScaleMode == ScaleModeTypes.NONE ||
		    TargetScreenSize == Vector2.zero ||
		    (UICamera.pixelWidth == TargetScreenSize.x &&
		     UICamera.pixelHeight == TargetScreenSize.y))
		{
			ActualScale = Scale;
			return;
		}
		
		float x = Scale.x;
		float y = Scale.y;
		float screenScaleX = UICamera.pixelWidth / TargetScreenSize.x;
		float screenScaleY = UICamera.pixelHeight / TargetScreenSize.y;
		
		switch (ScaleMode)
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
		
		x *= screenScaleX;
		y *= screenScaleY;
		
		ActualScale = new Vector2(x,y);
	}
	
	/**
	 * Calculates the axis-aligned bounding box (AABB) of the object, in screen coordinates.
	 */
	private void CalculateBoundingBox()
	{
		Vector2 centerPos = ActualPosition;
		Vector2 size = ActualSize;
		
		// Make sure we do not have a negative size
		size.x = Mathf.Abs(size.x);
		size.y = Mathf.Abs(size.y);
		
		// We add half of the width and height to the position since the underlying 3D pivot is
		// already at center. This only applies to objects with mesh geometry. It doesn't apply
		// to text meshes.
		if (GetComponent<MeshFilter>() != null)
		{
			centerPos.x += size.x / 2.0f;
			centerPos.y += size.y / 2.0f;
		}
		
		BoundingBox = new Bounds(centerPos, size);
	}
	
	/**
	 * When gizmos are enabled draws the bounding box in red.
	 */
	private void OnDrawGizmosSelected()
	{
		// Don't draw the bounding box while in edit mode. It causes LOTS of issues.
		if (Application.isEditor) return;
		
		Vector3 uiCameraWorldSize = UICamera.ViewportToWorldPoint(Vector3.one);
		uiCameraWorldSize *= 2.0f;
		
		// Determine the center of the bounding box in world coordinates.
		Vector3 center = UICamera.ScreenToWorldPoint(BoundingBox.center);
		center.z = 0.0f;
		
		// We calculate the size by taking the percentage of the screen we are taking up and calculating
		// that result with the viewport size in world coordinates.
		Bounds meshBounds = MeshBounds;
		float sizeX = (ActualSize.x / UICamera.pixelWidth) * uiCameraWorldSize.x;
		float sizeY = (ActualSize.y / UICamera.pixelHeight) * uiCameraWorldSize.y;
		Vector3 size = new Vector3(sizeX, sizeY, 1.0f);
		
		// Now draw the bounding box on screen
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(center, size);
	}
	
	/**
	 * Converts the specified screen coordinate to world coordinates.
	 */
	protected Vector3 ScreenToWorldPoint(Vector2 position)
	{
		Vector3 point = UICamera.ScreenToWorldPoint(new Vector3(position.x, position.y, 0));
		point.z = 0;
		return point;
	}
	
	/**
	 * 
	 */
	protected virtual void Update()
	{
		// When in edit mode we want to always perform updates so we can see our changes live. However when actually
		// the game is actually running we want to only rely on actual changes.
		if (Application.isEditor && !Application.isPlaying) IsDirty = true;
		
		// When the size of the viewport changes we need to recalculate everything
		if (UICamera != null &&
		    (m_lastViewWidth != UICamera.pixelWidth || m_lastViewHeight != UICamera.pixelHeight))
		{
			m_lastViewWidth = UICamera.pixelWidth;
			m_lastViewHeight = UICamera.pixelHeight;
			IsDirty = true;
		}
		
		if (IsDirty)
		{
			// Scale must be calculated first because position relies on it.
			CalculateActualScale();
			CalculateActualPosition();
			CalculateBoundingBox();
			
			UpdateWorldScale();
			UpdateWorldPosition();
			
			IsDirty = false;
		}
	}
	
	/**
	 * Updates the position of the object in world space.
	 */
	private void UpdateWorldPosition()
	{
		Vector3 worldPos = UICamera.ScreenToWorldPoint(BoundingBox.center);
		
		// Take the z position of the camera so that the GUI object is always within sight
		worldPos.z = UICamera.transform.position.z + ((float)Depth / MAX_DEPTH) + 1;
		
		this.transform.position = worldPos;
	}
	
	/**
	 * Updates the scale of the object in world coordinates.
	 */
	private void UpdateWorldScale()
	{
		// We don't want to apply any scale to text meshes as it messes things up.
		if (GetComponent<TextMesh>() != null)
		{
			return;
		}
		
		Vector3 uiCameraWorldSize = UICamera.ViewportToWorldPoint(Vector3.one);
		uiCameraWorldSize *= 2.0f;
		
		// We calculate the world scale by figuring out what percentage of the screen we are taking up, then determine
		// how much of the viewport we are taking up. This gives us world units but the size of the mesh is
		// likely bigger than a single unit so we have to compensate for that as well.
		Vector3 worldScale = Vector3.one;
		Bounds meshBounds = MeshBounds;
		// Avoid divide by zero errors
		if (meshBounds.size.x == 0.0f || meshBounds.size.z == 0.0f)
		{
			meshBounds = new Bounds(meshBounds.center, new Vector3(meshBounds.size.x != 0 ? meshBounds.size.x : 1.0f,
			                                                       1.0f,
			                                                       meshBounds.size.z != 0 ? meshBounds.size.z : 1.0f));
		}
		
		worldScale = new Vector3(((ActualSize.x / UICamera.pixelWidth) * uiCameraWorldSize.x) / meshBounds.size.x,
	                                 1.0f,
	                                 ((ActualSize.y / UICamera.pixelHeight) * uiCameraWorldSize.y) / meshBounds.size.z);
		
		// Set the scale of the transform
		this.transform.localScale = worldScale;
	}
	
	/**
	 * Converts the specified viewport coordinate to world coordinates.
	 */
	protected Vector3 ViewportToWorldPoint(Vector2 position)
	{
		Vector3 point = UICamera.ViewportToWorldPoint(new Vector3(position.x, position.y, 0));
		point.z = 0;
		return point;
	}
	
	/**
	 * Converts the specified world coordinate to screen coordinates.
	 */
	protected Vector2 WorldToScreenPoint(Vector3 position)
	{
		Vector3 screenPoint = UICamera.WorldToScreenPoint(position);
		return new Vector2(screenPoint.x, screenPoint.y);
	}

#endregion
}