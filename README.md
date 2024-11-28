# Chess engine designed with SOLID (Starting draft)

This is a new project created in c#, 

I'll implement the Minimax algorithm to determine the best moves. 

I will not use existing code - for learning purpose

## Model design 
- **Position**: The location on the board.
- **Move**: The action a player (or the engine) is considering
- **Board**: An 8x8 data structure that represents the chessboard.
- **Piece**: The player possition & piece & type
- **Castle**: A special move that add some more complexability (2 moves in one move)

<img width="500" alt="Chess-Models drawio (1)" src="https://github.com/user-attachments/assets/8a469c1d-837c-4deb-b516-23ffdf1d7871">



## Application design 
- **ChessEngine**: the entry point (Facade Pattern)
- **PositionEvaluator**: Responsible of returning all possible move by piece
- **BoardManager**: Responsible of getting & setting data on the board
- **GameEvaluator**: highly efficient component that return the best move based on the depth search

![Chess-Services drawio](https://github.com/user-attachments/assets/bf8d977f-9016-45fa-88ad-f67566d9473f)

  
<img width="500" alt="Chess-Models drawio (1)" src="https://github.com/user-attachments/assets/bf8d977f-9016-45fa-88ad-f67566d9473f">


