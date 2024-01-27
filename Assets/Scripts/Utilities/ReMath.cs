using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringExtensions
{
	public static string FirstCharToUpper(this string input)
	{

			return (input[0]).ToString().ToUpper() + input.Substring(1);
	
	}
}

public class Ref<T>
{
	private T backing;
	public T Value {get{return backing;}}
	public Ref(T reference)
	{
		backing = reference;
	}
}








public class ReMath  {






public static float ParseFloat(string str){
		return float.Parse(str, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
	}


	public static int MinLim ( int nb, int min = 0) {
		return(min < nb)? nb: min;
	}

	public static float MinLim ( float nb, float min = 0.0f) {
		return(min < nb)? nb: min;
	}


	public static int MaxLim (int nb, int max = 0) {
		return (int)((nb > max)?max:nb);
	}

	public static float MaxLim (float nb, float max = 0.0f) {
		return (float)((nb > max)?max:nb);
	}



	public static int Clamp (int nb,  int min, int max) {
		int g = MinLim (nb, min);

		if (g != nb)
			return g;
		
		return MaxLim (nb, max);
	}


	public static float Clamp (float nb,  float min, float max) {
		float g = MinLim (nb, min);

		if (g != nb)
			return g;
		
		return MaxLim (nb, max);

	}

	public static int RoundDouble(double db){
		return (int)db + ((db-(int)db > 0.5)? 1 : 0);
	}
	public static int AngleId4(float ang){
		int a =  Mathf.RoundToInt(ang / 90f);
		if (a == 4)
			a = 0;
		return a;
	}



    public static Vector3 ToVec3(Vector2Int v2){
		return new Vector3 (v2.x, v2.y, 0);
	}
	public static Vector2Int ToVec2Int(Vector2 v2){
		return new Vector2Int (Mathf.RoundToInt( v2.x),Mathf.RoundToInt(v2.y));
	}
    public static Vector2 ToVec2(Vector2Int v2)
    {
        return new Vector2(v2.x,v2.y);
    }
    public static Vector3Int ToVec3Int(Vector3 v3)
    {
        return new Vector3Int(Mathf.RoundToInt(v3.x), Mathf.RoundToInt(v3.y), Mathf.RoundToInt(v3.z));
    }
    public static Vector2Int ToVec2IntFloor(Vector2 v3)
    {
        return new Vector2Int(Mathf.FloorToInt(v3.x), Mathf.FloorToInt(v3.y));
    }

    public static Vector3 Center(Vector3 vec1, Vector3 vec2){
		return new Vector3 ( (vec1.x+vec2.x)/2.0f, (vec1.y+vec2.y)/2.0f, (vec1.z+vec2.z)/2.0f  );
	}
	public static Vector2 Center(Vector2 vec1, Vector2 vec2){
		return new Vector2 ( (vec1.x+vec2.x)/2.0f, (vec1.y+vec2.y)/2.0f);
	}
	public static Vector2 GetVec( Vector2 pt1, Vector2 pt2){
		return new Vector2 (pt1.x-pt2.x, pt1.y-pt2.y );
	}
	public static float Scalaire(Vector2 vec1, Vector2 vec2){
		return (vec1.x * vec2.x) + (vec1.y * vec2.y);
	}


	public static float VecAngle(Vector2 vec1, Vector2 vec2){
		float c =  Mathf.Acos ( Scalaire (vec1, vec2) / (vec1.magnitude * vec2.magnitude));
				if (vec1.x < vec2.x)
				c = -c;
		return c;
	}

	public static float VecAngleAtan(Vector2 a, Vector2 b){
		return Mathf.Atan2( a.x*b.y - a.y*b.x, a.x*b.x + a.y*b.y );
	} 
	public static Vector3 VecAngleAtanVec(Vector2 a, Vector2 b){
		return new Vector3(0,0,ToDeg(Mathf.Atan2( a.x*b.y - a.y*b.x, a.x*b.x + a.y*b.y )));
	} 

	public static float AngAtanToDeg(Vector2 a, Vector2 b){
		return ReMath.DegClamp(ToDeg(Mathf.Atan2( a.x*b.y - a.y*b.x, a.x*b.x + a.y*b.y )));
	} 

	public static Quaternion AngleToQuaternion(float rotZ){
		return Quaternion.Euler(new Vector3(0,0,rotZ));
	} 

	public static Vector2 VecNormalClockwise(Vector2 vec)
	{
		return new Vector2(vec.y, -vec.x);
	}
	
	public static int DegClamp(float ang){
		return Mathf.RoundToInt((ang< 0)? 360 - Mathf.Abs( Mathf.RoundToInt (ang))%360 : ang%360);
	}
	
	public static int DegClamp(int ang){
		return (ang< 0)? 360 - Mathf.Abs(ang)%360 : ang%360;
	}

	public static Vector2 VecBetwenPoints(Vector2 pt1, Vector2 pt2)
	{
		return new Vector2(pt2.x - pt1.x ,pt2.y - pt1.y);
	}

	public static Vector2 FloatToCircleVector(float x)
	{
		return new Vector2(Mathf.Cos((Mathf.PI * 2f) * x), Mathf.Sin((Mathf.PI * 2f) * x));
	}
	
	public static float VecAngleDown(Vector2 vec1){
			
		float c =  Mathf.Acos ( Scalaire (Vector2.down, vec1));
		if (vec1 == Vector2.zero)
			c = 0;

		 if (vec1.x < 0)
			c = 2f*Mathf.PI - c;
		return c;
	}
	
	public static float ToRad(float c){
		return (c*(Mathf.PI/180f));
	}

	public static float ToDeg(float c){
		return (c*(180f/Mathf.PI));
	}
	public static int sqr(int a){
		return a * a;
	}
	public static float sqr(float a){
		return a * a;
	}

	// Calculate the distance between
	// point pt and the segment p1 --> p2.
	public static float FindDistanceToSegment(
		Vector2 pt, Vector2 p1, Vector2 p2, out Vector2 closest)
	{
		float dx = p2.x - p1.x;
		float dy = p2.y - p1.y;
		if ((dx == 0) && (dy == 0))
		{
			// It's a point not a line segment.
			closest = p1;
			dx = pt.x - p1.x;
			dy = pt.y - p1.y;
			return Mathf.Sqrt(dx * dx + dy * dy);
		}

		// Calculate the t that minimizes the distance.
		float t = ((pt.x - p1.x) * dx + (pt.y - p1.y) * dy) /
			(dx * dx + dy * dy);

		// See if this represents one of the segment's
		// end points or a point in the middle.
		if (t < 0)
		{
			closest = new Vector2(p1.x, p1.y);
			dx = pt.x - p1.x;
			dy = pt.y - p1.y;
		}
		else if (t > 1)
		{
			closest = new Vector2(p2.x, p2.y);
			dx = pt.x - p2.x;
			dy = pt.y - p2.y;
		}
		else
		{
			closest = new Vector2(p1.x + t * dx, p1.y + t * dy);
			dx = pt.x - closest.x;
			dy = pt.y - closest.y;
		}

		return Mathf.Sqrt(dx * dx + dy * dy);
	}


	public static int distance(Vector2Int v1, Vector2Int v2){
		return Mathf.RoundToInt(Mathf.Sqrt (sqr(v1.x - v2.x) + sqr(v1.y - v2.y)));
	}
	public static int linDist(Vector2Int v1, Vector2Int v2){
		return Mathf.Abs(v1.x - v2.x)+Mathf.Abs(v1.y - v2.y);
	}
	public static int sng(int a){
		return ((a == 0) ? 0 : ((a < 0) ? -1 : 1));
	}
	public static int sng(float a){
		return ((a == 0) ? 0 : ((a < 0) ? -1 : 1));
	}
	public static int sngNo(int a){
		return  ((a < 0) ? -1 : 1);
	}
	public static int sngNo(float a){
		return ((a < 0) ? -1 : 1);
	}
	public static float RandFloatVec(Vector2 rand){
		return Random.Range(rand.x, rand.y);
	}
	public static int RandIntVec(Vector2Int rand){
		return Random.Range(rand.x, rand.y);
	}


	public static float RoundFloat(float val, float rnd){
		return  Mathf.Round (val / rnd) * rnd;
	}

	public static Vector3 RoundVec(Vector3 vec, float rnd){
		return new Vector3 (RoundFloat (vec.x, rnd), RoundFloat (vec.y, rnd), RoundFloat (vec.z, rnd));
	}

	public static Vector2 Rotate2D(Vector2 point, float angle, Vector2 center, bool toRad = false)
		{
		if(toRad)
		angle *= Mathf.PI / 180.0f;

			float s = Mathf.Sin(angle);
			float c = Mathf.Cos(angle);

			// translate point back to origin:
			point.x -= center.x;
			point.y -= center.y;

			// rotate point
		float xnew = point.x * c - point.y * s;
		float ynew = point.x * s + point.y * c;

			// translate point back:
		point.x = xnew + center.x;
		point.y = ynew + center.y;
		return point;
		}

	public static Vector3 GetZ_Euler(Transform obj, float rot){
		return new Vector3 (obj.eulerAngles.x, obj.eulerAngles.y, rot);
	}

	public static Vector3 GetZ_Euler_Local(Transform obj, float rot){
		return new Vector3 (obj.localEulerAngles.x, obj.localEulerAngles.y, rot);
	}


	public static T[] AddArray<T>(T[] arr, T newObj){
		if (arr == null)
			arr = new T[0];

		T[] cop = new T[arr.Length + 1];
		arr.CopyTo (cop, 0);
		cop [cop.Length - 1] = newObj;
		return cop;
	}

	public static T[] CloneLst<T>(T[] arr)
	{
		if (arr == null)
			return new T[0];

		T[] newLst = new T[arr.Length];
		arr.CopyTo(newLst, 0);

		return newLst;
	}

		public static T[] AddArrayConcatenate<T>(T[] arr, T[] newObjs)
	{
		if (arr == null)
			arr = new T[0];

		T[] cop = new T[arr.Length + newObjs.Length];
		arr.CopyTo(cop, 0);
		for(int i = arr.Length; i < cop.Length; i++)
			cop[i] = newObjs[i - arr.Length];
		return cop;
	}


	public static T[] AddArrayConcatenateIndex<T>(T[] arr, T[] newObjs, int index)
	{
		if (arr == null)
			arr = new T[0];

		int pos = 0;

		T[] cop = new T[arr.Length + newObjs.Length];

		newObjs.CopyTo(cop, index);


		for (int i = 0; i < cop.Length; i++)
		{
			if (i < index || i >= index + newObjs.Length)
			{
				cop[i] = arr[pos];
				pos++;
			}
		}


		return cop;
	}



	public static T[] AddArray<T>(T[] arr, T newObj, int index){
		if (arr == null)
			arr = new T[0];

		T[] cop = new T[arr.Length + 1];
		arr.CopyTo (cop, 0);

		for (int i = cop.Length - 1; i > index; i--) {
		
			if (i != index+1)
				cop [i] = cop [i - 1];
			else
				cop [i] = newObj;
				
		}



		return cop;
	}

	public static Vector3 StringToVector3(string sVector)
	{
		var ci = System.Globalization.CultureInfo.InvariantCulture;

		// Remove the parentheses
		if (sVector.StartsWith("(") && sVector.EndsWith(")"))
		{
			sVector = sVector.Substring(1, sVector.Length - 2);
		}

		// split the items
		string[] sArray = sVector.Split(',');

		// store as a Vector3
		Vector3 result = new Vector3(
			float.Parse(sArray[0], ci),
			float.Parse(sArray[1], ci),
			float.Parse(sArray[2], ci));

		return result;
	}


	public static Vector2 StringToVector2(string sVector)
	{
		var ci = System.Globalization.CultureInfo.InvariantCulture;

		if (sVector.StartsWith("(") && sVector.EndsWith(")"))
		{
			sVector = sVector.Substring(1, sVector.Length - 2);
		}
		string[] sArray = sVector.Split(',');
		Vector3 result = new Vector2(
			float.Parse(sArray[0], ci),
			float.Parse(sArray[1], ci));

		return result;
	}



	public static T[] RemoveArrayElement<T>(T[] arr, int id){
		if (arr == null)
			return null;
		

		T[] cop = new T[arr.Length - 1];

		for (int i = 0; i < arr.Length - 1; i++)
			cop [i] = arr [i + ((i >= id)?1:0)];

		return cop;
	}


	public static T RandomItem<T>(T[] arr){
		if (arr == null)
			return default(T);

		return arr[Random.Range(0,arr.Length)];
	}

	public static T[] CopyArray<T>(T[] arr){
		T[] ar2 = new T[arr.Length];
		arr.CopyTo (ar2, 0);

		return arr;
	}

	/*
	public static void PrintList(params object[] toPrint)
	{
		Debug.Log(ToStringList(toPrint));
	}

	public static string ToStringList(object[] obj)
	{
		if (obj == null)
			return "null";

		string[] final = new string[obj.Length];


		for (int i = 0; i < obj.Length; i++)
			final[i] = obj[i].ToString();

		return string.Join("\n", final);

	}*/

		public static T[] CreateEmptyItemsLst<T>(int cnt)
	{
		T[] cop = new T[cnt];
		
		for (int i = 0; i < cnt; i++)
			cop[i] = (T)System.Activator.CreateInstance(typeof(T));//default(T);

		return cop;
	}



	public static T[] AddEmptyItems<T>(T[] arr, int cnt){
		if (arr == null)
			arr = new T[0];

		T[] cop = new T[arr.Length + cnt];
		arr.CopyTo (cop, 0);
		return cop;
	}

	public static T[] ResizeArray<T>(T[] arr, int newLen){

		T[] end = new T[newLen];

		if (arr == null)
			return end;

		for (int i = 0; i < newLen && i < arr.Length; i++)
		{
			end[i] = arr[i];
		}
		

		return end;
	}


	public static T[] ResizeArrayV2<T>(T[] arr, int newLen, T addAtNew )
	{

		T[] end = new T[newLen];

		if (arr == null)
			arr = new T[0];

		for (int i = 0; i < newLen; i++)
		{

			if (i < arr.Length)
				end[i] = arr[i];
			else
				end[i] = addAtNew;
		}


		return end;
	}
	

	public static T[] AutoAddItem<T>(T[] arr, T item, ref int index ,int packAddSize = 5){

		if (arr == null) {
			index = 1;
			arr = new T[1]{item};
		} else {
			if (index >= arr.Length)
				arr = AddEmptyItems<T> (arr, packAddSize);

			arr [index] = item;
	
			index++;
		}

		return arr;
	}


	public static T[] RemoveEmptyElement<T>(T[] arr){  //Untested mais pratique
		T[] end;
		int c = 0, i;


		if (arr == null)
			return new T[0];

		for ( i = 0; i < arr.Length; i++) {
			if (arr [i] != null)
				c++;
			
		}

		end = new T[c];
		//Debug.LogError (c);
		c = 0;

		for ( i = 0; i < arr.Length; i++) {
			if(arr[i] != null){
				end [c++] = arr [i];
				}				
		}

		return end;
	}



	public static T GetComponentIn<T>(GameObject o){
		T a = o.GetComponent<T> ();

		if (a == null)
			a = o.GetComponentInChildren<T> ();

		if (a == null && o.transform.parent != null)
			a = o.GetComponentInParent<T> ();
		

		return a;

	}


	public static float OrthographicYtoX(float ortho){
		return ortho * ((float)Screen.currentResolution.width/(float)Screen.currentResolution.height);
	}




	public static Color[] Get16Color(Texture2D tex, Rect texRect){
		int x, y;
		Color[] end = new Color[ (int)(texRect.width * texRect.height)];
		for (x = 0; x < texRect.width; x++) {
			for (y =  0; y < texRect.height; y++) {
				end [ (int)(x + texRect.width * y)] = tex.GetPixel ((int)texRect.x + x, (int)texRect.y + y);
			}
		}
		return end;
	}


	public static Color[] Get16ColorNormal(Texture2D tex, Rect texRect, float coef){
		int x, y;
		Color[] end = new Color[ (int)(texRect.width * texRect.height)];
		Color endCol;

		for (x = 0; x < texRect.width; x++) {
			for (y =  0; y < texRect.height; y++) {
				endCol = tex.GetPixel ((int)texRect.x + x, (int)texRect.y + y);

				end [(int)(x + texRect.width * y)] = new Color (endCol.r * coef, endCol.g * coef, endCol.b * coef, endCol.a);
							}
		}
		return end;
	}


	public static Texture2D FillColorTexture32(Texture2D tex, Color32 col){

		Color32[] resetColorArray = tex.GetPixels32();

		for (int i = 0; i < resetColorArray.Length; i++) {
			resetColorArray[i] = col;
		}

		tex.SetPixels32(resetColorArray);
		tex.Apply();

		return tex;
	}

	public static Texture2D FillColorTexture(Texture2D tex, Color col){

		Color[] resetColorArray = tex.GetPixels();

		for (int i = 0; i < resetColorArray.Length; i++) {
			resetColorArray[i] = col;
		}

		tex.SetPixels(resetColorArray);
		tex.Apply();

		return tex;
	}
	//public static Texture2D SetPixels(Texture2D tex, Color[] cols){

	public static RectInt ToRectInt(Rect rect)
	{
		return new RectInt(ToVec2Int(rect.position), ToVec2Int(rect.size));
	}

    public static Rect ToRect(RectInt rect)
    {
        return new Rect( ToVec2(rect.position), ToVec2(rect.size));
    }



    public static RectInt GetIntersectRect(RectInt r1, RectInt r2)
	{

		int x5 = Mathf.Max(r1.xMin, r2.xMin);
		int y5 = Mathf.Max(r1.yMin, r2.yMin);


		int x6 = Mathf.Min(r1.xMax, r2.xMax);
		int y6 = Mathf.Min(r1.yMax, r2.yMax);

		// no intersection 
		if (x5 > x6 || y5 > y6)
		{
			return new RectInt();
		}

		return new RectInt(x5, y6, x6 - x5, y5 - y6);
	}




	public static Rect GetIntersectRect(Rect r1, Rect r2)
	{

		float x5 = Mathf.Max(r1.xMin, r2.xMin);
		float y5 = Mathf.Max(r1.yMin, r2.yMin);


		float x6 = Mathf.Min(r1.xMax, r2.xMax);
		float y6 = Mathf.Min(r1.yMax, r2.yMax);

		// no intersection 
		if (x5 > x6 || y5 > y6)
		{
			return new Rect();
		}

		return new Rect(x5, y6, x6 - x5 , y5 - y6);
	}





    public static void DrawWoldRect(RectInt rect, Color color, float duration)
    {
        DrawWoldRect(ToRect(rect), color, duration);
    }


    public static void DrawWoldRect(Rect rect, Color color, float duration)
	{
		Debug.DrawLine(new Vector2(rect.xMin, rect.yMin) , new Vector2(rect.xMin, rect.yMax), color, duration);
		Debug.DrawLine(new Vector2(rect.xMin, rect.yMax) , new Vector2(rect.xMax, rect.yMax), color, duration);
		Debug.DrawLine(new Vector2(rect.xMax, rect.yMin) , new Vector2(rect.xMax, rect.yMax), color, duration);
		Debug.DrawLine(new Vector2(rect.xMax, rect.yMin) , new Vector2(rect.xMin, rect.yMin), color, duration);
	}


	public static RectInt RectIntByCoord(Vector2Int min, Vector2Int max)
	{
		return new RectInt(min.x, min.y, Mathf.Abs(min.x - max.x), Mathf.Abs(min.y - max.y));
	}

    public static Rect RectByCoord(Vector2 min, Vector2 max)
    {
        return new Rect(min.x, min.y, Mathf.Abs(min.x - max.x), Mathf.Abs(min.y - max.y));
    }



}
