// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32892,y:32696,varname:node_3138,prsc:2|emission-931-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:31613,y:32454,ptovrint:False,ptlb:ColorA,ptin:_ColorA,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.2591912,c2:0.6911765,c3:0.3694218,c4:1;n:type:ShaderForge.SFN_Color,id:209,x:31613,y:32623,ptovrint:False,ptlb:ColorB,ptin:_ColorB,varname:node_209,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.1321367,c2:0.3823529,c3:0.2684614,c4:1;n:type:ShaderForge.SFN_Lerp,id:6855,x:32017,y:32673,varname:node_6855,prsc:2|A-209-RGB,B-7241-RGB,T-4756-R;n:type:ShaderForge.SFN_Tex2d,id:4756,x:31613,y:32800,ptovrint:False,ptlb:Noise,ptin:_Noise,varname:node_4756,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-2889-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:972,x:30817,y:32765,varname:node_972,prsc:2;n:type:ShaderForge.SFN_Append,id:2889,x:31453,y:32800,varname:node_2889,prsc:2|A-9573-R,B-9573-G;n:type:ShaderForge.SFN_Multiply,id:1639,x:31113,y:32780,varname:node_1639,prsc:2|A-972-XYZ,B-6939-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6939,x:30817,y:32926,ptovrint:False,ptlb:Scale,ptin:_Scale,varname:node_6939,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ComponentMask,id:9573,x:31278,y:32780,varname:node_9573,prsc:2,cc1:0,cc2:2,cc3:-1,cc4:-1|IN-1639-OUT;n:type:ShaderForge.SFN_LightVector,id:7,x:30893,y:33185,varname:node_7,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:2783,x:30893,y:33301,prsc:2,pt:False;n:type:ShaderForge.SFN_Dot,id:7502,x:31054,y:33234,varname:node_7502,prsc:2,dt:0|A-7-OUT,B-2783-OUT;n:type:ShaderForge.SFN_Smoothstep,id:7049,x:31688,y:33139,varname:node_7049,prsc:2|A-8239-OUT,B-8239-OUT,V-9020-OUT;n:type:ShaderForge.SFN_Slider,id:1188,x:31223,y:33515,ptovrint:False,ptlb:ShadowStep,ptin:_ShadowStep,varname:node_7334,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:-0.4358974,max:1;n:type:ShaderForge.SFN_Slider,id:8239,x:31195,y:33084,ptovrint:False,ptlb:LightStep,ptin:_LightStep,varname:node_3789,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0.6153846,max:1;n:type:ShaderForge.SFN_Smoothstep,id:2706,x:31678,y:33354,varname:node_2706,prsc:2|A-1188-OUT,B-1188-OUT,V-9020-OUT;n:type:ShaderForge.SFN_Clamp01,id:6614,x:31843,y:33354,varname:node_6614,prsc:2|IN-2706-OUT;n:type:ShaderForge.SFN_Clamp01,id:6591,x:31843,y:33139,varname:node_6591,prsc:2|IN-7049-OUT;n:type:ShaderForge.SFN_Lerp,id:9233,x:32179,y:33011,varname:node_9233,prsc:2|A-6855-OUT,B-1414-RGB,T-826-OUT;n:type:ShaderForge.SFN_Lerp,id:228,x:32425,y:32986,varname:node_228,prsc:2|A-9233-OUT,B-7329-RGB,T-8025-OUT;n:type:ShaderForge.SFN_Multiply,id:9020,x:31295,y:33213,varname:node_9020,prsc:2|A-7502-OUT,B-9639-OUT;n:type:ShaderForge.SFN_LightAttenuation,id:9639,x:31065,y:33377,varname:node_9639,prsc:2;n:type:ShaderForge.SFN_Multiply,id:826,x:32005,y:33082,varname:node_826,prsc:2|A-1102-OUT,B-6591-OUT;n:type:ShaderForge.SFN_Multiply,id:8025,x:32240,y:33290,varname:node_8025,prsc:2|A-9580-OUT,B-6192-OUT;n:type:ShaderForge.SFN_Slider,id:1102,x:31609,y:33044,ptovrint:False,ptlb:LightValue,ptin:_LightValue,varname:node_2893,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Slider,id:9580,x:31895,y:33505,ptovrint:False,ptlb:ShadowValue,ptin:_ShadowValue,varname:node_7402,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_OneMinus,id:6192,x:32018,y:33320,varname:node_6192,prsc:2|IN-6614-OUT;n:type:ShaderForge.SFN_Color,id:1414,x:31904,y:32905,ptovrint:False,ptlb:LightColor,ptin:_LightColor,varname:node_1414,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Color,id:7329,x:32303,y:32800,ptovrint:False,ptlb:ShadowColor,ptin:_ShadowColor,varname:node_7329,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_VertexColor,id:6180,x:32234,y:32570,varname:node_6180,prsc:2;n:type:ShaderForge.SFN_Lerp,id:931,x:32631,y:32668,varname:node_931,prsc:2|A-6314-OUT,B-228-OUT,T-6180-R;n:type:ShaderForge.SFN_Lerp,id:8328,x:32164,y:32421,varname:node_8328,prsc:2|A-6855-OUT,B-1414-RGB,T-1102-OUT;n:type:ShaderForge.SFN_Lerp,id:6314,x:32458,y:32388,varname:node_6314,prsc:2|A-8328-OUT,B-3327-RGB,T-6180-B;n:type:ShaderForge.SFN_Color,id:3327,x:32196,y:32252,ptovrint:False,ptlb:Tipcolor,ptin:_Tipcolor,varname:node_3327,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.05147058,c3:0.05147058,c4:1;proporder:7241-209-4756-6939-1188-8239-1102-9580-1414-7329-3327;pass:END;sub:END;*/

