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
 * Asset is a wrapper for Unity's Resources system to provides localized resource loading of assets based upon
 * the currently selected Locale.
 * <p>
 * Note that the class contains only static properties and methods and should never be instantiated.
 * 
 * @author Jean-Philippe Steinmetz
 */
public class Asset
{
	/**
	 * The path containing all managed asset files.
	 */
	public static string AssetsPath
	{
		get
		{
			return (string) GlobalVars.GetVariable("assetsPath");
		}
		set
		{
			GlobalVars.SetVariable("assetsPath", value);
		}
	}
	
	/**
	 * Throws an Exception as the class should never be instantiated.
	 */
	public Asset()
	{
		throw new Exception("This class should never be instantiated");
	}
	
	/**
	 * Initializes the AssetManager with any defaults. This should be called at application start.
	 */
	public static void Init()
	{
	}
	
	/**
	 * Retrieves the fully qualified path of a localized asset at the given path.
	 * 
	 * @param path The path of an asset to retrieve it's fully qualified path.
	 * @return The fully qualified path of the asset, otherwise null if no asset found.
	 */
	public string GetFullPath(string path)
	{
		string filename = path.Substring(path.LastIndexOf("/"), path.Length - path.LastIndexOf("/"));
		string localePath = Localization.CurrentLocale.GetFSCode() + "/" + path;
		string dirPath = localePath.Substring(0, localePath.LastIndexOf("/"));
		
		// Do any files exist at this path?
		string[] files = Directory.GetFiles(dirPath);
		foreach (string file in files)
		{
			// We just need one file match
			if (file == filename) return localePath;
		}
		
		// If we made it this far we didn't find the right asset
		localePath = Localization.DefaultLocale.GetFSCode() + "/" + path;
		dirPath = localePath.Substring(0, localePath.LastIndexOf("/"));
		
		// Do any files exist at this path?
		files = Directory.GetFiles(dirPath);
		foreach (string file in files)
		{
			// We just need one file match
			if (file == filename) return localePath;
		}
		
		// We didn't find anything
		return null;
	}
	
	/**
	 * Loads a localized asset at the specified path.
	 * <p>
	 * Attempts to retrieve an asset for the currently selected locale first, then if not found attempts to find
	 * the asset for the default locale. If no asset can be found returns null.
	 *
	 * @param path The path to load the asset from.
	 * @return The localized asset at path, otherwise null.
	 */
	public static UnityEngine.Object Load(string path)
	{
		return Load(path, null);
	}
	
	/**
	 * Loads a localized asset of the specified type at the given path.
	 * <p>
	 * Attempts to retrieve an asset for the currently selected locale first, then if not found attempts to find
	 * the asset for the default locale. If no asset can be found returns null.
	 *
	 * @param path The path to load the asset from.
	 * @param type The type of asset to load.
	 * @return The localized asset at path, otherwise null.
	 */
	public static UnityEngine.Object Load(string path, Type type)
	{
		// First try and find the asset based on the current locale
		string localePath = Localization.CurrentLocale.GetFSCode() + "/" + path;
		UnityEngine.Object asset = Resources.Load(localePath, type);
		
		// Did we find it?
		if (asset) return asset;
		
		// That must not have been it. Next try the default locale
		localePath = Localization.DefaultLocale.GetFSCode() + "/" + path;
		asset = Resources.Load(localePath, type);
		
		return asset;
	}
	
	/**
	 * Loads all assets at the given path.
	 * <p>
	 * Attempts to retrieve each asset for the currently selected locale first, then if not found attempts to find
	 * the asset for the default locale. If no asset can be found returns null.
	 * 
	 * @param path The path to the directory of assets to load
	 * @return The list of all successfully loaded assets, otherwise null.
	 */
	public static UnityEngine.Object[] LoadAll(string path)
	{
		return LoadAll(path, null);
	}
	
	/**
	 * Loads all assets at the given path of a given type.
	 * <p>
	 * Attempts to retrieve each asset for the currently selected locale first, then if not found attempts to find
	 * the asset for the default locale. If no asset can be found returns null.
	 * 
	 * @param path The path to the directory of assets to load
	 * @param type The type of assets to load.
	 * @return The list of all successfully loaded assets, otherwise null.
	 */
	public static UnityEngine.Object[] LoadAll(string path, Type type)
	{
		List<string> files = new List<string>();
		
		// Make a list of all files in the current locale and default for this path
		
		// Current locale
		string localePath = Localization.CurrentLocale.GetFSCode() + "/" + path;
		string[] results = Directory.GetFiles(localePath);
		
		files.AddRange(results);
		
		localePath = Localization.DefaultLocale.GetFSCode() + "/" + path;
		results = Directory.GetFiles(localePath);
		
		// Merge any files missing from the list
		foreach (string file in results)
		{
			if (!files.Contains(file))
			{
				files.Add(file);
			}
		}
		
		// Now go through all the files and load their resources. Also make sure to strip the file extension
		UnityEngine.Object[] objects = new UnityEngine.Object[files.Count];
		for (int i = 0; i < files.Count; ++i)
		{
			string file = files[i].Substring(0, files[i].LastIndexOf('.') + 1);
			objects[i] = Resources.Load(file);
		}
		
		return objects;
	}
	
	/**
	 * Unloads assets that are not currently in use.
	 * <p>
	 * An asset is deemed to be unused if it isn't reached after walking the whole game object hierarchy, including
	 * script components. Static variables are also examined.
	 */
	public static AsyncOperation UnloadUnusedAssets()
	{
		return Resources.UnloadUnusedAssets();
	}
}
