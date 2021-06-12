
# Contributing to Auth0 projects

A big welcome and thank you for looking at Ceres!
If you need any help with contributing, please join our [Discord](https://discord.gg/rECRhBT6Et) and ask for help!

Reading and following these guidelines will help us make the contribution process easy and effective for everyone involved. It also communicates that you agree to respect the time of the developers managing and developing these open source projects. In return, we will reciprocate that respect by addressing your issue, assessing changes, and helping you finalize your changes.

## Quicklinks

* [Code of Conduct](#code-of-conduct)
* [Getting Started](#getting-started)
    * [Issues](#issues)
    * [Pull Requests](#pull-requests)
	* [For Artists, Musicians, and Designers](#steps-for-artists-musicians-and-designers)
	* [For Developers](#steps-for-developers)
* [Getting Help](#getting-help)

## Code of Conduct

We take our open source community seriously and hold ourselves and other contributors to high standards of communication. By participating and contributing to this project, you agree to uphold our [Code of Conduct](https://github.com/ezraay/ceres/blob/master/CODE_OF_CONDUCT.md).

## Getting Started
**If you have never used Git or Github, it's recommended you watch [some videos](https://www.youtube.com/watch?v=RbSrx0QoTG4&list=PLZplUm29-Z-xOYY9Tw6t3tSIvlbhVhzUn&index=2) to get up to speed on how to add some changes to the game.**

### Issues
[Issues Page](https://github.com/Ezraay/ceres/issues)
Issues should be used to report problems with the game, request a new feature, or to discuss potential changes before a PR is created. When you create a new Issue, a template will be loaded that will guide you through collecting and providing the relevant information.

If you find an Issue that addresses a problem similar to the one you want to discuss, please add your own information to the existing issue rather than creating a new one. 

### Pull Requests
[Create a Pull Request](https://github.com/Ezraay/ceres/compare)
For changes that address core functionality or would require breaking changes (e.g. a major release), it's best to open an Issue to discuss your proposal first. This is not required but can save time creating and reviewing changes.

In general, we follow the ["fork-and-pull" Git workflow](https://github.com/susam/gitpr)

### Steps for Artists, Musicians, and Designers
1. Fork the repository to your own Github account
2. Clone the project to your machine
3. Select the necessary branch (art-assets, audio-assets, world-design)
4. Make some changes
5. Commit changes to the branch
6. Push changes to your fork
7. Open a PR in our repository and follow the PR template so that we can efficiently review the changes.

### Steps for Developers
Setting up a Development environment is simple for singleplayer development, but networking features won't work without more work. See below.  
1. Fork the repository to your own Github account
2. Clone the project to your machine
3. Create a branch locally with a succinct but descriptive name
4. Make some changes
5. Commit changes to the branch
6. Push changes to your fork
7. Open a PR in our repository and follow the PR template so that we can efficiently review the changes.  

#### Steps to set up the server
1. Create a new Firebase project. This may be removed in the future with a public test project or Steamworks integration for data saving.
2. Enable Unity development on it by clicking the Unity icon. 
3. Enable the Auth and Realtime Database modules.
4. Download the `google-services.json` file from the Project Settings and place in the StreamingAssets folder.
5. Force Unity to recompile. One way to do this is to change a file slightly and save. Not sure if there's another way.
6. Firebase SDK should've generated a `google-services-json` file in the same StreamingAssets folder.
7. To make a client build, build the game with Development Build enabled. Select Server Build to make a server instead.
8. You can also of course use the Unity Editor as either a Client or a Server for debugging. To run a client, load the Main Menu scene and start. To run a server, load the Game scene and change the Game Manager "Single Player" bool to false.

## Getting Help

Join us on [Discord](https://discord.gg/rECRhBT6Et) and post your question there in the correct category.