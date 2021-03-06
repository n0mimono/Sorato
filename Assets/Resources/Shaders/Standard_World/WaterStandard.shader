﻿// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Standard/Water" {
  Properties {
    _Color ("Color", Color) = (1,1,1,1)
    _MainTex ("Albedo (RGB)", 2D) = "white" {}

    _Metallic ("Metallic", Range(0, 1)) = 0
    _Gloss ("Gloss", Range(0, 1)) = 1

    _BaseNormal ("Base Normal Power", Float) = 0
    [NoScaleOffset] _BumpMap0 ("Normal Map 0", 2D) = "bump" {}
    [NoScaleOffset] _BumpMap1 ("Normal Map 1", 2D) = "bump" {}

    _WaveSpeed4 ("Wave Speed 4", Vector) = (1,1,-1,-1)
    _WaveScale4 ("Wave Scale 4", Vector) = (1,1,1,1)

    _Mip("Cupe Mip Resolution", Range(1, 10)) = 7
    [KeywordEnum(BRDF1, BRDF2, BRDF3)] _PBS_QUALITY("PBR Quality", Float) = 0

    _AmpFog ("Amplify Fog", Float) = 1
  }
  SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 100

    CGINCLUDE
    #include "UnityCG.cginc"
    #include "AutoLight.cginc"
    #include "UnityPBSLighting.cginc"
    #include "UnityStandardBRDF.cginc"
    #pragma multi_compile _PBS_QUALITY_BRDF1 _PBS_QUALITY_BRDF2 _PBS_QUALITY_BRDF3
    #if defined(_PBS_QUALITY_BRDF1)
       #define STANDARD_PBS BRDF1_Unity_PBS
    #elif defined(_PBS_QUALITY_BRDF2)
       #define STANDARD_PBS BRDF2_Unity_PBS
    #elif defined(_PBS_QUALITY_BRDF3)
       #define STANDARD_PBS BRDF3_Unity_PBS
    #else
       #define STANDARD_PBS BRDF1_Unity_PBS
    #endif

    half4 _Color;
    sampler2D _MainTex; half4 _MainTex_ST;

    uniform half _Metallic;
    uniform half _Gloss;

    uniform half _BaseNormal;

    uniform sampler2D _BumpMap0;
    uniform sampler2D _BumpMap1;
    uniform half4 _WaveSpeed4;
    uniform half4 _WaveScale4;
    uniform half _Mip;

    uniform half _AmpFog;

    struct v2f {
      float4 pos      : SV_POSITION;
      float2 uv       : TEXCOORD0;
      float3 worldPos : TEXCOORD1;
      float3 normal   : TEXCOORD2;
      float2 bumpuv0  : TEXCOORD3;
      float2 bumpuv1  : TEXCOORD4;
      LIGHTING_COORDS(5,6)
      UNITY_FOG_COORDS(7)
    };

    v2f vert(appdata_full v) {
      v2f o;

      o.pos = UnityObjectToClipPos(v.vertex);
      o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
      o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
      o.normal = v.normal;

      float4 wpos = o.worldPos.xzxz + o.worldPos.y;
      float4 temp = (wpos + _WaveSpeed4 * _Time.x) * _WaveScale4;
      o.bumpuv0 = temp.xy;
      o.bumpuv1 = temp.wz;

      TRANSFER_VERTEX_TO_FRAGMENT(o);
      UNITY_TRANSFER_FOG(o,o.pos);
      return o;
    }

    half4 frag(v2f i) : SV_Target {
      half atten = LIGHT_ATTENUATION(i);

      half3 normalDir  = i.normal;
      half3 viewDir    = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
      half3 lightDir   = normalize(_WorldSpaceLightPos0.xyz);

      // normal direction
      half3 bump0 = UnpackNormal(tex2D( _BumpMap0, i.bumpuv0 )).rbg;
      half3 bump1 = UnpackNormal(tex2D( _BumpMap1, i.bumpuv1 )).rbg;
      half3 bump = normalize(normalDir*_BaseNormal + bump0 + bump1);
      half3 bumpReflect = reflect( -viewDir, bump );

      // gi set
      UnityGI gi;
      half4 rgbm = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, bumpReflect, _Mip);
      gi.light.dir = lightDir;
      gi.light.color = _LightColor0.rgb * atten;
      gi.light.ndotl = LambertTerm (bump, gi.light.dir);
      gi.indirect.specular = 2.0 * rgbm.rgb;
      gi.indirect.diffuse  = half3(1,1,1);

      // main albedo
      half3 albedo = tex2D(_MainTex, i.uv) * _Color;

      // calc diffuse and specular
      half oneMinusReflectivity;
      half3 specColor;
      half3 diffColor = DiffuseAndSpecularFromMetallic(albedo, _Metallic, specColor, oneMinusReflectivity);

      // calc PBS
      fixed4 col = STANDARD_PBS(diffColor, specColor, oneMinusReflectivity, _Gloss, bump, viewDir, gi.light, gi.indirect);

      #if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
      i.fogCoord = i.fogCoord * _AmpFog;
      #endif
      UNITY_APPLY_FOG(i.fogCoord, col);
      return col;
    }

    ENDCG

    Pass {
      Tags { "LightMode" = "ForwardBase" }
      CGPROGRAM
      #pragma multi_compile_fwdbase
      #pragma multi_compile_fog
      #pragma vertex vert
      #pragma fragment frag
      #pragma target 3.0
      ENDCG
    }
  }
  FallBack "Mobile/Diffuse"
}