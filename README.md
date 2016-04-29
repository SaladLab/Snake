# Snake

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
  - Address of MongoDB can be configured on src/GameServer-Console/App.config.
- Run Server
  - Open Snake.sln with Visual Studio.
  - Run GameServer-Console.
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
 
## Known issues

#### Client cannot connect to GameServer sometimes

Please retry to connect. It's caused by SlimSocket and will be fixed.
