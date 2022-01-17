 Shader "Sprites/Custom"
 {
     Properties
     {
         _MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
         _Opacity("Opacity", Float) = 1
     }
     SubShader
     {
         Tags { 
			"RenderType"="Transparent" 
			"Queue"="Transparent"
		 }
         LOD 100
		 Blend SrcAlpha OneMinusSrcAlpha
 
         Pass
         {
			ZWrite Off
			 Cull Off

             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
 
             #include "UnityCG.cginc"
 
             struct appdata
             {
                 float4 vertex : POSITION;
                 float2 uv : TEXCOORD0;
             };
 
             struct v2f
             {
                 float2 uv : TEXCOORD0;
                 UNITY_FOG_COORDS(1)
                 float4 vertex : SV_POSITION;
             };
 
             sampler2D _MainTex;
             float4 _MainTex_ST;
             float _Opacity;
 
             v2f vert (appdata v)
             {
                 v2f o;
                 o.vertex = UnityObjectToClipPos(v.vertex);
                 o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                 return o;
             }
 
             fixed4 frag (v2f i) : SV_Target
             {
                 // sample the texture
                 fixed4 col = tex2D(_MainTex, i.uv);
				 col.a *= _Opacity;
                 return col;
             }
             ENDCG
         }
     }
 }