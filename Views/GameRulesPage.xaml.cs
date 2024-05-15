using Microsoft.Maui.Layouts;
using SudokuApp.Services;
using SudokuApp.ViewModels;


namespace SudokuApp.Views;

public partial class GameRulesPage : ContentPage
{
    private Frame secondFrame { get; set; }
    private Frame thirdFrame { get; set; }
    private Frame fourthFrame { get; set; }
    private Frame fifthFrame {  get; set; }
    private Frame sixthFrame { get; set; }
    private Frame finalFrame { get; set; }
    private Button finalButton {  get; set; }

    private readonly Color Primary = Color.FromArgb("#512BD4");
    private readonly Color MyPrimary = Color.FromArgb("#2196F3");

    private TranslationManager translate { get; set; }
    private string language;

    public GameRulesPage()
	{
        InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        MainThread.InvokeOnMainThreadAsync(GenerateSudokuGrid);

        language = Preferences.Default.Get("CurrentLanguage", "polski");
        translate = new();
        discardButton.Text = translate.Translate(language, "BtnDiscard");
        lblTouchCell.Text = translate.Translate(language, "LabelTouchCell");
    }

    private async void GenerateSudokuGrid()
    {
        var tcs = new TaskCompletionSource<bool>();
        await MainThread.InvokeOnMainThreadAsync(() =>
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

                            //pole tekstowe (TextBox) na podstawie wartoœci
                            Label label;
                            label = new Label
                            {
                                // Przypisz wspó³rzêdne do BindingContext
                                BindingContext = new GridCoordinates
                                {
                                    ColumnSudokuBoardXaml = col,
                                    RowSudokuBoardXaml = row,
                                    ColumnBoardSudoku = sudokuBoardCol,
                                    RowBoardSudoku = sudokuBoardRow
                                },
                                Text = "",
                                Style = (Style)Application.Current.Resources["LabelGrid"]
                            };
                            frame.Content = label;
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

        _ = Dispatcher.DispatchAsync(() =>
        {
            foreach (View child in sudokuBoardXaml.Children.Cast<View>())
            {
                if (child is Frame frame)
                {
                    if (frame.Content is Label label)
                    {
                        if (label.BindingContext is GridCoordinates coordinates)
                        {
                            if (coordinates.RowSudokuBoardXaml == 5 & coordinates.ColumnSudokuBoardXaml == 5)
                            {
                                label.Text = "9";
                            }
                            else if (coordinates.RowSudokuBoardXaml == 5 & coordinates.ColumnSudokuBoardXaml == 6)
                            {
                                label.Text = "7";
                            }
                            else if (coordinates.RowSudokuBoardXaml == 5 & coordinates.ColumnSudokuBoardXaml == 7)
                            {
                                label.Text = "5";
                            }
                            if (coordinates.RowSudokuBoardXaml == 6 & coordinates.ColumnSudokuBoardXaml == 5)
                            {
                                label.Text = "6";
                            }
                            else if (coordinates.RowSudokuBoardXaml == 6 & coordinates.ColumnSudokuBoardXaml == 6)
                            {
                                label.Text = "";
                                label.ClassId = "1";
                                label.BackgroundColor = Color.FromArgb("#FFA500");
                                TapGestureRecognizer tapGestureRecognizer = new();
                                tapGestureRecognizer.Tapped += (sender, e) =>
                                {
                                    OnLabelTapped(sender as Label);
                                };
                                label.GestureRecognizers.Add(tapGestureRecognizer);
                            }
                            else if (coordinates.RowSudokuBoardXaml == 6 & coordinates.ColumnSudokuBoardXaml == 7)
                            {
                                label.Text = "4";
                            }
                            if (coordinates.RowSudokuBoardXaml == 7 & coordinates.ColumnSudokuBoardXaml == 5)
                            {
                                label.Text = "3";
                            }
                            else if (coordinates.RowSudokuBoardXaml == 7 & coordinates.ColumnSudokuBoardXaml == 6)
                            {
                                label.Text = "2";
                            }
                            else if (coordinates.RowSudokuBoardXaml == 7 & coordinates.ColumnSudokuBoardXaml == 7)
                            {
                                label.Text = "1";
                                break;
                            }
                        }
                    }
                }
            }
        }); 
    }

    private Frame AddFrame()
    {
        return new Frame
        {
            CornerRadius = 10,
            Padding = new Thickness(0),
            IsVisible = true,
            BackgroundColor = Preferences.Default.Get("UserAppTheme", "") == "Light" ? Primary : MyPrimary
        };
    }

    private void AddSecondFrame()
    {
        secondFrame = AddFrame();
        var label = new Label
        {
            Text = translate.Translate(language, "LabelChooseNumber"),
            TextColor = Color.FromArgb("#FFFFFF")
        };

        secondFrame.Content = label;
        absoluteLayout.Children.Add(secondFrame);
        absoluteLayout.SetLayoutBounds(secondFrame, new Rect(0.95, 0.8, 0.3, 0.05));
        absoluteLayout.SetLayoutFlags(secondFrame, AbsoluteLayoutFlags.All);
    }

    private void AddThirdFrame()
    {
        thirdFrame = AddFrame();
        var label = new Label
        {
            Text = language switch
            {
                "deutsch" => translate.Translate(language, "LabelAddNumber"),
                _ => translate.Translate(language, "LabelAddNumber") + " 2",
            },
            TextColor = Color.FromArgb("#FFFFFF")
        };

        thirdFrame.Content = label;
        absoluteLayout.Children.Add(thirdFrame);
        absoluteLayout.SetLayoutBounds(thirdFrame, new Rect(1, 0.35, 0.4, 0.08));
        absoluteLayout.SetLayoutFlags(thirdFrame, AbsoluteLayoutFlags.All);
    }

    private void AddFourthFrame()
    {
        fourthFrame = AddFrame();
        var label = new Label
        {
            Text = translate.Translate(language, "LabelChooseNumber"),
            TextColor = Color.FromArgb("#FFFFFF")
        };

        fourthFrame.Content = label;
        absoluteLayout.Children.Add(fourthFrame);
        absoluteLayout.SetLayoutBounds(fourthFrame, new Rect(0, 0.815, 0.3, 0.05));
        absoluteLayout.SetLayoutFlags(fourthFrame, AbsoluteLayoutFlags.All);
    }

    private void AddFifthFrame()
    {
        fifthFrame = AddFrame();
        var label = new Label
        {
            Text =  language switch
            {
                "deutsch" => translate.Translate(language, "LabelAddNumber").Replace("2", "6"),
                _ => translate.Translate(language, "LabelAddNumber") + " 6",
            },
            TextColor = Color.FromArgb("#FFFFFF")
        };

        fifthFrame.Content = label;
        absoluteLayout.Children.Add(fifthFrame);
        absoluteLayout.SetLayoutBounds(fifthFrame, new Rect(0.55, 0.62, 0.5, 0.07));
        absoluteLayout.SetLayoutFlags(fifthFrame, AbsoluteLayoutFlags.All);
    }

    private void AddSixthFrame()
    {
        sixthFrame = AddFrame();
        var label = new Label
        {
            Text = translate.Translate(language, "LabelChooseNumber"),
            TextColor = Color.FromArgb("#FFFFFF")
        };

        sixthFrame.Content = label;
        absoluteLayout.Children.Add(sixthFrame);
        absoluteLayout.SetLayoutBounds(sixthFrame, new Rect(0.65, 0.815, 0.3, 0.05));
        absoluteLayout.SetLayoutFlags(sixthFrame, AbsoluteLayoutFlags.All);
    }

    private void AddFinalFrame()
    {
        finalFrame = AddFrame();
        var label = new Label
        {
            Text = translate.Translate(language, "LabelFinal"),
            TextColor = Color.FromArgb("#FFFFFF")
        };

        finalFrame.Content = label;
        absoluteLayout.Children.Add(finalFrame);
        absoluteLayout.SetLayoutBounds(finalFrame, new Rect(.65, 0.815, 1, 0.12));
        absoluteLayout.SetLayoutFlags(finalFrame, AbsoluteLayoutFlags.All);
    }

    private void AddFinalButton()
    {
        finalButton = new Button
        {
            Text = translate.Translate(language, "BtnFinal"),
            IsVisible = true
        };
        finalButton.Clicked += Back_Clicked;
        absoluteLayout.Children.Add(finalButton);
        AbsoluteLayout.SetLayoutBounds(finalButton, new Rect(0.5, 0.91, 0.65, 0.06));
        AbsoluteLayout.SetLayoutFlags(finalButton, AbsoluteLayoutFlags.All);
    }

    private void OnLabelTapped(Label label)
    {
        switch (label.ClassId) 
        {
            case "1" :
            {
                firstFrame.IsVisible = false;
                label.IsEnabled = false;
                foreach (View child in gridButtons.Children.Cast<View>())
                {
                    if (child is Button button)
                    {
                        if (button.ClassId == "8")
                        {
                            AddSecondFrame();
                            button.BackgroundColor = Color.FromArgb("#FFA500");
                            btnEight.IsEnabled = true;
                            break;
                        }
                    }
                }
                break;
            }
            case "2" :
            {
                foreach (View child in gridButtons.Children.Cast<View>())
                {
                    if (child is Button button)
                    {
                        if (button.ClassId == "2")
                        {
                            label.IsEnabled = false;
                            button.BackgroundColor = Color.FromArgb("#FFA500");
                            btnTwo.IsEnabled = true;
                            break;
                        }
                    }
                }
                thirdFrame.IsVisible = false;
                AddFourthFrame();
                break;
            }
            case "3":
            {
                foreach (View child in gridButtons.Children.Cast<View>())
                {
                    if (child is Button button)
                    {
                        if (button.ClassId == "6")
                        {
                            label.IsEnabled = false;
                            button.BackgroundColor = Color.FromArgb("#FFA500");
                            btnSix.IsEnabled = true;
                            break;
                        }
                    }
                }
                fifthFrame.IsVisible = false;
                AddSixthFrame();
                break;
            }
        } 
    }

    private async void Back_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync(true);   
    }

