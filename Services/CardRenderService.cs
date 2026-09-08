using Microsoft.Maui.Devices;

namespace munch_stamp.Services;

// Shared by LoyaltyCardDetailPage (share) and BusinessProfilePage (preview).
// Slides the render target up from below the screen, captures it once it's
// actually on-screen (translating it far off-screen instead causes some
// platforms to skip rendering it entirely, producing a blank capture),
// then slides it back down before cleanup.
public static class CardRenderService
{
    public static async Task<byte[]?> RenderToPngAsync(ContentView renderTarget, Controls.LoyaltyCardView cardView)
    {
        renderTarget.Content = cardView;
        renderTarget.IsVisible = true;

        double screenHeight = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;
        renderTarget.TranslationY = screenHeight;

        try
        {
            renderTarget.ForceLayout();
            await Task.Yield();

            await renderTarget.TranslateTo(0, 0, 350, Easing.CubicOut);
            await Task.Delay(150); // let the final frame settle before capturing

            var screenshotResult = await renderTarget.CaptureAsync();
            if (screenshotResult is null) return null;

            using var stream = await screenshotResult.OpenReadAsync();
            if (stream is null) return null;

            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            var bytes = memoryStream.ToArray();

            await renderTarget.TranslateTo(0, screenHeight, 250, Easing.CubicIn);

            return bytes;
        }
        finally
        {
            renderTarget.Content = null;
            renderTarget.IsVisible = false;
            renderTarget.TranslationY = 0;
        }
    }
}