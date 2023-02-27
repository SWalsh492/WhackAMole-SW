using Microsoft.Maui.Converters;
using System.Timers;

namespace WhackAMole_SW;

public partial class MainPage : ContentPage
{
	// Global Variables
	// Variables for countdown timer
	System.Timers.Timer _timer;
	readonly int myInterval = 250;
    int countdownStart = 150;
	// Variables for game grids
	int score = 0;
	double msTime = 1000;
    bool _IsTimerRunning;
    // Variables for node generation
    bool moreBlockers = false;
    readonly Random rng;

    public MainPage()
	{
		InitializeComponent();
		OnStartUp();
        rng = new Random(); // rng is my random number generator
	}

	private void OnStartUp()
	{
        // Displays welcome message, removes clicking anything else
		if (LblWelcome.IsVisible == true)
		{
			BtnChangeGrid.IsVisible = false;
			BtnStart.IsVisible = false;
			BtnLevel.IsVisible = false;
		}

		// Sets up countdown timer
        _timer = new System.Timers.Timer(); // instantiate
        _timer.Interval = myInterval;
        // Adds an event handler and sets the countdown start
        _timer.Elapsed += _timer_Elapsed;
        LblCounter.Text = countdownStart.ToString();

    }

    private void BtnDismissWelcome_Clicked(object sender, EventArgs e)
    {
        // Dismisses the welcome and brings up rest of buttons
        BtnDismissWelcome.IsVisible = false;
        LblWelcome.IsVisible = false;
        BtnChangeGrid.IsVisible = true;
        BtnStart.IsVisible = true;
        BtnLevel.IsVisible = true;
    }

