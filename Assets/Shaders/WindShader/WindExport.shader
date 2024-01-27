Shader "Wind"
{
 
    Properties
    {
        srpBatcherFix("srpBatcherFix", Float) = 0

        _Speed("Speed", Float) = 3
        _Tilling("Tilling", Float) = 1
        _Amplitude("Amplitude", Float) = 0.1
        _YVariation("YVariation", Float) = 0.5
        _GroundSpeedLimit("GroundSpeedLimit", Range(0, 1)) = 0
        _MainTex("MainTex", 2D) = "white" {}
        _LockingMovementReduction("LockingMovementReduction", Range(0, 3)) = 1
        _LockingYUvsPosition("LockingYUvsPosition", Float) = 0
        _LockingTreshold("LockingTreshold", Range(0.05, 3)) = 0.1
        [Toggle(_COLORPREVIEW)]_COLORPREVIEW("ColorPreview", Float) = 0
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Lit"
            "Queue"="Transparent"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"=""
        }
        Pass
        {
       

            Name "Sprite Lit"
            Tags
            {
                "LightMode" = "Universal2D"
            }
        
            // Render State
            Cull Off
        Blend One OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM


float srpBatcherFix;
            // Pragmas
            #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>
        
            // Keywords
            #pragma multi_compile_fragment _ DEBUG_DISPLAY
            #pragma shader_feature_local _ _COLORPREVIEW
        
        #if defined(_COLORPREVIEW)
            #define KEYWORD_PERMUTATION_0
        #else
            #define KEYWORD_PERMUTATION_1
        #endif
        
        
            // Defines
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define _SURFACE_TYPE_TRANSPARENT 1
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define _ALPHAPREMULTIPLY_ON 1
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define ATTRIBUTES_NEED_NORMAL
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define ATTRIBUTES_NEED_TANGENT
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define ATTRIBUTES_NEED_TEXCOORD0
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define ATTRIBUTES_NEED_COLOR
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_POSITION_WS
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_TEXCOORD0
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_COLOR
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_SCREENPOSITION
        #endif
        
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SPRITELIT
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
            // Includes
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
            struct Attributes
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 positionOS : POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 normalOS : NORMAL;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 tangentOS : TANGENT;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 uv0 : TEXCOORD0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 color : COLOR;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
            #endif
        };
        struct Varyings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 positionWS;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 texCoord0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 color;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 screenPosition;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 WorldSpacePosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 uv0;
            #endif
        };
        struct VertexDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 ObjectSpaceNormal;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 ObjectSpaceTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 ObjectSpacePosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 TimeParameters;
            #endif
        };
        struct PackedVaryings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 interp0 : INTERP0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 interp1 : INTERP1;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 interp2 : INTERP2;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 interp3 : INTERP3;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };
        
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyzw =  input.texCoord0;
            output.interp2.xyzw =  input.color;
            output.interp3.xyzw =  input.screenPosition;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            output.color = input.interp2.xyzw;
            output.screenPosition = input.interp3.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        #endif
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float _Speed;
        float _Tilling;
        float _Amplitude;
        float _YVariation;
        float _GroundSpeedLimit;
        float4 _MainTex_TexelSize;
        float4 _MainTex_ST;
        float _LockingMovementReduction;
        float _LockingYUvsPosition;
        float _LockingTreshold;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
            // Graph Includes
            // GraphIncludes: <None>
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Graph Functions
            
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Cosine_float(float In, out float Out)
        {
            Out = cos(In);
        }
        
        void Unity_Distance_float(float A, float B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        void Unity_Lerp_float3(float3 A, float3 B, float3 T, out float3 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_2f9d2b3a5d124d9e98433a5997953d56_Out_0 = _Speed;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_7a03097596a04c6fbaf7ac71a3a87967_Out_2;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_2f9d2b3a5d124d9e98433a5997953d56_Out_0, _Multiply_7a03097596a04c6fbaf7ac71a3a87967_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Split_7129975c62bc4203840744f4faa34665_R_1 = IN.ObjectSpacePosition[0];
            float _Split_7129975c62bc4203840744f4faa34665_G_2 = IN.ObjectSpacePosition[1];
            float _Split_7129975c62bc4203840744f4faa34665_B_3 = IN.ObjectSpacePosition[2];
            float _Split_7129975c62bc4203840744f4faa34665_A_4 = 0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_ac7d909912204819babee1a8fc569ca1_Out_0 = _Tilling;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_6086e5426cd74fb0a516e28465b0eb11_Out_2;
            Unity_Multiply_float_float(_Split_7129975c62bc4203840744f4faa34665_G_2, _Property_ac7d909912204819babee1a8fc569ca1_Out_0, _Multiply_6086e5426cd74fb0a516e28465b0eb11_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Add_d9603b01777f4b5fadb1eb750f87eac1_Out_2;
            Unity_Add_float(_Multiply_7a03097596a04c6fbaf7ac71a3a87967_Out_2, _Multiply_6086e5426cd74fb0a516e28465b0eb11_Out_2, _Add_d9603b01777f4b5fadb1eb750f87eac1_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Cosine_5a4679fd61034839827e62dcb7ca30d8_Out_1;
            Unity_Cosine_float(_Add_d9603b01777f4b5fadb1eb750f87eac1_Out_2, _Cosine_5a4679fd61034839827e62dcb7ca30d8_Out_1);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_511992636cc64eeead7a6646c9cd16c6_Out_0 = _Amplitude;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_d5afbd154c484b53a6e669eec85bcd4a_Out_2;
            Unity_Multiply_float_float(_Cosine_5a4679fd61034839827e62dcb7ca30d8_Out_1, _Property_511992636cc64eeead7a6646c9cd16c6_Out_0, _Multiply_d5afbd154c484b53a6e669eec85bcd4a_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Add_cc682e4e900d48f997b4ef5ce6b4c083_Out_2;
            Unity_Add_float(_Multiply_d5afbd154c484b53a6e669eec85bcd4a_Out_2, _Split_7129975c62bc4203840744f4faa34665_R_1, _Add_cc682e4e900d48f997b4ef5ce6b4c083_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_f37a64f636b5429e9e9af6f790e18e1c_Out_0 = _YVariation;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_cc0b44b5a4ad4555823ef225c9e176d9_Out_2;
            Unity_Multiply_float_float(_Multiply_7a03097596a04c6fbaf7ac71a3a87967_Out_2, _Property_f37a64f636b5429e9e9af6f790e18e1c_Out_0, _Multiply_cc0b44b5a4ad4555823ef225c9e176d9_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_fbafb3410e6b4ab6bc7212393f6c5ac9_Out_0 = _Tilling;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_2d14785807f0423c9c78d3a9afaa91a3_Out_2;
            Unity_Multiply_float_float(_Property_fbafb3410e6b4ab6bc7212393f6c5ac9_Out_0, _Property_f37a64f636b5429e9e9af6f790e18e1c_Out_0, _Multiply_2d14785807f0423c9c78d3a9afaa91a3_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_a88a41a8a1474e4ea1649a4f1391243e_Out_2;
            Unity_Multiply_float_float(_Split_7129975c62bc4203840744f4faa34665_R_1, _Multiply_2d14785807f0423c9c78d3a9afaa91a3_Out_2, _Multiply_a88a41a8a1474e4ea1649a4f1391243e_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Add_665434cb9c6f4dfa8970ed70c7fb5c19_Out_2;
            Unity_Add_float(_Multiply_cc0b44b5a4ad4555823ef225c9e176d9_Out_2, _Multiply_a88a41a8a1474e4ea1649a4f1391243e_Out_2, _Add_665434cb9c6f4dfa8970ed70c7fb5c19_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Cosine_a87a978d261943fca172c387b4f4b04a_Out_1;
            Unity_Cosine_float(_Add_665434cb9c6f4dfa8970ed70c7fb5c19_Out_2, _Cosine_a87a978d261943fca172c387b4f4b04a_Out_1);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_76423ae27d8c4db5805619e50d47777e_Out_0 = _Amplitude;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_a18a8c6099ec41eebc85fe3fc0b76840_Out_2;
            Unity_Multiply_float_float(_Cosine_a87a978d261943fca172c387b4f4b04a_Out_1, _Property_76423ae27d8c4db5805619e50d47777e_Out_0, _Multiply_a18a8c6099ec41eebc85fe3fc0b76840_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Add_746bf4335d654588935b51e29aae56c2_Out_2;
            Unity_Add_float(_Multiply_a18a8c6099ec41eebc85fe3fc0b76840_Out_2, _Split_7129975c62bc4203840744f4faa34665_G_2, _Add_746bf4335d654588935b51e29aae56c2_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Vector3_0ad052ff4f2a468798f4efcedf596053_Out_0 = float3(_Add_cc682e4e900d48f997b4ef5ce6b4c083_Out_2, _Add_746bf4335d654588935b51e29aae56c2_Out_2, _Split_7129975c62bc4203840744f4faa34665_B_3);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Split_eab0210b1fb548b989a1a99c49265331_R_1 = 0;
            float _Split_eab0210b1fb548b989a1a99c49265331_G_2 = 0;
            float _Split_eab0210b1fb548b989a1a99c49265331_B_3 = 0;
            float _Split_eab0210b1fb548b989a1a99c49265331_A_4 = 0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_b0c8f86d3f39489685df628a23cd0169_Out_0 = _LockingYUvsPosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Distance_012097377ae8486392a7434139dc9cd7_Out_2;
            Unity_Distance_float(_Split_eab0210b1fb548b989a1a99c49265331_G_2, _Property_b0c8f86d3f39489685df628a23cd0169_Out_0, _Distance_012097377ae8486392a7434139dc9cd7_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_1f6079c1ae98418da0c4eea4cb31b1d4_Out_0 = _LockingTreshold;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Divide_0c657c0956eb4910b755b8672d4987af_Out_2;
            Unity_Divide_float(_Distance_012097377ae8486392a7434139dc9cd7_Out_2, _Property_1f6079c1ae98418da0c4eea4cb31b1d4_Out_0, _Divide_0c657c0956eb4910b755b8672d4987af_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _OneMinus_3b199092de3248239799f933efbb85d5_Out_1;
            Unity_OneMinus_float(_Divide_0c657c0956eb4910b755b8672d4987af_Out_2, _OneMinus_3b199092de3248239799f933efbb85d5_Out_1);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_7acf19eeea1d44ea99d7853ebceb3935_Out_0 = _LockingMovementReduction;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_987883e3416f481bac7f359ef37a3225_Out_2;
            Unity_Multiply_float_float(_OneMinus_3b199092de3248239799f933efbb85d5_Out_1, _Property_7acf19eeea1d44ea99d7853ebceb3935_Out_0, _Multiply_987883e3416f481bac7f359ef37a3225_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Clamp_cebcfa880551453ca122e777cde3b476_Out_3;
            Unity_Clamp_float(_Multiply_987883e3416f481bac7f359ef37a3225_Out_2, 0, 1, _Clamp_cebcfa880551453ca122e777cde3b476_Out_3);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Vector3_57c97d1a28a64fbf8fbc73c504698939_Out_0 = float3(_Clamp_cebcfa880551453ca122e777cde3b476_Out_3, _Clamp_cebcfa880551453ca122e777cde3b476_Out_3, _Clamp_cebcfa880551453ca122e777cde3b476_Out_3);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Lerp_26c3d4e649bf43709ed9aeea6401a5ea_Out_3;
            Unity_Lerp_float3(_Vector3_0ad052ff4f2a468798f4efcedf596053_Out_0, IN.ObjectSpacePosition, _Vector3_57c97d1a28a64fbf8fbc73c504698939_Out_0, _Lerp_26c3d4e649bf43709ed9aeea6401a5ea_Out_3);
            #endif
            description.Position = _Lerp_26c3d4e649bf43709ed9aeea6401a5ea_Out_3;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreSurface' */
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
            float4 SpriteMask;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Split_e11a52eeb5974259ae3fdc03f16c88b6_R_1 = SHADERGRAPH_OBJECT_POSITION[0];
            float _Split_e11a52eeb5974259ae3fdc03f16c88b6_G_2 = SHADERGRAPH_OBJECT_POSITION[1];
            float _Split_e11a52eeb5974259ae3fdc03f16c88b6_B_3 = SHADERGRAPH_OBJECT_POSITION[2];
            float _Split_e11a52eeb5974259ae3fdc03f16c88b6_A_4 = 0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Split_4a9ab7976a2a42319d519c3a206784c5_R_1 = IN.WorldSpacePosition[0];
            float _Split_4a9ab7976a2a42319d519c3a206784c5_G_2 = IN.WorldSpacePosition[1];
            float _Split_4a9ab7976a2a42319d519c3a206784c5_B_3 = IN.WorldSpacePosition[2];
            float _Split_4a9ab7976a2a42319d519c3a206784c5_A_4 = 0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Subtract_fa4394f9d8314594b34b4d7d0ea9501a_Out_2;
            Unity_Subtract_float(_Split_e11a52eeb5974259ae3fdc03f16c88b6_R_1, _Split_4a9ab7976a2a42319d519c3a206784c5_R_1, _Subtract_fa4394f9d8314594b34b4d7d0ea9501a_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            UnityTexture2D _Property_75679be3672b4d37ac7e847641aa839e_Out_0 = UnityBuildTexture2DStruct(_MainTex);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_RGBA_0 = SAMPLE_TEXTURE2D(_Property_75679be3672b4d37ac7e847641aa839e_Out_0.tex, _Property_75679be3672b4d37ac7e847641aa839e_Out_0.samplerstate, _Property_75679be3672b4d37ac7e847641aa839e_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_R_4 = _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_RGBA_0.r;
            float _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_G_5 = _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_RGBA_0.g;
            float _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_B_6 = _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_RGBA_0.b;
            float _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_A_7 = _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_RGBA_0.a;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            #if defined(_COLORPREVIEW)
            float4 _ColorPreview_588fe1233ab04f95b91a2a1f678d94ff_Out_0 = (_Subtract_fa4394f9d8314594b34b4d7d0ea9501a_Out_2.xxxx);
            #else
            float4 _ColorPreview_588fe1233ab04f95b91a2a1f678d94ff_Out_0 = _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_RGBA_0;
            #endif
            #endif
            surface.BaseColor = (_ColorPreview_588fe1233ab04f95b91a2a1f678d94ff_Out_0.xyz);
            surface.Alpha = _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_A_7;
            surface.SpriteMask = IsGammaSpace() ? float4(1, 1, 1, 1) : float4 (SRGBToLinear(float3(1, 1, 1)), 1);
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpaceNormal =                          input.normalOS;
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpaceTangent =                         input.tangentOS.xyz;
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpacePosition =                        input.positionOS;
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.TimeParameters =                             _TimeParameters.xyz;
        #endif
        
        
            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorCopyToSDI' */
        
        
        
        
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpacePosition =                         input.positionWS;
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.uv0 =                                        input.texCoord0;
        #endif
        
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
            return output;
        }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"
        
            ENDHLSL
        }
        Pass
        {
            Name "Sprite Normal"
            Tags
            {
                "LightMode" = "NormalsRendering"
            }
        
            // Render State
            Cull Off
        Blend One OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>
        
            // Keywords
            // PassKeywords: <None>
            #pragma shader_feature_local _ _COLORPREVIEW
        
        #if defined(_COLORPREVIEW)
            #define KEYWORD_PERMUTATION_0
        #else
            #define KEYWORD_PERMUTATION_1
        #endif
        
        
            // Defines
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define _SURFACE_TYPE_TRANSPARENT 1
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define _ALPHAPREMULTIPLY_ON 1
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define ATTRIBUTES_NEED_NORMAL
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define ATTRIBUTES_NEED_TANGENT
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define ATTRIBUTES_NEED_TEXCOORD0
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_NORMAL_WS
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_TANGENT_WS
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_TEXCOORD0
        #endif
        
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SPRITENORMAL
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
            // Includes
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/NormalsRenderingShared.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
            struct Attributes
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 positionOS : POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 normalOS : NORMAL;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 tangentOS : TANGENT;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 uv0 : TEXCOORD0;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
            #endif
        };
        struct Varyings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 normalWS;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 tangentWS;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 texCoord0;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 TangentSpaceNormal;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 uv0;
            #endif
        };
        struct VertexDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 ObjectSpaceNormal;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 ObjectSpaceTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 ObjectSpacePosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 TimeParameters;
            #endif
        };
        struct PackedVaryings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 interp0 : INTERP0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 interp1 : INTERP1;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 interp2 : INTERP2;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };
        
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.normalWS;
            output.interp1.xyzw =  input.tangentWS;
            output.interp2.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.normalWS = input.interp0.xyz;
            output.tangentWS = input.interp1.xyzw;
            output.texCoord0 = input.interp2.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        #endif
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float _Speed;
        float _Tilling;
        float _Amplitude;
        float _YVariation;
        float _GroundSpeedLimit;
        float4 _MainTex_TexelSize;
        float4 _MainTex_ST;
        float _LockingMovementReduction;
        float _LockingYUvsPosition;
        float _LockingTreshold;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
            // Graph Includes
            // GraphIncludes: <None>
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Graph Functions
            
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Cosine_float(float In, out float Out)
        {
            Out = cos(In);
        }
        
        void Unity_Distance_float(float A, float B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        void Unity_Lerp_float3(float3 A, float3 B, float3 T, out float3 Out)
        {
            Out = lerp(A, B, T);
        }
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_2f9d2b3a5d124d9e98433a5997953d56_Out_0 = _Speed;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_7a03097596a04c6fbaf7ac71a3a87967_Out_2;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_2f9d2b3a5d124d9e98433a5997953d56_Out_0, _Multiply_7a03097596a04c6fbaf7ac71a3a87967_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Split_7129975c62bc4203840744f4faa34665_R_1 = IN.ObjectSpacePosition[0];
            float _Split_7129975c62bc4203840744f4faa34665_G_2 = IN.ObjectSpacePosition[1];
            float _Split_7129975c62bc4203840744f4faa34665_B_3 = IN.ObjectSpacePosition[2];
            float _Split_7129975c62bc4203840744f4faa34665_A_4 = 0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_ac7d909912204819babee1a8fc569ca1_Out_0 = _Tilling;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_6086e5426cd74fb0a516e28465b0eb11_Out_2;
            Unity_Multiply_float_float(_Split_7129975c62bc4203840744f4faa34665_G_2, _Property_ac7d909912204819babee1a8fc569ca1_Out_0, _Multiply_6086e5426cd74fb0a516e28465b0eb11_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Add_d9603b01777f4b5fadb1eb750f87eac1_Out_2;
            Unity_Add_float(_Multiply_7a03097596a04c6fbaf7ac71a3a87967_Out_2, _Multiply_6086e5426cd74fb0a516e28465b0eb11_Out_2, _Add_d9603b01777f4b5fadb1eb750f87eac1_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Cosine_5a4679fd61034839827e62dcb7ca30d8_Out_1;
            Unity_Cosine_float(_Add_d9603b01777f4b5fadb1eb750f87eac1_Out_2, _Cosine_5a4679fd61034839827e62dcb7ca30d8_Out_1);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_511992636cc64eeead7a6646c9cd16c6_Out_0 = _Amplitude;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_d5afbd154c484b53a6e669eec85bcd4a_Out_2;
            Unity_Multiply_float_float(_Cosine_5a4679fd61034839827e62dcb7ca30d8_Out_1, _Property_511992636cc64eeead7a6646c9cd16c6_Out_0, _Multiply_d5afbd154c484b53a6e669eec85bcd4a_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Add_cc682e4e900d48f997b4ef5ce6b4c083_Out_2;
            Unity_Add_float(_Multiply_d5afbd154c484b53a6e669eec85bcd4a_Out_2, _Split_7129975c62bc4203840744f4faa34665_R_1, _Add_cc682e4e900d48f997b4ef5ce6b4c083_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_f37a64f636b5429e9e9af6f790e18e1c_Out_0 = _YVariation;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_cc0b44b5a4ad4555823ef225c9e176d9_Out_2;
            Unity_Multiply_float_float(_Multiply_7a03097596a04c6fbaf7ac71a3a87967_Out_2, _Property_f37a64f636b5429e9e9af6f790e18e1c_Out_0, _Multiply_cc0b44b5a4ad4555823ef225c9e176d9_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_fbafb3410e6b4ab6bc7212393f6c5ac9_Out_0 = _Tilling;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_2d14785807f0423c9c78d3a9afaa91a3_Out_2;
            Unity_Multiply_float_float(_Property_fbafb3410e6b4ab6bc7212393f6c5ac9_Out_0, _Property_f37a64f636b5429e9e9af6f790e18e1c_Out_0, _Multiply_2d14785807f0423c9c78d3a9afaa91a3_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_a88a41a8a1474e4ea1649a4f1391243e_Out_2;
            Unity_Multiply_float_float(_Split_7129975c62bc4203840744f4faa34665_R_1, _Multiply_2d14785807f0423c9c78d3a9afaa91a3_Out_2, _Multiply_a88a41a8a1474e4ea1649a4f1391243e_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Add_665434cb9c6f4dfa8970ed70c7fb5c19_Out_2;
            Unity_Add_float(_Multiply_cc0b44b5a4ad4555823ef225c9e176d9_Out_2, _Multiply_a88a41a8a1474e4ea1649a4f1391243e_Out_2, _Add_665434cb9c6f4dfa8970ed70c7fb5c19_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Cosine_a87a978d261943fca172c387b4f4b04a_Out_1;
            Unity_Cosine_float(_Add_665434cb9c6f4dfa8970ed70c7fb5c19_Out_2, _Cosine_a87a978d261943fca172c387b4f4b04a_Out_1);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_76423ae27d8c4db5805619e50d47777e_Out_0 = _Amplitude;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_a18a8c6099ec41eebc85fe3fc0b76840_Out_2;
            Unity_Multiply_float_float(_Cosine_a87a978d261943fca172c387b4f4b04a_Out_1, _Property_76423ae27d8c4db5805619e50d47777e_Out_0, _Multiply_a18a8c6099ec41eebc85fe3fc0b76840_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Add_746bf4335d654588935b51e29aae56c2_Out_2;
            Unity_Add_float(_Multiply_a18a8c6099ec41eebc85fe3fc0b76840_Out_2, _Split_7129975c62bc4203840744f4faa34665_G_2, _Add_746bf4335d654588935b51e29aae56c2_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Vector3_0ad052ff4f2a468798f4efcedf596053_Out_0 = float3(_Add_cc682e4e900d48f997b4ef5ce6b4c083_Out_2, _Add_746bf4335d654588935b51e29aae56c2_Out_2, _Split_7129975c62bc4203840744f4faa34665_B_3);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Split_eab0210b1fb548b989a1a99c49265331_R_1 = 0;
            float _Split_eab0210b1fb548b989a1a99c49265331_G_2 = 0;
            float _Split_eab0210b1fb548b989a1a99c49265331_B_3 = 0;
            float _Split_eab0210b1fb548b989a1a99c49265331_A_4 = 0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_b0c8f86d3f39489685df628a23cd0169_Out_0 = _LockingYUvsPosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Distance_012097377ae8486392a7434139dc9cd7_Out_2;
            Unity_Distance_float(_Split_eab0210b1fb548b989a1a99c49265331_G_2, _Property_b0c8f86d3f39489685df628a23cd0169_Out_0, _Distance_012097377ae8486392a7434139dc9cd7_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_1f6079c1ae98418da0c4eea4cb31b1d4_Out_0 = _LockingTreshold;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Divide_0c657c0956eb4910b755b8672d4987af_Out_2;
            Unity_Divide_float(_Distance_012097377ae8486392a7434139dc9cd7_Out_2, _Property_1f6079c1ae98418da0c4eea4cb31b1d4_Out_0, _Divide_0c657c0956eb4910b755b8672d4987af_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _OneMinus_3b199092de3248239799f933efbb85d5_Out_1;
            Unity_OneMinus_float(_Divide_0c657c0956eb4910b755b8672d4987af_Out_2, _OneMinus_3b199092de3248239799f933efbb85d5_Out_1);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_7acf19eeea1d44ea99d7853ebceb3935_Out_0 = _LockingMovementReduction;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_987883e3416f481bac7f359ef37a3225_Out_2;
            Unity_Multiply_float_float(_OneMinus_3b199092de3248239799f933efbb85d5_Out_1, _Property_7acf19eeea1d44ea99d7853ebceb3935_Out_0, _Multiply_987883e3416f481bac7f359ef37a3225_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Clamp_cebcfa880551453ca122e777cde3b476_Out_3;
            Unity_Clamp_float(_Multiply_987883e3416f481bac7f359ef37a3225_Out_2, 0, 1, _Clamp_cebcfa880551453ca122e777cde3b476_Out_3);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Vector3_57c97d1a28a64fbf8fbc73c504698939_Out_0 = float3(_Clamp_cebcfa880551453ca122e777cde3b476_Out_3, _Clamp_cebcfa880551453ca122e777cde3b476_Out_3, _Clamp_cebcfa880551453ca122e777cde3b476_Out_3);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Lerp_26c3d4e649bf43709ed9aeea6401a5ea_Out_3;
            Unity_Lerp_float3(_Vector3_0ad052ff4f2a468798f4efcedf596053_Out_0, IN.ObjectSpacePosition, _Vector3_57c97d1a28a64fbf8fbc73c504698939_Out_0, _Lerp_26c3d4e649bf43709ed9aeea6401a5ea_Out_3);
            #endif
            description.Position = _Lerp_26c3d4e649bf43709ed9aeea6401a5ea_Out_3;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreSurface' */
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
            float3 NormalTS;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            UnityTexture2D _Property_75679be3672b4d37ac7e847641aa839e_Out_0 = UnityBuildTexture2DStruct(_MainTex);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_RGBA_0 = SAMPLE_TEXTURE2D(_Property_75679be3672b4d37ac7e847641aa839e_Out_0.tex, _Property_75679be3672b4d37ac7e847641aa839e_Out_0.samplerstate, _Property_75679be3672b4d37ac7e847641aa839e_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_R_4 = _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_RGBA_0.r;
            float _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_G_5 = _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_RGBA_0.g;
            float _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_B_6 = _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_RGBA_0.b;
            float _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_A_7 = _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_RGBA_0.a;
            #endif
            surface.Alpha = _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_A_7;
            surface.NormalTS = IN.TangentSpaceNormal;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpaceNormal =                          input.normalOS;
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpaceTangent =                         input.tangentOS.xyz;
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpacePosition =                        input.positionOS;
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.TimeParameters =                             _TimeParameters.xyz;
        #endif
        
        
            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorCopyToSDI' */
        
        
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.TangentSpaceNormal =                         float3(0.0f, 0.0f, 1.0f);
        #endif
        
        
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.uv0 =                                        input.texCoord0;
        #endif
        
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
            return output;
        }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteNormalPass.hlsl"
        
            ENDHLSL
        }
        Pass
        {
            Name "SceneSelectionPass"
            Tags
            {
                "LightMode" = "SceneSelectionPass"
            }
        
            // Render State
            Cull Off
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>
        
            // Keywords
            // PassKeywords: <None>
            #pragma shader_feature_local _ _COLORPREVIEW
        
        #if defined(_COLORPREVIEW)
            #define KEYWORD_PERMUTATION_0
        #else
            #define KEYWORD_PERMUTATION_1
        #endif
        
        
            // Defines
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define _SURFACE_TYPE_TRANSPARENT 1
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define _ALPHAPREMULTIPLY_ON 1
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define ATTRIBUTES_NEED_NORMAL
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define ATTRIBUTES_NEED_TANGENT
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define ATTRIBUTES_NEED_TEXCOORD0
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_TEXCOORD0
        #endif
        
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENESELECTIONPASS 1
        
        #define _ALPHATEST_ON 1
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
            // Includes
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
            struct Attributes
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 positionOS : POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 normalOS : NORMAL;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 tangentOS : TANGENT;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 uv0 : TEXCOORD0;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
            #endif
        };
        struct Varyings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 texCoord0;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 uv0;
            #endif
        };
        struct VertexDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 ObjectSpaceNormal;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 ObjectSpaceTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 ObjectSpacePosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 TimeParameters;
            #endif
        };
        struct PackedVaryings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 interp0 : INTERP0;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };
        
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.interp0.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        #endif
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float _Speed;
        float _Tilling;
        float _Amplitude;
        float _YVariation;
        float _GroundSpeedLimit;
        float4 _MainTex_TexelSize;
        float4 _MainTex_ST;
        float _LockingMovementReduction;
        float _LockingYUvsPosition;
        float _LockingTreshold;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
            // Graph Includes
            // GraphIncludes: <None>
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Graph Functions
            
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Cosine_float(float In, out float Out)
        {
            Out = cos(In);
        }
        
        void Unity_Distance_float(float A, float B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        void Unity_Lerp_float3(float3 A, float3 B, float3 T, out float3 Out)
        {
            Out = lerp(A, B, T);
        }
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_2f9d2b3a5d124d9e98433a5997953d56_Out_0 = _Speed;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_7a03097596a04c6fbaf7ac71a3a87967_Out_2;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_2f9d2b3a5d124d9e98433a5997953d56_Out_0, _Multiply_7a03097596a04c6fbaf7ac71a3a87967_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Split_7129975c62bc4203840744f4faa34665_R_1 = IN.ObjectSpacePosition[0];
            float _Split_7129975c62bc4203840744f4faa34665_G_2 = IN.ObjectSpacePosition[1];
            float _Split_7129975c62bc4203840744f4faa34665_B_3 = IN.ObjectSpacePosition[2];
            float _Split_7129975c62bc4203840744f4faa34665_A_4 = 0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_ac7d909912204819babee1a8fc569ca1_Out_0 = _Tilling;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_6086e5426cd74fb0a516e28465b0eb11_Out_2;
            Unity_Multiply_float_float(_Split_7129975c62bc4203840744f4faa34665_G_2, _Property_ac7d909912204819babee1a8fc569ca1_Out_0, _Multiply_6086e5426cd74fb0a516e28465b0eb11_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Add_d9603b01777f4b5fadb1eb750f87eac1_Out_2;
            Unity_Add_float(_Multiply_7a03097596a04c6fbaf7ac71a3a87967_Out_2, _Multiply_6086e5426cd74fb0a516e28465b0eb11_Out_2, _Add_d9603b01777f4b5fadb1eb750f87eac1_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Cosine_5a4679fd61034839827e62dcb7ca30d8_Out_1;
            Unity_Cosine_float(_Add_d9603b01777f4b5fadb1eb750f87eac1_Out_2, _Cosine_5a4679fd61034839827e62dcb7ca30d8_Out_1);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_511992636cc64eeead7a6646c9cd16c6_Out_0 = _Amplitude;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_d5afbd154c484b53a6e669eec85bcd4a_Out_2;
            Unity_Multiply_float_float(_Cosine_5a4679fd61034839827e62dcb7ca30d8_Out_1, _Property_511992636cc64eeead7a6646c9cd16c6_Out_0, _Multiply_d5afbd154c484b53a6e669eec85bcd4a_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Add_cc682e4e900d48f997b4ef5ce6b4c083_Out_2;
            Unity_Add_float(_Multiply_d5afbd154c484b53a6e669eec85bcd4a_Out_2, _Split_7129975c62bc4203840744f4faa34665_R_1, _Add_cc682e4e900d48f997b4ef5ce6b4c083_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_f37a64f636b5429e9e9af6f790e18e1c_Out_0 = _YVariation;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_cc0b44b5a4ad4555823ef225c9e176d9_Out_2;
            Unity_Multiply_float_float(_Multiply_7a03097596a04c6fbaf7ac71a3a87967_Out_2, _Property_f37a64f636b5429e9e9af6f790e18e1c_Out_0, _Multiply_cc0b44b5a4ad4555823ef225c9e176d9_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_fbafb3410e6b4ab6bc7212393f6c5ac9_Out_0 = _Tilling;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_2d14785807f0423c9c78d3a9afaa91a3_Out_2;
            Unity_Multiply_float_float(_Property_fbafb3410e6b4ab6bc7212393f6c5ac9_Out_0, _Property_f37a64f636b5429e9e9af6f790e18e1c_Out_0, _Multiply_2d14785807f0423c9c78d3a9afaa91a3_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_a88a41a8a1474e4ea1649a4f1391243e_Out_2;
            Unity_Multiply_float_float(_Split_7129975c62bc4203840744f4faa34665_R_1, _Multiply_2d14785807f0423c9c78d3a9afaa91a3_Out_2, _Multiply_a88a41a8a1474e4ea1649a4f1391243e_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Add_665434cb9c6f4dfa8970ed70c7fb5c19_Out_2;
            Unity_Add_float(_Multiply_cc0b44b5a4ad4555823ef225c9e176d9_Out_2, _Multiply_a88a41a8a1474e4ea1649a4f1391243e_Out_2, _Add_665434cb9c6f4dfa8970ed70c7fb5c19_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Cosine_a87a978d261943fca172c387b4f4b04a_Out_1;
            Unity_Cosine_float(_Add_665434cb9c6f4dfa8970ed70c7fb5c19_Out_2, _Cosine_a87a978d261943fca172c387b4f4b04a_Out_1);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_76423ae27d8c4db5805619e50d47777e_Out_0 = _Amplitude;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_a18a8c6099ec41eebc85fe3fc0b76840_Out_2;
            Unity_Multiply_float_float(_Cosine_a87a978d261943fca172c387b4f4b04a_Out_1, _Property_76423ae27d8c4db5805619e50d47777e_Out_0, _Multiply_a18a8c6099ec41eebc85fe3fc0b76840_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Add_746bf4335d654588935b51e29aae56c2_Out_2;
            Unity_Add_float(_Multiply_a18a8c6099ec41eebc85fe3fc0b76840_Out_2, _Split_7129975c62bc4203840744f4faa34665_G_2, _Add_746bf4335d654588935b51e29aae56c2_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Vector3_0ad052ff4f2a468798f4efcedf596053_Out_0 = float3(_Add_cc682e4e900d48f997b4ef5ce6b4c083_Out_2, _Add_746bf4335d654588935b51e29aae56c2_Out_2, _Split_7129975c62bc4203840744f4faa34665_B_3);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Split_eab0210b1fb548b989a1a99c49265331_R_1 = 0;
            float _Split_eab0210b1fb548b989a1a99c49265331_G_2 = 0;
            float _Split_eab0210b1fb548b989a1a99c49265331_B_3 = 0;
            float _Split_eab0210b1fb548b989a1a99c49265331_A_4 = 0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_b0c8f86d3f39489685df628a23cd0169_Out_0 = _LockingYUvsPosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Distance_012097377ae8486392a7434139dc9cd7_Out_2;
            Unity_Distance_float(_Split_eab0210b1fb548b989a1a99c49265331_G_2, _Property_b0c8f86d3f39489685df628a23cd0169_Out_0, _Distance_012097377ae8486392a7434139dc9cd7_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_1f6079c1ae98418da0c4eea4cb31b1d4_Out_0 = _LockingTreshold;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Divide_0c657c0956eb4910b755b8672d4987af_Out_2;
            Unity_Divide_float(_Distance_012097377ae8486392a7434139dc9cd7_Out_2, _Property_1f6079c1ae98418da0c4eea4cb31b1d4_Out_0, _Divide_0c657c0956eb4910b755b8672d4987af_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _OneMinus_3b199092de3248239799f933efbb85d5_Out_1;
            Unity_OneMinus_float(_Divide_0c657c0956eb4910b755b8672d4987af_Out_2, _OneMinus_3b199092de3248239799f933efbb85d5_Out_1);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_7acf19eeea1d44ea99d7853ebceb3935_Out_0 = _LockingMovementReduction;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_987883e3416f481bac7f359ef37a3225_Out_2;
            Unity_Multiply_float_float(_OneMinus_3b199092de3248239799f933efbb85d5_Out_1, _Property_7acf19eeea1d44ea99d7853ebceb3935_Out_0, _Multiply_987883e3416f481bac7f359ef37a3225_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Clamp_cebcfa880551453ca122e777cde3b476_Out_3;
            Unity_Clamp_float(_Multiply_987883e3416f481bac7f359ef37a3225_Out_2, 0, 1, _Clamp_cebcfa880551453ca122e777cde3b476_Out_3);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Vector3_57c97d1a28a64fbf8fbc73c504698939_Out_0 = float3(_Clamp_cebcfa880551453ca122e777cde3b476_Out_3, _Clamp_cebcfa880551453ca122e777cde3b476_Out_3, _Clamp_cebcfa880551453ca122e777cde3b476_Out_3);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Lerp_26c3d4e649bf43709ed9aeea6401a5ea_Out_3;
            Unity_Lerp_float3(_Vector3_0ad052ff4f2a468798f4efcedf596053_Out_0, IN.ObjectSpacePosition, _Vector3_57c97d1a28a64fbf8fbc73c504698939_Out_0, _Lerp_26c3d4e649bf43709ed9aeea6401a5ea_Out_3);
            #endif
            description.Position = _Lerp_26c3d4e649bf43709ed9aeea6401a5ea_Out_3;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
            #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            UnityTexture2D _Property_75679be3672b4d37ac7e847641aa839e_Out_0 = UnityBuildTexture2DStruct(_MainTex);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_RGBA_0 = SAMPLE_TEXTURE2D(_Property_75679be3672b4d37ac7e847641aa839e_Out_0.tex, _Property_75679be3672b4d37ac7e847641aa839e_Out_0.samplerstate, _Property_75679be3672b4d37ac7e847641aa839e_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_R_4 = _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_RGBA_0.r;
            float _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_G_5 = _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_RGBA_0.g;
            float _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_B_6 = _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_RGBA_0.b;
            float _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_A_7 = _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_RGBA_0.a;
            #endif
            surface.Alpha = _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_A_7;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpaceNormal =                          input.normalOS;
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpaceTangent =                         input.tangentOS.xyz;
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpacePosition =                        input.positionOS;
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.TimeParameters =                             _TimeParameters.xyz;
        #endif
        
        
            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            
        
        
        
        
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.uv0 =                                        input.texCoord0;
        #endif
        
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
            return output;
        }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
            ENDHLSL
        }
        Pass
        {
            Name "ScenePickingPass"
            Tags
            {
                "LightMode" = "Picking"
            }
        
            // Render State
            Cull Back
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>
        
            // Keywords
            // PassKeywords: <None>
            #pragma shader_feature_local _ _COLORPREVIEW
        
        #if defined(_COLORPREVIEW)
            #define KEYWORD_PERMUTATION_0
        #else
            #define KEYWORD_PERMUTATION_1
        #endif
        
        
            // Defines
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define _SURFACE_TYPE_TRANSPARENT 1
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define _ALPHAPREMULTIPLY_ON 1
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define ATTRIBUTES_NEED_NORMAL
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define ATTRIBUTES_NEED_TANGENT
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define ATTRIBUTES_NEED_TEXCOORD0
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_TEXCOORD0
        #endif
        
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENEPICKINGPASS 1
        
        #define _ALPHATEST_ON 1
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
            // Includes
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
            struct Attributes
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 positionOS : POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 normalOS : NORMAL;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 tangentOS : TANGENT;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 uv0 : TEXCOORD0;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
            #endif
        };
        struct Varyings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 texCoord0;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 uv0;
            #endif
        };
        struct VertexDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 ObjectSpaceNormal;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 ObjectSpaceTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 ObjectSpacePosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 TimeParameters;
            #endif
        };
        struct PackedVaryings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 interp0 : INTERP0;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };
        
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.interp0.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        #endif
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float _Speed;
        float _Tilling;
        float _Amplitude;
        float _YVariation;
        float _GroundSpeedLimit;
        float4 _MainTex_TexelSize;
        float4 _MainTex_ST;
        float _LockingMovementReduction;
        float _LockingYUvsPosition;
        float _LockingTreshold;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
            // Graph Includes
            // GraphIncludes: <None>
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Graph Functions
            
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Cosine_float(float In, out float Out)
        {
            Out = cos(In);
        }
        
        void Unity_Distance_float(float A, float B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        void Unity_Lerp_float3(float3 A, float3 B, float3 T, out float3 Out)
        {
            Out = lerp(A, B, T);
        }
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_2f9d2b3a5d124d9e98433a5997953d56_Out_0 = _Speed;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_7a03097596a04c6fbaf7ac71a3a87967_Out_2;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_2f9d2b3a5d124d9e98433a5997953d56_Out_0, _Multiply_7a03097596a04c6fbaf7ac71a3a87967_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Split_7129975c62bc4203840744f4faa34665_R_1 = IN.ObjectSpacePosition[0];
            float _Split_7129975c62bc4203840744f4faa34665_G_2 = IN.ObjectSpacePosition[1];
            float _Split_7129975c62bc4203840744f4faa34665_B_3 = IN.ObjectSpacePosition[2];
            float _Split_7129975c62bc4203840744f4faa34665_A_4 = 0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_ac7d909912204819babee1a8fc569ca1_Out_0 = _Tilling;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_6086e5426cd74fb0a516e28465b0eb11_Out_2;
            Unity_Multiply_float_float(_Split_7129975c62bc4203840744f4faa34665_G_2, _Property_ac7d909912204819babee1a8fc569ca1_Out_0, _Multiply_6086e5426cd74fb0a516e28465b0eb11_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Add_d9603b01777f4b5fadb1eb750f87eac1_Out_2;
            Unity_Add_float(_Multiply_7a03097596a04c6fbaf7ac71a3a87967_Out_2, _Multiply_6086e5426cd74fb0a516e28465b0eb11_Out_2, _Add_d9603b01777f4b5fadb1eb750f87eac1_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Cosine_5a4679fd61034839827e62dcb7ca30d8_Out_1;
            Unity_Cosine_float(_Add_d9603b01777f4b5fadb1eb750f87eac1_Out_2, _Cosine_5a4679fd61034839827e62dcb7ca30d8_Out_1);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_511992636cc64eeead7a6646c9cd16c6_Out_0 = _Amplitude;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_d5afbd154c484b53a6e669eec85bcd4a_Out_2;
            Unity_Multiply_float_float(_Cosine_5a4679fd61034839827e62dcb7ca30d8_Out_1, _Property_511992636cc64eeead7a6646c9cd16c6_Out_0, _Multiply_d5afbd154c484b53a6e669eec85bcd4a_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Add_cc682e4e900d48f997b4ef5ce6b4c083_Out_2;
            Unity_Add_float(_Multiply_d5afbd154c484b53a6e669eec85bcd4a_Out_2, _Split_7129975c62bc4203840744f4faa34665_R_1, _Add_cc682e4e900d48f997b4ef5ce6b4c083_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_f37a64f636b5429e9e9af6f790e18e1c_Out_0 = _YVariation;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_cc0b44b5a4ad4555823ef225c9e176d9_Out_2;
            Unity_Multiply_float_float(_Multiply_7a03097596a04c6fbaf7ac71a3a87967_Out_2, _Property_f37a64f636b5429e9e9af6f790e18e1c_Out_0, _Multiply_cc0b44b5a4ad4555823ef225c9e176d9_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_fbafb3410e6b4ab6bc7212393f6c5ac9_Out_0 = _Tilling;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_2d14785807f0423c9c78d3a9afaa91a3_Out_2;
            Unity_Multiply_float_float(_Property_fbafb3410e6b4ab6bc7212393f6c5ac9_Out_0, _Property_f37a64f636b5429e9e9af6f790e18e1c_Out_0, _Multiply_2d14785807f0423c9c78d3a9afaa91a3_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_a88a41a8a1474e4ea1649a4f1391243e_Out_2;
            Unity_Multiply_float_float(_Split_7129975c62bc4203840744f4faa34665_R_1, _Multiply_2d14785807f0423c9c78d3a9afaa91a3_Out_2, _Multiply_a88a41a8a1474e4ea1649a4f1391243e_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Add_665434cb9c6f4dfa8970ed70c7fb5c19_Out_2;
            Unity_Add_float(_Multiply_cc0b44b5a4ad4555823ef225c9e176d9_Out_2, _Multiply_a88a41a8a1474e4ea1649a4f1391243e_Out_2, _Add_665434cb9c6f4dfa8970ed70c7fb5c19_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Cosine_a87a978d261943fca172c387b4f4b04a_Out_1;
            Unity_Cosine_float(_Add_665434cb9c6f4dfa8970ed70c7fb5c19_Out_2, _Cosine_a87a978d261943fca172c387b4f4b04a_Out_1);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_76423ae27d8c4db5805619e50d47777e_Out_0 = _Amplitude;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_a18a8c6099ec41eebc85fe3fc0b76840_Out_2;
            Unity_Multiply_float_float(_Cosine_a87a978d261943fca172c387b4f4b04a_Out_1, _Property_76423ae27d8c4db5805619e50d47777e_Out_0, _Multiply_a18a8c6099ec41eebc85fe3fc0b76840_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Add_746bf4335d654588935b51e29aae56c2_Out_2;
            Unity_Add_float(_Multiply_a18a8c6099ec41eebc85fe3fc0b76840_Out_2, _Split_7129975c62bc4203840744f4faa34665_G_2, _Add_746bf4335d654588935b51e29aae56c2_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Vector3_0ad052ff4f2a468798f4efcedf596053_Out_0 = float3(_Add_cc682e4e900d48f997b4ef5ce6b4c083_Out_2, _Add_746bf4335d654588935b51e29aae56c2_Out_2, _Split_7129975c62bc4203840744f4faa34665_B_3);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Split_eab0210b1fb548b989a1a99c49265331_R_1 = 0;
            float _Split_eab0210b1fb548b989a1a99c49265331_G_2 = 0;
            float _Split_eab0210b1fb548b989a1a99c49265331_B_3 = 0;
            float _Split_eab0210b1fb548b989a1a99c49265331_A_4 = 0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_b0c8f86d3f39489685df628a23cd0169_Out_0 = _LockingYUvsPosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Distance_012097377ae8486392a7434139dc9cd7_Out_2;
            Unity_Distance_float(_Split_eab0210b1fb548b989a1a99c49265331_G_2, _Property_b0c8f86d3f39489685df628a23cd0169_Out_0, _Distance_012097377ae8486392a7434139dc9cd7_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_1f6079c1ae98418da0c4eea4cb31b1d4_Out_0 = _LockingTreshold;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Divide_0c657c0956eb4910b755b8672d4987af_Out_2;
            Unity_Divide_float(_Distance_012097377ae8486392a7434139dc9cd7_Out_2, _Property_1f6079c1ae98418da0c4eea4cb31b1d4_Out_0, _Divide_0c657c0956eb4910b755b8672d4987af_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _OneMinus_3b199092de3248239799f933efbb85d5_Out_1;
            Unity_OneMinus_float(_Divide_0c657c0956eb4910b755b8672d4987af_Out_2, _OneMinus_3b199092de3248239799f933efbb85d5_Out_1);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_7acf19eeea1d44ea99d7853ebceb3935_Out_0 = _LockingMovementReduction;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_987883e3416f481bac7f359ef37a3225_Out_2;
            Unity_Multiply_float_float(_OneMinus_3b199092de3248239799f933efbb85d5_Out_1, _Property_7acf19eeea1d44ea99d7853ebceb3935_Out_0, _Multiply_987883e3416f481bac7f359ef37a3225_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Clamp_cebcfa880551453ca122e777cde3b476_Out_3;
            Unity_Clamp_float(_Multiply_987883e3416f481bac7f359ef37a3225_Out_2, 0, 1, _Clamp_cebcfa880551453ca122e777cde3b476_Out_3);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Vector3_57c97d1a28a64fbf8fbc73c504698939_Out_0 = float3(_Clamp_cebcfa880551453ca122e777cde3b476_Out_3, _Clamp_cebcfa880551453ca122e777cde3b476_Out_3, _Clamp_cebcfa880551453ca122e777cde3b476_Out_3);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Lerp_26c3d4e649bf43709ed9aeea6401a5ea_Out_3;
            Unity_Lerp_float3(_Vector3_0ad052ff4f2a468798f4efcedf596053_Out_0, IN.ObjectSpacePosition, _Vector3_57c97d1a28a64fbf8fbc73c504698939_Out_0, _Lerp_26c3d4e649bf43709ed9aeea6401a5ea_Out_3);
            #endif
            description.Position = _Lerp_26c3d4e649bf43709ed9aeea6401a5ea_Out_3;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
            #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            UnityTexture2D _Property_75679be3672b4d37ac7e847641aa839e_Out_0 = UnityBuildTexture2DStruct(_MainTex);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_RGBA_0 = SAMPLE_TEXTURE2D(_Property_75679be3672b4d37ac7e847641aa839e_Out_0.tex, _Property_75679be3672b4d37ac7e847641aa839e_Out_0.samplerstate, _Property_75679be3672b4d37ac7e847641aa839e_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_R_4 = _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_RGBA_0.r;
            float _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_G_5 = _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_RGBA_0.g;
            float _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_B_6 = _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_RGBA_0.b;
            float _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_A_7 = _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_RGBA_0.a;
            #endif
            surface.Alpha = _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_A_7;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpaceNormal =                          input.normalOS;
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpaceTangent =                         input.tangentOS.xyz;
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpacePosition =                        input.positionOS;
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.TimeParameters =                             _TimeParameters.xyz;
        #endif
        
        
            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            
        
        
        
        
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.uv0 =                                        input.texCoord0;
        #endif
        
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
            return output;
        }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
            ENDHLSL
        }
        Pass
        {
            Name "Sprite Forward"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
        
            // Render State
            Cull Off
        Blend One OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>
        
            // Keywords
            #pragma multi_compile_fragment _ DEBUG_DISPLAY
            #pragma shader_feature_local _ _COLORPREVIEW
        
        #if defined(_COLORPREVIEW)
            #define KEYWORD_PERMUTATION_0
        #else
            #define KEYWORD_PERMUTATION_1
        #endif
        
        
            // Defines
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define _SURFACE_TYPE_TRANSPARENT 1
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define _ALPHAPREMULTIPLY_ON 1
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define ATTRIBUTES_NEED_NORMAL
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define ATTRIBUTES_NEED_TANGENT
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define ATTRIBUTES_NEED_TEXCOORD0
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define ATTRIBUTES_NEED_COLOR
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_POSITION_WS
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_TEXCOORD0
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        #define VARYINGS_NEED_COLOR
        #endif
        
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SPRITEFORWARD
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
            // Includes
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
            struct Attributes
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 positionOS : POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 normalOS : NORMAL;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 tangentOS : TANGENT;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 uv0 : TEXCOORD0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 color : COLOR;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
            #endif
        };
        struct Varyings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 positionWS;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 texCoord0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 color;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 TangentSpaceNormal;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 WorldSpacePosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 uv0;
            #endif
        };
        struct VertexDescriptionInputs
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 ObjectSpaceNormal;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 ObjectSpaceTangent;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 ObjectSpacePosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 TimeParameters;
            #endif
        };
        struct PackedVaryings
        {
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 positionCS : SV_POSITION;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float3 interp0 : INTERP0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 interp1 : INTERP1;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             float4 interp2 : INTERP2;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
            #endif
        };
        
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyzw =  input.texCoord0;
            output.interp2.xyzw =  input.color;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            output.color = input.interp2.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        #endif
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float _Speed;
        float _Tilling;
        float _Amplitude;
        float _YVariation;
        float _GroundSpeedLimit;
        float4 _MainTex_TexelSize;
        float4 _MainTex_ST;
        float _LockingMovementReduction;
        float _LockingYUvsPosition;
        float _LockingTreshold;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
            // Graph Includes
            // GraphIncludes: <None>
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Graph Functions
            
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Cosine_float(float In, out float Out)
        {
            Out = cos(In);
        }
        
        void Unity_Distance_float(float A, float B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        void Unity_Lerp_float3(float3 A, float3 B, float3 T, out float3 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_2f9d2b3a5d124d9e98433a5997953d56_Out_0 = _Speed;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_7a03097596a04c6fbaf7ac71a3a87967_Out_2;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_2f9d2b3a5d124d9e98433a5997953d56_Out_0, _Multiply_7a03097596a04c6fbaf7ac71a3a87967_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Split_7129975c62bc4203840744f4faa34665_R_1 = IN.ObjectSpacePosition[0];
            float _Split_7129975c62bc4203840744f4faa34665_G_2 = IN.ObjectSpacePosition[1];
            float _Split_7129975c62bc4203840744f4faa34665_B_3 = IN.ObjectSpacePosition[2];
            float _Split_7129975c62bc4203840744f4faa34665_A_4 = 0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_ac7d909912204819babee1a8fc569ca1_Out_0 = _Tilling;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_6086e5426cd74fb0a516e28465b0eb11_Out_2;
            Unity_Multiply_float_float(_Split_7129975c62bc4203840744f4faa34665_G_2, _Property_ac7d909912204819babee1a8fc569ca1_Out_0, _Multiply_6086e5426cd74fb0a516e28465b0eb11_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Add_d9603b01777f4b5fadb1eb750f87eac1_Out_2;
            Unity_Add_float(_Multiply_7a03097596a04c6fbaf7ac71a3a87967_Out_2, _Multiply_6086e5426cd74fb0a516e28465b0eb11_Out_2, _Add_d9603b01777f4b5fadb1eb750f87eac1_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Cosine_5a4679fd61034839827e62dcb7ca30d8_Out_1;
            Unity_Cosine_float(_Add_d9603b01777f4b5fadb1eb750f87eac1_Out_2, _Cosine_5a4679fd61034839827e62dcb7ca30d8_Out_1);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_511992636cc64eeead7a6646c9cd16c6_Out_0 = _Amplitude;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_d5afbd154c484b53a6e669eec85bcd4a_Out_2;
            Unity_Multiply_float_float(_Cosine_5a4679fd61034839827e62dcb7ca30d8_Out_1, _Property_511992636cc64eeead7a6646c9cd16c6_Out_0, _Multiply_d5afbd154c484b53a6e669eec85bcd4a_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Add_cc682e4e900d48f997b4ef5ce6b4c083_Out_2;
            Unity_Add_float(_Multiply_d5afbd154c484b53a6e669eec85bcd4a_Out_2, _Split_7129975c62bc4203840744f4faa34665_R_1, _Add_cc682e4e900d48f997b4ef5ce6b4c083_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_f37a64f636b5429e9e9af6f790e18e1c_Out_0 = _YVariation;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_cc0b44b5a4ad4555823ef225c9e176d9_Out_2;
            Unity_Multiply_float_float(_Multiply_7a03097596a04c6fbaf7ac71a3a87967_Out_2, _Property_f37a64f636b5429e9e9af6f790e18e1c_Out_0, _Multiply_cc0b44b5a4ad4555823ef225c9e176d9_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_fbafb3410e6b4ab6bc7212393f6c5ac9_Out_0 = _Tilling;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_2d14785807f0423c9c78d3a9afaa91a3_Out_2;
            Unity_Multiply_float_float(_Property_fbafb3410e6b4ab6bc7212393f6c5ac9_Out_0, _Property_f37a64f636b5429e9e9af6f790e18e1c_Out_0, _Multiply_2d14785807f0423c9c78d3a9afaa91a3_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_a88a41a8a1474e4ea1649a4f1391243e_Out_2;
            Unity_Multiply_float_float(_Split_7129975c62bc4203840744f4faa34665_R_1, _Multiply_2d14785807f0423c9c78d3a9afaa91a3_Out_2, _Multiply_a88a41a8a1474e4ea1649a4f1391243e_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Add_665434cb9c6f4dfa8970ed70c7fb5c19_Out_2;
            Unity_Add_float(_Multiply_cc0b44b5a4ad4555823ef225c9e176d9_Out_2, _Multiply_a88a41a8a1474e4ea1649a4f1391243e_Out_2, _Add_665434cb9c6f4dfa8970ed70c7fb5c19_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Cosine_a87a978d261943fca172c387b4f4b04a_Out_1;
            Unity_Cosine_float(_Add_665434cb9c6f4dfa8970ed70c7fb5c19_Out_2, _Cosine_a87a978d261943fca172c387b4f4b04a_Out_1);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_76423ae27d8c4db5805619e50d47777e_Out_0 = _Amplitude;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_a18a8c6099ec41eebc85fe3fc0b76840_Out_2;
            Unity_Multiply_float_float(_Cosine_a87a978d261943fca172c387b4f4b04a_Out_1, _Property_76423ae27d8c4db5805619e50d47777e_Out_0, _Multiply_a18a8c6099ec41eebc85fe3fc0b76840_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Add_746bf4335d654588935b51e29aae56c2_Out_2;
            Unity_Add_float(_Multiply_a18a8c6099ec41eebc85fe3fc0b76840_Out_2, _Split_7129975c62bc4203840744f4faa34665_G_2, _Add_746bf4335d654588935b51e29aae56c2_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Vector3_0ad052ff4f2a468798f4efcedf596053_Out_0 = float3(_Add_cc682e4e900d48f997b4ef5ce6b4c083_Out_2, _Add_746bf4335d654588935b51e29aae56c2_Out_2, _Split_7129975c62bc4203840744f4faa34665_B_3);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Split_eab0210b1fb548b989a1a99c49265331_R_1 = 0;
            float _Split_eab0210b1fb548b989a1a99c49265331_G_2 = 0;
            float _Split_eab0210b1fb548b989a1a99c49265331_B_3 = 0;
            float _Split_eab0210b1fb548b989a1a99c49265331_A_4 = 0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_b0c8f86d3f39489685df628a23cd0169_Out_0 = _LockingYUvsPosition;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Distance_012097377ae8486392a7434139dc9cd7_Out_2;
            Unity_Distance_float(_Split_eab0210b1fb548b989a1a99c49265331_G_2, _Property_b0c8f86d3f39489685df628a23cd0169_Out_0, _Distance_012097377ae8486392a7434139dc9cd7_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_1f6079c1ae98418da0c4eea4cb31b1d4_Out_0 = _LockingTreshold;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Divide_0c657c0956eb4910b755b8672d4987af_Out_2;
            Unity_Divide_float(_Distance_012097377ae8486392a7434139dc9cd7_Out_2, _Property_1f6079c1ae98418da0c4eea4cb31b1d4_Out_0, _Divide_0c657c0956eb4910b755b8672d4987af_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _OneMinus_3b199092de3248239799f933efbb85d5_Out_1;
            Unity_OneMinus_float(_Divide_0c657c0956eb4910b755b8672d4987af_Out_2, _OneMinus_3b199092de3248239799f933efbb85d5_Out_1);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Property_7acf19eeea1d44ea99d7853ebceb3935_Out_0 = _LockingMovementReduction;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Multiply_987883e3416f481bac7f359ef37a3225_Out_2;
            Unity_Multiply_float_float(_OneMinus_3b199092de3248239799f933efbb85d5_Out_1, _Property_7acf19eeea1d44ea99d7853ebceb3935_Out_0, _Multiply_987883e3416f481bac7f359ef37a3225_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Clamp_cebcfa880551453ca122e777cde3b476_Out_3;
            Unity_Clamp_float(_Multiply_987883e3416f481bac7f359ef37a3225_Out_2, 0, 1, _Clamp_cebcfa880551453ca122e777cde3b476_Out_3);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Vector3_57c97d1a28a64fbf8fbc73c504698939_Out_0 = float3(_Clamp_cebcfa880551453ca122e777cde3b476_Out_3, _Clamp_cebcfa880551453ca122e777cde3b476_Out_3, _Clamp_cebcfa880551453ca122e777cde3b476_Out_3);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float3 _Lerp_26c3d4e649bf43709ed9aeea6401a5ea_Out_3;
            Unity_Lerp_float3(_Vector3_0ad052ff4f2a468798f4efcedf596053_Out_0, IN.ObjectSpacePosition, _Vector3_57c97d1a28a64fbf8fbc73c504698939_Out_0, _Lerp_26c3d4e649bf43709ed9aeea6401a5ea_Out_3);
            #endif
            description.Position = _Lerp_26c3d4e649bf43709ed9aeea6401a5ea_Out_3;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
            #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
            float3 NormalTS;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Split_e11a52eeb5974259ae3fdc03f16c88b6_R_1 = SHADERGRAPH_OBJECT_POSITION[0];
            float _Split_e11a52eeb5974259ae3fdc03f16c88b6_G_2 = SHADERGRAPH_OBJECT_POSITION[1];
            float _Split_e11a52eeb5974259ae3fdc03f16c88b6_B_3 = SHADERGRAPH_OBJECT_POSITION[2];
            float _Split_e11a52eeb5974259ae3fdc03f16c88b6_A_4 = 0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Split_4a9ab7976a2a42319d519c3a206784c5_R_1 = IN.WorldSpacePosition[0];
            float _Split_4a9ab7976a2a42319d519c3a206784c5_G_2 = IN.WorldSpacePosition[1];
            float _Split_4a9ab7976a2a42319d519c3a206784c5_B_3 = IN.WorldSpacePosition[2];
            float _Split_4a9ab7976a2a42319d519c3a206784c5_A_4 = 0;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float _Subtract_fa4394f9d8314594b34b4d7d0ea9501a_Out_2;
            Unity_Subtract_float(_Split_e11a52eeb5974259ae3fdc03f16c88b6_R_1, _Split_4a9ab7976a2a42319d519c3a206784c5_R_1, _Subtract_fa4394f9d8314594b34b4d7d0ea9501a_Out_2);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            UnityTexture2D _Property_75679be3672b4d37ac7e847641aa839e_Out_0 = UnityBuildTexture2DStruct(_MainTex);
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            float4 _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_RGBA_0 = SAMPLE_TEXTURE2D(_Property_75679be3672b4d37ac7e847641aa839e_Out_0.tex, _Property_75679be3672b4d37ac7e847641aa839e_Out_0.samplerstate, _Property_75679be3672b4d37ac7e847641aa839e_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_R_4 = _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_RGBA_0.r;
            float _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_G_5 = _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_RGBA_0.g;
            float _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_B_6 = _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_RGBA_0.b;
            float _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_A_7 = _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_RGBA_0.a;
            #endif
            #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
            #if defined(_COLORPREVIEW)
            float4 _ColorPreview_588fe1233ab04f95b91a2a1f678d94ff_Out_0 = (_Subtract_fa4394f9d8314594b34b4d7d0ea9501a_Out_2.xxxx);
            #else
            float4 _ColorPreview_588fe1233ab04f95b91a2a1f678d94ff_Out_0 = _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_RGBA_0;
            #endif
            #endif
            surface.BaseColor = (_ColorPreview_588fe1233ab04f95b91a2a1f678d94ff_Out_0.xyz);
            surface.Alpha = _SampleTexture2D_3d32264bdfe24c83a6f76a5fcd2342c5_A_7;
            surface.NormalTS = IN.TangentSpaceNormal;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpaceNormal =                          input.normalOS;
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpaceTangent =                         input.tangentOS.xyz;
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.ObjectSpacePosition =                        input.positionOS;
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.TimeParameters =                             _TimeParameters.xyz;
        #endif
        
        
            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            
        
        
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.TangentSpaceNormal =                         float3(0.0f, 0.0f, 1.0f);
        #endif
        
        
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.WorldSpacePosition =                         input.positionWS;
        #endif
        
        #if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
        output.uv0 =                                        input.texCoord0;
        #endif
        
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
            return output;
        }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteForwardPass.hlsl"
        
            ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}