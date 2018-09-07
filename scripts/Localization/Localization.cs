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

/**
 * The Localization is responsible for maintaining the currently selected locale used for the application as
 * well as system defaults.
 * <p>
 * Note that the class contains only static properties and methods and should never be instantiated.
 * 
 * @author Jean-Philippe Steinmetz
 */
public class Localization
{
	/**
	 * The name of the file containing all localized strings for the application.
	 */
	private static string TEXT_ASSET_NAME = "strings";
	
	/**
	 * The sub-directory of the assets path containing TEXT_ASSET_NAME
	 */
	private static string TEXT_ASSET_SUBPATH = "/text";
	
	/**
	 * The last Locale that was set.
	 */
	private static Locale locale = null;
	
	/**
	 * The map of localized strings to use for the application.
	 */
	private static Dictionary<string,string> strings = new Dictionary<string,string>();
	
	/**
	 * The current Locale used for all localization translation. Defaults to
	 * DefaultLocale if set to null.
	 */
	public static Locale CurrentLocale
	{
		get
		{
			Locale result = (Locale) GlobalVars.GetVariable("currentLocale");
			
			if (result == null)
			{
				result = DefaultLocale;
			}
			
			// Did someone change the GlobalVars value instead of going through Localization?
			if (result != locale)
			{
				locale = result;
				
				// Re-initialize for the new locale
				Init();
			}
			
			return result;
		}
		set
		{
			if (value != null)
			{
				locale = value;
			}
			else
			{
				locale = DefaultLocale;
			}
			
			GlobalVars.SetVariable("currentLocale", locale);
			
			// Re-initialize for the new locale
			Init();
		}
	}
	
	/**
	 * The default Locale if the user has not selected another.
	 */
	public static Locale DefaultLocale
	{
		get
		{
			return (Locale) GlobalVars.GetVariable("defaultLocale");
		}
		set
		{
			GlobalVars.SetVariable("defaultLocale", value);
		}
	}
	
	/**
	 * Throws an Exception as the class should never be instantiated.
	 */
	public Localization()
	{
		throw new Exception("This class should not be instantiated");
	}
	
	/**
	 * Initializes the LocalizationManager with any defaults. This should be called at application start.
	 */
	public static void Init()
	{
		// Load the localized strings file for the default locale first
		string path = DefaultLocale.GetFSCode() + TEXT_ASSET_SUBPATH + "/" + TEXT_ASSET_NAME;
		PropertiesFile propFile = new PropertiesFile(path);
		
		// Set the default strings to the local map
		strings = propFile.Properties;
		
		if (CurrentLocale != DefaultLocale)
		{
			// Next load the current file of localized strings
			path = CurrentLocale.GetFSCode() + TEXT_ASSET_SUBPATH + "/" + TEXT_ASSET_NAME;
			propFile = new PropertiesFile(path);
			
			// Now merge the contents of the current locale
			foreach (string key in propFile.Properties.Keys)
			{
				strings[key] = propFile.GetProperty(key);
			}
		}
	}
	
	/**
	 * Returns the localized string of text with the given key name.
	 * 
	 * @param key The key name of the localized text to retrieve.
	 * @return The localized text of key, otherwise null.
	 */
	public static string GetText(string key)
	{
		if (CurrentLocale == null)
		{
			UnityEngine.Debug.LogError("No locale has been set!");
			return null;
		}
		
		if (!strings.ContainsKey(key)) return null;
		return strings[key];
	}
	
	/**
	 * Translates the provided text and returns a string containing localized text for each key contained within it.
	 * <p>
	 * A key is a string containing no spaces or special characters that is fully closed with the prefix "${" and
	 * postfix "}".
	 * <p>
	 * 	Example:<br/>
	 * The following text:
	 *   ${GREETING} John!
	 * When translated for the fr-FR locale would be:
	 * 	 Bonjour John!
	 * In the above example ${GREETING} is the key and GREETING is the key name.
	 * 
	 * @param text The textual string to translate.
	 * @return The new string containing localized translations.
	 */
	public static string TranslateText(string text)
	{
		if (CurrentLocale == null)
		{
			UnityEngine.Debug.LogError("No locale has been set!");
			return null;
		}
		
		string translated = text;
		int searchIdx = 0;
		int keyStart = 0;
		int keyEnd = 0;
		while (searchIdx < translated.Length)
		{
			// Look for the next key
			keyStart = translated.IndexOf("${", searchIdx);
			
			// If no key was found, exit
			if (keyStart < 0) break;
			
			// Find the end of the next key
			keyEnd = translated.IndexOf("}", keyStart);
			
			// Extract the key
			string key = translated.Substring(keyStart, keyEnd - keyStart + 1);
			
			// Replace the substring with the translation
			string translation = GetText(key.Substring(2, key.Length-3));
			if (translation != null)
			{
				translated = translated.Replace(key, translation);
				searchIdx = keyStart + translation.Length;
			}
			else
			{
				searchIdx = keyStart + key.Length;
			}
		}
		
		return translated;
	}
}
