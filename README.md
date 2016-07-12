# Snake

[![Build status](https://ci.appveyor.com/api/projects/status/vjl1hutlob8w3s5w?svg=true)](https://ci.appveyor.com/project/veblush/snake)
[![Coverage Status](https://coveralls.io/repos/github/SaladLab/Snake/badge.svg?branch=master)](https://coveralls.io/github/SaladLab/Snake/Snake?branch=master)

Reference game using EntityNetwork, Akka.Interfaced and TrackableData.

![Screenshot](https://raw.githubusercontent.com/SaladLab/Snake/master/docs/MainScene.jpg)
![Screenshot](https://raw.githubusercontent.com/SaladLab/Snake/master/docs/GameScene.jpg)

## How to run

### Prerequisites

- MongoDB 3 or later
- Visual Studio 2015 or later (it's not mandatory if you can build projects)
- Unity 5.3 or later

### Steps

- Make sure MongoDB is running well.
  - By default server connects to local MongoDB.
  - Address of MongoDB can be configured on src/GameServer/App.config.
- Run Server
  - Open Snake.sln with Visual Studio.
  - Run GameServer.
- Run Client
  - Open src/GameClient with Unity.
  - Open Scenes/MainScene and run.

### Steps for testing gameplay only

For testing gameplay itself, you can play game without server.

- Run Test Client
  - Open src/GameClient with Unity.
  - Open Scenes/GameTestScene and run.
  - For first client, select `Lan Host`.
  - For second client, select `Lan Client` with host address.
