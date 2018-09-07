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

using System.Text;
using UnityEngine;

/**
 * AppConfigManager provides utility functions for the saving and loading of persistent configuration data
 * to and from disk.
 * <p>
 * During a Save() operation the SaveConfig() method is called for all game objects in the scene. Afterwards
 * any GlobalVars variables that have been marked as persistent are written to disk.
 * <p>
 * Calling the Load() function causes all configuration data stored on disk to be loaded and overwrites the existing
 * corresponding values in GlobalVars.
 * 
 * @author Jean-Philippe Steinmetz
 */
public class AppConfigManager
{
	/**
	 * Initializes the AppConfigManager with any defaults.
	 */
	public static void Init()
	{
	}
	
	/**
	 * Encodes the given string using a simple custom encryption scheme and returns the result encoded as base64.
	 */
	private static string EncodeString(string toencode)
	{
		StringBuilder encrypted = new StringBuilder();
		
		// Go through each character and do a simple value arithmetic. This is a very simplistic encryption.
		int dir = 1;
		foreach (char c in toencode)
		{
			int charValue = System.Convert.ToInt32(c);
			charValue += 7 * dir;
			char newChar = System.Convert.ToChar(charValue);
			encrypted.Append(newChar);
			dir *= -1;
		}
		
		// Convert the string to a byte array
		UTF8Encoding ByteConverter = new UTF8Encoding();
		byte[] dataToEncrypt = ByteConverter.GetBytes(encrypted.ToString());
		
		// Now convert it to Base64 and return the result
		return System.Convert.ToBase64String(dataToEncrypt);
	}
	
	/**
	 * Decodes the given base64 encoded string using a simple custom algorithm.
	 */
	private static string DecodeString(string todecode)
	{
		byte[] dataToDecrypt = System.Convert.FromBase64String(todecode);
			
		// Convert back to unicode
		UTF8Encoding ByteConverter = new UTF8Encoding();
		string result = ByteConverter.GetString(dataToDecrypt);
		
		// Go through each character and do a simple reverse value arithmetic.
		StringBuilder decrypted = new StringBuilder();
		int dir = -1;
		foreach (char c in result)
		{
			int charValue = System.Convert.ToInt32(c);
			charValue += 7 * dir;
			char newChar = System.Convert.ToChar(charValue);
			decrypted.Append(newChar);
			dir *= -1;
		}
		
		return decrypted.ToString();
	}
	
	/**
	 * Loads all configuration data stored on disk, overwriting the corresponding values of variables stored in
	 * GlobalVars.
	 */
	public static void Load()
	{
		if (GlobalVars.PersistentNames == null || GlobalVars.PersistentNames.Length	== 0)
		{
			return;
		}
		
		foreach (string varName in GlobalVars.PersistentNames)
		{
			string encVarName = EncodeString(varName);
			
			if (PlayerPrefs.HasKey(encVarName))
			{
				System.Type objType = GlobalVars.GetVariableType(varName);
			
				// If no type was found for the variable we can't load it.
				if (objType	== null)
				{
					Debug.LogWarning("No class type found for: " + varName);
					continue;
				}
				
				if (objType == typeof(bool))
				{
					LoadBool(varName);
				}
				else if (objType == typeof(float))
				{
					LoadFloat(varName);
				}
				else if (objType == typeof(int))
				{
					LoadInt(varName);
				}
				else if (objType == typeof(string))
				{
					LoadString(varName);
				}
				else
				{
					LoadObject(varName, objType);
				}
			}
		}
	}
	
	/**
	 * Loads and overwrites the boolean variable value with the given name.
	 * 
	 * @param name The name of the variable to load.
	 */
	protected static void LoadBool(string name)
	{
		string strVal = DecodeString(PlayerPrefs.GetString(EncodeString(name)));
		
		if (strVal == null || strVal.Length == 0)
		{
			Debug.LogError("Error loading config: " + name);
			return;
		}
		
		bool val = System.Boolean.Parse(strVal);
		GlobalVars.SetVariable(name, val);
	}
	
	/**
	 * Loads and overwrites the floating point variable value with the given name.
	 * 
	 * @param name The name of the variable to load.
	 */
	protected static void LoadFloat(string name)
	{
		string strVal = DecodeString(PlayerPrefs.GetString(EncodeString(name)));
		
		if (strVal == null || strVal.Length == 0)
		{
			Debug.LogError("Error loading config: " + name);
			return;
		}
		
		float val = System.Convert.ToSingle(strVal);
		GlobalVars.SetVariable(name, val);
	}
	
