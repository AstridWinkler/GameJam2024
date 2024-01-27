Shader "MrKata/FillTileShaderCutout" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)

			_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
		_ExcludeColor ("ExcludeColor", Color) = (0,0,1,1)
		_ExcludePercent("Exclude%", Range(0,5)) = 1.0
		_TextureSize("TextureSize", Float) = 0.2

		_MainTex("Albedo (RGB)", 2D) = "white" {}

		_Color2("FillColor", Color) = (1,1,1,1)

		_FillTex ("Albedo (RGB)", 2D) = "white" {}
		// _BumpMap ("Bumpmap", 2D) = "bump" {}
		

	//	 _Bump ("Bump_Amplitude", Float) = 1.0

		 /*
		  _Bump ("Bump_Amplitude", Range(0,10.0)) = 1.0
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		*/
	}
	SubShader {
		/*
		Tags {
		"Queue" = "Transparent"
		"RenderType" = "Opaque"
		}
		*/

		Tags {"Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "Cutout"}
		//Tags {"Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout"}
		LOD 200


		//ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha Cull off


		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		//#pragma surface surf Standard fullforwardshadows alpha:fade 
		#pragma surface surf Lambert alphatest:_Cutoff

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		half Dif(half1 h1, half2 h2) {
	return (h1 + 1.0) / (h2 + 1.0f);
}


		sampler2D _MainTex;
		sampler2D _FillTex;
		sampler2D _BumpMap;
		 
		struct Input {
			float2 uv_MainTex;
			float2 uv_FillTex;
			float3 worldPos;

		//	float2 uv_BumpMap;
			fixed4 color : COLOR;
			fixed4 color2 : COLOR;
		};

	//	half _Glossiness;
	//	half _Metallic;
	//	half _Bump;
		half _TextureSize;

		fixed4 _Color;
		fixed4 _Color2;
		fixed4 _ExcludeColor; 
		half _ExcludePercent;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutput  o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 endCol = c.rgba * IN.color * _Color;
			fixed4 backCol = tex2D(_FillTex, IN.worldPos*0.2).rgba * IN.color * _Color2;

		//	screenUV += IN.uv_FillTex;

			

			// o.Normal = (UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap))*_Bump);
			//fixed3 nrm = UnpackNormal(tex2D (_BumpMap, IN.uv_BumpMap  ));

				 
			o.Alpha = c.a  * IN.color.a;			
			o.Albedo = c;

			half coef = abs((c.r + c.g) / 2.0);
			
			/*
			if(c.b > coef){
			//	half coef3 = clamp( (c.b * _ExcludePercent) - ((c.r+ c .g)), 0.0, 1.0);
			//half coef = clamp(_ExcludePercent *  abs(c.b + c.r - c .g), 0.0, 1.0);
			//	half coef = max(min((c.b * 2.0 * _ExcludePercent) / (c.r + c.g), 1.0), 0.0);
			//	half coef = (Dif(_ExcludeColor.r, c.r) + Dif(_ExcludeColor.g, c.g) + Dif(_ExcludeColor.b, c.b))/3.0;
			
				half coef2 = (c.b - coef);		
				fixed4 col = ( (c - float4(0,0, coef2,0)))*_ExcludePercent + (backCol *coef2);
				o.Albedo = col;
		//	o.Albedo = lerp(col, tex2D(_FillTex, IN.worldPos*0.2).rgb * IN.color * _Color2, 0.5f);
			}
			*/
			

			

			if (abs(_ExcludeColor.r - c.r) < _ExcludePercent && abs(_ExcludeColor.g - c.g) < _ExcludePercent && abs(_ExcludeColor.b - c.b < _ExcludePercent) && abs(_ExcludeColor.a - c.a < _ExcludePercent)) {
				o.Albedo = tex2D(_FillTex, IN.worldPos*_TextureSize).rgb * IN.color * _Color2;
			}
			else {
				o.Albedo = endCol;
			}


			/*else{
			
			 o.Normal = lerp(nrm, fixed3(0,0,1), -_Bump + 1);
			  o.Smoothness = _Glossiness;
			  o.Metallic = _Metallic;			

			  }
*/
			
		

			// Metallic and smoothness come from slider variables





		}
		ENDCG
	}
	FallBack "Diffuse"
}
