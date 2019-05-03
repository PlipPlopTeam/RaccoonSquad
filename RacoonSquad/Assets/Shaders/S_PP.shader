// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32719,y:32712,varname:node_3138,prsc:2|emission-61-RGB;n:type:ShaderForge.SFN_Tex2d,id:61,x:32183,y:32786,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_61,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-3983-OUT;n:type:ShaderForge.SFN_TexCoord,id:1671,x:31030,y:32771,varname:node_1671,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:5324,x:31251,y:32781,varname:node_5324,prsc:2|A-1671-UVOUT,B-1193-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1193,x:31138,y:33035,ptovrint:False,ptlb:Res,ptin:_Res,varname:node_1193,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:128;n:type:ShaderForge.SFN_Floor,id:9243,x:31433,y:32781,varname:node_9243,prsc:2|IN-5324-OUT;n:type:ShaderForge.SFN_Divide,id:8929,x:31606,y:32776,varname:node_8929,prsc:2|A-9243-OUT,B-1193-OUT;n:type:ShaderForge.SFN_Lerp,id:3983,x:31944,y:32822,varname:node_3983,prsc:2|A-1671-UVOUT,B-8929-OUT,T-2064-OUT;n:type:ShaderForge.SFN_Slider,id:2064,x:31606,y:33058,ptovrint:False,ptlb:Pixelate,ptin:_Pixelate,varname:node_2064,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;proporder:61-1193-2064;pass:END;sub:END;*/

Shader "Shader Forge/S_PP" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _Res ("Res", Float ) = 128
        _Pixelate ("Pixelate", Range(0, 1)) = 0
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
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _Res;
            uniform float _Pixelate;
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
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float2 node_3983 = lerp(i.uv0,(floor((i.uv0*_Res))/_Res),_Pixelate);
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_3983, _MainTex));
                float3 emissive = _MainTex_var.rgb;
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
