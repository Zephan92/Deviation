using Assets.Scripts.Controllers;
using Assets.Scripts.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Exchange
{
	public interface IProgressBar
	{
		void DrawProgressBar(ProgressBarDetails details, float barDisplay, string displayLabel = "");
	}

	public struct ProgressBarDetails
	{
		public Vector2 Position;
		public Vector2 Size;

		public Texture2D OutlineTexture;
		public Texture2D EmptyTexture;
		public Texture2D FullTexture;

		public ProgressBarDetails(Texture2D outline, Texture2D empty, Texture2D full, Vector2 position, Vector2 size)
		{
			OutlineTexture = outline;
			EmptyTexture = empty;
			FullTexture = full;
			Position = position;
			Size = size;
		}
	}

	public class ProgressBar : IProgressBar
	{
		public void DrawProgressBar(ProgressBarDetails details, float barDisplay, string displayLabel = "")
		{
			GUI.BeginGroup(new Rect(details.Position, details.Size));
				GUI.DrawTexture(new Rect(Vector2.zero, details.Size), details.OutlineTexture);
				GUI.DrawTexture(new Rect(Vector2.zero, details.Size - new Vector2(1,1)), details.EmptyTexture);

				GUI.BeginGroup(new Rect(0,0, details.Size.x * barDisplay, details.Size.y));
					GUI.DrawTexture(new Rect(Vector2.zero, details.Size - new Vector2(1, 1)), details.FullTexture);
				GUI.EndGroup();
				var style = new GUIStyle();
				style.alignment = TextAnchor.MiddleCenter;
				GUI.Label(new Rect(Vector2.zero, details.Size - new Vector2(1, 1)),displayLabel, style);
			GUI.EndGroup();
		} 
	}
}