    private void Button8_Clicked(object sender, EventArgs e)
    {
        foreach (View child in sudokuBoardXaml.Children.Cast<View>())
        {
            if (child is Frame frame)
            {
                if (frame.Content is Label label)
                {
                    if (label.BindingContext is GridCoordinates coordinates)
                    {
                        if (coordinates.RowSudokuBoardXaml == 6 & coordinates.ColumnSudokuBoardXaml == 6)
                        {
                            label.Text = "8";
                        }
                        if (coordinates.RowSudokuBoardXaml == 6 & coordinates.ColumnSudokuBoardXaml == 1)
                        {
                            label.Text = "5";
                        }
                        else if (coordinates.RowSudokuBoardXaml == 6 & coordinates.ColumnSudokuBoardXaml == 2)
                        {
                            label.Text = "9";
                        }
                        else if (coordinates.RowSudokuBoardXaml == 6 & coordinates.ColumnSudokuBoardXaml == 3)
                        {
                            label.Text = "3";
                        }
                        else if (coordinates.RowSudokuBoardXaml == 6 & coordinates.ColumnSudokuBoardXaml == 9)
                        {
                            label.Text = "1";
                        }
                        else if(coordinates.RowSudokuBoardXaml == 6 & coordinates.ColumnSudokuBoardXaml == 10)
                        {
                            label.ClassId = "2";
                            label.BackgroundColor = Color.FromArgb("#FFA500");
                            TapGestureRecognizer tapGestureRecognizer = new();
                            tapGestureRecognizer.Tapped += (sender, e) =>
                            {
                                OnLabelTapped(label);
                            };
                            label.GestureRecognizers.Add(tapGestureRecognizer);
                        }
                        else if (coordinates.RowSudokuBoardXaml == 6 & coordinates.ColumnSudokuBoardXaml == 11)
                        {
                            label.Text = "7";
                            break;
                        }
                    }
                }
            }
        }
        btnEight.IsEnabled = false;
        secondFrame.IsVisible = false;
        AddThirdFrame();
    }

