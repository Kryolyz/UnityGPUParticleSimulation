# Unity Particle Simulation

This repository contains a Unity project that implements a 2D particle simulation using 100% compute shaders. The simulation includes features such as collisions, drag, gravity, and two types of initial spawning setups.

## Features

- **Collisions**: Particles in the simulation can collide with each other and with the environment.
- **Drag**: Particles are subject to drag forces that slow them down over time.
- **Gravity**: Particles are affected by gravity, which pulls them towards the bottom of the simulation.
- **Initial Spawning Setups**: The simulation includes two types of initial spawning setups for particles.
- **Shader Manager**: The project includes a shader manager class that simplifies the use of shaders by implementing a plug-and-play system. Shaders only need to implement the `IShaderProgram` interface to be compatible with the manager, which handles their execution and allows you to control their call frequency and order through a priority system. The manager also enables the use of global variables in compute shaders, which is not typically possible, by adding them to a public list and setting them for all managed shaders. Global variables can be either constants or updatable, allowing for dynamic updating of shader variables and experimentation during runtime. This, however, comes at the cost of overhead since the variables currently get updated constantly in fixed update, instead of in an event system.

## Usage

To use this project, follow these steps:

1. Clone the repository to your local machine.
2. Open the project in Unity.
3. Press the Play button to start the simulation.

## Contributing

Contributions to this project are welcome! If you have an idea for a new feature or want to report a bug, please open an issue on GitHub.

## License

This project is licensed under the MIT License. See the LICENSE file for more information.
