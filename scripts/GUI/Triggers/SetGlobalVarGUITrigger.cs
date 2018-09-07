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
 * SetGlobalVarGUITrigger is a type of {@link GUITrigger} that when activated set the value of a particular
 * GlobalVars variable. Only primitive variables or classes whom have constructors taking a single string
 * argument may be used.
 * 
 * @author Jean-Philippe Steinmetz
 */
[AddComponentMenu("GUI/Triggers/Set GlobalVar")]
public class SetGlobalVarGUITrigger : GUITrigger
{
	/**
	 * The name of the GlobalVars variable to set when the trigger is activated.
	 */
	public string setVariableName = null;
	
	/**
	 * The value to set for the GlobalVars variable.
	 */
	public string setVariableValue = null;
	
	/**
	 * A cached reference to the variable's constructor.
	 */
	private System.Reflection.ConstructorInfo constructor = null;
	
	protected override void Start()
	{
		base.Start();
		
		object variable = GlobalVars.GetVariable(setVariableName);
		CheckVariable(variable);
	}
	
	protected override void OnActivate()
	{
		object variable = GlobalVars.GetVariable(setVariableName);
		
		if (CheckVariable(variable))
		{
			//Debug.Log("Setting " + setVariableName + ": " + setVariableValue);
			
			System.Type type = variable.GetType();
			if (type.IsPrimitive || type == typeof(string))
			{
				SetVariablePrimitive(variable, setVariableValue);
			}
			else
			{
				SetVariableObject(variable, setVariableValue);
			}
		}
	}
	
	/**
	 * Determines if the desired value can be set for the specified GlobalVars variable.
	 * 
	 * @param variable The variable to check.
	 * @return True if the variable's value can be set, otherwise false.
	 */
	private bool CheckVariable(object variable)
	{
		if (variable == null)
		{
			Debug.LogError("No GlobalVars variable has been defined with the name " + setVariableName);
			return false;
		}
		
		// For non-primitive types, check to see if we have a usable constructor
		System.Type type = variable.GetType();
		if (!type.IsPrimitive && type != typeof(string) && constructor == null)
		{
			constructor = type.GetConstructor(new System.Type[]{typeof(string)});
			
			if (constructor == null)
			{
				Debug.LogError(type.FullName + " must have a constructor taking a single string argument");
				return false;
			}
		}
		
		return true;
	}
	
	/**
	 * Sets the specified new value for the provided GlobalVars variable when the type is a complex object.
	 * 
	 * @param variable The variable to set.
	 * @param newValue The desired new value to set.
	 */
	private void SetVariableObject(object variable, string newValue)
	{
		try
		{
			GlobalVars.SetVariable(setVariableName, constructor.Invoke(new object[]{newValue}));
		}
		catch (Exception e)
		{
			Debug.LogError("Error setting GlobalVar " + setVariableName + " : " + e);
		}
	}
	
	/**
	 * Sets the specified new value for the provided GlobalVars variable when the type is primitive.
	 * 
	 * @param variable The variable to set.
	 * @param newValue The desired new value to set.
	 */
	private void SetVariablePrimitive(object variable, string newValue)
	{
		System.Type type = variable.GetType();
		
		if (type == typeof(string))
		{
			GlobalVars.SetVariable(setVariableName, newValue);
		}
		else if (type == typeof(bool))
		{
			GlobalVars.SetVariable(setVariableName, Convert.ToBoolean(newValue));
		}
		else if (type == typeof(byte))
		{
			GlobalVars.SetVariable(setVariableName, Convert.ToByte(newValue));
		}
		else if (type == typeof(char))
		{
			GlobalVars.SetVariable(setVariableName, Convert.ToChar(newValue));
		}
		else if (type == typeof(decimal))
		{
			GlobalVars.SetVariable(setVariableName, Convert.ToDecimal(newValue));
		}
		else if (type == typeof(double))
		{
			GlobalVars.SetVariable(setVariableName, Convert.ToDouble(newValue));
		}
		else if (type == typeof(float))
		{
			GlobalVars.SetVariable(setVariableName, Convert.ToSingle(newValue));
		}
		else if (type == typeof(int))
		{
			GlobalVars.SetVariable(setVariableName, Convert.ToInt32(newValue));
		}
		else if (type == typeof(long))
		{
			GlobalVars.SetVariable(setVariableName, Convert.ToInt64(newValue));
		}
		else if (type == typeof(sbyte))
		{
			GlobalVars.SetVariable(setVariableName, Convert.ToSByte(newValue));
		}
		else if (type == typeof(short))
		{
			GlobalVars.SetVariable(setVariableName, Convert.ToInt16(newValue));
		}
		else if (type == typeof(uint))
		{
			GlobalVars.SetVariable(setVariableName, Convert.ToUInt32(newValue));
		}
		else if (type == typeof(ulong))
		{
			GlobalVars.SetVariable(setVariableName, Convert.ToUInt64(newValue));
		}
		else if (type == typeof(ushort))
		{
			GlobalVars.SetVariable(setVariableName, Convert.ToUInt16(newValue));
		}
		else
		{
			Debug.LogError("Unsupported Type: " + type.FullName);
		}
	}
}