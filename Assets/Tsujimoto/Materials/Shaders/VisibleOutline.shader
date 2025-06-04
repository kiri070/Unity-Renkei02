Shader "Unlit/VisibleOutline"
{
    //色や太さを設定する場所
   Properties
   {
        _OutlineColor("Outline Color", Color) = (1, 0, 0, 1)
        _OutlineWidth("Outline Width", Float) = 0.05
   }

   //どんな描画をするか
   SubShader
   {
        Tags{"Queue"="Overlay"} //描画順:Overlay

        //描画処理
        Pass
        {
            Cull Front
            ZTest Always  //他のものを無視して常に描く
            ZWrite Off    //重なり順に干渉しない

            //メイン処理部分を始める宣言(中に#pragmaでどの関数を使うかなどを指定する)
            CGPROGRAM
            #pragma vertex vert //モデルの各頂点をどう描画位置に変換するか
            #pragma fragment frag //実際の画面上の色を塗る処理を書く関数
            #include "UnityCG.cginc" //Unityの便利関数を使うためのヘッダーファイル読み込み

            //シェーダー内で使う変数の定義
            fixed4 _OutlineColor;
            float _OutlineWidth;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            //モデルの頂点を外側に少し広げる関数
            v2f vert(appdata v)
            {
                v2f o;
                float3 norm = normalize(v.normal);
                float3 offset = norm * _OutlineWidth;
                o.pos = UnityObjectToClipPos(v.vertex + float4(offset, 0)); //ゲーム画面に映す座標に変換
                return o;
            }

            //アウトラインの色を塗る
            fixed4 frag(v2f i) : SV_Target
            {
                return _OutlineColor;
            }

            ENDCG
        }
   }
}