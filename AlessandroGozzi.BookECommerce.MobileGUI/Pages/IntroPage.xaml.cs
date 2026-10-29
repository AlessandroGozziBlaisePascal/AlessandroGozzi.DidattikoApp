namespace AlessandroGozzi.BookECommerce.MobileGUI.Pages;

public partial class IntroPage : ContentPage
{
    public IntroPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(100);

        double width = RootLayout.Width > 0 ? RootLayout.Width : 400;
        double height = RootLayout.Height > 0 ? RootLayout.Height : 800;

        // Coordinate iniziali fuori schermo
        WaveSplash.TranslationX = width * 1.4;
        WaveSplash.TranslationY = height * 0.4;
        WaveSplash.Rotation = 25;

        Drop1.TranslationX = width * 1.1;
        Drop2.TranslationX = width * 1.2;
        Drop1.Opacity = 0.85;
        Drop2.Opacity = 0.75;

        // --- FASE 1: Entrata dinamica gocce e onda a gradiente ---
        var moveDrops = Task.WhenAll(
            Drop1.TranslateTo(-width * 0.2, -80, 500, Easing.CubicOut),
            Drop2.TranslateTo(-width * 0.4, 60, 550, Easing.CubicOut)
        );

        var moveWave = Task.WhenAll(
            WaveSplash.TranslateTo(0, 0, 600, Easing.CubicOut),
            WaveSplash.RotateTo(0, 600, Easing.CubicOut)
        );

        await Task.WhenAll(moveDrops, moveWave);

        // L'onda espande il gradiente viola-blu coprendo l'intero display
        await Task.WhenAll(
            WaveSplash.ScaleTo(2.8, 350, Easing.SinIn),
            Drop1.FadeTo(0, 200),
            Drop2.FadeTo(0, 200)
        );

        // --- FASE 2: Ingresso del Logo in formato grande ---
        LogoImg.Opacity = 1.0;
        // Effetto elastico con atterraggio a Scale = 1.3 per renderlo ben visibile
        await LogoImg.ScaleTo(1.45, 450, Easing.SpringOut);
        await LogoImg.ScaleTo(1.30, 180, Easing.Linear);

        // --- FASE 3: Pausa statica di visibilità ---
        // 900ms a logo grande e perfettamente fermo
        await Task.Delay(900);

        // --- FASE 4: Mega Zoom Out e transizione al Login ---
        var zoomOut = LogoImg.ScaleTo(22.0, 500, Easing.CubicIn);
        var fadeOut = LogoImg.FadeTo(0, 400, Easing.CubicIn);

        await Task.WhenAll(zoomOut, fadeOut);

        // Apertura schermata Login
        await Shell.Current.GoToAsync("//LoginPage");
    }
}