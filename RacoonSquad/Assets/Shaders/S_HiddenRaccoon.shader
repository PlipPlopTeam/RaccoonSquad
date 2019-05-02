// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:3,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32719,y:32712,varname:node_3138,prsc:2|emission-7241-RGB,clip-2995-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32255,y:32816,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_Slider,id:2995,x:32150,y:32996,ptovrint:False,ptlb:Alpha,ptin:_Alpha,varname:node_2995,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.3162393,max:1;proporder:7241-2995;pass:END;sub:END;*/

Shader "Shader Forge/S_HiddenRaccoon" {
    Properties {
        _Color ("Color", Color) = (0.07843138,0.3921569,0.7843137,1)
        _Alpha ("Alpha", Range(0, 1)) = 0.3162393
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
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
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            // Dithering function, to use with scene UVs (screen pixel coords)
            // 4x4 Bayer matrix, based on https://en.wikipedia.org/wiki/Ordered_dithering
            float BinaryDither4x4( float value, float2 sceneUVs ) {
                float4x4 mtx = float4x4(
                    float4( 1,  9,  3, 11 )/17.0,
                    float4( 13, 5, 15,  7 )/17.0,
                    float4( 4, 12,  2, 10 )/17.0,
                    float4( 16, 8, 14,  6 )/17.0
                );
                float2 px = floor(_ScreenParams.xy * sceneUVs);
                int xSmp = fmod(px.x,4);
                int ySmp = fmod(px.y,4);
                float4 xVec = 1-saturate(abs(float4(0,1,2,3) - xSmp));
                float4 yVec = 1-saturate(abs(float4(0,1,2,3) - ySmp));
                float4 pxMult = float4( dot(mtx[0],yVec), dot(mtx[1],yVec), dot(mtx[2],yVec), dot(mtx[3],yVec) );
                return round(value + dot(pxMult, xVec));
            }
            uniform float4 _Color;
            uniform float _Alpha;
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 projPos : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float2 sceneUVs = (i.projPos.xy / i.projPos.w);
                clip( BinaryDither4x4(_Alpha - 1.5, sceneUVs) );
////// Lighting:
////// Emissive:
                float3 emissive = _Color.rgb;
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Back
            
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
            // Dithering function, to use with scene UVs (screen pixel coords)
            // 4x4 Bayer matrix, based on https://en.wikipedia.org/wiki/Ordered_dithering
            float BinaryDither4x4( float value, float2 sceneUVs ) {
                float4x4 mtx = float4x4(
                    float4( 1,  9,  3, 11 )/17.0,
                    float4( 13, 5, 15,  7 )/17.0,
                    float4( 4, 12,  2, 10 )/17.0,
                    float4( 16, 8, 14,  6 )/17.0
                );
                float2 px = floor(_ScreenParams.xy * sceneUVs);
                int xSmp = fmod(px.x,4);
                int ySmp = fmod(px.y,4);
                float4 xVec = 1-saturate(abs(float4(0,1,2,3) - xSmp));
                float4 yVec = 1-saturate(abs(float4(0,1,2,3) - ySmp));
                float4 pxMult = float4( dot(mtx[0],yVec), dot(mtx[1],yVec), dot(mtx[2],yVec), dot(mtx[3],yVec) );
                return round(value + dot(pxMult, xVec));
            }
            uniform float _Alpha;
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float4 projPos : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float2 sceneUVs = (i.projPos.xy / i.projPos.w);
                clip( BinaryDither4x4(_Alpha - 1.5, sceneUVs) );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
