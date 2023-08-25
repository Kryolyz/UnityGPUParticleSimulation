# Unity Particle Simulation

This repository contains a Unity project that implements a 2D particle simulation using 100% compute shaders. The simulation includes features such as collisions, drag, gravity, and two types of initial spawning setups.

## Features

- **Collisions**: Particles in the simulation can collide with each other and with the environment.
- **Drag**: Particles are subject to drag forces that slow them down over time.
- **Gravity**: Particles are affected by gravity, which pulls them towards the bottom of the simulation.
- **Initial Spawning Setups**: The simulation includes two types of initial spawning setups for particles.
- **Shader Manager**: The project includes a shader manager class that simplifies the use of shaders by implementing a plug-and-play system. Shaders only need to implement the `IShaderProgram` interface to be compatible with the manager, which handles their execution and allows you to control their call frequency and order through a priority system. The manager also enables the use of global variables in compute shaders, which is not typically possible, by adding them to a public list and setting them for all managed shaders. Global variables can be either constants or updatable, allowing for dynamic updating of shader variables and experimentation during runtime. This, however, comes at the cost of overhead since the variables currently get updated constantly in fixed update, instead of in an event system.

## Demo Video

A demo video of the particle simulation in action is available in the `ImagesVideos` folder in the root of the repository. The video, named `512x512.mp4`, shows an example simulation in a grid of 512x512. In the video, particles are sprayed in a rainbow coloring and the source, initial direction, and velocity are moved around. 
Higher quality version in ImagesVideos to download. The embedding only allows small files.

https://github.com/Kryolyz/UnityGPUParticleSimulation/assets/33807498/2241f356-1f0b-403a-91ea-3164bd1cec1c



## Usage

To use this project, follow these steps:

1. Clone the repository to your local machine.
2. Open the project in Unity.
3. Press the Play button to start the simulation.

## Performance

The particle simulation system is currently capable of comfortably handling about 100,000 particles on my gpu but starts dropping somewhere in the lower 6 digits. The only major optimization implemented is the classic binning technique for collisions. However, this level of performance is not sufficient for the intended goals of the project. Dispatch calls and shader variable updates appear to be significant performance bottlenecks. As a result, the system will be rewritten in C++ and OpenGL to improve performance.