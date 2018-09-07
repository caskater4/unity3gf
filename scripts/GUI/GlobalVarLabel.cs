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

/**
 * The GlobalVarLabel displays a GlobalVars variable as a GUI element.
 * 
 * @author Jean-Philippe Steinmetz
 */
[AddComponentMenu("GUI/GlobalVar Label")]
[ExecuteInEditMode]
public class GlobalVarLabel : Label
{

#region INSPECTOR_PROPERTIES
	
	/**
	 * Do not modify this variable directly! Use the VariableName property instead.
	 */
	public string m_variableName = "";
	
	/**
	 * Do not modify this variable directly! Use the Prefix property instead.
	 */
	public string m_prefix = "";
	
	/**
	 * Do not modify this variable directly! Use the Suffix property instead.
	 */
	public string m_suffix = "";

#endregion

#region VARIABLES
#endregion

#region PROPERTIES
	
	/**
	 * The name of the global variable to display.
	 */
	public string VariableName
	{
		get
		{
			return m_variableName;
		}
		set
		{
			m_variableName = value;
		}
	}
	
	/**
	 * The prefix to add before the variable
	 */
	public string Prefix
	{
		get
		{
			return m_prefix;
		}
		set
		{
			m_prefix = value;
		}
	}
	
	/**
	 * The suffix to add after the variable
	 */
	public string Suffix
	{
		get
		{
			return m_suffix;
		}
		set
		{
			m_suffix = value;
		}
	}

#endregion

#region INITIALIZERS
#endregion

#region FUNCTIONS

	protected override void Update()
	{
		// If this is the editor, in edit mode, apply all inspector properties
		if (Application.isEditor && !Application.isPlaying)
		{
			Prefix = m_prefix;
			Suffix = m_suffix;
			VariableName = m_variableName;
		}
		
		// Retrieve the value of the variable we are watching. Do this using reflection.
		object variable = null;
		if (VariableName != null && VariableName.Length > 0)
		{
			if (GlobalVars.Exists(VariableName))
			{
				variable = GlobalVars.GetVariable(VariableName);
			}
			else
			{
				variable = "undefined";
			}
		}
		
		// We always want to display some value
		if (variable == null)
		{
			variable = "null";
		}
		
		// Update the content
		Text = Localization.TranslateText(Prefix) +
			Localization.TranslateText(variable.ToString()) +
			Localization.TranslateText(Suffix);
		
		base.Update();
	}
	
#endregion
}
