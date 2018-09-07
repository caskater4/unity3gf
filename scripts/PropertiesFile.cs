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
using System.IO;
using UnityEngine;

/**
 * PropertiesFile provides an easy way to load and retrieve properties stored in a
 * plain text file that uses the "key=value" format.
 * 
 * @author Jean-Philippe Steinmetz
 */
public class PropertiesFile
{
	private Dictionary<string,string> map = new Dictionary<string,string>();
	
	/**
	 * The dictionary containing all properties in the file.
	 */
	public Dictionary<string,string> Properties
	{
		get
		{
			return map;
		}
	}
	
	/**
	 * Creates a new instance of PropertiesFile.
	 * 
	 * @param filename The fully qualified path of the property file to parse.
	 */
	public PropertiesFile(String filename)
	{
		LoadFile(filename);
	}
	
	/**
	 * Loads and populates the internal dictionary with the contents of the file.
	 * 
	 * @param file The file to load as properties.
	 */
	private void LoadFile(String filename)
	{
		TextAsset asset = (TextAsset) Resources.Load(filename, typeof(TextAsset));
		StreamReader reader = new StreamReader(new MemoryStream(asset.bytes));
		
		while (!reader.EndOfStream)
		{
			String line = reader.ReadLine();
			
			// We place a maximum count of 2 here because we don't care about '=' characters stored in the value.
			string delimStr = "=";
			String[] parts = line.Split(delimStr.ToCharArray(), 2);
			
			// Only add true key=>value pairs to the map
			if (parts.Length == 2)
			{
				map.Add(parts[0], parts[1]);
			}
		}
		
		reader.Close();
	}
	
	/**
	 * Returns the value of the property with the specified key name.
	 * 
	 * @param key The name of the property to return the value of.
	 * @return The value of the property, otherwise null.
	 */
	public String GetProperty(String key)
	{
		if (!map.ContainsKey(key)) return null;
		return map[key];
	}
	
	/**
	 * Returns a list of property values for the list of key names.
	 * 
	 * @param keys The list of property key names to retrieve values of.
	 * @return The list of property values. Never null.
	 */
	public List<String> GetProperties(String[] keys)
	{
		List<String> values = new List<String>();
		
		foreach (String key in keys)
		{
			String property = GetProperty(key);
			
			if (property != null)
			{
				values.Add(property);
			}
		}
		
		return values;
	}
}
