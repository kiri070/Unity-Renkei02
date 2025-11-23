Shader "Unlit/Scroll"
{
   Properties
   {
       // Unityのインスペクター上で変更できる値
       // インスペクター上で不透明なエフェクトが流れる速さや色が変更できる
       _MainTex ("Texture", 2D) = "white" {}
       _AlphaTex ("Texture", 2D) = "white" {}
       ScrollSpeed ("ScrollSpeed", Range(0, 5)) = 0
       _Color ("Color", Color) = (1,1,1,1)
   }
   SubShader
   {
       // タグは適切に設定しないと透過処理がうまくできません
       Tags { "RenderType"="Transparent" "Queue" = "Transparent" }

       // 距離に応じて処理を軽くする設定。今回はデフォルトのまま
       LOD 100

       // Alpha用
       Blend SrcAlpha OneMinusSrcAlpha

       // 後で説明
       // ZWrite Off

       // 複数のパスを記述することで処理を使い分けることもできる
       Pass
       {
           // CGPROGRAM ~ ENDCG がシェーダープログラムの範囲
           CGPROGRAM

           // 頂点・フラグメントシェーダーの関数名の設定
           #pragma vertex vert
           #pragma fragment frag
           // make fog work
           #pragma multi_compile_fog

           #include "UnityCG.cginc"

           // Unity側から送られる構造体。定義済みのものも存在する。
           // ：以降はセマンティクスと呼ばれる
           struct appdata
           {
               float4 vertex : POSITION;
               float2 uv : TEXCOORD0;
           };

           // 頂点シェーダーからフラグメントシェーダーへ渡す構造体の定義
           // 自分で定義した値も渡せる
           struct v2f
           {
               float2 uv : TEXCOORD0;
               UNITY_FOG_COORDS(1)
               float4 vertex : SV_POSITION;
           };

           // プロパティで宣言していてもここで宣言していなければ使えない
           sampler2D _MainTex;
           sampler2D _AlphaTex;
           float4 _Color;
           float ScrollSpeed;
           
           // オフセットなどの値が入っているみたい
           float4 _MainTex_ST;

           // 頂点シェーダーの部分
           v2f vert (appdata v)
           {
               v2f o;
               // オブジェクトの頂点情報からスクリーンに相当する値に変換
               o.vertex = UnityObjectToClipPos(v.vertex);
               // 同じくテクスチャも変換
               o.uv = TRANSFORM_TEX(v.uv, _MainTex);

               // ※フォグ用
               UNITY_TRANSFER_FOG(o,o.vertex);
               return o;
           }

           fixed4 frag (v2f i) : SV_Target
           {
               // _TimeでUnity上の時間を受け取れる。y成分のみ使用する
               float2 samplepoint = -ScrollSpeed * _Time;
               samplepoint.x = 0;

               // ※処理部分
               fixed4 col = tex2D(_MainTex, i.uv);
               fixed4 alphacol = tex2D(_AlphaTex, i.uv + samplepoint);

               // ※処理部分
               col.rgb = col.b * 0.5 + _Color.rgb;
               col.a = alphacol.r;

               // apply fog
               UNITY_APPLY_FOG(i.fogCoord, col);

               // 決定された色を返す
               return col;
           }
           ENDCG
       }
   }
}