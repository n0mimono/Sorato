Shader "Custom/WeekLit" {
  Properties {
    _MainTex ("Texture", 2D) = "white" {}
    _LitRate ("Emission Rate", Range(0, 1)) = 0.5
    _Tint ("Tint Color", Color) = (1,1,1,1)
    _Emission ("Emission", Color) = (0,0,0,0)
  }
  SubShader {
    Tags { "RenderType"="Opaque" "LightMode"="ForwardBase" }
    LOD 100

    Pass {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #pragma multi_compile_fog
      #include "UnityCG.cginc"

      sampler2D _MainTex;
      float4 _MainTex_ST;
      float _LitRate;
      float4 _Tint;
      float4 _Emission;
      
      struct appdata {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
        float3 normal : NORMAL;
      };

      struct v2f {
        float4 vertex : SV_POSITION;
        float2 uv : TEXCOORD0;
        float3 normal : TEXCOORD1;
        UNITY_FOG_COORDS(2)
      };

      v2f vert (appdata v) {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.uv = TRANSFORM_TEX(v.uv, _MainTex);
        o.normal = UnityObjectToWorldNormal(v.normal);
        UNITY_TRANSFER_FOG(o,o.vertex);
        return o;
      }
      
      fixed4 frag (v2f i) : SV_Target {
        half2 normalDir = normalize(i.normal);
        half3 lightDir = _WorldSpaceLightPos0.xyz;
        half NdotL = max(0, dot(normalDir, lightDir));
        half diff = lerp(NdotL, 1, _LitRate);

        fixed4 col = _Tint * tex2D(_MainTex, i.uv) * diff + _Emission;
        UNITY_APPLY_FOG(i.fogCoord, col);
        return col;
      }
      ENDCG
    }
  }
  Fallback "Diffuse"
}
