#nullable enable
/*
 * 이 스크립트는 SC KRM에서 따왔기 때문에 개적화입니다
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using RuniEngine.Resource;
using RuniEngine.Resource.Images;
using System.IO;
using RuniEngine.Jsons;
using RuniEngine.Rendering;

using static RuniEngine.Editor.EditorTool;

using SpriteMetaData = RuniEngine.Resource.Images.SpriteMetaData;
using Object = UnityEngine.Object;
using RuniEngine.Editor.APIBridge.UnityEditor.UI;

namespace RuniEngine.Editor.ProjectSettings
{
    public class SpriteProjectSetting : SettingsProvider
    {
        public SpriteProjectSetting(string path, SettingsScope scopes) : base(path, scopes) { }

        static SettingsProvider? instance;
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider() => instance ??= new SpriteProjectSetting("Runi Engine/Sprite Setting", SettingsScope.Project);



        [SerializeField] string nameSpace = ResourceManager.defaultNameSpace;
        [SerializeField] string type = "";
        [SerializeField] string name = "";
        [SerializeField] string tag = ImageLoader.spriteDefaultTag;
        [SerializeField] int index = 0;
        [SerializeField] float previewSize = 200;
        public override void OnGUI(string searchContext)
        {
            //라벨 길이 설정 안하면 유니티 버그 때매 이상해짐
            BeginLabelWidth(0);

            nameSpace = DrawNameSpace(TryGetText("gui.namespace"), nameSpace);
            type = DrawStringArray(TryGetText("gui.type"), type, ImageLoader.GetTypes(nameSpace));
            name = DrawStringArray(TryGetText("gui.name"), name, ImageLoader.GetSpriteNames(type, nameSpace));

            tag = EditorGUILayout.TextField(TryGetText("gui.tag"), tag);

            EditorGUILayout.Space();

            index = EditorGUILayout.IntField(TryGetText("gui.index"), index).Clamp(0);

            DrawGUI(nameSpace, type, name, tag, index);
            DrawSprite(nameSpace, type, name, tag, index, ref previewSize);

            EndLabelWidth();
        }

        public static void DrawGUI(string nameSpace, string type, string name, string tag, int index)
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            string typePath = Path.Combine(Kernel.streamingAssetsPath, ResourceManager.rootName, nameSpace, ImageLoader.name, type);
            string filePath = Path.Combine(typePath, name);
            string relativePath = Path.Combine(ResourceManager.rootName, nameSpace, ImageLoader.name, type, name);

            ResourceManager.FileExtensionExists(filePath, out filePath, ExtensionFilter.pictureFileFilter);

            if (!Directory.Exists(typePath))
                return;

            DrawLine();

            TextureMetaData textureMetaData = JsonManager.JsonRead<TextureMetaData>(typePath + ".json");

            //텍스쳐 메타데이터
            {
                EditorGUI.BeginChangeCheck();

                textureMetaData.filterMode = (FilterMode)EditorGUILayout.EnumPopup(TryGetText("texture_meta_data.sprite.filter_mode"), textureMetaData.filterMode);
                textureMetaData.generateMipmap = EditorGUILayout.Toggle(TryGetText("texture_meta_data.sprite.generate_mipmap"), textureMetaData.generateMipmap);
                textureMetaData.compressionType = (Resource.Images.TextureCompressionQuality)EditorGUILayout.EnumPopup(TryGetText("texture_meta_data.sprite.compression_type"), textureMetaData.compressionType);

                if (EditorGUI.EndChangeCheck())
                {
                    File.WriteAllText(typePath + ".json", JsonManager.ToJson(textureMetaData));
                    AssetDatabase.Refresh();
                }
            }

            Texture2D? texture = ImageLoader.GetTexture(filePath, textureMetaData, TextureFormat.Alpha8);
            if (texture == null || string.IsNullOrWhiteSpace(name))
            {
                DrawLine();

                EditorGUILayout.LabelField($"{TryGetText("gui.path")} - " + relativePath.Replace("\\", "/"));
                return;
            }

            DrawLine();

            //스프라이트 메타데이터
            try
            {
                Dictionary<string, List<SpriteMetaData>>? spriteMetaDataLists = JsonManager.JsonRead<Dictionary<string, List<SpriteMetaData>>>(filePath + ".json");
                spriteMetaDataLists ??= new Dictionary<string, List<SpriteMetaData>>();

                if (!spriteMetaDataLists.ContainsKey(ImageLoader.spriteDefaultTag))
                    spriteMetaDataLists.Add(ImageLoader.spriteDefaultTag, new List<SpriteMetaData>());

                List<SpriteMetaData> spriteMetaDatas;
                if (spriteMetaDataLists.ContainsKey(tag))
                    spriteMetaDatas = spriteMetaDataLists[tag];
                else
                    spriteMetaDatas = spriteMetaDataLists[ImageLoader.spriteDefaultTag];

                EditorGUI.BeginChangeCheck();

                if (index < spriteMetaDatas.Count)
                {
                    SpriteMetaData spriteMetaData = spriteMetaDatas[index];
                    spriteMetaData.RectMinMax(texture.width, texture.height);

                    spriteMetaData.pivot = EditorGUILayout.Vector2Field(TryGetText("gui.pivot"), spriteMetaData.pivot);
                    spriteMetaData.rect = EditorGUILayout.Vector4Field(TryGetText("sprite_meta_data.rect"), spriteMetaData.rect);
                    spriteMetaData.border = EditorGUILayout.Vector4Field(TryGetText("gui.border"), spriteMetaData.border);

                    EditorGUILayout.Space();

                    spriteMetaData.pixelsPerUnit = EditorGUILayout.FloatField(TryGetText("sprite_meta_data.pixelsPerUnit"), spriteMetaData.pixelsPerUnit);

                    DrawLine();

                    spriteMetaDatas[index] = spriteMetaData;

                    if (GUILayout.Button(TryGetText("project_setting.sprite.sprite_delete")))
                        spriteMetaDatas.RemoveAt(index);
                }
                else if (GUILayout.Button(TryGetText("project_setting.sprite.sprite_create")))
                {
                    SpriteMetaData spriteMetaData = new SpriteMetaData();

                    spriteMetaData.RectMinMax(texture.width, texture.height);
                    spriteMetaDatas.Add(spriteMetaData);
                }

                //태그
                if (!spriteMetaDataLists.ContainsKey(tag))
                {
                    if (GUILayout.Button(TryGetText("project_setting.sprite.tag_create")))
                        spriteMetaDataLists.Add(tag, new List<SpriteMetaData>());

                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox(TryGetText("project_setting.sprite.tag_warning").Replace("{tag}", tag), MessageType.None);
                }
                else
                {
                    EditorGUI.BeginDisabledGroup(tag == ImageLoader.spriteDefaultTag);

                    if (GUILayout.Button(TryGetText("project_setting.sprite.tag_delete")))
                        spriteMetaDataLists.Remove(tag);

                    EditorGUI.EndDisabledGroup();
                }

                if (EditorGUI.EndChangeCheck())
                {
                    File.WriteAllText(filePath + ".json", JsonManager.ToJson(spriteMetaDataLists));
                    AssetDatabase.Refresh();
                }
            }
            finally
            {
                Object.DestroyImmediate(texture);
            }

            DrawLine();

            EditorGUILayout.LabelField($"{TryGetText("gui.path")} - " + relativePath.Replace("\\", "/"));
        }

        public static void DrawSprite(string nameSpace, string type, string name, string tag, int index, ref float previewSize)
        {
            Sprite? sprite = SpriteSetterBase.GetSprite(type, name, index, nameSpace, tag);
            if (sprite == null)
                return;

            DrawLine();

            {
                Space();

                GUILayout.BeginHorizontal();

                Space();

                previewSize = GUILayout.VerticalSlider(previewSize, 0, 400, GUILayout.Height(400));

                Space();

                SpriteDrawUtility.DrawSprite(sprite, EditorGUILayout.GetControlRect(false, previewSize), Color.white);

                GUILayout.EndHorizontal();
            }

            Object.DestroyImmediate(sprite.texture);
            Object.DestroyImmediate(sprite);
        }
    }
}
