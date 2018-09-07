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

/**
 * Represents a particular language and region locale for use with localization. The underlying representation encodes
 * the language and region using the BCP 47 code standard.
 * 
 * @author Jean-Philippe Steinmetz
 */
public class Locale
{
	/**
	 * The locale for English, United Kingdom
	 */
	public static Locale EN_UK = new Locale("en-UK");
	
	/**
	 * The locale for English, United States
	 */
	public static Locale EN_US = new Locale("en-US");
	
	/**
	 * The locale for German, Germany
	 */
	public static Locale DE_DE = new Locale("de-DE");
	
	/**
	 * The locale for French, France
	 */
	public static Locale FR_FR = new Locale("fr-FR");
	
	/**
	 * The locale for Italian, Italy
	 */
	public static Locale IT_IT = new Locale("it-IT");
	
	/**
	 * The locale for Portuguese, Portugal
	 */
	public static Locale PT_PT = new Locale("pt-PT");
	
	/**
	 * The locale for Spanish, Spain
	 */
	public static Locale ES_ES = new Locale("es-ES");
	
	/**
	 * The locale for Polish, Poland
	 */
	public static Locale PL_PL = new Locale("pl-PL");
	
	/**
	 * The locale for Swedish, Sweden
	 */
	public static Locale SV_SE = new Locale("sv-SE");
	
	/**
	 * The BCP 47 code of the locale.
	 */
	public String Code
	{
		get
		{
			return Language + "-" + Country;
		}
	}
	
	/**
	 * The ISO 3166-1 alpha-2 country or region code of the locale.
	 */
	public String Country
	{
		get;
		private set;
	}
	
	/**
	 * The ISO 639-1 language code of the locale.
	 */
	public String Language
	{
		get;
		private set;
	}
	
	/**
	 * Creates a new Locale instance.
	 * 
	 * @param locale The BCP 47 code of the locale.
	 * @throws ArgumentException Thrown if locale is not a properly formatted BCP 47 code.
	 */
	public Locale(String locale)
	{
		string[] code = locale.Split('-');
		
		if (code.Length	!= 2)
		{
			throw new ArgumentException("Invalid locale code format: " + locale);
		}
		
		Language = code[0];
		Country = code[1];
	}
	
	/**
	 * Creates a new Locale instance.
	 * 
	 * @param language The ISO 639-1 language code of the locale.
	 * @param country The ISO 3166-1 alpha-2 country or region code of the locale.
	 */
	public Locale(String language, String country)
	{
		this.Country = country;
		this.Language = language;
	}
	
	/**
	 * Returns the file system friendly name of the locale's BCP 47 code.
	 */
	public String GetFSCode()
	{
		return Language + "_" + Country;
	}
	
	public override string ToString()
	{
		return Code;
	}
}
