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
using System.Collections.Generic;

/**
 * AchievementManager handles the awarding of achievements to the player.
 * <p>
 * When an achievement is awarded a new GlobalVar variable is set whose name is the achievement with "Complete"
 * concatenated to the end. For example, the AchievementTypes.TooCoolForSchool achievement would yield a GlobalVar
 * variable with the name 'TooCoolForSchoolComplete' with a boolean type and value of true.
 * 
 * @author Jean-Philippe Steinmetz
 */
public class AchievementManager : MonoBehaviour
{
	/**
	 * Set to the name of an achievement when it has been granted.
	 */
	private static string grantedAchievement = null;
	
	private static List<string> grantedAchievements = new List<string>();
	
	/**
	 * The prefab to use to notify the player when an achievement has been earned.
	 */
	public GameObject notification = null;
	
	/**
	 * Grants the player the specified achievement.
	 * 
	 * @param achievement The type of achievement being awarded.
	 */
	public static void GrantAchievement(AchievementTypes achievement)
	{
		string aName = achievement.ToString() + "Complete";
		
		// Do not award an achievement more than once.
		if (GlobalVars.Exists(aName) && (bool)GlobalVars.GetVariable(aName))
		{
			//Debug.Log("Achievement " + achievement + " has already been awarded.");
			return;
		}
		
		// Mark the achievement as received for bookkeeping purposes
		GlobalVars.SetVariable(aName, true);
		GlobalVars.SetVariable("gotAchievement", true);
		grantedAchievements.Add(aName);
		grantedAchievement = achievement.ToString();
		Debug.Log("Achievement Awarded: " + grantedAchievement);
	}
	
	public void Update()
	{
		if (grantedAchievement != null && grantedAchievement.Length > 0)
		{
			// Display the notification on screen so that the player knows what they got!
			if (notification != null)
			{
				GameObject notifier = (GameObject) Instantiate(notification);
				
				if (notifier == null)
				{
					Debug.LogError("Error creating instance of " + notification.name);
					return;
				}
				
				GameObject icon = GameObject.Find(notifier.name + "/Icon");
				if (icon != null)
				{
					Sprite sprite = icon.GetComponent<Sprite>();
					if (sprite != null)
					{
						sprite.Texture = grantedAchievement;
					}
					else
					{
						Debug.LogError("AchievementPopup: Icon has no sprite!");
					}
				}
				else
				{
					Debug.LogError("AchievementPopup: Icon not found!");
				}
				
				GameObject label = GameObject.Find(notifier.name + "/Label");
				GameObject label_shadow = GameObject.Find(notifier.name + "/Shadow");
				if (label != null)
				{
					Label text = label.GetComponent<Label>();
					Label text2 = label_shadow.GetComponent<Label>();
					if (text != null)
					{
						text.Text = Localization.GetText(grantedAchievement);
						text2.Text = Localization.GetText(grantedAchievement);
					}
					else
					{
						Debug.LogError("AchievementPopup: Label has no label!");
					}
				}
				else
				{
					Debug.LogError("AchievementPopup: Label not found!");
				}
			}
			
			grantedAchievement = null;
		}
	}
	
	public bool Reset() 
	{
		for(int i = 0; i < grantedAchievements.Count; i++)
		{
			GlobalVars.SetVariable(grantedAchievements[i], false);
		}
		GlobalVars.SetVariable("gotAchievement", false);
		grantedAchievements.Clear();
		
		return true;
	}
}
