In the Switch case of the MonogameBlock class, I set the variable blockstate to the current state of BlockState.
Not doing this led to blocks not being removed since the if statement in UpdateCheckBlocksForCollision method of
BlockManager class is calling for the state of the block. This led to the prevention of a bug where the ball would
travel sideways and only traveling horizontally.

I added a new class called LevelManager. This class stores the enum level, where the state of level changes between
level 1, level 2, or level 3 depending if the player has beaten the previous level. Using the switch case, I set 
each level to have a different amount of height relative to the level.

In the method UpdateCheckBlocksForCollision of BlockManager class, the if statement if(b.BlockState == BlockState.Broken)
was changed to if(b.BlockState == BlockState.Hit). This was because if b.BlockState == BlockState.Broken remained, then it
it would require a another intersect for the block to be removed. So changing it to b.BlockState == BlockState.Hit means
that once the ball intersects a block that has already been hit then it is removed. 

In BlockManager I added a reference to class LevelManager called lvl. With this, I changed the parameters of the 
CreateBlockArrayByWidthAndHeight() call in Loadlevel(). Changing from numbers to lvl.width, lvl.height, lvl.margin
values that were declared in the switch case in LevelManager. This way, when levels change, so do the the amount of blocks

In method UpdateRemoveDisabledBlocks() I changed ScoreManager.Score++ to ScoreManager.Score+=100. This makes scores more
appealing that low digit numbers of ScoreManager.Score++

Created a new method in BlockManager class called ReloadBlocks. This method is to create new blocks whenever the level is
complete. So in the method, there is an if statement (Blocks.Count <=0) This reads if the number of blocks present is 0.
Then another if-statement(!isUpdated), isUpdated is a bool that keeps the ball from hitting new blocks when they're loaded.
So, if-statement(!isUpdated) holds a method call to Update in LevelManager class to increase the level. Then method
call ball.BallReset() to reset the ball onto the block. Then finally the set isUpdated to true, so that this ifStatement
doesnt get called again. Then after that if another if-statement(ReadyPlayer()) the ifstatement calls for a bool method
that requires the player to press space to continue. Once the player hits the space key, then the Loadlevel method is called
and reloads the blocks of the new level.

In Paddles class, I reset the Location of the paddle to new Vector2(this.Game.GraphicsDevice.Viewport.Width/2, this.Game.GraphicsDevice.Viewport.Height - (this.spriteTexture.Height*2))
This way, the paddle will always be set in the middle lower half of the game screen regardless of the size of the screen.

In the Game 1 class, the following changes were made:
-	LevelManager was added with a reference
-	public enum GameState was created with values Instruction, GamePlay, GameOver, Win.
-	public static GameState state is created in order to be used for launching the ball.
-	Loaded a SpriteFont called text to display text of the Instruction, Win, and GameOver GameStates
-	Method UpdateGame() is created and called for in Update. This method is set to change GameState depending on the
	situation of the game. First, if-statement(input.KeyboardState.IsKeyDown(Keys.Space) && gameState == GameState.Instruction)
	is made and in the code block field gameState = GameState.GamePlay and state = GameState.Gameplay. This code was made
	to change the GameState when the game starts into GameState.Instruction and the player presses space key.
	Another if-statement ScoreManager.Lives <= 0 was made to set the state of the game to GameOver when the lives
	of the player hits 0. Another if-Statement (lvl.lvl == level.Complete) is used to change the GameState to
	the Win state of the game. Essentially when the player beats all three levels. Another if-statement
	if (input.KeyboardState.IsKeyDown(Keys.Space) && (gameState == GameState.GameOver || gameState == GameState.Win))
	to reset the game's Lives, Score, and restart the level back to 1 when the player hits the space key and the
	GameState is either in GameOver or Win
-	In the Draw method, a Switch was made to get into different Draw methods from other classes depending on the
	GameState. Instruction case brings up how to play the game. GamePlay is the actual Breakout Game. GameOver is
	Game Over text. Win case is the Win text.
	
In the PaddleController, I adjusted the if-statement(ball.State == BallState.OnPaddleStart) to be 
if (ball.State == BallState.OnPaddleStart && Game1.state == GameState.GamePlay). This way the ball has to be on the
paddles and the GameState has to be on GamePlay in order to launch.

In the Ball method:
-	I added a Random instance and used it in the LaunchBall method. This is so that the declared variable launchRand
	gets a random (float) value between -3 and 3. From there a new method setBallSpeed() was made to recalibrate the
	speed of the ball depending on the value of launchRand. Then, finally setting the Direction of the ball to
	new Vector2(launchRand, setVertLaunch). This way, the ball will randomly in any direction horizontally while remain
	traveling vertically. This better than setting it as new Vector(1,-1) where the ball will always travel in the same
	direction when launched.
-	I reset the Location of the paddle to new Vector2(this.Game.GraphicsDevice.Viewport.Width/2, this.Game.GraphicsDevice.Viewport.Height - (this.spriteTexture.Height*2))
	This way, the ball will always be set in the middle lower half of the game screen regardless of the size of the screen with the paddle.
-	Reflect Method was slightly tweaked. It now has some collision detection where if the ball hits the bottom of the block, the ball goes down.
	If the ball hits the top of the block, it goes up. However, the side collisions do not work as intended. They ball will still go through
	the block causing a double intersect and destroying the block instantly.
