using UnityEditor;
using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class ThirdPersonCharacterBuilder
    {
    private const string AnimatorPath = "Assets/StarterAssets/ThirdPersonController/Character/Animations/StarterAssetsThirdPerson.controller";
    private const string InputAssetPath = "Assets/StarterAssets/InputSystem/StarterAssets.inputactions";
    private const string CameraTargetObjectName = "CameraTarget";

    [MenuItem("Ready Player Me/Setup Character", true, 0)]
    public static bool SetupCharacterValidate()
        {
        return Selection.activeGameObject != null;
        }

    [MenuItem("Ready Player Me/Setup Character")]
    public static void SetupCharacter()
        {
        // Cache selected object to add the components
        GameObject character = Selection.activeGameObject;

        // Create camera follow target
        GameObject cameraTarget = new GameObject(CameraTargetObjectName);
        cameraTarget.transform.parent = character.transform;
        cameraTarget.transform.localPosition = new Vector3(0, 1.5f, 0);

        // Set the animator controller and disable root motion
        Animator animator = character.GetComponent<Animator>();
        animator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(AnimatorPath);
        animator.applyRootMotion = false;

        // Add tp controller and set values
        ThirdPersonController tpsController = character.AddComponent<ThirdPersonController>();
        tpsController.GroundedOffset = 0.1f;
        tpsController.GroundLayers = 1;
        tpsController.JumpTimeout = 0.5f;
        tpsController.CinemachineCameraTarget = cameraTarget;

        // Add character controller and set size
        CharacterController characterController = character.GetComponent<CharacterController>();
        characterController.center = new Vector3(0, 1, 0);
        characterController.radius = 0.3f;
        characterController.height = 1.9f;

        // Add player input and set actions asset
        PlayerInput playerInput = character.GetComponent<PlayerInput>();
        playerInput.actions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputAssetPath);

        // Add components with default values
        character.AddComponent<BasicRigidBodyPush>();
        character.AddComponent<StarterAssetsInputs>();

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
    }