using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public static class Extension
{
	public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
	{
		return Utils.GetOrAddComponent<T>(go);
	}

	public static void BindEvent(this GameObject go, Action action, Define.UIEvent type = Define.UIEvent.Click)
	{
		UI_Base.BindEvent(go, action, type);
	}

	static System.Random _rand = new System.Random();

	public static void Shuffle<T>(this IList<T> list)
	{
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = _rand.Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}

	public static void TryUpdateShapeToAttachedSprite(this PolygonCollider2D collider)
	{
		collider.UpdateShapeToSprite(collider.GetComponent<SpriteRenderer>().sprite);
	}

	public static void UpdateShapeToSprite(this PolygonCollider2D collider, Sprite sprite)
	{
		// ensure both valid
		if (collider != null && sprite != null)
		{
			// update count
			collider.pathCount = sprite.GetPhysicsShapeCount();

			// new paths variable
			List<Vector2> path = new List<Vector2>();

			// loop path count
			for (int i = 0; i < collider.pathCount; i++)
			{
				// clear
				path.Clear();
				// get shape
				sprite.GetPhysicsShape(i, path);
				// set path
				collider.SetPath(i, path.ToArray());
			}
		}
	}
}