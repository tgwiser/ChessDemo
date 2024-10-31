# Chess engine designed with SOLID (Starting draft)

This is a new project created in c#, 

We will use the MinMax algoritem for finding best move

I will not use existing code - for learning purpose

## Model design 
- Possition: location on the board
- Move: the move the player (or the engine) think of playing
- Board: a data structure of 8x8 which represent the chess board
- Piece: the player, possition & type
- Castle: This is a special move that add some more complexability (2 moves in one move)

<img width="500" alt="Chess-Models drawio (1)" src="https://github.com/user-attachments/assets/8a469c1d-837c-4deb-b516-23ffdf1d7871">



## Appication design 
- ChessEngine: the entry point (Facade Pattern)
- PositionEvaluator: Responsible of returning all possible move by piece
- BoardManager: Responsible of getting & setting data on the board
- GameEvaluator: highly efficient component that return the best move based on the depth search
  
<img width="500" alt="Chess-Models drawio (1)" src="https://github.com/user-attachments/assets/571ae7bd-f20b-4174-8341-4b0993db5d57">


