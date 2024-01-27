Shader "MrKata/FillTileShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_ExcludeColor ("ExcludeColor", Color) = (0,0,1,1)
		_ExcludePercent("Exclude%", Range(0,5)) = 1.0
		_TextureSize("TextureSize", Float) = 0.2

		_MainTex("Albedo (RGB)", 2D) = "white" {}

		_Color2("FillColor", Color) = (1,1,1,1)

		_FillTex ("Albedo (RGB)", 2D) = "white" {}

	}
	SubShader {

		Tags {
		"Queue" = "Transparent"
		"RenderType" = "Transparent"
		}

		LOD 100


        Blend SrcAlpha OneMinusSrcAlpha Cull off


		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha:fade 

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

			fixed4 color : COLOR;
			fixed4 color2 : COLOR;
		};


		half _TextureSize;

		fixed4 _Color;
		fixed4 _Color2;
		fixed4 _ExcludeColor; 
		half _ExcludePercent;

			UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard  o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 endCol = c.rgba * IN.color * _Color;
			fixed4 backCol = tex2D(_FillTex, IN.worldPos*0.2).rgba * IN.color * _Color2;


				 
			o.Alpha = c.a  * IN.color.a;			
			o.Albedo = c;

			half coef = abs((c.r + c.g) / 2.0);
			
		

			

			if (abs(_ExcludeColor.r - c.r) < _ExcludePercent && abs(_ExcludeColor.g - c.g) < _ExcludePercent && abs(_ExcludeColor.b - c.b < _ExcludePercent) && abs(_ExcludeColor.a - c.a < _ExcludePercent)) {
				o.Albedo = tex2D(_FillTex, IN.worldPos*_TextureSize).rgb * IN.color * _Color2;
			}
			else {
				o.Albedo = endCol;
			}






		}
		ENDCG
	}
	FallBack "Diffuse"
}
