Shader "Unlit/Particle"
{
   Properties
   {
       // Alphaで透明とする閾値を設定できるようにしている
       _MainTex ("Texture", 2D) = "white" {}
       _Color ("Color", Color) = (1,1,1,1)
       _Alpha ("Alpha", Range(0, 1)) = 1
   }
   SubShader
   {
       Tags { "RenderType"="Transparent" "Queue"="Transparent"}
       LOD 100

       // デフォルトでは裏側は表示されないが、表示するようにする設定
	   Cull off

       Blend SrcAlpha OneMinusSrcAlpha
       
       // ZWrite Off

       Pass
       {
           CGPROGRAM
           #pragma vertex vert
           #pragma fragment frag
           #pragma multi_compile_fog

           #include "UnityCG.cginc"

           // パーティクルで設定した色がcolorに入る
           struct appdata
           {
               float4 vertex : POSITION;
               float4 color : COLOR;
               float2 uv : TEXCOORD0;
           };

           struct v2f
           {
               float2 uv : TEXCOORD0;
               float4 color : COLOR;
               UNITY_FOG_COORDS(1)
               float4 vertex : SV_POSITION;
           };

           sampler2D _MainTex;
           float4 _MainTex_ST;
           float4 _Color;
           float _Alpha;

           v2f vert (appdata v)
           {
               v2f o;
               o.vertex = UnityObjectToClipPos(v.vertex);
               o.uv = TRANSFORM_TEX(v.uv, _MainTex);
               o.color = v.color;
               UNITY_TRANSFER_FOG(o,o.vertex);
               return o;
           }

           fixed4 frag (v2f i) : SV_Target
           {
               fixed4 col = tex2D(_MainTex, i.uv);

               // 引数が負の場合には表示を行わず、これ以降の処理は走らない
               clip(col.a - 0.1);

               col *= _Color * i.color;
               col.a = _Alpha;

               UNITY_APPLY_FOG(i.fogCoord, col);
               return col;
           }
           ENDCG
       }
   }
}