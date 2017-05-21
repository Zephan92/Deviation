using UnityEngine;

namespace Assets.Scripts.Exchange.Display
{
	public interface IExchangeTimer
	{
		void DrawExchangeTimer(ExchangeTimerDetails details, string displayLabel = "");
	}

	public struct ExchangeTimerDetails
	{
		public Vector2 Position;
		public Vector2 Size;

		public Texture2D OuterTexture;

		public ExchangeTimerDetails(Texture2D outerTexture, Vector2 position, Vector2 size)
		{
			OuterTexture = outerTexture;
			Position = position;
			Size = size;
		}
	}

	public class ExchangeTimer : IExchangeTimer
	{
		public void DrawExchangeTimer(ExchangeTimerDetails details, string displayLabel = "")
		{
			GUI.BeginGroup(new Rect(new Vector2(details.Position.x, details.Position.y), details.Size));
				GUI.DrawTexture(new Rect(Vector2.zero, details.Size), details.OuterTexture);

				var style = new GUIStyle();
				style.alignment = TextAnchor.MiddleCenter;
				GUI.Label(new Rect(Vector2.zero, details.Size - new Vector2(1, 1)), displayLabel, style);
			GUI.EndGroup();
		}
	}
}
