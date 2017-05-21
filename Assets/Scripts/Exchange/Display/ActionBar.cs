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
		public void DrawActionBar(ActionBarDetails details)
		{
			GUI.BeginGroup(new Rect(details.Position, details.Size));
			GUI.DrawTexture(new Rect(Vector2.zero, details.Size), details.OuterTexture);
			float actionOffset = 0;
			int i = 0;
			foreach (Texture2D actionTexture in details.ActionTextures)
			{
				GUI.DrawTexture(new Rect(new Vector2(actionOffset, 0), new Vector2(details.Size.x * 0.25f, details.Size.y) - new Vector2(1, 1)), actionTexture);
				var style = new GUIStyle();
				style.alignment = TextAnchor.MiddleCenter;
				GUI.Label(new Rect(new Vector2(actionOffset, 0), new Vector2(details.Size.x * 0.25f, details.Size.y) - new Vector2(1, 1)), details.DisplayLabel[i], style);
				
				if(details.TimerManager.GetRemainingCooldown(details.DisplayLabel[i], 0) > 0)
				{
					Color color = Color.white;
					color.a = 0.8f;
					GUI.color = color;
					float cooldownPercentage = details.TimerManager.GetRemainingCooldown(details.DisplayLabel[i], 0) / details.TimerManager.GetTimerCooldownLength(details.DisplayLabel[i], 0);
					GUI.DrawTexture(new Rect(new Vector2(actionOffset, 0), new Vector2(details.Size.x * 0.25f * cooldownPercentage, details.Size.y) - new Vector2(1, 1)), details.CooldownTexture);
					GUI.color = Color.white;
					var fontStyle = new GUIStyle();
					fontStyle.alignment = TextAnchor.MiddleCenter;
					fontStyle.normal.textColor = Color.white;
					fontStyle.fontSize = 20;
					GUI.Label(new Rect(new Vector2(actionOffset, 0), new Vector2(details.Size.x * 0.25f, details.Size.y) - new Vector2(1, 1)), (1 + (int) details.TimerManager.GetRemainingCooldown(details.DisplayLabel[i], 0)).ToString(), fontStyle);
				}
				actionOffset += details.Size.x * 0.25f;

				i++;
			}

			GUI.EndGroup();
		}
	}
}
