using Assets.Scripts.Interface;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Exchange.Display
{
	public interface IActionBar
	{
		void DrawActionBar(ActionBarDetails details);
	}

	public struct ActionBarDetails
	{
		public Vector2 Position;
		public Vector2 Size;

		public Texture2D OuterTexture;
		public Texture2D CooldownTexture;

		public Texture2D[] ActionTextures;
		public string[] DisplayLabel;

		public ITimerManager TimerManager;

		public ActionBarDetails(Texture2D outerTexture, Texture2D[] actionTextures, Texture2D cooldownTexture, Vector2 position, Vector2 size, string[] displayLabel, ITimerManager timerManager)
		{
			OuterTexture = outerTexture;
			ActionTextures = actionTextures;
			CooldownTexture = cooldownTexture;
			Position = position;
			Size = size;
			DisplayLabel = displayLabel;
			TimerManager = timerManager;
		}
	}
	public class ActionBar : IActionBar
	{
		private GUIStyle actionLabelStyle;
		private GUIStyle cooldownLabelStyle;

		public ActionBar()
		{
			actionLabelStyle = new GUIStyle();
			actionLabelStyle.alignment = TextAnchor.MiddleCenter;

			cooldownLabelStyle = new GUIStyle();
			cooldownLabelStyle.alignment = TextAnchor.MiddleCenter;
			cooldownLabelStyle.normal.textColor = Color.white;
			cooldownLabelStyle.fontSize = 20;
		}

		public void DrawActionBar(ActionBarDetails details)
		{
			float actionOffset = 0;
			int i = 0;

			GUI.BeginGroup(new Rect(details.Position, details.Size));
			GUI.DrawTexture(new Rect(Vector2.zero, details.Size), details.OuterTexture);//warning null textures
			
			foreach (Texture2D actionTexture in details.ActionTextures)
			{
				string attackName = details.DisplayLabel[i];
				Vector2 actionSize = new Vector2(details.Size.x * 0.25f, details.Size.y) - new Vector2(1, 1);
				Rect textureDimensions = new Rect(new Vector2(actionOffset, 0), actionSize);
				float timeLeft = details.TimerManager.GetRemainingCooldown(attackName, 0);
				float totalTime = details.TimerManager.GetTimerCooldownLength(attackName, 0);

				GUI.DrawTexture(textureDimensions, actionTexture);
				GUI.Label(textureDimensions, attackName, actionLabelStyle);
				
				if(timeLeft > 0)
				{
					float cooldownPercentage = timeLeft / totalTime;
					Rect cooldownDimensions = new Rect(textureDimensions.position, new Vector2(cooldownPercentage, 1f));
					Rect cooldownPosition = new Rect(textureDimensions.position, new Vector2(textureDimensions.size.x * cooldownPercentage, textureDimensions.size.y));
					//string cooldownLabel = (1 + (int) timeLeft).ToString();
					//Color color = Color.white;
					//color.a = 0.8f;
					//GUI.color = color;
					GUI.DrawTextureWithTexCoords(cooldownPosition, details.CooldownTexture, cooldownDimensions, true);
					//GUI.color = Color.white;
					//GUI.Label(textureDimensions, cooldownLabel, cooldownLabelStyle);
				}

				actionOffset += details.Size.x * 0.25f;
				i++;
			}

			GUI.EndGroup();
		}
	}
}
