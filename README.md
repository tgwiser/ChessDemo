# Chess engine designed with SOLID
This is a new project created in .netCore, 

I'll implement the Minimax algorithm to determine the best moves. 

I will not use existing code - for learning purpose

## Modeel design 
- **Position**: The location on the board.
- **Move**: The action a player (or the engine) is considering
- **Board**: An 8x8 data structure that represents the chessboard.
- **Piece**: The player possition & piece & type
- **Castle**: A special move that add some more complexability (2 moves in one move)

<img width="500" alt="Chess-Models drawio (1)" src="https://github.com/user-attachments/assets/8a469c1d-837c-4deb-b516-23ffdf1d7871">




## Services design 
- **Persistense**: Responsible for Saving & Loading games, (This feture will enable loading of PGN games)
- **GameHistory**: Responsible for Storing the game moves, (This feture will enable saving the game, Moving to prev moves & more) 
- **BoardManager**: Responsible of getting & setting data on the board
- **PositionEvaluator**: Responsible of returning all possible move by piece
- **GameEvaluator**: highly efficient component that return the best move based on the depth search


<img width="500" alt="Chess-Models drawio (1)" src="https://github.com/user-attachments/assets/d94255b9-f369-4497-8128-a09067473602">

## Application design 
- **ChessWeb**: Demo client
- **ChessEngine**: the entry point (Facade Pattern)

<img width="500" alt="Chess-Models drawio (1)" src="https://github.com/user-attachments/assets/17d2e13e-aff6-498d-911b-31a612b141e3">


