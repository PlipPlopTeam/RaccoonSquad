// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32862,y:32710,varname:node_3138,prsc:2|emission-8691-OUT,clip-1822-A;n:type:ShaderForge.SFN_Tex2d,id:1822,x:32052,y:32837,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:node_1822,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:de13ca2ffb07de64982c857b06aadd7d,ntxv:0,isnm:False;n:type:ShaderForge.SFN_TexCoord,id:1870,x:31580,y:33049,varname:node_1870,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Slider,id:1262,x:31434,y:33298,ptovrint:False,ptlb:Fill,ptin:_Fill,varname:node_1262,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.485976,max:1;n:type:ShaderForge.SFN_Add,id:7223,x:32027,y:33162,varname:node_7223,prsc:2|A-1870-U,B-5019-OUT;n:type:ShaderForge.SFN_Color,id:4138,x:32052,y:32640,ptovrint:False,ptlb:ColorA,ptin:_ColorA,varname:node_4138,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_RemapRange,id:5019,x:31814,y:33274,varname:node_5019,prsc:2,frmn:0,frmx:1,tomn:-0.51,tomx:0.51|IN-1262-OUT;n:type:ShaderForge.SFN_Lerp,id:8691,x:32550,y:32731,varname:node_8691,prsc:2|A-5127-RGB,B-4138-RGB,T-8272-OUT;n:type:ShaderForge.SFN_Clamp01,id:8272,x:32403,y:32880,varname:node_8272,prsc:2|IN-3097-OUT;n:type:ShaderForge.SFN_Color,id:5127,x:32127,y:32411,ptovrint:False,ptlb:ColorB,ptin:_ColorB,varname:node_5127,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.9716981,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Step,id:3097,x:32256,y:33162,varname:node_3097,prsc:2|A-7223-OUT,B-8149-OUT;n:type:ShaderForge.SFN_Vector1,id:8149,x:32045,y:33329,varname:node_8149,prsc:2,v1:0.5;proporder:1822-1262-4138-5127;pass:END;sub:END;*/

Shader "Shader Forge/S_SpriteUnlitFill" {
    Properties {
        _Mask ("Mask", 2D) = "white" {}
        _Fill ("Fill", Range(0, 1)) = 0.485976
        _ColorA ("ColorA", Color) = (0.5,0.5,0.5,1)
        _ColorB ("ColorB", Color) = (0.9716981,0,0,1)
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
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform float _Fill;
            uniform float4 _ColorA;
            uniform float4 _ColorB;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                clip(_Mask_var.a - 0.5);
////// Lighting:
////// Emissive:
                float node_7223 = (i.uv0.r+(_Fill*1.02+-0.51));
                float3 emissive = lerp(_ColorB.rgb,_ColorA.rgb,saturate(step(node_7223,0.5)));
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
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                clip(_Mask_var.a - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
