using SudokuApp.Services;
using Mopups.Services;
using Plugin.MauiMTAdmob;
using SudokuApp.Views;
using SudokuApp.Services.Audio;
using SudokuApp.Services.Vibrations;
using Plugin.MauiMTAdmob.Controls;
using SudokuApp.ViewModels;


namespace SudokuApp
{
    public partial class GamePage : ContentPage
    {
        private SudokuGenerator sudokuGenerator { get; set; }
        private string AdsInterstitial { get; set; }
        private MTAdView AdsBanner { get; set; }

        private readonly string fileUserSudoku = "usersudoku.json";
        private readonly string fileSolvedSudoku = "solvedsudoku.json";
        private readonly string fileRemovedNumberSudoku = "removednumberssudoku.json";

        private int Row { get; set; }
        private int Col { get; set; }
        private int[,] SudokuBoard { get; set; } //Sudoku board with removed value(inserted -1), called as UserSudokuBoard
        private int[,] SudokuBoardOryg { get; set; } //Solved Sudoku Board

        private Label CurrentLabel { get; set; } //currently selected label

        private int Mistakes { get; set; } //the number of errors made in the current game

        private readonly PlayerMove playerMove = new();

        private CountTimerUp TimerProperty { get; set; }
        private bool NewGame { get; set; }
        private int Level { get; set; }

        private TranslationManager translate { get; set;}
        private string language;

        UserStatistics userStatistics { get; set; }
        SaveUserStatistics saveUserStatistics { get; set; }

        public GamePage(int level, bool newgame, MTAdView adsBanner)
        {//new game = true, continue game = false
            _ = Task.Run(() =>
            {
                AdsBanner = adsBanner;
                if (!Preferences.ContainsKey("MistakesAvilable"))
                    Preferences.Default.Set("MistakesAvilable", 3);

                AdsInterstitial = Preferences.Default.Get("AdsGoogleInterstitial", "");
            });

            userStatistics = new()
            {
                Level = level switch
                {
                    1 => "Easy",
                    2 => "Medium",
                    3 => "Hard",
                    4 => "Expert",
                    _ => Preferences.Default.Get("GameLevelName", ""),
                }
            };
            saveUserStatistics = new();

            NewGame = newgame;
            Level = level;

            //_ = Task.Run(() => // We run the task in another thread
            //{
                SaveCurrentGame currentSave = new();
                
                if (newgame)
                {                
                    sudokuGenerator = new SudokuGenerator(level);
                    SudokuBoard = sudokuGenerator.GetBoard();
                    SudokuBoardOryg = sudokuGenerator.GetBoardOryg();
                    currentSave.SaveSudokuBoard(SudokuBoard, $"{fileRemovedNumberSudoku}");
                    currentSave.SaveSudokuBoard(SudokuBoardOryg, $"{fileSolvedSudoku}");
                    
                    if (Preferences.Get("HintAvilable", 0) == 0)
                        Preferences.Default.Set("HintAvilable", 1);

                    Preferences.Default.Set("MakesMistakes", 0);
                    //while (userStatistics == null) ;
                    userStatistics.GameStarted = 1;
                    //while (saveUserStatistics == null) ;
                    saveUserStatistics.SaveUserStatisticsFile(userStatistics);
                }
                else
                { // reading the current game
                    SudokuBoardOryg = currentSave.LoadSudokuBoard($"{fileSolvedSudoku}");
                    switch (level)
                    {
                        case 5: //restart, i.e. the original board with the numbers removed
                            SudokuBoard = currentSave.LoadSudokuBoard($"{fileRemovedNumberSudoku}");
                            Preferences.Default.Set("MakesMistakes", 0);
                            Preferences.Default.Set("SetCountTime", 0);
                            //while (userStatistics == null) ;
                            userStatistics.GameStarted = 1;
                            //while (saveUserStatistics == null) ;
                            saveUserStatistics.SaveUserStatisticsFile(userStatistics);
                            break;
                        case 6: //continue the user's saved game
                            SudokuBoard = currentSave.LoadSudokuBoard($"{fileUserSudoku}");
                            break;
                    }
                }
            //});
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            // Zwróæ true, aby uniemo¿liwiæ domyœln¹ akcjê przycisku powrotu
            return base.OnBackButtonPressed();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            CurrentGameSave(); //save current game
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //_ = Task.Run(() =>
            //{
                if (StackReklama.Children.Count == 0)
                    StackReklama.Children.Add(AdsBanner);
            //});
            
            if (Preferences.Default.Get("OpenSettings", "") == "False")
            {
                //MainThread.InvokeOnMainThreadAsync(GenerateSudokuGrid);
                Task.Run(GenerateSudokuGrid);
            }
            InitializePage();
        }

