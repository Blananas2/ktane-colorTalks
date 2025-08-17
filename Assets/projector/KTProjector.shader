Shader "KT/Projector" {
    Properties {
        _MainTex ("Projected Texture", 2D) = "white" {}
        _FalloffTex ("Falloff", 2D) = "white" {}
    }
    
    Subshader {
        Tags {"Queue"="Transparent"}
        Pass {
            ZWrite Off
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha
            Offset -1, -1

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"
            
            struct v2f {
                float4 uvMain : TEXCOORD0;
                float4 uvFalloff : TEXCOORD1;
                UNITY_FOG_COORDS(2)
                float4 pos : SV_POSITION;
            };
            
            float4x4 unity_Projector;
            float4x4 unity_ProjectorClip;
            
            v2f vert (float4 vertex : POSITION) {
                v2f o;
                o.pos = UnityObjectToClipPos(vertex);
                o.uvMain = mul(unity_Projector, vertex);
                o.uvFalloff = mul(unity_ProjectorClip, vertex);
                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }
            
            sampler2D _MainTex;
            sampler2D _FalloffTex;
            
            fixed4 frag (v2f i) : SV_Target {
                float4 projCoords = i.uvMain;
                
                if (projCoords.w <= 0) {
                    discard;
                }
                
                float2 uv = projCoords.xy / projCoords.w;
                
                if (uv.x < 0 || uv.x > 1 || uv.y < 0 || uv.y > 1) {
                    discard;
                }
                
                fixed4 texMain = tex2Dproj(_MainTex, UNITY_PROJ_COORD(i.uvMain));
                
                fixed4 texFalloff = tex2Dproj(_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff));
                
                fixed4 result = texMain;
                result.a *= texFalloff.a;
                
                UNITY_APPLY_FOG(i.fogCoord, result);
                return result;
            }
            ENDCG
        }
    }
}