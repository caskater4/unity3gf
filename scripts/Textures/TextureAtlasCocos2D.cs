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
using System.Xml;
using UnityEngine;

/**
 * Implementation of TextureAtlas that reads the Cocos2D data format.
 * 
 * @author Jean-Philippe Steinmetz
 */
public class TextureAtlasCocos2D : TextureAtlasXML
{
	/**
	 * Creates a new TextureAtlasCocos2D instance.
	 * 
	 * @param asset The asset containing the Cocos2D formatted sprite atlas data.
	 */
	public TextureAtlasCocos2D(TextAsset asset) : base(asset)
	{
	}
	
	protected override void ParseAsset(TextAsset asset)
	{
        Frames = new List<TextureAtlasFrame>();
		
        if (xml.DocumentElement.Name == "plist")
        {
            XmlNode frames = xml.DocumentElement.SelectSingleNode("dict/key");
			
            if (frames != null && frames.InnerText == "frames")
            {
                XmlNodeList subTextureNames = xml.DocumentElement.SelectNodes("dict/dict/key");
                XmlNodeList subTextures = xml.DocumentElement.SelectNodes("dict/dict/dict");
				
                try
                {
                    for (int si = 0; si < subTextures.Count; si++)
                    {
                        XmlNode subTexture = subTextures[si];
                        TextureAtlasFrame frameData = new TextureAtlasFrame();
						
						// Remove the file extension from the name, if applicable
						int extIdx = subTextureNames[si].InnerText.LastIndexOf(".");
                        if (extIdx < 0)
                        {
                            extIdx = subTextureNames[si].InnerText.Length;
                        }
						frameData.Name = subTextureNames[si].InnerText.Substring(0, extIdx);
						
						Rect frame = GetRect(subTexture, "frame");
                        frameData.Position = new Vector2(frame.xMin, frame.yMin);
						
						Rect colorRect = GetRect(subTexture, "sourceColorRect");
						frameData.Height = colorRect.height;
						frameData.Width = colorRect.width;
						
						Vector2 sourceSize = GetVector2(subTexture, "sourceSize");
                        frameData.SourceWidth = sourceSize.x;
						frameData.SourceHeight = sourceSize.y;
						
                        //frameData.offset = new Vector2(colorRect.xMin, colorRect.yMin);
						
						frameData.Rotated = GetBool(subTexture, "rotated");

                        Frames.Add(frameData);
                    }
                }
                catch (System.Exception ERR)
                {
                    Debug.LogError("Cocos2D Atlas Import error!");
                    Debug.LogError(ERR.Message);
                }
            }
        }
	}
	
	private static bool GetBool(XmlNode subTexture, string name)
    {
        XmlNode nameNode = subTexture.SelectSingleNode("key[.='" + name + "']");
		
        if (nameNode != null)
        {
            XmlNode boolNode = nameNode.NextSibling;
            return (boolNode.Name.ToLower() == "true");
        }
		
        return false;
    }
	
	private static Rect GetRect(XmlNode subTexture, string name)
    {
        XmlNode nameNode = subTexture.SelectSingleNode("key[.='" + name + "']");
		
        if (nameNode != null)
        {
            XmlNode stringNode = nameNode.NextSibling;
            return StringToRect(stringNode.InnerText);
        }
		
        return new Rect(0, 0, 0, 0);
    }

    private static Vector2 GetVector2(XmlNode subTexture, string name)
    {
        XmlNode nameNode = subTexture.SelectSingleNode("key[.='" + name + "']");
		
        if (nameNode != null)
        {
            XmlNode stringNode = nameNode.NextSibling;
            return StringToVector2(stringNode.InnerText);
        }
		
        return Vector2.zero;
    }
	
	private static Rect StringToRect(string s)
    {
        string _s = s.Substring(1, s.Length - 2);
        string[] sa = _s.Split(new string[] { "},{" }, System.StringSplitOptions.None);
        Vector2 v1 = StringToVector2(sa[0]+"}");
        Vector2 v2 = StringToVector2("{"+sa[1]);
		
        return new Rect(v1.x, v1.y, v2.x, v2.y);
    }
	
	private static Vector2 StringToVector2(string s)
    {
        string _s = s.Substring(1, s.Length - 2);
        string[] sa = _s.Split(',');
		
        return new Vector2(System.Convert.ToSingle(sa[0]), System.Convert.ToSingle(sa[1]));
    }
}
