using RaceBike.ViewModel;

namespace RaceBike.View;

public partial class GamePage : ContentPage
{
    private readonly RaceBikeViewModel _vm;

    public GamePage(RaceBikeViewModel viewModel)
	{
		InitializeComponent();

        _vm = viewModel;
        BindingContext = viewModel;

        Appearing += (s, e) =>
        {
            base.OnAppearing();
            _vm?.ResumeGame();
        };

        Disappearing += (s, e) =>
        {
            base.OnDisappearing();
            _vm?.PauseGame();
        };
    }
}