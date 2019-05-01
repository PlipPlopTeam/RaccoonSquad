// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32719,y:32712,varname:node_3138,prsc:2|emission-4540-RGB,clip-5842-OUT;n:type:ShaderForge.SFN_VertexColor,id:4540,x:32186,y:32681,varname:node_4540,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:548,x:31579,y:32992,ptovrint:False,ptlb:DissolveTexture,ptin:_DissolveTexture,varname:node_548,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:a6ed99041c8b48f4a8b94311cfba2c95,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Step,id:5842,x:32281,y:33016,varname:node_5842,prsc:2|A-6722-OUT,B-8435-OUT;n:type:ShaderForge.SFN_Vector1,id:8435,x:32168,y:33189,varname:node_8435,prsc:2,v1:0.5;n:type:ShaderForge.SFN_RemapRange,id:5865,x:31711,y:33177,varname:node_5865,prsc:2,frmn:0,frmx:1,tomn:-0.49,tomx:0.51|IN-4513-Z;n:type:ShaderForge.SFN_Add,id:6722,x:31918,y:33047,varname:node_6722,prsc:2|A-548-R,B-5865-OUT;n:type:ShaderForge.SFN_TexCoord,id:4513,x:31269,y:33020,varname:node_4513,prsc:2,uv:0,uaff:True;proporder:548;pass:END;sub:END;*/

Shader "Shader Forge/S_DissolveParticle" {
    Properties {
        _DissolveTexture ("DissolveTexture", 2D) = "white" {}
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
            uniform sampler2D _DissolveTexture; uniform float4 _DissolveTexture_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float4 _DissolveTexture_var = tex2D(_DissolveTexture,TRANSFORM_TEX(i.uv0, _DissolveTexture));
                clip(step((_DissolveTexture_var.r+(i.uv0.b*1.0+-0.49)),0.5) - 0.5);
////// Lighting:
////// Emissive:
                float3 emissive = i.vertexColor.rgb;
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
            uniform sampler2D _DissolveTexture; uniform float4 _DissolveTexture_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float4 uv0 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float4 _DissolveTexture_var = tex2D(_DissolveTexture,TRANSFORM_TEX(i.uv0, _DissolveTexture));
                clip(step((_DissolveTexture_var.r+(i.uv0.b*1.0+-0.49)),0.5) - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
