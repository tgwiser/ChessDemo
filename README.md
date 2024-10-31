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

![Chess-Models drawio](https://github.com/user-attachments/assets/7d753a97-e20d-4b7c-b98f-138d81232080)




## Appication design 
- ChessEngine: the entry point (Facade Pattern)
- PositionEvaluator: Responsible of returning all possible move by piece
- BoardManager: Responsible of getting & setting data on the board
- GameEvaluator: highly efficient component that return the best move based on the depth search
  

![Chess-Page-3 drawio (2)](https://github.com/user-attachments/assets/571ae7bd-f20b-4174-8341-4b0993db5d57)