        private void InitializePage()
        {
            language = Preferences.Default.Get("CurrentLanguage", "polski");
            translate = new();

            _ = Task.Run(() =>
            {
                switch (Preferences.Default.Get("TimerSwitch", ""))
                {
                    case "True":
                        ImagePlayStop.IsVisible = true;
                        LblTime.IsVisible = true;
                        break;
                    case "False":
                        ImagePlayStop.IsVisible = false;
                        LblTime.IsVisible = false;
                        break;
                }
                switch (Preferences.Default.Get("MistakesSwitch", ""))
                {
                    case "True":
                        LblMistake.IsVisible = true;
                        break;
                    case "False":
                        LblMistake.IsVisible = false;
                        break;
                }
                
                LblLevel.Text = Preferences.Get("GameLevelName", "") switch
                {
                    "Easy" => translate.Translate(language, "LabelLevel1"),
                    "Medium" => translate.Translate(language, "LabelLevel2"),
                    "Hard" => translate.Translate(language, "LabelLevel3"),
                    "Expert" => translate.Translate(language, "LabelLevel4"),
                    _ => ""
                };

                //warunek daj zero Mistakes je¿eli user uruchamia ponownie(start od zera) planszê lub wywo³uje now¹ grê
                if (Level < 6)
                    Preferences.Default.Set("MakesMistakes", 0);

                Mistakes = Preferences.Default.Get("MakesMistakes", 0);
                
                LblMistake.Text = $"{translate.Translate(language, "LabelMistake")} " +
                $"{Mistakes}/{Preferences.Default.Get("MistakesAvilable", 0)}";

                if (CurrentLabel != null)
                    OnLabelTapped(CurrentLabel);
            });

            _ = MainThread.InvokeOnMainThreadAsync(() => //MainThread.InvokeOnMainThreadAsync
            {
                int hint = Preferences.Default.Get("HintAvilable", 0);
                switch (hint)
                {
                    case 0:
                        lblHint.IsVisible = false;
                        imageGetHint.IsVisible = true;
                        break;
                    default:
                        lblHint.IsVisible = true;
                        imageGetHint.IsVisible = false;
                        break;
                }
                foreach (View view in gridImage.Children.Cast<View>())
                {
                    if (view is Label label)
                    {
                        label.Text = label.ClassId switch
                        {
                            "GetHint" => $"Free x {hint}",
                            _ => translate.Translate(language, label.ClassId),
                        };
                    }
                }
            });

            _ = Task.Run(async() =>
            {
                await Task.Delay(1500);
                switch  (NewGame)
                {
                    case true:
                        TimerProperty = new CountTimerUp(0); // Set the countup time in seconds
                        TimerProperty.CountdownTick += OnCountdownTick;
                        NewGame = false;
                        TimerProperty?.Start();   
                        break;
                    default:
                        if (Preferences.Default.Get("OpenSettings", "") == "False")
                        {
                            TimerProperty = new CountTimerUp(Preferences.Default.Get("SetCountTime", 0));
                            TimerProperty.CountdownTick += OnCountdownTick; // Set the countup time in seconds
                            TimerProperty?.Start();    
                        }
                        break;
                }
            });
        }

        private void CurrentGameSave()
        {
            if ((bool)!TimerProperty?.IsPaused)
                PlayPause_Tapped(null, null);
            Task.Run(() => // zapisanie aktualnej gry
            {
                SaveCurrentGame currentSave = new();

                foreach (View childView in sudokuBoardXaml.Children.Cast<View>())
                {
                    if (childView is Frame frame)
                    {
                        if (frame.Content is Grid grid)
                        {
                            if (grid[0] is Label label)
                            {
                                // Tutaj masz dostêp do Label
                                if (label.BindingContext is GridCoordinates coordinates)
                                {
                                    if (!int.TryParse(label.Text, out int parsedValue))
                                    {
                                        parsedValue = -1;
                                    }
                                    currentSave.BoardUser[coordinates.RowBoardSudoku, coordinates.ColumnBoardSudoku] = parsedValue;
                                }
                            }
                        }
                    }
                }
                currentSave.SaveSudokuBoard(currentSave.BoardUser, $"{fileUserSudoku}");
            });
        }

