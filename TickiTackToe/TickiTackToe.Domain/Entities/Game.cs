﻿
using System.Text.Json;
using TickiTackToe.Domain.Enums;

namespace TickiTackToe.Domain.Entities
{
    public class Game
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int GameSize { get; set; }
        public int WinCondition { get; set; }
        public GameStatus Status { get; set; } = GameStatus.InProgress;
        public int MoveNumber { get; set; } = 1;
        public CellState CurrentPlayer { get; set; } = CellState.X;

        public string Field {  get; set; }

        private Game() { }
        public Game(int gameSize, int winCondition)
        {
            if (gameSize < 3)
                throw new ArgumentException("Game size < 3");
            if (winCondition > gameSize)
                throw new ArgumentException("Win condition > game size");

            GameSize = gameSize;
            WinCondition = winCondition;
            Field = InitialField(gameSize);
        }

        public void MakeMove(int row, int column, Func<int, bool> isLucky)
        {
            var field = GetField();

            if (Status != GameStatus.InProgress || CurrentPlayer == CellState.Empty)
                throw new InvalidOperationException("Game is finish");
            if (row >= GameSize || column >= GameSize || row < 0 || column < 0)
                throw new ArgumentOutOfRangeException("Move is out of range field");
            if (field[row][column] != CellState.Empty)
                throw new InvalidOperationException("Cell is set");

            var playerToSet = GetPlayerForCurrentMove(isLucky);

            field[row][column] = playerToSet;

            UpdateGame(field, playerToSet, row, column);

            SetField(field);

            if (Status == GameStatus.InProgress)
            {
                ChangePlayer();
                MoveNumber++;
            }
        }

        private string InitialField(int gameSize)
        {
            var field = new CellState[gameSize][];
            for (int i = 0; i < gameSize; i++)
            {
                field[i] = new CellState[gameSize];
                for (int j = 0; j < gameSize; j++)
                {
                    field[i][j] = CellState.Empty;
                }
            }

            return JsonSerializer.Serialize(field);
        }

        public CellState[][]? GetField()
        {
            return JsonSerializer.Deserialize<CellState[][]>(Field);
        }

        private void SetField(CellState[][] field)
        {
            Field = JsonSerializer.Serialize(field);
        }

        private CellState GetPlayerForCurrentMove(Func<int, bool> isLucky)
        {
            if (MoveNumber % 3 == 0 && isLucky(10))
            {
                return (CurrentPlayer == CellState.X) ? CellState.O : CellState.X;
            }
            return CurrentPlayer;
        }

        private void ChangePlayer()
        {
            CurrentPlayer = (CurrentPlayer == CellState.X) ? CellState.O : CellState.X;
        }

        private void UpdateGame(CellState[][] field, CellState player, int lastRow, int lastCol)
        {
            if (CountInDirection(field, player, lastRow, lastCol, 0, 1) >= WinCondition || // горизонталь →
                CountInDirection(field, player, lastRow, lastCol, 1, 0) >= WinCondition || // вертикаль ↓
                CountInDirection(field, player, lastRow, lastCol, 1, 1) >= WinCondition || // диагональ ↘
                CountInDirection(field, player, lastRow, lastCol, 1, -1) >= WinCondition)  // диагональ ↙
            {
                Status = player == CellState.X ? GameStatus.XWinPlayer : GameStatus.OWinPlayer;
                return;
            }
            else if (MoveNumber == GameSize * GameSize)
            {
                Status = GameStatus.Draw;
                CurrentPlayer = CellState.Empty;
            }
        }

        private int CountInDirection(CellState[][] field, CellState player, int row, int col, int rowDir, int colDir)
        {
            int count = 1;

            int r = row + rowDir;
            int c = col + colDir;
            while (IsInsideField(r, c) && field[r][c] == player)
            {
                count++;
                r += rowDir;
                c += colDir;
            }

            r = row - rowDir;
            c = col - colDir;
            while (IsInsideField(r, c) && field[r][c] == player)
            {
                count++;
                r -= rowDir;
                c -= colDir;
            }

            return count;
        }

        private bool IsInsideField(int row, int col)
        {
            return row >= 0 && row < GameSize && col >= 0 && col < GameSize;
        }

    }
}
