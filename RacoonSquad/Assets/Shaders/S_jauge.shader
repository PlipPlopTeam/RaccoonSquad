// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32696,y:32587,varname:node_3138,prsc:2|emission-8474-OUT;n:type:ShaderForge.SFN_Slider,id:9745,x:31628,y:32920,ptovrint:False,ptlb:Value,ptin:_Value,varname:node_9745,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_TexCoord,id:8649,x:31954,y:32778,varname:node_8649,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:6106,x:32128,y:32788,varname:node_6106,prsc:2|A-8649-U,B-8409-OUT;n:type:ShaderForge.SFN_Step,id:4570,x:32315,y:32788,varname:node_4570,prsc:2|A-6106-OUT,B-6792-OUT;n:type:ShaderForge.SFN_Vector1,id:6792,x:31954,y:33082,varname:node_6792,prsc:2,v1:0.5;n:type:ShaderForge.SFN_RemapRange,id:8409,x:31954,y:32920,varname:node_8409,prsc:2,frmn:0,frmx:1,tomn:0.51,tomx:-0.51|IN-9745-OUT;n:type:ShaderForge.SFN_Lerp,id:8474,x:32517,y:32682,varname:node_8474,prsc:2|A-100-RGB,B-504-RGB,T-4570-OUT;n:type:ShaderForge.SFN_Color,id:100,x:32283,y:32438,ptovrint:False,ptlb:ColorA,ptin:_ColorA,varname:node_100,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.6764706,c2:0.6764706,c3:0.6764706,c4:1;n:type:ShaderForge.SFN_Color,id:504,x:32283,y:32613,ptovrint:False,ptlb:ColorB,ptin:_ColorB,varname:node_504,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.212154,c2:0.8014706,c3:0.4113024,c4:1;proporder:9745-100-504;pass:END;sub:END;*/

Shader "Shader Forge/S_jauge" {
    Properties {
        _Value ("Value", Range(0, 1)) = 0
        _ColorA ("ColorA", Color) = (0.6764706,0.6764706,0.6764706,1)
        _ColorB ("ColorB", Color) = (0.212154,0.8014706,0.4113024,1)
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
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float _Value;
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
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float3 emissive = lerp(_ColorA.rgb,_ColorB.rgb,step((i.uv0.r+(_Value*-1.02+0.51)),0.5));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
