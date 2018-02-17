// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32719,y:32712,varname:node_3138,prsc:2|emission-3448-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:31703,y:32963,ptovrint:False,ptlb:ColorBottom,ptin:_ColorBottom,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_FragmentPosition,id:6522,x:31553,y:33143,varname:node_6522,prsc:2;n:type:ShaderForge.SFN_RemapRange,id:7757,x:31716,y:33143,varname:node_7757,prsc:2,frmn:-1,frmx:1,tomn:-0.5,tomx:1|IN-6522-Y;n:type:ShaderForge.SFN_Color,id:7256,x:31703,y:32809,ptovrint:False,ptlb:ColorTop,ptin:_ColorTop,varname:node_7256,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0.931035,c4:1;n:type:ShaderForge.SFN_Lerp,id:1372,x:31906,y:32809,varname:node_1372,prsc:2|A-7256-RGB,B-7241-RGB,T-7757-OUT;n:type:ShaderForge.SFN_Lerp,id:2399,x:32321,y:32927,varname:node_2399,prsc:2|A-435-OUT,B-3811-OUT,T-7757-OUT;n:type:ShaderForge.SFN_Vector1,id:3811,x:32110,y:32937,varname:node_3811,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:435,x:32110,y:32885,varname:node_435,prsc:2,v1:2;n:type:ShaderForge.SFN_Multiply,id:3448,x:32487,y:32813,varname:node_3448,prsc:2|A-1372-OUT,B-2399-OUT;proporder:7241-7256;pass:END;sub:END;*/

Shader "Shader Forge/Bloom" {
    Properties {
        _ColorBottom ("ColorBottom", Color) = (0.07843138,0.3921569,0.7843137,1)
        _ColorTop ("ColorTop", Color) = (1,0,0.931035,1)
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
            uniform float4 _ColorBottom;
            uniform float4 _ColorTop;
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float node_7757 = (i.posWorld.g*0.75+0.25);
                float3 emissive = (lerp(_ColorTop.rgb,_ColorBottom.rgb,node_7757)*lerp(2.0,0.0,node_7757));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
