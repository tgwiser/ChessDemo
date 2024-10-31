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



  
![Chess drawio](https://github.com/user-attachments/assets/f5ee26c7-3cf0-4221-961c-10db6b36248d)




## Model design 
- ChessEngine: the entry point (Facade Pattern)
- PositionEvaluator: Responsible of returning all possible move by piece
- BoardManager: Responsible of getting & setting data on the board
- GameEvaluator: highly efficient component that return the best move based on the depth search
  

![Chess-Page-3 drawio (1)](https://github.com/user-attachments/assets/82de6c8c-306f-47da-8858-9eca97e2b489)


