# Mesekai Unity
<p align="center">
  <img src="https://github.com/Neleac/MesekaiUnity/blob/main/public/avatar.PNG" width="45%" />
  <img src="https://github.com/Neleac/MesekaiUnity/blob/main/public/world.PNG" width="42.75%" />
</p>

Mesekai is a real-time motion tracking virtual avatar application. Use your physical body through webcam video to control the avatar's arms, fingers, head, and facial expressions. It supports personalized [ReadyPlayerMe](https://readyplayer.me) avatars.

The primary use case is Vtubing. Additionally, the motion tracking feature can be applied to a 3rd person online multiplayer game for immersive social interactions. Enter the hub world to try this feature with other players.

## Trailer
[Mesekai - Webcam Motion Tracking Avatars](https://www.youtube.com/watch?v=rYbg6OU8-7E)

## Download
[itch.io page](https://neleac.itch.io/mesekai)

## Browser Verison
[Mesekai repository](https://github.com/Neleac/Mesekai)

## Extended Abstract
[Motion Tracking Avatars in Desktop Multiplayer Online Games](https://github.com/Neleac/MesekaiUnity/blob/main/public/Motion%20Tracking%20Avatars%20in%20Desktop%20Multiplayer%20Online%20Games.pdf)

## Dependencies
This project performs pose estimation using my fork of the [MediaPipeUnityPlugin](https://github.com/Neleac/MediaPipeUnityPlugin/tree/mesekai). Refer to the [git documentation](https://git-scm.com/book/en/v2/Git-Tools-Submodules) on setting up submodules. Follow MediaPipeUnityPlugin's instructions to build the plugin. Once successful, move `/Packages/com.github.homuler.mediapipe/` and `/Assets/StreamingAssets/` into the main project.

Also need [git LFS](https://git-lfs.com/).
