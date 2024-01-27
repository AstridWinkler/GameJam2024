using logiked.source.extentions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace logiked.source.utilities
{

    /// <summary>
    /// Des fonctions de conversions vers des bizzareries
    /// </summary>
    public class ConvertUtils
	{

		private static Vector2Int[] clockWiseVectors4 = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
		public static Vector2Int IntToVectorClockwise4(int source)
		{
			return clockWiseVectors4[source.Cycle(4)];
		}

	}
}