        private void AddNotesGrid()
        {
            _ = Task.Run(() =>
            {
                while (sudokuBoardXaml.Children.Count < 89) ;
                foreach (var child in sudokuBoardXaml.Children)
                {
                    if (child is Frame frame && frame.Content is Grid grid &&
                        grid.Children[0] is Label label && label.Text == "")
                    {
                        var parent = (Grid)label.Parent;
                        var notesGrid = new Grid
                        {
                            Style = (Style)Application.Current.Resources["NotesGrid"],
                            RowDefinitions =
                            {
                                new RowDefinition { Height = GridLength.Star },
                                new RowDefinition { Height = GridLength.Star },
                                new RowDefinition { Height = GridLength.Star }
                            },
                            ColumnDefinitions =
                            {
                                new ColumnDefinition { Width = GridLength.Star },
                                new ColumnDefinition { Width = GridLength.Star },
                                new ColumnDefinition { Width = GridLength.Star }
                            }
                        };

                        for (int row = 0; row < 3; row++)
                        {
                            for (int col = 0; col < 3; col++)
                            {
                                var noteLabel = new Label
                                {
                                    Text = ((row * 3) + col + 1).ToString(),
                                    ClassId = $"{(row * 3) + col + 1}",
                                    Style = (Style)Application.Current.Resources["LabelNotesGrid"],
                                    IsVisible = false
                                };
                                notesGrid.Children.Add(noteLabel);
                                Grid.SetRow(noteLabel, row);
                                Grid.SetColumn(noteLabel, col);
                            }
                        }

                        parent.Children.Add(notesGrid);
                        Grid.SetRow(notesGrid, 0);
                        Grid.SetColumn(notesGrid, 0);
                    }
                }
            });
        }

        private async void GenerateSudokuGrid()
        {
            var tcs = new TaskCompletionSource<bool>();

            await MainThread.InvokeOnMainThreadAsync(() => //MainThread.InvokeOnMainThreadAsync
            {
                // Dodaj linie poziome i pionowe
                for (int i = 0; i < 13; i++)
                {
                    if (i == 0 || i == 4 || i == 8 || i == 12)
                    {
                        sudokuBoardXaml.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Absolute) });
                        sudokuBoardXaml.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                        BoxView horizontalLine = new BoxView { HeightRequest = 2 };
                        sudokuBoardXaml.Children.Add(horizontalLine);
                        Grid.SetRow(horizontalLine, i);
                        Grid.SetColumn(horizontalLine, 0);
                        Grid.SetColumnSpan(horizontalLine, 13);

                        BoxView verticalLine = new BoxView { WidthRequest = 2 };
                        sudokuBoardXaml.Children.Add(verticalLine);
                        Grid.SetRow(verticalLine, 0);
                        Grid.SetRowSpan(verticalLine, 13);
                        Grid.SetColumn(verticalLine, i);
                    }
                    else
                    {
                        sudokuBoardXaml.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                        sudokuBoardXaml.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    }
                }

