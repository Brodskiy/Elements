# Description
This project is an Elements game developed as part of a test job for a Unity developer position. The game mechanics resemble the classic match3 genre, but with unique features such as the ability to move blocks in any direction in a single turn and a system to normalize the field after shifting and destroying blocks.

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
1. Create a new ScriptableObject "Block".
<img width="413" alt="image" src="https://github.com/Brodskiy/Elements/assets/27737492/9c1a7ee0-3d8a-4b58-990e-a81242e2fa80">

3. Add data to ScriptableObject "Block" - "Id" and "Animation".
<img width="419" alt="image" src="https://github.com/Brodskiy/Elements/assets/27737492/903f8211-444c-442e-95aa-acd331321105">

4. If the ScriptableObject "BlocksContainer" has not been created, then create it.
<img width="413" alt="image" src="https://github.com/Brodskiy/Elements/assets/27737492/3fd39986-fee8-479b-9dc9-edea481be52a">

5. Add data to ScriptableObject "BlockContainer".
<img width="419" alt="image" src="https://github.com/Brodskiy/Elements/assets/27737492/e6257b91-85bc-444a-9d29-ac32cbb0fd3f">


Adding new levels:
1. If the ScriptableObject "LevelsContainer" has not been created, then create it.
<img width="388" alt="image" src="https://github.com/Brodskiy/Elements/assets/27737492/bb7991ad-d3e6-49cf-b944-476d1034cab7">

2. Add data to ScriptableObject "LevelsContainer":
   - "Level" - level index;
   - "Columns" - columns count;
   - "Rows" - rows count;
   - "Block Datas" - block ScriptableObject
     !!!Enter an array of values starting at the lower left corner and moving upward.!!!
<img width="416" alt="image" src="https://github.com/Brodskiy/Elements/assets/27737492/524c60bc-9373-47d6-9f01-20fb399337f7">


Desired refinements:
1. Now the work with resources is realized through the folder "Resources". The "Addressables" would be better for working with resources.
2. Write a service to work with resources and control them.
3. Develop a tool for creating new levels and adding new blocks.
4. Write a service for working with animations.
5. Add block drop animation.
6. Infuse CI/CD.
7. Write unit and integration tests.
