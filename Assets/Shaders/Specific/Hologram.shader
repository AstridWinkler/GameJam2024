// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "MrKata/Hologram" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_SpecColor("Specular Color", Color) = (0.5, 0.5, 0.5, 0)
		[PowerSlider(5.0)] _Shininess("Shininess", Range(0.01, 1)) = 0.078125
		_Brightness("Brightness", Range(0.01, 5)) = 1
		_Tessellation("Tessellation", Float) = 1
		
		_OffsetX("OffsetX", Float) = 0
		_OffsetY("OffsetY", Float) = 0


		_MainTex("Base (RGB) TransGloss (A)", 2D) = "white" {}
	_Flick1("Flicker1", 2D) = "white" {}



	}

		SubShader{
		Tags{ "Queue" = "Transparent" /*"IgnoreProjector" = "True" */"RenderType" = "Transparent" }
		LOD 400
		Cull Off
		CGPROGRAM
#pragma surface surf BlinnPhong alpha:fade

#pragma target 3.0

	sampler2D _MainTex;
	sampler2D _Flick1;

	
	fixed4 _Color;
	half _Shininess;
	half _Brightness;
	half _Tessellation;

	half _OffsetX;
	half _OffsetY;



	struct Input {
		float2 uv_MainTex;
		float2 uv_Flick1;
		float3 viewDir;
		float3 worldPos;
		fixed4 color : COLOR;
	};



	void surf(Input IN, inout SurfaceOutput o) {
		fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
		fixed4 tex2 = tex2D(_Flick1, IN.uv_Flick1);
		
		

		o.Albedo = tex.rgb * _Color.rgb * IN.color;

		o.Emission = o.Albedo * _Brightness;

		
		o.Gloss = tex.a;
		o.Alpha = (tex.a * _Color.a * tex2.r  -  ( ( min(0, sin(_Time[1]*8 + 50* IN.worldPos.y)) )*0.2 + (1 + cos(_Time[1] * 15 + 80 * IN.worldPos.y))*0.2)  ) - (1 + sin(_Time[1] * 100))*0.2;
		o.Alpha *= IN.color.a;
		o.Specular = _Shininess;
	}
	ENDCG
	}

		FallBack "Legacy Shaders/Transparent/VertexLit"
}
