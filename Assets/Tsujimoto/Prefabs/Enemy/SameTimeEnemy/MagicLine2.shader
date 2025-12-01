Shader "Custom/MagicLine_True"
{
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _Speed("Scroll Speed", Float) = 1
    }
        SubShader{
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
            Blend One One
            ZWrite Off
            Cull Off

            Pass{
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                sampler2D _MainTex;
                float _Speed;

                struct appdata { float4 vertex:POSITION; float2 uv:TEXCOORD0; };
                struct v2f { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; };

                v2f vert(appdata v) {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);

                    // ★★ LineRenderer 用：UV.x に Time を加算（Tile / Stretch どちらでも動く）
                    o.uv = v.uv;
                    o.uv.x += _Time.y * _Speed;

                    return o;
                }

                fixed4 frag(v2f i) :SV_Target{
                    return tex2D(_MainTex, i.uv);
                }
                ENDCG
            }
        }
}
