# Sudoku web

This small project is a two-dimensional array playground.
You will find the **sudoku.cs** class that can create valid sudoku boards using different kind of approaches, and also the possibility to randomize existing boards to create endless combinations.
You can fork and play this directly on your browser, or use this code for your existing project.

## Generation Methods
There are 5 generation methods under a stochastic methodology, which creates sets of 9 numbers to be housed in 3x3 two-dimensional arrays to create a valid board, following the 3 basic rules of Sudoku: 
1. A number can only appear once in a row.
2. A number can only appear once in a column.
3. A number can only appear once in its inner group or square.

These methods show that the ordering indeed impacts the time it takes to generate a valid board. There is also a chance that, being a random method, the possibilities of finding a valid board in the last group are exceeded, when testing the all the available 362,880 combinations generated by the following operation for each set: 1*2*3*4*5*6* 7*8*9.

## How to use

### 1. Create valid boards
There are two projects. Run **SudokuTester** to generate initial valid boards.
Select option 1: **Generate sudokus**
Then select any ordering method from 1 to 4, being the **number 3** the recommended.
This process will create a folder in the disk C automatically. The full path will be **C:/Sudoku/**
The system will start generating files with random Guid names in txt format. This will take several minutes to make some files, if you want to speed the process there is a folder **/Content/Sudoku** with 100 pre-generated files by me on the project **SudokuWebMVC**

Once you have some files created, you can re-run the SudokuTester, choose option 1: **Generate sudokus** and then option 5: **Randomize from existing folder** this will start creating valid boards from the existing ones by a simple methodology of replacing numbers. This process won't stop, so you have to stop it when you think you have enough valid files.

### 2. Play the game
Start **SudokuWebMVC** and playit on your browser.
Do not expect many validations in UI, since the fun part was creating the backend, so the front is just a simple interface to interact with.

Have fun.