                int sudokuBoardRow = 0;
                for (int row = 0; row < 13; row++)
                {
                    if (row != 0 && row != 4 && row != 8 && row != 12)
                    {
                        int sudokuBoardCol = 0;
                        for (int col = 0; col < 13; col++)
                        {
                            // Ustaw wartoœæ na planszy sudoku
                            if (col != 0 && col != 4 && col != 8 && col != 12)
                            {
                                Frame frame = new()
                                {
                                    Style = (Style)Application.Current.Resources["FrameStyle"]
                                };

                                int value = SudokuBoard[sudokuBoardRow, sudokuBoardCol];
                                //pole tekstowe (TextBox) na podstawie wartoœci
                                Label label = new();
                                label = new Label
                                {
                                    GestureRecognizers = { new TapGestureRecognizer { Command = new Command(() => OnLabelTapped(label)) } },
                                    // Przypisz wspó³rzêdne do BindingContext
                                    BindingContext = new GridCoordinates
                                    {
                                        ColumnSudokuBoardXaml = col,
                                        RowSudokuBoardXaml = row,
                                        ColumnBoardSudoku = sudokuBoardCol,
                                        RowBoardSudoku = sudokuBoardRow
                                    },
                                    Text = value == -1 ? "" : value.ToString()
                                };
                                var coordinates = (GridCoordinates)label.BindingContext;
                                if (SudokuBoardOryg[coordinates.RowBoardSudoku, coordinates.ColumnBoardSudoku]
                                    != value && value != -1)
                                {
                                    label.Style = (Style)Application.Current.Resources["WrongLabelGrid"];
                                }
                                else
                                {
                                    label.Style = (Style)Application.Current.Resources["LabelGrid"];
                                }

                                var containerGrid = new Grid
                                {
                                    Style = (Style)Application.Current.Resources["ContainerGrid"]
                                };
                                containerGrid.Children.Add(label);

                                frame.Content = containerGrid;

                                sudokuBoardXaml.Children.Add(frame); //, col, row
                                Grid.SetRow(frame, row);
                                Grid.SetColumn(frame, col);
                                sudokuBoardCol++;
                            }
                        }
                    }
                    if (row != 0 && row != 4 && row != 8 && row != 12)
                    {
                        sudokuBoardRow++;
                    }
                }
                tcs.SetResult(true);
            });

            await tcs.Task; // czekaj na zakoñczenie operacji

            _ = Task.Run(() =>
            {
                while (sudokuBoardXaml.Children.Count < 89) ;
                Frame frameTemp = (Frame)sudokuBoardXaml.Children[8];
                Grid views = (Grid)frameTemp.Content;
                OnLabelTapped((Label)views[0]);
            });
            _ = Task.Run(AddNotesGrid);
        }

        private async void OnGenerateSudokuClicked(object sender, EventArgs e)
        {
            AdsPopupPage popup = new();
            popup.Disappearing += Popup_Closed;
            await MopupService.Instance.PushAsync(popup);
            //await Navigation.PushAsync(popup);
        }

        private void Popup_Closed(object sender, EventArgs e)
        {
            TapSound.PlaySound();
            InitializePage();
        }

        private void OnLabelTapped(Label tappedLabel)
        {
            CurrentLabel = tappedLabel;
            // Pobierz wspó³rzêdne z BindingContext
            if (tappedLabel.BindingContext is GridCoordinates coordinates)
            {
                Row = coordinates.RowSudokuBoardXaml;
                Col = coordinates.ColumnSudokuBoardXaml;
                //wyczyœæ background wszystkich Label
                foreach (View child in sudokuBoardXaml.Children.Cast<View>())
                {
                    if (child is Frame frame)
                    {
                        if (frame.Content is Grid grid)
                        {
                            if (grid[0] is Label label)
                            {
                                if (label.BindingContext is GridCoordinates coordinates1)
                                {
                                    if (coordinates1.RowSudokuBoardXaml == Row && coordinates1.ColumnSudokuBoardXaml == Col) //kliknite pole zaznacz innym kolorem
                                    {
                                        label.BackgroundColor = Color.FromArgb("#D3D3D3");
                                    }
                                    else
                                    {
                                        label.BackgroundColor = Color.FromArgb("#00FFFFFF");
                                    }
                                }
                            }
                        }
                    }
                }

                // Zaznacz wszystkie pola w kwadracie 3x3, oraz kolumnê i wiersz
                if (Preferences.Default.Get("HighlightAreas", "") == "True")
                {
                    int[,] squareCoordinates = new int[,]
                    {
                        {1, 1, 3, 3}, {1, 5, 3, 7}, {1, 9, 3, 11},
                        {5, 1, 7 ,3}, {5, 5, 7, 7}, {5, 9, 7, 11},
                        {9, 1, 11, 3}, {9, 5, 11, 7}, {9, 9, 11, 11}
                    };

                    // ZnajdŸ odpowiadaj¹ce koordynaty dla klikniêtego pola
                    int startRowSquare = -1;
                    int startColSquare = -1;

                    for (int i = 0; i < squareCoordinates.GetLength(0); i++)
                    {
                        int squareStartRow = squareCoordinates[i, 0];
                        int squareStartCol = squareCoordinates[i, 1];
                        int squareEndRow = squareCoordinates[i, 2];
                        int squareEndCol = squareCoordinates[i, 3];

                        if (Row >= squareStartRow && Row <= squareEndRow && Col >= squareStartCol && Col <= squareEndCol)
                        {
                            startRowSquare = squareStartRow;
                            startColSquare = squareStartCol;
                            break;
                        }
                    }
                    // Zaznacz wszystkie pola w kwadracie 3x3
                    if (startRowSquare != -1 && startColSquare != -1)
                    {
                        for (int i = startRowSquare; i <= startRowSquare + 2; i++)
                        {
                            for (int j = startColSquare; j <= startColSquare + 2; j++)
                            {
                                GetLabelAt(i, j, Color.FromArgb("#808080")); // Ustaw preferowany kolor t³a
                            }
                        }
                    }

                    // zaznacz w danym wierszu/kolumnie wszystkie pola
                    foreach (View child in sudokuBoardXaml.Children.Cast<View>())
                    {
                        if (child is Frame frame)
                        {
                            if (frame.Content is Grid grid)
                            {
                                if (grid[0] is Label label)
                                {
                                    if (label.BindingContext is GridCoordinates coordinates1)
                                    {
                                        if (coordinates1.RowSudokuBoardXaml == Row) //zaznacz wszystkie pola w wierszu
                                        {
                                            label.BackgroundColor = Color.FromArgb("#808080");
                                        }
                                        if (coordinates1.ColumnSudokuBoardXaml == Col) //zaznacz wszystkie pola w kolumnie
                                        {
                                            label.BackgroundColor = Color.FromArgb("#808080");
                                        }
                                        if (coordinates1.RowSudokuBoardXaml == Row && coordinates1.ColumnSudokuBoardXaml == Col) //kliknite pole zaznacz innym kolorem
                                        {
                                            label.BackgroundColor = Color.FromArgb("#D3D3D3");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                //zaznacz te same cyfry z planszy, je¿eli u¿ytkownik w³¹czy³ t¹ opcjê, domyœlnie true
                if (Preferences.Default.Get("HighlightIdenticalNumber", "") == "True")
                {
                    foreach (View child in sudokuBoardXaml.Children.Cast<View>())
                    {
                        if (child is Frame frame)
                        {
                            if (frame.Content is Grid grid)
                            {
                                if (grid[0] is Label label)
                                {

                                    if (tappedLabel.Text != "" && tappedLabel.Text == label.Text)
                                    {
                                        label.BackgroundColor = Color.FromArgb("#ADD8E6");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            TapSound.PlaySound();
            TapVibrations.PlayVibrations();
        }

        private void GetLabelAt(int row, int col, string lblText)
        {
            foreach (View child in sudokuBoardXaml.Children.Cast<View>())
            {
                if (child is Frame frame)
                {
                    if (frame.Content is Grid grid)
                    {
                        if (grid[0] is Label label)
                        {
                            if (label.BindingContext is GridCoordinates coordinates)
                            {
                                if (coordinates.RowSudokuBoardXaml == row && coordinates.ColumnSudokuBoardXaml == col)
                                {
                                    label.Text = lblText;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void GetLabelAt(int row, int col, Color lblColor)
        {
            // Pobierz Label na danej pozycji w siatce
            foreach (View child in sudokuBoardXaml.Children.Cast<View>())
            {
                if (child is Frame frame)
                {
                    if (frame.Content is Grid grid)
                    {
                        if (grid[0] is Label label)
                        {
                            if (label.BindingContext is GridCoordinates coordinates)
                            {
                                if (coordinates.RowSudokuBoardXaml == row && coordinates.ColumnSudokuBoardXaml == col)
                                {
                                    if (lblColor != Color.FromArgb("#00FFFFFF"))
                                        label.BackgroundColor = lblColor;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            TapSound.PlaySound();
            await Navigation.PushModalAsync(new SettingsPage(AdsBanner), true);
        }

        private void ShowNotes(Button button)
        {
            if (CurrentLabel.Text == "")
            {
                Grid gridParent = (Grid)CurrentLabel.Parent;
                foreach (var firstChild in gridParent.Children)
                {
                    if (firstChild is Grid secondChild)
                    {
                        foreach (var gridChildChild in secondChild.Children)
                        {
                            if (gridChildChild is Label label)
                            {
                                if (label.ClassId == button.ClassId)
                                {
                                    label.IsVisible = label.IsVisible != true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void EraseNotes()
        {
            if (CurrentLabel.Text == "")
            {
                Grid gridParent = (Grid)CurrentLabel.Parent;
                foreach (var firstChild in gridParent.Children)
                {
                    if (firstChild is Grid secondChild)
                    {
                        foreach (var gridChildChild in secondChild.Children)
                        {
                            if (gridChildChild is Label label)
                            {
                                label.IsVisible = false;
                            }
                        }
                    }
                }
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            TapSound.PlaySound();
            TapVibrations.PlayVibrations();
            if (sender is Button button)
            {
                var coordinates = (GridCoordinates)CurrentLabel.BindingContext;

                if (Preferences.Default.Get("EnableNotes", "") == "True") //notes enabled
                {
                    ShowNotes(button); //poka¿ notatki
                }
                else //standard numbers add to sudoku
                {
                    EraseNotes();
                    if (SudokuBoard[coordinates.RowBoardSudoku, coordinates.ColumnBoardSudoku] !=
                        SudokuBoardOryg[coordinates.RowBoardSudoku, coordinates.ColumnBoardSudoku])
                    {
                        //save last move
                        if (!int.TryParse(CurrentLabel.Text, out int parsedValue))
                        {
                            parsedValue = -1;
                        }

                        playerMove.SaveMove(coordinates.RowSudokuBoardXaml, coordinates.ColumnSudokuBoardXaml, coordinates.RowBoardSudoku,
                        coordinates.ColumnBoardSudoku, parsedValue, int.Parse(button.ClassId));
                        //save last move

                        CurrentLabel.Text = button.ClassId.ToString();

                        if (SudokuBoardOryg[coordinates.RowBoardSudoku, coordinates.ColumnBoardSudoku] != int.Parse(CurrentLabel.Text))
                        {
                            CurrentLabel.Style = (Style)Application.Current.Resources["WrongLabelGrid"];

                            if (Preferences.Default.Get("MistakesSwitch", "") == "True")
                            {
                                Mistakes++;
                                if (Mistakes >= 3)
                                {
                                    await ShowAlertDialog(); //wyœwietl alert o reklamie
                                }
                                Preferences.Default.Set("MakesMistakes", Mistakes);
                                LblMistake.Text = $"{translate.Translate(language, "LabelMistake")} " +
                                    $"{Mistakes}/{Preferences.Default.Get("MistakesAvilable", 0)}";
                            }
                        }
                        else
                        {
                            CurrentLabel.Style = (Style)Application.Current.Resources["LabelGrid"];
                            if (Preferences.Default.Get("HighlightIdenticalNumber", "") == "True")
                                OnLabelTapped(CurrentLabel);
                        }
                    }

                    if (CheckSudokuBoard()) //sprawdzam, czy user rozwi¹za³ Sudoku 
                    {
                        gridImage.IsEnabled = false;
                        sudokuBoardXaml.IsEnabled = false;
                        TimerProperty.Stop();
                        _ = Task.Run(() =>
                        {
                            saveUserStatistics.SaveUserStatisticsFile(userStatistics);
                        });
                        string body = translate.Translate(language, "DisplayAlertFinalText");
                        var customDialog = new CustomDialogGameFinalText($"Wow!!!", $"{body}", $"OK");
                        await MopupService.Instance.PushAsync(customDialog, true);

                        //await DisplayAlert("Wow!!!", $"{translate.Translate(language, "DisplayAlertFinalText")}", "OK");
                    }
                }
            }
        }

        private async Task ShowAlertDialog()
        {
            CrossMauiMTAdmob.Current.LoadInterstitial($"{AdsInterstitial}");
            string title = translate.Translate(language, "AdTitleText");
            string body = translate.Translate(language, "AdBodyText");
            string yes = translate.Translate(language, "TextYes");
            string no = translate.Translate(language, "TextNo");

            var customDialog = new CustomDialogGame($"{title}", $"{body}", $"{yes}", $"{no}");
            await MopupService.Instance.PushAsync(customDialog, true);

            // Oczekiwanie na zamkniêcie okna dialogowego
            var result = await customDialog.WaitForDialogResultAsync();
            //bool result = await DisplayAlert($"{title}", $"{body}", $"{yes}", $"{no}");

            if (result)
            {
                // Poczekaj na za³adowanie reklamy
                while (!CrossMauiMTAdmob.Current.IsInterstitialLoaded())
                {
                    await Task.Delay(50); // Poczekaj 50 ms przed sprawdzeniem ponownie
                }

                // Wyœwietl reklamê po jej za³adowaniu
                CrossMauiMTAdmob.Current.ShowInterstitial();

                Mistakes = 2;
                Preferences.Default.Set("MakesMistakes", Mistakes);
                LblMistake.Text = $"{translate.Translate(language, "LabelMistake")} " +
                                    $"{Mistakes}/{Preferences.Default.Get("MistakesAvilable", 0)}";
            }
            else
            {
                gridImage.IsEnabled = false;
            }
            while (MopupService.Instance.PopupStack.Count > 0)
            {
                await MopupService.Instance.PopAsync();
            };
        }

        private async void Image_Tapped(object sender, EventArgs e)
        {
            TapSound.PlaySound();
            if (sender is Label label)
            {
                if (label.ClassId == "Notes")
                    sender = btnNotes;
                if (label.ClassId == "GetHint")
                    sender = btnHint;
            }

            if (sender is Image image)
            {
                if (image.ClassId == "GetHint")
                    image = btnHint;

                var coordinates = (GridCoordinates)CurrentLabel.BindingContext;
                switch (image.ClassId.ToString())
                {
                    case "1": //Back
                        {
                            var lastmove = playerMove.UndoLastMoves();
                            if (lastmove != null)
                            {
                                string previousValue = lastmove.PreviousValue == -1 ? previousValue = string.Empty :
                                    previousValue = lastmove.PreviousValue.ToString();

                                GetLabelAt(lastmove.RowSudokuBoardXaml, lastmove.ColumnSudokuBoardXaml,
                                    previousValue);
                            }
                            break;
                        }
                    case "2": //Erase
                        {
                            if (SudokuBoard[coordinates.RowBoardSudoku, coordinates.ColumnBoardSudoku] !=
                                    SudokuBoardOryg[coordinates.RowBoardSudoku, coordinates.ColumnBoardSudoku])
                            {
                                CurrentLabel.Text = "";
                                CurrentLabel.BackgroundColor = Color.FromArgb("#D3D3D3");
                            }
                            EraseNotes();
                            break;
                        }
                    case "3": //Hint
                        {
                            if (Preferences.Default.Get("HintAvilable", 0).ToString() != "0")
                            {
                                if (SudokuBoard[coordinates.RowBoardSudoku, coordinates.ColumnBoardSudoku] == -1)
                                {
                                    EraseNotes(); //hide all notes in CurrentLabel
                                    CurrentLabel.Text = $"{SudokuBoardOryg[coordinates.RowBoardSudoku, coordinates.ColumnBoardSudoku]}";
                                    CurrentLabel.Style = (Style)Application.Current.Resources["LabelGrid"];
                                    Preferences.Default.Set("HintAvilable", Preferences.Default.Get("HintAvilable", 0) - 1);

                                    if (Preferences.Default.Get("HintAvilable", 0).ToString() == "0")
                                    {
                                        lblHint.IsVisible = false;
                                        imageGetHint.IsVisible = true;
                                    }
                                    lblHint.Text = $"Free x {Preferences.Default.Get("HintAvilable", 0)}";
                                }
                            }
                            else
                            {
                                if (MopupService.Instance.PopupStack.Count == 0)
                                {
                                    var popup = new AdsPopupPage();
                                    popup.Disappearing += (sender, args) =>
                                    {
                                        // Po zamkniêciu Popup'u, wywo³aj InitializePage na stronie GamePage
                                        InitializePage();
                                    };
                                    await MopupService.Instance.PushAsync(popup);
                                }
                            }
                            break;
                        }
                    case "4": //W³¹cz notatki - Notes
                        {
                            if (Preferences.Default.Get("EnableNotes", "") == "True")
                            {
                                Preferences.Default.Set("EnableNotes", "False");
                                lblNotes.Text = translate.Translate(language, "LabelNotesOff");
                            }
                            else
                            {
                                Preferences.Default.Set("EnableNotes", "True");
                                lblNotes.Text = translate.Translate(language, "LabelNotesOn");
                            }
                            foreach (View child in gridButtons.Children.Cast<View>())
                            {
                                if (child is Button button)
                                {
                                    if (Preferences.Default.Get("EnableNotes", "") == "True")
                                    {
                                        button.Style = (Style)Application.Current.Resources["ButtonStyleNotes"];
                                    }
                                    else
                                    {
                                        button.Style = (Style)Application.Current.Resources["ButtonStyle"];
                                    }
                                }
                            }
                            break;
                        }
                }
            }
        }

        private async void ImageBack_Tapped(object sender, TappedEventArgs e)
        {
            TapSound.PlaySound();
            CurrentGameSave(); //save current game
            // Zamknij bie¿¹c¹ stronê
            while (Navigation.ModalStack.Count > 0)
            {
                await Navigation.PopModalAsync();
            }
        }

        private async void ImageSettings_Tapped(object sender, TappedEventArgs e)
        {
            if ((bool)!TimerProperty?.IsPaused)
                PlayPause_Tapped(null, null);
            TapSound.PlaySound();
            Preferences.Default.Set("OpenSettings", "True");
            CurrentGameSave();
            await Navigation.PushModalAsync(new SettingsPage(AdsBanner), true);
        }

        //Ads Banner load
        //private void MyAds_Loaded(object sender, EventArgs e) => MyAds.LoadAd();

        private void OnCountdownTick(object sender, int countedSeconds)
        {
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                // Convert seconds to hours, minutes, and seconds
                //int hours = countedSeconds / 3600;
                int minutes = countedSeconds % 3600 / 60;
                int seconds = countedSeconds % 60;

                LblTime.Text = string.Format("  {0:D2}:{1:D2}", minutes, seconds); //string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
            });//string.Format("{0:D2}:{1:D2}", minutes, seconds)

            Preferences.Default.Set("SetCountTime", countedSeconds);
        }

        private void PlayPause_Tapped(object sender, TappedEventArgs e)
        {
            ImageSource imageSource;
            if (TimerProperty.IsPaused)
            {
                TimerProperty?.Resume();
                imageSource = Application.Current.RequestedTheme ==
                              AppTheme.Light ? "ic_action_pause_light.png" : "ic_action_pause_dark.png";
                gridImage.IsEnabled = true;
                sudokuBoardXaml.IsEnabled = true;
            }
            else
            {
                TimerProperty?.Pause();
                // Pobierz obrazek na podstawie aktualnego trybu tematu
                imageSource = Application.Current.RequestedTheme ==
                              AppTheme.Light ? "ic_action_play_light.png" : "ic_action_play_dark.png";
                gridImage.IsEnabled = false;
                sudokuBoardXaml.IsEnabled = false;
            }
            // Ustaw nowe Ÿród³o obrazka
            ImagePlayStop.Source = imageSource;
        }

        private bool CheckSudokuBoard()
        {
            foreach (View childView in sudokuBoardXaml.Children.Cast<View>())
            {
                if (childView is Frame frame)
                {
                    if (frame.Content is Grid grid)
                    {
                        if (grid[0] is Label label)
                        {
                            // Tutaj masz dostêp do Label
                            if (label.BindingContext is GridCoordinates coordinates)
                            {
                                if (label.Text != SudokuBoardOryg[coordinates.RowBoardSudoku,
                                    coordinates.ColumnBoardSudoku].ToString())
                                {
                                    return false; // Jeœli znaleziono ró¿nicê, zwróæ false
                                }
                            }
                        }
                    }
                }
            }
            userStatistics.GameWon = 1;
            userStatistics.GameStarted = 0;
            if (Mistakes == 0)
                userStatistics.WinsWithoutMistakes = 1;
            userStatistics.BestTime = Preferences.Default.Get("SetCountTime", 0);

            return true;
        }
    }
}