Shader "Shader Forge/S_Ground" {
    Properties {
        _ColorA ("ColorA", Color) = (0.2591912,0.6911765,0.3694218,1)
        _ColorB ("ColorB", Color) = (0.1321367,0.3823529,0.2684614,1)
        _Noise ("Noise", 2D) = "white" {}
        _Scale ("Scale", Float ) = 0
        _ShadowStep ("ShadowStep", Range(-1, 1)) = -0.4358974
        _LightStep ("LightStep", Range(-1, 1)) = 0.6153846
        _LightValue ("LightValue", Range(0, 1)) = 1
        _ShadowValue ("ShadowValue", Range(0, 1)) = 1
        _LightColor ("LightColor", Color) = (0.5,0.5,0.5,1)
        _ShadowColor ("ShadowColor", Color) = (0.5,0.5,0.5,1)
        _Tipcolor ("Tipcolor", Color) = (1,0.05147058,0.05147058,1)
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _ColorA;
            uniform float4 _ColorB;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float _Scale;
            uniform float _ShadowStep;
            uniform float _LightStep;
            uniform float _LightValue;
            uniform float _ShadowValue;
            uniform float4 _LightColor;
            uniform float4 _ShadowColor;
            uniform float4 _Tipcolor;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(2,3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
////// Emissive:
                float2 node_9573 = (i.posWorld.rgb*_Scale).rb;
                float2 node_2889 = float2(node_9573.r,node_9573.g);
                float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(node_2889, _Noise));
                float3 node_6855 = lerp(_ColorB.rgb,_ColorA.rgb,_Noise_var.r);
                float node_9020 = (dot(lightDirection,i.normalDir)*attenuation);
                float3 emissive = lerp(lerp(lerp(node_6855,_LightColor.rgb,_LightValue),_Tipcolor.rgb,i.vertexColor.b),lerp(lerp(node_6855,_LightColor.rgb,(_LightValue*saturate(smoothstep( _LightStep, _LightStep, node_9020 )))),_ShadowColor.rgb,(_ShadowValue*(1.0 - saturate(smoothstep( _ShadowStep, _ShadowStep, node_9020 ))))),i.vertexColor.r);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _ColorA;
            uniform float4 _ColorB;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float _Scale;
            uniform float _ShadowStep;
            uniform float _LightStep;
            uniform float _LightValue;
            uniform float _ShadowValue;
            uniform float4 _LightColor;
            uniform float4 _ShadowColor;
            uniform float4 _Tipcolor;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(2,3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 finalColor = 0;
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