    // Countdown timer controls
    private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        Dispatcher.Dispatch(
            () => { UpdateAfterTimer(); });
    }

    // Ticks down and edits countdown display based on time remaining
    private void UpdateAfterTimer()
    {
        int counter;
        // retrieves from the text on the screen
        counter = Convert.ToInt32(LblCounter.Text);
        counter--;
        LblCounter.Text = counter.ToString();
        if (counter < 75)
        {
            LblCounter.TextColor = Colors.Yellow;
        }
        if (counter < 50)
        {
            LblCounter.TextColor = Colors.Orange;
        }
        if (counter < 25)
        {
            LblCounter.TextColor = Colors.Red;
        }
        if (counter < 10)
        {
            LblCounter.TextColor = Colors.DarkRed;
        }
        if (counter == 0)
        {
            // Displays gameover message
            Gameover.IsVisible = true;
            _timer.Stop();
            // resets button text
            BtnStart.Text = "Start Again?";
        }
    }

    private async void BtnChangeGrid_Clicked(object sender, EventArgs e)
    {
        // Changes visibility on grids, uses text on button

        switch (BtnChangeGrid.Text)
        {
            case "5 x 5": // switch to 5x5
                {
                    GameGrid3x3.IsVisible = false;
                    GameGrid5x5.IsVisible = true;
                    BtnChangeGrid.Text = "3 x 3";
                    break;
                }
            case "3 x 3": // switch to 3x3
                {
                    GameGrid3x3.IsVisible = true;
                    GameGrid5x5.IsVisible = false;
                    BtnChangeGrid.Text = "5 x 5";
                    break;
                }
        }
        if (BtnStart.Text == "Stop")
        {
            bool confirm = await DisplayAlert("End Game", "Are you sure you want to end your current round?", "Yes", "No");

            if(confirm)
            {
                // Reset game
                ResetTheGame();
                BtnStart.Text = "Start";
                _timer.Stop();
            }
        }
    }

    private async void BtnStart_Clicked(object sender, EventArgs e)
    {
        _IsTimerRunning = true;
        // Starts game
        if (BtnStart.Text == "Start" || BtnStart.Text == "Start Again?")
        {
            ResetTheGame();
            _timer.Interval = 500;
            _timer.Start();
            _IsTimerRunning = true;


            // Dispatches StartTimer using milliseconds
            Dispatcher.StartTimer(TimeSpan.FromMilliseconds(msTime),
                () =>
                {
                    Dispatcher.Dispatch(() => { MoveMoles(); }); // Calls method to move moles
                    return _IsTimerRunning;
                });
        }
        else if (BtnStart.Text == "Stop")
        {
            // Alert message, if yes, ends game
            bool confirm = await DisplayAlert("End Game", "Are you sure you want to end your current round?", "Yes", "No");

            if (confirm)
            {
                EndGame();
            }

        }



        

    }

    // Image button methods
    private void Node_Clicked(object sender, EventArgs e)
	{
		// Increases score when clicked and makes image button dissapear

		score += 10;
		LblScore.Text = score.ToString();

		// uses sender object to hide visibility
		ImageButton ib = (ImageButton)sender;
		ib.IsVisible = false;
	}

    private void Blocker_Clicked(object sender, EventArgs e)
    {
        // decreases score when clicked and makes image button disappear
        int score = Convert.ToInt32(LblScore.Text);
        score -= 10;
        LblScore.Text = score.ToString();

        // use sender object to hide visibility
        ImageButton ib = (ImageButton)sender;
        ib.IsVisible = false;
    }

    // Moves moles using random number generator
    private bool MoveMoles()
	{
        int Max;
        ImageButton currentImageButton1, currentImageButton2, currentImageButton3, currentImageButton4, currentImageButton5;
        
        // Assigns Image buttons to moles according to which grid is visible
        if (GameGrid3x3.IsVisible == true)
        {
            Max = 3;
            currentImageButton1 = node3_1st;
            currentImageButton2 = node3_2nd;
			currentImageButton3 = block3_1st;
			currentImageButton4 = block3_2nd;
			currentImageButton5 = block3_3rd;
        }
        else
        {
            Max = 5;
            currentImageButton1 = node5_1st;
            currentImageButton2 = node5_2nd;
            currentImageButton3 = block5_1st;
            currentImageButton4 = block5_2nd;
            currentImageButton5 = block5_3rd;
        }

        // Generates random nums for rows and columns of each mole
        int node1Row, node1Col, node2Row, node2Col, block1Row, block1Col, block2Row, block2Col, block3Row, block3Col;
        node1Row = rng.Next(0, Max);
        node1Col = rng.Next(0, Max);
        node2Row = rng.Next(0, Max);
        node2Col = rng.Next(0, Max);
		block1Row = rng.Next(0, Max);
		block1Col = rng.Next(0, Max);
		block2Row = rng.Next(0, Max);
		block2Col = rng.Next(0, Max);
        block3Row = rng.Next(0, Max);
        block3Col = rng.Next(0, Max);

        // Moves mole image to random position
        currentImageButton1.SetValue(Grid.RowProperty, node1Row);
        currentImageButton1.SetValue(Grid.ColumnProperty, node1Col);
        // Make mole visible
        currentImageButton1.IsVisible = true;

        // Repeat for second Mole
        currentImageButton2.SetValue(Grid.RowProperty, node2Row);
        currentImageButton2.SetValue(Grid.ColumnProperty, node2Col);
        currentImageButton2.IsVisible = true;
			
		currentImageButton3.SetValue(Grid.RowProperty, block1Row);
        currentImageButton3.SetValue(Grid.ColumnProperty, block1Col);
        currentImageButton3.IsVisible = true;

        // Based on BtnLevel. If hard difficulty is selected, display more blockers
		if(moreBlockers == true)
		{
            currentImageButton4.SetValue(Grid.RowProperty, block2Row);
            currentImageButton4.SetValue(Grid.ColumnProperty, block2Col);
            currentImageButton4.IsVisible = true;

            currentImageButton5.SetValue(Grid.RowProperty, block3Row);
            currentImageButton5.SetValue(Grid.ColumnProperty, block3Col);
            currentImageButton5.IsVisible = true;
        }
		else
		{
			currentImageButton4.IsVisible = false;
			currentImageButton5.IsVisible = false;
		}


        if(BtnStart.Text == "Start" || BtnStart.Text == "Start Again?")
        {
            // halt and hide moles when game is stopped
            currentImageButton1.SetValue(Grid.RowProperty, 0);
            currentImageButton1.SetValue(Grid.ColumnProperty, 0);
            currentImageButton2.SetValue(Grid.RowProperty, 0);
            currentImageButton2.SetValue(Grid.ColumnProperty, 0);
            currentImageButton3.SetValue(Grid.RowProperty, 0);
            currentImageButton3.SetValue(Grid.ColumnProperty, 0);
            currentImageButton4.SetValue(Grid.RowProperty, 0);
            currentImageButton4.SetValue(Grid.ColumnProperty, 0);
            currentImageButton5.SetValue(Grid.RowProperty, 0);
			currentImageButton5.SetValue(Grid.ColumnProperty, 0);

            currentImageButton1.IsVisible = false;
            currentImageButton2.IsVisible = false;
            currentImageButton3.IsVisible = false;
            currentImageButton4.IsVisible = false;
            currentImageButton5.IsVisible = false;

			return _IsTimerRunning = false;
        }
		return _IsTimerRunning = true;
    }

    // Resets game
	private void ResetTheGame()
    {
        // Resets variables, buttons, countdown and score text,
        // NOTE: Does not end game, just resets elements
        LblScore.Text = "0";
        score = 0;
        LblCounter.Text = countdownStart.ToString();
        LblCounter.TextColor = Colors.White;
        BtnStart.Text = "Stop";
        Gameover.IsVisible = false; // Hides gameover image
    }

    private void EndGame()
    {
        //Calls reset and ends ongoing game entirely
        ResetTheGame();
        BtnStart.Text = "Start";
        _timer.Stop();
        _IsTimerRunning = false;
    }

    // Difficulty Changer
    private void BtnLevel_Clicked(object sender, EventArgs e)
	{
        // resets board
        ResetTheGame();
        BtnStart.Text = "Start";
        _timer.Stop();
        _IsTimerRunning = false;

        if (BtnLevel.Text == "Easy")
		{
			BtnLevel.Text = "Hard";

			msTime = 1000;
			countdownStart = 150;
            LblCounter.Text = countdownStart.ToString();
			moreBlockers = false;
        }
        else
		{
			BtnLevel.Text = "Easy";
			msTime = 750;
			countdownStart = 80;
            LblCounter.Text = countdownStart.ToString();
			moreBlockers = true; // True = 2 more blockers are used in MoveMoles()
        }
    }
}

