# Description
This project is an Elements game developed as part of a test job "https://docs.google.com/document/d/18Da2pMWS0g-YczhCYPDZDKoXbEIo4qvG-qTU9SKuUho/edit" for a Unity developer position. The game mechanics resemble the classic match3 genre, but with unique features such as the ability to move blocks in any direction in a single turn and a system to normalize the field after shifting and destroying blocks.

Startup instructions:
1. Clone the repository using the command: [git clone https://github.com/your_username/elements_game.git](https://github.com/Brodskiy/Elements.git)
2. Open the project in Unity.
3. Launch the MainScene.
4. Enjoy the game!)

Controls:
Use the swipe gesture to move the blocks in one of the four directions: top, bottom, right, left.

Game Objective:
Destroy all the blocks on the field in the minimum number of moves. The player who reaches this goal wins.

Adding new blocks:
To add new blocks, open ScriptableObject "BlocksContainer" and add a new element

<img width="413" alt="image" src="https://github.com/Brodskiy/Elements/assets/27737492/5a266a20-707d-4684-893a-2252f9f02880">

Adding new levels:
To add new levels, just make changes to the ScriptableObject "LevelsContainer".
<img width="413" alt="image" src="https://github.com/Brodskiy/Elements/assets/27737492/09e40a93-bd1a-4caf-b0ad-69648e051704">

Desired refinements:
1. Now the work with resources is realized through the folder "Resources". The "Addressables" would be better for working with resources.
2. Write a service to work with resources and control them.
3. Develop a tool for creating new levels and adding new blocks.
4. Write a service for working with animations.
5. Add block drop animation.
6. Infuse CI/CD.
7. Write unit and integration tests.
