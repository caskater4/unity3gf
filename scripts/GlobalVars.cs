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
 * The GlobalVars class is a collection of variables that may be globally referenced throughout the game.
 * Variables are accessed through the methods GetVariable and SetVariable.
 * <p>
 * Note that the class contains only static properties and methods and should never be instantiated.
 * 
 * @author Jean-Philippe Steinmetz
 */
public class GlobalVars
{
	private static List<string> persistentVariables = null;
	private static Dictionary<string, object> variables = null;
	
	/**
	 * The list of all variable names that have been marked as persistent.
	 */
	public static string[] PersistentNames
	{
		get
		{
			return persistentVariables != null ? persistentVariables.ToArray() : null;
		}
	}
	
	/**
	 * The list of names for all variables stored.
	 */
	public static string[] VariableNames
	{
		get
		{
			if (variables == null)
			{
				return null;
			}
			
			string[] keys = new string[variables.Keys.Count];
			variables.Keys.CopyTo(keys, 0);
			return keys;
		}
	}
	
	public GlobalVars()
	{
		throw new Exception("This class should not be instantiated");
	}
	
	/**
	 * Returns true if there exists a global variable with the specified name, otherwise false.
	 * 
	 * @param name The name of the variable to search for.
	 */
	public static bool Exists(string name)
	{
		return variables != null && variables.ContainsKey(name);
	}
	
	/**
	 * Returns the value of the variable with the specified name.
	 * 
	 * @param name The name of the variable to retrieve.
	 * @return The value of the variable, otherwise null.
	 */
	public static object GetVariable(string name)
	{
		return variables != null && name != null && variables.ContainsKey(name) ? variables[name] : null;
	}
	
	/**
	 * Returns the class type of the variable with the specified name.
	 * 
	 * @param name The name of the variable to retrieve.
	 * @return The class type of the variable, otherwise null.
	 */
	public static System.Type GetVariableType(string name)
	{
		object objValue = GetVariable(name);
		return objValue != null ? objValue.GetType() : null;
	}
	
	/**
	 * Returns true if the variable with the specified name is persistent, otherwise false.
	 * 
	 * @param name The name of the variable to inquire.
	 */
	public static bool IsPersistent(string name)
	{
		return persistentVariables != null && persistentVariables.Contains(name);
	}
	
	/**
	 * Marks the variable with the specified name as persistent. Persistent variables are saved to disk.
	 * 
	 * @param name The name of the variable to make persistent.
	 */
	public static void SetPersistent(string name)
	{
		if (persistentVariables == null)
		{
			persistentVariables = new List<string>();
		}
		
		if (persistentVariables.Contains(name))
		{
			return;
		}
		
		persistentVariables.Add(name);
	}
	
	/**
	 * Sets the value of the variable with the specified name.
	 * 
	 * @param name The name of the variable to set.
	 * @param value The value to set.
	 */
	public static void SetVariable(string name, object value)
	{
		if (variables == null)
		{
			variables = new Dictionary<string, object>();
		}
		
		variables[name] = value;
		
		//Debug.Log( name + " = "+ value );
	}
	
	/**
	 * Resets all standard variables. Should occur only once per application run.
	 */
	public static void Reset()
	{
		// Debug
		SetVariable("debugMode", (bool) false);
		
		// Pause
		SetVariable("isPaused", (bool) false );
		
		// System
		SetVariable("assetsPath", "Assets/Resources");
		SetVariable("defaultLocale", Locale.EN_US);
		SetVariable("currentLocale", Locale.EN_US);
		SetVariable("loadingScene", "loader");
		SetVariable("targetScreenSize", new Vector2(1920.0f, 1080.0f));
		
		// Social Platforms
		SetVariable("fbAuthToken", "");
		SetVariable("fbLinkName", "");
		SetVariable("fbLinkURL", "");
		
		SetVariable("twAuthToken", "");
		SetVariable("twLinkName", "");
		SetVariable("twLinkURL", "");
		
		// Specify all persistent variables
		SetPersistent("currentLocale");
		SetPersistent("fbAuthToken");
		SetPersistent("twAuthToken");
	}
	
	/**
	 * Resets all standard variables that must be reset on a new level load. This includes anything variables 
	 * that may carry from level to level, but must reset when the scene is Restored. 
	 * */
	public static void ResetLevel()
	{
		// Pause
		SetVariable("isPaused", (bool) false );
	}
}
