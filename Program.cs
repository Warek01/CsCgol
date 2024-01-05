using CsGame.Models;

using var game = new Game
{
  FullscreenOnly  = true,
  ScreenIndex = 1,
  Title       = "Test",
};

game.Start();
