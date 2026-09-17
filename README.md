# A Contented Day in Arthur's Pass

A VR experience created for **FIT2098 - Virtual and Augmented Reality** at Monash University.

You play as a DOC ranger completing daily tasks around Arthur's Pass National Park, New Zealand. The project focuses on exploration, simple VR interactions and creating a calm, contented atmosphere as the environment progresses from morning to night.

## Gameplay

The player follows a checklist of ranger duties across the environment, including:

- Chopping and storing firewood
- Clearing fallen logs
- Repairing a damaged sign
- Collecting rubbish
- Clearing a mudslide
- Climbing to the upper area
- Checking pest traps
- Monitoring a kiwi nest
- Watching the sunset
- Returning to the cabin at the end of the day

Some areas and interactions only become available after earlier tasks are completed.

## Features

- Unity-based VR experience
- XR grab and socket interactions
- Teleportation and climbing
- Gaze-based interactions
- Custom C# task progression system
- Prerequisite and state tracking between objectives
- Interactive world-space checklist
- Dynamic time-of-day progression
- Environmental and interaction audio
- Custom low-poly 3D assets
- VR-focused level design and navigation

## Development

The project was built around connecting multiple VR interactions into a complete progression system.

A custom checklist system tracks completed objectives and controls when later tasks become available. Player progression is also connected to the environment, with the lighting and time of day gradually changing as tasks are completed.

The project also involved testing and debugging VR-specific issues including:

- XR grab behaviour
- Socket interactions
- Object pivots and attach points
- Interaction layers
- Teleport placement
- Colliders and physics
- Task progression and sequence breaking

## Tools

- **Unity**
- **C#**
- **Unity XR Interaction Toolkit**
- **Autodesk Maya**
- **Adobe Substance 3D Painter**
- **Git**

## Running the Project

The playable release is a **Windows PC VR build**.

1. Download and extract the build.
2. Connect a compatible VR headset and motion controllers to your PC.
3. Make sure the headset is recognised by your VR software.
4. Run the `.exe` file.
5. Use the blue teleport points and checklist to progress through the experience.

> Do not run the executable directly from inside the ZIP file.

## Controls

Controls use standard VR interactions:

- **Grab / Interact** - Pick up tools and objects
- **Teleport** - Move between teleport points
- **Climb** - Grab climbing holds
- **Gaze** - Look at specific interactable objects

Exact controller buttons may vary depending on the headset and controller setup.

## Project Context

This project was created individually for **FIT2098 Virtual and Augmented Reality** at Monash University.

The environment was designed around the mood of **contentment**, using a New Zealand mountain setting, environmental audio, gradual lighting changes and low-pressure interactions to create a relaxing VR experience.

## Links

- [GitHub Profile](https://github.com/Randomystic/)
- [Itch.io Portfolio](https://rand0mystic.itch.io/)
