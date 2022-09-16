# Flappy Bird Using Reinforcement Learning
## Table of contents
* [General info](#general-info)
* [Requirements](#requirements)
* [Setup](#setup)
* [Details](#details)
* [Configuration](#configuration)

## General info
This project allows you to train a AI agent that can play flappy bird. its current high score is 978, but it can improved even further.
I've also created a level manager so oracle change have more difficult levels. You can change the level difficulty by accessing `Levels` file in `Data` folder

## Requirements
Project is created with:
* Python version: 3.6+ 64-bit
* Unity version: 2020.1+
* Unity ML-Agents version: 1.0.0+ (see https://github.com/Unity-Technologies/ml-agents/blob/release_12_docs/docs/Installation.md)
* Python mlagents version: 2.6.1+

## Setup
To run this project,
1. Install dependencies:

```
$ pip install mlagents
```

2. Clone this repository.
3. Open project in Unity
4. go to project directory:
```
$ cd {project directory}\flappy-bird-AI\Assets\ML-Agents\config
```
5. run below command to connect mlagents to unity environment. after seeing unity logo in command prompt, hit play in unity to start training the agent.
```
$ mlagents-learn ./FlappyBird.yaml --run-id={model name}
```

6. Once the training is complete, brain data are created in the folder:
```
./results/{model name}
```
This file `{model name}.onnx` contains the actual trained neural network. Copy this file to the Unity project folder and assign it to the `Model` field of the `Behaviour Parameters` . You can now hit play.


**P.S** if you wish to stop the game and continue the learning process you can simply stop the game in unity and resume it later using this command:
```
$ mlagents-learn ./FlappyBird.yaml --run-id={model name} --resume
```

## Configuration
While the system is able to learn the behavior quite well already, I found it is better to increase the complexity of the neural network a bit. Create a trainer_config.yaml file in the current folder, with the following content:
```
behaviors:
    FlappyBird::
        trainer_type: ppo    
        summary_freq: 10000
        max_steps: 5.0e6
        network_settings:
            hidden_units: 256

```
This will double the number of neurons per layer and add one more layer, allowing the system to learn a more complex function.

## Details
My **hb_05 model** was able to get scores over 70 most of the times after 2 hours of training. you can change `FlappyBird.yaml` file to imrpove its current score.
This is a image of its totall reward during the training:
![rewards](https://user-images.githubusercontent.com/45734322/190683309-caffa01b-be1a-4cc6-8988-1d8ff2b6f7e4.png)
there is a sudden drop after 550K epochs because I changed some of the yaml file behaviors during the training which turned out to be not good so I undid my changes.

### Observers
There are a totall of 10 observations:
2. Distance from , second, and back pipes in z axis (3 observations)
3. Distance from up and down pipes in y axis (2 observations)
4. Distance from up and down back pipes in y axis (2 observations)
5. Distance from the ground (-2.65) (1 observation)
6. Bird's y velocity (1 observation)
7. Speed of the curent level (1 observation)

### Actions
There is just 1 action:
1. Jump