    private void Button2_Clicked(object sender, EventArgs e)
    {
        foreach (View child in sudokuBoardXaml.Children.Cast<View>())
        {
            if (child is Frame frame)
            {
                if (frame.Content is Label label)
                {
                    if (label.BindingContext is GridCoordinates coordinates)
                    {
                        if (coordinates.RowSudokuBoardXaml == 1 & coordinates.ColumnSudokuBoardXaml == 10)
                        {
                            label.Text = "5";
                        }
                        else if (coordinates.RowSudokuBoardXaml == 2 & coordinates.ColumnSudokuBoardXaml == 10)
                        {
                            label.Text = "9";
                        }
                        else if (coordinates.RowSudokuBoardXaml == 3 & coordinates.ColumnSudokuBoardXaml == 10)
                        {
                            label.Text = "3";
                        }
                        else if (coordinates.RowSudokuBoardXaml == 5 & coordinates.ColumnSudokuBoardXaml == 10)
                        {
                            label.Text = "1";
                        }
                        else if (coordinates.RowSudokuBoardXaml == 6 & coordinates.ColumnSudokuBoardXaml == 10)
                        {
                            label.Text = "2";
                        }
                        else if (coordinates.RowSudokuBoardXaml == 7 & coordinates.ColumnSudokuBoardXaml == 10)
                        {
                            label.Text = "4";
                        }
                        else if (coordinates.RowSudokuBoardXaml == 9 & coordinates.ColumnSudokuBoardXaml == 10)
                        {
                            label.Text = "8";
                        }
                        else if (coordinates.RowSudokuBoardXaml == 10 & coordinates.ColumnSudokuBoardXaml == 10)
                        {
                            label.ClassId = "3";
                            label.BackgroundColor = Color.FromArgb("#FFA500");
                            TapGestureRecognizer tapGestureRecognizer = new();
                            tapGestureRecognizer.Tapped += (sender, e) =>
                            {
                                OnLabelTapped(label);
                            };
                            label.GestureRecognizers.Add(tapGestureRecognizer);
                        }
                        else if (coordinates.RowSudokuBoardXaml == 11 & coordinates.ColumnSudokuBoardXaml == 10)
                        {
                            label.Text = "7";
                            break;
                        }
                        
                    }
                }
            }
        }
        btnTwo.IsEnabled = false;
        fourthFrame.IsVisible = false;
        AddFifthFrame();
    }

    private void Button6_Clicked(object sender, EventArgs e)
    {
        foreach (View child in sudokuBoardXaml.Children.Cast<View>())
        {
            if (child is Frame frame)
            {
                if (frame.Content is Label label)
                {
                    if (label.BindingContext is GridCoordinates coordinates)
                    {
                        if (coordinates.RowSudokuBoardXaml == 10 & coordinates.ColumnSudokuBoardXaml == 10)
                        {
                            label.Text = "6";
                            break;
                        }
                    }
                }
            }
        }
        sixthFrame.IsVisible = false;
        gridButtons.IsVisible = false;
        AddFinalFrame();
        AddFinalButton();
    }

}