	/**
	 * Loads and overwrites the integer variable value with the given name.
	 * 
	 * @param name The name of the variable to load.
	 */
	protected static void LoadInt(string name)
	{
		string strVal = DecodeString(PlayerPrefs.GetString(EncodeString(name)));
		
		if (strVal == null || strVal.Length == 0)
		{
			Debug.LogError("Error loading config: " + name);
			return;
		}
		
		int val = System.Convert.ToInt32(strVal);
		GlobalVars.SetVariable(name, val);
	}
	
	/**
	 * Loads and overwrites the object variable value with the given name. This requires that the class type has a
	 * constructor which takes a string as a single argument.
	 * 
	 * @param name The name of the variable to load.
	 * @param type The class type of the variable to load.
	 */
	protected static void LoadObject(string name, System.Type type)
	{
		System.Reflection.ConstructorInfo constInfo = null;
		System.Object objValue = null;
		
		// Look for a constructor containing a single string argument constructor.
		try
		{
			System.Type[] types = new System.Type[1];
			types[0] = typeof(string);
			constInfo = type.GetConstructor(types);
		}
		catch (System.Exception e)
		{
			Debug.LogError("Error retrieving constructor information: " + e);
			return;
		}
		
		if (constInfo == null)
		{
			Debug.LogError("No suitable constructor found for: " + type);
			return;
		}
		
		// Attempt to instantiate a new object using the constructor we found and the value
		// stored in PlayerPrefs.
		try
		{
			string strVal = DecodeString(PlayerPrefs.GetString(EncodeString(name)));
			
			if (strVal == null || strVal.Length == 0)
			{
				Debug.LogError("Error loading config: " + name);
				return;
			}
			
			System.Object[] args = new System.Object[1];
			args[0] = strVal;
			objValue = constInfo.Invoke(args);
		}
		catch (System.Exception e)
		{
			Debug.LogError("Error instantiating object " + type + ": " + e);
			return;
		}
		
		GlobalVars.SetVariable(name, objValue);
	}
	
	/**
	 * Loads and overwrites the string variable value with the given name.
	 * 
	 * @param name The name of the variable to load.
	 */
	protected static void LoadString(string name)
	{
		GlobalVars.SetVariable(name, PlayerPrefs.GetString(name));
	}
	
	/**
	 * Saves all persistent configuration data from GlobalVars to disk. This function also calls SaveConfig for each
	 * game object in the scene before the save.
	 * <p>
	 * Since this function makes use of PlayerPrefs it may cause a hiccup during execution. It is advised to call this
	 * function only at times when it is acceptable for the application to experience a stop or pause.
	 */
	public static void Save()
	{
		if (GlobalVars.PersistentNames == null || GlobalVars.PersistentNames.Length	== 0)
		{
			Debug.Log("No changes to save.");
			return;
		}
		
		bool storageDirty = false;
		
		// Go through each game object and call the SaveConfig method
		GameObject[] gameObjects = (GameObject[]) GameObject.FindObjectsOfType(typeof(GameObject));
		foreach (GameObject gameObj in gameObjects)
		{
			gameObj.BroadcastMessage("SaveConfig", SendMessageOptions.DontRequireReceiver);
		}
		
		// Now save all persistent GlobalVars to disk
		foreach (string varName in GlobalVars.VariableNames)
		{
			if (!varName.StartsWith("_"))
			{
				object varValue = GlobalVars.GetVariable(varName);
				System.Type varType = varValue.GetType();
				
				// Only write peristent variables to disk and don't write data that hasn't changed (doing so is
				// wasteful).
				if (GlobalVars.IsPersistent(varName))
				{
					string encVarName = EncodeString(varName);
					string strValue = varValue.ToString();
					strValue = EncodeString(strValue);
					
					if (strValue != PlayerPrefs.GetString(encVarName))
					{
						PlayerPrefs.SetString(encVarName, strValue);
						storageDirty = true;
					}
				}
			}
		}
		
		// Write any changes to disk
		if (storageDirty)
		{
			Debug.Log("Saving to disk...");
			PlayerPrefs.Save();
		}
		else
		{
			Debug.Log("No changes to save.");
		}
	}
}
