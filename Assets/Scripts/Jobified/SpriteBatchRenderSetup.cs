using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;


namespace vadersb.utils.unity.jobs
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class SpriteBatchRenderSetup : MonoBehaviour
    {
        //from: https://thomasmountainborn.com/2016/05/25/materialpropertyblocks/
        //Using renderer.SetPropertyBlock(), we can apply our new values to the renderer.
        //Job done! However, it is important to note that setting a property block on a renderer
        //overwrites any other data set by property blocks on that renderer.
        //If your new property block doesn't hold a value that was present before, that value will be reset. 

        [SerializeField]
        private SpriteList m_SpriteList;

        [SerializeField]
        private Color m_Color = Color.white;


        //refs
        private MeshRenderer m_MeshRenderer;
        private MeshFilter m_MeshFilter;

        //mesh
        private Mesh m_Mesh;
        
        //material properties block
        private MaterialPropertyBlock m_MaterialPropertyBlock;
        
        
        private static readonly int s_ColorProperty = Shader.PropertyToID("_RendererColor");
        private static readonly int s_TextureProperty = Shader.PropertyToID("_MainTex");


        void Awake()
        {
            m_MeshRenderer = GetComponent<MeshRenderer>();
            Debug.Assert(m_MeshRenderer != null);

            m_MeshFilter = GetComponent<MeshFilter>();
            Debug.Assert(m_MeshFilter != null);
            
            #if DEBUG
            if (m_MeshFilter.sharedMesh != null)
            {
                Debug.LogWarning("Mesh filter contains some mesh that will be overridden by sprite batcher!");
            }
            #endif

            m_Mesh = new Mesh();
            m_Mesh.indexFormat = IndexFormat.UInt32;
            m_Mesh.MarkDynamic();

            m_MeshFilter.sharedMesh = m_Mesh;
            
            m_MaterialPropertyBlock = new MaterialPropertyBlock();
            
            m_SpriteList.Init();

            RefreshMaterialPropertyTexture();
            RefreshMaterialPropertyColor();
            ApplyMaterialPropertyBlock();
        }


        private void OnValidate()
        {
            //Debug.Log("SpriteBatchRenderSetup.OnValidate()");
            bool applyPropertyBlock = m_MaterialPropertyBlock != null && m_MeshRenderer != null;
            
            if (m_SpriteList != null)
            {
                //m_SpriteList.Init();

                if (applyPropertyBlock)
                {
                    RefreshMaterialPropertyTexture();
                }
            }

            if (applyPropertyBlock)
            {
                RefreshMaterialPropertyColor();
                ApplyMaterialPropertyBlock();
            }
        }


        private void OnDestroy()
        {
            m_SpriteList.Dispose();
        }


        public Mesh Mesh => m_Mesh;

        
        public MaterialPropertyBlock MaterialPropertyBlock => m_MaterialPropertyBlock;


        public NativeArray<SpriteData> SpriteDataArray => m_SpriteList.SpriteDataArray;

        public Color Color
        {
            get => m_Color;
            set
            {
                m_Color = value;
                RefreshMaterialPropertyColor();
                ApplyMaterialPropertyBlock();
            }
        }


        private void RefreshMaterialPropertyTexture()
        {
            Debug.Assert(m_MaterialPropertyBlock != null);

            Texture texture = m_SpriteList.Texture;

            m_MaterialPropertyBlock.SetTexture(s_TextureProperty, texture);
        }

        
        private void RefreshMaterialPropertyColor()
        {
            Debug.Assert(m_MaterialPropertyBlock != null);
            
            m_MaterialPropertyBlock.SetColor(s_ColorProperty, m_Color);
        }


        private void ApplyMaterialPropertyBlock()
        {
            Debug.Assert(m_MaterialPropertyBlock != null);
            Debug.Assert(m_MeshRenderer != null);
            
            m_MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);
        }
    }
}