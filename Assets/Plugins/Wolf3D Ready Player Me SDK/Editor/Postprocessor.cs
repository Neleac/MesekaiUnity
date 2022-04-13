using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Wolf3D.ReadyPlayerMe.AvatarSDK
{
    public class Postprocessor : AssetPostprocessor
    {
        #region Animation
        private const string AnimationAssetPath = "Assets/Plugins/Wolf3D Ready Player Me SDK/Resources/Animations";
        private const string AnimationTargetPath = "Assets/Plugins/Wolf3D Ready Player Me SDK/Resources/AnimationTargets";

        private const string MaleAnimationTargetName = "AnimationTargets/MaleAnimationTargetV2";
        private const string FemaleAnimationTargetName = "AnimationTargets/FemaleAnimationTargetV2";

        private static readonly string[] AnimationFiles = new string[]
        {
            "Assets/Plugins/Wolf3D Ready Player Me SDK/Resources/Animations/Female/FemaleAnimationTargetV2@Breathing Idle.fbx",
            "Assets/Plugins/Wolf3D Ready Player Me SDK/Resources/Animations/Female/FemaleAnimationTargetV2@Walking.fbx",
            "Assets/Plugins/Wolf3D Ready Player Me SDK/Resources/Animations/Male/MaleAnimationTargetV2@Breathing Idle.fbx",
            "Assets/Plugins/Wolf3D Ready Player Me SDK/Resources/Animations/Male/MaleAnimationTargetV2@Walking.fbx"
        };

        private void UpdateAnimationFileSettings(ModelImporter modelImporter)
        {
            void SetModelImportData()
            {
                if (modelImporter is null) return;
                modelImporter.useFileScale = false;
                modelImporter.animationType = ModelImporterAnimationType.Human;
            }

            if (assetPath.Contains(AnimationAssetPath))
            {
                SetModelImportData();

                bool isFemaleFolder = assetPath.Contains("Female");
                GameObject animationTarget = Resources.Load<GameObject>(isFemaleFolder ? FemaleAnimationTargetName : MaleAnimationTargetName);

                if (animationTarget != null)
                {
                    modelImporter.sourceAvatar = animationTarget.GetComponent<Animator>().avatar;
                }
            }
            else if (assetPath.Contains(AnimationTargetPath))
            {
                SetModelImportData();
            }
        }
        #endregion

        #region Shader Settings
        private const string UrpAssetName = "UniversalRenderPipelineAsset";
        private const string IncludeShaderProperty = "m_AlwaysIncludedShaders";
        private const string GraphicsSettingPath = "ProjectSettings/GraphicsSettings.asset";

        private static readonly string[] AlwaysIncludeShader = new string[4];

        private static readonly string[] ShaderNames = {
            "Standard (Specular)",
            "Standard Transparent (Specular)",
            "Standard (Metallic)",
            "Standard Transparent (Metallic)"
        };

        private static string GetShaderRoot()
        {
            var pipeline = GraphicsSettings.renderPipelineAsset;
            return pipeline?.GetType().Name == UrpAssetName ? "GLTFUtility/URP" : "GLTFUtility";
        }

        private static void UpdateAlwaysIncludedShaderList()
        {
            for (int i = 0; i < AlwaysIncludeShader.Length; i++)
            {
                AlwaysIncludeShader[i] = $"{GetShaderRoot()}/{ShaderNames[i]}";
            }

            var graphicsSettings = AssetDatabase.LoadAssetAtPath<GraphicsSettings>(GraphicsSettingPath);
            var serializedGraphicsObject = new SerializedObject(graphicsSettings);
            var shaderIncludeArray = serializedGraphicsObject.FindProperty(IncludeShaderProperty);
            var includesShader = false;

            foreach (var includeShaderName in AlwaysIncludeShader)
            {
                var shader = Shader.Find(includeShaderName);
                if (shader == null)
                {
                    break;
                }

                for (int i = 0; i < shaderIncludeArray.arraySize; ++i)
                {
                    var shaderInArray = shaderIncludeArray.GetArrayElementAtIndex(i);
                    if (shader == shaderInArray.objectReferenceValue)
                    {
                        includesShader = true;
                        break;
                    }
                }

                if (!includesShader)
                {
                    int newArrayIndex = shaderIncludeArray.arraySize;
                    shaderIncludeArray.InsertArrayElementAtIndex(newArrayIndex);
                    var shaderInArray = shaderIncludeArray.GetArrayElementAtIndex(newArrayIndex);
                    shaderInArray.objectReferenceValue = shader;
                    serializedGraphicsObject.ApplyModifiedProperties();
                }
            }
            
            AssetDatabase.SaveAssets();
        }
        #endregion

        private void OnPreprocessModel()
        {
            ModelImporter modelImporter = assetImporter as ModelImporter;
            UpdateAnimationFileSettings(modelImporter);
            
            UpdateAlwaysIncludedShaderList();
        }

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string item in importedAssets)
            {
                if (item.Contains(MaleAnimationTargetName))
                {
                    for (int i = 0; i < AnimationFiles.Length; i++)
                    {
                        AssetDatabase.ImportAsset(AnimationFiles[i]);
                    }
                }
            }
        }
    }
}