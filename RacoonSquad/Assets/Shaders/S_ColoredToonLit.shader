// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32719,y:32712,varname:node_3138,prsc:2|emission-126-OUT;n:type:ShaderForge.SFN_LightVector,id:9700,x:30942,y:32986,varname:node_9700,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:1120,x:30942,y:33102,prsc:2,pt:False;n:type:ShaderForge.SFN_Dot,id:9209,x:31103,y:33035,varname:node_9209,prsc:2,dt:0|A-9700-OUT,B-1120-OUT;n:type:ShaderForge.SFN_Smoothstep,id:4237,x:31737,y:32940,varname:node_4237,prsc:2|A-9765-OUT,B-9765-OUT,V-2167-OUT;n:type:ShaderForge.SFN_Slider,id:9859,x:31272,y:33316,ptovrint:False,ptlb:ShadowStep,ptin:_ShadowStep,varname:node_7334,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:-0.4358974,max:1;n:type:ShaderForge.SFN_Slider,id:9765,x:31244,y:32885,ptovrint:False,ptlb:LightStep,ptin:_LightStep,varname:node_3789,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0.6153846,max:1;n:type:ShaderForge.SFN_Smoothstep,id:7237,x:31727,y:33155,varname:node_7237,prsc:2|A-9859-OUT,B-9859-OUT,V-2167-OUT;n:type:ShaderForge.SFN_Clamp01,id:4534,x:31892,y:33155,varname:node_4534,prsc:2|IN-7237-OUT;n:type:ShaderForge.SFN_Clamp01,id:4939,x:31892,y:32940,varname:node_4939,prsc:2|IN-4237-OUT;n:type:ShaderForge.SFN_Lerp,id:6299,x:32228,y:32812,varname:node_6299,prsc:2|A-5504-RGB,B-6494-RGB,T-7093-OUT;n:type:ShaderForge.SFN_Color,id:6494,x:31861,y:32518,ptovrint:False,ptlb:LightColor,ptin:_LightColor,varname:node_7382,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Lerp,id:126,x:32474,y:32787,varname:node_126,prsc:2|A-6299-OUT,B-2414-RGB,T-6392-OUT;n:type:ShaderForge.SFN_Color,id:2414,x:32098,y:32518,ptovrint:False,ptlb:ShadowColor,ptin:_ShadowColor,varname:node_4424,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Multiply,id:2167,x:31344,y:33014,varname:node_2167,prsc:2|A-9209-OUT,B-8235-OUT;n:type:ShaderForge.SFN_LightAttenuation,id:8235,x:31114,y:33178,varname:node_8235,prsc:2;n:type:ShaderForge.SFN_Multiply,id:7093,x:32054,y:32883,varname:node_7093,prsc:2|A-6353-OUT,B-4939-OUT;n:type:ShaderForge.SFN_Multiply,id:6392,x:32289,y:33091,varname:node_6392,prsc:2|A-2797-OUT,B-1353-OUT;n:type:ShaderForge.SFN_Slider,id:6353,x:31658,y:32845,ptovrint:False,ptlb:LightValue,ptin:_LightValue,varname:node_2893,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Slider,id:2797,x:31944,y:33306,ptovrint:False,ptlb:ShadowValue,ptin:_ShadowValue,varname:node_7402,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_OneMinus,id:1353,x:32067,y:33121,varname:node_1353,prsc:2|IN-4534-OUT;n:type:ShaderForge.SFN_Color,id:5504,x:31511,y:32625,ptovrint:False,ptlb:BaseColor,ptin:_BaseColor,varname:node_5504,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;proporder:9859-9765-6494-2414-6353-2797-5504;pass:END;sub:END;*/

Shader "Shader Forge/S_ColoredToonLit" {
    Properties {
        _ShadowStep ("ShadowStep", Range(-1, 1)) = -0.4358974
        _LightStep ("LightStep", Range(-1, 1)) = 0.6153846
        _LightColor ("LightColor", Color) = (1,1,1,1)
        _ShadowColor ("ShadowColor", Color) = (0,0,0,1)
        _LightValue ("LightValue", Range(0, 1)) = 1
        _ShadowValue ("ShadowValue", Range(0, 1)) = 1
        _BaseColor ("BaseColor", Color) = (0.5,0.5,0.5,1)
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
            uniform float _ShadowStep;
            uniform float _LightStep;
            uniform float4 _LightColor;
            uniform float4 _ShadowColor;
            uniform float _LightValue;
            uniform float _ShadowValue;
            uniform float4 _BaseColor;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                LIGHTING_COORDS(2,3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
////// Emissive:
                float node_2167 = (dot(lightDirection,i.normalDir)*attenuation);
                float3 emissive = lerp(lerp(_BaseColor.rgb,_LightColor.rgb,(_LightValue*saturate(smoothstep( _LightStep, _LightStep, node_2167 )))),_ShadowColor.rgb,(_ShadowValue*(1.0 - saturate(smoothstep( _ShadowStep, _ShadowStep, node_2167 )))));
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
            uniform float _ShadowStep;
            uniform float _LightStep;
            uniform float4 _LightColor;
            uniform float4 _ShadowColor;
            uniform float _LightValue;
            uniform float _ShadowValue;
            uniform float4 _BaseColor;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                LIGHTING_COORDS(2,3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 finalColor = 0;
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
